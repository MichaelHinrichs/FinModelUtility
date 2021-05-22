using fin.model;

namespace fin.exporter {
  public interface IExporter {
    void Export(string outputPath, IModel model);
  }
}