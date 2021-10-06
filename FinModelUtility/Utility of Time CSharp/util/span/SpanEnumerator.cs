using System.Collections;
using System.Collections.Generic;

namespace UoT.util.span {
  public class SpanEnumerator<T> : IEnumerator<T> {
    private readonly ISpannable<T> data_;
    private int index_;

    public SpanEnumerator(ISpannable<T> data) {
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