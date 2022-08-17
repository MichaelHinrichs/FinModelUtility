using schema;


namespace dat.schema {
  [Schema]
  public partial class DObjData : IBiSerializable {
    public uint StringOffset { get; set; }
    public uint NextObjectOffset { get; set; }
    public uint MaterialStructOffset { get; set; }
    public uint MeshStructOffset { get; set; }
  }

  public class DObj {
    public DObjData Data { get; } = new();
    public string Name { get; set; }

    public List<PObj> PObjs { get; } = new();
  }
}