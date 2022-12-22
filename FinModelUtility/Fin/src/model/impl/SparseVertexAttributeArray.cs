using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fin.model.impl {
  public interface IVertexAttributeArray<T> : IEnumerable<(int, T)>
      where T : notnull {
    int Count { get; }

    T? Get(int index) => this[index];
    void Set(int index, T? value) => this[index] = value;

    T? this[int index] { get; set; }
  }

  public class SparseVertexAttributeArray<T> : IVertexAttributeArray<T>
      where T : notnull {
    private IList<T?>? impl_;

    public int Count => this.impl_?.Count(value => value != null) ?? 0;

    public T? this[int index] {
      get => index < (this.impl_?.Count ?? 0) ? this.impl_[index] : default;
      set {
        this.impl_ ??= new List<T?>();
        while (this.impl_?.Count <= index) {
          this.impl_.Add(default);
        }
        this.impl_![index] = value;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    public IEnumerator<(int, T)> GetEnumerator() {
      if (this.impl_ != null) {
        for (var i = 0; i < this.impl_.Count; ++i) {
          var value = this.impl_[i];
          if (value != null) {
            yield return (i, value);
          }
        }
      }
    }
  }
}