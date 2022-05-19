namespace uni.ui.common {
  public class ModelFile : IUiFile {
    public string? FileName { get; set; }
    public string? BetterFileName { get; set; }
  }

  public class ModelFileTreeView : FileTreeView<ModelFile, ModelFile> {
    protected override void PopulateImpl(ModelFile zFiles, FileNode root) {
      var modelsNode = root.AddChild("Glover");
    }
  }
}