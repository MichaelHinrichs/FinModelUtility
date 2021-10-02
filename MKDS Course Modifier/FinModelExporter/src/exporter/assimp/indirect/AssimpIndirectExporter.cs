using System.IO;
using System.Linq;

using Assimp;

using fin.exporter.gltf;
using fin.io;
using fin.model;
using fin.src.exporter.assimp.indirect;
using fin.util.asserts;

using WrapMode = fin.model.WrapMode;

namespace fin.exporter.assimp {
  using FinBlendMode = fin.model.BlendMode;

  public class AssimpIndirectExporter : IExporter {
    // You can bet your ass I'm gonna prefix everything with ass.

    public void Export(IFile outputFile, IModel model) {
      var outputPath = outputFile.FullName;
      var outputExtension = outputFile.Extension;

      using var ctx = new AssimpContext();

      string exportFormatId;
      {
        var supportedExportFormats = ctx.GetSupportedExportFormats();
        var exportFormatIds =
            supportedExportFormats
                .Where(exportFormat
                           => outputExtension ==
                              $".{exportFormat.FileExtension}")
                .Select(exportFormat => exportFormat.FormatId);
        Asserts.True(exportFormatIds.Any(),
                     $"'{outputExtension}' is not a supported export format!");

        exportFormatId = exportFormatIds.First();
      }

      var assScene = new Scene();
      assScene.RootNode = new Node("ROOT");

      var inputFile = outputFile.CloneWithExtension(".glb");
      var inputPath = inputFile.FullName;

      var gltfExporter = new GltfExporter();
      gltfExporter.UvIndices = true;
      gltfExporter.Embedded = true;
      gltfExporter.Export(inputFile, model);
      var sc = ctx.ImportFile(inputPath);

      // Importing the pre-generated GLTF file does most of the hard work off
      // the bat: generating the mesh with properly weighted bones.

      // Bone orientation is already correct, you just need to enable
      // "Automatic Bone Orientation" if importing in Blender.

      new AssimpIndirectAnimationFixer().Fix(model, sc);
      new AssimpIndirectUvFixer().Fix(model, sc);
      new AssimpIndirectTextureFixer().Fix(model, sc);

      var success = ctx.ExportFile(sc, outputPath, exportFormatId);
      Asserts.True(success, "Failed to export model.");

      File.Delete(inputPath);
    }
  }
}