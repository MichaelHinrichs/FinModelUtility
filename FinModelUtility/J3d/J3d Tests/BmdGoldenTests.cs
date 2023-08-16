using System.Reflection;

using fin.io;
using fin.testing;

using j3d.exporter;
using j3d.GCN;

namespace j3d {
  public class BmdGoldenTests {
    [Test]
    public void TestExportsExactModel() {
      ModelGoldenAssert.AssertExportGoldens(
          GetGoldensDirectory_(),
          new BmdModelLoader(),
          GetModelFileBundleInDirectory_);
    }

    [Test]
    public async Task TestExportBmdDrw1s() {
      foreach (var bmd in GetBmds_()) {
        await SchemaTesting.WritesAndReadsIdentically(bmd.DRW1);
      }
    }

    [Test]
    public async Task TestExportBmdInf1s() {
      foreach (var bmd in GetBmds_()) {
        await SchemaTesting.WritesAndReadsIdentically(bmd.INF1);
      }
    }

    [Test]
    public async Task TestExportBmdJnt1s() {
      foreach (var bmd in GetBmds_()) {
        await SchemaTesting.WritesAndReadsIdentically(bmd.JNT1);
      }
    }

    private static IEnumerable<BmdModelFileBundle> GetBundles()
      => ModelGoldenAssert.GetGoldenModelBundles(
          GetGoldensDirectory_(),
          GetModelFileBundleInDirectory_);

    private static IEnumerable<BMD> GetBmds_()
      => GetBundles().Select(bundle => new BMD(bundle.BmdFile.ReadAllBytes()));


    private static ISystemDirectory GetGoldensDirectory_()
      => ModelGoldenAssert.GetRootGoldensDirectory(
          Assembly.GetExecutingAssembly());

    private static BmdModelFileBundle GetModelFileBundleInDirectory_(
        IFileHierarchyDirectory directory)
      => new() {
          GameName = "foobar",
          BmdFile = directory.FilesWithExtension(".bmd").Single(),
          BcxFiles = directory.FilesWithExtensions(".bca", ".bck").ToArray(),
          BtiFiles = directory.FilesWithExtension(".bti").ToArray(),
      };
  }
}