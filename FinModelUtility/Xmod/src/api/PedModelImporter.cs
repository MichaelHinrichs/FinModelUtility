using fin.io;
using fin.model;
using fin.model.io;
using fin.model.io.importer;

namespace xmod.api {
  public class PedModelFileBundle : IModelFileBundle {
    public string GameName => "midnight_club_2";
    public IReadOnlyTreeFile MainFile => this.PedFile;
    public required IReadOnlyTreeFile PedFile { get; init; }
  }

  public class PedModelImporter : IModelImporter<PedModelFileBundle> {
    public IModel ImportModel(PedModelFileBundle modelFileBundle) {
      throw new NotImplementedException();
    }
  }
}