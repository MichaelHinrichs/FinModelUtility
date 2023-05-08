using System.Collections.Generic;
using System.IO;
using System.Linq;

using Assimp;

using fin.exporter.gltf;
using fin.exporter.gltf.lowlevel;
using fin.gl.material;
using fin.io;
using fin.util.asserts;
using fin.util.gc;
using fin.util.linq;

using SharpGLTF.Schema2;
using SharpGLTF.Validation;


namespace fin.exporter.assimp.indirect {
  public class AssimpIndirectExporter : IExporter {
    // You can bet your ass I'm gonna prefix everything with ass.

    public bool LowLevel { get; set; }
    public bool ForceGarbageCollection { get; set; }

    public void Export(IExporterParams exporterParams)
      => ExportExtensions(exporterParams,
                          !LowLevel
                              ? new[] { ".fbx", ".glb" }
                              : new[] { ".gltf" },
                          false);

    public void ExportExtensions(IExporterParams exporterParams,
                                 IReadOnlyList<string> exportedExtensions,
                                 bool exportAllTextures) {
      var supportedExportFormats = AssimpUtil.SupportedExportFormats;
      var exportedFormats =
          exportedExtensions
              .Select(exportedExtension => exportedExtension.ToLower())
              .Select(exportedExtension =>
                          supportedExportFormats
                              .Where(exportFormat
                                         => exportedExtension ==
                                            $".{exportFormat.FileExtension}")
                              .First(
                                  $"'{exportedExtension}' is not a supported export format!"))
              .ToArray();
      this.ExportFormats(exporterParams, exportedFormats, exportAllTextures);
    }

    public void ExportFormats(IExporterParams exporterParams,
                              IReadOnlyList<ExportFormatDescription>
                                  exportedFormats,
                              bool exportAllTextures) {
      var outputFile = exporterParams.OutputFile;
      var outputDirectory = outputFile.GetParent();
      var model = exporterParams.Model;
      var scale = exporterParams.Scale;

      if (exportedFormats.Count == 0) {
        return;
      }

      IGltfExporter gltfExporter = !this.LowLevel
          ? new GltfExporter()
          : new LowLevelGltfExporter();

      var isGltfFormat = (ExportFormatDescription format)
          => format.FileExtension is "gltf" or "glb";
      var gltfFormats = exportedFormats
                        .Where(isGltfFormat)
                        .ToArray();
      var nonGltfFormats = exportedFormats
                           .Where(exportedFormat =>
                                      !isGltfFormat(exportedFormat))
                           .ToArray();

      if (exportAllTextures) {
        foreach (var texture in model.MaterialManager.Textures) {
          texture.SaveInDirectory(outputDirectory);
        }
      }

      var finMaterials = model.MaterialManager.All;
      for (var i = 0; i < finMaterials.Count; ++i) {
        var finMaterial = finMaterials[i];
        var materialName = finMaterial.Name ?? $"material{i}";
        
        var shaderSource = finMaterial.ToShaderSource(model);
        var vertexShaderFile = (ISystemFile) new FinFile(
            Path.Combine(outputDirectory.FullName,
                         $"{materialName}_vertex_shader.glsl"));
        var fragmentShaderFile = (ISystemFile) new FinFile(
            Path.Combine(outputDirectory.FullName,
                         $"{materialName}_fragment_shader.glsl"));
        vertexShaderFile.WriteAllText(shaderSource.VertexShaderSource);
        fragmentShaderFile.WriteAllText(shaderSource.FragmentShaderSource);
      }

      if (gltfFormats.Length > 0) {
        gltfExporter.UvIndices = false;
        gltfExporter.Embedded = false;

        var gltfModelRoot = gltfExporter.CreateModelRoot(model, scale);
        if (ForceGarbageCollection) {
          GcUtil.ForceCollectEverything();
        }

        foreach (var gltfFormat in gltfFormats) {
          var gltfOutputFile =
              outputFile.CloneWithExtension($".{gltfFormat.FileExtension}");

          var gltfWriteSettings =
              WriteContext.CreateFromFile(gltfOutputFile.FullName);
          gltfWriteSettings.ImageWriting = gltfExporter.Embedded
              ? ResourceWriteMode.EmbeddedAsBase64
              : ResourceWriteMode.SatelliteFile;

          if (LowLevel) {
            gltfWriteSettings.MergeBuffers = false;
            gltfWriteSettings.Validation = ValidationMode.Skip;
          }

          var name =
              Path.GetFileNameWithoutExtension(gltfOutputFile
                                                   .FullNameWithoutExtension);
          if (gltfFormat.FileExtension == "glb") {
            gltfWriteSettings.WriteBinarySchema2(name, gltfModelRoot);
          } else {
            gltfWriteSettings.WriteTextSchema2(name, gltfModelRoot);
          }

          //gltfModelRoot.Save(gltfOutputFile.FullName, gltfWriteSettings);
          if (ForceGarbageCollection) {
            GcUtil.ForceCollectEverything();
          }
        }
      }

      if (!LowLevel && nonGltfFormats.Length > 0) {
        gltfExporter.UvIndices = true;
        gltfExporter.Embedded = true;

        var inputFile = outputFile.CloneWithExtension(".tmp.glb");
        var inputPath = inputFile.FullName;
        gltfExporter.Export(new ExporterParams {
            OutputFile = inputFile, Model = model, Scale = scale * 100,
        });
        if (ForceGarbageCollection) {
          GcUtil.ForceCollectEverything();
        }

        using var ctx = new AssimpContext();
        var assScene = ctx.ImportFile(inputPath);
        File.Delete(inputPath);

        // Importing the pre-generated GLTF file does most of the hard work off
        // the bat: generating the mesh with properly weighted bones.

        // Bone orientation is already correct, you just need to enable
        // "Automatic Bone Orientation" if importing in Blender.

        new AssimpIndirectAnimationFixer().Fix(model, assScene);
        new AssimpIndirectUvFixer().Fix(model, assScene);
        new AssimpIndirectTextureFixer().Fix(model, assScene);

        foreach (var nonGltfFormat in nonGltfFormats) {
          var nonGltfOutputFile =
              outputFile.CloneWithExtension($".{nonGltfFormat.FileExtension}");

          var outputPath = nonGltfOutputFile.FullName;
          var outputExtension = nonGltfOutputFile.Extension;

          var supportedExportFormats = ctx.GetSupportedExportFormats();

          // TODO: Are these all safe to include?
          var preProcessing =
              PostProcessSteps.FindInvalidData |
              PostProcessSteps.JoinIdenticalVertices;

          var success =
              ctx.ExportFile(assScene,
                             outputPath,
                             nonGltfFormat.FormatId,
                             preProcessing);
          Asserts.True(success, "Failed to export model.");

          if (ForceGarbageCollection) {
            GcUtil.ForceCollectEverything();
          }
        }
      }
    }
  }
}