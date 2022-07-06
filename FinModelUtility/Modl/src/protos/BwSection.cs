using fin.util.asserts;

using schema;


namespace modl.protos {
  public class BwSection : IDeserializable {
    public string Name { get; private set; }
    public IList<BwResource> Entries { get; } = new List<BwResource>();

    public BwSection() { }

    public BwSection(BwResource impl, string expectedName) {
      this.PopulateFromResource_(impl, expectedName);
    }

    public void Read(EndianBinaryReader er) 
      => this.PopulateFromResource_(er.ReadNew<BwResource>());

    private void PopulateFromResource_(BwResource impl, string expectedName = null) {
      this.Name = impl.Name;
      if (expectedName != null) {
        Asserts.Equal(this.Name, expectedName);
      }

      this.Entries.Clear();
      using var er = new EndianBinaryReader(new MemoryStream(impl.Data), Endianness.LittleEndian);
      while (er.BaseStream.Position < er.BaseStream.Length) {
        this.Entries.Add(er.ReadNew<BwResource>());
      }
    }
  }
}