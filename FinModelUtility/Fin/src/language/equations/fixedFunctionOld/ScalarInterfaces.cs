using System.Collections.Generic;

namespace fin.language.equations.fixedFunctionOld {
  public interface IScalarNamedValue<TIdentifier> : INamedValue<TIdentifier>,
                                                    IScalarFactor {
    IScalarValue ScalarValue { get; }
  }

  public interface IScalarInput<TIdentifier> : IScalarNamedValue<TIdentifier> {
    IScalarConstant DefaultValue { get; }
    IScalarConstant? CustomValue { get; set; }
  }

  public interface IScalarOutput<TIdentifier>
      : IScalarNamedValue<TIdentifier> {}


  public interface IScalarValue {
    IScalarExpression Add(IScalarValue term1, params IScalarValue[] terms);
    IScalarExpression Subtract(IScalarValue term1, params IScalarValue[] terms);
    IScalarTerm Multiply(IScalarValue factor1, params IScalarValue[] factors);
    IScalarTerm Divide(IScalarValue factor1, params IScalarValue[] factors);
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