using uni.platforms;
using uni.platforms.threeDs;
using uni.util.io;
using uni.util.bundles;

using cmb.api;
using fin.io.bundles;


namespace uni.games.ocarina_of_time_3d {
  public class OcarinaOfTime3dFileGatherer 
      : IFileBundleGatherer<CmbModelFileBundle> {
    // TODO: Add support for Link
    // TODO: Add support for faceb
    // TODO: Add support for cmab
    // TODO: Add support for other texture types

    // *sigh*
    // Why tf did they have to name things so randomly????
    private readonly IModelSeparator separator_
        = new ModelSeparator(directory => directory.Name)
          .Register(new AllAnimationsModelSeparatorMethod(),
              "zelda_am",
              "zelda_aob",
              "zelda_av",
              "zelda_bb",
              "zelda_bdan_objects",
              "zelda_bji",
              "zelda_bob",
              "zelda_bombf",
              "zelda_bowl",
              "zelda_brob",
              "zelda_bw",
              "zelda_cne",
              "zelda_cow",
              "zelda_crow",
              "zelda_cs",
              "zelda_dodojr",
              "zelda_dodongo",
              "zelda_dog",
              "zelda_ds",
              "zelda_ei",
              "zelda_ff",
              "zelda_fu",
              "zelda_gi_bottle",
              "zelda_gi_coin",
              "zelda_gi_compass",
              "zelda_gi_dekupouch",
              "zelda_gi_egg",
              "zelda_horse",
              "zelda_horse_ganon",
              "zelda_horse_normal",
              "zelda_horse_zelda",
              "zelda_hs",
              "zelda_jj",
              "zelda_ka",
              "zelda_kanban",
              "zelda_kibako2",
              "zelda_killer_door",
              "zelda_km1",
              "zelda_kogoma",
              "zelda_kusa",
              "zelda_kz",
              "zelda_lightswitch",
              "zelda_ma2",
              "zelda_mjin",
              "zelda_rd",
              "zelda_ru1",
              "zelda_ru2",
              "zelda_sa",
              "zelda_tk",
              "zelda_ydan_objects",
              "zelda_yukabyun",
              "zelda_zf",
              "zelda_zl1",
              "zelda_zl2",
              "zelda_zl4",
              "zelda_zo"
          )
          // TODO: This is probably wrong
          .Register("zelda_box",
                    new NoAnimationsModelSeparatorMethod()
              /*new ExactCasesMethod()
                  .Case("demo_tre_lgt_mdl_info.cmb",
                        "demo_tre_lgt_c_fcurve_data.csab",
                        "demo_tre_lgt_fcurve_data.csab")
                  .Case("tr_box.cmb",
                        "cdemo_box_boxA.csab",
                        "demo_box_boxA.csab",
                        "demo_box_boxB.csab")*/)
          // TODO: This is *definitely* wrong
          .Register("zelda_bv",
                    new NoAnimationsModelSeparatorMethod()
              /*new PrefixCasesMethod()
                  .Case("balinadearm", "bva_", "bv_arm_")
                  .Case("bve_model", "bve_")
                  .Case("balinadecore", "bvc_")
                  .Case("efc_bari_model", "bvb_")
                  .Case("bv_inazumaMINI2_modelT", "bl2")*/)
          .Register("zelda_bxa",
                    new ExactCasesMethod()
                        .Case("balinadearm.cmb",
                              "tentacle_motion_test01.csab",
                              "baarm_death.csab")
                        .Case("balinadetrap.cmb", "balinadetrap.csab"))
          // TODO: Figure these all out
          .Register("zelda_dekubaba", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_dekunuts", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_dh", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_dnk", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_dns", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_dy_obj", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_ec", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_ec2", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_efc_tw", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_fantomHG", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_fd", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_field_keep", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_fishing", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_fr", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_fw", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_ganon", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_ganon2", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_ganon_down", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_gnd", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_gndd", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_goma", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_haka_door", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_hidan_objects",
                    new NoAnimationsModelSeparatorMethod())
          .Register("zelda_hintnuts", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_ik", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_mag", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_mb", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_mizu_objects",
                    new NoAnimationsModelSeparatorMethod())
          .Register("zelda_mu", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_nw", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_oc2", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_oF1d", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_owl", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_ph", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_po", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_po_composer", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_po_field", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_po_sisters", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_ps", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_sd", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_shopnuts", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_skj", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_spot02_objects",
                    new NoAnimationsModelSeparatorMethod())
          .Register("zelda_sst", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_st", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_tr", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_tw", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_vali", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_wm2", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_xc", new NoAnimationsModelSeparatorMethod());

    public IEnumerable<CmbModelFileBundle> GatherFileBundles(bool assert) {
      var ocarinaOfTime3dRom =
          DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "ocarina_of_time_3d.cia", assert);
      if (ocarinaOfTime3dRom == null) {
        return Enumerable.Empty<CmbModelFileBundle>();
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor()
              .ExtractFromRom(ocarinaOfTime3dRom);

      return new FileHierarchyAssetBundleSeparator<CmbModelFileBundle>(
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
      ).GatherFileBundles(assert);
    }
  }
}