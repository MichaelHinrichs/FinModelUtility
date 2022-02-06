using uni.platforms;
using uni.platforms.desktop;

namespace uni.games.halo_wars {
  internal class HaloWarsExtractor {
    public void ExtractAll() {
      new HaloWarsTools.Program().Run(
          DirectoryConstants.ROMS_DIRECTORY.GetSubdir("halo_wars", true).FullName,
          DirectoryConstants.OUT_DIRECTORY.GetSubdir("halo_wars", true).FullName);
    }
  }
}