using fin.model;

using uni.platforms;
using uni.platforms.threeDs;
using uni.util.io;
using uni.util.separator;

using cmb.api;


namespace uni.games.majoras_mask_3d {
  public class MajorasMask3dFileGatherer : IModelFileGatherer<
      CmbModelFileBundle> {
    private readonly IModelSeparator separator_
        = new ModelSeparator(directory => directory.Name);


    public IModelDirectory<CmbModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var majorasMask3dRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "majoras_mask_3d.cia");
      if (majorasMask3dRom == null) {
        return null;
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor()
              .ExtractFromRom(majorasMask3dRom);

      return new FileHierarchyBundler<CmbModelFileBundle>(
          subdir => {
            if (!separator_.Contains(subdir)) {
              return null;
            }

            var cmbFiles =
                subdir.FilesWithExtensionsRecursive(".cmb").ToArray();
            if (cmbFiles.Length == 0) {
              return null;
            }

            var csabFiles =
                subdir.FilesWithExtensionsRecursive(".csab").ToArray();
            var ctxbFiles =
                subdir.FilesWithExtensionsRecursive(".ctxb").ToArray();

            try {
              var bundles =
                  this.separator_.Separate(subdir, cmbFiles, csabFiles);

              return bundles.Select(bundle => new CmbModelFileBundle(
                                        bundle.ModelFile,
                                        bundle.AnimationFiles.ToArray(),
                                        ctxbFiles,
                                        null
                                    ));
            } catch {
              return null;
            }
          }
      ).GatherBundles(fileHierarchy);
    }
  }
}