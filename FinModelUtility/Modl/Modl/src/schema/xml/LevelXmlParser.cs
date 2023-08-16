using System.Collections.Concurrent;
using System.Drawing;
using System.IO.Compression;
using System.Xml;

using fin.color;
using fin.data.queue;
using fin.io;
using fin.math;
using fin.math.matrix;
using fin.math.rotations;
using fin.model;
using fin.model.impl;
using fin.scene;
using fin.schema.vector;

using modl.api;
using modl.schema.terrain;

namespace modl.schema.xml {
  public interface IBwObject {
    string Id { get; }
    string? ModelName { get; set; }
  }

  public class LevelObject : IBwObject {
    public required string Id { get; init; }
    public string? ModelName { get; set; }

    public IReadOnlyFinMatrix4x4? Matrix { get; set; }

    public LinkedList<string> Children { get; } = new();

    public string? NextLinkId { get; set; } = null;
    public bool? StickToFloor { get; set; } = null;

    public void AddChild(string child) {
      if (child == "0") {
        return;
      }

      this.Children.AddLast(child);
    }
  }

  public class SkydomeObject : IBwObject {
    public required string Id { get; init; }
    public string? ModelName { get; set; }
  }

  public class LevelXmlParser {
    public IScene Parse(ISystemFile mainXmlFile,
                        GameVersion gameVersion) {
      var scene = new SceneImpl();
      var sceneArea = scene.AddArea();

      var mainXml = new XmlDocument();
      mainXml.LoadXml(mainXmlFile.ReadAllText());

      var mainXmlDirectory = mainXmlFile.GetParent();

      var levelfilesTag = mainXml["levelfiles"];

      var levelFilename =
          levelfilesTag["level"]["objectfiles"]["file"].GetAttribute("name");
      var levelXmlFile = mainXmlDirectory.GetExistingFile(levelFilename);
      ILighting? lighting = null;
      var objectMap =
          this.ReadObjectMap_(levelXmlFile,
                              gameVersion,
                              ref lighting,
                              out var terrainLightScale);

      IBwTerrain bwTerrain;
      {
        var terrainFilename =
            levelfilesTag["terrain"]["file"].GetAttribute("name");
        var outFile = mainXmlDirectory.GetExistingFile(terrainFilename);
        this.AddTerrain_(sceneArea,
                         outFile,
                         gameVersion,
                         out bwTerrain,
                         terrainLightScale);
      }

      {
        AddObjects_(sceneArea,
                    levelXmlFile,
                    gameVersion,
                    bwTerrain,
                    objectMap,
                    lighting);
      }

      return scene;
    }

    private void AddTerrain_(ISceneArea sceneArea,
                             ISystemFile outFile,
                             GameVersion gameVersion,
                             out IBwTerrain bwTerrain,
                             float terrainLightScale) {
      sceneArea.AddObject()
               .AddSceneModel(
                   new OutModelLoader().LoadModel(
                       outFile,
                       gameVersion,
                       out bwTerrain,
                       terrainLightScale));
    }

    private void AddObjects_(ISceneArea sceneArea,
                             ISystemFile levelXmlFile,
                             GameVersion gameVersion,
                             IBwTerrain bwTerrain,
                             IDictionary<string, IBwObject> objectMap,
                             ILighting? lighting) {
      var levelDirectory =
          new FinDirectory(levelXmlFile.FullNameWithoutExtension);
      var modelFiles = levelDirectory
                       .GetExistingFiles()
                       .Where(file => file.Name.EndsWith(".modl"))
                       .ToArray();
      var animationFiles = levelDirectory
                           .GetExistingFiles()
                           .Where(file => file.Name.EndsWith(".anim"))
                           .ToArray();

      var fvAnimFiles =
          animationFiles.Where(
                            animFile =>
                                animFile.NameWithoutExtension.StartsWith("FV"))
                        .ToArray();
      var fgAnimFiles =
          animationFiles.Where(
                            animFile =>
                                animFile.NameWithoutExtension.StartsWith("FG"))
                        .ToArray();

      var modlLoader = new ModlModelLoader();

      var modelMap = new ConcurrentDictionary<string, IModel>();
      var task = Parallel.ForEachAsync(
          modelFiles,
          async (modelFile, _) => {
            var modelId = modelFile.NameWithoutExtension;

            IList<ISystemFile>? animFiles = null;
            if (gameVersion == GameVersion.BW1) {
              if (modelId.Length == 4 && modelId.EndsWith("VET")) {
                var firstTwoCharactersInModelId = modelId.Substring(0, 2);
                animFiles = fvAnimFiles
                            .Concat(animationFiles.Where(
                                        file => file.Name.StartsWith(
                                            firstTwoCharactersInModelId)))
                            .ToArray();
              } else if (modelId.Length == 6 && modelId.EndsWith("GRUNT")) {
                var firstTwoCharactersInModelId = modelId.Substring(0, 2);
                animFiles = fgAnimFiles
                            .Concat(animationFiles.Where(
                                        file => file.Name.StartsWith(
                                            firstTwoCharactersInModelId)))
                            .ToArray();
              }
            }

            modelMap[modelId] =
                await modlLoader.LoadModelAsync(
                    modelFile,
                    animFiles,
                    gameVersion,
                    lighting);
          });
      task.ConfigureAwait(false);
      task.Wait();

      var levelObjMap = new Dictionary<string, ISceneObject>();

      foreach (var obj in objectMap.Values) {
        switch (obj) {
          case SkydomeObject skyboxObj: {
            if (skyboxObj.ModelName != null) {
              sceneArea.BackgroundColor = Color.Black;

              var skydomeModel = modelMap[skyboxObj.ModelName];
              foreach (var finMaterial in skydomeModel.MaterialManager.All) {
                finMaterial.DepthMode = DepthMode.IGNORE_DEPTH_BUFFER;
                finMaterial.DepthCompareType = DepthCompareType.Always;
              }

              var skydomeObject = sceneArea.CreateCustomSkyboxObject();
              skydomeObject.AddSceneModel(skydomeModel);
              skydomeObject.Rotation.SetDegrees(90, 0, 0);
            }

            break;
          }
          case LevelObject levelObj: {
            var rootMatrix = levelObj.Matrix;
            if (rootMatrix == null) {
              continue;
            }

            var childIdQueue =
                new FinTuple3Queue<string, (bool?, string?),
                    IReadOnlyFinMatrix4x4>(
                    levelObj.Children.Select(
                        child => (
                            child, (levelObj.StickToFloor, levelObj.NextLinkId),
                            rootMatrix)));
            while (childIdQueue.TryDequeue(out var childId,
                                           out var stickToFloorAndNextLinkId,
                                           out var parentMatrix)) {
              objectMap.TryGetValue(childId, out var genericChild);
              var child = genericChild as LevelObject;
              if (child == null) {
                continue;
              }

              var (stickToFloor, nextLinkId) = stickToFloorAndNextLinkId;
              stickToFloor ??= child.StickToFloor;
              nextLinkId ??= child.NextLinkId;

              IReadOnlyFinMatrix4x4 childMatrix;
              if (child.Matrix == null) {
                childMatrix = parentMatrix;
              } else {
                childMatrix = parentMatrix.CloneAndMultiply(child.Matrix);
              }

              if (child.ModelName != null) {
                var sceneObject = sceneArea.AddObject();

                childMatrix.Decompose(out var translation,
                                      out var rotation,
                                      out var scale);

                if (stickToFloor != false && nextLinkId != null) {
                  // Should be an invalid case...?
                }

                if (stickToFloor != false) {
                  sceneObject.SetPosition(
                      translation.X,
                      translation.Y + bwTerrain.Heightmap
                                               .GetHeightAtPosition(
                                                   translation.X,
                                                   translation.Z),
                      translation.Z);
                } else if (nextLinkId != null) {
                  sceneObject.SetPosition(
                      translation.X,
                      levelObjMap[nextLinkId].Position.Y,
                      translation.Z);
                } else {
                  sceneObject.SetPosition(
                      translation.X,
                      translation.Y,
                      translation.Z);
                }

                levelObjMap[levelObj.Id] = sceneObject;

                if (nextLinkId != null) {
                  var nextLinkObj = levelObjMap[nextLinkId];

                  var nextLinkRotation = nextLinkObj.Rotation;
                  sceneObject.Rotation.SetDegrees(
                      nextLinkRotation.XDegrees,
                      nextLinkRotation.YDegrees,
                      nextLinkRotation.ZDegrees);

                  var nextLinkScale = nextLinkObj.Scale;
                  sceneObject.SetScale(nextLinkScale.X,
                                       nextLinkScale.Y,
                                       nextLinkScale.Z);
                } else {
                  sceneObject.Rotation.SetQuaternion(rotation);
                  sceneObject.SetScale(scale.X, scale.Y, scale.Z);
                }

                sceneObject.AddSceneModel(modelMap[child.ModelName]);
              }

              childIdQueue.Enqueue(
                  child.Children.Select(grandchild
                                            => (grandchild,
                                                (stickToFloor, nextLinkId),
                                                childMatrix)));
            }

            break;
          }
        }
      }
    }

    private IDictionary<string, IBwObject> ReadObjectMap_(
        ISystemFile levelXmlFile,
        GameVersion gameVersion,
        ref ILighting? lighting,
        out float terrainLightScale) {
      Stream levelXmlStream;
      if (gameVersion == GameVersion.BW2) {
        using var gZipStream =
            new GZipStream(levelXmlFile.OpenRead(),
                           CompressionMode.Decompress);

        levelXmlStream = new MemoryStream();
        gZipStream.CopyTo(levelXmlStream);
        levelXmlStream.Position = 0;
      } else {
        levelXmlStream = levelXmlFile.OpenRead();
      }

      using var levelXmlReader = new StreamReader(levelXmlStream);
      var levelXml = new XmlDocument();
      levelXml.LoadXml(levelXmlReader.ReadToEnd());

      var instances = levelXml["Instances"];

      var objectsById = new Dictionary<string, IBwObject>();
      var types = new HashSet<string>();

      terrainLightScale = 1;
      var objectTags = instances.GetElementsByTagName("Object");
      for (var i = 0; i < objectTags.Count; ++i) {
        var objectTag = objectTags[i];

        LevelObject? node = null;

        var objectType = objectTag.Attributes["type"].Value;
        types.Add(objectType);

        if (objectType == "cRenderParams") {
          // TODO: Handle fog color

          terrainLightScale =
              float.Parse(objectTag.GetAttributeValue("mTerrainLightScale"));

          lighting = new LightingImpl();
          lighting.AmbientLightColor =
              objectTag.GetAttributeLightColor("mSunAmbientColor");

          {
            var sunLight = lighting.CreateLight();
            sunLight.SetColor(
                objectTag.GetAttributeLightColor("mSunDirectionalColor"));

            var sunYawRadians =
                float.Parse(objectTag.GetAttributeValue("mSunRotation"));
            var sunPitchRadians =
                MathF.Asin(
                    float.Parse(objectTag.GetAttributeValue("mSunElevation")));
            FinTrig.FromPitchYawRadians(sunPitchRadians,
                                        sunYawRadians,
                                        out var sunXNormal,
                                        out var sunYNormal,
                                        out var sunZNormal);
            sunLight.SetNormal(new Vector3f {
                X = -sunXNormal, Y = -sunYNormal, Z = -sunZNormal,
            });
          }

          {
            var antiSunLight = lighting.CreateLight();
            antiSunLight.SetColor(
                objectTag.GetAttributeLightColor("mAntiSunDirectionalColor"));

            var antiSunYawRadians =
                float.Parse(objectTag.GetAttributeValue("mAntiSunRotation"));
            var antiSunPitchRadians =
                MathF.Asin(
                    float.Parse(
                        objectTag.GetAttributeValue("mAntiSunElevation")));
            FinTrig.FromPitchYawRadians(antiSunPitchRadians,
                                        antiSunYawRadians,
                                        out var antiSunXNormal,
                                        out var antiSunYNormal,
                                        out var antiSunZNormal);
            antiSunLight.SetNormal(new Vector3f {
                X = -antiSunXNormal, Y = -antiSunYNormal, Z = -antiSunZNormal,
            });
          }


          var skydomeId = objectTag.GetAttributeValue("mpWorldSkydome");
          if (objectsById.TryGetValue(skydomeId, out var skydomeModelObject)) {
            var skydomeModelName = skydomeModelObject.ModelName;
            objectsById[skydomeId] = new SkydomeObject {
                Id = skydomeId, ModelName = skydomeModelName,
            };
          }

          continue;
        }

        var isUsefulNode = false;

        var objId = objectTag.Attributes["id"].Value;
        foreach (var childTag in objectTag.Children()) {
          var childNameAttribute = childTag.Attributes["name"]?.Value;

          switch (childTag.Name) {
            case "Attribute": {
              if (childNameAttribute is "mMatrix" or "Mat") {
                var floats = new float[16];
                var floatsText = childTag["Item"].InnerText;

                var currentIndex = 0;
                for (var fI = 0; fI < floats.Length; ++fI) {
                  var nextCommaIndex = floatsText.IndexOf(',', currentIndex);

                  var subText =
                      nextCommaIndex > 0
                          ? floatsText.Substring(currentIndex,
                                                 nextCommaIndex - currentIndex)
                          : floatsText.Substring(currentIndex);
                  floats[fI] = float.Parse(subText);

                  currentIndex = nextCommaIndex + 1;
                }

                isUsefulNode = true;
                node ??= new LevelObject { Id = objId };
                node.Matrix = new FinMatrix4x4(floats);
              } else if (objectType is "cNodeHierarchyResource" &&
                         childNameAttribute is "mName") {
                isUsefulNode = true;
                node ??= new LevelObject { Id = objId };
                node.ModelName = childTag["Item"].InnerText;
              }

              break;
            }
            case "Enum": {
              if (childNameAttribute is "mStickToFloor") {
                node ??= new LevelObject { Id = objId };
                node.StickToFloor = childTag["Item"].InnerText == "eTrue";
              }

              break;
            }
            case "Pointer": {
              if (childNameAttribute is "mBase") {
                isUsefulNode = true;
                node ??= new LevelObject { Id = objId };
                node.AddChild(childTag["Item"].InnerText);
              }

              if (childNameAttribute is "NextLinkObject") {
                var nextLinkId = childTag["Item"].InnerText;
                if (nextLinkId != "0") {
                  node ??= new LevelObject { Id = objId };
                  node.NextLinkId = nextLinkId;
                }
              }

              break;
            }
            case "Resource": {
              if (childNameAttribute is "mModel"
                                        or "mBAN_Model"
                                        or "Model"
                                        or "model") {
                isUsefulNode = true;
                node ??= new LevelObject { Id = objId };
                node.AddChild(childTag["Item"].InnerText);
              } else if (childNameAttribute is "Element") {
                isUsefulNode = true;
                node ??= new LevelObject { Id = objId };
                var itemNodes = childTag.ChildNodes;
                for (var itemI = 0; itemI < itemNodes.Count; ++itemI) {
                  node.AddChild(itemNodes[itemI].InnerText);
                }
              }

              break;
            }
          }
        }

        if (isUsefulNode) {
          objectsById[objId] = node;
        }
      }

      var ids = objectsById.Keys.Select(ulong.Parse).ToList();
      ids.Sort();

      return objectsById;
    }
  }

  public static class XmlExtensions {
    public static IEnumerable<XmlNode> Children(
        this XmlNode xmlNode) => xmlNode.Cast<XmlNode>();

    public static IEnumerable<XmlNode> Children(
        this XmlNodeList xmlNodeList) => xmlNodeList.Cast<XmlNode>();

    public static string GetAttributeValue(this XmlNode xmlNode, string name)
      => xmlNode.Children()
                .Single(child => child.Attributes?["name"].Value == name)
                .FirstChild?.InnerText!;

    public static IColor GetAttributeColor(this XmlNode xmlNode, string name) {
      var bytes = GetAttributeValue(xmlNode, name)
                  .Split(',')
                  .Select(byte.Parse)
                  .ToArray();

      return FinColor.FromRgbaBytes(bytes[0], bytes[1], bytes[2], bytes[3]);
    }

    public static IColor GetAttributeLightColor(this XmlNode xmlNode,
                                                string name) {
      var rgbaColor = GetAttributeColor(xmlNode, name);
      return FinColor.FromRgbFloats(rgbaColor.Rf * rgbaColor.Af,
                                    rgbaColor.Gf * rgbaColor.Af,
                                    rgbaColor.Bf * rgbaColor.Af);
    }
  }
}