using fin.data;

using schema;


namespace modl.schema.terrain.bw1 {
  public class Bw1Heightmap : IBwHeightmap, IDeserializable {
    public Grid<IBwHeightmapChunk?> Chunks { get; private set; }

    public void Read(EndianBinaryReader er) {
      er.AssertStringEndian("TERR");
      var terrSize = er.ReadUInt32();
      er.Position += terrSize;

      er.AssertStringEndian("CHNK");
      var chnkSize = er.ReadInt32();
      var tilesBytes = er.ReadBytes(chnkSize);

      er.AssertStringEndian("GPNF");
      var fnpgSize = er.ReadUInt32();
      er.Position += fnpgSize;

      er.AssertStringEndian("CMAP");
      var cmapSize = er.ReadInt32();
      var tilemapBytes = er.ReadBytes(cmapSize);

      var heightmapParser = new HeightmapParser(tilemapBytes, tilesBytes);
      this.Chunks = heightmapParser.Chunks;
    }
  }
}