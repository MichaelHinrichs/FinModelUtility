using System.Reflection;

using fin.io;
using fin.testing.model;

using glo.api;
using glo.schema;

using NUnit.Framework;

using schema.binary;
using schema.binary.testing;

namespace glo {
  public class
      GloModelGoldenTests : BModelGoldenTests<GloModelFileBundle,
          GloModelImporter> {
    [Test]
    [TestCaseSource(nameof(GetGoldenDirectories_))]
    public async Task TestReadsAndWritesIdentically(
        IFileHierarchyDirectory goldenDirectory) {
      var goldenBundle =
          this.GetFileBundleFromDirectory(
              goldenDirectory.AssertGetExistingSubdir("input"));

      var er = new EndianBinaryReader(goldenBundle.GloFile.OpenRead());
      await BinarySchemaAssert.ReadsAndWritesIdentically<Glo>(er);
    }

    [Test]
    [TestCaseSource(nameof(GetGoldenDirectories_))]
    public void TestExportsGoldenAsExpected(IFileHierarchyDirectory goldenDirectory)
      => this.AssertGolden(goldenDirectory);

    public override GloModelFileBundle GetFileBundleFromDirectory(
        IFileHierarchyDirectory directory)
      => new(directory.FilesWithExtension(".glo").Single(),
             new[] { directory });

    private static IFileHierarchyDirectory[] GetGoldenDirectories_() {
      var rootGoldenDirectory = ModelGoldenAssert.GetRootGoldensDirectory(
          Assembly.GetExecutingAssembly());
      return ModelGoldenAssert.GetGoldenDirectories(rootGoldenDirectory)
                              .ToArray();
    }
  }
}