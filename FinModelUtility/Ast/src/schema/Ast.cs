using schema;


namespace ast.schema {
  public class Ast : IDeserializable {
    public StrmHeader StrmHeader { get; } = new();

    public short[][] ChannelData { get; private set; }

    public void Read(EndianBinaryReader er) {
      this.StrmHeader.Read(er);

      // TODO: This is almost certainly wrong
      switch (this.StrmHeader.Format) {
        case AstAudioFormat.ADPCM: {
          this.ReadAdpcm_(er);
          break;
        }
        case AstAudioFormat.PCM16: {
          this.ReadPcm16_(er);
          break;
        }
        default: throw new NotImplementedException();
      }
    }

    private void ReadAdpcm_(EndianBinaryReader er) {
      var channelCount = this.StrmHeader.ChannelCount;
      var sampleCount = this.StrmHeader.SampleCount;

      this.ChannelData = new short[channelCount][];
      for (var i = 0; i < channelCount; ++i) {
        this.ChannelData[i] = new short[sampleCount];
      }

      for (var i = 0; i < channelCount; i += 2) {
        // TODO: This doesn't look right???
        er.Position = 0x40;

        for (var currentSample = 0; currentSample < sampleCount;) {
          if (er.Eof) {
            break;
          }

          var blckHeader = er.ReadNew<BlckHeader>();

          var blockSizeInBytes = blckHeader.BlockSizeInBytes;
          var blockSizeInShorts = blockSizeInBytes / 2;

          for (var chan = 0; chan < channelCount; chan++) {
            if (chan / 2 == i / 2) {
              var shorts =
                  er.ReadInt16s(
                      (int)Math.Min(blockSizeInShorts,
                                    (er.Length - er.Position) / 2));

              if (shorts.Length != blockSizeInShorts) {
                if (currentSample + blockSizeInBytes / 2 >= sampleCount &&
                    chan + 1 == channelCount) {
                  /* it seems that the last block is cut off? */
                  /*printf("last block missing %#x bytes, filling with zero\n",
                          bytes_short);
                  memset(dumpbuf + bytes_read, 0, bytes_short);*/
                }
              }

              for (var s = 0; s < shorts.Length; ++s) {
                if (currentSample + s < sampleCount) {
                  this.ChannelData[chan][currentSample + s] = shorts[s];
                }
              }
            } else {
              // Skips samples
              er.Position += blockSizeInBytes;
            }
          }

          currentSample += (int)blockSizeInShorts;
        }
      }
    }

    private void ReadPcm16_(EndianBinaryReader er) {
      var channelCount = this.StrmHeader.ChannelCount;
      var sampleCount = this.StrmHeader.SampleCount;

      this.ChannelData = new short[channelCount][];
      for (var i = 0; i < channelCount; ++i) {
        this.ChannelData[i] = new short[sampleCount];
      }

      for (var i = 0; i < channelCount; i += 2) {
        // TODO: This doesn't look right???
        er.Position = 0x40;

        for (var currentSample = 0; currentSample < sampleCount;) {
          if (er.Eof) {
            break;
          }

          var blckHeader = er.ReadNew<BlckHeader>();

          var blockSizeInBytes = blckHeader.BlockSizeInBytes;
          var blockSizeInShorts = blockSizeInBytes / 2;

          for (var chan = 0; chan < channelCount; chan++) {
            if (chan / 2 == i / 2) {
              var shorts =
                  er.ReadInt16s(
                      (int)Math.Min(blockSizeInShorts,
                                    (er.Length - er.Position) / 2));

              if (shorts.Length != blockSizeInShorts) {
                if (currentSample + blockSizeInBytes / 2 >= sampleCount &&
                    chan + 1 == channelCount) {
                  /* it seems that the last block is cut off? */
                  /*printf("last block missing %#x bytes, filling with zero\n",
                          bytes_short);
                  memset(dumpbuf + bytes_read, 0, bytes_short);*/
                }
              }

              for (var s = 0; s < shorts.Length; ++s) {
                if (currentSample + s < sampleCount) {
                  this.ChannelData[chan][currentSample + s] = shorts[s];
                }
              }
            } else {
              // Skips samples
              er.Position += blockSizeInBytes;
            }
          }

          currentSample += (int)blockSizeInShorts;
        }
      }
    }
  }
}