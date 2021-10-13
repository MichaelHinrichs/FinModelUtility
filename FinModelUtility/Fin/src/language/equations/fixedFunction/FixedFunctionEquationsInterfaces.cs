using System.Collections.Generic;

namespace fin.language.equations.fixedFunction {
  public interface IFixedFunctionEquations<TIdentifier> {
    IScalarConstant CreateScalarConstant(double v);

    IColorConstant CreateColorConstant(
        double r,
        double g,
        double b,
        double? a = null);

    IColorConstant CreateColorConstant(
        double intensity,
        double? a = null);

    IColorFactor CreateColor(IScalarValue r,
                             IScalarValue g,
                             IScalarValue b,
                             IScalarValue? a = null);

    IColorFactor CreateColor(IScalarValue intensity,
                             IScalarValue? a = null);


    IReadOnlyDictionary<TIdentifier, IScalarInput<TIdentifier>>
        ScalarInputs { get; }

    IScalarInput<TIdentifier> CreateScalarInput(
        TIdentifier identifier,
        IScalarConstant defaultValue);


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

    IReadOnlyDictionary<TIdentifier, IColorOutput<TIdentifier>>
        ColorOutputs { get; }

    IColorOutput<TIdentifier> CreateColorOutput(
        TIdentifier identifier,
        IColorValue value);
  }

  public interface INamedValue<TIdentifier> {
    TIdentifier Identifier { get; }
  }
}