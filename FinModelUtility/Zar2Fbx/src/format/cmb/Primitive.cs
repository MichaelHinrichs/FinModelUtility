using System.IO;

using fin.io;
using fin.util.strings;

namespace zar.format.cmb {
  public class Primitive : IDeserializable {
    public uint chunkSize;
    public bool isVisible;
    public PrimitiveMode primitiveMode;
    public DataType dataType;
    public ushort indicesCount;
    public ushort offset;

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("prm" + AsciiUtil.GetChar(20));

      this.chunkSize = r.ReadUInt32();
      this.isVisible = r.ReadUInt32() != 0;

      // Other modes don't exist in OoT3D's shader so we'd never know
      this.primitiveMode = (PrimitiveMode) r.ReadUInt32();
      this.dataType = (DataType) r.ReadUInt32();

      this.indicesCount = r.ReadUInt16();
      this.offset = r.ReadUInt16();
    }
  }
}