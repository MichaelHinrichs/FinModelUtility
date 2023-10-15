using j3d.api;

namespace uni.games.super_mario_sunshine {
  public class SuperMarioSunshineMassExporter : IMassExporter {
    public void ExportAll()
      => ExporterUtil.ExportAllForCli(new SuperMarioSunshineModelAnnotatedFileGatherer(),
                                  new BmdModelImporter());
  }
}