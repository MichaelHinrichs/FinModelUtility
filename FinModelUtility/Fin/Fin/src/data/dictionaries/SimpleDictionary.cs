using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace fin.data.dictionaries {
  public class SimpleDictionary<TKey, TValue>(IDictionary<TKey, TValue>? impl = null)
      : IFinDictionary<TKey, TValue> {
    private readonly IDictionary<TKey, TValue> impl_ = impl ?? new ConcurrentDictionary<TKey, TValue>();

    public void Clear() => this.impl_.Clear();

    public int Count => this.impl_.Count;
    public IEnumerable<TKey> Keys => this.impl_.Keys;
    public IEnumerable<TValue> Values => this.impl_.Values;
    public bool ContainsKey(TKey key) => this.impl_.ContainsKey(key);

    public bool TryGetValue(TKey key, out TValue value) => this.impl_.TryGetValue(key, out value);

    public TValue this[TKey key] {
      get => this.impl_[key];
      set => this.impl_[key] = value;
    }
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<(TKey Key, TValue Value)> GetEnumerator() {
      foreach (var (key, value) in this.impl_) {
        yield return (key, value);
      }
    }

  }
}
