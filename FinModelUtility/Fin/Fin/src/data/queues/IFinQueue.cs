using System.Collections.Generic;

namespace fin.data.queues {
  public interface IReadOnlyFinQueue<out T> : IReadOnlyFinCollection<T> { }

  /// <summary>
  ///   Simpler interface for queues that is easier to implement.
  /// </summary>
  public interface IFinQueue<T> : IReadOnlyFinQueue<T>, IFinCollection<T> {
    void Enqueue(T first, params T[] rest);
    void Enqueue(IEnumerable<T> values);

    T Dequeue();
    bool TryDequeue(out T value);

    T Peek();
    bool TryPeek(out T value);
  }
}