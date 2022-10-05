using fin.model;


namespace fin.audio {
  // - concepts:
  //   - buffers hold PCM data
  //   - 

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


  /// <summary>
  ///   Type for storing static audio data, e.g. a loaded audio file.
  /// </summary>
  public interface IAudioBuffer {
    string Name { get; }
    AudioChannelsType AudioChannelsType { get; }
    int Frequency { get; }

    byte GetPcm(AudioChannelType channelType, long index); 
    long Length { get; }
  }

  /// <summary>
  ///   Type that streams out audio data. Can be used as an input for other
  ///   streams to apply effects, or played out to the speakers via an audio
  ///   source.
  /// </summary>
  public interface IAudioStream { }

  public interface IBufferAudioStream : IAudioStream {
    IAudioBuffer InputBuffer { get; set; }
    long Position { get; set; }

    bool Looping { get; set; }
    bool Reversed { get; set; }
  }

  public interface IEffectAudioStream : IAudioStream {
    IAudioStream InputStream { get; set; }
  }

  public interface IVolumeEffectAudioStream : IEffectAudioStream {
    float Volume { get; set; }
  }

  public interface ISpeedEffectAudioStream : IEffectAudioStream {
    float Speed { get; set; }
  }

  public interface IDopplerEffectAudioStream : IEffectAudioStream {
    IVector3 RelativePosition { get; set; }
    IVector3 RelativeVelocity { get; set; }
  }


  public interface IAudioSource {
    void PlayStream(IAudioStream stream);
  }
}