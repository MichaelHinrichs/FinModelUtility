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

  public interface IValue { }

  public interface INamedValue : IValue {
    string Name { get; }
  }
}