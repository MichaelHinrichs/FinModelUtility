using System.IO;

namespace schema {
  public interface ISerializable {
    void Write(EndianBinaryWriter ew);
  }

  public interface IDeserializable {
    void Read(EndianBinaryReader er);
  }

  public interface IBiSerializable : ISerializable, IDeserializable {}
}