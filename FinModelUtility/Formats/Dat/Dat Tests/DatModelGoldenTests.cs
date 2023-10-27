using System.Reflection;

using fin.io;
using fin.testing.model;

using dat.api;

namespace dat {
  public class DatModelGoldenTests
      : BModelGoldenTests<DatModelFileBundle, DatModelImporter> {
    [Test]
    [TestCaseSource(nameof(GetGoldenDirectories_))]
    public void TestExportsGoldenAsExpected(
        IFileHierarchyDirectory goldenDirectory)
      => this.AssertGolden(goldenDirectory);

    public override DatModelFileBundle GetFileBundleFromDirectory(
        IFileHierarchyDirectory directory)
      => new() {
          GameName = directory.Parent.Parent.Name,
          PrimaryDatFile = directory.FilesWithExtension(".dat").Single(),
      };

    private static IFileHierarchyDirectory[] GetGoldenDirectories_()
      => ModelGoldenAssert
         .GetGoldenDirectories(
             ModelGoldenAssert
                 .GetRootGoldensDirectory(Assembly.GetExecutingAssembly()))
         .SelectMany(dir => dir.GetExistingSubdirs())
         .ToArray();
  }
}