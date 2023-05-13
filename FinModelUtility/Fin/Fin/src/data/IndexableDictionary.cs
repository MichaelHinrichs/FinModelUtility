using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace fin.data {
  public interface IIndexable {
    int Index { get; }
  }

  public interface
      IReadOnlyIndexableDictionary<TIndexable, TValue> : IEnumerable<TValue>
      where TIndexable : IIndexable {
    int Length { get; }

    TValue this[int index] { get; }
    TValue this[TIndexable key] { get; }

    bool TryGetValue(int index, out TValue value);
    bool TryGetValue(TIndexable key, out TValue value);
  }

  public interface IIndexableDictionary<TIndexable, TValue> :
      IReadOnlyIndexableDictionary<TIndexable, TValue>
      where TIndexable : IIndexable {
    void Clear();
    new TValue this[int index] { get; set; }
    new TValue this[TIndexable key] { get; set; }
  }

  public class IndexableDictionary<TIndexable, TValue>
      : IIndexableDictionary<TIndexable, TValue> where TIndexable : IIndexable {
    private static readonly ArrayPool<bool> boolPool_ = ArrayPool<bool>.Shared;
    private static readonly ArrayPool<TValue> pool_ = ArrayPool<TValue>.Shared;

    private bool[] hasKeys_ = Array.Empty<bool>();
    private TValue[] impl_ = Array.Empty<TValue>();

    public IndexableDictionary() : this(0) { }

    public IndexableDictionary(int length) => this.ResizeLength_(length);

    public int Length { get; private set; }

    public void Clear() {
      for (var i = 0; i < this.Length; i++) {
        hasKeys_[i] = false;
        this.impl_[i] = default;
      }

      boolPool_.Return(this.hasKeys_);
      this.hasKeys_ = Array.Empty<bool>();

      pool_.Return(this.impl_);
      this.impl_ = Array.Empty<TValue>();

      this.Length = 0;
    }

    private void ResizeLength_(int newLength) {
      var oldCount = this.Length;
      if (oldCount < newLength) {
        this.Length = newLength;

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

    public TValue this[int index] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => impl_[index];
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set {
        ResizeLength_(Math.Max(this.Length, index + 1));

        this.impl_[index] = value;
        this.hasKeys_[index] = true;
      }
    }

    public TValue this[TIndexable key] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => this[key.Index];
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set => this[key.Index] = value;
    }

    public bool TryGetValue(int index, out TValue value) {
      if (index >= this.Length) {
        value = default!;
        return false;
      }

      value = this.impl_[index];
      return this.hasKeys_[index];
    }

    public bool TryGetValue(TIndexable key, out TValue value)
      => TryGetValue(key.Index, out value);


    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<TValue> GetEnumerator() {
      if (this.impl_ != null) {
        for (var i = 0; i < this.Length; i++) {
          if (this.hasKeys_[i]) {
            yield return this.impl_[i];
          }
        }
      }
    }
  }
}