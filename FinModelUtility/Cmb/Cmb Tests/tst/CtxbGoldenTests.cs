using System.Reflection;

using cmb.schema.cmb;
using cmb.schema.ctxb;

using fin.io;
using fin.testing.model;
using fin.util.strings;

using schema.binary;
using schema.binary.testing;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Version = cmb.schema.cmb.Version;

namespace cmb {
  public class CtxbGoldenTests {
    [Test]
    [TestCaseSource(nameof(GetGoldenFiles_))]
    public async Task TestExportsGoldenAsExpected(ISystemFile goldenFile) {
      var goldenGameDir = goldenFile.GetParent();

      CmbHeader.Version = goldenGameDir.Name switch {
          "luigis_mansion_3d"  => Version.LUIGIS_MANSION_3D,
          "majoras_mask_3d"    => Version.MAJORAS_MASK_3D,
          "ocarina_of_time_3d" => Version.OCARINA_OF_TIME_3D
      };

      var er = new EndianBinaryReader(goldenFile.OpenRead());
      await BinarySchemaAssert.ReadsAndWritesIdentically<Ctxb>(er);
    }

    private static ISystemFile[] GetGoldenFiles_() {
      var rootGoldenDirectory
          = ModelGoldenAssert
            .GetRootGoldensDirectory(Assembly.GetExecutingAssembly())
            .GetSubdir("ctxb");
      return rootGoldenDirectory.GetExistingSubdirs()
                                .SelectMany(dir => dir.GetExistingFiles())
                                .ToArray();
    }
  }
}