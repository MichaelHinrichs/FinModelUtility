using fin.audio;

using OpenTK.Audio;

namespace fin.ui.playback.al {
  public partial class AlAudioManager : IAudioManager<short> {
    private readonly AudioContext context_ = new();
    public bool IsDisposed { get; private set; }
    public IAudioPlayer<short> AudioPlayer { get; }

    public AlAudioManager() {
      this.AudioPlayer = new AlAudioPlayer();
    }

    ~AlAudioManager() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.IsDisposed = true;
      this.AudioPlayer.Dispose();
      this.context_.Dispose();
    }
  }
}