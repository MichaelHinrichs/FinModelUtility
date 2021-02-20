using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using mkds.exporter;

using MKDS_Course_Modifier.GCN;

public class Cli {
  public static void Main(string[] args) {
    string outputPath =
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/out.glb";
    //string bmdPath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/enemy.bmd";
    string bmdPath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/roms/pkmn2.gcm_dir/enemy/data/Kogane./model.szs 0.rarc_dir./model/enemy.bmd";
    IList<string> bcaPaths = new[] {
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack0.bca",
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack1.bca",
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack4.bca",
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/flick.bca",
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/move.bca"
    };

    if (args.Length >= 2) {
      outputPath = args[0];
      bmdPath = args[1];
      bcaPaths = args.Skip(2).ToList();
    }

    var bmd = new BMD(File.ReadAllBytes(bmdPath));
    var pathsAndBcas = bcaPaths
                       .Select(bcaPath => (bcaPath,
                                           new BCA(File.ReadAllBytes(bcaPath))))
                       .ToList();

    GltfExporter.Export(outputPath, bmd, pathsAndBcas);
  }
}