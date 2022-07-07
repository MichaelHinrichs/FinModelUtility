using System.Collections;
using System.Collections.Generic;


namespace fin.data {
  public class ListDictionary<TKey, TValue>
      : IEnumerable<KeyValuePair<TKey, IList<TValue>>> {
    private readonly Dictionary<TKey, IList<TValue>> impl_ = new();
    private readonly List<TValue> nullImpl_ = new();

    public void Clear() {
      this.impl_.Clear();
      this.nullImpl_.Clear();
    }

    public void Add(TKey key, TValue value) {
      IList<TValue> list;
      if (key == null) {
        list = nullImpl_;
      } else if (!this.impl_.TryGetValue(key, out list)) {
        this.impl_[key] = list = new List<TValue>();
      }

      list.Add(value);
    }

    public bool TryGetList(TKey key, out IList<TValue>? list) {
      if (key == null) {
        if (this.nullImpl_.Count > 0) {
          list = this.nullImpl_;
          return true;
        } else {
          list = null;
          return false;
        }
      }

      return this.impl_.TryGetValue(key, out list);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator() {
      foreach (var value in this.impl_) {
        yield return value;
      }

      if (nullImpl_.Count > 0) {
        yield return new(default!, this.nullImpl_);
      }
    }
  }
}