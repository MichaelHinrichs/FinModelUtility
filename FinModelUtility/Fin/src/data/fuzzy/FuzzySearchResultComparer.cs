using System.Collections.Generic;

namespace fin.data.fuzzy {
  public class FuzzySearchResultComparer<T> : IComparer<IFuzzySearchResult<T>> {
    public int Compare(IFuzzySearchResult<T> lhs,
                       IFuzzySearchResult<T> rhs)
      => -lhs.MatchPercentage.CompareTo(rhs.MatchPercentage);
  }
}
