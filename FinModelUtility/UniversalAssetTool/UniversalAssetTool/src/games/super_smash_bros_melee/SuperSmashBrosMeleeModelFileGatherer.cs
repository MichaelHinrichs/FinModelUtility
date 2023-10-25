using dat.api;

using fin.io;
using fin.io.bundles;

using uni.platforms.gcn;

namespace uni.games.super_smash_bros_melee {
  using IAnnotatedDatBundle = IAnnotatedFileBundle<DatModelFileBundle>;

  public class SuperSmashBrosMeleeModelAnnotatedFileGatherer
      : IAnnotatedFileBundleGatherer<DatModelFileBundle> {
    public string Name => "super_smash_bros_melee";

    private const string STAGE_PREFIX = "Gr";
    private const string TROPHY_PREFIX = "Ty";

    private const string CHARACTER_PREFIX = "Pl";
    private const string ANIMATION_SUFFIX = "AJ";

    public IEnumerable<IAnnotatedDatBundle>? GatherFileBundles() {
      if (!new GcnFileHierarchyExtractor().TryToExtractFromGame(
              "super_smash_bros_melee",
              out var fileHierarchy)) {
        yield break;
      }

      var stageFiles = new LinkedList<IFileHierarchyFile>();
      var trophyFiles = new LinkedList<IFileHierarchyFile>();
      var plFilesByName = new Dictionary<string, IFileHierarchyFile>();

      foreach (var datFile in fileHierarchy.Root.FilesWithExtension(".dat")) {
        var datFileName = datFile.NameWithoutExtension;

        if (datFileName.StartsWith(STAGE_PREFIX)) {
          stageFiles.AddLast(datFile);
          continue;
        }

        if (datFileName.StartsWith(TROPHY_PREFIX)) {
          trophyFiles.AddLast(datFile);
          continue;
        }

        if (datFileName.StartsWith(CHARACTER_PREFIX)) {
          plFilesByName.Add(datFileName, datFile);
        }
      }

      foreach (var stageOrTrophyFile in stageFiles.Concat(trophyFiles)) {
        yield return new DatModelFileBundle {
            GameName = "super_smash_bros_melee",
            PrimaryDatFile = stageOrTrophyFile,
        }.Annotate(stageOrTrophyFile);
      }

      // TODO: How to optimize this??
      foreach (var plNameWithoutExtension in plFilesByName.Keys) {
        var plFilesStartingWithName =
            plFilesByName
                .Values
                .Where(otherPlFile => {
                  var otherPlNameWithoutExtension =
                      otherPlFile.NameWithoutExtension;
                  return otherPlNameWithoutExtension.StartsWith(
                             plNameWithoutExtension) &&
                         otherPlNameWithoutExtension.Length >
                         plNameWithoutExtension.Length;
                })
                .ToDictionary(file => file.NameWithoutExtension);

        if (!plFilesStartingWithName.TryGetValue(
                $"{plNameWithoutExtension}{ANIMATION_SUFFIX}",
                out var animationPlFile)) {
          continue;
        }

        foreach (var modelFile in
                 plFilesStartingWithName
                     .Where(pair => !pair.Key.EndsWith(ANIMATION_SUFFIX))
                     .Select(pair => pair.Value)) {
          yield return new DatModelFileBundle {
              GameName = "super_smash_bros_melee",
              PrimaryDatFile = modelFile,
              AnimationDatFile = animationPlFile,
          }.Annotate(modelFile);
        }
      }
    }
  }
}