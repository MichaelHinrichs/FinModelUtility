using System.Text;

using fin.io;
using fin.log;
using fin.util.asserts;

using uni.util.cmd;


namespace uni.platforms.gcn.tools {
  public class RarcDump2 {
    public bool Run(IFileHierarchyFile rarcFile, bool cleanup) {
      Asserts.True(
          rarcFile.Impl.Exists,
          $"Cannot dump RARC because it does not exist: {rarcFile}");

      var finalDirectoryPath = rarcFile.FullNameWithoutExtension;
      if (Directory.Exists(finalDirectoryPath)) {
        return false;
      }

      if (!MagicTextUtil.Verify(rarcFile, "RARC")) {
        return false;
      }

      var directoryPath = rarcFile.FullName + "_dir";
      if (!Directory.Exists(directoryPath)) {
        var logger = Logging.Create<RarcDump>();
        logger.LogInformation($"Dumping RARC {rarcFile.LocalPath}...");

        // TODO: Is this implementation right? It *seems* to only export the
        // first node in a RARC.
        Files.RunInDirectory(
            rarcFile.Impl.GetParent()!,
            () => {
              ProcessUtil.ExecuteBlockingSilently(
                  GcnToolsConstants.RARCDUMP_EXE,
                  $"\"{rarcFile.FullName}\"");
            });
        Asserts.True(Directory.Exists(directoryPath),
                     $"Directory was not created: {directoryPath}");
      }

      Directory.Move(directoryPath, finalDirectoryPath);
      if (cleanup) {
        rarcFile.Impl.Info.Delete();
      }

      return true;
    }

    // Based on version 1 of rarcdump by thakis.
    // Expanded with information from: http://wiki.tockdom.com/wiki/RARC_(File_Format)
    private bool Impl_(IFileHierarchyFile rarcFile) {
      var directoryPath = rarcFile.FullName + "_dir";
      if (Directory.Exists(directoryPath)) {
        //return false;
      }

      var logger = Logging.Create<RarcDump>();
      using var rarcDumpScope = logger.BeginScope("rarcdump");

      logger.LogInformation($"Dumping ARC {rarcFile.LocalPath}...");
      return this.ReadFile_(rarcFile);
    }

    private bool ReadFile_(IFileHierarchyFile rarcFile) {
      using var er = new EndianBinaryReader(rarcFile.Impl.OpenRead());

      var header = new RarcHeader();
      header.type = er.ReadString(Encoding.ASCII, 4);

      if (header.type != "RARC") {
        return false;
      }

      header.size = er.ReadUInt32();
      header.unknown = er.ReadUInt32();
      header.dataStartOffset = er.ReadUInt32();
      er.ReadUInt32s(header.unknown2);

      header.numNodes = er.ReadUInt32();
      header.firstNodeOffset = er.ReadUInt32();
      header.numDirectories = er.ReadUInt32();
      header.fileEntriesOffset = er.ReadUInt32();
      header.stringTableLength = er.ReadUInt32();
      header.stringTableOffset = er.ReadUInt32();
      er.ReadUInt32s(header.unknown5);


      var cwd = Directory.GetCurrentDirectory();

      var directoryPath = rarcFile.FullName + "_dir";
      {
        var nodes = new RarcNode[header.numNodes];
        for (var i = 0; i < header.numNodes; ++i) {
          nodes[i] = this.GetNode_(er, header, i);
        }

        ;

        foreach (var node in nodes) {
          Directory.SetCurrentDirectory(directoryPath);
          this.DumpNode_(er, node, header);
        }
      }

      Directory.SetCurrentDirectory(cwd);

      return true;
    }

    private RarcNode GetNode_(EndianBinaryReader er, RarcHeader h, int i) {
      var node = new RarcNode();

      node.type = er.ReadString(Encoding.UTF8, 4);

      var fileNameOffset = er.ReadUInt32();
      var expectedFileNameHash = er.ReadUInt16();
      node.numFileEntries = er.ReadUInt16();
      node.firstFileEntryOffset = er.ReadUInt32();

      var position = er.Position;
      {
        er.Position = 0x20 + h.stringTableOffset + fileNameOffset;
        node.fileName = er.ReadStringNT(Encoding.UTF8);

        var actualFileNameHash = 0;
        foreach (var c in node.fileName) {
          actualFileNameHash *= 3;
          actualFileNameHash += (byte) c;
        }
        Asserts.Equal(expectedFileNameHash,
                      actualFileNameHash,
                      "Node did not have the correct hash!");
      }
      { }
      er.Position = position;

      return node;
    }

    private void DumpNode_(EndianBinaryReader er, RarcNode node, RarcHeader h) {
      /*
string nodeName = getString(0x20 + n.filenameOffset + h.stringTableOffset, f);
        _mkdir(nodeName.c_str());
        _chdir(nodeName.c_str());

        for (int i = 0; i < n.numFileEntries; ++i) {
          RarcDump.FileEntry curr = getFileEntry(n.firstFileEntryOffset + i, h, f);

          if (curr.id == 0xFFFF) //subdirectory
          {
            if (curr.filenameOffset != 0 &&
                curr.filenameOffset != 2) //don't go to "." and ".."
              dumpNode(getNode(curr.dataOffset, f), h, f);
          } else //file
          {
            string currName =
                getString(curr.filenameOffset + h.stringTableOffset + 0x20, f);
            cout << nodeName << "/" << currName << endl;
            FILE* dest = fopen(currName.c_str(), "wb");

            u32 read = 0;
            u8 buff[1024];
            fseek(f, curr.dataOffset + h.dataStartOffset + 0x20, SEEK_SET);
            while (read < curr.dataSize) {
              int r = fread(buff, 1, min(1024, curr.dataSize - read), f);
              fwrite(buff, 1, r, dest);
              read += r;
            }
            fclose(dest);
          }
        }

        _chdir("..");       */
    }

    private FileEntry GetFileEntry_(
        EndianBinaryReader er,
        RarcHeader header,
        RarcNode node,
        int i) {
      er.Position = 0x20 +
                    header.fileEntriesOffset +
                    node.firstFileEntryOffset +
                    i * 20;

      var fileEntry = new FileEntry();
      fileEntry.id = er.ReadUInt16();
      fileEntry.unknown = er.ReadUInt16();
      fileEntry.unknown2 = er.ReadUInt16();
      fileEntry.filenameOffset = er.ReadUInt16();

      fileEntry.dataOffset = er.ReadUInt32();

      fileEntry.dataSize = er.ReadUInt32();
      fileEntry.zero = er.ReadUInt32();

      return fileEntry;
    }

    public class RarcHeader {
      public string type; //'RARC'
      public uint size; //size of the file
      public uint unknown;
      public uint dataStartOffset; //where does the actual data start?
      public uint[] unknown2 = new uint[4];

      public uint numNodes;
      public uint firstNodeOffset;
      public uint numDirectories;
      public uint fileEntriesOffset;
      public uint stringTableLength;
      public uint stringTableOffset; //where is the string table stored?
      public uint[] unknown5 = new uint[2];
    }

    public class RarcNode {
      public string type;
      public ushort numFileEntries; //how many files belong to this node?
      public uint firstFileEntryOffset;
      public IList<FileEntry> fileEntries;

      public string fileName;
    }

    public class FileEntry {
      public ushort
          id; //file id. If this is 0xFFFF, then this entry is a subdirectory link

      public ushort unknown;
      public ushort unknown2;
      public ushort filenameOffset; //file/subdir name, offset into string table

      public uint
          dataOffset; //offset to file data (for subdirs: index of Node representing the subdir)

      public uint dataSize; //size of data
      public uint zero; //seems to be always '0'
    };
  }
}