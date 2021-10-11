using System.Collections.Generic;
using System.IO;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.util.cmd;

namespace uni.platforms.gcn.tools {
  public class Bmd2Fbx {
    public void Run(
        IFile bmdFile,
        IFile[]? bcxFiles,
        IFile[]? btiFiles,
        IDirectory baseRomDirectory) {
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
      var romName =
          baseRomDirectory.Name.Substring(0,
                                          baseRomDirectory.Name.Length -
                                          "_dir".Length -
                                          ".gcm".Length);
      var localFilePath =
          bmdFile.FullName.Substring(baseRomDirectory.FullName.Length);
      var localDirectoryPath =
          localFilePath.Substring(0,
                                  localFilePath.Length - bmdFile.Name.Length);

      ;

      var outDirectory =
          DirectoryConstants.OUT_DIRECTORY.TryToGetSubdir(romName, true);

      var args = new List<string>();

      args.Add("manual");
      args.Add("--static");
      args.Add("--verbose");

      args.Add("--out");
      args.Add($"\"{outDirectory.FullName}\"");

      args.Add("--bmd");
      args.Add($"\"{bmdFile.FullName}\"");

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