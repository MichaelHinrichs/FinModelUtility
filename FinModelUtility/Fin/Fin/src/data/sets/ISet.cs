using System.Collections.Generic;

namespace fin.data.sets {
  public interface ISet<T> : IEnumerable<T> {
    int Count { get; }

    void Clear();

    bool Add(T value);
    bool Remove(T value);
  }
}
