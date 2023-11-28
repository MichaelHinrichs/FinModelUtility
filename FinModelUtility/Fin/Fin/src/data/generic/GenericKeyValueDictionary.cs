namespace fin.data.generic {
  public interface IGenericKey<T> { }

  public interface IGenericValue<T> { }

  public interface IGenericKeyValueDictionary<T, TKey, TValue>
      where TKey : IGenericKey<T>
      where TValue : IGenericValue<T> {

  }
}