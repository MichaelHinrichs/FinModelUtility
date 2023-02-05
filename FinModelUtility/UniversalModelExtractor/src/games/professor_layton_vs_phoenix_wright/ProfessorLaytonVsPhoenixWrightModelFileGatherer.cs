using fin.io;
using fin.io.bundles;
using level5.api;
using level5.schema;
using uni.platforms;
using uni.platforms.threeDs;
using uni.platforms.threeDs.tools;
using uni.util.io;


namespace uni.games.professor_layton_vs_phoenix_wright {
  public class ProfessorLaytonVsPhoenixWrightModelFileGatherer 
      : IFileBundleGatherer<XcModelFileBundle> {
    public IFileBundleDirectory<XcModelFileBundle>? GatherFileBundles(
        bool assert) {
      var professorLaytonVsPhoenixWrightRom =
          DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "professor_layton_vs_phoenix_wright.cia", assert);
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
              using var er =
                  new EndianBinaryReader(xcFile.Impl.OpenRead(),
                                         Endianness.LittleEndian);

              if (er.ReadString(4) != "XPCK") {
                continue;
              }

              try {
                er.Position = 0;
                var xc = er.ReadNew<Xc>();

                if (xc.FilesByExtension.TryGetList(".prm", out _)) {
                  filesWithModels.Add(xcFile);
                }

                if (xc.FilesByExtension.TryGetList(".mtn2", out _)) {
                  filesWithAnimations.Add(xcFile);
                }
              } catch {
                ;
              }
            }

            var filesWithModelsAndAnimations =
                filesWithModels.Intersect(filesWithAnimations).ToHashSet();

            var xcBundles = Array.Empty<IXcFiles>();
            if (directory.LocalPath == "\\vs1\\chr") {
              xcBundles = new[] {
                  GetSameFile("Emeer Punchenbaug", directory, "c206.xc"),
                  GetSameFile("Espella Cantabella", directory, "c105.xc"),
                  GetSameFile("Flynch", directory, "c203.xc"),
                  GetSameFile("Johnny Smiles", directory, "c201.xc"),
                  GetSameFile("Judge", directory, "c107.xc"),
                  GetSameFile("Kira", directory, "c215.xc"),
                  GetSameFile("Kira (with flower petals)", directory, "c216.xc"),
                  GetSameFile("Knightle", directory, "c213.xc"),
                  GetSameFile("Miles Edgeworth", directory, "c401.xc"),
                  GetSameFile("Olivia Aldente", directory, "c202.xc"),
                  GetSameFile("Phoenix Wright", directory, "c102.xc"),
                  GetSameFile("Phoenix Wright (Baker)", directory, "c113.xc"),
                  GetSameFile("Professor Layton", directory, "c101.xc"),
                  GetSameFile("Professor Layton (Gold)", directory, "c301.xc"),
                  GetSameFile("Storyteller", directory, "c134.xc"),
                  GetSameFile("Wordsmith", directory, "c211.xc"),
                  GetSameFile("Zacharias Barnham", directory, "c106_a.xc"),
              };
            }

            var bundles = new List<XcModelFileBundle>();
            foreach (var xcBundle in xcBundles) {
              bundles.Add(new XcModelFileBundle {
                BetterFileName = xcBundle.Name,
                ModelXcFile = xcBundle.ModelFile,
                AnimationXcFiles = xcBundle.AnimationFiles,
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
                AnimationXcFiles = new[] { fileWithModelsAndAnimations },
              });
            }
            foreach (var fileWithModel in filesWithModels) {
              if (xcBundles.Any(xcBundle =>
                    xcBundle.ModelFile == fileWithModel)) {
                continue;
              }

              var name = fileWithModel.NameWithoutExtension;
              var underscoreIndex = name.IndexOf('_');
              var nameUpToUnderscore = underscoreIndex == -1 ? name : name.Substring(0, underscoreIndex);

              var animations = filesWithAnimations.Where(fileWithAnimations => fileWithAnimations.Name.StartsWith(nameUpToUnderscore)).ToArray();

              bundles.Add(new XcModelFileBundle {
                ModelXcFile = fileWithModel,
                AnimationXcFiles = animations,
              });
            }

            return bundles.OrderBy(bundle => bundle.BetterFileName ?? bundle.MainFile.Name).ToArray();
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

    internal IXcFiles GetModelAndAnimations(string name,
                                           IFileHierarchyDirectory directory,
                                           string modelFileName,
                                           params string[] animationFileNames)
      => new ModelAndAnimations(
          name,
          directory.Files.Single(file => file.Name == modelFileName),
          animationFileNames.Select(animationFileName => directory.Files.Single(file => file.Name == animationFileName)).ToArray());

    internal interface IXcFiles {
      string Name { get; }
      IFileHierarchyFile ModelFile { get; }
      IFileHierarchyFile[]? AnimationFiles { get; }
    }


    internal record ModelOnly(
        string Name,
        IFileHierarchyFile ModelFile) : IXcFiles {
      public IFileHierarchyFile[]? AnimationFiles => null;
    }

    internal record SameFile(
        string Name,
        IFileHierarchyFile ModelFile) : IXcFiles {
      public IFileHierarchyFile[] AnimationFiles { get; } = { ModelFile };
    }

    internal record ModelAndAnimations(
        string Name,
        IFileHierarchyFile ModelFile,
        params IFileHierarchyFile[] AnimationFiles) : IXcFiles;
  }
}