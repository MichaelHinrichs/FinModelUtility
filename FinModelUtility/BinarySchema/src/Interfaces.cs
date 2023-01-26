using System.IO;

namespace schema {
  public interface ISerializable {
    void Write(ISubEndianBinaryWriter ew);
  }

  public interface IDeserializable {
    void Read(IEndianBinaryReader er);
  }

  public interface IBiSerializable : ISerializable, IDeserializable {}
}