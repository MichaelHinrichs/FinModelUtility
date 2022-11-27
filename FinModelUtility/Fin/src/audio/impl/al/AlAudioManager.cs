using OpenTK.Audio;
using System;


namespace fin.audio.impl.al {
  public partial class AlAudioManager : IAudioManager<short> {
    private readonly AudioContext context_ = new();

    ~AlAudioManager() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.context_.Dispose();
    }
  }
}