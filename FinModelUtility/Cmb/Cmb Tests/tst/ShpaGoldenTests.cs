using System.Reflection;

using cmb.schema.cmb;
using cmb.schema.shpa;

using fin.io;
using fin.testing;
using fin.testing.model;

using schema.binary;

using Version = cmb.schema.cmb.Version;

namespace cmb {
  public class ShpaGoldenTests {
    [Test]
    [TestCaseSource(nameof(GetGoldenFiles_))]
    public async Task TestExportsGoldenAsExpected(ISystemFile goldenFile) {
      var goldenGameDir = goldenFile.AssertGetParent();

      CmbHeader.Version = goldenGameDir.Name switch {
          "luigis_mansion_3d" => Version.LUIGIS_MANSION_3D,
      };

      var er = new EndianBinaryReader(goldenFile.OpenRead());
      await SchemaTesting.ReadsAndWritesIdentically<Shpa>(
          er,
          assertExactEndPositions: false);
    }

    private static ISystemFile[] GetGoldenFiles_() {
      var rootGoldenDirectory
          = ModelGoldenAssert
            .GetRootGoldensDirectory(Assembly.GetExecutingAssembly())
            .AssertGetExistingSubdir("shpa");
      return rootGoldenDirectory.GetExistingSubdirs()
                                .SelectMany(dir => dir.GetExistingFiles())
                                .ToArray();
    }
  }
}