using System.Reflection;

using cmb.api;

using fin.io;
using fin.testing;


namespace cmb {
  public class CmbGoldenTests {
    [Test]
    public void TestExportsExactModel() {
      var cmbGoldensDirectory = this.GetCmbGoldensDirectory_();
      foreach (var subdir in cmbGoldensDirectory.GetExistingSubdirs()) {
        var gameName = subdir.Name;
        ModelGoldenAssert.AssertExportGoldens(
            subdir,
            new CmbModelLoader(),
            dir => this.GetCmbModelFileBundleInDirectory_(dir, gameName));
      }
    }

    private IDirectory GetCmbGoldensDirectory_()
      => ModelGoldenAssert.GetRootGoldensDirectory(
                              Assembly.GetExecutingAssembly())
                          .GetSubdir("cmb");

    private CmbModelFileBundle GetCmbModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory,
        string gameName) {
      var cmbFile = directory.FilesWithExtension(".cmb").Single();
      return new CmbModelFileBundle(
          gameName,
          cmbFile,
          directory.FilesWithExtension(".csab").ToArray(),
          null,
          null);
    }
  }
}