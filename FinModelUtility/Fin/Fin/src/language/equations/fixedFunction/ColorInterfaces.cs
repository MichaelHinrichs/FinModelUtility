namespace fin.language.equations.fixedFunction {
  public interface IColorNamedValue : INamedValue, IColorFactor {
    IColorValue ColorValue { get; }
  }

  public interface IColorIdentifiedValue<TIdentifier>
      : IIdentifiedValue<TIdentifier>,
        IColorFactor {
    IColorValue ColorValue { get; }
  }

  public interface IColorInput<TIdentifier>
      : IColorIdentifiedValue<TIdentifier> {
    IColorConstant DefaultValue { get; }
    IColorConstant? CustomValue { get; set; }
  }

  public interface IColorOutput<TIdentifier>
      : IColorIdentifiedValue<TIdentifier> { }

  public enum ColorSwizzle {
    R,
    G,
    B,
  }

  public interface IColorNamedValueSwizzle<TIdentifier> : IScalarFactor {
    IColorIdentifiedValue<TIdentifier> Source { get; }
    ColorSwizzle SwizzleType { get; }
  }

  public interface IColorValueSwizzle : IScalarFactor {
    IColorValue Source { get; }
    ColorSwizzle SwizzleType { get; }
  }


  public interface IColorValue
      : IValue<IColorValue, IColorTerm, IColorExpression> {
    IScalarValue? Intensity { get; }
    IScalarValue R { get; }
    IScalarValue G { get; }
    IScalarValue B { get; }

    bool Clamp { get; set; }
  }

  public interface IColorTerm
      : IColorValue,
        ITerm<IColorValue, IColorTerm, IColorExpression> { }

  public interface IColorExpression
      : IColorValue,
        IExpression<IColorValue, IColorTerm, IColorExpression> { }

  public interface IColorFactor : IColorValue { }

  public interface IColorConstant : IColorFactor {
    double? IntensityValue { get; }
    double RValue { get; }
    double GValue { get; }
    double BValue { get; }
  }
}