using schema.binary;

namespace dat.schema {
  /// <summary>
  ///   Data object.
  /// </summary>
  public partial class DObj : IBinaryDeserializable {
    private readonly Dat dat_;

    public DObj(Dat dat) {
      this.dat_ = dat;
    }

    [BinarySchema]
    public partial class DObjHeader : IBinaryConvertible {
      public uint StringOffset { get; set; }
      public uint NextDObjOffset { get; set; }
      public uint MObjOffset { get; set; }
      public uint FirstPObjOffset { get; set; }
    }

    public DObjHeader Header { get; } = new();
    public string Name { get; set; }

    public DObj? NextDObj { get; private set; }
    public MObj? MObj { get; private set; }
    public PObj? FirstPObj { get; private set; }

    public IEnumerable<PObj> PObjs {
      get {
        var current = this.FirstPObj;
        while (current != null) {
          yield return current;
          current = current.NextPObj;
        }
      }
    }

    public void Read(IBinaryReader br) {
      this.Header.Read(br);

      if (this.Header.FirstPObjOffset != 0) {
        br.Position = this.Header.FirstPObjOffset;
        this.FirstPObj = new PObj(this.dat_);
        this.FirstPObj.Read(br);
      }

      if (this.Header.MObjOffset != 0) {
        br.Position = this.Header.MObjOffset;
        this.MObj = br.ReadNew<MObj>();
      }

      if (this.Header.NextDObjOffset != 0) {
        br.Position = this.Header.NextDObjOffset;
        this.NextDObj = new DObj(this.dat_);
        this.NextDObj.Read(br);
      }
    }
  }
}