namespace fin.data.lists {
  public interface IReadOnlyFinList<out T> : IReadOnlyFinCollection<T> {
    T this[int index] { get; }
  }

  public interface IFinList<T> : IReadOnlyFinList<T>, IFinCollection<T> {
    new T this[int index] { get; set; }
  }
}