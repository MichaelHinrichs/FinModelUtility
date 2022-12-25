using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System;
using System.Buffers;


namespace fin.data {
  public interface IIndexable {
    int Index { get; }
  }

  public interface
      IReadOnlyIndexableDictionary<TIndexable, TValue> : IEnumerable<(TIndexable
          , TValue)>
      where TIndexable : IIndexable {
    TValue this[TIndexable key] { get; }
    bool TryGetValue(TIndexable key, out TValue value);
  }

  public interface
      IIndexableDictionary<TIndexable, TValue> : IReadOnlyIndexableDictionary<
          TIndexable, TValue>
      where TIndexable : IIndexable {
    void Clear();
    new TValue this[TIndexable key] { get; set; }
  }

  public class IndexableDictionary<TIndexable, TValue>
      : IIndexableDictionary<TIndexable, TValue> where TIndexable : IIndexable {
    private static readonly ArrayPool<IndexableDictionaryValue> pool_
      = ArrayPool<IndexableDictionaryValue>.Shared;

    private IndexableDictionaryValue[] impl_ = Array.Empty<IndexableDictionaryValue>();
    private int length_;

    public IndexableDictionary() : this(0) { }

    public IndexableDictionary(int length) => this.ResizeLength_(length);

    public void Clear() {
      pool_.Return(this.impl_);
      this.impl_ = Array.Empty<IndexableDictionaryValue>();
      this.length_ = 0;
    }

    private void ResizeLength_(int newLength) {
      var oldCount = this.length_;
      if (oldCount < newLength) {
        this.length_ = newLength;
        var oldImpl = this.impl_;
        this.impl_ = pool_.Rent(newLength);

        for (var i = 0; i < oldCount; i++) {
          this.impl_[i] = oldImpl[i];
        }

        for (var i = oldCount; i < newLength; ++i) {
          this.impl_[i] = new IndexableDictionaryValue();
        }

        if (oldImpl != null) {
          pool_.Return(oldImpl);
        }
      } else if (oldCount > newLength) {
        throw new NotSupportedException();
      }
    }

    public TValue this[TIndexable key] {
      get => impl_[key.Index].Value!;
      set {
        var id = key.Index;
        ResizeLength_(Math.Max(this.length_, id + 1));

        var current = this.impl_[key.Index];
        current.HasValue = true;
        current.Key = key;
        current.Value = value;
      }
    }

    public bool TryGetValue(TIndexable key, out TValue value) {
      var index = key.Index;

      if (index >= this.length_) {
        value = default!;
        return false;
      }

      var indexableDictionaryValue = this.impl_[index];
      value = indexableDictionaryValue.Value;
      return indexableDictionaryValue.HasValue;
    }

    private class IndexableDictionaryValue {
      public bool HasValue;
      public TIndexable Key;
      public TValue? Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<(TIndexable, TValue)> GetEnumerator() {
      if (this.impl_ != null) {
        foreach (var node in this.impl_
                   .Take(this.length_)
                   .Where(value => value.HasValue)
                   .Select(value => (value.Key, value.Value))) {
          yield return node;
        }
      }
    }
  }
}