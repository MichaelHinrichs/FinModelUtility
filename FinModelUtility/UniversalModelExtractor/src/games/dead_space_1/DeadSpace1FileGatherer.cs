using fin.io;
using fin.io.bundles;

using geo.schema.str;

using uni.platforms.desktop;


namespace uni.games.dead_space_1 {
  public class DeadSpace1FileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles(bool assert) {
      if (!SteamUtils.TryGetGameDirectory("Dead Space",
                                          out var deadSpaceDir,
                                          assert)) {
        return Enumerable.Empty<IFileBundle>();
      }

      var fileHierarchy = new FileHierarchy(deadSpaceDir);

      var strFile = fileHierarchy.Root.GetExistingFile("ch01_fit/ch01_flt.str");
      var str = strFile.Impl.ReadNew<StreamSetFile>();

      return Enumerable.Empty<IFileBundle>();
    }
  }
}