using fin.decompression;

namespace level5.decompression {
  public class RleDecompressor : BDecompressor {
    public override bool TryDecompress(byte[] src, out byte[] dst) {
      long inLength = src.Length;
      long readBytes = 0;
      int p = 0;

      p++;

      int decompressedSize = (src[p++] & 0xFF)
                             | ((src[p++] & 0xFF) << 8)
                             | ((src[p++] & 0xFF) << 16);
      readBytes += 4;
      if (decompressedSize == 0) {
        decompressedSize = decompressedSize
                           | ((src[p++] & 0xFF) << 24);
        readBytes += 4;
      }

      List<byte> outstream = new List<byte>();

      while (p < src.Length) {
        int flag = (byte)src[p++];
        readBytes++;

        bool compressed = (flag & 0x80) > 0;
        int length = flag & 0x7F;

        if (compressed)
          length += 3;
        else
          length += 1;

        if (compressed) {
          int data = (byte)src[p++];
          readBytes++;

          byte bdata = (byte)data;
          for (int i = 0; i < length; i++) {
            outstream.Add(bdata);
          }
        } else {
          int tryReadLength = length;
          if (readBytes + length > inLength)
            tryReadLength = (int)(inLength - readBytes);

          readBytes += tryReadLength;

          for (int i = 0; i < tryReadLength; i++) {
            outstream.Add((byte)(src[p++] & 0xFF));
          }
        }
      }

      if (readBytes < inLength) { }

      dst = outstream.ToArray();
      return true;
    }
  }
}