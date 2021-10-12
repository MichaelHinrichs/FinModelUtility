using System.Collections.Generic;

using fin.util.asserts;

using uni.games;
using uni.util.cmd;
using uni.util.io;

namespace uni.platforms.gcn.tools {
  public class Mod2Fbx {
    public void Run(
        IFileHierarchyFile[] modFiles,
        IFileHierarchyFile[]? anmFiles) {
      Asserts.True(modFiles.Length > 0, "Expected to have at least one MOD!");
      foreach (var modFile in modFiles) {
        Asserts.True(
            modFile.Exists,
            $"Cannot extract MOD because it does not exist: {modFile}");
        Asserts.Equal(
            ".mod",
            modFile.Extension,
            $"Cannot extract model because it is not a MOD: {modFile}");
      }

      anmFiles ??= new IFileHierarchyFile[] {};
      foreach (var anmFile in anmFiles) {
        Asserts.True(
            anmFile.Exists,
            $"Cannot extract ANM because it does not exist: {anmFile}");
        Asserts.Equal(
            ".anm",
            anmFile.Extension,
            $"Cannot extract animation because it is not an ANM: {anmFile}");
      }

      // TODO: Make sure model doesn't already exist
      var outDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForFile(modFiles[0]);

      var args = new List<string> {
          "manual",
          "--verbose",
          "--out",
          $"\"{outDirectory.FullName}\"",
      };

      args.Add("--mod");
      foreach (var modFile in modFiles) {
        args.Add($"\"{modFile.FullName}\"");
      }

      if (anmFiles.Length > 0) {
        args.Add("--anm");
        foreach (var anmFile in anmFiles) {
          args.Add($"\"{anmFile.FullName}\"");
        }
      }

      ProcessUtil.ExecuteBlocking(GcnToolsConstants.MOD2FBX_EXE,
                                  args.ToArray());
    }
  }
}