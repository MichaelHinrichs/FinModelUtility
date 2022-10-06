using OpenTK.Audio;


namespace fin.audio.impl.al {
  public partial class AlAudioManager : IAudioManager<short> {
    private readonly AudioContext context_ = new AudioContext();
  }
}