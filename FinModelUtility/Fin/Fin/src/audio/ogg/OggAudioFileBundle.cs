using fin.io;

namespace fin.audio.ogg {
  public class OggAudioFileBundle(IFileHierarchyFile oggFile) : IAudioFileBundle {
    public string? GameName { get; init; }
    public IFileHierarchyFile MainFile => this.OggFile;

    public IFileHierarchyFile OggFile { get; } = oggFile;
  }
}
