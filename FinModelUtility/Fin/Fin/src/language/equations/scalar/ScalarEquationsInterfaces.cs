using System.Collections.Generic;

namespace fin.language.equations.scalar {
  public interface IScalarEquations<TIdentifier> {
    IScalarConstant CreateScalarConstant(double v);

    IReadOnlyDictionary<TIdentifier, IScalarInput<TIdentifier>> Inputs { get; }

    IScalarInput<TIdentifier> CreateScalarInput(
        TIdentifier identifier,
        IScalarConstant defaultValue);

    IReadOnlyDictionary<TIdentifier, IScalarOutput<TIdentifier>> Outputs {
      get;
    }

    IScalarOutput<TIdentifier> CreateScalarOutput(
        TIdentifier identifier,
        IScalarValue value);
  }

  public interface INamedValue<TIdentifier> : IScalarFactor {
    TIdentifier Identifier { get; }
    IScalarValue Value { get; }
  }


  public interface IScalarInput<TIdentifier> : INamedValue<TIdentifier> {
    IScalarConstant DefaultValue { get; }
    IScalarConstant? CustomValue { get; set; }
  }

  public interface IScalarOutput<TIdentifier> : INamedValue<TIdentifier> {}


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