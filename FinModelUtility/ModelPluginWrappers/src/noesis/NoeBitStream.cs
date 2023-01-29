#pragma warning disable IDE1006 // Naming Styles

using System.Text;

using IronPython.Runtime;

using Microsoft.Scripting.Utils;

namespace ModelPluginWrappers.src.noesis {
  public enum NoeEndianness {
    NOE_LITTLEENDIAN = 0,
    NOE_BIGENDIAN = 1,
  }

  public interface INoeBitStream {
    int getSize();

    void setOffset(int ofs);
    int getOffset();
  }

  public interface INoeBitStreamReader : INoeBitStream {
    string readline();
  }

  public class NoeBitStreamReader : INoeBitStreamReader {
    private readonly EndianBinaryReader impl_; 

    public NoeBitStreamReader(byte[] data, NoeEndianness endianness = NoeEndianness.NOE_LITTLEENDIAN) {
      this.impl_ = new EndianBinaryReader(data, endianness switch {
        NoeEndianness.NOE_BIGENDIAN => Endianness.BigEndian,
        NoeEndianness.NOE_LITTLEENDIAN => Endianness.LittleEndian,
      });
    }

    public int getSize() => (int) this.impl_.Length;

    public int getOffset() => (int) this.impl_.Position;
    public void setOffset(int ofs) => this.impl_.Position = ofs;

    public string readline() {
      var str = this.impl_.ReadLine();
      return str;
    }
  }
}
