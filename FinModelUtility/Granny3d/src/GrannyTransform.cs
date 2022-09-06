using System.Numerics;

using fin.schema.vector;

using schema;


namespace granny3d {
  public class GrannyTransform : IGrannyTransform, IDeserializable {
    public GrannyTransformFlags Flags { get; private set; }
    public Vector3f Position { get; private set; }
    public Quaternion Orientation { get; private set; }
    public Vector3f[] ScaleShear { get; } = new Vector3f[3];

    public void Read(EndianBinaryReader er) {
      this.Flags = (GrannyTransformFlags)er.ReadInt32();

      this.Position = er.ReadNew<Vector3f>();

      this.Orientation = new Quaternion(er.ReadSingle(), er.ReadSingle(),
                                        er.ReadSingle(), er.ReadSingle());

      for (var i = 0; i < 3; ++i) {
        this.ScaleShear[i] = er.ReadNew<Vector3f>();
      }
    }
  }
}
