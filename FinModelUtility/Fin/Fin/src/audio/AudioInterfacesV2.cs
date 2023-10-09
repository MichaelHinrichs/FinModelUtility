using System;
using System.Numerics;

using fin.audio;

namespace fin.audioV2 {
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


  // Data source types
  public interface IAudioDataSource<out TPcm> where TPcm : INumber<TPcm> {
    AudioChannelsType AudioChannelsType { get; }
    int Frequency { get; }

    TPcm GetPcm(AudioChannelType channelType, int sampleOffset);
  }


  public interface IReadOnlyStaticAudioBuffer<out TPcm>
      : IAudioDataSource<TPcm> where TPcm : INumber<TPcm> {
    int SampleCount { get; }
  }

  public interface IStaticAudioBuffer<TPcm>
      : IReadOnlyStaticAudioBuffer<TPcm> where TPcm : INumber<TPcm> {
    new int Frequency { get; set; }

    void SetMonoPcm(TPcm[] samples);

    void SetStereoPcm(TPcm[] leftChannelSamples,
                      TPcm[] rightChannelSamples);
  }


  public interface IDynamicAudio<out TPcm> : IAudioDataSource<TPcm>
      where TPcm : INumber<TPcm> {
    int BufferSize { get; }
  }

  public interface IBufferedAudioFeed<TPcm>
      : IDynamicAudio<TPcm>, IDisposable where TPcm : INumber<TPcm> {
    void PopulateNextBufferPcm(TPcm[] data);
  }

  // Playback types
  public interface IAudioManager<TPcm> : IDisposable
      where TPcm : INumber<TPcm> {
    // TODO: Add support for looping a certain section of audio

    IStaticAudioBuffer<TPcm> CreateStaticAudioBuffer();

    IBufferedAudioFeed<TPcm> CreateBufferedAudioFeed(
        AudioChannelsType audioChannelsType,
        int frequency,
        int bufferCount);

    IAudioPlayer<TPcm> AudioPlayer { get; }
  }

  public interface IAudioPlayer<TPcm> : IDisposable
      where TPcm : INumber<TPcm> {
    IAudioPlayback<TPcm> Create(IAudioDataSource<TPcm> buffer);
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

  /// <summary>
  ///   An actively playing sound. Certain attributes can be mutated on-the-fly,
  ///   like volume, offset, etc.
  /// </summary>
  public interface IAudioPlayback<out TNumber>
      : IStaticAudioFormat<TNumber>, IDisposable
      where TNumber : INumber<TNumber> {
    IAudioDataSource<TNumber> Source { get; }

    PlaybackState State { get; }

    void Play();
    void Stop();
    void Pause();

    int SampleOffset { get; set; }
    TNumber GetPcm(AudioChannelType channelType);

    float Volume { get; set; }

    LoopState LoopState { get; }
    bool Looping { get; set; }
  }
}