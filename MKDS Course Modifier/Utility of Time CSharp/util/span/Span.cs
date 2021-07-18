using System.Collections;
using System.Collections.Generic;

namespace UoT.util.span {
  public partial class SpannableArray<T> : ISpannableArray<T> {
    private class Span : ISpan<T> {
      private readonly ISpannable<T> parent_;

      public Span(ISpannable<T> parent, int offset, int length) {
        Asserts.Assert(offset >= 0, "Spanned with a negative offset!");
        Asserts.Assert(length >= 0, "Spanned with a negative length!");
        Asserts.Assert(offset + length <= parent.Length,
                       "Spanned beyond the length of the parent!");

        this.parent_ = parent;
        this.Offset = offset;
        this.Length = length;
      }

      public ISpan<T> Sub(int offset, int length)
        => this.parent_.Sub(this.Offset + offset, length);

      public int Offset { get; }
      public int Length { get; }

      public T this[int localOffset] {
        get {
          Asserts.Assert(localOffset >= 0, "Accessed a negative offset!");
          Asserts.Assert(localOffset < this.Length,
                         "Accessed past the bounds of the span!");
          return this.parent_![this.Offset + localOffset];
        }
        set {
          Asserts.Assert(localOffset >= 0, "Accessed a negative offset!");
          Asserts.Assert(localOffset < this.Length,
                         "Accessed past the bounds of the span!");
          this.parent_![this.Offset + localOffset] = value;
        }
      }

      public IEnumerator<T> GetEnumerator() => new SpanEnumerator<T>(this);

      IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
  }
}