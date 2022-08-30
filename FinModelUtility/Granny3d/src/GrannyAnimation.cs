using schema;


namespace granny3d {
  public class GrannyAnimation : IGrannyAnimation, IDeserializable {
    public string Name { get; private set; }
    public float Duration { get; private set; }
    public float TimeStep { get; private set; }
    public float Oversampling { get; private set; }

    public IList<IGrannyTrackGroup> TrackGroups { get; } =
      new List<IGrannyTrackGroup>();

    public void Read(EndianBinaryReader er) {
      GrannyUtils.SubreadUInt64Pointer(
          er, ser => { this.Name = ser.ReadStringNT(); });

      this.Duration = er.ReadSingle();
      this.TimeStep = er.ReadSingle();
      this.Oversampling = er.ReadSingle();

      var trackGroupCount = er.ReadUInt32();
      GrannyUtils.SubreadUInt64Pointer(er, ser => {
        for (var i = 0; i < trackGroupCount; ++i) {
          var trackGroup = new GrannyTrackGroup();
          trackGroup.Read(ser);
          this.TrackGroups.Add(trackGroup);
        }
      });
    }
  }

  public class GrannyTrackGroup : IGrannyTrackGroup, IDeserializable {
    public void Read(EndianBinaryReader er) {
      var name = "";
      GrannyUtils.SubreadUInt64Pointer(er, ser => name = ser.ReadStringNT());

      ;
    }
  }
}