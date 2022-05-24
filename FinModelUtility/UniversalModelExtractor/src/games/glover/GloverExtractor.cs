using glo.api;


namespace uni.games.glover {
  internal class GloverExtractor {
    public void ExtractAll() {
      var rootModelDirectory =
          new GloverModelFileGatherer().GatherModelFileBundles(true)!;

      rootModelDirectory.ForEachTyped(fileBundle => {
        var gloFile = fileBundle.GloFile;

        var parentOutputDirectory =
            GameFileHierarchyUtil.GetOutputDirectoryForDirectory(
                gloFile.Parent!);
        var outputDirectory =
            parentOutputDirectory.GetSubdir(gloFile.NameWithoutExtension,
                                            true);

        new ManualGloApi().Run(outputDirectory, fileBundle);
      });
    }
  }
}