using fin.io;
using fin.scene;
using modl.api;
using System.IO.Compression;
using System.Xml;


namespace modl.schema.xml {
  public class LevelXmlParser {
    public IScene Parse(IFile mainXmlFile,
                        GameVersion gameVersion) {
      var scene = new SceneImpl();
      var sceneArea = scene.AddArea();

      var mainXml = new XmlDocument();
      mainXml.LoadXml(mainXmlFile.ReadAllText());

      var mainXmlDirectory = mainXmlFile.GetParent();

      var levelfilesTag = mainXml["levelfiles"];

      {
        var terrainFilename =
            levelfilesTag["terrain"]["file"].GetAttribute("name");
        var outFile = mainXmlDirectory.GetExistingFile(terrainFilename);
        this.AddTerrain_(sceneArea, outFile, gameVersion);
      }

      {
        var levelFilename =
            levelfilesTag["level"]["objectfiles"]["file"].GetAttribute("name");
        var levelXmlFile = mainXmlDirectory.GetExistingFile(levelFilename);
        AddObjects_(sceneArea, levelXmlFile, gameVersion);
      }

      return scene;
    }

    private void AddTerrain_(ISceneArea sceneArea,
                             IFile outFile,
                             GameVersion gameVersion) {
      sceneArea.AddObject()
               .AddSceneModel(
                   new OutModelLoader().LoadModel(outFile, gameVersion));
    }

    private void AddObjects_(ISceneArea sceneArea,
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

      var types = new HashSet<string>();

      var objectTags = instances.GetElementsByTagName("Object");
      for (var i = 0; i < objectTags.Count; ++i) {
        var objectTag = objectTags[i];

        var id = objectTag.Attributes["id"].Value;
        var type = objectTag.Attributes["type"].Value;

        types.Add(type);
      }
    }
  }
}