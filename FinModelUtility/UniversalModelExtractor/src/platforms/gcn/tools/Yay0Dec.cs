using System.IO;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.util.cmd;
using uni.util.io;

namespace uni.platforms.gcn.tools {
  public class Yay0Dec {
    public bool Run(IFileHierarchyFile szpFile) {
      Asserts.True(
          szpFile.Exists,
          $"Cannot decrypt SZP because it does not exist: {szpFile}");
      Asserts.Equal(
          ".szp",
          szpFile.Extension,
          $"Cannot decrypt file because it is not an SZP: {szpFile}");

      //var rarcPath = szpFile.FullName + " 0.rarc";
      var didRarcExist = false; //File.Exists(rarcPath);
      if (!didRarcExist) {
        var logger = Logging.Create<Yay0Dec>();
        logger.LogInformation($"Decrypting SZP {szpFile.LocalPath}...");

        Files.RunInDirectory(
            szpFile.Impl.GetParent()!,
            () => {
              ProcessUtil.ExecuteBlockingSilently(
                  GcnToolsConstants.YAY0DEC_EXE,
                  $"\"{szpFile.FullName}\"");
            });
        //Asserts.True(File.Exists(rarcPath),
        //             $"File was not created: {rarcPath}");
      }

      return !didRarcExist;
    }
  }
}