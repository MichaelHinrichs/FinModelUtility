using schema;


namespace modl.protos {
  public class BwResource : IDeserializable {
    public string Name { get; private set; }
    public byte[] Data { get; private set; }

    public void Read(EndianBinaryReader er) {
      this.Name = er.ReadString(4);
      var size = er.ReadUInt32();
      this.Data = er.ReadBytes((int) size);
    }
  }
}