using System.Reflection;

using fin.io;
using fin.testing;

using j3d.exporter;


namespace j3d {
  public class BmdGoldenTests {
    [Test]
    public void TestExportsExactModel() {
      ModelGoldenAssert.AssertExportGoldens(
          this.GetGoldensDirectory_(),
          new BmdModelLoader(),
          this.GetModelFileBundleInDirectory_);
    }

    private ISystemDirectory GetGoldensDirectory_()
      => ModelGoldenAssert.GetRootGoldensDirectory(
          Assembly.GetExecutingAssembly());

    private BmdModelFileBundle GetModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory) {
      return new BmdModelFileBundle {
          GameName = "foobar",
          BmdFile = directory.FilesWithExtension(".bmd").Single(),
          BcxFiles = directory.FilesWithExtensions(".bca", ".bck").ToArray(),
          BtiFiles = directory.FilesWithExtension(".bti").ToArray(),
      };
    }
  }
}