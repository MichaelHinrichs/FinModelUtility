using System.Collections.Generic;

namespace fin.data {
  public interface IReadOnlyFinCollection<out T> : IEnumerable<T> {
    int Count { get; }
  }

  public interface IFinCollection<out T> : IReadOnlyFinCollection<T> {
    void Clear();
  }
}