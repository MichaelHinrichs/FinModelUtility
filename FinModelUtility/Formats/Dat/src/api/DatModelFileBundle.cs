using fin.io;
using fin.model.io;

namespace dat.api {
  public class DatModelFileBundle : IModelFileBundle {
    public required string GameName { get; init; }

    public IReadOnlyTreeFile MainFile => this.PrimaryDatFile;
    public required IReadOnlyTreeFile PrimaryDatFile { get; init; }
    public IReadOnlyTreeFile? AnimationDatFile { get; init; }
  }
}