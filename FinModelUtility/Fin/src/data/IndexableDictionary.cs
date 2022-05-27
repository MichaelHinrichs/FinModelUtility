using System.Collections.Generic;

using fin.util.optional;


namespace fin.data {
  public interface IIndexable {
    int Index { get; }
  }

  public interface IIndexableDictionaryValue<TIndexable, TValue>
      where TIndexable : IIndexable {
    TIndexable Key { get; }
    Optional<TValue> Value { get; }
  }

  public interface IIndexableDictionary<TIndexable, TValue>
      where TIndexable : IIndexable {
    TValue this[TIndexable key] { get; set; }
    bool TryGet(TIndexable key, out TValue value);
  }

  public class
      IndexableDictionary<TIndexable, TValue> : IIndexableDictionary<TIndexable,
          TValue> where TIndexable : IIndexable {
    private readonly List<IndexableDictionaryValue> impl_ = new();

    public TValue this[TIndexable key] {
      get => this.impl_[key.Index].Value.Assert();
      set {
        var id = key.Index;

        while (this.impl_.Count < id + 1) {
          this.impl_.Add(new IndexableDictionaryValue {
              Key = default,
              Value = Optional<TValue>.None(),
          });
        }

        var current = this.impl_[key.Index];
        current.Key = key;
        current.Value = Optional<TValue>.Of(value);
      }
    }

    public bool TryGet(TIndexable key, out TValue value) {
      var index = key.Index;

      if (this.impl_.Count < index + 1) {
        value = default;
        return false;
      }

      var indexableDictionaryValue = this.impl_[index];
      return indexableDictionaryValue.Value.Try(out value);
    }

    private class
        IndexableDictionaryValue : IIndexableDictionaryValue<TIndexable,
            TValue> {
      public TIndexable Key { get; set; }
      public Optional<TValue> Value { get; set; }
    }
  }
}