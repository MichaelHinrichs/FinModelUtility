using fin.util.asserts;

using schema;


namespace dat.schema {
  public class Dat : IDeserializable {
    public List<BoneStruct> RootBoneStructs { get; } = new();

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

      // Reads root nodes
      er.Position = rootNodeOffset;
      var rootNodes = new RootNode[fileHeader.RootNodeCount];
      for (var i = 0; i < fileHeader.RootNodeCount; i++) {
        var rootNode = rootNodes[i] = new RootNode();
        rootNode.Data.Read(er);

        er.Subread(stringTableOffset + rootNode.Data.StringOffset,
                   ser => { rootNode.Name = ser.ReadStringNT(); });
      }

      // Reads root bone structures
      var boneStructQueue = new Queue<(BoneStruct? parentBoneStruct, uint dataOffset)>();
      foreach (var rootNode in rootNodes) {
        var boneDataOffset = rootNode.Data.DataOffset;
        if (boneDataOffset != 0) {
          boneStructQueue.Enqueue((null, boneDataOffset));
        }
      }

      this.RootBoneStructs.Clear();
      while (boneStructQueue.Count > 0) {
        var (parentBoneStruct, dataOffset) = boneStructQueue.Dequeue();

        var boneStruct = new BoneStruct();
        var data = boneStruct.Data;

        if (parentBoneStruct == null) {
          this.RootBoneStructs.Add(boneStruct);
        } else {
          parentBoneStruct.Children.Add(boneStruct);
        }

        er.Position = dataBlockOffset + dataOffset;
        data.Read(er);

        var boneNameOffset = data.StringOffset;
        if (boneNameOffset != 0) {
          er.Position = stringTableOffset + boneStruct.Data.StringOffset;
          boneStruct.Name = er.ReadStringNT();
        }

        var objectStructOffset = boneStruct.Data.ObjectStructOffset;
        while (objectStructOffset != 0) {
          var objectStruct = new ObjectStruct();
          boneStruct.ObjectStructs.Add(objectStruct);

          er.Position = dataBlockOffset + objectStructOffset;
          objectStruct.Data.Read(er);

          var objectNameOffset = objectStruct.Data.StringOffset;
          if (objectNameOffset != 0) {
            er.Position = stringTableOffset + objectNameOffset;
            objectStruct.Name = er.ReadStringNT();
          }

          objectStructOffset = objectStruct.Data.NextObjectOffset;
        }

        var firstChildOffset = boneStruct.Data.FirstChildBoneOffset;
        if (firstChildOffset != 0) {
          boneStructQueue.Enqueue((boneStruct, firstChildOffset));
        }

        var nextSiblingOffset = boneStruct.Data.NextSiblingBoneOffset;
        if (nextSiblingOffset != 0) {
          boneStructQueue.Enqueue((parentBoneStruct, nextSiblingOffset));
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

    [StringLengthSource(4)] public string Unknown0 { get; set; }
    public uint Unknown1 { get; set; }
    public uint Unknown2 { get; set; }
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

  [Schema]
  public partial class BoneStructData : IBiSerializable {
    public uint StringOffset { get; set; }
    public uint UnknownFlags { get; set; }
    public uint FirstChildBoneOffset { get; set; }
    public uint NextSiblingBoneOffset { get; set; }
    public uint ObjectStructOffset { get; set; }
    public float RotationRadiansX { get; set; }
    public float RotationRadiansY { get; set; }
    public float RotationRadiansZ { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float ScaleZ { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public uint InverseBindMatrixOffset { get; set; }
    public uint UnknownPointer { get; set; }
  }

  public class BoneStruct {
    public BoneStructData Data { get; } = new();

    public string Name { get; set; }
    public List<ObjectStruct> ObjectStructs { get; } = new();

    public List<BoneStruct> Children { get; } = new();
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