using fin.io;
using fin.model;


namespace UoT.api {
  public class OotModelFileBundle : IModelFileBundle {
    public OotModelFileBundle(
        IFileHierarchyDirectory directory,
        ISystemFile ootRom,
        string fileName,
        uint offset,
        uint length) {
      this.Directory = directory;
      this.OotRom = ootRom;
      this.FileName = fileName;
      this.Offset = offset;
      this.Length = length;
    }

    public string GameName => "ocarina_of_time";

    public IFileHierarchyFile? MainFile => null;
    public IFileHierarchyDirectory Directory { get; }

    public ISystemFile OotRom { get; }
    public string FileName { get; }
    public uint Offset { get; }
    public uint Length { get; }

    string IUiFile.BetterName => FileName;
    public string TrueFullName => this.OotRom.FullName;
  }
}