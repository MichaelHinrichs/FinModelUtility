using dat.schema.material;

using schema.binary;
using schema.binary.attributes;

namespace dat.schema {
  /// <summary>
  ///   Data object.
  /// </summary>
  [BinarySchema]
  public partial class DObj : IDatLinkedListNode<DObj>, IBinaryDeserializable {
    public uint StringOffset { get; set; }
    public uint NextDObjOffset { get; set; }
    public uint MObjOffset { get; set; }
    public uint FirstPObjOffset { get; set; }


    [Ignore]
    public string? Name { get; set; }

    [RAtPositionOrNull(nameof(NextDObjOffset))]
    public DObj? NextSibling { get; private set; }

    [RAtPositionOrNull(nameof(MObjOffset))]
    public MObj? MObj { get; private set; }

    [RAtPositionOrNull(nameof(FirstPObjOffset))]
    public PObj? FirstPObj { get; private set; }


    [Ignore]
    public IEnumerable<PObj> PObjs => this.FirstPObj.GetSelfAndSiblings();
  }
}