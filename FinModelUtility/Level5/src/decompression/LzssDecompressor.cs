using fin.decompression;


namespace level5.decompression {
  public class LzssDecompressor : BDecompressor {
    public override bool TryDecompress(byte[] src, out byte[] dst) {
      var size = 0;
      {
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
            p++;
            size++;
            op++;
          } else {
            if (p + 2 > src.Length) break;
            int dat = ((src[p++] & 0xFF) << 8) | (src[p++] & 0xFF);
            int pos = (dat & 0x0FFF) + 1;

            if (op >= pos) {
              int length = (dat >> 12) + 3;
              size += length;
              op += length;
            }

            /*for (int i = 0; i < length; i++) {
              if (op - pos >= 0) {
                size++;
                op++;
              }
            }*/
          }
          mask >>= 1;
        }
      }

      dst = new byte[size];
      var oI = 0;
      {
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
            dst[oI++] = src[p++];
            op++;
          } else {
            if (p + 2 > src.Length) break;
            int dat = ((src[p++] & 0xFF) << 8) | (src[p++] & 0xFF);
            int pos = (dat & 0x0FFF) + 1;

            if (op >= pos) {
              int length = (dat >> 12) + 3;
              for (int i = 0; i < length; i++) {
                dst[oI] = dst[op - pos >= oI ? 0 : op - pos];
                oI++;
                op++;
              }
            }
          }
          mask >>= 1;
        }
      }

      return true;
    }
  }
}