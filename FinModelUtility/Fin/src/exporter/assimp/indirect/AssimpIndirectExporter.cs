using System.IO;
using System.Linq;

using Assimp;

using fin.exporter.gltf;
using fin.exporter.gltf.lowlevel;
using fin.io;
using fin.model;
using fin.util.asserts;

using WrapMode = fin.model.WrapMode;


namespace fin.exporter.assimp.indirect {
  using FinBlendMode = fin.model.BlendMode;

  public class AssimpIndirectExporter : IExporter {
    // You can bet your ass I'm gonna prefix everything with ass.

    public bool LowLevel { get; set; }

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

      var inputFile = !this.LowLevel
                          ? outputFile.CloneWithExtension(".glb")
                          : outputFile.CloneWithExtension(".gltf");
      var inputPath = inputFile.FullName;

      IGltfExporter gltfExporter = !this.LowLevel
                                       ? new GltfExporter()
                                       : new LowLevelGltfExporter();

      Scene? assScene = null;
      if (!this.LowLevel) {
        gltfExporter.UvIndices = true;
        gltfExporter.Embedded = true;
        gltfExporter.Export(inputFile, model);

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

      // Reexports the GLTF version in case the FBX version is screwed up.
      gltfExporter.UvIndices = false;
      gltfExporter.Embedded = false;
      gltfExporter.Export(
          new FinFile(inputFile.FullName.Replace(".glb", "_gltf.glb")),
          model);

      if (assScene != null) {
        // Finally exports the fbx version.
        // TODO: Are these all safe to include?
        var preProcessing =
            PostProcessSteps.FindInvalidData |
            PostProcessSteps.JoinIdenticalVertices;

        var success =
            ctx.ExportFile(assScene, outputPath, exportFormatId, preProcessing);
        Asserts.True(success, "Failed to export model.");
      }
    }
  }
}