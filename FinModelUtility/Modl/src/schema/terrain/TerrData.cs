using fin.util.strings;

using schema;


namespace modl.schema.terrain {
  [BinarySchema]
  public partial class TerrData : IBiSerializable {
    public int ChunkCountX { get; } = 64;
    public int ChunkCountY { get; } = 64;

    public uint SomeCount { get; } = 1;

    public int MaterialCount { get; private set; }
  }
}
