using fin.io;
using fin.scene;
using modl.schema.xml;


namespace modl.api {
  public class BwSceneFileBundle : IBattalionWarsFileBundle, ISceneFileBundle {
    public IFileHierarchyFile MainFile => this.MainXmlFile;

    public GameVersion GameVersion { get; set; }
    public IFileHierarchyFile MainXmlFile { get; set; }
  }

  public class BwSceneLoader : ISceneLoader<BwSceneFileBundle> {
    public IScene LoadScene(BwSceneFileBundle sceneFileBundle)
      => new LevelXmlParser().Parse(sceneFileBundle.MainXmlFile.Impl,
                                    sceneFileBundle.GameVersion);
  }
}