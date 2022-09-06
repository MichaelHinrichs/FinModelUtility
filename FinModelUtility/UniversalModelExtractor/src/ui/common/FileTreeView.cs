using System.Reflection;

using fin.data.fuzzy;
using fin.data.queue;
using fin.io;
using fin.util.asserts;

#pragma warning disable CS8604


namespace uni.ui.common {
  public interface IFileTreeView<TFile> {
    public delegate void FileSelectedHandler(IFileTreeNode<TFile> fileNode);

    event FileSelectedHandler FileSelected;


    public delegate void DirectorySelectedHandler(
        IFileTreeNode<TFile> directoryNode);

    event DirectorySelectedHandler DirectorySelected;
  }

  public interface IFileTreeNode<TFile> {
    string Text { get; }

    TFile? File { get; }

    IFileTreeNode<TFile>? Parent { get; }
    IEnumerable<IFileTreeNode<TFile>> Children { get; }
  }


  public abstract partial class FileTreeView<TFile, TFiles> : UserControl,
      IFileTreeView<TFile>
      where TFile : notnull, IUiFile where TFiles : notnull {
    // TODO: Add tests.
    // TODO: Move the fuzzy logic to a separate reusable component.
    // TODO: Add support for different sorting systems.
    // TODO: Add support for different hierarchies.
    // TODO: Clean up the logic here.

    private readonly BetterTreeView<FileNode> betterTreeView_;

    private readonly IFuzzyFilterTree<FileNode> filterImpl_ =
        new FuzzyFilterTree<FileNode>(fileNode => {
          var keywords = new HashSet<string>();

          var file = fileNode.File;
          if (file != null) {
            var fileName = file.FileName;
            keywords.Add(fileName);

            var betterFileName = file.BetterFileName;
            if (!string.IsNullOrEmpty(betterFileName)) {
              keywords.Add(betterFileName);
            }
          }

          return keywords;
        });

    public event IFileTreeView<TFile>.FileSelectedHandler FileSelected =
        delegate { };


    public event IFileTreeView<TFile>.DirectorySelectedHandler
        DirectorySelected = delegate { };


    // TODO: Clean this up.
    protected class FileNode : IFileTreeNode<TFile> {
      private static readonly Assembly assembly_ =
          Assembly.GetExecutingAssembly();

      private static readonly Image folderClosedImage =
          EmbeddedResourceUtil.Load(FileNode.assembly_,
                                    "uni.img.folder_closed.png");

      private static readonly Image folderOpenImage =
          EmbeddedResourceUtil.Load(FileNode.assembly_,
                                    "uni.img.folder_open.png");

      private static readonly Image fileImage =
          EmbeddedResourceUtil.Load(FileNode.assembly_, "uni.img.file.png");

      private readonly IBetterTreeNode<FileNode> treeNode_;
      private readonly IFuzzyNode<FileNode> filterNode_;

      public FileNode(FileTreeView<TFile, TFiles> treeView) {
        this.treeNode_ = treeView.betterTreeView_.Root;
        this.treeNode_.Data = this;

        this.filterNode_ = treeView.filterImpl_.Root.AddChild(this);

        this.InitDirectory_();
      }

      private FileNode(FileNode parent, TFile file) {
        this.File = file;

        this.treeNode_ =
            parent.treeNode_.Add(file.BetterFileName ?? file.FileName);
        this.treeNode_.Data = this;

        this.filterNode_ = parent.filterNode_.AddChild(this);

        this.InitFile_();
      }

      private FileNode(FileNode parent, string text) {
        this.treeNode_ = parent.treeNode_.Add(text);
        this.treeNode_.Data = this;

        this.filterNode_ = parent.filterNode_.AddChild(this);

        this.InitDirectory_();
      }

      private void InitDirectory_() {
        this.treeNode_.ClosedImage = FileNode.folderClosedImage;
        this.treeNode_.OpenImage = FileNode.folderOpenImage;
      }

      private void InitFile_() {
        this.treeNode_.ClosedImage = FileNode.fileImage;
        this.treeNode_.OpenImage = FileNode.fileImage;
      }

      public string Text => this.treeNode_.Text ?? "n/a";

      public TFile? File { get; set; }

      public IFileTreeNode<TFile>? Parent => this.filterNode_.Parent?.Data;
      public float MatchPercentage => this.filterNode_.MatchPercentage;
      public IReadOnlySet<string> Keywords => this.filterNode_.Keywords;

      public FileNode AddChild(TFile file) => new(this, file);
      public FileNode AddChild(string text) => new(this, text);

      public IEnumerable<IFileTreeNode<TFile>> Children
        => this.filterNode_.Children.Select(fuzzyNode => fuzzyNode.Data);

      public void Expand() => this.treeNode_.Expand();
    }

    public FileTreeView() {
      this.InitializeComponent();

      this.filterTextBox_.TextChanged += (_, _) => this.Filter_();

      this.betterTreeView_ = new BetterTreeView<FileNode>(this.fileTreeView_);
      this.betterTreeView_.Selected += betterTreeNode => {
        var fileNode = betterTreeNode.Data;
        if (fileNode == null) {
          return;
        }

        var selectedFile = fileNode.File;
        if (selectedFile != null) {
          this.FileSelected.Invoke(fileNode);
        } else {
          this.DirectorySelected.Invoke(fileNode);
        }
      };
    }

    public void Populate(TFiles files) {
      this.betterTreeView_.BeginUpdate();

      this.PopulateImpl(files, new FileNode(this));

      this.betterTreeView_.ScrollToTop();

      this.InitializeAutocomplete_();

      this.betterTreeView_.EndUpdate();
    }

    protected abstract void PopulateImpl(
        TFiles files,
        FileNode root);

    private void InitializeAutocomplete_() {
      var allAutocompleteKeywords = new AutoCompleteStringCollection();

      var queue = new FinQueue<IFuzzyNode<FileNode>>(this.filterImpl_.Root);
      while (queue.TryDequeue(out var filterNode)) {
        foreach (var keyword in filterNode.Keywords) {
          allAutocompleteKeywords.Add(keyword);
        }

        queue.Enqueue(filterNode.Children);
      }

      this.filterTextBox_.AutoCompleteCustomSource = allAutocompleteKeywords;
    }

    // TODO: Debounce this method.
    private void Filter_() {
      var filterText = this.filterTextBox_.Text.ToLower();

      this.filterImpl_.Reset();

      this.betterTreeView_.BeginUpdate();

      if (string.IsNullOrEmpty(filterText)) {
        this.betterTreeView_.Root.ResetChildrenRecursively();

        this.betterTreeView_.Comparer = null;
      } else {
        const float minMatchPercentage = .2f;
        this.filterImpl_.Filter(filterText, minMatchPercentage);

        this.betterTreeView_.Root.ResetChildrenRecursively(
            betterTreeNode
                => Asserts.Assert(betterTreeNode.Data).MatchPercentage >=
                   minMatchPercentage);

        this.betterTreeView_.Comparer ??= new FuzzyTreeComparer();
      }

      this.betterTreeView_.EndUpdate();
    }

    private class FuzzyTreeComparer : IComparer<IBetterTreeNode<FileNode>> {
      public int Compare(
          IBetterTreeNode<FileNode> lhs,
          IBetterTreeNode<FileNode> rhs)
        => -Asserts.Assert(lhs.Data)
                   .MatchPercentage.CompareTo(
                       Asserts.Assert(rhs.Data).MatchPercentage);
    }
  }
}