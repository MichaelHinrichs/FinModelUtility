namespace fin.data.stacks {
  public interface IReadOnlyFinStack<out T> : IReadOnlyFinCollection<T> {
    T Top { get; }
  }

  /// <summary>
  ///   Simpler interface for stacks that is easier to implement.
  /// </summary>
  public interface IFinStack<T> : IReadOnlyFinStack<T>, IFinCollection<T> {
    new T Top { get; set; }

    bool TryPop(out T item);
    T Pop();
    void Push(T item);
  }
}