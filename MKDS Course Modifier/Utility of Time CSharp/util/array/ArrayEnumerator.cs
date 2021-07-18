using System.Collections;
using System.Collections.Generic;

namespace UoT.util.array {
  public class ArrayEnumerator<T> : IEnumerator<T> {
    private readonly T[] data_;
    private int index_;

    public ArrayEnumerator(T[] data) {
      this.data_ = data;
      this.Reset();
    }

    public void Dispose() { }

    public T Current => this.data_[this.index_];
    object IEnumerator.Current => this.Current!;

    public bool MoveNext() => ++this.index_ < this.data_.Length;
    public void Reset() => this.index_ = -1;
  }
}