using System.Collections.Concurrent;
using System.Drawing;

using fin.data.queue;
using fin.io;
using fin.math;
using fin.math.matrix;
using fin.model;
using fin.scene;

using modl.api;
using modl.schema.terrain;

using System.IO.Compression;
using System.Xml;


namespace modl.schema.xml {
  public interface IBwObject {
    string? ModelName { get; set; }
  }

  public class LevelObject : IBwObject {
    public string? ModelName { get; set; }

    public IReadOnlyFinMatrix4x4? Matrix { get; set; }

    public LinkedList<string> Children { get; } = new();

    public void AddChild(string child) {
      if (child == "0") {
        return;
      }

      this.Children.AddLast(child);
    }
  }

  public class SkydomeObject : IBwObject {
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


      IBwTerrain bwTerrain;
      {
        var terrainFilename =
            levelfilesTag["terrain"]["file"].GetAttribute("name");
        var outFile = mainXmlDirectory.GetExistingFile(terrainFilename);
        this.AddTerrain_(sceneArea, outFile, gameVersion, out bwTerrain);
      }

      {
        var levelFilename =
            levelfilesTag["level"]["objectfiles"]["file"].GetAttribute("name");
        var levelXmlFile = mainXmlDirectory.GetExistingFile(levelFilename);
        AddObjects_(sceneArea, levelXmlFile, gameVersion, bwTerrain);
      }

      return scene;
    }

    private void AddTerrain_(ISceneArea sceneArea,
                             ISystemFile outFile,
                             GameVersion gameVersion,
                             out IBwTerrain bwTerrain) {
      sceneArea.AddObject()
               .AddSceneModel(
                   new OutModelLoader().LoadModel(
                       outFile,
                       gameVersion,
                       out bwTerrain));
    }

    private void AddObjects_(ISceneArea sceneArea,
                             ISystemFile levelXmlFile,
                             GameVersion gameVersion,
                             IBwTerrain bwTerrain) {
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

      var objectMap = this.ReadObjectMap_(levelXmlFile, gameVersion);

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
                    gameVersion);
          });
      task.ConfigureAwait(false);
      task.Wait();

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
                new FinTuple2Queue<string, IReadOnlyFinMatrix4x4>(
                    levelObj.Children.Select(child => (child, rootMatrix)));
            while (childIdQueue.TryDequeue(out var childId,
                                           out var parentMatrix)) {
              objectMap.TryGetValue(childId, out var genericChild);
              var child = genericChild as LevelObject;
              if (child == null) {
                continue;
              }

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
                sceneObject.SetPosition(
                    translation.X,
                    translation.Y,
                    translation.Z);
                if (sceneObject.Position.Y == 0) {
                  sceneObject.SetPosition(sceneObject.Position.X,
                                          bwTerrain.Heightmap
                                                   .GetHeightAtPosition(
                                                       sceneObject.Position.X,
                                                       sceneObject.Position.Z),
                                          sceneObject.Position.Z);
                }

                sceneObject.Rotation.SetQuaternion(rotation);
                sceneObject.SetScale(scale.X, scale.Y, scale.Z);

                sceneObject.AddSceneModel(modelMap[child.ModelName]);
              }

              childIdQueue.Enqueue(
                  child.Children.Select(grandchild
                                            => (grandchild, childMatrix)));
            }

            break;
          }
        }
      }
    }

    private IDictionary<string, IBwObject> ReadObjectMap_(
        ISystemFile levelXmlFile,
        GameVersion gameVersion) {
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

      var objectTags = instances.GetElementsByTagName("Object");
      for (var i = 0; i < objectTags.Count; ++i) {
        var objectTag = objectTags[i];

        LevelObject? node = null;

        var objectType = objectTag.Attributes["type"].Value;
        types.Add(objectType);

        if (objectType == "cRenderParams") {
          // TODO: Handle fog color
          // TODO: Handle sky color
          // TODO: Handle sun

          var mpWorldSkydomeResource =
              objectTag
                  .Children()
                  .Single(child => child.Attributes?["name"].Value ==
                                  "mpWorldSkydome");
          var skydomeId = mpWorldSkydomeResource.FirstChild?.InnerText!;
          if (objectsById.TryGetValue(skydomeId, out var skydomeModelObject)) {
            var skydomeModelName = skydomeModelObject.ModelName;
            objectsById[skydomeId] = new SkydomeObject {
                ModelName = skydomeModelName,
            };
          }

          continue;
        }

        var isUsefulNode = false;

        var childTags = objectTag.ChildNodes;
        for (var childIndex = 0; childIndex < childTags.Count; ++childIndex) {
          var childTag = childTags[childIndex];
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
                node ??= new LevelObject();
                node.Matrix = new FinMatrix4x4(floats);
              } else if (objectType is "cNodeHierarchyResource" &&
                         childNameAttribute is "mName") {
                isUsefulNode = true;
                node ??= new LevelObject();
                node.ModelName = childTag["Item"].InnerText;
              }

              break;
            }
            case "Pointer": {
              if (childNameAttribute is "mBase") {
                isUsefulNode = true;
                node ??= new LevelObject();
                node.AddChild(childTag["Item"].InnerText);
              }

              break;
            }
            case "Resource": {
              if (childNameAttribute is "mModel"
                                        or "mBAN_Model"
                                        or "Model"
                                        or "model") {
                isUsefulNode = true;
                node ??= new LevelObject();
                node.AddChild(childTag["Item"].InnerText);
              } else if (childNameAttribute is "Element") {
                isUsefulNode = true;
                node ??= new LevelObject();
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
          var id = objectTag.Attributes["id"].Value;
          objectsById[id] = node;
        }
      }

      return objectsById;
    }
  }

  public static class XmlExtensions {
    public static IEnumerable<XmlNode> Children(
        this XmlNode xmlNode) => xmlNode.Cast<XmlNode>();

    public static IEnumerable<XmlNode> Children(
        this XmlNodeList xmlNodeList) => xmlNodeList.Cast<XmlNode>();
  }
}