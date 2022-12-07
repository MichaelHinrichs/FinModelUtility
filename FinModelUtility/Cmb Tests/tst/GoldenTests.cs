using cmb.schema.cmb;
using cmb.schema.ctxb;
using System.Reflection;
using fin.io;
using fin.util.strings;
using schema.testing;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace cmb {
  public class GoldenTests {
    [Test]
    public async Task TestEachCtxbGolden() {
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

      var cmbTestsDir = currentDir;
      var goldensDir = cmbTestsDir.GetSubdir("goldens/ctxb");

      var goldenGameDirs = goldensDir.GetExistingSubdirs();
      foreach (var goldenGameDir in goldenGameDirs) {
        CmbHeader.Version = goldenGameDir.Name switch {
            "luigis_mansion_3d" => CmbVersion.LUIGIS_MANSION_3D,
            "majoras_mask_3d" => CmbVersion.MAJORAS_MASK_3D,
            "ocarina_of_time_3d" => CmbVersion.OCARINA_OF_TIME_3D
        };

        var goldenFiles = goldenGameDir.GetExistingFiles();
        foreach (var goldenFile in goldenFiles) {
          var er = new EndianBinaryReader(goldenFile.OpenRead());
          await BinarySchemaAssert.ReadsAndWritesIdentically<Ctxb>(er);
        }
      }
    }
  }
}