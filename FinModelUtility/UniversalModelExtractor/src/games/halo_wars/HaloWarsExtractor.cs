using hw.api;

namespace uni.games.halo_wars {
  internal class HaloWarsExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(
        new HaloWarsModelFileGatherer(),
        new HaloWarsModelLoader());

      /*new HaloWarsTools.Program().Run(
          DirectoryConstants.ROMS_DIRECTORY.GetSubdir("halo_wars", true).FullName,
          DirectoryConstants.OUT_DIRECTORY.GetSubdir("halo_wars", true).FullName);*/
  }
}