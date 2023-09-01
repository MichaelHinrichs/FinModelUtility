using fin.io;
using fin.model.io;

using UoT.memory;

namespace UoT.api {
  public class OotModelFileBundle(
      IFileHierarchyDirectory directory,
      IReadOnlySystemFile ootRom,
      IZFile zFile) : IModelFileBundle {
    public string GameName => "ocarina_of_time";

    public IFileHierarchyFile? MainFile => null;
    public IFileHierarchyDirectory Directory { get; } = directory;

    public IReadOnlySystemFile OotRom { get; } = ootRom;
    public IZFile ZFile { get; } = zFile;

    string IUiFile.HumanReadableName => ZFile.FileName;
    public string TrueFullPath => this.OotRom.FullPath;
  }
}