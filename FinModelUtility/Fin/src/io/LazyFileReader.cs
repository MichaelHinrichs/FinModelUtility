using System;
using System.Collections.Generic;
using System.Linq;


namespace fin.io {
  public interface ILazyFileReader {
    string Name { get; }
  }

  public interface ILazyFileReader<out T> : ILazyFileReader {
    T Read();
  }


  public class FuncLazyFileReader<T> : ILazyFileReader<T> {
    private readonly Func<T> callback_;

    public FuncLazyFileReader(string name, Func<T> callback) {
      this.Name = name;
      this.callback_ = callback;
    }

    public string Name { get; }

    public T Read() => this.callback_();
  }


  public interface ILazyFileReaderMap {
    void Add<T>(ILazyFileReader<T> reader);

    bool TryGet<T>(out IEnumerable<ILazyFileReader<T>>? typedFileReaders);
  }

  public class LazyFileReaderMap : ILazyFileReaderMap {
    private IDictionary<Type, IList<ILazyFileReader>> impl_;

    public void Add<T>(ILazyFileReader<T> reader) {
      var type = typeof(T);
      if (!this.impl_.TryGetValue(type, out var fileReaders)) {
        fileReaders = new List<ILazyFileReader>();
        this.impl_[type] = fileReaders;
      }

      fileReaders.Add(reader);
    }

    public bool
        TryGet<T>(out IEnumerable<ILazyFileReader<T>>? typedFileReaders) {
      var type = typeof(T);
      if (!this.impl_.TryGetValue(type, out var fileReaders)) {
        typedFileReaders = null;
        return false;
      }

      typedFileReaders = fileReaders.Cast<ILazyFileReader<T>>();
      return true;
    }
  }
}