using fin.io;
using fin.io.bundles;

using uni.platforms.desktop;

namespace uni.games.dead_space_2 {
  public class DeadSpace2FileBundleGatherer : IAnnotatedFileBundleGatherer {
    public IEnumerable<IAnnotatedFileBundle> GatherFileBundles() {
      if (!EaUtils.TryGetGameDirectory("Dead Space 2", out var deadSpace2Dir)) {
        yield break;
      }

      var originalGameFileHierarchy =
          new FileHierarchy("dead_space_2", deadSpace2Dir);
    }
  }
}