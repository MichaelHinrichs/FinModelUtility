using fin.exporter.assimp.indirect;
using fin.exporter.gltf;
using fin.io;
using fin.io.bundles;
using fin.log;
using fin.model;
using fin.util.asserts;
using uni.config;
using uni.msg;


namespace uni.games {
  public static class ExtractorUtil {
    static ExtractorUtil() {
      ExtractorUtil.logger_ = Logging.Create("extractor");
    }

    private static readonly ILogger logger_;

    public static void ExtractAll<T>(
        IFileBundleGatherer<T> gatherer,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      var fileBundles = new List<T>();

      var root = gatherer.GatherFileBundles(true);
      root.ForEachTyped(fileBundles.Add);

      ExtractorUtil.ExtractAll(fileBundles, loader);
    }

    public static void ExtractAll<T>(
        IEnumerable<T> modelFileBundles,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      foreach (var modelFileBundle in modelFileBundles) {
        ExtractorUtil.Extract(modelFileBundle, loader);
      }
    }

    public static void ExtractAll<T>(
        IFileBundleGatherer<IFileBundle> gatherer,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      var fileBundles = new List<IFileBundle>();

      var root = gatherer.GatherFileBundles(true);
      root.ForEachTyped(fileBundles.Add);

      ExtractorUtil.ExtractAll(fileBundles, loader);
    }

    public static void ExtractAll<T>(
        IEnumerable<IFileBundle> fileBundles,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      foreach (var fileBundle in fileBundles) {
        if (fileBundle is T modelFileBundle) {
          ExtractorUtil.Extract(modelFileBundle, loader);
        }
      }
    }

    public static void Extract<T>(T modelFileBundle, IModelLoader<T> loader)
        where T : IModelFileBundle {
      ExtractorUtil.Extract(modelFileBundle,
                            () => loader.LoadModel(modelFileBundle));
    }

    public static void Extract<T>(T modelFileBundle, Func<IModel> loaderHandler)
        where T : IModelFileBundle {
      var mainFile = Asserts.CastNonnull(modelFileBundle.MainFile);

      var parentOutputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForFile(mainFile);
      var outputDirectory =
          parentOutputDirectory.GetSubdir(mainFile.NameWithoutExtension,
                                          true);

      var existingOutputFile =
          outputDirectory.GetExistingFiles()
                         .Where(file => file.Extension is ".fbx" or ".glb")
                         .Any(file => file.NameWithoutExtension ==
                                     mainFile.NameWithoutExtension);

      if (existingOutputFile) {
        MessageUtil.LogAlreadyProcessed(ExtractorUtil.logger_, mainFile);
        return;
      }

      MessageUtil.LogExtracting(ExtractorUtil.logger_, mainFile);

      try {
        var model = loaderHandler();
        new AssimpIndirectExporter().Export(
            new FinFile(Path.Join(outputDirectory.FullName,
                                  mainFile.NameWithoutExtension + ".foo")),
            Config.Instance.ExportedFormats,
            model);
      } catch (Exception e) {
        ExtractorUtil.logger_.LogError(e.ToString());
      }
      ExtractorUtil.logger_.LogInformation(" ");
    }
  }
}