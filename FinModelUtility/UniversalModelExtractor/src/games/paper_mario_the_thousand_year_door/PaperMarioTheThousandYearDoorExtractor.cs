using fin.log;

using uni.platforms.gcn;

namespace uni.games.paper_mario_the_thousand_year_door {
  public class PaperMarioTheThousandYearDoorExtractor : IExtractor {
    private readonly ILogger logger_ =
        Logging.Create<PaperMarioTheThousandYearDoorExtractor>();

    public void ExtractAll() {
      if (!new GcnFileHierarchyExtractor().TryToExtractFromGame(
              "paper_mario_the_thousand_year_door",
              out var fileHierarchy)) {
        return;
      }
    }
  }
}