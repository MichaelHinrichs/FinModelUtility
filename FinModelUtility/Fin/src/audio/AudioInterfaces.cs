using fin.io;
using fin.model;
using System;
using System.Numerics;


namespace fin.audio {
  // - concepts:
  //   - buffers hold PCM data
  //   - 


  public interface IAudioManager<TNumber> where TNumber : INumber<TNumber> {
    IAudioBuffer<TNumber> LoadIntoBuffer(IFile file);

    IBufferAudioStream<TNumber> CreateBufferAudioStream(
        IAudioBuffer<TNumber> buffer);

    IAudioSource<TNumber> CreateAudioSource();
  }


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


  public interface IAudioFormat<out TNumber> where TNumber : INumber<TNumber> {
    AudioChannelsType AudioChannelsType { get; }
    int Frequency { get; }
    long Length { get; }
  }

  public interface IAudioData<out TNumber> : IAudioFormat<TNumber>
      where TNumber : INumber<TNumber> {
    TNumber GetPcm(AudioChannelType channelType, long index);
  }


  /// <summary>
  ///   Type for storing static audio data, e.g. a loaded audio file.
  /// </summary>
  public interface IAudioBuffer<out TNumber> : IAudioData<TNumber>, IDisposable
      where TNumber : INumber<TNumber> {
    string Name { get; }
  }

  /// <summary>
  ///   Type that streams out audio data. Can be used as an input for other
  ///   streams to apply effects, or played out to the speakers via an audio
  ///   source.
  /// </summary>
  public interface IAudioStream<out TNumber>
      : IAudioData<TNumber> where TNumber : INumber<TNumber> { }

  public interface IBufferAudioStream<TNumber> : IAudioStream<TNumber>
      where TNumber : INumber<TNumber> {
    IAudioBuffer<TNumber> Buffer { get; }
    bool Reversed { get; set; }
  }

  public interface IEffectAudioStream<TNumber> : IAudioStream<TNumber>
      where TNumber : INumber<TNumber> {
    IAudioStream<TNumber> InputStream { get; set; }
  }

  public interface IVolumeEffectAudioStream<TNumber>
      : IEffectAudioStream<TNumber> where TNumber : INumber<TNumber> {
    float Volume { get; set; }
  }

  public interface ISpeedEffectAudioStream<TNumber>
      : IEffectAudioStream<TNumber> where TNumber : INumber<TNumber> {
    float Speed { get; set; }
  }

  public interface IDopplerEffectAudioStream<TNumber>
      : IEffectAudioStream<TNumber> where TNumber : INumber<TNumber> {
    IVector3 RelativePosition { get; set; }
    IVector3 RelativeVelocity { get; set; }
  }


  public interface IAudioSource<TNumber> where TNumber : INumber<TNumber> {
    IActiveSound<TNumber> Create(IAudioBuffer<TNumber> buffer);
    IActiveSound<TNumber> Play(IAudioBuffer<TNumber> buffer);

    IActiveSound<TNumber> Create(IAudioStream<TNumber> stream);
    IActiveSound<TNumber> Play(IAudioStream<TNumber> stream);
  }

  public enum SoundState {
    UNDEFINED,
    STOPPED,
    PLAYING,
    PAUSED,
    DISPOSED,
  }

  public interface IActiveSound<out TNumber> : IDisposable
      where TNumber : INumber<TNumber> {
    SoundState State { get; }

    void Play();
    void Stop();
    void Pause();

    long Index { get; set; }
    TNumber GetPcm(AudioChannelType channelType);

    bool Looping { get; set; }
  }
}