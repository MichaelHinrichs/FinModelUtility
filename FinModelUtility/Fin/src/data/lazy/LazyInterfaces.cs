using System.Threading.Tasks;

namespace fin.data.lazy {
  public interface ILazyDictionary<in TKey, TValue> {
    TValue this[TKey key] { get; set; }
  }

  public interface ILazyArray<T> : ILazyDictionary<int, T> { }


  public interface ILazyAsyncDictionary<in TKey, TValue>
      : ILazyDictionary<TKey, Task<TValue>> { }

  public interface ILazyAsyncArray<T> : ILazyAsyncDictionary<int, T> { }
}