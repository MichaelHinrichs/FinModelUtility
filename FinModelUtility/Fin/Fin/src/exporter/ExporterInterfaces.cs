using fin.io;
using fin.model;

namespace fin.exporter {
  public interface IExporterParams {
    ISystemFile OutputFile { get; }
    IModel Model { get; }
    float Scale { get; }
  }

  public class ExporterParams : IExporterParams {
    public required ISystemFile OutputFile { get; set; }
    public required IModel Model { get; set; }
    public float Scale { get; set; } = 1;
  }

  public interface IExporter {
    void Export(IExporterParams exporterParams);
  }
}