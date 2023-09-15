using System.Collections.Generic;

namespace fin.data.queue {
  /// <summary>
  ///   Simpler interface for queues that is easier to implement.
  /// </summary>
  public interface IQueue<T> : IEnumerable<T> {
    int Count { get; }

    void Clear();

    void Enqueue(T first, params T[] rest);
    void Enqueue(IEnumerable<T> values);

    T Dequeue();
    bool TryDequeue(out T value);

    T Peek();
    bool TryPeek(out T value);
  }
}
