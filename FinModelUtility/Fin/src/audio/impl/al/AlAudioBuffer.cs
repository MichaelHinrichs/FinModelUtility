using fin.io;
using fin.util.asserts;
using NVorbis;
using NVorbis.Contracts;
using System;


namespace fin.audio.impl.al {
  public partial class AlAudioManager {
    public IAudioBuffer<short> LoadIntoBuffer(IFile file) {
      Asserts.Equal(".ogg", file.Extension.ToLower());

      using var ogg = new VorbisReader(file.FullName);
      return new AlAudioBuffer(ogg);
    }

    private class AlAudioBuffer : IAudioBuffer<short> {
      private readonly short[][] channels_;

      public AlAudioBuffer(IVorbisReader ogg) {
        var sampleCount = this.SampleCount = (int)ogg.TotalSamples;

        var channelCount = ogg.Channels;
        this.AudioChannelsType = channelCount switch {
            1 => AudioChannelsType.MONO,
            2 => AudioChannelsType.STEREO,
        };

        this.Frequency = ogg.SampleRate;

        var floatCount = channelCount * sampleCount;
        var floatPcm = new float[floatCount];
        ogg.ReadSamples(floatPcm, 0, floatCount);

        this.channels_ = new short[channelCount][];
        for (var c = 0; c < channelCount; ++c) {
          this.channels_[c] = new short[sampleCount];
        }

        for (var i = 0; i < sampleCount; ++i) {
          for (var c = 0; c < channelCount; ++c) {
            var floatSample = floatPcm[2 * i + c];

            var floatMin = -1f;
            var floatMax = 1f;

            var normalizedFloatSample =
                (MathF.Max(floatMin, Math.Min(floatSample, floatMax)) -
                 floatMin) / (floatMax - floatMin);

            float shortMin = short.MinValue;
            float shortMax = short.MaxValue;

            var shortSample =
                (short)Math.Round(shortMin +
                                  normalizedFloatSample *
                                  (shortMax - shortMin));

            this.channels_[c][i] = shortSample;
          }
        }
      }

      public AudioChannelsType AudioChannelsType { get; }
      public int Frequency { get; }
      public int SampleCount { get; }

      public short GetPcm(AudioChannelType channelType, int sampleOffset)
        => this.channels_[channelType switch {
            AudioChannelType.MONO         => 0,
            AudioChannelType.STEREO_LEFT  => 0,
            AudioChannelType.STEREO_RIGHT => 1
        }][sampleOffset];
    }
  }
}