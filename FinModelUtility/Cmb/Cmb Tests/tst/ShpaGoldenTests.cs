using System.Reflection;

using cmb.schema.cmb;
using cmb.schema.shpa;

using fin.io;
using fin.testing;
using fin.util.strings;

using schema.binary;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Version = cmb.schema.cmb.Version;

namespace cmb {
  public class ShpaGoldenTests {
    [Test]
    public async Task TestEachShpaGolden() {
      var executingAssembly = Assembly.GetExecutingAssembly();
      var assemblyName =
          StringUtil.SubstringUpTo(executingAssembly.ManifestModule.Name, ".dll");

      var executingAssemblyDll =
          new FinFile(Assembly.GetExecutingAssembly().Location);
      var executingAssemblyDir = executingAssemblyDll.GetParent();

      var currentDir = executingAssemblyDir;
      while (currentDir.Name != assemblyName) {
        currentDir = currentDir.GetParent();
      }

      Assert.IsNotNull(currentDir);

      var cmbTestsDir = currentDir;
      var goldensDir = cmbTestsDir.GetSubdir("goldens/shpa");

      var goldenGameDirs = goldensDir.GetExistingSubdirs();
      foreach (var goldenGameDir in goldenGameDirs) {
        CmbHeader.Version = goldenGameDir.Name switch {
            "luigis_mansion_3d" => Version.LUIGIS_MANSION_3D,
        };

        var goldenFiles = goldenGameDir.GetExistingFiles();
        foreach (var goldenFile in goldenFiles) {
          var er = new EndianBinaryReader(goldenFile.OpenRead());
          await SchemaTesting.ReadsAndWritesIdentically<Shpa>(
              er,
              assertExactEndPositions: false);
        }
      }
    }
  }
}