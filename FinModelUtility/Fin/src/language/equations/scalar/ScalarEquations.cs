using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using fin.util.asserts;
using fin.util.data;


namespace fin.language.equations.scalar {
  // TODO: Optimize this.
  public class ScalarEquations<TIdentifier> : IScalarEquations<TIdentifier>
      where TIdentifier : notnull {
    private readonly Dictionary<TIdentifier, IScalarInput<TIdentifier>>
        inputs_ = new();

    private readonly Dictionary<TIdentifier, IScalarOutput<TIdentifier>>
        outputs_ = new();

    public ScalarEquations() {
      this.Inputs =
          new ReadOnlyDictionary<TIdentifier, IScalarInput<TIdentifier>>(
              this.inputs_);
      this.Outputs =
          new ReadOnlyDictionary<TIdentifier, IScalarOutput<TIdentifier>>(
              this.outputs_);
    }

    public IScalarConstant CreateScalarConstant(double v)
      => new ScalarConstant(v);

    public IReadOnlyDictionary<TIdentifier, IScalarInput<TIdentifier>>
        Inputs { get; }

    public IScalarInput<TIdentifier> CreateScalarInput(
        TIdentifier identifier,
        IScalarConstant defaultValue) {
      Asserts.False(this.inputs_.ContainsKey(identifier));
      Asserts.False(this.outputs_.ContainsKey(identifier));

      var input = new ScalarInput(identifier, defaultValue);
      this.inputs_[identifier] = input;
      return input;
    }

    public IReadOnlyDictionary<TIdentifier, IScalarOutput<TIdentifier>>
        Outputs { get; }

    public IScalarOutput<TIdentifier> CreateScalarOutput(
        TIdentifier identifier,
        IScalarValue value) {
      Asserts.False(this.inputs_.ContainsKey(identifier));
      Asserts.False(this.outputs_.ContainsKey(identifier));

      var output = new ScalarOutput(identifier, value);
      this.outputs_[identifier] = output;
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

      public IScalarValue Value => this.CustomValue ?? this.DefaultValue;
    }

    private class ScalarOutput : BScalarValue, IScalarOutput<TIdentifier> {
      public ScalarOutput(TIdentifier identifier, IScalarValue value) {
        this.Identifier = identifier;
        this.Value = value;
      }

      public TIdentifier Identifier { get; }

      public IScalarValue Value { get; }
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
    }

    private class ScalarExpression : BScalarValue, IScalarExpression {
      public ScalarExpression(IReadOnlyList<IScalarValue> terms) {
        this.Terms = terms;
      }

      public IReadOnlyList<IScalarValue> Terms { get; }

      public IScalarExpression Add(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ScalarExpression(ListUtil.ReadonlyConcat(this.Terms, terms));

      public IScalarExpression Subtract(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ScalarExpression(
            ListUtil.ReadonlyConcat(this.Terms,
                                    this.NegateTerms(term1, terms)));
    }

    private class ScalarTerm : BScalarValue, IScalarTerm {
      public ScalarTerm(IReadOnlyList<IScalarValue> numeratorFactors) {
        this.NumeratorFactors = numeratorFactors;
      }

      public ScalarTerm(
          IReadOnlyList<IScalarValue> numeratorFactors,
          IReadOnlyList<IScalarValue> denominatorFactors) {
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
      public static readonly ScalarConstant NEGATIVE_ONE = new(-1);

      public ScalarConstant(double value) {
        this.Value = value;
      }

      public double Value { get; }
    }
  }
}