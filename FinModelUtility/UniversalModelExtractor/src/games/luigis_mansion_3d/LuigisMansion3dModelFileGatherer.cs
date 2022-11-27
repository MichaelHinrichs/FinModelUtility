using fin.io;

using uni.platforms;
using uni.platforms.threeDs;
using uni.util.separator;

using cmb.api;
using fin.io.bundles;


namespace uni.games.luigis_mansion_3d {
  public class LuigisMansion3dModelFileGatherer 
      : IFileBundleGatherer<CmbModelFileBundle> {
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

    public IFileBundleDirectory<CmbModelFileBundle>? GatherFileBundles(
        bool assert) {
      var luigisMansionRom =
          DirectoryConstants.ROMS_DIRECTORY.PossiblyAssertExistingFile(
              "luigis_mansion_3d.cia", assert);
      if (luigisMansionRom == null) {
        return null;
      }

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor().ExtractFromRom(
              luigisMansionRom);

      var rootModelDirectory =
          new FileBundleDirectory<CmbModelFileBundle>("luigis_mansion_3d");
      var queue =
          new Queue<(IFileHierarchyDirectory,
              IFileBundleDirectory<CmbModelFileBundle>)>();
      queue.Enqueue((fileHierarchy.Root, rootModelDirectory));
      while (queue.Any()) {
        var (directory, node) = queue.Dequeue();

        this.ExtractModel_(node, directory);

        foreach (var subdir in directory.Subdirs) {
          queue.Enqueue((subdir, node.AddSubdir(subdir.Name)));
        }
      }
      return rootModelDirectory;
    }

    public void ExtractModel_(
        IFileBundleDirectory<CmbModelFileBundle> parentNode,
        IFileHierarchyDirectory subdir) {
      var cmbFiles = subdir.FilesWithExtension(".cmb").ToArray();
      if (cmbFiles.Length == 0) {
        return;
      }

      var csabFiles = subdir.FilesWithExtension(".csab").ToArray();
      var ctxbFiles = subdir.FilesWithExtension(".ctxb").ToArray();
      var shpaFiles = subdir.FilesWithExtension(".shpa").ToArray();

      try {
        var bundles =
            this.separator_.Separate(subdir, cmbFiles, csabFiles);

        foreach (var bundle in bundles) {
          parentNode.AddFileBundle(new CmbModelFileBundle(
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