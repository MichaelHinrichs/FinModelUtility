using modl.api;

namespace uni.games.battalion_wars_2 {
  public class BattalionWars2MassExporter : IMassExporter {
    public void ExportAll()
      => ExporterUtil.ExportAllForCli(new BattalionWars2AnnotatedFileGatherer(),
                                  new BattalionWarsModelImporter());
  }
}