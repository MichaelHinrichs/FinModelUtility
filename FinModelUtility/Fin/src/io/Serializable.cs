using System.IO;

namespace fin.io {
  public interface ISerializable {
    public void Write(EndianBinaryWriter w);
  }

  public interface IDeserializable {
    public void Read(EndianBinaryReader r);
  }

  public interface IBiSerializable : ISerializable, IDeserializable {}
}