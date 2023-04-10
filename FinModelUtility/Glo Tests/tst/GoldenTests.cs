using System.Reflection;

using fin.io;
using fin.testing;

using glo.api;
using glo.schema;

using NUnit.Framework;

using schema.binary.testing;


namespace glo {
  public class GoldenTests {
    [Test]
    public async Task TestReadsAndWritesIdentically() {
      foreach (var goldenBundle in ModelGoldenAssert
                                   .GetGoldenInputDirectories(
                                       this.GetGloGoldensDirectory_())
                                   .Select(
                                       GetGloModelFileBundleInDirectory_)) {
        var er = new EndianBinaryReader(goldenBundle.GloFile.OpenRead());
        await BinarySchemaAssert.ReadsAndWritesIdentically<Glo>(er);
      }
    }

    [Test]
    public void TestExportsExactModel()
      => ModelGoldenAssert.AssertExportGoldens(
          this.GetGloGoldensDirectory_(),
          new GloModelLoader(),
          GetGloModelFileBundleInDirectory_);

    private ISystemDirectory GetGloGoldensDirectory_()
      => ModelGoldenAssert.GetRootGoldensDirectory(
          Assembly.GetExecutingAssembly());

    private GloModelFileBundle GetGloModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory) {
      var gloFile = directory.FilesWithExtension(".glo").Single();
      return new GloModelFileBundle(
          gloFile,
          new[] {directory});
    }
  }
}