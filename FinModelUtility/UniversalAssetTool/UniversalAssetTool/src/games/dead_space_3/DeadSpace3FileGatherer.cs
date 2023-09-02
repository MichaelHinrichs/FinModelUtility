using fin.io;
using fin.io.bundles;

using uni.platforms.desktop;

namespace uni.games.dead_space_3 {
  public class DeadSpace3FileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles() {
      if (!EaUtils.TryGetGameDirectory("Dead Space 3", out var deadSpace3Dir)) {
        yield break;
      }

      var originalGameFileHierarchy = new FileHierarchy(deadSpace3Dir);
    }
  }
}