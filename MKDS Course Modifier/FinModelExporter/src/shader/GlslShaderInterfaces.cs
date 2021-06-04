using System;
using System.Collections.Generic;

namespace fin.shader {
  public interface IGlslShader {
    IReadOnlyList<IGlslVariable> Ins { get; }
    TValue In<TValue>(string paramName) where TValue : IGlslValue;

    IReadOnlyList<IGlslVariable> Outs { get; }
    TValue Out<TValue>(string paramName) where TValue : IGlslValue;

    IReadOnlyList<IGlslVariable> Uniforms { get; }
    TValue Uniform<TValue>(string paramName) where TValue : IGlslValue;

    // TODO: Support structs?

    IReadOnlyList<IGlslMethod> Methods { get; }
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

  public enum GlslValueType {
    BOOL,
    INT,
    UINT,
    FLOAT,
    DOUBLE,

    BVEC2,
    BVEC3,
    BVEC4,

    IVEC2,
    IVEC3,
    IVEC4,

    UVEC2,
    UVEC3,
    UVEC4,

    VEC2,
    VEC3,
    VEC4,

    DVEC2,
    DVEC3,
    DVEC4,

    MAT2,
    MAT2X3,
    MAT2X4,

    MAT3x2,
    MAT3,
    MAT3x4,

    MAT4X2,
    MAT4X3,
    MAT4,

    SAMPLER2D,
  }


  public interface IGlslValue {
    GlslValueType Type { get; }
  }


  public interface IGlslVariable {
    string Name { get; }
    IGlslValue UntypedValue { get; }
  }

  public interface IGlslVariable<TValue> : IGlslVariable where TValue : IGlslValue {
    TValue Value { get; }
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