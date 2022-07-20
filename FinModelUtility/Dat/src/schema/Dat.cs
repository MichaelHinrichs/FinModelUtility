using fin.util.asserts;

using schema;


namespace dat.schema {
  public class Dat : IDeserializable {
    public List<JObj> RootBoneStructs { get; } = new();

    public void Read(EndianBinaryReader er) {
      var fileHeader = er.ReadNew<FileHeader>();
      Asserts.Equal(er.Length, fileHeader.FileSize);

      var dataBlockOffset = 0x20;
      var relocationTableOffset = dataBlockOffset + fileHeader.DataBlockSize;
      var rootNodeOffset =
          relocationTableOffset + 4 * fileHeader.RelocationTableCount;
      var referenceNodeOffset = rootNodeOffset + 8 * fileHeader.RootNodeCount;
      var stringTableOffset =
          referenceNodeOffset + 8 * fileHeader.ReferenceNodeCount;

      // Reads relocation table
      var validOffsets = new HashSet<uint>();
      for (var i = 0; i < fileHeader.RelocationTableCount; ++i) {
        er.Position = relocationTableOffset + 4 * i;
        var relocationTableEntryOffset = er.ReadUInt32();

        er.Position = dataBlockOffset + relocationTableEntryOffset;
        var relocationTableValue = er.ReadUInt32();

        validOffsets.Add(relocationTableValue);
      }

      var assertNullOrValidPointer = (uint pointer) => {
        if (pointer == 0) {
          return false;
        }

        if (!validOffsets.Contains(pointer)) {
          ;
        }

        Asserts.True(validOffsets.Contains(pointer));
        return true;
      };

      var tryToResolvePointer = (uint pointer, out uint offset) => {
        if (assertNullOrValidPointer(pointer)) {
          offset = pointer;
          return true;
        }

        offset = 0;
        return false;
      };

      // Reads root nodes
      er.Position = rootNodeOffset;
      var rootNodes = new RootNode[fileHeader.RootNodeCount];
      for (var i = 0; i < fileHeader.RootNodeCount; i++) {
        var rootNode = rootNodes[i] = new RootNode();
        rootNode.Data.Read(er);

        er.Subread(stringTableOffset + rootNode.Data.StringOffset,
                   ser => { rootNode.Name = ser.ReadStringNT(); });
      }

      // TODO: Handle reference nodes

      // Reads root bone structures
      var jobjQueue =
          new Queue<(RootNode rootNode, JObj? parentBoneStruct, uint dataOffset
              )>();
      foreach (var rootNode in rootNodes) {
        var rootNodeName = rootNode.Name;

        // The data for each root node is not necessarily a joint--can be a texture, material, animation, etc.
        if (rootNodeName.EndsWith("_Share_joint")) {
          var boneDataOffset = rootNode.Data.DataOffset;
          if (boneDataOffset != 0) {
            jobjQueue.Enqueue((rootNode, null, boneDataOffset));
          }
        } else if (rootNodeName.EndsWith("_TopN_joint")) {
          ;
        } else if (rootNodeName.EndsWith("_matanim_joint")) {
          ;
        } else if (rootNodeName.EndsWith("_image")) {
          ;
        } else if (rootNodeName.EndsWith("_scene_data")) {
          ;
        } else if (rootNodeName.EndsWith("_scene_modelset")) {
          ;
        } else if (rootNodeName.EndsWith("_tlut")) {
          ;
        } else if (rootNodeName.EndsWith("_tlut_desc")) {
          ;
        } else {
          ;
        }
      }

      this.RootBoneStructs.Clear();
      while (jobjQueue.Count > 0) {
        var (rootNode, parentBoneStruct, dataOffset) = jobjQueue.Dequeue();

        var boneStruct = new JObj();
        var data = boneStruct.Data;

        if (parentBoneStruct == null) {
          this.RootBoneStructs.Add(boneStruct);
        } else {
          parentBoneStruct.Children.Add(boneStruct);
        }

        er.Position = dataBlockOffset + dataOffset;
        data.Read(er);

        if (tryToResolvePointer(data.StringOffset, out var boneNameOffset)) {
          if (stringTableOffset + boneNameOffset >= er.Length) {
            ;
          }

          er.Position = stringTableOffset + boneNameOffset;
          boneStruct.Name = er.ReadStringNT();
        }

        var objectStructOffset = boneStruct.Data.ObjectStructOffset;
        while (assertNullOrValidPointer(objectStructOffset)) {
          var objectStruct = new ObjectStruct();
          boneStruct.ObjectStructs.Add(objectStruct);

          er.Position = dataBlockOffset + objectStructOffset;
          objectStruct.Data.Read(er);

          if (tryToResolvePointer(objectStruct.Data.StringOffset,
                                  out var objectNameOffset)) {
            er.Position = stringTableOffset + objectNameOffset;
            objectStruct.Name = er.ReadStringNT();
          }

          objectStructOffset = objectStruct.Data.NextObjectOffset;
        }

        if (tryToResolvePointer(boneStruct.Data.FirstChildBoneOffset,
                                out var firstChildOffset)) {
          jobjQueue.Enqueue((rootNode, boneStruct, firstChildOffset));
        }

        if (tryToResolvePointer(boneStruct.Data.NextSiblingBoneOffset,
                                out var nextSiblingOffset)) {
          jobjQueue.Enqueue((rootNode, parentBoneStruct, nextSiblingOffset));
        }
      }
    }
  }

  [Schema]
  public partial class FileHeader : IBiSerializable {
    public uint FileSize { get; set; }
    public uint DataBlockSize { get; set; }

    public uint RelocationTableCount { get; set; }
    public uint RootNodeCount { get; set; }
    public uint ReferenceNodeCount { get; set; }

    [StringLengthSource(4)] public string Version { get; set; }
    public uint Padding1 { get; set; }
    public uint Padding2 { get; set; }
  }

  [Schema]
  public partial class RootNodeData : IBiSerializable {
    public uint DataOffset { get; set; }
    public uint StringOffset { get; set; }
  }

  public class RootNode {
    public RootNodeData Data { get; } = new();
    public string Name { get; set; }
  }

  public class ObjectStruct {
    public ObjectStructData Data { get; } = new();
    public string Name { get; set; }
  }

  [Schema]
  public partial class ObjectStructData : IBiSerializable {
    public uint StringOffset { get; set; }
    public uint NextObjectOffset { get; set; }
    public uint MaterialStructOffset { get; set; }
    public uint MeshStructOffset { get; set; }
  }
}