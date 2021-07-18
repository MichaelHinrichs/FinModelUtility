using System.Collections.Generic;
using System.Linq;

using FastFuzzyStringMatcher;

namespace UoT.common.fuzzy {
  public class BkTreeFuzzySearchResult<T> : IFuzzySearchResult<T> {
    // TODO: Make notnull w/ C#9.
    public T AssociatedData { get; }
    public float MatchPercentage { get; set; }

    public BkTreeFuzzySearchResult(T associatedData) {
      this.AssociatedData = associatedData;
    }
  }

  public class BkTreeFuzzySearchDictionary<T> : IFuzzySearchDictionary<T> {
    private readonly StringMatcher<T> impl_ = new StringMatcher<T>();

    public void Add(string keyword, T associatedData)
      => this.impl_.Add(keyword, associatedData);

    public IEnumerable<IFuzzySearchResult<T>> Search(
        string keyword,
        float matchPercentage)
      => this.impl_.Search(keyword, matchPercentage)
             .Select(searchResult
                         => new BkTreeFuzzySearchResult<T>(
                             searchResult.AssociatedData) {
                             MatchPercentage = searchResult.MatchPercentage
                         });
  }
}