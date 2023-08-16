using System.Reflection;

using fin.io;
using fin.testing;

using hw.api;

namespace hw {
  public class XtdGoldenTests {
    [Test]
    public void TestExportsExactModel() {
      ModelGoldenAssert.AssertExportGoldens(
          this.GetXtdGoldensDirectory_(),
          new XtdModelLoader(),
          this.GetModelFileBundleInDirectory_);
    }

    private ISystemDirectory GetXtdGoldensDirectory_()
      => ModelGoldenAssert.GetRootGoldensDirectory(
                              Assembly.GetExecutingAssembly())
                          .GetSubdir("xtd");

    private XtdModelFileBundle GetModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory)
      => new(directory.FilesWithExtension(".xtd").Single(),
             directory.FilesWithExtension(".xtt").Single());
  }
}