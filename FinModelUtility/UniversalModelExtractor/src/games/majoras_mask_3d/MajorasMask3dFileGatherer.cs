using uni.platforms;
using uni.platforms.threeDs;
using uni.util.io;
using uni.util.bundles;

using cmb.api;

using fin.io;
using fin.io.bundles;


namespace uni.games.majoras_mask_3d {
  public class MajorasMask3dFileGatherer
      : IFileBundleGatherer<CmbModelFileBundle> {
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


    public IEnumerable<CmbModelFileBundle> GatherFileBundles(
        bool assert) {
      if (!DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "majoras_mask_3d.cia",
              assert,
              out var majorasMask3dRom)) {
        return Enumerable.Empty<CmbModelFileBundle>();
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor()
              .ExtractFromRom(majorasMask3dRom);

      return new FileBundleGathererAccumulator<CmbModelFileBundle>()
             .Add(() => GetModelsViaSeparator_(fileHierarchy))
             .Add(() => this.GetLinkModels_(fileHierarchy))
             .GatherFileBundles(assert);
    }

    private IEnumerable<CmbModelFileBundle> GetModelsViaSeparator_(
        IFileHierarchy fileHierarchy)
      => new FileHierarchyAssetBundleSeparator<CmbModelFileBundle>(
          fileHierarchy,
          subdir => {
            if (!separator_.Contains(subdir)) {
              return Enumerable.Empty<CmbModelFileBundle>();
            }

            var cmbFiles =
                subdir.FilesWithExtensionsRecursive(".cmb").ToArray();
            if (cmbFiles.Length == 0) {
              return Enumerable.Empty<CmbModelFileBundle>();
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
              return Enumerable.Empty<CmbModelFileBundle>();
            }
          }
      ).GatherFileBundles(false);

    private IEnumerable<CmbModelFileBundle> GetLinkModels_(
        IFileHierarchy fileHierarchy) {
      var actorsDir = fileHierarchy.Root.GetExistingSubdir("actors");

      var cmbFiles =
          new[] {
              actorsDir.GetExistingFile("zelda2_link_child_new/link_child.cmb"),
              actorsDir.GetExistingFile("zelda2_link_goron_new/link_goron.cmb"),
              actorsDir.GetExistingFile("zelda2_link_nuts_new/link_deknuts.cmb"),
              actorsDir.GetExistingFile("zelda2_link_zora_new/link_zora.cmb"),
          };

      var csabFiles = fileHierarchy
                      .Root.GetExistingSubdir("actors/zelda2_link_new")
                      .FilesWithExtension(".csab")
                      .ToArray();

      return cmbFiles.Select(cmbFile => new CmbModelFileBundle(
                                 cmbFile,
                                 csabFiles,
                                 null,
                                 null));
    }
  }
}