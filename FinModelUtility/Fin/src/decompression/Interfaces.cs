using System;


namespace fin.decompression {
  public interface IDecompressor {
    bool TryDecompress(byte[] src, out byte[] dst);

    byte[] Decompress(byte[] src);
  }

  public abstract class BDecompressor : IDecompressor {
    public abstract bool TryDecompress(byte[] src, out byte[] dst);

    public byte[] Decompress(byte[] src) {
      if (TryDecompress(src, out byte[] dst)) {
        return dst;
      }

      throw new Exception("Failed to decompress bytes.");
    }
  }
}