namespace fin.data.lazy {
  public interface ILazyDictionary<in TKey, TValue> {
    int Count { get; }
    void Clear();
    bool ContainsKey(TKey key);
    TValue this[TKey key] { get; set; }
  }

  public interface ILazyArray<T> : ILazyDictionary<int, T> { }
}