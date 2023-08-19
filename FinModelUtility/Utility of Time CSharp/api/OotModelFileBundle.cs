using fin.io;
using fin.model;

using UoT.memory;

namespace UoT.api {
  public class OotModelFileBundle : IModelFileBundle {
    public OotModelFileBundle(
        IFileHierarchyDirectory directory,
        ISystemFile ootRom,
        IZFile zFile) {
      this.Directory = directory;
      this.OotRom = ootRom;
      this.ZFile = zFile;
    }

    public string GameName => "ocarina_of_time";

    public IFileHierarchyFile? MainFile => null;
    public IFileHierarchyDirectory Directory { get; }

    public ISystemFile OotRom { get; }
    public IZFile ZFile { get; }

    string IUiFile.HumanReadableName => ZFile.FileName;
    public string TrueFullPath => this.OotRom.FullPath;
  }
}