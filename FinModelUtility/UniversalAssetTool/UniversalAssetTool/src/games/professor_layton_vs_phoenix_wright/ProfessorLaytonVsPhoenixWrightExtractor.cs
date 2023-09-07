using level5.api;

namespace uni.games.professor_layton_vs_phoenix_wright {
  public class ProfessorLaytonVsPhoenixWrightExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(
          new ProfessorLaytonVsPhoenixWrightModelAnnotatedFileGatherer(),
          new XcModelImporter());
  }
}