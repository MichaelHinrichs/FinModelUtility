using j3d.api;

namespace uni.games.mario_kart_double_dash {
  public class MarioKartDoubleDashMassExporter : IMassExporter {
    public void ExportAll()
      => ExporterUtil.ExportAllForCli(new MarioKartDoubleDashAnnotatedFileGatherer(),
                                  new BmdModelImporter());
  }
}