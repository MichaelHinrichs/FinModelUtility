using System;
using System.Collections.Generic;
using System.Linq;


namespace fin.data.fuzzy {
  public class
      LevenshteinTreeFuzzySearchDictionary<T> : IFuzzySearchDictionary<T> {
    private readonly ListDictionary<T, string> dataToKeywords_ = new();

    public int MaxChangeDistance { get; set; } = 0;

    public void Add(string keyword, T associatedData)
      => this.dataToKeywords_.Add(associatedData, keyword);

    public IEnumerable<IFuzzySearchResult<T>> Search(
        string filterText,
        float minMatchPercentage) {
      var filterTokens = filterText.Split(' ');

      /*filterTokens.Select(token => {
            this.dataToKeywords_.Select(pair => {
              var (data, keywords) = pair;

              var changeDistanceAndSimilarities =
                  keywords.Select(keyword => {
                    var changeDistance =
                        token.Length -
                        FindLongestLengthOfSubstring(
                            keyword, token);
                    var addOrRemoveCount =
                        Math.Abs(keyword.Length - token.Length);


                  });
            });
          });*/
      return null!;
    }

    private record FuzzySearchResult
        (T Data, float MatchPercentage) : IFuzzySearchResult<T>;

    private int
        FindLongestLengthOfSubstring(string container, string substring) {
      var substringLength = substring.Length;
      var matchLengths = new int[substringLength];

      var bestMatchLength = 0;
      for (var containerIndex = 0;
           containerIndex < container.Length;
           ++containerIndex) {
        var containerChar = container[containerIndex];

        for (var substringIndex = 0;
             substringIndex < substringLength;
             ++substringIndex) {
          var i = (containerIndex + substringIndex) % substringLength;
          var substringChar = substring[substringIndex];

          var matchLength = i == 0 ? 0 : matchLengths[substringIndex];

          matchLength += substringChar == containerChar ? 1 : 0;

          matchLengths[substringIndex] = matchLength;

          bestMatchLength = Math.Max(bestMatchLength, matchLength);
          if (bestMatchLength == substringLength) {
            return bestMatchLength;
          }
        }
      }

      return bestMatchLength;
    }
  }
}