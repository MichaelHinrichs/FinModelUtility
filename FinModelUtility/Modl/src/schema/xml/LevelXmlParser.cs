using fin.data;
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
  public class LevelObject {
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

  public class LevelXmlParser {
    public IScene Parse(IFile mainXmlFile,
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
                             IFile outFile,
                             GameVersion gameVersion,
                             out IBwTerrain bwTerrain) {
      sceneArea.AddObject()
               .AddSceneModel(
                   new OutModelLoader().LoadModel(
                       outFile, gameVersion, out bwTerrain));
    }

    private void AddObjects_(ISceneArea sceneArea,
                             IFile levelXmlFile,
                             GameVersion gameVersion,
                             IBwTerrain bwTerrain) {
      var levelDirectory =
          new FinDirectory(levelXmlFile.FullNameWithoutExtension);
      var modelFiles = levelDirectory
                       .GetExistingFiles()
                       .Where(file => file.Name.EndsWith(".modl"))
                       .ToArray();

      var modlLoader = new ModlModelLoader();

      var objectMap = this.ReadObjectMap_(levelXmlFile, gameVersion);
      var lazyModelMap = new LazyDictionary<string, IModel>(modelId => {
        var modelFile =
            modelFiles.Single(file => file.NameWithoutExtension == modelId);
        var model = modlLoader.LoadModel(modelFile, null, gameVersion);
        return model;
      });

      foreach (var obj in objectMap.Values) {
        var matrix = obj.Matrix;
        if (matrix == null) {
          continue;
        }

        var sceneObject = sceneArea.AddObject();

        matrix.CopyTranslationInto(sceneObject.Position);
        if (sceneObject.Position.Y == 0) {
          sceneObject.Position.Y =
              bwTerrain.Heightmap.GetHeightAtPosition(
                  sceneObject.Position.X, sceneObject.Position.Z);
        }


        matrix.CopyRotationInto(out var rotation);
        var eulerRadians = QuaternionUtil.ToEulerRadians(rotation);
        sceneObject.Rotation.SetRadians(
            eulerRadians.X, eulerRadians.Y, eulerRadians.Z);
        
        matrix.CopyScaleInto(sceneObject.Scale);

        var childIdQueue = new FinQueue<string>(obj.Children);
        while (childIdQueue.TryDequeue(out var childId)) {
          objectMap.TryGetValue(childId, out var child);
          if (child == null) {
            continue;
          }

          if (child.ModelName != null) {
            sceneObject.AddSceneModel(lazyModelMap[child.ModelName]);
          }

          childIdQueue.Enqueue(child.Children);
        }
      }
    }

    private IDictionary<string, LevelObject> ReadObjectMap_(
        IFile levelXmlFile,
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

      var objectsById = new Dictionary<string, LevelObject>();
      var types = new HashSet<string>();

      var objectTags = instances.GetElementsByTagName("Object");
      for (var i = 0; i < objectTags.Count; ++i) {
        var objectTag = objectTags[i];

        LevelObject? node = null;

        var objectType = objectTag.Attributes["type"].Value;
        types.Add(objectType);

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
                node.Matrix = new FinMatrix4x4(floats).TransposeInPlace();
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
}