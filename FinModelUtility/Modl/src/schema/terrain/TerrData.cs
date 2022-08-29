using fin.util.strings;

using schema;


namespace modl.schema.terrain {
  [BinarySchema]
  public partial class TerrData : IBiSerializable {
    public uint ChunkCountX { get; } = 64;
    public uint ChunkCountY { get; } = 64;

    public uint SomeCount { get; } = 1;

    public int MaterialCount { get; private set; }
  }
}
