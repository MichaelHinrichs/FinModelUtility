using fin.decompression;

namespace level5.decompression {
  public class Level5Decompressor : BDecompressor {
    public override bool TryDecompress(byte[] src, out byte[] dst) {
      int tableType = (src[0] & 0xFF);

      if (new ZlibDecompressor().TryDecompress(src, out dst)) {
        return true;
      }

      switch (tableType & 0xF) {
        case 0x01:
          dst = new LzssDecompressor().Decompress(src);
          break;
        case 0x02:
          dst = new HuffmanDecompressor(0x24).Decompress(src);
          break;
        case 0x03:
          dst = new HuffmanDecompressor(0x28).Decompress(src);
          break;
        case 0x04:
          dst = new RleDecompressor().Decompress(src);
          break;
        default:
          dst = new byte[src.Length - 4];
          src.AsSpan(4).CopyTo(dst);
          break;
      }

      return true;
    }
  }
}