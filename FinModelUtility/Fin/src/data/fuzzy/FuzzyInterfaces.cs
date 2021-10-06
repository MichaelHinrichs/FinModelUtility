using System.Collections.Generic;

using fin.util.asserts;

namespace fin.data.fuzzy {
  // TODO: Need to support tokenizing.

  public interface IFuzzySearchResult<out T> {
    T Data { get; }
    float MatchPercentage { get; }
  }

  public interface IFuzzySearchDictionary<T> {
    void Add(string keyword, T data);

    IEnumerable<IFuzzySearchResult<T>> Search(
        string keyword,
        float minMatchPercentage);
  }


  public interface IFuzzyNode<T> {
    T Data { get; set; }
    float MatchPercentage { get; }

    IReadOnlySet<string> Keywords { get; }

    IFuzzyNode<T>? Parent { get; }
    IReadOnlyList<IFuzzyNode<T>> Children { get; }

    IFuzzyNode<T> AddChild(T data);
  }

  public interface IFuzzyFilterTree<T> {
    IFuzzyNode<T> Root { get; }

    void Reset();

    void Filter(
        string keyword,
        float minMatchPercentage);
  }
}