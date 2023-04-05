using System.Reflection;

using fin.io;
using fin.testing;
using fin.util.strings;

using glo.api;
using glo.schema;

using NUnit.Framework;

using schema.binary.testing;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace glo {
  public class GoldenTests {
    [Test]
    public async Task TestReadsAndWritesIdentically() {
      foreach (var goldenBundle in ModelGoldenAssert
                                   .GetGoldenDirectories(
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

    private IDirectory GetGloGoldensDirectory_() {
      var executingAssembly = Assembly.GetExecutingAssembly();
      var assemblyName =
          StringUtil.UpTo(executingAssembly.ManifestModule.Name, ".dll");

      var executingAssemblyDll =
          new FinFile(Assembly.GetExecutingAssembly().Location);
      var executingAssemblyDir = executingAssemblyDll.GetParent();

      var currentDir = executingAssemblyDir;
      while (currentDir.Name != assemblyName) {
        currentDir = currentDir.GetParent();
      }

      Assert.IsNotNull(currentDir);

      var gloTestsDir = currentDir;
      var goldensDirectory = gloTestsDir.GetSubdir("goldens");

      return goldensDirectory;
    }

    private GloModelFileBundle GetGloModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory) {
      var gloFile = directory.FilesWithExtension(".glo").Single();
      return new GloModelFileBundle(
          gloFile,
          new[] {directory});
    }
  }
}