using fin.math.matrix.four;
using fin.util.asserts;
using fin.util.enumerables;
using fin.util.hex;

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

    public IEnumerable<JObj> JObjs => this.RootJObjs.SelectMany(
        this.EnumerateSelfAndChildrenWithinJObj_);

    private Dictionary<uint, JObj> jObjByOffset_ = new();
    public IReadOnlyDictionary<uint, JObj> JObjByOffset => this.jObjByOffset_;

    private IEnumerable<JObj> EnumerateSelfAndChildrenWithinJObj_(JObj jObj)
      => jObj.Yield()
             .Concat(jObj.Children.SelectMany(
                         this.EnumerateSelfAndChildrenWithinJObj_));

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
      this.ReadDObjs_(br);
      this.ReadNames_(br);
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

        this.jObjByOffset_[jObjDataOffset] = jObj;
        jObj.Name = jObjDataOffset.ToHex();

        if (parentJObj == null) {
          this.RootJObjs.Add(jObj);
        } else {
          parentJObj.Children.Add(jObj);
        }

        br.Position = jObjDataOffset;
        jObjData.Read(br);

        if (jObjData.InverseBindMatrixOffset != 0) {
          br.Position = jObjData.InverseBindMatrixOffset;

          var inverseBindMatrixValues = new float[4 * 4];
          br.ReadSingles(inverseBindMatrixValues.AsSpan(0, 4 * 3));
          inverseBindMatrixValues[15] = 1;
          jObj.InverseBindMatrix =
              new FinMatrix4x4(inverseBindMatrixValues).TransposeInPlace();
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

    private void ReadDObjs_(IBinaryReader br) {
      br.Position = this.dataBlockOffset_;
      br.PushLocalSpace();

      foreach (var jObj in this.JObjs) {
        if (jObj.Data.FirstDObjOffset != 0) {
          br.Position = jObj.Data.FirstDObjOffset;
          jObj.FirstDObj = new DObj(this);
          jObj.FirstDObj.Read(br);
        }
      }

      br.PopLocalSpace();
    }

    private void ReadNames_(IBinaryReader br) {
      br.Position = this.stringTableOffset_;
      br.PushLocalSpace();

      foreach (var jObj in this.JObjs) {
        var jObjStringOffset = jObj.Data.StringOffset;
        if (jObjStringOffset != 0) {
          br.Position = jObjStringOffset;
          jObj.Name = br.ReadStringNT();
        }

        foreach (var dObj in jObj.DObjs) {
          var dObjStringOffset = dObj.Header.StringOffset;
          if (dObjStringOffset != 0) {
            br.Position = dObjStringOffset;
            dObj.Name = br.ReadStringNT();
          }

          var mObj = dObj.MObj;
          if (mObj != null) {
            var mObjStringOffset = mObj.StringOffset;
            if (mObjStringOffset != 0) {
              br.Position = mObjStringOffset;
              mObj.Name = br.ReadStringNT();
            }

            foreach (var (_, tObj) in mObj.TObjsAndOffsets) {
              var tObjStringOffset = tObj.StringOffset;
              if (tObjStringOffset != 0) {
                br.Position = tObj.StringOffset;
                tObj.Name = br.ReadStringNT();
              }
            }
          }
        }
      }

      br.PopLocalSpace();
    }
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

    public override string ToString() => $"[{Type}]: {Name}";

    public RootNodeType Type { get; private set; }

    private static RootNodeType GetTypeFromName_(string name) {
      // TODO: Use flags for this instead
      if (name.EndsWith("_joint") && !name.Contains("matanim") &&
          !name.Contains("anim_joint")) {
        return RootNodeType.JObj;
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