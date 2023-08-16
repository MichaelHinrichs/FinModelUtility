using fin.io.bundles;

namespace fin.audio {
  public interface IAudioFileBundle : IFileBundle { }

  public interface IAudioLoader<in TAudioFileBundle>
      where TAudioFileBundle : IAudioFileBundle {
    IAudioBuffer<short> LoadAudio(
        IAudioManager<short> audioManager,
        TAudioFileBundle audioFileBundle);
  }
}