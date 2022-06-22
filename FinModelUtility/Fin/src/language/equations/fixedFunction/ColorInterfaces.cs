using System.Collections.Generic;

namespace fin.language.equations.fixedFunction {
  public interface IColorNamedValue<TIdentifier> : INamedValue<TIdentifier>,
                                                   IColorFactor {
    IColorValue ColorValue { get; }
  }

  public interface IColorInput<TIdentifier> : IColorNamedValue<TIdentifier> {
    IColorConstant DefaultValue { get; }
    IColorConstant? CustomValue { get; set; }
  }

  public interface IColorOutput<TIdentifier> : IColorNamedValue<TIdentifier> {}

  public enum ColorSwizzle {
    R,
    G,
    B,
  }

  public interface IColorNamedValueSwizzle<TIdentifier> : IScalarFactor {
    IColorNamedValue<TIdentifier> Source { get; }
    ColorSwizzle SwizzleType { get; }
  }


  public interface IColorValue {
    IColorExpression Add(IColorValue term1, params IColorValue[] terms);
    IColorExpression Subtract(IColorValue term1, params IColorValue[] terms);
    IColorTerm Multiply(IColorValue factor1, params IColorValue[] factors);
    IColorTerm Divide(IColorValue factor1, params IColorValue[] factors);

    IColorExpression Add(IScalarValue term1, params IScalarValue[] terms);
    IColorExpression Subtract(IScalarValue term1, params IScalarValue[] terms);
    IColorTerm Multiply(IScalarValue factor1, params IScalarValue[] factors);
    IColorTerm Divide(IScalarValue factor1, params IScalarValue[] factors);

    IScalarValue? Intensity { get; }
    IScalarValue R { get; }
    IScalarValue G { get; }
    IScalarValue B { get; }

    bool Clamp { get; set; }
  }

  public interface IColorExpression : IColorValue {
    IReadOnlyList<IColorValue> Terms { get; }
  }

  public interface IColorTerm : IColorValue {
    IReadOnlyList<IColorValue> NumeratorFactors { get; }
    IReadOnlyList<IColorValue>? DenominatorFactors { get; }
  }

  public interface IColorFactor : IColorValue {}

  public interface IColorConstant : IColorFactor {
    double? IntensityValue { get; }
    double RValue { get; }
    double GValue { get; }
    double BValue { get; }
  }
}