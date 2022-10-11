using fin.math;
using schema.util;
using static schema.SchemaStructureParser;
using System;


namespace ast.schema {
  /// <summary>
  ///   Cobbled together from various sources:
  ///     - https://github.com/vgmstream/vgmstream/blob/37cc12295c92ec6aa874118fb237bd3821970836/src/meta/ast.c
  /// </summary>
  public partial class Ast {
    private void ReadAdpcm_(EndianBinaryReader er) {
      var channelCount = this.StrmHeader.ChannelCount;
      var sampleCount = this.StrmHeader.SampleCount;

      this.ChannelData = new short[channelCount][];
      for (var i = 0; i < channelCount; ++i) {
        this.ChannelData[i] = new short[sampleCount];
      }

      Asserts.Equal(2, channelCount);

      // TODO: This doesn't look right???
      er.Position = 0x40;

      var adpcmData = new List<byte>();
      while (!er.Eof) {
        if (er.Eof) {
          break;
        }

        var blckHeader = er.ReadNew<BlckHeader>();

        // TODO: Does this need to be split up as left/right channels??
        var blockSize = 2 * blckHeader.BlockSizeInBytes;
        adpcmData.AddRange(er.ReadBytes((int)blockSize));
      }

      var frameCount = adpcmData.Count / 9;
      var expectedSampleCount = frameCount * 16;

      this.decode_ngc_afc(adpcmData, (int)sampleCount);
      ;
    }

    private static (short, short)[] afc_coefs = {
        (0, 0), (2048, 0), (0, 2048), (1024, 1024), (4096, -2048),
        (3584, -1536), (3072, -1024), (4608, -2560), (4200, -2248),
        (4800, -2300), (5120, -3072), (2048, -2048), (1024, -1024),
        (-1024, 1024), (-1024, 0), (-2048, 0),
    };

    void decode_ngc_afc(IList<byte> adpcmData,
                        int sampleCount) {
      var bytesPerFrame = 0x09;
      var samplesPerFrame = 16; // (bytesPerFrame - 1) * 2

      var frameCount = Math.Min(adpcmData.Count / bytesPerFrame,
                                sampleCount / samplesPerFrame);

      var hist1 = 0;
      var hist2 = 0;

      // TODO: Are these nested in the correct order??
      for (var frameIndex = 0; frameIndex < frameCount; ++frameIndex) {
        /* parse frame header */
        var frameOffset = frameIndex * bytesPerFrame;

        var frame = new byte[bytesPerFrame];
        for (var i = 0; i < bytesPerFrame; ++i) {
          frame[i] = adpcmData[frameOffset + i];
        }

        var scale = 1 << ((frame[0] >> 4) & 0xf);
        var index = (frame[0] & 0xf);
        var coef1 = afc_coefs[index].Item1;
        var coef2 = afc_coefs[index].Item2;

        /* decode nibbles */
        for (var sampleIndex = 0;
             sampleIndex < samplesPerFrame;
             sampleIndex++) {
          var nibbles = frame[0x01 + sampleIndex / 2];

          var isLeftChannel = (sampleIndex & 1) != 0;
          var sample = isLeftChannel
                           ? /* high nibble first */
                           get_low_nibble_signed(nibbles)
                           : get_high_nibble_signed(nibbles);
          sample = ((sample * scale) << 11);
          sample = (sample + coef1 * hist1 + coef2 * hist2) >> 11;

          sample = clamp16(sample);

          var sampleOffset = frameIndex * samplesPerFrame + (sampleIndex / 2);
          this.ChannelData[isLeftChannel ? 0 : 1][sampleOffset] = (short)sample;

          hist2 = hist1;
          hist1 = sample;
        }
      }
    }

    static int[] nibble_to_int = {
        0, 1, 2, 3, 4, 5, 6, 7, -8, -7, -6, -5, -4, -3, -2, -1
    };

    private static int get_high_nibble_signed(byte n) {
      /*return ((n&0x70)-(n&0x80))>>4;*/
      return nibble_to_int[n >> 4];
    }

    private static int get_low_nibble_signed(byte n) {
      /*return (n&7)-(n&8);*/
      return nibble_to_int[n & 0xf];
    }

    static int clamp16(int val) {
      if (val > 32767) return 32767;
      else if (val < -32768) return -32768;
      else return val;
    }
  }
}