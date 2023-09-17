namespace fin.language.equations.fixedFunction {
  public interface IScalarNamedValue : INamedValue, IScalarFactor {
    IScalarValue ScalarValue { get; }
  }

  public interface IScalarIdentifiedValue<TIdentifier> :
      IIdentifiedValue<TIdentifier>,
      IScalarFactor {
    IScalarValue ScalarValue { get; }
  }

  public interface
      IScalarInput<TIdentifier> : IScalarIdentifiedValue<TIdentifier> {
    IScalarConstant DefaultValue { get; }
    IScalarConstant? CustomValue { get; set; }
  }

  public interface IScalarOutput<TIdentifier>
      : IScalarIdentifiedValue<TIdentifier> { }


  public interface IScalarValue
      : IValue<IScalarValue, IScalarTerm, IScalarExpression> {
    bool Clamp { get; set; }

    IColorValueTernaryOperator TernaryOperator(
        BoolComparisonType comparisonType,
        IScalarValue other,
        IColorValue trueValue,
        IColorValue falseValue);
  }

  public interface IScalarTerm
      : IScalarValue,
        ITerm<IScalarValue, IScalarTerm, IScalarExpression> { }

  public interface IScalarExpression
      : IScalarValue,
        IExpression<IScalarValue, IScalarTerm, IScalarExpression> { }


  public interface IScalarFactor : IScalarValue { }

  public interface IScalarConstant : IScalarFactor {
    double Value { get; }
  }
}