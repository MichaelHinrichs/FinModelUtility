using System.Collections.Concurrent;

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

using Microsoft.Toolkit.HighPerformance.Helpers;


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
      ParallelHelper.For(0,
                         modelFiles.Length,
                         new ModlLoader(modlLoader,
                                        modelFiles,
                                        modelMap,
                                        animationFiles,
                                        fvAnimFiles,
                                        fgAnimFiles,
                                        gameVersion));

      foreach (var obj in objectMap.Values) {
        var rootMatrix = obj.Matrix;
        if (rootMatrix == null) {
          continue;
        }

        var childIdQueue = new FinTuple2Queue<string, IReadOnlyFinMatrix4x4>(obj.Children.Select(child => (child, rootMatrix)));
        while (childIdQueue.TryDequeue(out var childId, out var parentMatrix)) {
          objectMap.TryGetValue(childId, out var child);
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

            childMatrix.Decompose(out var translation, out var rotation, out var scale);
            sceneObject.SetPosition(
              translation.X,
              translation.Y,
              translation.Z);
            if (sceneObject.Position.Y == 0) {
              sceneObject.SetPosition(sceneObject.Position.X, 
                bwTerrain.Heightmap.GetHeightAtPosition(
                  sceneObject.Position.X, sceneObject.Position.Z),
                sceneObject.Position.Z);
            }

            sceneObject.Rotation.SetQuaternion(rotation);
            sceneObject.SetScale(scale.X, scale.Y, scale.Z);

            sceneObject.AddSceneModel(modelMap[child.ModelName]);
          }

          childIdQueue.Enqueue(child.Children.Select(grandchild => (grandchild, childMatrix)));
        }
      }
    }

    private readonly struct ModlLoader : IAction {
      private readonly ModlModelLoader modlLoader_;
      private readonly IReadOnlyList<IFile> modelFiles_;
      private readonly IDictionary<string, IModel> modelMap_;
      private readonly IReadOnlyList<IFile> animationFiles_;
      private readonly IReadOnlyList<IFile> fvAnimFiles_;
      private readonly IReadOnlyList<IFile> fgAnimFiles_;
      private readonly GameVersion gameVersion_;

      public ModlLoader(
          ModlModelLoader modlLoader,
          IReadOnlyList<IFile> modelFiles,
          IDictionary<string, IModel> modelMap,
          IReadOnlyList<IFile> animationFiles,
          IReadOnlyList<IFile> fvAnimFiles,
          IReadOnlyList<IFile> fgAnimFiles,
          GameVersion gameVersion) {
        this.modlLoader_ = modlLoader;
        this.modelFiles_ = modelFiles;
        this.modelMap_ = modelMap;
        this.animationFiles_ = animationFiles;
        this.fvAnimFiles_ = fvAnimFiles;
        this.fgAnimFiles_ = fgAnimFiles;
        this.gameVersion_ = gameVersion;
      }

      public void Invoke(int index) {
        var modelFile = modelFiles_[index];
        var modelId = modelFile.NameWithoutExtension;

        IList<IFile>? animFiles = null;
        if (this.gameVersion_ == GameVersion.BW1) {
          if (modelId.Length == 4 && modelId.EndsWith("VET")) {
            var firstTwoCharactersInModelId = modelId.Substring(0, 2);
            animFiles = this.fvAnimFiles_.Concat(this.animationFiles_.Where(file => file.Name.StartsWith(firstTwoCharactersInModelId))).ToArray();
          } else if (modelId.Length == 6 && modelId.EndsWith("GRUNT")) {
            var firstTwoCharactersInModelId = modelId.Substring(0, 2);
            animFiles = this.fgAnimFiles_.Concat(this.animationFiles_.Where(file => file.Name.StartsWith(firstTwoCharactersInModelId))).ToArray();
          }
        }

        this.modelMap_[modelId] = this.modlLoader_.LoadModel(modelFile, animFiles, this.gameVersion_);
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
}