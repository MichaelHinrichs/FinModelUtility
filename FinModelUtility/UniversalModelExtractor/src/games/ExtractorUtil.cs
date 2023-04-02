using System.Linq;

using fin.exporter;
using fin.exporter.assimp.indirect;
using fin.io;
using fin.io.bundles;
using fin.log;
using fin.model;
using fin.util.asserts;

using level5.api;

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
        IEnumerable<IFile> outputFiles,
        out IReadOnlyList<IFile> existingOutputFiles) {
      existingOutputFiles =
          outputFiles.Where(file => file.Exists).ToArray();
      return existingOutputFiles.Count > 0;
    }

    public static bool CheckIfModelFileBundlesAlreadyExtracted(
        IEnumerable<IModelFileBundle> modelFileBundles,
        out IReadOnlyList<IModelFileBundle> existingModelFileBundles) {
      existingModelFileBundles =
          modelFileBundles.Where(CheckIfModelFileBundleAlreadyExtracted)
                          .ToArray();
      return existingModelFileBundles.Count > 0;
    }

    public static bool CheckIfModelFileBundleAlreadyExtracted(
        IModelFileBundle modelFileBundle) {
      var mainFile = Asserts.CastNonnull(modelFileBundle.MainFile);

      var parentOutputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForFile(mainFile);
      var outputDirectory = new FinDirectory(
          Path.Join(parentOutputDirectory.FullName,
                    mainFile.NameWithoutExtension));

      // TODO: Pass in extensions
      if (outputDirectory.Exists) {
        var existingOutputFile =
            outputDirectory.GetExistingFiles()
                           .Where(file => file.Extension is ".fbx" or ".glb"
                                      or ".gltf")
                           .Any(file => file.NameWithoutExtension ==
                                        mainFile.NameWithoutExtension);
        return true;
      }

      return false;
    }

    public enum ExtractorPromptChoice {
      CANCEL,
      SKIP_EXISTING,
      OVERWRITE_EXISTING,
    }

    public static ExtractorPromptChoice PromptIfFilesAlreadyExist(
        IReadOnlyList<IFile> outputFiles) {
      if (CheckIfFilesAlreadyExist(outputFiles, out var existingOutputFiles)) {
        if (outputFiles.Count == 1) {
          var result =
              MessageBox.Show($"Output file \"{existingOutputFiles.First()}\" already exists. Would you like to overwrite it?",
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
              MessageBox.Show($"\"{existingOutputFiles.Count()}\" output files already exist. Would you like to continue extracting and overwrite them?",
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

    public static ExtractorPromptChoice PromptIfModelFileBundlesAlreadyExtracted(
        IReadOnlyList<IModelFileBundle> modelFileBundles) {
      if (ExtractorUtil.CheckIfModelFileBundlesAlreadyExtracted(
              modelFileBundles,
              out var existingOutputFiles)) {
        var totalCount = modelFileBundles.Count;
        if (totalCount == 1) {
          var result =
              MessageBox.Show($"Model defined in \"{existingOutputFiles.First().DisplayFullName}\" has already been extracted. Would you like to overwrite it?", 
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
              MessageBox.Show($"{existingCount} model{(existingCount != 1 ? "s have" : " has")} already been extracted. Select 'Yes' to overwrite them, 'No' to skip them, or 'Cancel' to abort this operation.",
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
        IModelLoader<T> loader,
        bool overwriteExistingFiles)
        where T : IModelFileBundle {
      ExtractorUtil.ExtractAll(gatherer.GatherFileBundles(true),
                               loader,
                               overwriteExistingFiles);
    }

    public static void ExtractAll<T>(
        IEnumerable<T> modelFileBundles,
        IModelLoader<T> loader,
        bool overwriteExistingFiles)
        where T : IModelFileBundle {
      foreach (var modelFileBundle in modelFileBundles) {
        ExtractorUtil.Extract(modelFileBundle, loader, overwriteExistingFiles);
      }
    }

    public static void ExtractAll<T>(
        IFileBundleGatherer<IFileBundle> gatherer,
        IModelLoader<T> loader,
        bool overwriteExistingFiles)
        where T : IModelFileBundle {
      ExtractorUtil.ExtractAll(gatherer.GatherFileBundles(true),
                               loader,
                               overwriteExistingFiles);
    }

    public static void ExtractAll<T>(
        IEnumerable<IFileBundle> fileBundles,
        IModelLoader<T> loader,
        bool overwriteExistingFiles)
        where T : IModelFileBundle {
      foreach (var fileBundle in fileBundles) {
        if (fileBundle is T modelFileBundle) {
          ExtractorUtil.Extract(modelFileBundle,
                                loader,
                                overwriteExistingFiles);
        }
      }
    }

    public static void Extract<T>(T modelFileBundle,
                                  IModelLoader<T> loader,
                                  bool overwriteExistingFile)
        where T : IModelFileBundle {
      ExtractorUtil.Extract(modelFileBundle,
                            () => loader.LoadModel(modelFileBundle),
                            overwriteExistingFile);
    }

    public static void Extract<T>(T modelFileBundle,
                                  Func<IModel> loaderHandler,
                                  bool overwriteExistingFile)
        where T : IModelFileBundle {
      var mainFile = Asserts.CastNonnull(modelFileBundle.MainFile);

      var parentOutputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForFile(mainFile);
      var outputDirectory = new FinDirectory(
          Path.Join(parentOutputDirectory.FullName,
                    mainFile.NameWithoutExtension));

      if (!overwriteExistingFile && outputDirectory.Exists) {
        var existingOutputFile =
            outputDirectory.GetExistingFiles()
                           .Where(file => file.Extension is ".fbx" or ".glb"
                                      or ".gltf")
                           .Any(file => file.NameWithoutExtension ==
                                        mainFile.NameWithoutExtension);

        if (existingOutputFile) {
          MessageUtil.LogAlreadyProcessed(ExtractorUtil.logger_, mainFile);
          return;
        }
      }

      MessageUtil.LogExtracting(ExtractorUtil.logger_, mainFile);

      try {
        var model = loaderHandler();

        outputDirectory.Create();

        if (Config.Instance.ExportAllTextures) {
          foreach (var texture in model.MaterialManager.Textures) {
            texture.SaveInDirectory(outputDirectory);
          }
        }

        new AssimpIndirectExporter {
            LowLevel = modelFileBundle.UseLowLevelExporter,
            ForceGarbageCollection = modelFileBundle.ForceGarbageCollection,
        }.Export(new ExporterParams {
                     OutputFile = new FinFile(
                         Path.Join(outputDirectory.FullName,
                                   mainFile.NameWithoutExtension + ".foo")),
                     Model = model,
                     Scale = new ScaleSource(
                         Config.Instance.ExportedModelScaleSource).GetScale(
                         model,
                         modelFileBundle)
                 },
                 !modelFileBundle.UseLowLevelExporter
                     ? Config.Instance.ExportedFormats
                     : new[] { ".gltf" });

        if (Config.Instance.ThirdParty.ExportBoneScaleAnimationsSeparately) {
          new BoneScaleAnimationExporter().Export(
              new FinFile(Path.Join(outputDirectory.FullName,
                                    mainFile.NameWithoutExtension +
                                    "_bone_scale_animations.lua")),
              model);
        }
      } catch (Exception e) {
        ExtractorUtil.logger_.LogError(e.ToString());
      }

      ExtractorUtil.logger_.LogInformation(" ");
    }
  }
}