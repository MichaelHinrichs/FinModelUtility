using System.Collections.Generic;

namespace fin.data.sets {
  /// <summary>
  ///   Simpler interface for readonly sets that is easier to implement.
  /// </summary>
  public interface IReadOnlyFinSet<T> : IEnumerable<T> {
    int Count { get; }
    bool Contains(T value);
  }

  /// <summary>
  ///   Simpler interface for sets that is easier to implement.
  /// </summary>
  public interface IFinSet<T> : IReadOnlyFinSet<T> {
    void Clear();

    bool Add(T value);
    bool Remove(T value);
  }
}