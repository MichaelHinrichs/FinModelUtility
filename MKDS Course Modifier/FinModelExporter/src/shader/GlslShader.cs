using System;

namespace fin.shader {
  public interface IGlslShader {
    TValue In<TValue>(string paramName) where TValue : IGlslValue;
    TValue Out<TValue>(string paramName) where TValue : IGlslValue;
    TValue Uniform<TValue>(string paramName) where TValue : IGlslValue;

    IGlslAction Main();

    IGlslFunc<TOut> Method<TOut>(string methodName)
        where TOut : IGlslValue;

    /*IGlslFunc<TIn, TOut> Method<TIn, TOut>(
        string methodName,
        Func<TIn, TOut> impl)
        where TIn : IGlslValue
        where TOut : IGlslValue;*/

    IGlslFunc<TIn1, TIn2, TOut> Method<TIn1, TIn2, TOut>(
        string methodName,
        Func<TIn1, TIn2, TOut> impl)
        where TIn1 : IGlslValue
        where TIn2 : IGlslValue
        where TOut : IGlslValue;
  }

  public interface IGlslValue {}

  public interface IGlslVariable<TValue> where TValue : IGlslValue {
    IGlslValue Value { get; }
  }


  public interface IGlslArithmeticOperators<T> : IGlslPlusOperators<T>,
                                                 IGlslMinusOperators<T> {}

  public interface IGlslArithmeticOperators<out TLhs, in TRhs> :
      IGlslPlusOperators<TLhs, TRhs>,
      IGlslMinusOperators<TLhs, TRhs>,
      IGlslTimesOperators<TLhs, TRhs>,
      IGlslOverOperators<TLhs, TRhs>,
      IGlslPowOperators<TLhs, TRhs> {}


  public interface IGlslPlusOperators<T> : IGlslPlusOperators<T, T> {}

  public interface IGlslPlusOperators<out TLhs, in TRhs> {
    TLhs Plus(TRhs other);
    TLhs PlusEquals(TRhs other);
  }

  public interface IGlslMinusOperators<T> : IGlslMinusOperators<T, T> {}

  public interface IGlslMinusOperators<out TLhs, in TRhs> {
    TLhs Minus(TRhs other);
    TLhs MinusEquals(TRhs other);
  }

  public interface IGlslTimesOperators<T> : IGlslTimesOperators<T, T> {}

  public interface IGlslTimesOperators<out TLhs, in TRhs> {
    TLhs Times(TRhs other);
    TLhs TimesEquals(TRhs other);
  }

  public interface IGlslOverOperators<T> : IGlslOverOperators<T, T> {}

  public interface IGlslOverOperators<out TLhs, in TRhs> {
    TLhs Over(TRhs other);
    TLhs OverEquals(TRhs other);
  }

  public interface IGlslPowOperators<T> : IGlslPowOperators<T, T> {}

  public interface IGlslPowOperators<out TLhs, in TRhs> {
    TLhs Pow(TRhs other);
    TLhs PowEquals(TRhs other);
  }


  public interface IGlslXy {
    IGlslFloat X { get; }
    IGlslFloat Y { get; }
  }

  public interface IGlslXyz : IGlslXy {
    IGlslFloat Z { get; }
  }

  public interface IGlslXyzw : IGlslXyz {
    IGlslFloat W { get; }
  }

  // TODO: How to do swizzling?

  public interface IGlslVec2 :
      IGlslValue,
      IGlslXy,
      IGlslPlusOperators<IGlslVec2>,
      IGlslMinusOperators<IGlslVec2>,
      IGlslArithmeticOperators<IGlslVec2, IGlslFloat> {}

  public interface IGlslVec3 :
      IGlslValue,
      IGlslXyz,
      IGlslPlusOperators<IGlslVec3>,
      IGlslMinusOperators<IGlslVec3>,
      IGlslArithmeticOperators<IGlslVec3, IGlslFloat> {}

  public interface IGlslVec4 :
      IGlslValue,
      IGlslXyzw,
      IGlslPlusOperators<IGlslVec4>,
      IGlslMinusOperators<IGlslVec4>,
      IGlslArithmeticOperators<IGlslVec4, IGlslFloat> {}

  public interface IGlslFloat :
      IGlslValue,
      IGlslArithmeticOperators<IGlslFloat> {}
}