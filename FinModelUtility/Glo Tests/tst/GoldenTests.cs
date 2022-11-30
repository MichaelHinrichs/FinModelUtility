using System.Reflection;

using fin.io;
using fin.util.strings;

using glo.schema;
using NUnit.Framework;
using schema.testing;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace fin.data {
  public class GoldenTests {
    [Test]
    public async Task TestEachGolden() {
      var executingAssembly = Assembly.GetExecutingAssembly();
      var assemblyName =
          StringUtil.UpTo(executingAssembly.ManifestModule.Name, ".dll");

      var executingAssemblyDll =
          new FinFile(Assembly.GetExecutingAssembly().Location);
      var executingAssemblyDir = executingAssemblyDll.GetParent();

      var currentDir = executingAssemblyDir;
      while (currentDir != null && currentDir.Name != assemblyName) {
        currentDir = currentDir.GetParent();
      }
      Assert.IsNotNull(currentDir);

      var gloTestsDir = currentDir;
      var goldensDir = gloTestsDir.GetSubdir("goldens");

      var goldenFiles = goldensDir.GetExistingFiles();
      foreach (var goldenFile in goldenFiles) {
        var er =
            new EndianBinaryReader(goldenFile.OpenRead(),
                                   Endianness.LittleEndian);
        await BinarySchemaAssert.ReadsAndWritesIdentically<Glo>(er);
      }
    }
  }
}