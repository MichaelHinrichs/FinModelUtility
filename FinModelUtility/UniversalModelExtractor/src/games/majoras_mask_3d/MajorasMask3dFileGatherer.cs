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
        = new ModelSeparator(directory => directory.Name)
          .Register(new SameNameSeparatorMethod(), "zelda2_zoraband")
          .Register(new AllAnimationsModelSeparatorMethod(),
                    "zelda_cow",
                    "zelda_cne",
                    "zelda_crow",
                    "zelda_fz",
                    "zelda_tt",
                    "zelda2_ani",
                    "zelda2_aob",
                    "zelda2_bat",
                    "zelda2_bai",
                    "zelda2_bee",
                    "zelda2_cs",
                    "zelda2_gi_bosskey",
                    "zelda2_gi_compass",
                    "zelda2_gi_fish",
                    "zelda2_gi_hearts",
                    "zelda2_gi_mask03",
                    "zelda2_gi_mask21",
                    "zelda2_gi_mask22",
                    "zelda2_gi_mask23",
                    "zelda2_kz",
                    "zelda_maruta",
                    "zelda2_nb",
                    "zelda2_ny",
                    "zelda2_oyu",
                    "zelda2_pamera",
                    "zelda2_shn",
                    "zelda2_snowwd",
                    "zelda2_tab"
          );


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