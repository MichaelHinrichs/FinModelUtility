using fin.io;
using fin.io.bundles;


namespace sm64.api {
  public class Sm64LevelFileBundle : IFileBundle {
    public Sm64LevelFileBundle(
        IFile sm64Rom,
        LevelId levelId) {
      this.Sm64Rom = sm64Rom;
      this.LevelId = levelId;
    }

    public IFileHierarchyFile MainFile => null!;

    public IFile Sm64Rom { get; }
    public LevelId LevelId { get; }
    string IUiFile.FileName => $"{LevelId}";
  }
}