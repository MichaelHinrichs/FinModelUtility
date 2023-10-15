using j3d.api;

namespace uni.games.wind_waker {
  public class WindWakerMassExporter : IMassExporter {
    public void ExportAll()
      => ExporterUtil.ExportAllForCli(new WindWakerAnnotatedFileGatherer(),
                                  new BmdModelImporter());
  }
}