using System.Text;

using fin.io;
using fin.log;
using fin.util.asserts;
using fin.util.strings;


namespace uni.platforms.threeDs.tools {
  public class ZarExtractor {
    public bool Extract(IFileHierarchyFile zarFile) {
      Asserts.True(zarFile.Exists,
                   $"Could not extract ZAR because it does not exist: {zarFile.FullName}");
      Asserts.Equal(".zar",
                    zarFile.Extension,
                    $"Could not extract file because it is not a ZAR: {zarFile.FullName}");

      var directoryPath =
          zarFile.FullName.Substring(0,
                                     zarFile.FullName.Length - ".zar".Length);
      var directory = new FinDirectory(directoryPath);

      if (directory.Exists) {
        return false;
      }

      var logger = Logging.Create<ZarExtractor>();
      logger.LogInformation($"Extracting ZAR {zarFile.LocalPath}...");

      Zar zar;
      {
        using var er =
            new EndianBinaryReader(zarFile.Impl.OpenRead(),
                                   Endianness.LittleEndian);
        zar = new Zar(er);
      }

      foreach (var fileType in zar.FileTypes) {
        foreach (var file in fileType.Files) {
          var filePath = Path.Join(directoryPath, file.FileName);

          Directory.CreateDirectory(
              Asserts.CastNonnull(Path.GetDirectoryName(filePath)));
          File.WriteAllBytes(filePath, file.Bytes);
        }
      }

      zarFile.Impl.Delete();

      return true;
    }

    private class Zar {
      public ZarHeader Header { get; }
      public ZarFileType[] FileTypes { get; }

      public Zar(IEndianBinaryReader er) {
        this.Header = new ZarHeader(er);

        this.FileTypes = new ZarFileType[this.Header.FileTypeCount];
        for (var i = 0; i < this.FileTypes.Length; ++i) {
          this.FileTypes[i] = new ZarFileType(er, this.Header, i);
        }
      }
    }

    private class ZarHeader {
      public int Size { get; }

      public short FileTypeCount { get; }
      public short FileCount { get; }

      public int FileTypesOffset { get; }
      public int FileMetadataOffset { get; }
      public int DataOffset { get; }

      public ZarHeader(IEndianBinaryReader er) {
        er.AssertMagicText("ZAR" + AsciiUtil.GetChar(1));

        this.Size = er.ReadInt32();

        this.FileTypeCount = er.ReadInt16();
        this.FileCount = er.ReadInt16();

        this.FileTypesOffset = er.ReadInt32();
        this.FileMetadataOffset = er.ReadInt32();
        this.DataOffset = er.ReadInt32();
      }
    }

    private class ZarFileType {
      public int FileCount { get; }
      public int FileListOffset { get; }
      public int TypeNameOffset { get; }
      public string TypeName { get; }

      public ZarSubfile[] Files { get; }

      public ZarFileType(
          IEndianBinaryReader er,
          ZarHeader header,
          int fileTypeIndex) {
        er.Position = header.FileTypesOffset + 16 * fileTypeIndex;

        this.FileCount = er.ReadInt32();
        this.FileListOffset = er.ReadInt32();
        this.TypeNameOffset = er.ReadInt32();
        er.ReadInt32();

        /*er.Position = this.TypeNameOffset;
        this.TypeName = er.ReadStringNT(Encoding.UTF8);*/

        this.Files = new ZarSubfile[this.FileCount];
        for (var i = 0; i < this.FileCount; ++i) {
          this.Files[i] = new ZarSubfile(er, header, this, i);
        }
      }
    }

    private class ZarSubfile {
      public string FileName { get; }
      public byte[] Bytes { get; }

      public ZarSubfile(
          IEndianBinaryReader er,
          ZarHeader header,
          ZarFileType fileType,
          int fileInFileTypeIndex) {
        er.Position = fileType.FileListOffset + 4 * fileInFileTypeIndex;
        var fileIndex = er.ReadInt32();

        er.Position = header.FileMetadataOffset + 8 * fileIndex;
        var fileSize = er.ReadInt32();
        var fileNameOffset = er.ReadInt32();

        er.Position = fileNameOffset;
        this.FileName = er.ReadStringNT(Encoding.UTF8);

        er.Position = header.DataOffset + 4 * fileIndex;
        var fileOffset = er.ReadInt32();

        er.Position = fileOffset;
        this.Bytes = er.ReadBytes(fileSize);
      }
    }
  }
}