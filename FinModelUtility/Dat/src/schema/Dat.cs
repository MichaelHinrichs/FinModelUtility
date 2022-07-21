using fin.io;
using fin.util.asserts;

using gx;

using schema;


namespace dat.schema {
  // AObj: animation
  // DObj: ?
  // FObj: animation track for a single joint?
  // JObj: joint (bone)
  // MObj: material
  // PObj: primitive
  // TObj: texture

  public class Dat : IDeserializable {
    private readonly List<RootNode> rootNodes_ = new();
    private readonly HashSet<uint> validOffsets_ = new();

    private uint dataBlockOffset_;
    private uint relocationTableOffset_;
    private uint rootNodeOffset_;
    private uint referenceNodeOffset_;
    private uint stringTableOffset_;

    public List<JObj> RootJObjs { get; } = new();

    public void Read(EndianBinaryReader er) {
      var fileHeader = er.ReadNew<FileHeader>();
      Asserts.Equal(er.Length, fileHeader.FileSize);

      this.dataBlockOffset_ = 0x20;
      this.relocationTableOffset_ =
          this.dataBlockOffset_ + fileHeader.DataBlockSize;
      this.rootNodeOffset_ =
          this.relocationTableOffset_ + 4 * fileHeader.RelocationTableCount;
      this.referenceNodeOffset_ =
          this.rootNodeOffset_ + 8 * fileHeader.RootNodeCount;
      this.stringTableOffset_ =
          this.referenceNodeOffset_ + 8 * fileHeader.ReferenceNodeCount;

      // Reads relocation table
      this.validOffsets_.Clear();
      for (var i = 0; i < fileHeader.RelocationTableCount; ++i) {
        er.Position = this.relocationTableOffset_ + 4 * i;
        var relocationTableEntryOffset = er.ReadUInt32();

        er.Position = this.dataBlockOffset_ + relocationTableEntryOffset;
        var relocationTableValue = er.ReadUInt32();

        this.validOffsets_.Add(relocationTableValue);
      }

      // Reads root nodes
      this.rootNodes_.Clear();
      for (var i = 0; i < fileHeader.RootNodeCount; i++) {
        er.Position = this.rootNodeOffset_ + 8 * i;

        var rootNode = new RootNode();
        rootNode.Data.Read(er);

        er.Subread(this.stringTableOffset_ + rootNode.Data.StringOffset,
                   ser => { rootNode.Name = ser.ReadStringNT(); });

        this.rootNodes_.Add(rootNode);
      }

      // TODO: Handle reference nodes

      // Reads root bone structures
      foreach (var rootNode in this.rootNodes_) {
        if (rootNode.Type == RootNodeType.Undefined) {
          ;
        }
      }

      this.ReadJObs_(er);
    }

    private bool AssertNullOrValidPointer_(uint pointer) {
      if (pointer == 0) {
        return false;
      }

      if (!this.validOffsets_.Contains(pointer)) {
        ;
      }

      Asserts.True(this.validOffsets_.Contains(pointer));
      return true;
    }

    private void ReadJObs_(EndianBinaryReader er) {
      var jObjQueue =
          new Queue<(
              RootNode rootNode,
              JObj? parentJObj,
              uint jObjDataOffset
              )>();

      foreach (var rootNode in this.rootNodes_.Where(
                   rootNode => rootNode.Type == RootNodeType.JObj)) {
        jObjQueue.Enqueue((rootNode, null, rootNode.Data.DataOffset));
      }

      this.RootJObjs.Clear();
      while (jObjQueue.Count > 0) {
        var (rootNode, parentJObj, jObjDataOffset) = jObjQueue.Dequeue();

        var jObj = new JObj();
        var jObjData = jObj.Data;

        if (parentJObj == null) {
          this.RootJObjs.Add(jObj);
        } else {
          parentJObj.Children.Add(jObj);
        }

        er.Position = this.dataBlockOffset_ + jObjDataOffset;
        jObjData.Read(er);

        var jObjNameOffset = jObjData.StringOffset;
        if (this.AssertNullOrValidPointer_(jObjNameOffset)) {
          if (this.stringTableOffset_ + jObjNameOffset >= er.Length) {
            ;
          }

          er.Position = this.stringTableOffset_ + jObjNameOffset;
          jObj.Name = er.ReadStringNT();
        }

        var objectStructOffset = jObj.Data.ObjectStructOffset;
        while (this.AssertNullOrValidPointer_(objectStructOffset)) {
          var objectStruct = new ObjectStruct();
          jObj.ObjectStructs.Add(objectStruct);

          er.Position = this.dataBlockOffset_ + objectStructOffset;
          objectStruct.Data.Read(er);

          var pObjDataOffset = objectStruct.Data.MeshStructOffset;
          while (this.AssertNullOrValidPointer_(pObjDataOffset)) {
            er.Position = this.dataBlockOffset_ + pObjDataOffset;

            var pObj = new PObj();
            var pObjData = pObj.Data;
            pObjData.Read(er);

            var pObjType = pObjData.Flags & PObjFlags.OBJTYPE_SKIN;
            switch (pObjType) {
              case PObjFlags.OBJTYPE_SKIN: {
                er.Position = this.dataBlockOffset_ +
                              pObjData.VertexDescriptorListOffset;

                while (true) {
                  var vertexDescriptor = new VertexDescriptor();
                  var vertexDescriptorData = vertexDescriptor.Data;
                  vertexDescriptorData.Read(er);

                  if (vertexDescriptorData.Attribute == GxAttribute.NULL) {
                    break;
                  }

                  pObj.VertexDescriptors.Add(vertexDescriptor);
                }

                break;
              }
              case PObjFlags.OBJTYPE_ENVELOPE: {
                break;
              }
              case PObjFlags.OBJTYPE_SHAPEANIM: {
                break;
              }
              default: throw new NotImplementedException();
            }

            pObjDataOffset = pObjData.NextPObjOffset;
          }

          objectStructOffset = objectStruct.Data.NextObjectOffset;
        }

        var firstChildOffset = jObj.Data.FirstChildBoneOffset;
        if (this.AssertNullOrValidPointer_(firstChildOffset)) {
          jObjQueue.Enqueue((rootNode, jObj, firstChildOffset));
        }

        var nextSiblingOffset = jObj.Data.NextSiblingBoneOffset;
        if (this.AssertNullOrValidPointer_(nextSiblingOffset)) {
          jObjQueue.Enqueue((rootNode, parentJObj, nextSiblingOffset));
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

  public enum RootNodeType {
    Undefined,
    JObj,

    TOPN_JOINT,
    MATANIM_JOINT,
    IMAGE,
    SCENE_DATA,
    SCENE_MODELSET,
    TLUT,
    TLUT_DESC,
  }

  public class RootNode {
    private string name_;

    public RootNodeData Data { get; } = new();

    public string Name {
      get => this.name_;
      set => this.Type = RootNode.GetTypeFromName_(this.name_ = value);
    }

    public RootNodeType Type { get; private set; }

    private static RootNodeType GetTypeFromName_(string name) {
      if (name.EndsWith("_Share_joint")) {
        return RootNodeType.JObj;
      }
      if (name.EndsWith("_TopN_joint")) {
        return RootNodeType.TOPN_JOINT;
      }
      if (name.EndsWith("_matanim_joint")) {
        return RootNodeType.MATANIM_JOINT;
      }
      if (name.EndsWith("_image")) {
        return RootNodeType.IMAGE;
      }
      if (name.EndsWith("_scene_data")) {
        return RootNodeType.SCENE_DATA;
      }
      if (name.EndsWith("_scene_modelset")) {
        return RootNodeType.SCENE_MODELSET;
      }
      if (name.EndsWith("_tlut")) {
        return RootNodeType.TLUT;
      }
      if (name.EndsWith("_tlut_desc")) {
        return RootNodeType.TLUT_DESC;
      }
      return RootNodeType.Undefined;
    }
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

  [Flags]
  public enum PObjFlags : ushort {
    OBJTYPE_SKIN = 0 << 12,
    OBJTYPE_SHAPEANIM = 1 << 12,
    OBJTYPE_ENVELOPE = 2 << 12,
    OBJTYPE_MASK = 0x3000,

    CULLFRONT = 1 << 14,
    CULLBACK = 1 << 15,
  }

  [Schema]
  public partial class PObjData : IBiSerializable {
    public uint StringOffset { get; set; }
    public uint NextPObjOffset { get; set; }
    public uint VertexDescriptorListOffset { get; set; }
    public PObjFlags Flags { get; set; }
    public ushort nDisp { get; set; }
    public ushort DisplayOffset { get; set; }
    public ushort ContentsOffset { get; set; }
  }

  public class PObj {
    public PObjData Data { get; set; } = new();
    public List<VertexDescriptor> VertexDescriptors { get; } = new();
  }

  [Schema]
  public partial class VertexDescriptorData : IBiSerializable {
    [Format(SchemaNumberType.UINT32)] public GxAttribute Attribute { get; set; }

    [Format(SchemaNumberType.UINT32)]
    public GxAttributeType AttributeType { get; set; }

    [Format(SchemaNumberType.UINT32)]
    public GxComponentCount CompCnt { get; set; }

    [Format(SchemaNumberType.UINT32)]
    public GxComponentType CompType { get; set; }
    public ushort CompShift { get; set; }

    public ushort Stride { get; set; }

    public uint ArrayOffset { get; set; }
  }

  public class VertexDescriptor {
    public VertexDescriptorData Data { get; } = new();
  }
}