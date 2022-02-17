using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using fin.util.image;

// From https://github.com/mafaca/Dxt


namespace Dxt {
  public static class DxtDecoder {
    public static unsafe Bitmap DecompressDXT1(
        byte[] src,
        int srcOffset,
        int width,
        int height) {
      int offset = srcOffset;
      int bcw = (width + 3) / 4;
      int bch = (height + 3) / 4;
      int clen_last = (width + 3) % 4 + 1;
      int[] buffer = new int[16];
      int[] colors = new int[4];

      var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
      BitmapUtil.InvokeAsLocked(bitmap, bmpData => {
        var dst = (byte*) bmpData.Scan0.ToPointer();

        for (int t = 0; t < bch; t++) {
          for (int s = 0; s < bcw; s++, offset += 8) {
            int r0, g0, b0, r1, g1, b1;
            int q0 = src[offset + 0] | src[offset + 1] << 8;
            int q1 = src[offset + 2] | src[offset + 3] << 8;
            Rgb565(q0, out r0, out g0, out b0);
            Rgb565(q1, out r1, out g1, out b1);
            colors[0] = Color(r0, g0, b0, 255);
            colors[1] = Color(r1, g1, b1, 255);
            if (q0 > q1) {
              colors[2] = Color((r0 * 2 + r1) / 3,
                                (g0 * 2 + g1) / 3,
                                (b0 * 2 + b1) / 3,
                                255);
              colors[3] = Color((r0 + r1 * 2) / 3,
                                (g0 + g1 * 2) / 3,
                                (b0 + b1 * 2) / 3,
                                255);
            } else {
              colors[2] = Color((r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2,
                                255);
            }

            uint d = BitConverter.ToUInt32(src, offset + 4);
            for (int i = 0; i < 16; i++, d >>= 2) {
              buffer[i] = colors[d & 3];
            }

            int clen = (s < bcw - 1 ? 4 : clen_last);
            for (int i = 0, y = t * 4; i < 4 && y < height; i++, y++) {
              var dstBuffer = dst + (y * width + s * 4) * 4;
              var dstPtr = (IntPtr) dstBuffer;

              Marshal.Copy(buffer,
                           i * 4,
                           dstPtr,
                           clen);
            }
          }
        }
      });

      return bitmap;
    }


    public static void DecompressDXT3(
        byte[] input,
        int width,
        int height,
        byte[] output) {
      int offset = 0;
      int bcw = (width + 3) / 4;
      int bch = (height + 3) / 4;
      int clen_last = (width + 3) % 4 + 1;
      uint[] buffer = new uint[16];
      int[] colors = new int[4];
      int[] alphas = new int[16];
      for (int t = 0; t < bch; t++) {
        for (int s = 0; s < bcw; s++, offset += 16) {
          for (int i = 0; i < 4; i++) {
            int alpha = input[offset + i * 2] | input[offset + i * 2 + 1] << 8;
            alphas[i * 4 + 0] = (((alpha >> 0) & 0xF) * 0x11) << 24;
            alphas[i * 4 + 1] = (((alpha >> 4) & 0xF) * 0x11) << 24;
            alphas[i * 4 + 2] = (((alpha >> 8) & 0xF) * 0x11) << 24;
            alphas[i * 4 + 3] = (((alpha >> 12) & 0xF) * 0x11) << 24;
          }

          int r0, g0, b0, r1, g1, b1;
          int q0 = input[offset + 8] | input[offset + 9] << 8;
          int q1 = input[offset + 10] | input[offset + 11] << 8;
          Rgb565(q0, out r0, out g0, out b0);
          Rgb565(q1, out r1, out g1, out b1);
          colors[0] = Color(r0, g0, b0, 0);
          colors[1] = Color(r1, g1, b1, 0);
          if (q0 > q1) {
            colors[2] = Color((r0 * 2 + r1) / 3,
                              (g0 * 2 + g1) / 3,
                              (b0 * 2 + b1) / 3,
                              0);
            colors[3] = Color((r0 + r1 * 2) / 3,
                              (g0 + g1 * 2) / 3,
                              (b0 + b1 * 2) / 3,
                              0);
          } else {
            colors[2] = Color((r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2, 0);
          }

          uint d = BitConverter.ToUInt32(input, offset + 12);
          for (int i = 0; i < 16; i++, d >>= 2) {
            buffer[i] = unchecked((uint) (colors[d & 3] | alphas[i]));
          }

          int clen = (s < bcw - 1 ? 4 : clen_last) * 4;
          for (int i = 0, y = t * 4; i < 4 && y < height; i++, y++) {
            Buffer.BlockCopy(buffer,
                             i * 4 * 4,
                             output,
                             (y * width + s * 4) * 4,
                             clen);
          }
        }
      }
    }

    /*public static void WriteToStreamFromRawDxt5a(
        Stream dst,
        byte[] src,
        int srcOffset,
        int width,
        int height) {
      var ew = new EndianBinaryWriter(dst, Endianness.LittleEndian);

      var imageSize = width * height / 2;

      ew.Write("DDS ", Encoding.ASCII, false);
      ew.Write(124);
      ew.Write(0x000A1007);
      ew.Write(width);
      ew.Write(height);
      ew.Write(imageSize);
      ew.Write(0);
      ew.Write(1);

      for (var i = 0; i < 11; ++i) {
        ew.Write(0);
      }

      ew.Write(0x20);
      ew.Write(4);
      ew.Write("ATI1", Encoding.ASCII, false);
      ew.Write(0);
      ew.Write(0);
      ew.Write(0);
      ew.Write(0);
      ew.Write(0);
      ew.Write(0x401008);

      var fixedBuffer = new byte[imageSize];
      for (var i = 0; i < imageSize; i += 8) {
        fixedBuffer[i + 0] = src[srcOffset + i + 1];
        fixedBuffer[i + 1] = src[srcOffset + i + 0];

        fixedBuffer[i + 2] = src[srcOffset + i + 3];
        fixedBuffer[i + 3] = src[srcOffset + i + 2];
        fixedBuffer[i + 4] = src[srcOffset + i + 5];

        fixedBuffer[i + 5] = src[srcOffset + i + 4];
        fixedBuffer[i + 6] = src[srcOffset + i + 7];
        fixedBuffer[i + 7] = src[srcOffset + i + 6];
      }

      ew.Position = 128;
      ew.Write(fixedBuffer, 0, imageSize);
      Asserts.Equal(128 + imageSize, ew.Position);
    }*/

    public static unsafe Bitmap DecompressDxt5a(
        byte[] src,
        int srcOffset,
        int width,
        int height) {
      const int blockSize = 4;
      var blockCountX = width / blockSize;
      var blockCountY = height / blockSize;

      var imageSize = width * height / 2;

      var monoTable = new byte[8];
      var rIndices = new byte[16];

      // TODO: Support grayscale?
      var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
      BitmapUtil.InvokeAsLocked(bitmap, bmpData => {
        var dst = (byte*) bmpData.Scan0.ToPointer();

        for (var i = 0; i < imageSize; i += 8) {
          var iOff = srcOffset + i;

          // Gathers up color palette.
          var mono0 = monoTable[0] = src[iOff + 0];
          var mono1 = monoTable[1] = src[iOff + 1];

          var useEightIndexMode = mono0 > mono1;
          if (useEightIndexMode) {
            monoTable[2] = (byte) ((6 * mono0 + 1 * mono1) / 7f);
            monoTable[3] = (byte) ((5 * mono0 + 2 * mono1) / 7f);
            monoTable[4] = (byte) ((4 * mono0 + 3 * mono1) / 7f);
            monoTable[5] = (byte) ((3 * mono0 + 4 * mono1) / 7f);
            monoTable[6] = (byte) ((2 * mono0 + 5 * mono1) / 7f);
            monoTable[7] = (byte) ((1 * mono0 + 6 * mono1) / 7f);
          } else {
            monoTable[2] = (byte) ((4 * mono0 + 1 * mono1) / 5f);
            monoTable[3] = (byte) ((3 * mono0 + 2 * mono1) / 5f);
            monoTable[4] = (byte) ((2 * mono0 + 3 * mono1) / 5f);
            monoTable[5] = (byte) ((1 * mono0 + 4 * mono1) / 5f);
            monoTable[6] = 0;
            monoTable[7] = 255;
          }

          // Gathers up color indices.
          ulong indices = 0;
          for (var b = 0; b < 6; ++b) {
            ulong part = src[iOff + 2 + b];
            part <<= (8 * b);
            indices |= part;
          }

          for (var ii = 0; ii < 16; ++ii) {
            rIndices[ii] = (byte) (indices & 7);
            indices >>= 3;
          }

          // Writes pixels to output image.
          // TODO: This might actually be flipped across the X/Y axis. This is
          // kept this way to align with the albedo texture for now.
          var tileIndex = i / 8;
          var tileY = tileIndex % blockCountY;
          var tileX = (tileIndex - tileY) / blockCountX;

          for (var j = 0; j < blockSize; j++) {
            for (var k = 0; k < blockSize; k++) {
              var value = monoTable[rIndices[(j * blockSize) + k]];

              var outX = (tileX * blockSize) + j;
              var outY = tileY * blockSize + k;

              var outIndex = (outY * width + outX) * 3;

              dst[outIndex] =
                  dst[outIndex + 1] = dst[outIndex + 2] = value;
            }
          }
        }
      });

      return bitmap;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Rgb565(int c, out int r, out int g, out int b) {
      r = (c & 0xf800) >> 8;
      g = (c & 0x07e0) >> 3;
      b = (c & 0x001f) << 3;
      r |= r >> 5;
      g |= g >> 6;
      b |= b >> 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Color(int r, int g, int b, int a) {
      return r << 16 | g << 8 | b | a << 24;
    }
  }
}