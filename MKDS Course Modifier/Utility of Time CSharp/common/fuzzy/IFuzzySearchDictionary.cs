using System.Collections.Generic;

namespace UoT.common.fuzzy {
  // TODO: Need to support tokenizing.

  public interface IFuzzySearchResult<out T> {
    T AssociatedData { get; }
    float MatchPercentage { get; }
  }

  public interface IFuzzySearchDictionary<T> {
    void Add(string keyword, T associatedData);
    IEnumerable<IFuzzySearchResult<T>> Search(string keyword, float matchPercentage);
  }
}