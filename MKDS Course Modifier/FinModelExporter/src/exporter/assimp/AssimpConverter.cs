using System.Linq;

using Assimp;

using fin.io;
using fin.model;
using fin.util.asserts;

namespace fin.exporter.assimp {
  public class AssimpConverter : IExporter {
    public void Export(IFile outputFile, IModel model) {
      var outputPath = outputFile.FullName;
      var outputExtension = outputFile.Extension;

      var inputFile = outputFile.CloneWithExtension(".glb");
      var inputPath = inputFile.FullName;
      var inputExtension = inputFile.Extension;

      var ctx = new AssimpContext();

      string exportFormatId;
      {
        var supportedImportFormatExtensions = ctx.GetSupportedImportFormats();
        Asserts.True(supportedImportFormatExtensions.Contains(inputExtension),
                     $"'{inputExtension}' is not a supported import format!");

        var supportedExportFormats = ctx.GetSupportedExportFormats();
        var exportFormatIds =
            supportedExportFormats
                .Where(exportFormat
                           => outputExtension == $".{exportFormat.FileExtension}")
                .Select(exportFormat => exportFormat.FormatId);
        Asserts.True(exportFormatIds.Any(),
                     $"'{outputExtension}' is not a supported export format!");

        exportFormatId = exportFormatIds.First();
      }

      var sc = ctx.ImportFile(inputPath);
      var success = ctx.ExportFile(sc, outputPath, exportFormatId);
      Asserts.True(success, "Failed to export model.");
    }
  }
}