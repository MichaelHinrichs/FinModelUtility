using System;
using System.Collections.Generic;
using System.Linq;


namespace fin.io {
  public interface ILazy {
    string Name { get; }
  }

  public interface ILazy<out T> : ILazy {
    T Get();
  }


  public class FuncLazy<T> : ILazy<T> {
    private readonly Func<T> callback_;

    public FuncLazy(string name, Func<T> callback) {
      this.Name = name;
      this.callback_ = callback;
    }

    public string Name { get; }

    public T Get() => this.callback_();
  }


  public interface ILazyMap {
    void Add<T>(ILazy<T> lazy);

    bool TryGet<T>(out IEnumerable<ILazy<T>>? typedLazies);
  }

  public class LazyMap : ILazyMap {
    private IDictionary<Type, IList<ILazy>> impl_;

    public void Add<T>(ILazy<T> reader) {
      var type = typeof(T);
      if (!this.impl_.TryGetValue(type, out var fileReaders)) {
        fileReaders = new List<ILazy>();
        this.impl_[type] = fileReaders;
      }

      fileReaders.Add(reader);
    }

    public bool
        TryGet<T>(out IEnumerable<ILazy<T>>? typedLazies) {
      var type = typeof(T);
      if (!this.impl_.TryGetValue(type, out var fileReaders)) {
        typedLazies = null;
        return false;
      }

      typedLazies = fileReaders.Cast<ILazy<T>>();
      return true;
    }
  }
}