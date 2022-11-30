using fin.decompression;


namespace level5.decompression {
  public class LzssDecompressor : BDecompressor {
    public override bool TryDecompress(byte[] src, out byte[] dst) {
      List<byte> o = new List<byte>();

      int p = 4;
      int op = 0;

      int mask = 0;
      int flag = 0;

      while (p < src.Length) {
        if (mask == 0) {
          flag = (src[p++] & 0xFF);
          mask = 0x80;
        }

        if ((flag & mask) == 0) {
          if (p + 1 > src.Length) break;
          o.Add(src[p++]);
          op++;
        } else {
          if (p + 2 > src.Length) break;
          int dat = ((src[p++] & 0xFF) << 8) | (src[p++] & 0xFF);
          int pos = (dat & 0x0FFF) + 1;
          int length = (dat >> 12) + 3;

          for (int i = 0; i < length; i++) {
            if (op - pos >= 0) {
              o.Add(o[op - pos >= o.Count ? 0 : op - pos]);
              op++;
            }
          }
        }
        mask >>= 1;
      }

      dst = o.ToArray();
      return true;
    }
  }
}