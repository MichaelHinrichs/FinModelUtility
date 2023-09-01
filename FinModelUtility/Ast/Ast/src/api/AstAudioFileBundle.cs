using fin.audio;
using fin.io;

namespace ast.api {
  public class AstAudioFileBundle : IAudioFileBundle {
    public required string GameName { get; init; }

    public IFileHierarchyFile MainFile => this.AstFile;

    public required IFileHierarchyFile AstFile { get; init; }
  }
}
