using System.Collections.Generic;
using System.Linq;
using fin.util.asserts;
using fin.util.data;


namespace fin.language.equations.fixedFunction {
  // TODO: Optimize this.
  public partial class FixedFunctionEquations<TIdentifier> {
    private readonly Dictionary<double, IScalarConstant> scalarConstants_ =
        new();

    private readonly Dictionary<TIdentifier, IScalarInput<TIdentifier>>
        scalarInputs_ = new();

    private readonly Dictionary<TIdentifier, IScalarOutput<TIdentifier>>
        scalarOutputs_ = new();

    public IScalarConstant CreateScalarConstant(double v) {
      if (this.scalarConstants_.TryGetValue(
              v, out var scalarConstant)) {
        return scalarConstant;
      }

      return this.scalarConstants_[v] = new ScalarConstant(v);
    }

    public IReadOnlyDictionary<TIdentifier, IScalarInput<TIdentifier>>
        ScalarInputs { get; }

    public IScalarInput<TIdentifier> CreateScalarInput(
        TIdentifier identifier,
        IScalarConstant defaultValue) {
      Asserts.False(this.scalarInputs_.ContainsKey(identifier));
      Asserts.False(this.scalarOutputs_.ContainsKey(identifier));

      var input = new ScalarInput(identifier, defaultValue);
      this.scalarInputs_[identifier] = input;
      return input;
    }

    public IReadOnlyDictionary<TIdentifier, IScalarOutput<TIdentifier>>
        ScalarOutputs { get; }

    public IScalarOutput<TIdentifier> CreateScalarOutput(
        TIdentifier identifier,
        IScalarValue value) {
      Asserts.False(this.scalarInputs_.ContainsKey(identifier));
      Asserts.False(this.scalarOutputs_.ContainsKey(identifier));

      var output = new ScalarOutput(identifier, value);
      this.scalarOutputs_[identifier] = output;
      return output;
    }


    private class ScalarInput : BScalarValue, IScalarInput<TIdentifier> {
      public ScalarInput(TIdentifier identifier, IScalarConstant defaultValue) {
        this.Identifier = identifier;
        this.DefaultValue = defaultValue;
      }

      public TIdentifier Identifier { get; }

      public IScalarConstant DefaultValue { get; }
      public IScalarConstant? CustomValue { get; set; }

      public IScalarValue ScalarValue => this.CustomValue ?? this.DefaultValue;
    }

    private class ScalarOutput : BScalarValue, IScalarOutput<TIdentifier> {
      public ScalarOutput(TIdentifier identifier, IScalarValue value) {
        this.Identifier = identifier;
        this.ScalarValue = value;
      }

      public TIdentifier Identifier { get; }

      public IScalarValue ScalarValue { get; }
    }

    private abstract class BScalarValue : IScalarValue {
      public IScalarExpression Add(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ScalarExpression(ListUtil.ReadonlyFrom(this, term1, terms));

      public IScalarExpression Subtract(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ScalarExpression(
            ListUtil.ReadonlyFrom(
                this,
                this.NegateTerms(term1, terms)));

      public IScalarTerm Multiply(
          IScalarValue factor1,
          params IScalarValue[] factors)
        => new ScalarTerm(ListUtil.ReadonlyFrom(this, factor1, factors));

      public IScalarTerm Divide(
          IScalarValue factor1,
          params IScalarValue[] factors)
        => new ScalarTerm(ListUtil.ReadonlyFrom(this),
                          ListUtil.ReadonlyFrom(factor1, factors));

      protected IScalarValue[] NegateTerms(
          IScalarValue term1,
          params IScalarValue[] terms)
        => this.NegateTerms(ListUtil.From(term1, terms).ToArray());

      protected IScalarValue[] NegateTerms(
          params IScalarValue[] terms)
        => terms.Select(
                    term => new ScalarTerm(
                        ListUtil.ReadonlyFrom(
                            ScalarConstant.NEGATIVE_ONE,
                            term)))
                .ToArray();

      public bool Clamp { get; set; }

      public IColorValueTernaryOperator TernaryOperator(
          BoolComparisonType comparisonType,
          IScalarValue other,
          IColorValue trueValue,
          IColorValue falseValue)
        => new ColorValueTernaryOperator {
            ComparisonType = comparisonType,
            Lhs = this,
            Rhs = other,
            TrueValue = trueValue,
            FalseValue = falseValue
        };
    }

    private class ScalarExpression : BScalarValue, IScalarExpression {
      public ScalarExpression(IReadOnlyList<IScalarValue> terms) {
        this.Terms = terms;
      }

      public IReadOnlyList<IScalarValue> Terms { get; }

      public IScalarExpression Add(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ScalarExpression(
            ListUtil.ReadonlyConcat(this.Terms, new[] {term1}, terms));

      public IScalarExpression Subtract(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ScalarExpression(
            ListUtil.ReadonlyConcat(this.Terms,
                                    this.NegateTerms(term1, terms)));
    }

    private class ScalarTerm : BScalarValue, IScalarTerm {
      public ScalarTerm(
          IReadOnlyList<IScalarValue> numeratorFactors,
          IReadOnlyList<IScalarValue>? denominatorFactors = null) {
        this.NumeratorFactors = numeratorFactors;
        this.DenominatorFactors = denominatorFactors;
      }

      public IReadOnlyList<IScalarValue> NumeratorFactors { get; }
      public IReadOnlyList<IScalarValue>? DenominatorFactors { get; }

      public IScalarTerm Multiply(
          IScalarValue factor1,
          params IScalarValue[] factors)
        => new ScalarTerm(ListUtil.ReadonlyConcat(
                              this.NumeratorFactors,
                              ListUtil.ReadonlyFrom(factor1, factors)));

      public IScalarTerm Divide(
          IScalarValue factor1,
          params IScalarValue[] factors)
        => new ScalarTerm(this.NumeratorFactors,
                          ListUtil.ReadonlyConcat(
                              this.DenominatorFactors,
                              ListUtil.ReadonlyFrom(factor1, factors)));
    }

    private class ScalarConstant : BScalarValue, IScalarConstant {
      public static readonly ScalarConstant ONE = new(1);
      public static readonly ScalarConstant NEGATIVE_ONE = new(-1);

      public ScalarConstant(double value) {
        this.Value = value;
      }

      public double Value { get; }
    }
  }
}