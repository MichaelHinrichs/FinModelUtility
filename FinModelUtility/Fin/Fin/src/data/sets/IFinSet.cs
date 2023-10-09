namespace fin.data.sets {
  /// <summary>
  ///   Simpler interface for readonly sets that is easier to implement.
  /// </summary>
  public interface IReadOnlyFinSet<T> : IReadOnlyFinCollection<T> {
    bool Contains(T value);
  }

  /// <summary>
  ///   Simpler interface for sets that is easier to implement.
  /// </summary>
  public interface IFinSet<T> : IReadOnlyFinSet<T>, IFinCollection<T> {
    bool Add(T value);
    bool Remove(T value);
  }
}