using System.Collections.Generic;
using System.Linq;

using FastFuzzyStringMatcher;

namespace fin.data.fuzzy {
  public class BkTreeFuzzySearchDictionary<T> : IFuzzySearchDictionary<T> {
    private readonly StringMatcher<T> impl_ = new();

    public void Add(string keyword, T associatedData)
      => this.impl_.Add(keyword, associatedData);

    public IEnumerable<IFuzzySearchResult<T>> Search(
        string keyword,
        float minMatchPercentage)
      => this.impl_.Search(keyword, minMatchPercentage * 100)
             .Select(searchResult
                         => new BkTreeFuzzySearchResult(
                             searchResult.AssociatedData,
                             searchResult.MatchPercentage));

    private class BkTreeFuzzySearchResult : IFuzzySearchResult<T> {
      public T Data { get; }
      public float MatchPercentage { get; }

      public BkTreeFuzzySearchResult(T data, float matchPercentage) {
        this.Data = data;
        this.MatchPercentage = matchPercentage;
      }
    }
  }
}