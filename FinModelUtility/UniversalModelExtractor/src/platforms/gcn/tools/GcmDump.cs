using System.Text;

using fin.data.stack;
using fin.io;
using fin.util.asserts;

using schema.binary;

namespace uni.platforms.gcn.tools {
  /// <summary>
  ///   Shamelessly ported from version 1.0 (20050213) of gcmdump by by thakis.
  /// </summary>
  public partial class GcmDump {
    // TODO: Add support for NKit ISOs

    public bool Run(IFile romFile, out IFileHierarchy hierarchy)
      => Run(romFile.OpenRead(),
             romFile.FullNameWithoutExtension,
             out hierarchy);

    public bool Run(Stream romStream,
                    string directoryPath,
                    out IFileHierarchy hierarchy) {
      var didChange = false;

      var directory = new FinDirectory(directoryPath);
      if (!directory.Exists) {
        didChange = true;

        this.DumpRom_(romStream, directoryPath);
        Asserts.True(directory.Exists,
                     $"Directory was not created: {directory}");
      }

      hierarchy = new FileHierarchy(directory);
      return didChange;
    }

    private void DumpRom_(Stream romStream, string directoryPath) {
      using var er = new EndianBinaryReader(romStream, Endianness.BigEndian);

      var diskHeader = er.ReadNew<DiskHeader>();
      var fileEntries = this.ReadFileSystemTable_(er, diskHeader);

      //copy file system to harddisk

      //for now, dump directory structure
      var rootDirectory = new FinDirectory(directoryPath);
      rootDirectory.Create();

      var directories = new IDirectory[fileEntries.Count];
      directories[0] = rootDirectory;

      var directoryStack =
          new FinStack<(IDirectory directory, uint lastChildIndex)>(
              (rootDirectory, (uint) fileEntries.Count));

      var fileTableOffset = 12 * fileEntries.Count;
      for (int i = 1; i < fileEntries.Count; ++i) {
        var e = fileEntries[i];

        // Pop to reach parent directory
        while (i >= directoryStack.Top.lastChildIndex) {
          directoryStack.Pop();
        }

        // Get name
        er.Position = diskHeader.FileSystemTableOffset + fileTableOffset +
                      e.FileNameOffset;
        var name = er.ReadStringNT(Encoding.UTF8);

        // Push new directory
        if (e.IsDirectory) {
          var parentDir = directories[e.FileOrParentOffset];
          var childDir = parentDir.GetSubdir(name, true);

          directories[i] = childDir;
          directoryStack.Push((childDir, e.FileLengthOrNextOffset));
        }
        // Export file
        else {
          var file =
              new FinFile(
                  Path.Combine(directoryStack.Top.directory.FullName, name));

          er.Position = e.FileOrParentOffset;

          using var fw = file.OpenWrite();
          fw.Write(er.ReadBytes(e.FileLengthOrNextOffset));
        }
      }
    }

    private IList<FileEntry> ReadFileSystemTable_(IEndianBinaryReader er,
                                                  DiskHeader diskHeader) {
      var entries = new List<FileEntry>();

      //read files
      er.Position = diskHeader.FileSystemTableOffset;
      uint numFiles = 1;
      for (int i = 0; i < numFiles; ++i) {
        var entry = er.ReadNew<FileEntry>();
        entries.Add(entry);
        if (i == 0) {
          numFiles = entry.FileLengthOrNextOffset;
        }
      }

      return entries;
    }

    [BinarySchema]
    private partial class Ids : IBiSerializable {
      public byte ConsoleId { get; set; }
      public ushort GameId { get; set; }
      public byte CountryId { get; set; }
      public ushort MakerId { get; set; }
    }

    [BinarySchema]
    private partial class DiskHeader : IBiSerializable {
      public Ids Ids { get; } = new();
      public byte DiskId { get; set; }
      public byte Version { get; set; }
      public byte AudioStreaming { get; set; }
      public byte StreamBufferSize { get; set; }

      [ArrayLengthSource(0x12)]
      public byte[] Unused { get; set; }

      [StringLengthSource(4)]
      public string DvdMagicWord { get; set; }

      [StringLengthSource(0x3e0)]
      public string GameName { get; set; }

      public uint DebugMonitorOffset { get; set; }
      public uint DebugLoadAddress { get; set; }

      [ArrayLengthSource(0x18)]
      public byte[] Unused2 { get; set; }

      public uint DolOffset { get; set; }

      public uint FileSystemTableOffset { get; set; }
      public uint FileSystemTableSize { get; set; }
      public uint FileSystemTableMaximumSize { get; set; }

      public uint UserPosition { get; set; }
      public uint UserLength { get; set; }

      public uint Unknown { get; set; }
      public uint Unused3 { get; set; }
    }

    [BinarySchema]
    private partial class FileEntry : IBiSerializable {
      [IntegerFormat(SchemaIntegerType.BYTE)]
      public bool IsDirectory { get; set; }

      [IntegerFormat(SchemaIntegerType.UINT24)]
      public uint FileNameOffset { get; set; }

      public uint FileOrParentOffset { get; set; }
      public uint FileLengthOrNextOffset { get; set; }
    }
  }
}