using System.Collections.Generic;

namespace fin.language.equations.fixedFunction {
  public interface IFixedFunctionEquations<TIdentifier> {
    IScalarConstant CreateScalarConstant(double v);

    IColorConstant CreateColorConstant(
        double r,
        double g,
        double b);

    IColorConstant CreateColorConstant(
        double intensity);

    IColorFactor CreateColor(IScalarValue r,
                             IScalarValue g,
                             IScalarValue b);

    IColorFactor CreateColor(IScalarValue intensity);


    IReadOnlyDictionary<TIdentifier, IScalarInput<TIdentifier>>
        ScalarInputs { get; }

    IScalarInput<TIdentifier> CreateScalarInput(
        TIdentifier identifier,
        IScalarConstant defaultValue);

    IScalarInput<TIdentifier> CreateOrGetScalarInput(
        TIdentifier identifier,
        IScalarConstant defaultValue);

    IScalarInput<TIdentifier> CreateOrGetScalarInput(
        TIdentifier identifier);


    IReadOnlyDictionary<TIdentifier, IScalarOutput<TIdentifier>>
        ScalarOutputs { get; }

    IScalarOutput<TIdentifier> CreateScalarOutput(
        TIdentifier identifier,
        IScalarValue value);


    IReadOnlyDictionary<TIdentifier, IColorInput<TIdentifier>>
        ColorInputs { get; }

    IColorInput<TIdentifier> CreateColorInput(
        TIdentifier identifier,
        IColorConstant value);

    IColorInput<TIdentifier> CreateOrGetColorInput(
        TIdentifier identifier,
        IColorConstant defaultValue);

    IColorInput<TIdentifier> CreateOrGetColorInput(
        TIdentifier identifier);


    IReadOnlyDictionary<TIdentifier, IColorOutput<TIdentifier>>
        ColorOutputs { get; }

    IColorOutput<TIdentifier> CreateColorOutput(
        TIdentifier identifier,
        IColorValue value);

    bool HasInput(TIdentifier identifier);
    bool DoOutputsDependOn(TIdentifier[] outputIdentifiers, IValue value);
  }

  public interface IIdentifiedValue<TIdentifier> : IValue {
    TIdentifier Identifier { get; }
  }

  public interface INamedValue : IValue {
    string Name { get; }
  }

  // Simple 
  public interface IValue { }

  public interface ITerm : IValue { }

  public interface IExpression : IValue { }


  // Typed
  public interface IValue<in TValue, out TTerm, out TExpression>
      : IValue
      where TValue : IValue<TValue, TTerm, TExpression>
      where TTerm : ITerm<TValue, TTerm, TExpression>
      where TExpression : IExpression<TValue, TTerm, TExpression> {
    TExpression Add(TValue term1, params TValue[] terms);
    TExpression Subtract(TValue term1, params TValue[] terms);
    TTerm Multiply(TValue factor1, params TValue[] factors);
    TTerm Divide(TValue factor1, params TValue[] factors);

    TExpression Add(IScalarValue term1, params IScalarValue[] terms);
    TExpression Subtract(IScalarValue term1, params IScalarValue[] terms);
    TTerm Multiply(IScalarValue factor1, params IScalarValue[] factors);
    TTerm Divide(IScalarValue factor1, params IScalarValue[] factors);
  }

  public interface ITerm<TValue, out TTerm, out TExpression>
      : ITerm, IValue<TValue, TTerm, TExpression>
      where TValue : IValue<TValue, TTerm, TExpression>
      where TTerm : ITerm<TValue, TTerm, TExpression>
      where TExpression : IExpression<TValue, TTerm, TExpression> {
    IReadOnlyList<TValue> NumeratorFactors { get; }
    IReadOnlyList<TValue>? DenominatorFactors { get; }
  }

  public interface IExpression<TValue, out TTerm, out TExpression>
      : IExpression, IValue<TValue, TTerm, TExpression>
      where TValue : IValue<TValue, TTerm, TExpression>
      where TTerm : ITerm<TValue, TTerm, TExpression>
      where TExpression : IExpression<TValue, TTerm, TExpression> {
    IReadOnlyList<TValue> Terms { get; }
  }
}