using fin.io;
using fin.model;
using level5.api;
using level5.schema;
using uni.platforms;
using uni.platforms.threeDs;
using uni.platforms.threeDs.tools;
using uni.util.io;


namespace uni.games.professor_layton_vs_phoenix_wright {
  public class
      ProfessorLaytonVsPhoenixWrightModelFileGatherer : IModelFileGatherer<
          XcModelFileBundle> {
    public IModelDirectory<XcModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var professorLaytonVsPhoenixWrightRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "professor_layton_vs_phoenix_wright.cia");
      if (professorLaytonVsPhoenixWrightRom == null) {
        return null;
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor().ExtractFromRom(
              professorLaytonVsPhoenixWrightRom);

      if (new ThreeDsXfsaTool().Extract(
              fileHierarchy.Root.Files.Single(file => file.Name == "vs1.fa"))) {
        fileHierarchy.Root.Refresh(true);
      }

      var root = new FileHierarchyBundler<XcModelFileBundle>(
          directory => {
            /*var xcFiles = directory.FilesWithExtension(".xc")
                                   .ToDictionary(
                                       xcFile => xcFile.NameWithoutExtension);

            var animationXcFiles =
                xcFiles
                    .Where(xcFile => xcFile.Key.EndsWith("_mn"))
                    .ToArray();

            var animationAndModelXcFiles =
                animationXcFiles
                    .Select<KeyValuePair<string, IFileHierarchyFile>, (
                        IFileHierarchyFile,
                        IFileHierarchyFile)?>(animationXcFile => {
                      if (xcFiles.TryGetValue(animationXcFile.Key[..^3],
                                              out var modelXcFile)) {
                        return (modelXcFile, animationXcFile.Value);
                      }
                      return null;
                    })
                    .Where(value => value != null)
                    .ToArray();

            var bundles =
                animationAndModelXcFiles
                    .Select(
                        animationAndModelXcFile => {
                          var (modelXcFile, animationXcFile) = animationAndModelXcFile.Value;
                          return new XcModelFileBundle {
                              ModelXcFile = modelXcFile,
                              AnimationXcFile = animationXcFile,
                          };
                        })
                    .ToList();*/

            HashSet<IFileHierarchyFile> filesWithModels = new();
            HashSet<IFileHierarchyFile> filesWithAnimations = new();
            var xcFiles = directory.FilesWithExtension(".xc");

            foreach (var xcFile in xcFiles) {
              try {
                var xc = xcFile.Impl.ReadNew<Xc>(Endianness.LittleEndian);

                if (xc.FilesByExtension.TryGetList(".prm", out _)) {
                  filesWithModels.Add(xcFile);
                }

                if (xc.FilesByExtension.TryGetList(".mtn2", out _)) {
                  filesWithAnimations.Add(xcFile);
                }
              } catch { }
            }

            var filesWithModelsAndAnimations =
                filesWithModels.Intersect(filesWithAnimations).ToHashSet();

            var xcBundles = new IXcFiles[0];
            if (directory.LocalPath == "\\vs1\\chr") {
              xcBundles = new[] {
                  GetSameFile("Emeer Punchenbaug", directory, "c206.xc"),
                  GetSameFile("Flynch", directory, "c203.xc"),
                  GetSameFile("Kira", directory, "c215.xc"),
                  GetSameFile("Kira (with flower petals)", directory, "c216.xc"),
                  GetSameFile("Knightle", directory, "c213.xc"),
                  GetSameFile("Miles Edgeworth", directory, "c401.xc"),
                  GetSameFile("Professor Layton (Gold)", directory, "c301.xc"),
                  GetSameFile("Wordsmith", directory, "c211.xc"),
              };
            }

            var bundles = new List<XcModelFileBundle>();
            foreach (var xcBundle in xcBundles) {
              bundles.Add(new XcModelFileBundle {
                  BetterFileName = xcBundle.Name,
                  ModelXcFile = xcBundle.ModelFile,
                  AnimationXcFile = xcBundle.AnimationFile,
              });
            }
            foreach (var fileWithModelsAndAnimations in
                     filesWithModelsAndAnimations) {
              if (xcBundles.Any(xcBundle =>
                                    xcBundle.ModelFile ==
                                    fileWithModelsAndAnimations)) {
                continue;
              }

              bundles.Add(new XcModelFileBundle {
                  ModelXcFile = fileWithModelsAndAnimations,
                  AnimationXcFile = fileWithModelsAndAnimations,
              });
            }

            return bundles;
          }
      ).GatherBundles(fileHierarchy);

      return root;
    }

    internal IXcFiles GetModelOnly(string name,
                                   IFileHierarchyDirectory directory,
                                   string modelFileName)
      => new ModelOnly(name,
                       directory.Files.Single(
                           file => file.Name == modelFileName));

    internal IXcFiles GetSameFile(string name,
                                  IFileHierarchyDirectory directory,
                                  string modelFileName)
      => new SameFile(name,
                      directory.Files.Single(
                          file => file.Name == modelFileName));

    internal IXcFiles GetModelAndAnimation(string name,
                                           IFileHierarchyDirectory directory,
                                           string modelFileName,
                                           string animationFileName)
      => new ModelAndAnimation(
          name,
          directory.Files.Single(file => file.Name == modelFileName),
          directory.Files.Single(file => file.Name == animationFileName));

    internal interface IXcFiles {
      string Name { get; }
      IFileHierarchyFile ModelFile { get; }
      IFileHierarchyFile? AnimationFile { get; }
    }


    internal record ModelOnly(
        string Name,
        IFileHierarchyFile ModelFile) : IXcFiles {
      public IFileHierarchyFile? AnimationFile => null;
    }

    internal record SameFile(
        string Name,
        IFileHierarchyFile ModelFile) : IXcFiles {
      public IFileHierarchyFile? AnimationFile => ModelFile;
    }

    internal record ModelAndAnimation(
        string Name,
        IFileHierarchyFile ModelFile,
        IFileHierarchyFile AnimationFile) : IXcFiles;
  }
}