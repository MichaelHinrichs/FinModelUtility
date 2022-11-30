using fin.decompression;
using System.IO.Compression;


namespace level5.decompression {
  public class ZlibDecompressor : BDecompressor {
    public override bool TryDecompress(byte[] src, out byte[] dst) {
      var b = src;
      if (b.Length < 6) {
        dst = null;
        return false;
      }

      if (b[4] != 0x78) {
        dst = null;
        return false;
      }

      var decomLength = (b[0] & 0xFF) | ((b[1] & 0xFF) << 8) |
                        ((b[2] & 0xFF) << 16) | ((b[3] & 0xFF) << 24);
      var data = new byte[b.Length - 4];
      Array.Copy(b, 4, data, 0, b.Length - 4);

      var stream = new MemoryStream();
      var ms = new MemoryStream(data);
      ms.ReadByte();
      ms.ReadByte();
      var zlibStream = new DeflateStream(ms, CompressionMode.Decompress);
      byte[] buffer = new byte[2048];
      while (true) {
        int size = zlibStream.Read(buffer, 0, buffer.Length);
        if (size > 0)
          stream.Write(buffer, 0, buffer.Length);
        else
          break;
      }
      zlibStream.Close();
      dst = stream.ToArray();
      return true;
    }
  }
}