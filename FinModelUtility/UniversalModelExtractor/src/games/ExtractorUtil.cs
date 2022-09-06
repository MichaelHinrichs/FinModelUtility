using fin.exporter.assimp.indirect;
using fin.exporter.gltf;
using fin.io;
using fin.log;
using fin.model;

using uni.msg;
using uni.platforms;


namespace uni.games {
  public class Config {
    public bool IncludeFbx { get; set; }
  }

  public static class ExtractorUtil {
    public static void ExtractAll<T>(
        IModelFileGatherer<T> gatherer,
        IModelLoader<T> loader)
        where T : IModelFileBundle {
      var logger = Logging.Create<T>();

      var config = DirectoryConstants.CONFIG_FILE.Deserialize<Config>();
      var includeFbx = config.IncludeFbx;

      var root = gatherer.GatherModelFileBundles(true);
      root.ForEachTyped(fileBundle => {
        var mainFile = fileBundle.MainFile;

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
          MessageUtil.LogAlreadyProcessed(logger, mainFile);
          return;
        }

        MessageUtil.LogExtracting(logger, mainFile);

        try {
          var model = loader.LoadModel(fileBundle);

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
          logger.LogError(e.ToString());
        }
        logger.LogInformation(" ");
      });
    }
  }
}