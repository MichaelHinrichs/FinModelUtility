using System.Collections.Generic;
using System.Windows.Forms;

using UoT.common.fuzzy;
using UoT.ui.common.component;
using UoT.util;

namespace UoT.ui.main.files {
  public interface IUiFile {
    // TODO: Make these nonnull via init setters in C#9.
    string? FileName { get; set; }
    string? BetterFileName { get; set; }
  }

  public abstract partial class FileTreeView<TFile, TFiles> : UserControl
      where TFile : notnull, IUiFile where TFiles : notnull {
    // TODO: Add tests.
    // TODO: Move the fuzzy logic to a separate reusable component.
    // TODO: Add support for different sorting systems.
    // TODO: Add support for different hierarchies.
    // TODO: Clean up the logic here.

    private readonly BetterTreeView<FileNode> betterTreeView_;

    private LinkedList<FileNode> fileNodes_ = new();

    private readonly IFuzzySearchDictionary<FileNode> filterImpl_ =
        new SymSpellFuzzySearchDictionary<FileNode>();

    public delegate void FileSelectedHandler(TFile file);

    public event FileSelectedHandler FileSelected = delegate {};

    // TODO: Clean this up.
    protected class FileNode {
      public FileNode? Parent { get; set; }
      public TFile? File { get; set; }
      public HashSet<string> Keywords { get; } = new();
      public double MatchPercentage { get; set; }

      public List<FileNode> AllChildFileNodes { get; } = new();
    }

    public FileTreeView() {
      this.InitializeComponent();

      this.filterTextBox_.TextChanged += (sender, args) => { this.Filter_(); };

      this.betterTreeView_ = new BetterTreeView<FileNode>(this.fileTreeView_);

      this.betterTreeView_.Selected += betterTreeNode => {
        var associatedData = betterTreeNode?.AssociatedData;
        if (associatedData == null) {
          return;
        }

        var selectedFile = associatedData.File;
        if (selectedFile != null) {
          this.FileSelected.Invoke(selectedFile);
        }
      };
    }

    public void Populate(TFiles files) {
      this.betterTreeView_.BeginUpdate();

      var root = this.betterTreeView_.Root;
      this.PopulateImpl(files, root);

      this.InitializeAutocomplete_();

      this.betterTreeView_.EndUpdate();
    }

    protected abstract void PopulateImpl(
        TFiles files,
        BetterTreeNode<FileNode> root);

    protected void AddFileNodeFor(
        TFile? file,
        BetterTreeNode<FileNode> treeNode) {
      var parentFileNode = treeNode.Parent?.AssociatedData;

      var fileNode = new FileNode {
          Parent = parentFileNode,
          File = file,
      };
      treeNode.AssociatedData = fileNode;

      this.fileNodes_.AddLast(fileNode);
      parentFileNode?.AllChildFileNodes?.Add(fileNode);

      // Gathers keywords.
      var keywords = fileNode.Keywords;

      if (file != null) {
        var fileName = file.FileName;
        keywords.Add(fileName!);

        var betterFileName = file.BetterFileName;
        if (!string.IsNullOrEmpty(betterFileName)) {
          keywords.Add(betterFileName!);
        }

        foreach (var keyword in keywords) {
          this.filterImpl_.Add(keyword, fileNode);
        }
      }
    }

    private void InitializeAutocomplete_() {
      var allAutocompleteKeywords = new AutoCompleteStringCollection();

      foreach (var zFileNode in this.fileNodes_) {
        foreach (var keyword in zFileNode.Keywords) {
          allAutocompleteKeywords.Add(keyword.ToLower());
        }
      }

      this.filterTextBox_.AutoCompleteCustomSource = allAutocompleteKeywords;
    }

    // TODO: Debounce this method.
    private void Filter_() {
      var filterText = this.filterTextBox_.Text.ToLower();

      this.ResetMatchPercentages_();

      this.betterTreeView_.BeginUpdate();

      if (string.IsNullOrEmpty(filterText)) {
        this.betterTreeView_.Root.ResetChildrenRecursively();

        this.betterTreeView_.Comparer = null;
      } else {
        const float matchPercentage = 20;

        var matches = this.filterImpl_.Search(filterText, matchPercentage);

        this.PropagateMatchPercentages_(matches);
        this.betterTreeView_.Root.ResetChildrenRecursively(
            betterTreeNode
                => Asserts.Assert(betterTreeNode.AssociatedData)
                          .MatchPercentage >=
                   matchPercentage);

        this.betterTreeView_.Comparer =
            this.betterTreeView_.Comparer ?? new FuzzyTreeComparer();
      }

      this.betterTreeView_.EndUpdate();
    }

    private void ResetMatchPercentages_() {
      foreach (var fileNode in this.fileNodes_) {
        fileNode.MatchPercentage = 0;
      }
    }

    private void PropagateMatchPercentages_(
        IEnumerable<IFuzzySearchResult<FileNode>> matches) {
      foreach (var match in matches) {
        SetMatchPercentage_(match.AssociatedData, match.MatchPercentage);
      }
    }

    private static void SetMatchPercentage_(
        FileNode fileNode,
        double matchPercentage) {
      if (matchPercentage <= fileNode.MatchPercentage) {
        return;
      }
      fileNode.MatchPercentage = matchPercentage;

      var parentZFileNode = fileNode.Parent;
      if (parentZFileNode == null) {
        return;
      }

      SetMatchPercentage_(parentZFileNode, matchPercentage);
    }

    private class FuzzyTreeComparer : IComparer<BetterTreeNode<FileNode>> {
      public int Compare(
          BetterTreeNode<FileNode> lhs,
          BetterTreeNode<FileNode> rhs)
        => -Asserts.Assert(lhs.AssociatedData)
                   .MatchPercentage.CompareTo(
                       Asserts.Assert(rhs.AssociatedData).MatchPercentage);
    }
  }
}