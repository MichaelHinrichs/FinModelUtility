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
          .Register("zelda_cow", new AllAnimationsModelSeparatorMethod())
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
          .Register("zelda_rd", new NoAnimationsModelSeparatorMethod())
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
          .Register("zelda_xc", new NoAnimationsModelSeparatorMethod())
          .Register("zelda_zf", new NoAnimationsModelSeparatorMethod());

    public IModelDirectory<ZarModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var ocarinaOfTime3dRom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "ocarina_of_time_3d.cia");

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