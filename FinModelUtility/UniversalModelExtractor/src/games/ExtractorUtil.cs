using Assimp;

using fin.exporter;
using fin.exporter.assimp;
using fin.exporter.assimp.indirect;
using fin.io;
using fin.io.bundles;
using fin.log;
using fin.model;
using fin.util.asserts;
using fin.util.linq;

using uni.config;
using uni.model;
using uni.msg;
using uni.thirdparty;

namespace uni.games {
  public static class ExtractorUtil {
    static ExtractorUtil() {
      ExtractorUtil.logger_ = Logging.Create("extractor");
    }

    private static readonly ILogger logger_;

    public static bool CheckIfFilesAlreadyExist(
        IEnumerable<ISystemFile> outputFiles,
        out IReadOnlyList<ISystemFile> existingOutputFiles) {
      existingOutputFiles =
          outputFiles.Where(file => file.Exists).ToArray();
      return existingOutputFiles.Count > 0;
    }

    public static bool CheckIfModelFileBundlesAlreadyExtracted(
        IEnumerable<IModelFileBundle> modelFileBundles,
        IReadOnlyList<string> extensions,
        out IReadOnlyList<IModelFileBundle> existingModelFileBundles) {
      existingModelFileBundles =
          modelFileBundles
              .Where(mfb => CheckIfModelFileBundleAlreadyExtracted(
                         mfb,
                         extensions))
              .ToArray();
      return existingModelFileBundles.Count > 0;
    }

    public static bool CheckIfModelFileBundleAlreadyExtracted(
        IModelFileBundle modelFileBundle,
        IEnumerable<string> extensions) {
      var mainFile = Asserts.CastNonnull(modelFileBundle.MainFile);

      var parentOutputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForFile(mainFile);
      var outputDirectory = new FinDirectory(
          Path.Join(parentOutputDirectory.FullName,
                    mainFile.NameWithoutExtension));

      if (outputDirectory.Exists) {
        return extensions.All(
            extension => outputDirectory
                         .GetExistingFiles()
                         .Where(file => extensions.Contains(file.Extension))
                         .Any(file => file.NameWithoutExtension ==
                                      mainFile.NameWithoutExtension));
      }

      return false;
    }

    public enum ExtractorPromptChoice {
      CANCEL,
      SKIP_EXISTING,
      OVERWRITE_EXISTING,
    }

    public static ExtractorPromptChoice PromptIfFilesAlreadyExist(
        IReadOnlyList<ISystemFile> outputFiles) {
      if (CheckIfFilesAlreadyExist(outputFiles, out var existingOutputFiles)) {
        if (outputFiles.Count == 1) {
          var result =
              MessageBox.Show(
                  $"Output file \"{existingOutputFiles.First()}\" already exists. Would you like to overwrite it?",
                  "Output file already exists!",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Warning,
                  MessageBoxDefaultButton.Button1);
          return result switch {
              DialogResult.Yes => ExtractorPromptChoice.OVERWRITE_EXISTING,
              DialogResult.No  => ExtractorPromptChoice.CANCEL,
          };
        } else {
          var result =
              MessageBox.Show(
                  $"\"{existingOutputFiles.Count()}\" output files already exist. Would you like to continue extracting and overwrite them?",
                  "Some output files already exist!",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Warning,
                  MessageBoxDefaultButton.Button1);
          return result switch {
              DialogResult.Yes    => ExtractorPromptChoice.OVERWRITE_EXISTING,
              DialogResult.No     => ExtractorPromptChoice.SKIP_EXISTING,
              DialogResult.Cancel => ExtractorPromptChoice.CANCEL,
          };
        }
      }

      return ExtractorPromptChoice.SKIP_EXISTING;
    }

    public static ExtractorPromptChoice
        PromptIfModelFileBundlesAlreadyExtracted(
            IReadOnlyList<IModelFileBundle> modelFileBundles,
            IReadOnlyList<string> extensions) {
      if (ExtractorUtil.CheckIfModelFileBundlesAlreadyExtracted(
              modelFileBundles,
              extensions,
              out var existingOutputFiles)) {
        var totalCount = modelFileBundles.Count;
        if (totalCount == 1) {
          var result =
              MessageBox.Show(
                  $"Model defined in \"{existingOutputFiles.First().DisplayFullName}\" has already been extracted. Would you like to overwrite it?",
                  "Model has already been extracted!",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Warning,
                  MessageBoxDefaultButton.Button1);
          return result switch {
              DialogResult.Yes => ExtractorPromptChoice.OVERWRITE_EXISTING,
              DialogResult.No  => ExtractorPromptChoice.CANCEL,
          };
        } else {
          var existingCount = existingOutputFiles.Count();
          var result =
              MessageBox.Show(
                  $"{existingCount} model{(existingCount != 1 ? "s have" : " has")} already been extracted. Select 'Yes' to overwrite them, 'No' to skip them, or 'Cancel' to abort this operation.",
                  $"{existingCount}/{totalCount} models have already been extracted!",
                  MessageBoxButtons.YesNoCancel,
                  MessageBoxIcon.Warning,
                  MessageBoxDefaultButton.Button1);
          return result switch {
              DialogResult.Yes    => ExtractorPromptChoice.OVERWRITE_EXISTING,
              DialogResult.No     => ExtractorPromptChoice.SKIP_EXISTING,
              DialogResult.Cancel => ExtractorPromptChoice.CANCEL,
          };
        }
      }

      return ExtractorPromptChoice.SKIP_EXISTING;
    }

    public static void ExtractAll<T>(
        IFileBundleGatherer<T> gatherer,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      ExtractorUtil.ExtractAll(gatherer.GatherFileBundles(true),
                               loader,
                               Config.Instance.ExporterSettings.ExportedFormats,
                               false);
    }

    public static void ExtractAll<T>(
        IEnumerable<T> modelFileBundles,
        IModelLoader<T> loader,
        IReadOnlyList<string> extensions,
        bool overwriteExistingFiles)
        where T : IModelFileBundle {
      foreach (var modelFileBundle in modelFileBundles) {
        ExtractorUtil.Extract(modelFileBundle,
                              loader,
                              extensions,
                              overwriteExistingFiles);
      }
    }

    public static void ExtractAll<T>(
        IFileBundleGatherer<IFileBundle> gatherer,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      ExtractorUtil.ExtractAll(gatherer.GatherFileBundles(true),
                               loader,
                               Config.Instance.ExporterSettings.ExportedFormats,
                               false);
    }

    public static void ExtractAll<T>(
        IEnumerable<IFileBundle> fileBundles,
        IModelLoader<T> loader,
        IReadOnlyList<string> extensions,
        bool overwriteExistingFiles)
        where T : IModelFileBundle {
      var fileBundleArray = fileBundles.WhereIs<IFileBundle, T>()
                                       .ToArray();
      for (var i = 0; i < fileBundleArray.Length; ++i) {
        var modelFileBundle = fileBundleArray[i];
        ExtractorUtil.Extract(modelFileBundle,
                              loader,
                              extensions,
                              overwriteExistingFiles);
      }
    }

    public static void ExtractAll<T>(
        IEnumerable<IFileBundle> fileBundles,
        IModelLoader<T> loader,
        IProgress<(float, T?)> progress,
        CancellationTokenSource cancellationTokenSource,
        IReadOnlyList<string> extensions,
        bool overwriteExistingFiles)
        where T : IModelFileBundle {
      var fileBundleArray = fileBundles.WhereIs<IFileBundle, T>()
                                       .ToArray();
      for (var i = 0; i < fileBundleArray.Length; ++i) {
        if (cancellationTokenSource.IsCancellationRequested) {
          break;
        }

        var modelFileBundle = fileBundleArray[i];
        progress.Report((i * 1f / fileBundleArray.Length, modelFileBundle));
        ExtractorUtil.Extract(modelFileBundle,
                              loader,
                              extensions,
                              overwriteExistingFiles);
      }

      progress.Report((1, default));
    }

    public static void Extract<T>(T modelFileBundle,
                                  IModelLoader<T> loader,
                                  IReadOnlyList<string> extensions,
                                  bool overwriteExistingFile)
        where T : IModelFileBundle {
      ExtractorUtil.Extract(modelFileBundle,
                            () => loader.LoadModel(modelFileBundle),
                            extensions,
                            overwriteExistingFile);
    }

    public static void Extract<T>(T modelFileBundle,
                                  Func<IModel> loaderHandler,
                                  IReadOnlyList<string> extensions,
                                  bool overwriteExistingFile)
        where T : IModelFileBundle {
      var mainFile = Asserts.CastNonnull(modelFileBundle.MainFile);

      var parentOutputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForFile(mainFile);
      var outputDirectory = new FinDirectory(
          Path.Join(parentOutputDirectory.FullName,
                    mainFile.NameWithoutExtension));

      Extract<T>(modelFileBundle,
                 loaderHandler,
                 outputDirectory,
                 extensions,
                 overwriteExistingFile);
    }

    public static void Extract<T>(T modelFileBundle,
                                  Func<IModel> loaderHandler,
                                  ISystemDirectory outputDirectory,
                                  IReadOnlyList<string> extensions,
                                  bool overwriteExistingFile,
                                  string? overrideName = null)
        where T : IModelFileBundle
      => Extract(modelFileBundle,
                 loaderHandler,
                 outputDirectory,
                 extensions.Select(AssimpUtil.GetExportFormatFromExtension)
                           .ToArray(),
                 overwriteExistingFile,
                 overrideName);


    public static void Extract<T>(
        T modelFileBundle,
        Func<IModel> loaderHandler,
        ISystemDirectory outputDirectory,
        IReadOnlyList<ExportFormatDescription> formats,
        bool overwriteExistingFile,
        string? overrideName = null)
        where T : IModelFileBundle {
      var mainFile = Asserts.CastNonnull(modelFileBundle.MainFile);
      var name = overrideName ?? mainFile.NameWithoutExtension;

      if (modelFileBundle.UseLowLevelExporter) {
        formats = new[] { AssimpUtil.GetExportFormatFromExtension(".gltf") };
      }

      if (!overwriteExistingFile && CheckIfModelFileBundleAlreadyExtracted(
              modelFileBundle,
              formats.Select(format => $".{format.FileExtension}"))) {
        MessageUtil.LogAlreadyProcessed(ExtractorUtil.logger_, mainFile);
        return;
      }

      outputDirectory.Create();
      MessageUtil.LogExtracting(ExtractorUtil.logger_, mainFile);

      try {
        var model = loaderHandler();

        new AssimpIndirectExporter {
            LowLevel = modelFileBundle.UseLowLevelExporter,
            ForceGarbageCollection = modelFileBundle.ForceGarbageCollection,
        }.ExportFormats(new ExporterParams {
                            OutputFile = new FinFile(
                                Path.Join(outputDirectory.FullName,
                                          name + ".foo")),
                            Model = model,
                            Scale = new ScaleSource(
                                    Config.Instance.ExporterSettings.ExportedModelScaleSource)
                                .GetScale(
                                    model,
                                    modelFileBundle)
                        },
                        formats,
                        Config.Instance.ExporterSettings.ExportAllTextures);

        if (Config.Instance.ThirdPartySettings.ExportBoneScaleAnimationsSeparately) {
          new BoneScaleAnimationExporter().Export(
              new FinFile(Path.Join(outputDirectory.FullName,
                                    name + "_bone_scale_animations.lua")),
              model);
        }
      } catch (Exception e) {
        ExtractorUtil.logger_.LogError(e.ToString());
      }

      ExtractorUtil.logger_.LogInformation(" ");
    }
  }
}