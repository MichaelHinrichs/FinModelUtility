using fin.decompression;

namespace level5.decompression {
  public class HuffmanDecompressor : BDecompressor {
    private readonly byte aType_;

    public HuffmanDecompressor(byte aType) {
      this.aType_ = aType;
    }

    public override bool TryDecompress(byte[] src, out byte[] dst) {
      HuffStream instream = new HuffStream(src);
      long readBytes = 0;

      byte type = (byte)instream.ReadByte();
      type = this.aType_;
      if (type != 0x28 && type != 0x24) {
        dst = null;
        return false;
      }
      int decompressedSize = instream.ReadThree();
      readBytes += 4;
      if (decompressedSize == 0) {
        instream.p -= 3;
        decompressedSize = instream.ReadInt32();
        readBytes += 4;
      }

      List<byte> o = new List<byte>();

      int treeSize = instream.ReadByte(); readBytes++;
      treeSize = (treeSize + 1) * 2;

      long treeEnd = (instream.p - 1) + treeSize;

      // the relative offset may be 4 more (when the initial decompressed size is 0), but
      // since it's relative that doesn't matter, especially when it only matters if
      // the given value is odd or even.
      HuffTreeNode rootNode = new HuffTreeNode(instream, false, 5, treeEnd);

      readBytes += treeSize;
      // re-position the stream after the tree (the stream is currently positioned after the root
      // node, which is located at the start of the tree definition)
      instream.p = (int)treeEnd;

      // the current u32 we are reading bits from.
      int data = 0;
      // the amount of bits left to read from <data>
      byte bitsLeft = 0;

      // a cache used for writing when the block size is four bits
      int cachedByte = -1;

      // the current output size
      HuffTreeNode currentNode = rootNode;

      while (instream.HasBytes()) {
        while (!currentNode.isData) {
          // if there are no bits left to read in the data, get a new byte from the input
          if (bitsLeft == 0) {
            readBytes += 4;
            data = instream.ReadInt32();
            bitsLeft = 32;
          }
          // get the next bit
          bitsLeft--;
          bool nextIsOne = (data & (1 << bitsLeft)) != 0;
          // go to the next node, the direction of the child depending on the value of the current/next bit
          currentNode = nextIsOne ? currentNode.child1 : currentNode.child0;
        }

        switch (type) {
          case 0x28: {
              // just copy the data if the block size is a full byte
              //                        outstream.WriteByte(currentNode.Data);
              o.Add(currentNode.data);
              break;
            }
          case 0x24: {
              // cache the first half of the data if the block size is a half byte
              if (cachedByte < 0) {
                cachedByte = currentNode.data;
              } else {
                cachedByte |= currentNode.data << 4;
                o.Add((byte)cachedByte);
                cachedByte = -1;
              }
              break;
            }
        }

        currentNode = rootNode;
      }

      if (readBytes % 4 != 0)
        readBytes += 4 - (readBytes % 4);


      dst = o.ToArray();
      return true;
    }

    private class HuffStream {
      public byte[] bytes;
      public int p = 0;
      public int length;
      public HuffStream(byte[] b) {
        bytes = b;
        length = b.Length;
      }

      public bool HasBytes() {
        return p < bytes.Length;
      }

      public int ReadByte() {
        return bytes[p++] & 0xFF;
      }
      public int ReadThree() {
        return ((bytes[p++] & 0xFF)) | ((bytes[p++] & 0xFF) << 8) | ((bytes[p++] & 0xFF) << 16);
      }
      public int ReadInt32() {
        if (p >= length)
          return 0;
        else
          return ((bytes[p++] & 0xFF)) | ((bytes[p++] & 0xFF) << 8) | ((bytes[p++] & 0xFF) << 16) | ((bytes[p++] & 0xFF) << 24);
      }
    }

    private class HuffTreeNode {
      public byte data;
      public bool isData;
      public HuffTreeNode child0; public HuffTreeNode child1;
      public HuffTreeNode(HuffStream stream, bool isData, long relOffset, long maxStreamPos) {
        if (stream.p >= maxStreamPos) {
          return;
        }
        int readData = stream.ReadByte();
        this.data = (byte)readData;

        this.isData = isData;

        if (!this.isData) {
          int offset = this.data & 0x3F;
          bool zeroIsData = (this.data & 0x80) > 0;
          bool oneIsData = (this.data & 0x40) > 0;

          long zeroRelOffset = (relOffset ^ (relOffset & 1)) + (offset * 2) + 2;

          int currStreamPos = stream.p;
          stream.p += (int)(zeroRelOffset - relOffset) - 1;
          this.child0 = new HuffTreeNode(stream, zeroIsData, zeroRelOffset, maxStreamPos);
          this.child1 = new HuffTreeNode(stream, oneIsData, zeroRelOffset + 1, maxStreamPos);

          stream.p = currStreamPos;
        }
      }
    }
  }
}