using ast.api;

using fin.audio;
using fin.audio.io;
using fin.audio.io.importers;
using fin.audio.io.importers.ogg;

namespace uni.ui.winforms {
  public class GlobalAudioReader : IAudioImporter<IAudioFileBundle> {
    public IAudioBuffer<short> ImportAudio(IAudioManager<short> audioManager,
                                           IAudioFileBundle audioFileBundle)
      => audioFileBundle switch {
          AstAudioFileBundle astAudioFileBundle
              => new AstAudioReader().ImportAudio(
                  audioManager,
                  astAudioFileBundle),
          OggAudioFileBundle oggAudioFileBundle
              => new OggAudioImporter().ImportAudio(
                  audioManager,
                  oggAudioFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(audioFileBundle))
      };
  }
}