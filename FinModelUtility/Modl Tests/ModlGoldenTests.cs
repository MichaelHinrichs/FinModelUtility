using System.Reflection;

using fin.io;
using fin.testing;

using modl.api;


namespace modl {
  public class ModlGoldenTests {
    [Test]
    public void TestExportsExactModel() {
      ModelGoldenAssert.AssertExportGoldens(
          this.GetBattalionWars1ModlGoldensDirectory_(),
          new ModlModelLoader(),
          this.GetBattalionWars1ModelFileBundleInDirectory_);
    }

    private ISystemDirectory GetBattalionWars1ModlGoldensDirectory_()
      => ModelGoldenAssert
         .GetRootGoldensDirectory(Assembly.GetExecutingAssembly())
         .GetSubdir("modl/battalion_wars_1");

    private ModlModelFileBundle GetBattalionWars1ModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory)
      => new() {
          GameName = "foobar",
          GameVersion = GameVersion.BW1,
          ModlFile = directory.FilesWithExtension(".modl").Single(),
          AnimFiles = directory.FilesWithExtension(".anim").ToArray()
      };
  }
}