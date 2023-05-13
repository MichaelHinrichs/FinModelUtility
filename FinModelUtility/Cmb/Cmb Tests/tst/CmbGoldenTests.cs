using System.Reflection;

using cmb.api;

using fin.io;
using fin.testing;


namespace cmb {
  public class CmbGoldenTests {
    [Test]
    public void TestExportsExactModel() {
      ModelGoldenAssert.AssertExportGoldens(
          this.GetCmbGoldensDirectory_(),
          new CmbModelLoader(),
          this.GetModelFileBundleInDirectory_);
    }

    private ISystemDirectory GetCmbGoldensDirectory_()
      => ModelGoldenAssert.GetRootGoldensDirectory(
                              Assembly.GetExecutingAssembly())
                          .GetSubdir("cmb");

    private CmbModelFileBundle GetModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory) {
      var cmbFile = directory.FilesWithExtension(".cmb").Single();
      return new CmbModelFileBundle(
          "foobar",
          cmbFile,
          directory.FilesWithExtension(".csab").ToArray(),
          null,
          null);
    }
  }
}