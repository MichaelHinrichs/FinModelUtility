using fin.io;
using fin.io.bundles;

using pmdc.api;

using uni.platforms;

namespace uni.games.paper_mario_directors_cut {
  public class PaperMarioDirectorsCutFileGatherer
      : IAnnotatedFileBundleGatherer<OmdModelFileBundle> {
    public IEnumerable<IAnnotatedFileBundle<OmdModelFileBundle>>
        GatherFileBundles() {
      if (!DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingSubdir(
              "paper_mario_directors_cut/prereqs",
              out var pmdcDir)) {
        yield break;
      }

      var fileHierarchy =
          new FileHierarchy("paper_mario_directors_cut", pmdcDir);

      var omdFiles = fileHierarchy.Root.GetFilesWithFileType(".omd", true);
      foreach (var omdFile in omdFiles) {
        yield return new AnnotatedFileBundle<OmdModelFileBundle>(
            new OmdModelFileBundle { OmdFile = omdFile },
            omdFile);
      }
    }
  }
}