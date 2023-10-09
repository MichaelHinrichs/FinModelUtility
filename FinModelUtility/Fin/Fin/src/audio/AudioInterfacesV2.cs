using System;
using System.Numerics;

namespace fin.audioV2 {
  // Playback types
  public interface IAudioManager<TPcm> : IDisposable
      where TPcm : INumber<TPcm> {
    // TODO: Add support for looping a certain section of audio

    IAudioBuffer<TPcm> CreateStaticAudioBuffer();

    IBufferedAudioFeed<TPcm> CreateBufferedAudioFeed(
        AudioChannelsType audioChannelsType,
        int frequency,
        int bufferCount);

    IAudioPlayer<TPcm> AudioPlayer { get; }
  }

  public interface IAudioPlayer<TPcm> : IDisposable
      where TPcm : INumber<TPcm> {
    IAudioPlayer<TPcm> CreateSubPlayer();

    IAudioPlayback<TPcm> Create(IAudioDataSource<TPcm> buffer);
    float Volume { get; set; }
  }

  public enum PlaybackState {
    UNDEFINED,
    STOPPED,
    PLAYING,
    PAUSED,
    DISPOSED,
  }

  public enum LoopState {
    UNDEFINED,
    DISABLED,
    NOT_LOOPING,
    LOOPING,
  }


  public interface IReadOnlyAudioPlayback<out TPcm>
      where TPcm : INumber<TPcm> {
    IAudioDataSource<TPcm> Source { get; }

    PlaybackState State { get; }

    int SampleOffset { get; }
    TPcm GetPcm(AudioChannelType channelType);

    float Volume { get; }

    LoopState LoopState { get; }
    bool Looping { get; }
  }

  /// <summary>
  ///   An actively playing sound. Certain attributes can be mutated on-the-fly,
  ///   like volume, offset, etc.
  /// </summary>
  public interface IAudioPlayback<out TPcm>
      : IReadOnlyAudioPlayback<TPcm>, IDisposable where TPcm : INumber<TPcm> {
    void Play();
    void Stop();
    void Pause();

    new int SampleOffset { get; set; }

    new float Volume { get; set; }

    new bool Looping { get; set; }
  }


  // Data source types
  public enum AudioChannelsType {
    UNDEFINED,
    MONO,
    STEREO,
  }

  public enum AudioChannelType {
    UNDEFINED,
    MONO,
    STEREO_LEFT,
    STEREO_RIGHT,
  }

  public interface IAudioDataSource<out TPcm> where TPcm : INumber<TPcm> {
    AudioChannelsType AudioChannelsType { get; }
    int Frequency { get; }
    int LengthInSamples { get; }

    TPcm GetPcm(AudioChannelType channelType, int sampleOffset);
  }


  public interface IAotAudioDataSource<out TPcm> : IAudioDataSource<TPcm>
      where TPcm : INumber<TPcm> { }

  public interface IReadOnlyAudioBuffer<out TPcm>
      : IAotAudioDataSource<TPcm> where TPcm : INumber<TPcm> {
    int SampleCount { get; }
  }

  public interface IAudioBuffer<TPcm>
      : IReadOnlyAudioBuffer<TPcm> where TPcm : INumber<TPcm> {
    new int Frequency { get; set; }

    void SetMonoPcm(TPcm[] samples);

    void SetStereoPcm(TPcm[] leftChannelSamples,
                      TPcm[] rightChannelSamples);
  }


  public interface IJitAudioDataSource<out TPcm> : IAudioDataSource<TPcm>
      where TPcm : INumber<TPcm> { }

  public interface IBufferedAudioFeed<TPcm>
      : IJitAudioDataSource<TPcm>, IDisposable where TPcm : INumber<TPcm> {
    void PopulateNextBufferPcm(TPcm[] data);
  }
}