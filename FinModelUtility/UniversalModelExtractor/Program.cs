using fin.io;

using uni.platforms.gcn;

namespace uni {
  public class Program {
    public static void Main(string[] args) {
      var pikmin2Rom =
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pikmin_2.gcm");
      GcnFileHierarchyExtractor.ExtractFromRom(pikmin2Rom);
    }
  }
}