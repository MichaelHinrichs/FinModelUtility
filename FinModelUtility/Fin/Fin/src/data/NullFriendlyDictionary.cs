using schema.binary.util;

using System.Collections;
using System.Collections.Generic;


namespace fin.data {
  public class NullFriendlyDictionary<TKey, TValue>
      : IEnumerable<KeyValuePair<TKey, TValue>> {
    private readonly Dictionary<TKey, TValue> impl_ = new();

    private bool hasNull_;
    private TValue nullValue_;

    public void Clear() {
      this.impl_.Clear();
      this.hasNull_ = false;
    }

    public IEnumerable<TKey?> Keys {
      get {
        foreach (var key in this.impl_.Keys) {
          yield return key;
        }
        if (this.hasNull_) {
          yield return default;
        }
      }
    }

    public IEnumerable<TValue> Values {
      get {
        foreach (var value in this.impl_.Values) {
          yield return value;
        }
        if (this.hasNull_) {
          yield return this.nullValue_;
        }
      }
    }

    public bool ContainsKey(TKey key)
      => key == null ? this.hasNull_ : this.impl_.ContainsKey(key);

    public void Add(TKey key, TValue value) {
      if (key == null) {
        this.hasNull_ = true;
        this.nullValue_ = value;
      } else {
        this.impl_[key] = value;
      }
    }

    public TValue this[TKey key] {
      get {
        if (!this.TryGetValue(key, out var value)) {
          Asserts.Fail($"Expected to find key {key} in dictionary!");
        }
        return value!;
      }
      set => this.Add(key, value);
    }

    public bool TryGetValue(TKey key, out TValue? value) {
      if (key == null) {
        if (this.hasNull_) {
          value = this.nullValue_;
          return true;
        } else {
          value = default;
          return false;
        }
      }

      return this.impl_.TryGetValue(key, out value);
    }

    public bool Remove(TKey key) => this.Remove(key, out _);

    public bool Remove(TKey key, out TValue value) {
      bool didRemove;
      if (key == null) {
        didRemove = this.hasNull_;
        value = this.nullValue_;
        this.hasNull_ = false;
      } else {
        didRemove = this.impl_.Remove(key, out value);
      }
      return didRemove;
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      foreach (var value in this.impl_) {
        yield return value;
      }

      if (this.hasNull_) {
        yield return new(default!, this.nullValue_);
      }
    }
  }
}