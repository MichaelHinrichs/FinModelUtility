using System.Collections;
using System.Collections.Generic;

using fin.data.sets;

namespace fin.data.indexable {
  public interface IReadOnlyIndexableSet<TIndexable> : IFinSet<TIndexable>
      where TIndexable : IIndexable {
    bool Contains(int index);
    TIndexable this[int index] { get; }
    bool TryGetValue(int index, out TIndexable value);
  }

  public interface IIndexableSet<TIndexable> :
      IReadOnlyIndexableSet<TIndexable>,
      IFinSet<TIndexable>
      where TIndexable : IIndexable { }
}