using System.Collections.Generic;

namespace fin.language.equations.fixedFunction {
  public interface IScalarNamedValue : INamedValue, IScalarFactor {
    IScalarValue ScalarValue { get; }
  }

  public interface IScalarIdentifiedValue<TIdentifier> : IIdentifiedValue<TIdentifier>,
                                                    IScalarFactor {
    IScalarValue ScalarValue { get; }
  }

  public interface IScalarInput<TIdentifier> : IScalarIdentifiedValue<TIdentifier> {
    IScalarConstant DefaultValue { get; }
    IScalarConstant? CustomValue { get; set; }
  }

  public interface IScalarOutput<TIdentifier>
      : IScalarIdentifiedValue<TIdentifier> {}


  public interface IScalarValue {
    IScalarExpression Add(IScalarValue term1, params IScalarValue[] terms);
    IScalarExpression Subtract(IScalarValue term1, params IScalarValue[] terms);
    IScalarTerm Multiply(IScalarValue factor1, params IScalarValue[] factors);
    IScalarTerm Divide(IScalarValue factor1, params IScalarValue[] factors);

    bool Clamp { get; set; }

    IColorValueTernaryOperator TernaryOperator(
        BoolComparisonType comparisonType,
        IScalarValue other,
        IColorValue trueValue,
        IColorValue falseValue);
  }

  public interface IScalarExpression : IScalarValue {
    IReadOnlyList<IScalarValue> Terms { get; }
  }

  public interface IScalarTerm : IScalarValue {
    IReadOnlyList<IScalarValue> NumeratorFactors { get; }
    IReadOnlyList<IScalarValue>? DenominatorFactors { get; }
  }

  public interface IScalarFactor : IScalarValue {}

  public interface IScalarConstant : IScalarFactor {
    double Value { get; }
  }
}