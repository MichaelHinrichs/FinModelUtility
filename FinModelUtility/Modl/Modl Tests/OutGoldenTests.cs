using System.Reflection;

using fin.io;
using fin.testing;
using fin.util.enumerables;

using modl.api;


namespace modl {
  public class OutGoldenTests {
    [Test]
    public void TestExportsExactModel() {
      ModelGoldenAssert.AssertExportGoldens(
          this.GetBattalionWars1OutGoldensDirectory_(),
          new OutModelLoader(),
          this.GetBattalionWars1ModelFileBundleInDirectory_);
    }

    private ISystemDirectory GetBattalionWars1OutGoldensDirectory_()
      => ModelGoldenAssert
         .GetRootGoldensDirectory(Assembly.GetExecutingAssembly())
         .GetSubdir("out/battalion_wars_1");

    private OutModelFileBundle GetBattalionWars1ModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory)
      => new() {
          GameName = "foobar",
          GameVersion = GameVersion.BW1,
          OutFile = directory.FilesWithExtension(".out").Single(),
          TextureDirectories = directory.Yield(),
      };
  }
}