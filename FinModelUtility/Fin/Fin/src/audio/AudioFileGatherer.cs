using fin.io.bundles;

namespace fin.audio {
  public interface IAudioFileBundle : IFileBundle { }

  public interface IAudioReader<in TAudioFileBundle>
      where TAudioFileBundle : IAudioFileBundle {
    IAudioBuffer<short> ReadAudio(
        IAudioManager<short> audioManager,
        TAudioFileBundle audioFileBundle);
  }
}