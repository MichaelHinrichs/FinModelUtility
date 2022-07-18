using fin.io;
using fin.log;
using fin.util.asserts;

using modl.schema.res;

using uni.platforms.gcn.tools;


namespace uni.games.battalion_wars {
  public class ResDump {
    public bool Run(IFileHierarchyFile resFile) {
      Asserts.True(
          resFile.Impl.Exists,
          $"Cannot dump RES because it does not exist: {resFile}");

      if (!MagicTextUtil.Verify(resFile, "RXET")) {
        return false;
      }

      var directory = new FinDirectory(resFile.FullNameWithoutExtension);
      if (!directory.Exists) {
        var logger = Logging.Create<ResDump>();
        logger.LogInformation($"Dumping RES {resFile.LocalPath}...");

        var bwArchive = resFile.Impl.ReadNew<BwArchive>(Endianness.LittleEndian);

        directory.Create();
        foreach (var (bwFileExtension, bwFiles) in bwArchive.Files) {
          foreach (var bwFile in bwFiles) {
            var fileName = $"{bwFile.FileName}.{bwFileExtension.ToLower()}";
            var file = new FinFile(Path.Join(directory.FullName, fileName));
            using var w = file.OpenWrite();
            w.Write(bwFile.Data);
          }
        }

        foreach (var texture in bwArchive.Texr.Textures) {
          texture.Image.Save(Path.Join(directory.FullName,
                                       $"{texture.Name}.png"));
        }

        return true;
      }

      return false;
    }
  }
}