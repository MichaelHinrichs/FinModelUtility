using fin.data.queues;

using schema.binary;

namespace dat.schema {
  /// <summary>
  ///   Data object.
  /// </summary>
  [BinarySchema]
  public partial class DObjData : IBinaryConvertible {
    public uint StringOffset { get; set; }
    public uint NextObjectOffset { get; set; }
    public uint MaterialStructOffset { get; set; }
    public uint MeshStructOffset { get; set; }
  }

  public class DObj {
    public DObjData Data { get; } = new();
    public string Name { get; set; }

    public PObj? FirstPObj { get; set; }

    public IEnumerable<PObj> PObjs {
      get {
        var current = this.FirstPObj;
        while (current != null) {
          yield return current;
          current = current.NextPObj;
        }
      }
    }
  }
}