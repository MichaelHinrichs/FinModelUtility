using System.IO;

namespace mod.gcn {
  public interface IGcnSerializable {
    void Read(EndianBinaryReader reader);
    void Write(EndianBinaryWriter writer);
  }
}