using fin.io;
using fin.log;
using fin.util.asserts;

using uni.msg;
using uni.platforms;
using uni.platforms.threeDs;
using uni.util.separator;

using zar.api;

namespace uni.games.luigis_mansion_3d {
  public class LuigisMansion3dExtractor {
    private readonly ILogger logger_ =
        Logging.Create<LuigisMansion3dExtractor>();

    private readonly IModelSeparator separator_ =
        new ModelSeparator(directory => directory.LocalPath)
            .Register(@"\effect\effect_mdl", new PrefixModelSeparatorMethod())
            .Register(@"\model\dluige01",
                      new NameModelSeparatorMethod("Luigi.cmb"))
            .Register(@"\model\dluige02",
                      new NameModelSeparatorMethod("Luigi.cmb"))
            .Register(@"\model\dluige03",
                      new NameModelSeparatorMethod("Luigi.cmb"))
            .Register(@"\model\luige",
                      new NameModelSeparatorMethod("Luigi.cmb"));

    public void ExtractAll() {
      var luigisMansionRom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "luigis_mansion_3d.cia");

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor().ExtractFromRom(
              luigisMansionRom);

      /*this.ExtractModel_(
          fileHierarchy.Root.TryToGetSubdir(@"\model\luige_head"));
      return;*/

      foreach (var subdir in fileHierarchy) {
        this.ExtractModel_(subdir);
      }
    }

    public void ExtractModel_(IFileHierarchyDirectory subdir) {
      var cmbFiles = subdir.FilesWithExtension(".cmb").ToArray();
      if (cmbFiles.Length == 0) {
        return;
      }

      var csabFiles = subdir.FilesWithExtension(".csab").ToArray();
      var ctxbFiles = subdir.FilesWithExtension(".ctxb").ToArray();
      var shpaFiles = subdir.FilesWithExtension(".shpa").ToArray();
      var bundles =
          this.separator_.Separate(subdir, cmbFiles, csabFiles);

      foreach (var bundle in bundles) {
        this.ExtractModels_(subdir,
                            new[] { bundle.ModelFile },
                            bundle.AnimationFiles.ToArray(),
                            ctxbFiles,
                            shpaFiles);
      }
    }

    private void ExtractModels_(
        IFileHierarchyDirectory directory,
        IReadOnlyList<IFileHierarchyFile> cmbFiles,
        IReadOnlyList<IFileHierarchyFile>? csabFiles = null,
        IReadOnlyList<IFileHierarchyFile>? ctxbFiles = null,
        IReadOnlyList<IFileHierarchyFile>? shpaFiles = null
    ) {
      Asserts.True(cmbFiles.Count > 0);

      var outputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForDirectory(directory);

      var matches = 0;
      var existingModelFiles =
          outputDirectory.GetExistingFiles()
                         .Where(file => file.Extension == ".fbx" ||
                                        file.Extension == ".glb")
                         .ToArray();

      foreach (var cmbFile in cmbFiles) {
        if (existingModelFiles.Any(
                existingModelFile => {
                  var existingName = existingModelFile.NameWithoutExtension;
                  var cmbName = cmbFile.NameWithoutExtension;

                  return cmbName == existingName ||
                         cmbName + "_gltf" == existingName;
                })) {
          ++matches;
        }
      }

      /*if (matches == cmbFiles.Count) {
        MessageUtil.LogAlreadyProcessed(this.logger_, directory, cmbFiles);
        return;
      }*/

      MessageUtil.LogExtracting(this.logger_, directory, cmbFiles);

      //try {
      new ManualZar2FbxApi().Run(outputDirectory,
                                 cmbFiles.Select(file => file.Impl)
                                         .ToArray(),
                                 csabFiles?.Select(file => file.Impl)
                                          .ToArray(),
                                 ctxbFiles?.Select(file => file.Impl)
                                          .ToArray(),
                                 shpaFiles?.Select(file => file.Impl)
                                          .ToArray(),
                                 30);
      /*} catch (Exception e) {
        this.logger_.LogError(e.ToString());
      }*/
      this.logger_.LogInformation(" ");
    }
  }
}