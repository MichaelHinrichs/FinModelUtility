using fin.exporter.assimp.indirect;
using fin.exporter.gltf;
using fin.io;
using fin.io.bundles;
using fin.log;
using fin.model;

using uni.msg;
using uni.platforms;


namespace uni.games {
  public class Config {
    public bool IncludeFbx { get; set; }
  }

  public static class ExtractorUtil {
    static ExtractorUtil() {
      ExtractorUtil.logger_ = Logging.Create("extractor");
      ExtractorUtil.config_ =
          DirectoryConstants.CONFIG_FILE.Deserialize<Config>();
    }

    private static readonly ILogger logger_;
    private static readonly Config config_;

    public static void ExtractAll<T>(
        IFileBundleGatherer<T> gatherer,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      var modelFileBundles = new List<T>();

      var root = gatherer.GatherFileBundles(true);
      root.ForEachTyped(modelFileBundles.Add);

      ExtractorUtil.ExtractAll(modelFileBundles, loader);
    }

    public static void ExtractAll<T>(
        IEnumerable<T> modelFileBundles,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      foreach (var modelFileBundle in modelFileBundles) {
        ExtractorUtil.Extract(modelFileBundle, loader);
      }
    }

    public static void Extract<T>(T modelFileBundle, IModelLoader<T> loader)
        where T : IModelFileBundle {
      ExtractorUtil.Extract(modelFileBundle,
                            () => loader.LoadModel(modelFileBundle));
    }

    public static void Extract<T>(T modelFileBundle, Func<IModel> loaderHandler)
        where T : IModelFileBundle {
      var mainFile = modelFileBundle.MainFile;

      var parentOutputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForFile(mainFile);
      var outputDirectory =
          parentOutputDirectory.GetSubdir(mainFile.NameWithoutExtension,
                                          true);

      var existingOutputFile =
          outputDirectory.GetExistingFiles()
                         .Where(file => file.Extension is ".fbx" or ".glb")
                         .SingleOrDefault(
                             file => file.NameWithoutExtension ==
                                     mainFile.NameWithoutExtension);

      if (existingOutputFile != null) {
        MessageUtil.LogAlreadyProcessed(ExtractorUtil.logger_, mainFile);
        return;
      }

      MessageUtil.LogExtracting(ExtractorUtil.logger_, mainFile);

      try {
        var model = loaderHandler();

        var includeFbx = ExtractorUtil.config_.IncludeFbx;
        if (includeFbx) {
          new AssimpIndirectExporter().Export(
              new FinFile(Path.Join(outputDirectory.FullName,
                                    mainFile.NameWithoutExtension +
                                    ".fbx")),
              model);
        } else {
          new GltfExporter().Export(
              new FinFile(Path.Join(outputDirectory.FullName,
                                    mainFile.NameWithoutExtension +
                                    ".glb")),
              model);
        }
      } catch (Exception e) {
        ExtractorUtil.logger_.LogError(e.ToString());
      }
      ExtractorUtil.logger_.LogInformation(" ");
    }
  }
}