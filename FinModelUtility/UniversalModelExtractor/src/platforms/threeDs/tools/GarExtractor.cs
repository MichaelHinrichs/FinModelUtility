using asserts;

using fin.io;
using fin.log;
using fin.util.strings;


namespace uni.platforms.threeDs.tools {
  public class GarExtractor {
    public bool Extract(IFileHierarchyFile garFile) {
      Asserts.True(garFile.Exists,
                   $"Could not extract GAR because it does not exist: {garFile.FullName}");

      var directoryPath = StringUtil.UpTo(garFile.FullName, ".gar");
      var directory = new FinDirectory(directoryPath);

      if (directory.Exists) {
        return false;
      }

      var logger = Logging.Create<ZarExtractor>();
      logger.LogInformation($"Extracting GAR {garFile.LocalPath}...");

      Gar gar;
      {
        using var er =
            new EndianBinaryReader(garFile.OpenRead(),
                                   Endianness.LittleEndian);
        gar = new Gar(er);
      }

      foreach (var fileType in gar.FileTypes) {
        foreach (var file in fileType.Files) {
          var fileName = file.FileName;

          if (!fileName.EndsWith($".{fileType.TypeName}")) {
            fileName = $"{fileName}.{fileType.TypeName}";
          }

          var filePath = Path.Join(directoryPath, fileName);
          Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
          File.WriteAllBytes(filePath, file.Bytes);
        }
      }

      garFile.Impl.Delete();

      return true;
    }

    /// <summary>
    ///   Based on the following:
    ///   - https://github.com/xdanieldzd/Scarlet/blob/master/Scarlet.IO.ContainerFormats/GARv2.cs
    ///   - https://github.com/xdanieldzd/Scarlet/blob/master/Scarlet.IO.ContainerFormats/GARv5.cs
    /// </summary>
    private class Gar {
      public GarHeader Header { get; }
      public IGarFileType[] FileTypes { get; }

      public Gar(IEndianBinaryReader er) {
        var isCompressed =
            new LzssDecompressor().TryToDecompress(er, out var decompressedGar);
        if (isCompressed) {
          er = new EndianBinaryReader(decompressedGar!, er.Endianness);
        }

        this.Header = new GarHeader(er);

        this.FileTypes = new IGarFileType[this.Header.FileTypeCount];
        for (var i = 0; i < this.FileTypes.Length; ++i) {
          this.FileTypes[i] = this.Header.Version switch {
              2 => new Gar2FileType(er, this.Header, i),
              5 => new Gar5FileType(er, this.Header, i),
              _ => throw new NotImplementedException()
          };
        }

        if (isCompressed) {
          er.Close();
        }
      }
    }

    private class GarHeader {
      public int Version { get; }

      public int Size { get; }

      public short FileTypeCount { get; }
      public short FileCount { get; }

      public int FileTypesOffset { get; }
      public int FileMetadataOffset { get; }
      public int DataOffset { get; }

      public GarHeader(IEndianBinaryReader er) {
        er.AssertMagicText("GAR");

        this.Version = er.ReadByte();
        Asserts.True(this.Version is 2 or 5);

        this.Size = er.ReadInt32();

        this.FileTypeCount = er.ReadInt16();
        this.FileCount = er.ReadInt16();

        this.FileTypesOffset = er.ReadInt32();
        this.FileMetadataOffset = er.ReadInt32();
        this.DataOffset = er.ReadInt32();
      }
    }

    public interface IGarFileType {
      string TypeName { get; }
      IGarSubfile[] Files { get; }
    }

    public interface IGarSubfile {
      string FileName { get; }
      byte[] Bytes { get; }
    }

    private class Gar2FileType : IGarFileType {
      public int FileCount { get; }
      public int FileListOffset { get; }
      public int TypeNameOffset { get; }
      public string TypeName { get; }

      public IGarSubfile[] Files { get; }

      public Gar2FileType(
          IEndianBinaryReader er,
          GarHeader header,
          int fileTypeIndex) {
        er.Position = header.FileTypesOffset + 16 * fileTypeIndex;

        this.FileCount = er.ReadInt32();
        this.FileListOffset = er.ReadInt32();
        this.TypeNameOffset = er.ReadInt32();
        er.ReadInt32();

        er.Position = this.TypeNameOffset;
        this.TypeName = er.ReadStringNT();

        this.Files = new IGarSubfile[Math.Max(0, this.FileCount)];
        for (var i = 0; i < this.FileCount; ++i) {
          this.Files[i] = new Gar2Subfile(er, header, this, i);
        }
      }
    }

    private class Gar2Subfile : IGarSubfile {
      public string FileName { get; }
      public string FullPath { get; }
      public byte[] Bytes { get; }

      public Gar2Subfile(
          IEndianBinaryReader er,
          GarHeader header,
          Gar2FileType fileType,
          int fileInFileTypeIndex) {
        er.Position = fileType.FileListOffset + 4 * fileInFileTypeIndex;
        var fileIndex = er.ReadInt32();

        er.Position = header.FileMetadataOffset + 12 * fileIndex;
        var fileSize = er.ReadInt32();
        var fileNameOffset = er.ReadInt32();
        var fullPathOffset = er.ReadInt32();

        er.Position = fileNameOffset;
        this.FileName = er.ReadStringNT();

        er.Position = fullPathOffset;
        this.FullPath = er.ReadStringNT();

        er.Position = header.DataOffset + 4 * fileIndex;
        var fileOffset = er.ReadInt32();

        er.Position = fileOffset;
        this.Bytes = er.ReadBytes(Math.Max(fileSize, 0));
      }
    }

    private class Gar5FileType : IGarFileType {
      public uint FileCount { get; }
      public int FirstFileIndex { get; }
      public int TypeNameOffset { get; }
      public string TypeName { get; }

      public IGarSubfile[] Files { get; }

      public Gar5FileType(
          IEndianBinaryReader er,
          GarHeader header,
          int fileTypeIndex) {
        er.Position = header.FileTypesOffset + 8 * 4 * fileTypeIndex;

        this.FileCount = er.ReadUInt32();
        er.ReadUInt32();
        this.FirstFileIndex = er.ReadInt32();
        this.TypeNameOffset = er.ReadInt32();
        er.ReadInt32();
        er.ReadUInt32();
        er.ReadUInt32();
        er.ReadUInt32();

        er.Position = this.TypeNameOffset;
        this.TypeName = er.ReadStringNT();

        this.Files = new IGarSubfile[this.FileCount];
        for (var i = 0; i < this.FileCount; ++i) {
          this.Files[i] = new Gar5Subfile(er, header, this, i);
        }
      }
    }

    private class Gar5Subfile : IGarSubfile {
      public string FileName { get; }
      public byte[] Bytes { get; }

      public Gar5Subfile(
          IEndianBinaryReader er,
          GarHeader header,
          Gar5FileType fileType,
          int fileInFileTypeIndex) {
        er.Position = header.FileMetadataOffset +
                      (fileType.FirstFileIndex + fileInFileTypeIndex) * 4 * 4;
        var fileSize = er.ReadInt32();
        var fileOffset = er.ReadUInt32();
        var fileNameOffset = er.ReadInt32();
        var fullPathOffset = er.ReadInt32();

        er.Position = fullPathOffset != -1 ? fullPathOffset : fileNameOffset;
        this.FileName = er.ReadStringNT();

        if (Path.GetExtension(this.FileName) == string.Empty) {
          this.FileName += $".{fileType.TypeName}";
        }

        er.Position = fileOffset;
        this.Bytes = er.ReadBytes(fileSize);
      }
    }
  }
}