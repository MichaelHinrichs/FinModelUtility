using System.IO;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.util.cmd;

namespace uni.platforms.gcn.tools {
  public class Yaz0Dec {
    public bool Run(IFile szsFile, IDirectory baseRomDirectory) {
      Asserts.True(
          szsFile.Exists,
          $"Cannot decrypt SZS because it does not exist: {szsFile}");
      Asserts.Equal(
          ".szs",
          szsFile.Extension,
          $"Cannot decrypt file because it is not an SZS: {szsFile}");

      var rarcPath = szsFile.FullName + " 0.rarc";
      var didRarcExist = File.Exists(rarcPath);
      if (!didRarcExist) {
        var logger = Logging.Create<Yaz0Dec>();
        using var yaz0DecScope = logger.BeginScope("yaz0dec");

        var localSzsFilePath =
            szsFile.FullName.Substring(baseRomDirectory.FullName.Length);
        logger.LogInformation($"Decrypting SZS {localSzsFilePath}...");

        Files.RunInDirectory(
            szsFile.GetParent()!,
            () => {
              ProcessUtil.ExecuteBlockingSilently(
                  GcnToolsConstants.YAZ0DEC_EXE,
                  $"\"{szsFile.FullName}\"");
            });
        Asserts.True(File.Exists(rarcPath),
                     $"File was not created: {rarcPath}");
      }

      return !didRarcExist;
    }
  }
}