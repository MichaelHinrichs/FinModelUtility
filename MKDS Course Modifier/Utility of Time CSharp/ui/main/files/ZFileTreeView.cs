using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using UoT.common.fuzzy;
using UoT.memory.files;
using UoT.ui.common.component;
using UoT.util;

namespace UoT.ui.main.files {
  public partial class ZFileTreeView : UserControl {
    // TODO: Add tests.
    // TODO: Move the fuzzy logic to a separate reusable component.
    // TODO: Add support for different sorting systems.
    // TODO: Add support for different hierarchies.
    // TODO: Clean up the logic here.

    private readonly BetterTreeView<ZFileNode> betterTreeView_;

    private LinkedList<ZFileNode> zFileNodes_ = new LinkedList<ZFileNode>();

    private readonly IFuzzySearchDictionary<ZFileNode> filterImpl_ =
        new SymSpellFuzzySearchDictionary<ZFileNode>();

    public delegate void FileSelectedHandler(IZFile file);

    public event FileSelectedHandler FileSelected = delegate {};

    // TODO: Clean this up.
    private class ZFileNode {
      public ZFileNode? Parent { get; set; }
      public IZFile? ZFile { get; set; }
      public ISet<string> Keywords { get;} = new HashSet<string>();
      public double MatchPercentage { get; set; }

      public IList<ZFileNode> AllChildZFileNodes { get; } =
        new List<ZFileNode>();
    }

    public ZFileTreeView() {
      this.InitializeComponent();

      this.filterTextBox_.TextChanged += (sender, args) => { this.Filter_(); };

      this.betterTreeView_ = new BetterTreeView<ZFileNode>(this.fileTreeView_);

      this.betterTreeView_.Selected += betterTreeNode => {
        var selectedFile = betterTreeNode?.AssociatedData?.ZFile;
        if (selectedFile != null) {
          this.FileSelected.Invoke(selectedFile);
        }
      };
    }

    public void Populate(ZFiles zFiles) {
      this.betterTreeView_.BeginUpdate();

      var root = this.betterTreeView_.Root;

      var modelsNode = root.Add("Actor models");
      this.AddZFileNodeFor(null, modelsNode);
      foreach (var model in zFiles.Objects) {
        var modelNode = modelsNode.Add(model.BetterFileName!);
        this.AddZFileNodeFor(model, modelNode);
      }

      var actorCodeNode = root.Add("Actor code");
      this.AddZFileNodeFor(null, actorCodeNode);
      foreach (var code in zFiles.ActorCode) {
        var codeNode = actorCodeNode.Add(code.BetterFileName!);
        this.AddZFileNodeFor(code, codeNode);
      }

      var scenesNode = root.Add("Scenes");
      this.AddZFileNodeFor(null, scenesNode);
      foreach (var scene in zFiles.Scenes) {
        var sceneNode = scenesNode.Add(scene.BetterFileName!);
        this.AddZFileNodeFor(scene, sceneNode);

        foreach (var map in Asserts.Assert(scene.Maps)) {
          var mapNode = sceneNode.Add(map.BetterFileName!);
          this.AddZFileNodeFor(map, mapNode);
        }
      }

      var othersNode = root.Add("Others");
      this.AddZFileNodeFor(null, othersNode);
      foreach (var other in zFiles.Others) {
        var otherNode = othersNode.Add(other.BetterFileName!);
        this.AddZFileNodeFor(other, otherNode);
      }

      this.InitializeAutocomplete_();

      this.betterTreeView_.EndUpdate();
    }

    private void AddZFileNodeFor(
        IZFile? zFile,
        BetterTreeNode<ZFileNode> treeNode) {
      var parentZFileNode = treeNode.Parent?.AssociatedData;

      var zFileNode = new ZFileNode {
          Parent = parentZFileNode,
          ZFile = zFile,
      };
      treeNode.AssociatedData = zFileNode;

      this.zFileNodes_.AddLast(zFileNode);
      parentZFileNode?.AllChildZFileNodes?.Add(zFileNode);

      // Gathers keywords.
      var keywords = zFileNode.Keywords;

      if (zFile != null) {
        var fileName = zFile.FileName;
        keywords.Add(fileName!);

        var betterFileName = zFile.BetterFileName;
        if (!string.IsNullOrEmpty(betterFileName)) {
          keywords.Add(betterFileName!);
        }

        foreach (var keyword in keywords) {
          this.filterImpl_.Add(keyword, zFileNode);
        }
      }
    }

    private void InitializeAutocomplete_() {
      var allAutocompleteKeywords = new AutoCompleteStringCollection();

      foreach (var zFileNode in this.zFileNodes_) {
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
                => Asserts.Assert(betterTreeNode.AssociatedData).MatchPercentage >=
                   matchPercentage);

        this.betterTreeView_.Comparer =
            this.betterTreeView_.Comparer ?? new FuzzyTreeComparer();
      }

      this.betterTreeView_.EndUpdate();
    }

    private void ResetMatchPercentages_() {
      foreach (var zFileNode in this.zFileNodes_) {
        zFileNode.MatchPercentage = 0;
      }
    }

    private void PropagateMatchPercentages_(
        IEnumerable<IFuzzySearchResult<ZFileNode>> matches) {
      foreach (var match in matches) {
        ZFileTreeView.SetMatchPercentage_(match.AssociatedData,
                                          match.MatchPercentage);
      }
    }

    private static void SetMatchPercentage_(
        ZFileNode zFileNode,
        double matchPercentage) {
      if (matchPercentage <= zFileNode.MatchPercentage) {
        return;
      }
      zFileNode.MatchPercentage = matchPercentage;

      var parentZFileNode = zFileNode.Parent;
      if (parentZFileNode == null) {
        return;
      }

      ZFileTreeView.SetMatchPercentage_(parentZFileNode, matchPercentage);
    }

    private class FuzzyTreeComparer : IComparer<BetterTreeNode<ZFileNode>> {
      public int Compare(
          BetterTreeNode<ZFileNode> lhs,
          BetterTreeNode<ZFileNode> rhs)
        => -Asserts.Assert(lhs.AssociatedData).MatchPercentage.CompareTo(
               Asserts.Assert(rhs.AssociatedData).MatchPercentage);
    }
  }
}