using System.Collections;
using System.Collections.Generic;

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
    private T? first_;
    private T? second_;

    // TODO: Look into if an array is better here.
    private Dictionary<int, T> extras_ = new();

    public int Count {
      get {
        var total = 0;

        if (this.first_ != null) {
          ++total;
        }
        if (this.second_ != null) {
          ++total;
        }
        total += this.extras_.Count;

        return total;
      }
    }

    public T? this[int index] {
      get => index switch {
          0 => this.first_,
          1 => this.second_,
          _ => this.extras_.TryGetValue(index - 2, out var value)
                   ? value
                   : default,
      };
      set {
        switch (index) {
          case 0: {
            this.first_ = value;
            break;
          }
          case 1: {
            this.second_ = value;
            break;
          }
          default: {
            index -= 2;
            if (value == null) {
              this.extras_.Remove(index);
            } else {
              this.extras_[index] = value;
            }
            break;
          }
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    public IEnumerator<(int, T)> GetEnumerator() {
      if (this.first_ != null) {
        yield return (0, this.first_);
      }
      if (this.second_ != null) {
        yield return (1, this.second_);
      }
      foreach (var item in this.extras_) {
        yield return (item.Key, item.Value);
      }
    }
  }
}