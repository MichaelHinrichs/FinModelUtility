using asserts;

using fin.io;
using fin.log;

using uni.util.cmd;

namespace uni.platforms.gcn.tools {
  public class Yay0Dec {
    public bool Run(IFileHierarchyFile file, bool cleanup) {
      Asserts.True(
          file.Exists,
          $"Cannot decrypt YAY0 because it does not exist: {file}");

      var finalRarcPath = file.FullNameWithoutExtension + ".rarc";
      if (File.Exists(finalRarcPath)) {
        return false;
      }

      if (!MagicTextUtil.Verify(file, "Yay0")) {
        return false;
      }

      var rarcPath = file.FullName + " 0.rarc";
      if (!File.Exists(rarcPath)) {
        var logger = Logging.Create<Yay0Dec>();
        Files.RunInDirectory(
            file.Impl.GetParent()!,
            () => {
              ProcessUtil.ExecuteBlockingSilently(
                  GcnToolsConstants.YAY0DEC_EXE,
                  $"\"{file.FullName}\"");
            });
        Asserts.True(File.Exists(rarcPath),
                     $"File was not created: {rarcPath}");
      }

      File.Move(rarcPath, finalRarcPath);
      if (cleanup) {
        file.Impl.Delete();
      }

      return true;
    }
  }
}