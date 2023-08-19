using System.Reflection;

using fin.io;
using fin.testing.model;

using hw.api;

namespace hw {
  public class XtdModelGoldenTests
      : BModelGoldenTests<XtdModelFileBundle, XtdModelLoader> {
    [Test]
    [TestCaseSource(nameof(GetGoldenDirectories_))]
    public void TestExportsGoldenAsExpected(
        IFileHierarchyDirectory goldenDirectory)
      => this.AssertGolden(goldenDirectory);

    public override XtdModelFileBundle GetFileBundleFromDirectory(
        IFileHierarchyDirectory directory)
      => new(directory.FilesWithExtension(".xtd").Single(),
             directory.FilesWithExtension(".xtt").Single());

    private static IFileHierarchyDirectory[] GetGoldenDirectories_()
      => ModelGoldenAssert
         .GetGoldenDirectories(
             ModelGoldenAssert
                 .GetRootGoldensDirectory(Assembly.GetExecutingAssembly())
                 .AssertGetExistingSubdir("xtd"))
         .ToArray();
  }
}