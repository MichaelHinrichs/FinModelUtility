using fin.io;
using fin.model.io;

namespace dat.api {
  public class DatModelFileBundle : IModelFileBundle {
    public required string GameName { get; init; }

    public IReadOnlyTreeFile MainFile => this.DatFile;
    public required IReadOnlyTreeFile DatFile { get; init; }
  }
}