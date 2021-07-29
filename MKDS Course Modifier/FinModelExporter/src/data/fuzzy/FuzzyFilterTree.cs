using System;
using System.Collections.Generic;

namespace fin.data.fuzzy {
  public class FuzzyFilterTree<T> : IFuzzyFilterTree<T> {
    // TODO: Add tests.
    // TODO: Add support for different sorting systems.

    private readonly List<FuzzyNode> nodes_ = new();
    private readonly Func<T, IReadOnlySet<string>> nodeToKeywords_;

    private readonly IFuzzySearchDictionary<FuzzyNode> impl_ =
        new SymSpellFuzzySearchDictionary<FuzzyNode>();

    // TODO: Clean this up.
    private class FuzzyNode : IFuzzyNode<T> {
      private readonly FuzzyFilterTree<T> tree_;
      private readonly List<IFuzzyNode<T>> children_ = new();

      public FuzzyNode(FuzzyFilterTree<T> tree) {
        this.tree_ = tree;
        tree.nodes_.Add(this);

        this.Keywords = new HashSet<string>();

        this.Children = this.children_;
      }

      private FuzzyNode(
          FuzzyFilterTree<T> tree,
          T data,
          IFuzzyNode<T> parent) {
        this.tree_ = tree;
        tree.nodes_.Add(this);

        this.Data = data;

        this.Keywords = tree.nodeToKeywords_(data);
        foreach (var keyword in this.Keywords) {
          tree.impl_.Add(keyword, this);
        }

        this.Parent = parent;
        this.Children = this.children_;
      }

      public T Data { get; set; }
      public float MatchPercentage { get; set; }

      public IReadOnlySet<string> Keywords { get; }

      public IFuzzyNode<T>? Parent { get; }
      public IReadOnlyList<IFuzzyNode<T>> Children { get; }

      public IFuzzyNode<T> AddChild(T data) {
        FuzzyNode child = new(this.tree_, data, this);
        this.children_.Add(child);
        return child;
      }
    }

    public FuzzyFilterTree(Func<T, IReadOnlySet<string>> nodeToKeywords) {
      this.nodeToKeywords_ = nodeToKeywords;
      this.Root = new FuzzyNode(this);
    }

    public IFuzzyNode<T> Root { get; }

    public void Reset() {
      foreach (var node in this.nodes_) {
        node.MatchPercentage = 0;
      }
    }

    public void Filter(
        string keyword,
        float minMatchPercentage) { 
      var matches = this.impl_.Search(keyword, minMatchPercentage);
      this.PropagateMatchPercentages_(matches);
    }

    private void PropagateMatchPercentages_(
        IEnumerable<IFuzzySearchResult<FuzzyNode>> matches) {
      foreach (var match in matches) {
        SetMatchPercentage_(match.Data, match.MatchPercentage);
      }
    }

    private static void SetMatchPercentage_(
        FuzzyNode node,
        float matchPercentage) {
      if (matchPercentage <= node.MatchPercentage) {
        return;
      }
      node.MatchPercentage = matchPercentage;

      if (node.Parent is not FuzzyNode parentNode) {
        return;
      }

      SetMatchPercentage_(parentNode, matchPercentage);
    }
  }
}