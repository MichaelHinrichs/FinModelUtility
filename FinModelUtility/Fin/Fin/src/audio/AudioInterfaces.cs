using System;
using System.Numerics;

namespace fin.audio {
  public interface IAudioManager<TNumber> : IDisposable
      where TNumber : INumber<TNumber> {
    // TODO: Add support for looping a certain section of audio

    IMutableAudioBuffer<TNumber> CreateMutableBuffer();

    IBufferAudioStream<TNumber> CreateBufferAudioStream(
        IAudioBuffer<TNumber> buffer);

    IAudioSource<TNumber> CreateAudioSource();

    ICircularQueueActiveSound<TNumber> CreateBufferedSound(
        AudioChannelsType audioChannelsType,
        int frequency,
        int bufferCount);
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
  }

  public interface IStaticAudioFormat<out TNumber>
      : IAudioFormat<TNumber> where TNumber : INumber<TNumber> {
    int SampleCount { get; }
  }


  public interface IAudioData<out TNumber> : IStaticAudioFormat<TNumber>
      where TNumber : INumber<TNumber> {
    TNumber GetPcm(AudioChannelType channelType, int sampleOffset);
  }


  /// <summary>
  ///   Type for storing static audio data, e.g. a loaded audio file.
  /// </summary>
  public interface IAudioBuffer<out TNumber> : IAudioData<TNumber>
      where TNumber : INumber<TNumber> { }

  /// <summary>
  ///   Type for storing static audio data that can be mutated dynamically.
  /// </summary>
  public interface IMutableAudioBuffer<TNumber> : IAudioBuffer<TNumber>
      where TNumber : INumber<TNumber> {
    new int Frequency { get; set; }

    void SetPcm(TNumber[][] channelSamples);

    void SetMonoPcm(TNumber[] samples);

    void SetStereoPcm(TNumber[] leftChannelSamples,
                      TNumber[] rightChannelSamples);
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


  public interface IAudioSource<TNumber> where TNumber : INumber<TNumber> {
    IActiveSound<TNumber> Create(IAudioBuffer<TNumber> buffer);
    IActiveSound<TNumber> Create(IAudioStream<TNumber> stream);


    IActiveMusic<TNumber> CreateMusic(IAudioBuffer<TNumber> introBuffer,
                                      IAudioBuffer<TNumber> loopBuffer);

    IActiveMusic<TNumber> CreateMusic(IAudioStream<TNumber> introStream,
                                      IAudioStream<TNumber> loopStream);
  }

  public enum SoundState {
    UNDEFINED,
    STOPPED,
    PLAYING,
    PAUSED,
    DISPOSED,
  }

  /// <summary>
  ///   An actively playing sound. Certain attributes can be mutated on-the-fly,
  ///   like volume, offset, etc.
  /// </summary>
  public interface IActiveSound<out TNumber>
      : IStaticAudioFormat<TNumber>, IDisposable
      where TNumber : INumber<TNumber> {
    IAudioStream<TNumber> Stream { get; }

    SoundState State { get; }

    void Play();
    void Stop();
    void Pause();

    int SampleOffset { get; set; }
    TNumber GetPcm(AudioChannelType channelType);

    float Volume { get; set; }
    bool Looping { get; set; }
  }

  /// <summary>
  ///   Actively playing music. Certain attributes can be mutated on-the-fly,
  ///   like volume, offset, etc.
  ///
  ///   Different from an actively played sound because this assumes one audio
  ///   stream will be used to play an intro, and another a loop.
  /// </summary>
  // TODO: Probably should represented in some other way, this is clearly just
  // one case for chained audio
  public interface IActiveMusic<out TNumber>
      : IStaticAudioFormat<TNumber>, IDisposable
      where TNumber : INumber<TNumber> {
    IAudioStream<TNumber> IntroStream { get; }
    IAudioStream<TNumber> LoopStream { get; }

    SoundState State { get; }

    void Play();
    void Pause();

    float Volume { get; set; }
  }

  /// <summary>
  ///   An actively played sound that represents some kind of live, streaming
  ///   audio.
  /// </summary>
  public interface ICircularQueueActiveSound<TNumber>
      : IAudioFormat<TNumber>, IDisposable where TNumber : INumber<TNumber> {
    SoundState State { get; }

    void Play();
    void Stop();
    void Pause();

    float Volume { get; set; }

    uint QueuedSamples { get; }
    int BufferCount { get; }

    void PopulateNextBufferPcm(TNumber[] data);
  }
}