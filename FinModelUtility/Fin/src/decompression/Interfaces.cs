namespace fin.decompression {
  public interface IDecompressor {
    byte[] Decompress(byte[] bytes);
  }
}