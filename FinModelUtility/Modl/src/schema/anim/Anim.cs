using schema;


namespace modl.schema.anim {
  public class Anim : IDeserializable {
    public void Read(EndianBinaryReader er) {
      var name0Length = er.ReadUInt32();
      var name0 = er.ReadString((int) name0Length);

      var unknown = er.ReadUInt32();
      er.AssertUInt32(0);

      // The name is repeated once more, excepted null-terminated.
      var name1 = er.ReadStringNT();

      // Next is a series of "0xcd" values--one for each node in the mesh.
      var boneCount = 0;
      char c;
      while ((c = er.ReadChar()) == 0xcd) {
        ++boneCount;
      }
      --er.Position;

      // Next is a series of bone definitions. Each one has a length of 64.
      for (var i = 0; i < boneCount; ++i) {
        er.Position += 64;
      }
    }
  }
}
