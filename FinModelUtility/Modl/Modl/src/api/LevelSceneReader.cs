using fin.io;
using fin.scene;

using modl.schema.xml;

namespace modl.api {
  public class BwSceneFileBundle : IBattalionWarsFileBundle, ISceneFileBundle {
    public required string GameName { get; init; }
    public IFileHierarchyFile MainFile => this.MainXmlFile;

    public required GameVersion GameVersion { get; init; }
    public required IFileHierarchyFile MainXmlFile { get; init; }
  }

  public class BwSceneReader : ISceneReader<BwSceneFileBundle> {
    public IScene ReadScene(BwSceneFileBundle sceneFileBundle)
      => new LevelXmlParser().Parse(sceneFileBundle.MainXmlFile.Impl,
                                    sceneFileBundle.GameVersion);
  }
}