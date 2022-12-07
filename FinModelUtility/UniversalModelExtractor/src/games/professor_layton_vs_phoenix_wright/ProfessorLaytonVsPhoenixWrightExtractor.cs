using level5.api;


namespace uni.games.professor_layton_vs_phoenix_wright {
  public class ProfessorLaytonVsPhoenixWrightExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(
        new ProfessorLaytonVsPhoenixWrightModelFileGatherer(),
                                  new XcModelLoader());
  }
}