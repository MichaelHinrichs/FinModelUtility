using System.Reflection;

using fin.io;
using fin.util.strings;

using glo.schema;

using NUnit.Framework;

using schema.binary.testing;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace glo {
  public class GoldenTests {
    [Test]
    public async Task TestReadingAndWritingEachGloGolden() {
      this.GetGloGoldens_(out _, out var gloGoldenFiles);
      foreach (var goldenFile in gloGoldenFiles) {
        var er = new EndianBinaryReader(goldenFile.OpenRead());
        await BinarySchemaAssert.ReadsAndWritesIdentically<Glo>(er);
      }
    }

    private void GetGloGoldens_(
        out IFileHierarchyDirectory gloGoldensDirectory,
        out IEnumerable<IFileHierarchyFile> gloGoldenFiles) {
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

      var hierarchy = new FileHierarchy(goldensDirectory);

      gloGoldensDirectory = hierarchy.Root;
      gloGoldenFiles = gloGoldensDirectory.FilesWithExtension(".glo");
    }
  }
}