using System;
using System.Collections.Generic;

namespace fin.data {
  public class LazyDictionary<TKey, TValue> {
    private readonly Dictionary<TKey, TValue> impl_ = new();
    private Func<TKey, TValue> handler_;

    public LazyDictionary(Func<TKey, TValue> handler) {
      this.handler_ = handler;
    }

    public void Clear() => this.impl_.Clear();

    public int Count => this.impl_.Count;

    public bool ContainsKey(TKey key) => this.impl_.ContainsKey(key);

    public TValue this[TKey key] => this.Get(key);

    public TValue Get(TKey key) {
      if (this.impl_.TryGetValue(key, out var value)) {
        return value;
      }

      value = this.handler_(key);
      this.impl_[key] = value;
      return value;
    }
  }
}
