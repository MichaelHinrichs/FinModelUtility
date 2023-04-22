using fin.io;
using fin.model;
using fin.util.hex;


namespace UoT.api {
  public class OotModelFileBundle : IModelFileBundle {
    public OotModelFileBundle(
        IFileHierarchyDirectory directory,
        ISystemFile ootRom,
        uint offset) {
      this.Directory = directory;
      this.OotRom = ootRom;
      this.Offset = offset;
    }

    public string GameName => "ocarina_of_time";

    public IFileHierarchyFile? MainFile => null;
    public IFileHierarchyDirectory Directory { get; }

    public ISystemFile OotRom { get; }
    public uint Offset { get; }

    string IUiFile.BetterName => this.Offset.ToHex();
    public string TrueFullName => this.OotRom.FullName;
  }
}