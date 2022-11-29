using fin.decompression;


namespace level5.decompression {
  public class LzssDecompressor : IDecompressor {
    public byte[] Decompress(byte[] data) {
      List<byte> o = new List<byte>();

      int p = 4;
      int op = 0;

      int mask = 0;
      int flag = 0;

      while (p < data.Length) {
        if (mask == 0) {
          flag = (data[p++] & 0xFF);
          mask = 0x80;
        }

        if ((flag & mask) == 0) {
          if (p + 1 > data.Length) break;
          o.Add(data[p++]);
          op++;
        } else {
          if (p + 2 > data.Length) break;
          int dat = ((data[p++] & 0xFF) << 8) | (data[p++] & 0xFF);
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

      return o.ToArray();
    }
  }
}