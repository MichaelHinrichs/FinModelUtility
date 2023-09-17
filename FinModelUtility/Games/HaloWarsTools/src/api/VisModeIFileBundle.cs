using fin.io;

using HaloWarsTools;

namespace hw.api {
  public class VisModelFileBundle : IHaloWarsModelFileBundle {
    public VisModelFileBundle(IReadOnlyTreeFile visFile, HWContext context) {
      this.VisFile = visFile;
      this.Context = context;
    }

    public string GameName => "halo_wars";
    public IReadOnlyTreeFile MainFile => this.VisFile;
    public IReadOnlyTreeFile VisFile { get; }

    public HWContext Context { get; }
  }
}