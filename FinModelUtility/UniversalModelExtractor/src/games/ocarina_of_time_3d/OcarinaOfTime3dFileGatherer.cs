using fin.io;
using fin.model;

using uni.platforms;
using uni.platforms.threeDs;
using uni.util.separator;

using zar.api;


namespace uni.games.ocarina_of_time_3d {
  public class OcarinaOfTime3dFileGatherer : IModelFileGatherer<
      ZarModelFileBundle> {
    // TODO: Add support for Link
    // TODO: Add support for faceb
    // TODO: Add support for cmab
    // TODO: Add support for other texture types

    // *sigh*
    // Why tf did they have to name things so randomly????
    private readonly IModelSeparator separator_
        = new ModelSeparator(directory => directory.Name)
          .Register("zelda_am", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_aob", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_av", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_bb", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_bdan_objects", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_bji", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_bob", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_bombf", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_bowl", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_brob", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_bw", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_cow", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_dodojr", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_dodongo", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_dog", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_ds", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_ei", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_ff", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_fu", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_gi_bottle", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_gi_coin", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_gi_compass", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_gi_dekupouch", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_gi_egg", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_horse", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_horse_ganon", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_horse_normal", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_horse_zelda", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_hs", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_jj", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_ka", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_kanban", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_kibako2", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_killer_door", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_km1", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_kogoma", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_kusa", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_kz", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_lightswitch", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_ma2", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_mjin", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_rd", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_ru1", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_ru2", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_sa", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_tk", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_ydan_objects", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_yukabyun", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_zf", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_zl1", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_zl2", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_zl4", new AllAnimationsModelSeparatorMethod())
          .Register("zelda_zo", new AllAnimationsModelSeparatorMethod())
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

    public IModelDirectory<ZarModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var ocarinaOfTime3dRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "ocarina_of_time_3d.cia");
      if (ocarinaOfTime3dRom == null) {
        return null;
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor()
              .ExtractFromRom(ocarinaOfTime3dRom);

      var rootModelDirectory =
          new ModelDirectory<ZarModelFileBundle>("ocarina_of_time_3d");
      var queue =
          new Queue<(IFileHierarchyDirectory,
              IModelDirectory<ZarModelFileBundle>)>();
      queue.Enqueue((fileHierarchy.Root, rootModelDirectory));
      while (queue.Any()) {
        var (directory, node) = queue.Dequeue();

        if (this.separator_.Contains(directory)) {
          this.ExtractModel_(node, directory);
        }

        foreach (var subdir in directory.Subdirs) {
          queue.Enqueue((subdir, node.AddSubdir(subdir.Name)));
        }
      }
      return rootModelDirectory;
    }

    public void ExtractModel_(
        IModelDirectory<ZarModelFileBundle> parentNode,
        IFileHierarchyDirectory subdir) {
      var cmbFiles = subdir.FilesWithExtensionsRecursive(".cmb").ToArray();
      if (cmbFiles.Length == 0) {
        return;
      }

      var csabFiles = subdir.FilesWithExtensionsRecursive(".csab").ToArray();
      var ctxbFiles = subdir.FilesWithExtensionsRecursive(".ctxb").ToArray();
      var shpaFiles = subdir.FilesWithExtensionsRecursive(".shpa").ToArray();

      try {
        var bundles =
            this.separator_.Separate(subdir, cmbFiles, csabFiles);

        foreach (var bundle in bundles) {
          parentNode.AddFileBundle(new ZarModelFileBundle(
                                       bundle.ModelFile,
                                       bundle.AnimationFiles.ToArray(),
                                       ctxbFiles,
                                       shpaFiles
                                   ));
        }
      } catch { }
    }
  }
}