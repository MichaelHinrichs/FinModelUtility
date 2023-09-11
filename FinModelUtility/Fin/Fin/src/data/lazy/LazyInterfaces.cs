using System.Collections.Generic;

namespace fin.data.lazy {
  public interface ILazyDictionary<TKey, TValue> {
    int Count { get; }
    void Clear();
    bool ContainsKey(TKey key);
    TValue this[TKey key] { get; set; }

    IEnumerable<TKey> Keys { get; }
    IEnumerable<TValue> Values { get; }
  }

  public interface ILazyArray<T> : ILazyDictionary<int, T> { }
}