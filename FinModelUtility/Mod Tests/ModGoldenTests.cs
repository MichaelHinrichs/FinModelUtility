using System.Reflection;

using fin.io;
using fin.testing;

using mod.cli;

using NUnit.Framework;


namespace mod {
  public class ModGoldenTests {
    [Test]
    public void TestExportsExactModel() {
      ModelGoldenAssert.AssertExportGoldens(
          this.GetGoldensDirectory_(),
          new ModModelLoader(),
          this.GetModModelFileBundleInDirectory_);
    }

    private IDirectory GetGoldensDirectory_()
      => ModelGoldenAssert.GetRootGoldensDirectory(
          Assembly.GetExecutingAssembly());

    private ModModelFileBundle GetModModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory) {
      return new ModModelFileBundle {
          GameName = "pikmin_1",
          ModFile = directory.FilesWithExtension(".mod").Single(),
          AnmFile = directory.FilesWithExtension(".anm").Single(),
      };
    }
  }
}