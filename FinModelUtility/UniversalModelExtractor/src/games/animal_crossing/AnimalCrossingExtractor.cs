using fin.log;

using uni.platforms;
using uni.platforms.gcn;

namespace uni.games.animal_crossing {
  public class AnimalCrossingExtractor : IExtractor {
    public void ExtractAll() {
      var animalCrossingRom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "animal_crossing.gcm");

      var options = GcnFileHierarchyExtractor.Options.Standard();

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              animalCrossingRom);

      var logger = Logging.Create<AnimalCrossingExtractor>();

      // TODO: Extract models via display lists
    }
  }
}