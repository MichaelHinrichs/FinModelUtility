using fin.util.asserts;

using schema.binary;


namespace modl.schema.terrain.bw1 {
  public class Bw2Terrain : IBwTerrain, IBinaryDeserializable {
    public IBwHeightmap Heightmap { get; private set; }
    public IList<BwHeightmapMaterial> Materials { get; private set; }

    public void Read(IEndianBinaryReader er) {
      var sections = new Dictionary<string, BwSection>();
      while (!er.Eof) {
        SectionHeaderUtil.ReadNameAndSize(er, out var name, out var size);
        var offset = er.Position;

        sections[name] = new BwSection(name, (int) size, offset);

        er.Position += size;
      }

      var terrSection = sections["TERR"];
      er.Position = terrSection.Offset;
      var terrData = er.ReadNew<TerrData>();

      var chnkSection = sections["CHNK"];
      er.Position = chnkSection.Offset;
      var tilesBytes = er.ReadBytes(chnkSection.Size);

      var cmapSection = sections["CMAP"];
      er.Position = cmapSection.Offset;
      var tilemapBytes = er.ReadBytes(cmapSection.Size);

      var matlSection = sections["MATL"];
      er.Position = matlSection.Offset;
      var expectedMatlSectionSize = terrData.MaterialCount * 48;
      Asserts.Equal(expectedMatlSectionSize, matlSection.Size);
      er.ReadNewArray<BwHeightmapMaterial>(out var materials,
                                           terrData.MaterialCount);

      this.Heightmap = new HeightmapParser(terrData, tilemapBytes, tilesBytes);
      this.Materials = materials;
    }
  }
}