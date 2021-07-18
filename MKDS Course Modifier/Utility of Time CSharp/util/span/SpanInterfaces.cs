using System.Collections.Generic;

namespace UoT.util.span {
  public interface ISpannable<T> : IEnumerable<T> {
    int Length { get; }

    T this[int localOffset] { get; set; }

    ISpan<T> Sub(int localOffset, int length);
  }

  public interface ISpan<T> : ISpannable<T> {
    int Offset { get; }
  }

  public interface ISpannableArray<T> : ISpannable<T> {}
}