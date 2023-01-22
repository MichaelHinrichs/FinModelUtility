using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace fin.data {
  public interface IIndexable {
    int Index { get; }
  }

  public interface
      IReadOnlyIndexableDictionary<TIndexable, TValue> : IEnumerable<TValue>
      where TIndexable : IIndexable {
    TValue this[TIndexable key] { get; }
    bool TryGetValue(TIndexable key, out TValue value);
  }

  public interface IIndexableDictionary<TIndexable, TValue> :
      IReadOnlyIndexableDictionary<TIndexable, TValue>
      where TIndexable : IIndexable {
    void Clear();
    new TValue this[TIndexable key] { get; set; }
  }

  public class IndexableDictionary<TIndexable, TValue>
      : IIndexableDictionary<TIndexable, TValue> where TIndexable : IIndexable {
    private static readonly ArrayPool<bool> boolPool_ = ArrayPool<bool>.Shared;
    private static readonly ArrayPool<TValue> pool_ = ArrayPool<TValue>.Shared;

    private bool[] hasKeys_ = Array.Empty<bool>();
    private TValue[] impl_ = Array.Empty<TValue>();

    private int length_;

    public IndexableDictionary() : this(0) { }

    public IndexableDictionary(int length) => this.ResizeLength_(length);

    public void Clear() {
      for (var i = 0; i < this.length_; i++) {
        hasKeys_[i] = false;
        this.impl_[i] = default;
      }

      boolPool_.Return(this.hasKeys_);
      this.hasKeys_ = Array.Empty<bool>();

      pool_.Return(this.impl_);
      this.impl_ = Array.Empty<TValue>();

      this.length_ = 0;
    }

    private void ResizeLength_(int newLength) {
      var oldCount = this.length_;
      if (oldCount < newLength) {
        this.length_ = newLength;

        {
          var oldImpl = this.hasKeys_;
          this.hasKeys_ = boolPool_.Rent(newLength);

          for (var i = 0; i < oldCount; i++) {
            this.hasKeys_[i] = oldImpl[i];
          }

          for (var i = oldCount; i < this.hasKeys_.Length; i++) {
            this.hasKeys_[i] = false;
          }

          if (oldImpl != null) {
            boolPool_.Return(oldImpl);
          }
        }

        {
          var oldImpl = this.impl_;
          this.impl_ = pool_.Rent(newLength);

          for (var i = 0; i < oldCount; i++) {
            this.impl_[i] = oldImpl[i];
          }

          for (var i = oldCount; i < this.impl_.Length; i++) {
            this.impl_[i] = default;
          }

          if (oldImpl != null) {
            pool_.Return(oldImpl);
          }
        }
      } else if (oldCount > newLength) {
        throw new NotSupportedException();
      }
    }

    public TValue this[TIndexable key] {
      get => impl_[key.Index];
      set {
        var id = key.Index;
        ResizeLength_(Math.Max(this.length_, id + 1));

        this.impl_[id] = value;
        this.hasKeys_[id] = true;
      }
    }

    public bool TryGetValue(TIndexable key, out TValue value) {
      var index = key.Index;

      if (index >= this.length_) {
        value = default!;
        return false;
      }

      value = this.impl_[index];
      return this.hasKeys_[index];
    }


    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<TValue> GetEnumerator() {
      if (this.impl_ != null) {
        for (var i = 0; i < this.length_; i++) {
          if (this.hasKeys_[i]) {
            yield return this.impl_[i];
          }
        }
      }
    }
  }
}