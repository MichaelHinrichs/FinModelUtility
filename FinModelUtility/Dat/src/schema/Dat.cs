using fin.util.asserts;
using fin.util.enums;

using gx;

using schema.binary;
using schema.binary.attributes.ignore;


namespace dat.schema {
  // AObj: animation
  // CObj: camera
  // DObj: ?
  // FObj: keyframe descriptor
  // IObj: image
  // JObj: joint (bone)
  // MObj: material
  // PObj: primitive
  // SObj: Scene object
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

    public void Read(IEndianBinaryReader er) {
      Dat.vertexDescriptorValue_ = (uint) 0;

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

    private static uint vertexDescriptorValue_;

    private void ReadJObs_(IEndianBinaryReader er) {
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

        var jObjFlags = jObjData.Flags;
        var isSpline = jObjFlags.CheckFlag(JObjFlags.SPLINE);
        var isParticleJoint = jObjFlags.CheckFlag(JObjFlags.PTCL);
        var isDObj = !isSpline && !isParticleJoint;

        if (isDObj) {
          this.ReadDObjIntoJObj_(er, jObj, jObj.Data.ObjectStructOffset);
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

    private void ReadDObjIntoJObj_(IEndianBinaryReader er,
                                   JObj jObj,
                                   uint objectStructOffset) {
      if (!this.AssertNullOrValidPointer_(objectStructOffset)) {
        return;
      }

      var dObj = new DObj();
      jObj.DObjs.Add(dObj);

      er.Position = this.dataBlockOffset_ + objectStructOffset;
      dObj.Data.Read(er);

      this.ReadPObjIntoDObj_(er, dObj, dObj.Data.MeshStructOffset);

      this.ReadDObjIntoJObj_(er, jObj, dObj.Data.NextObjectOffset);
    }

    private void ReadPObjIntoDObj_(IEndianBinaryReader er,
                                   DObj dObj,
                                   uint pObjDataOffset) {
      if (!this.AssertNullOrValidPointer_(pObjDataOffset)) {
        return;
      }

      er.Position = this.dataBlockOffset_ + pObjDataOffset;

      var pObj = new PObj();
      dObj.PObjs.Add(pObj);

      var pObjData = pObj.Data;
      pObjData.Read(er);

      var pObjType = pObjData.Flags & PObjFlags.OBJTYPE_SKIN;
      switch (pObjType) {
        case PObjFlags.OBJTYPE_SKIN: {
          er.Position = this.dataBlockOffset_ +
                        pObjData.VertexDescriptorListOffset;

          // Reads vertex descriptors
          while (true) {
            var vertexDescriptor = new VertexDescriptor();
            var vertexDescriptorData = vertexDescriptor.Data;
            vertexDescriptorData.Read(er);

            if (vertexDescriptorData.Attribute == GxAttribute.NULL) {
              break;
            }

            pObj.VertexDescriptors.Add(vertexDescriptor);
          }

          var startingOffset =
              er.Position =
                  this.dataBlockOffset_ + pObjData.DisplayOffset;

          // Reads display list
          while (er.Position - startingOffset < pObjData.nDisp * 32) {
            var opcode = (GxOpcode) er.ReadByte();
            if (opcode == GxOpcode.NOP) {
              break;
            }

            switch (opcode) {
              case GxOpcode.LOAD_CP_REG: {
                var command = er.ReadByte();
                var value = er.ReadUInt32();

                if (command == 0x50) {
                  Dat.vertexDescriptorValue_ &= ~((uint) 0x1FFFF);
                  Dat.vertexDescriptorValue_ |= value;
                } else if (command == 0x60) {
                  value <<= 17;
                  Dat.vertexDescriptorValue_ &= 0x1FFFF;
                  Dat.vertexDescriptorValue_ |= value;
                } else {
                  throw new NotImplementedException();
                }

                break;
              }
              case GxOpcode.LOAD_XF_REG: {
                var lengthMinusOne = er.ReadUInt16();
                var length = lengthMinusOne + 1;

                // http://hitmen.c02.at/files/yagcd/yagcd/chap5.html#sec5.11.4
                var firstXfRegisterAddress = er.ReadUInt16();

                var values = er.ReadUInt32s(length);
                // TODO: Implement
                break;
              }
              case GxOpcode.DRAW_TRIANGLES:
              case GxOpcode.DRAW_QUADS:
              case GxOpcode.DRAW_TRIANGLE_STRIP: {
                var vertexCount = er.ReadUInt16();

                for (var i = 0; i < vertexCount; ++i) {
                  foreach (var vertexDescriptor in
                           pObj.VertexDescriptors) {
                    var vertexAttribute = vertexDescriptor.Data.Attribute;
                    var vertexFormat =
                        vertexDescriptor.Data.AttributeType;

                    var value = vertexFormat switch {
                        GxAttributeType.DIRECT => er.ReadByte(),
                        GxAttributeType.INDEX_8 => er.ReadByte(),
                        GxAttributeType.INDEX_16 => er.ReadUInt16(),
                        _ => throw new NotImplementedException(),
                    };

                    switch (vertexAttribute) {
                      case GxAttribute.PNMTXIDX: {
                        Asserts.Equal(0, value % 3);
                        value /= 3;
                        break;
                      }
                      case GxAttribute.POS: {
                        break;
                      }
                      case GxAttribute.NRM: {
                        break;
                      }
                    }
                  }
                }

                break;
              }
              default: {
                break;
              }
            }
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

      this.ReadPObjIntoDObj_(er, dObj, pObjData.NextPObjOffset);
    }
  }

  [BinarySchema]
  public partial class FileHeader : IBiSerializable {
    public uint FileSize { get; set; }
    public uint DataBlockSize { get; set; }

    public uint RelocationTableCount { get; set; }
    public uint RootNodeCount { get; set; }
    public uint ReferenceNodeCount { get; set; }

    [StringLengthSource(4)]
    public string Version { get; set; }

    public uint Padding1 { get; set; }
    public uint Padding2 { get; set; }


    [Ignore]
    public uint DataBlockOffset => 0x20;

    [Ignore]
    public uint RelocationTableOffset => DataBlockOffset + DataBlockSize;

    [Ignore]
    public uint RootNodeOffset =>
        RelocationTableOffset + 4 * RelocationTableCount;

    [Ignore]
    public uint ReferenceNodeOffset => RootNodeOffset + 8 * RootNodeCount;

    [Ignore]
    public uint StringTableOffset =>
        ReferenceNodeOffset + 8 * ReferenceNodeCount;
  }

  [BinarySchema]
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
      // TODO: Use flags for this instead
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

  [BinarySchema]
  public partial class VertexDescriptorData : IBiSerializable {
    public GxAttribute Attribute { get; set; }

    [IntegerFormat(SchemaIntegerType.UINT32)]
    public GxAttributeType AttributeType { get; set; }

    [IntegerFormat(SchemaIntegerType.UINT32)]
    public GxComponentCount CompCnt { get; set; }

    [IntegerFormat(SchemaIntegerType.UINT32)]
    public GxComponentType CompType { get; set; }

    public byte CompShift { get; set; }

    public byte Padding { get; set; }

    public ushort Stride { get; set; }

    public uint ArrayOffset { get; set; }
  }

  public class VertexDescriptor {
    public VertexDescriptorData Data { get; } = new();
  }
}