using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;


namespace fin.data {
  public class LazyAsyncDictionary<TKey, TValue> {
    private readonly ConcurrentDictionary<TKey, Task<TValue>> impl_ = new();

    private Func<TKey, Task<TValue>> handler_;

    public LazyAsyncDictionary(Func<TKey, Task<TValue>> handler) {
      this.handler_ = handler;
    }

    public LazyAsyncDictionary(Func<LazyAsyncDictionary<TKey, TValue>, TKey, Task<TValue>> handler) {
      this.handler_ = (TKey key) => handler(this, key);
    }

    public void Clear() => this.impl_.Clear();

    public int Count => this.impl_.Count;

    public bool ContainsKey(TKey key) => this.impl_.ContainsKey(key);

    public Task<TValue> this[TKey key] {
      get => this.GetAsync(key);
      set => this.impl_[key] = value;
    }

    public Task<TValue> GetAsync(TKey key) {
      if (this.impl_.TryGetValue(key, out var value)) {
        return value;
      }

      value = this.handler_(key);
      this.impl_[key] = value;
      return value;
    }
  }
}
