using System.Collections.Generic;

namespace fin.data.dictionaries {
  public interface IReadOnlyFinDictionary<TKey, TValue>
      : IReadOnlyFinCollection<(TKey Key, TValue Value)> {
    IEnumerable<TKey> Keys { get; }
    IEnumerable<TValue> Values { get; }

    bool ContainsKey(TKey key);
    bool TryGetValue(TKey key, out TValue value);
    TValue this[TKey key] { get; }
  }

  public interface IFinDictionary<TKey, TValue>
      : IReadOnlyFinDictionary<TKey, TValue>,
        IFinCollection<(TKey Key, TValue Value)> {
    new TValue this[TKey key] { get; set; }
  }
}