using ast.api;

using fin.audio;


namespace uni.ui {
  public class GlobalAudioLoader : IAudioLoader<IAudioFileBundle> {
    public IAudioBuffer<short> LoadAudio(IAudioManager<short> audioManager,
                                         IAudioFileBundle audioFileBundle)
      => audioFileBundle switch {
          AstAudioFileBundle astAudioFileBundle
              => new AstAudioLoader().LoadAudio(
                  audioManager, astAudioFileBundle),
          OggAudioFileBundle oggAudioFileBundle
              => new OggAudioLoader().LoadAudio(
                  audioManager, oggAudioFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(audioFileBundle))
      };
  }
}