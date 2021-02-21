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
    string bmdPath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/enemy.bmd";
    IList<string> bcxPaths = new[] {
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack0.bca",
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack1.bca",
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/attack4.bca",
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/flick.bca",
        "R:/Documents/CSharpWorkspace/Pikmin2Utility/cli/out/move.bca"
    };

    if (args.Length >= 2) {
      outputPath = args[0];
      bmdPath = args[1];
      bcxPaths = args.Skip(2).ToList();
    }

    var bmd = new BMD(File.ReadAllBytes(bmdPath));
    var pathsAndBcxs = bcxPaths
                       .Select(bcxPath => {
                         var extension =
                             new FileInfo(bcxPath).Extension.ToLower();
                         IBcx bcx = extension switch {
                             ".bca" => new BCA(File.ReadAllBytes(bcxPath)),
                             ".bck" => new BCK(File.ReadAllBytes(bcxPath)),
                             _      => throw new NotSupportedException(),
                         };
                         return (bcxPath, bcx);
                       })
                       .ToList();

    GltfExporter.Export(outputPath, bmd, pathsAndBcxs);
  }
}