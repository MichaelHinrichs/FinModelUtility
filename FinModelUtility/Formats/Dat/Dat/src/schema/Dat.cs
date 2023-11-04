using dat.schema.animation;

using fin.data.queues;
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

    private readonly Dictionary<uint, JObj> jObjByOffset_ = new();
    public IReadOnlyDictionary<uint, JObj> JObjByOffset => this.jObjByOffset_;

    public IEnumerable<JObj> JObjs => this.RootJObjs.SelectMany(
        DatNodeExtensions.GetSelfAndChildrenAndSiblings);


    public List<MatAnimJoint> RootMatAnimJoints { get; } = new();

    public IEnumerable<MatAnimJoint> MatAnimJoints
      => this.RootMatAnimJoints.SelectMany(
          DatNodeExtensions.GetSelfAndChildrenAndSiblings);


    public void Read(IBinaryReader br) {
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

      this.ReadRootNodeObjects_(br);
      this.ReadNames_(br);
    }

    private void ReadRootNodeObjects_(IBinaryReader br) {
      br.Position = this.dataBlockOffset_;
      br.PushLocalSpace();

      var jObjQueue = new FinTuple2Queue<uint, JObj>();

      this.RootJObjs.Clear();
      this.RootMatAnimJoints.Clear();
      foreach (var rootNode in this.rootNodes_) {
        switch (rootNode.Type) {
          case RootNodeType.JObj: {
            var jObjOffset = rootNode.Data.DataOffset;
            br.Position = jObjOffset;

            var jObj = br.ReadNew<JObj>();
            this.RootJObjs.Add(jObj);

            jObjQueue.Enqueue((jObjOffset, jObj));
            break;
          }
          case RootNodeType.MATANIM_JOINT: {
            br.Position = rootNode.Data.DataOffset;
            this.RootMatAnimJoints.Add(br.ReadNew<MatAnimJoint>());
            break;
          }
        }
      }

      br.PopLocalSpace();

      while (jObjQueue.TryDequeue(out var jObjOffset, out var jObj)) {
        this.jObjByOffset_[jObjOffset] = jObj;

        if (jObj.FirstChild != null) {
          jObjQueue.Enqueue((jObj.FirstChildBoneOffset, jObj.FirstChild));
        }

        if (jObj.NextSibling != null) {
          jObjQueue.Enqueue((jObj.NextSiblingBoneOffset, jObj.NextSibling));
        }
      }
    }

    private void ReadNames_(IBinaryReader br) {
      br.Position = this.stringTableOffset_;
      br.PushLocalSpace();

      foreach (var jObj in this.JObjs) {
        var jObjStringOffset = jObj.StringOffset;
        if (jObjStringOffset != 0) {
          br.Position = jObjStringOffset;
          jObj.Name = br.ReadStringNT();
        }

        foreach (var dObj in jObj.DObjs) {
          var dObjStringOffset = dObj.StringOffset;
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