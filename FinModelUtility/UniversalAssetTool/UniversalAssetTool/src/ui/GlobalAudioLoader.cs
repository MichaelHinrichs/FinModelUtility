using ast.api;

using fin.audio;
using fin.audio.ogg;

namespace uni.ui {
  public class GlobalAudioReader : IAudioReader<IAudioFileBundle> {
    public IAudioBuffer<short> ReadAudio(IAudioManager<short> audioManager,
                                         IAudioFileBundle audioFileBundle)
      => audioFileBundle switch {
          AstAudioFileBundle astAudioFileBundle
              => new AstAudioReader().ReadAudio(
                  audioManager, astAudioFileBundle),
          OggAudioFileBundle oggAudioFileBundle
              => new OggAudioReader().ReadAudio(
                  audioManager, oggAudioFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(audioFileBundle))
      };
  }
}