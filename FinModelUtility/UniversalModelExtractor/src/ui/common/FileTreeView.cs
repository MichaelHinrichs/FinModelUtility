using System.Reflection;
using fin.data.fuzzy;
using fin.data.queue;
using fin.io;
using fin.util.actions;
using fin.util.asserts;

#pragma warning disable CS8604


namespace uni.ui.common {
  public interface IFileTreeView<TFile> {
    public delegate void FileSelectedHandler(IFileTreeNode<TFile> fileNode);

    event FileSelectedHandler FileSelected;


    public delegate void DirectorySelectedHandler(
        IFileTreeNode<TFile> directoryNode);

    event DirectorySelectedHandler DirectorySelected;

    Image GetImageForFile(TFile file);
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

            var uiPath = "";
            IFileTreeNode<TFile>? current = fileNode;
            while (current != null) {
              uiPath = $"{current.Text}/{uiPath}";
              current = current.Parent;
            }
            keywords.Add(uiPath);
          }

          return keywords;
        });

    public event IFileTreeView<TFile>.FileSelectedHandler FileSelected =
        delegate { };


    public event IFileTreeView<TFile>.DirectorySelectedHandler
        DirectorySelected = delegate { };


    // TODO: Clean this up.
    protected class FileNode : IFileTreeNode<TFile> {
      private readonly IFileTreeView<TFile> treeview_;
      private readonly IBetterTreeNode<FileNode> treeNode_;
      private readonly IFuzzyNode<FileNode> filterNode_;

      public FileNode(FileTreeView<TFile, TFiles> treeView) {
        this.treeview_ = treeView;
        this.treeNode_ = treeView.betterTreeView_.Root;
        this.treeNode_.Data = this;

        this.filterNode_ = treeView.filterImpl_.Root.AddChild(this);

        this.InitDirectory_();
      }

      private FileNode(FileNode parent, TFile file) {
        this.File = file;

        this.treeview_ = parent.treeview_;
        this.treeNode_ =
            parent.treeNode_.Add(file.BetterFileName ?? file.FileName);
        this.treeNode_.Data = this;

        this.filterNode_ = parent.filterNode_.AddChild(this);

        this.InitFile_();
      }

      private FileNode(FileNode parent, string text) {
        this.treeview_ = parent.treeview_;
        this.treeNode_ = parent.treeNode_.Add(text);
        this.treeNode_.Data = this;

        this.filterNode_ = parent.filterNode_.AddChild(this);

        this.InitDirectory_();
      }

      private void InitDirectory_() {
        this.treeNode_.ClosedImage = Icons.folderClosedImage;
        this.treeNode_.OpenImage = Icons.folderOpenImage;
      }

      private void InitFile_()
        => this.treeNode_.ClosedImage =
               this.treeNode_.OpenImage =
                   this.treeview_.GetImageForFile(this.File!);

      public string Text => this.treeNode_.Text ?? "n/a";

      public TFile? File { get; set; }

      public IFileTreeNode<TFile>? Parent => this.treeNode_.Parent?.Data;
      public float Similarity => this.filterNode_.Similarity;
      public float ChangeDistance => this.filterNode_.ChangeDistance;
      public IReadOnlySet<string> Keywords => this.filterNode_.Keywords;

      public FileNode AddChild(TFile file) => new(this, file);
      public FileNode AddChild(string text) => new(this, text);

      public IEnumerable<IFileTreeNode<TFile>> Children
        => this.filterNode_.Children.Select(fuzzyNode => fuzzyNode.Data);

      public void Expand() => this.treeNode_.Expand();
    }

    public FileTreeView() {
      this.InitializeComponent();

      var callFilterFromMainThread = () => this.Invoke(this.Filter_);
      this.filterDebounced_ = callFilterFromMainThread.Debounce();
      this.filterTextBox_.TextChanged += (_, _) => this.filterDebounced_();

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

    public abstract Image GetImageForFile(TFile file);


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

    private readonly Action filterDebounced_;

    private void Filter_() {
      var filterText = this.filterTextBox_.Text.ToLower();

      if (string.IsNullOrEmpty(filterText) || filterText.Length <= 2) {
        if (this.betterTreeView_.Comparer != null) {
          this.betterTreeView_.SelectedNode?.EnsureParentIsExpanded();

          this.filterImpl_.Reset();
          this.betterTreeView_.BeginUpdate();

          this.betterTreeView_.Root.ResetChildrenRecursively();
          this.betterTreeView_.Comparer = null;

          this.betterTreeView_.EndUpdate();
        }
      } else {
        this.filterImpl_.Reset();
        this.betterTreeView_.BeginUpdate();
        
        this.filterImpl_.Filter(filterText, -1);
        this.betterTreeView_.Root.ResetChildrenRecursively(
            betterTreeNode =>
                Asserts.Assert(betterTreeNode.Data).ChangeDistance <= 0);
        this.betterTreeView_.Comparer ??= new FuzzyTreeComparer();

        this.betterTreeView_.EndUpdate();
      }
    }

    private class FuzzyTreeComparer : IComparer<IBetterTreeNode<FileNode>> {
      public int Compare(
          IBetterTreeNode<FileNode> lhs,
          IBetterTreeNode<FileNode> rhs)
        => -Asserts.Assert(lhs.Data)
                   .Similarity.CompareTo(
                       Asserts.Assert(rhs.Data).Similarity);
    }
  }
}