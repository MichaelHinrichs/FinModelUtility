using fin.log;

using uni.platforms;
using uni.platforms.gcn;

namespace uni.games.animal_crossing {
  public class AnimalCrossingExtractor {
    public void ExtractAll() {
      var animalCrossingRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "animal_crossing.gcm");

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(animalCrossingRom);

      var logger = Logging.Create<AnimalCrossingExtractor>();

      // TODO: Extract models via display lists
    }
  }
}