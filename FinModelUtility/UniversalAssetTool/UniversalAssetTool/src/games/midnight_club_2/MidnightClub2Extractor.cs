using uni.ui;

namespace uni.games.midnight_club_2 {
  public class MidnightClub2Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(new MidnightClub2FileGatherer(),
                                  new GlobalModelImporter());
  }
}