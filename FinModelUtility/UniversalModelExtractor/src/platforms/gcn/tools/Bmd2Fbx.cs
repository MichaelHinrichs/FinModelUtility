using fin.io;
using fin.util.asserts;

using uni.games;
using uni.util.cmd;

namespace uni.platforms.gcn.tools {
  public class Bmd2Fbx {
    public void Run(
        IFileHierarchyFile bmdFile,
        IFile[]? bcxFiles,
        IFile[]? btiFiles) {
      Asserts.True(
          bmdFile.Exists,
          $"Cannot extract BMD because it does not exist: {bmdFile}");
      Asserts.Equal(
          ".bmd",
          bmdFile.Extension,
          $"Cannot extract model because it is not a BMD: {bmdFile}");

      bcxFiles ??= new IFile[] {};
      foreach (var bcxFile in bcxFiles) {
        Asserts.True(
            bcxFile.Exists,
            $"Cannot extract BCA/BCK because it does not exist: {bcxFile}");
        Asserts.True(
            bcxFile.Extension == ".bca" || bcxFile.Extension == ".bck",
            $"Cannot extract animation because it is not a BCA/BCK: {bcxFile}");
      }

      btiFiles ??= new IFile[] {};
      foreach (var btiFile in btiFiles) {
        Asserts.True(
            btiFile.Exists,
            $"Cannot extract BTI because it does not exist: {btiFile}");
        Asserts.Equal(
            ".bti",
            btiFile.Extension,
            $"Cannot extract model because it is not a BTI: {btiFile}");
      }

      // TODO: Make sure model doesn't already exist
      var outDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForFile(bmdFile);

      ;

      var args = new List<string> {
          "manual",
          "--verbose",
          "--out",
          $"\"{outDirectory.FullName}\"",
          "--bmd",
          $"\"{bmdFile.FullName}\""
      };

      if (bcxFiles.Length > 0) {
        args.Add("--bcx");
        foreach (var bcxFile in bcxFiles) {
          args.Add($"\"{bcxFile.FullName}\"");
        }
      }

      if (btiFiles.Length > 0) {
        args.Add("--bti");
        foreach (var btiFile in btiFiles) {
          args.Add($"\"{btiFile.FullName}\"");
        }
      }

      ProcessUtil.ExecuteBlocking(GcnToolsConstants.BMD2FBX_EXE,
                                  args.ToArray());
    }
  }
}