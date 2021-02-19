using System.Windows.Forms;
using System.IO;

using MKDS_Course_Modifier.GCN;

public class Cli {
  public static void Main(string[] args) {
    // TODO: Support multiple BCA names.
    /*var outputName = args[0];
    var bmdName = args[1];
    var bcaName = args[2];*/

    var outputName = "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/out.ma";
    var bmdName = "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/enemy.bmd";
    var bcaName = "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack0.bca";

    var bmd = new BMD(File.ReadAllBytes(bmdName));
    var bca = new BCA(File.ReadAllBytes(bcaName));

    File.WriteAllBytes(outputName, bca.ANF1.ExportBonesAnimation(bmd));
  }
}