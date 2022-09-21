using System.Collections.Generic;
using System.Linq;

using FastFuzzyStringMatcher;

namespace fin.data.fuzzy {
  public class BkTreeFuzzySearchDictionary<T> : IFuzzySearchDictionary<T> {
    private readonly StringMatcher<T> impl_ = new();

    public void Add(string keyword, T associatedData)
      => this.impl_.Add(keyword, associatedData);

    public IEnumerable<IFuzzySearchResult<T>> Search(
        string filterText,
        float minMatchPercentage)
      => this.impl_.Search(filterText, minMatchPercentage * 100)
             .Select(searchResult
                         => new BkTreeFuzzySearchResult(
                             searchResult.AssociatedData,
                             searchResult.MatchPercentage));

    private class BkTreeFuzzySearchResult : IFuzzySearchResult<T> {
      public T Data { get; }
      public int ChangeDistance { get; }
      public float Similarity { get; }

      public BkTreeFuzzySearchResult(T data, float matchPercentage) {
        this.Data = data;
        this.Similarity = matchPercentage;
      }
    }
  }
}