using fin.log;

using uni.platforms.gcn;

namespace uni.games.paper_mario_the_thousand_year_door {
  public class PaperMarioTheThousandYearDoorMassExporter : IMassExporter {
    private readonly ILogger logger_ =
        Logging.Create<PaperMarioTheThousandYearDoorMassExporter>();

    public void ExportAll() {
      if (!new GcnFileHierarchyExtractor().TryToExtractFromGame(
              "paper_mario_the_thousand_year_door",
              out var fileHierarchy)) {
        return;
      }
    }
  }
}