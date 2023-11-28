using System;
using System.Collections;
using System.Collections.Generic;

using fin.data.dictionaries;

namespace fin.data.lazy {
  /// <summary>
  ///   Dictionary implementation that lazily populates its entries when
  ///   accessed.
  /// </summary>
  public class LazyDictionary<TKey, TValue> : ILazyDictionary<TKey, TValue> {
    private readonly NullFriendlyDictionary<TKey, TValue> impl_ = new();
    private readonly Func<TKey, TValue> handler_;

    public LazyDictionary(Func<TKey, TValue> handler) {
      this.handler_ = handler;
    }

    public LazyDictionary(
        Func<LazyDictionary<TKey, TValue>, TKey, TValue> handler) {
      this.handler_ = (TKey key) => handler(this, key);
    }

    public int Count => this.impl_.Count;
    public void Clear() => this.impl_.Clear();
    public bool ContainsKey(TKey key) => this.impl_.ContainsKey(key);

    public bool TryGetValue(TKey key, out TValue value)
      => this.impl_.TryGetValue(key, out value);

    public TValue this[TKey key] {
      get => this.impl_.TryGetValue(key, out var value)
          ? value
          : this.impl_[key] = this.handler_(key);
      set => this.impl_[key] = value;
    }

    public IEnumerable<TKey> Keys => this.impl_.Keys;
    public IEnumerable<TValue> Values => this.impl_.Values;

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<(TKey Key, TValue Value)> GetEnumerator()
      => this.impl_.GetEnumerator();
  }
}