using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fin.src.data {
  public class ReadOnlyListDictionary<TKey, TValue>
      : IEnumerable<KeyValuePair<TKey, IReadOnlyList<TValue>>>
      where TKey : notnull {
    private readonly IReadOnlyDictionary<TKey, IReadOnlyList<TValue>> impl_;

    public ReadOnlyListDictionary(
        IEnumerable<TValue> values,
        Func<TValue, TKey> valueToKey) {
      var readonlyDict = new Dictionary<TKey, IReadOnlyList<TValue>>();

      var dict = new Dictionary<TKey, IList<TValue>>();
      foreach (var value in values) {
        var key = valueToKey(value);

        if (dict.TryGetValue(key, out var valueList)) {
          valueList.Add(value);
        } else {
          valueList = new List<TValue>();
          valueList.Add(value);

          dict[key] = valueList;
          readonlyDict[key] = new ReadOnlyCollection<TValue>(valueList);
        }
      }

      this.impl_ =
          new ReadOnlyDictionary<TKey, IReadOnlyList<TValue>>(readonlyDict);
    }


    public IEnumerable<TKey> Keys => this.impl_.Keys;
    public IEnumerable<IReadOnlyList<TValue>> Values => this.impl_.Values;

    
    public IReadOnlyCollection<TValue> this[TKey key] => this.impl_[key];


    public IEnumerator<KeyValuePair<TKey, IReadOnlyList<TValue>>>
        GetEnumerator()
      => this.impl_.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => ((IEnumerable) this.impl_).GetEnumerator();
  }
}