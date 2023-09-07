using glo.api;

namespace uni.games.glover {
  internal class GloverExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAllForCli(new GloverModelAnnotatedFileGatherer(),
                                  new GloModelImporter());
  }
}