using System.IO;
using System.Linq;
using Assimp;
using fin.exporter.gltf;
using fin.exporter.gltf.lowlevel;
using fin.io;
using fin.model;
using fin.util.asserts;
using SharpGLTF.Schema2;
using Scene = Assimp.Scene;


namespace fin.exporter.assimp.indirect {
  public class AssimpIndirectExporter : IExporter {
    // You can bet your ass I'm gonna prefix everything with ass.

    public bool LowLevel { get; set; }

    public void Export(IFile outputFile, IModel model)
      => Export(outputFile,
                new[] {".fbx", ".glb"},
                model);

    public void Export(IFile outputFile,
                       string[] exportedFormats,
                       IModel model) {
      if (exportedFormats.Length == 0) {
        return;
      }

      exportedFormats =
          exportedFormats.Select(exportedFormat => exportedFormat.ToLower())
                         .ToArray();

      IGltfExporter gltfExporter = !this.LowLevel
                                       ? new GltfExporter()
                                       : new LowLevelGltfExporter();

      var isGltfFormat = (string format) => format is ".gltf" or ".glb";
      var gltfFormats = exportedFormats
                        .Where(isGltfFormat)
                        .ToArray();
      var nonGltfFormats = exportedFormats
                           .Where(exportedFormat =>
                                      !isGltfFormat(exportedFormat))
                           .ToArray();

      if (gltfFormats.Length > 0) {
        gltfExporter.UvIndices = false;
        gltfExporter.Embedded = false;

        var gltfModelRoot = gltfExporter.CreateModelRoot(model);

        var gltfWriteSettings = new WriteSettings {
            ImageWriting = gltfExporter.Embedded
                               ? ResourceWriteMode.Embedded
                               : ResourceWriteMode.SatelliteFile,
        };

        foreach (var gltfFormat in gltfFormats) {
          var gltfOutputFile = outputFile.CloneWithExtension(gltfFormat);
          gltfModelRoot.Save(gltfOutputFile.FullName, gltfWriteSettings);
        }
      }

      if (nonGltfFormats.Length > 0) {
        gltfExporter.UvIndices = true;
        gltfExporter.Embedded = true;

        var inputFile = !this.LowLevel
                            ? outputFile.CloneWithExtension(".tmp.glb")
                            : outputFile.CloneWithExtension(".tmp.gltf");
        var inputPath = inputFile.FullName;
        gltfExporter.Export(inputFile, model);

        using var ctx = new AssimpContext();
        var supportedExportFormats = ctx.GetSupportedExportFormats();

        Scene? assScene = null;
        if (!this.LowLevel) {
          assScene = ctx.ImportFile(inputPath);
          File.Delete(inputPath);

          // Importing the pre-generated GLTF file does most of the hard work off
          // the bat: generating the mesh with properly weighted bones.

          // Bone orientation is already correct, you just need to enable
          // "Automatic Bone Orientation" if importing in Blender.

          new AssimpIndirectAnimationFixer().Fix(model, assScene);
          new AssimpIndirectUvFixer().Fix(model, assScene);
          new AssimpIndirectTextureFixer().Fix(model, assScene);
        }

        if (assScene != null) {
          foreach (var nonGltfFormat in nonGltfFormats) {
            var nonGltfOutputFile =
                outputFile.CloneWithExtension(nonGltfFormat);

            var outputPath = nonGltfOutputFile.FullName;
            var outputExtension = nonGltfOutputFile.Extension;

            string exportFormatId;
            {
              var exportFormatIds =
                  supportedExportFormats
                      .Where(exportFormat
                                 => outputExtension ==
                                    $".{exportFormat.FileExtension}")
                      .Select(exportFormat => exportFormat.FormatId)
                      .ToArray();
              Asserts.True(exportFormatIds.Any(),
                           $"'{outputExtension}' is not a supported export format!");

              exportFormatId = exportFormatIds.First();
            }

            // TODO: Are these all safe to include?
            var preProcessing =
                PostProcessSteps.FindInvalidData |
                PostProcessSteps.JoinIdenticalVertices;

            var success =
                ctx.ExportFile(assScene, outputPath, exportFormatId,
                               preProcessing);
            Asserts.True(success, "Failed to export model.");
          }
        }
      }
    }
  }
}