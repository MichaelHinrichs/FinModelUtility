using System.IO.Compression;

using fin.io;
using fin.log;
using fin.util.asserts;

using modl.schema.res;

using uni.platforms.gcn.tools;


namespace uni.games.battalion_wars_1 {
  public class ResDump {
    public bool Run(IFileHierarchyFile resFile) {
      Asserts.True(
          resFile.Impl.Exists,
          $"Cannot dump RES because it does not exist: {resFile}");

      var directoryFullName = resFile.FullNameWithoutExtension;

      var isResGz = resFile.Name.EndsWith(".res.gz");
      if (isResGz) {
        directoryFullName = directoryFullName[..^4];
      }

      var directory = new FinDirectory(directoryFullName);

      if (!isResGz && MagicTextUtil.Verify(resFile, "RXET")) {
        return false;
      }

      if (!directory.Exists) {
        var logger = Logging.Create<ResDump>();
        logger.LogInformation($"Dumping RES {resFile.LocalPath}...");

        Stream stream;
        if (isResGz) {
          using var gZipStream =
              new GZipStream(resFile.Impl.OpenRead(),
                             CompressionMode.Decompress);

          stream = new MemoryStream();
          gZipStream.CopyTo(stream);
          stream.Position = 0;
        } else {
          stream = resFile.Impl.OpenRead();
        }

        using var er = new EndianBinaryReader(stream, Endianness.LittleEndian);

        var bwArchive = er.ReadNew<BwArchive>();

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