using fin.io;
using fin.model;

namespace fin.exporter {
  public interface IExporter {
    void Export(IFile outputFile, IModel model);
  }
}