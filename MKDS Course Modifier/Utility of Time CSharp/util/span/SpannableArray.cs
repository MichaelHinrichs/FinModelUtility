using System.Collections;
using System.Collections.Generic;

using UoT.util.array;

namespace UoT.util.span {
  public partial class SpannableArray<T> : ISpannableArray<T> {
    private readonly T[] impl_;

    public SpannableArray(T[] impl) => this.impl_ = impl;

    public int Length => this.impl_.Length;

    public T this[int localOffset] {
      get => this.impl_[localOffset];
      set => this.impl_[localOffset] = value;
    }

    public ISpan<T> Sub(int localOffset, int length)
      => new Span(this, localOffset, length);

    public IEnumerator<T> GetEnumerator() => new ArrayEnumerator<T>(this.impl_);
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
  }
}