using System.Collections;
using System.Collections.Generic;

namespace fin.data {
  public class ListDictionary<TKey, TValue>
      : IEnumerable<KeyValuePair<TKey, IList<TValue>>> {
    private readonly Dictionary<TKey, IList<TValue>> impl_ = new();

    public int Count => this.impl_.Count;
    public void Clear() => this.impl_.Clear();

    public void Add(TKey key, TValue value) {
      if (!this.impl_.TryGetValue(key, out var list)) {
        this.impl_[key] = list = new List<TValue>();
      }

      list.Add(value);
    }

    public bool TryGetList(TKey key, out IList<TValue> list)
      => this.impl_.TryGetValue(key, out list);

    public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator()
      => this.impl_.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
  }
}