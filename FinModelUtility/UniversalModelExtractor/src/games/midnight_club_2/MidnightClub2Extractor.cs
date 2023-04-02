using uni.ui;


namespace uni.games.midnight_club_2 {
  public class MidnightClub2Extractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new MidnightClub2FileGatherer(),
                                  new GlobalModelLoader(),
                                  false);
  }
}