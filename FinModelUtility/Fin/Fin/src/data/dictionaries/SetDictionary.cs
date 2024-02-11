using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fin.data.dictionaries {
  /// <summary>
  ///   An implementation for a dictionary of sets. Each value added for a key
  ///   will be stored in that key's corresponding set.
  /// </summary>
  public class SetDictionary<TKey, TValue>(
      IFinDictionary<TKey, ISet<TValue>> impl)
      : IFinCollection<(TKey Key, ISet<TValue> Value)> {
    public SetDictionary() : this(
        new NullFriendlyDictionary<TKey, ISet<TValue>>()) { }

    public void Clear() => impl.Clear();

    public int Count => impl.Values.Select(list => list.Count).Sum();

    public void Add(TKey key, TValue value) {
      if (!impl.TryGetValue(key, out var set)) {
        impl[key] = set = new HashSet<TValue>();
      }

      set.Add(value);
    }

    public ISet<TValue> this[TKey key] => impl[key];

    public bool TryGetSet(TKey key, out ISet<TValue>? set)
      => impl.TryGetValue(key, out set);

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public IEnumerator<(TKey Key, ISet<TValue> Value)> GetEnumerator()
      => impl.GetEnumerator();
  }
}