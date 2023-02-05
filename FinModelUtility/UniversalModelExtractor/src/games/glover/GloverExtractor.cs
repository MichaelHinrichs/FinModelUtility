using glo.api;


namespace uni.games.glover {
  internal class GloverExtractor : IExtractor {
    public void ExtractAll()
      => ExtractorUtil.ExtractAll(new GloverModelFileGatherer(),
                                  new GloModelLoader());
  }
}