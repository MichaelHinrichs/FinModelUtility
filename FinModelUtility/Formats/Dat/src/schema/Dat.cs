using System.Numerics;

using CommunityToolkit.HighPerformance;

using fin.color;
using fin.util.asserts;

using gx;

using schema.binary;
using schema.binary.attributes;

namespace dat.schema {
  // AObj: animation
  // CObj: camera
  // DObj: ?
  // FObj: keyframe descriptor
  // IObj: image
  // JObj: joint (bone)
  // SObj: Scene object

  /// <summary>
  ///   References:
  ///     - https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/DAT.cs
  /// </summary>
  public class Dat : IBinaryDeserializable {
    private readonly List<RootNode> rootNodes_ = new();
    private readonly HashSet<uint> validOffsets_ = new();

    private uint dataBlockOffset_;
    private uint relocationTableOffset_;
    private uint rootNodeOffset_;
    private uint referenceNodeOffset_;
    private uint stringTableOffset_;

    public List<JObj> RootJObjs { get; } = new();

    public void Read(IBinaryReader br) {
      this.VertexDescriptorValue = (uint) 0;

      var fileHeader = br.ReadNew<FileHeader>();
      Asserts.Equal(br.Length, fileHeader.FileSize);

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
        br.Position = this.relocationTableOffset_ + 4 * i;
        var relocationTableEntryOffset = br.ReadUInt32();

        br.Position = this.dataBlockOffset_ + relocationTableEntryOffset;
        var relocationTableValue = br.ReadUInt32();

        this.validOffsets_.Add(relocationTableValue);
      }

      // Reads root nodes
      this.rootNodes_.Clear();
      for (var i = 0; i < fileHeader.RootNodeCount; i++) {
        br.Position = this.rootNodeOffset_ + 8 * i;

        var rootNode = new RootNode();
        rootNode.Data.Read(br);

        br.SubreadAt(this.stringTableOffset_ + rootNode.Data.StringOffset,
                     sbr => { rootNode.Name = sbr.ReadStringNT(); });

        this.rootNodes_.Add(rootNode);
      }

      // TODO: Handle reference nodes

      // Reads root bone structures
      foreach (var rootNode in this.rootNodes_) {
        if (rootNode.Type == RootNodeType.Undefined) {
          ;
        }
      }

      this.ReadJObs_(br);
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

    public uint VertexDescriptorValue { get; set; }

    private void ReadJObs_(IBinaryReader br) {
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

      br.Position = this.dataBlockOffset_;
      br.PushLocalSpace();

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

        br.Position = jObjDataOffset;
        jObjData.Read(br);

        var jObjNameOffset = jObjData.StringOffset;
        if (this.AssertNullOrValidPointer_(jObjNameOffset)) {
          if (this.stringTableOffset_ + jObjNameOffset >= br.Length) {
            ;
          }

          br.Position = this.stringTableOffset_ + jObjNameOffset;
          jObj.Name = br.ReadStringNT();
        }

        if (jObj.Data.FirstDObjOffset != 0) {
          br.Position = jObj.Data.FirstDObjOffset;
          jObj.FirstDObj = new DObj(this);
          jObj.FirstDObj.Read(br);
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

      br.PopLocalSpace();
    }
  }

  public static class BinaryReaderExtensions {
    public static Vector2 ReadVector2(this IBinaryReader br,
                                      VertexDescriptor descriptor) {
      var vec2 = new Vector2();
      br.ReadIntoVector(descriptor,
                        new Span<Vector2>(ref vec2).Cast<Vector2, float>());
      return vec2;
    }

    public static Vector3 ReadVector3(this IBinaryReader br,
                                      VertexDescriptor descriptor) {
      var vec3 = new Vector3();
      br.ReadIntoVector(descriptor,
                        new Span<Vector3>(ref vec3).Cast<Vector3, float>());
      return vec3;
    }

    public static Vector4 ReadVector4(this IBinaryReader br,
                                      VertexDescriptor descriptor) {
      var vec4 = new Vector4();
      br.ReadIntoVector(descriptor,
                        new Span<Vector4>(ref vec4).Cast<Vector4, float>());
      return vec4;
    }

    public static void ReadIntoVector(this IBinaryReader br,
                                      VertexDescriptor descriptor,
                                      Span<float> floats) {
      Asserts.True(floats.Length >= descriptor.ComponentCount);

      var scaleMultiplier = 1f / (1 << descriptor.Scale);
      for (var i = 0; i < descriptor.ComponentCount; ++i) {
        floats[i] = scaleMultiplier * descriptor.AxesComponentType switch {
            GxComponentType.U8  => br.ReadByte(),
            GxComponentType.S8  => br.ReadByte(),
            GxComponentType.U16 => br.ReadUInt16(),
            GxComponentType.S16 => br.ReadInt16(),
            GxComponentType.F32 => br.ReadSingle(),
        };
      }
    }
  }

  public class DatPrimitive {
    public required GxOpcode Type { get; init; }
    public required IReadOnlyList<DatVertex> Vertices { get; init; }
  }

  public class DatVertex {
    public required int BoneId { get; init; }
    public required Vector3 Position { get; init; }
    public Vector3? Normal { get; init; }
    public Vector2? Uv0 { get; init; }
    public Vector2? Uv1 { get; init; }
    public IColor? Color { get; init; }
  }

  [BinarySchema]
  public partial class FileHeader : IBinaryConvertible {
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
  public partial class RootNodeData : IBinaryConvertible {
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
}