using fin.io;
using fin.io.bundles;

using uni.platforms.desktop;

namespace uni.games.dead_space_2 {
  public class DeadSpace2FileGatherer : IFileBundleGatherer<IFileBundle> {
    public IEnumerable<IFileBundle> GatherFileBundles() {
      if (!EaUtils.TryGetGameDirectory("Dead Space 2", out var deadSpace2Dir)) {
        yield break;
      }

      var originalGameFileHierarchy = new FileHierarchy(deadSpace2Dir);
    }
  }
}