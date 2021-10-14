using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using fin.util.asserts;
using fin.util.data;

namespace fin.language.equations.fixedFunction {
  // TODO: Optimize this.
  public partial class FixedFunctionEquations<TIdentifier> {
    private readonly Dictionary<TIdentifier, IColorInput<TIdentifier>>
        colorInputs_ = new();

    private readonly Dictionary<TIdentifier, IColorOutput<TIdentifier>>
        colorOutputs_ = new();

    public IColorConstant CreateColorConstant(
        double r,
        double g,
        double b)
      => new ColorConstant(r, g, b);

    public IColorConstant CreateColorConstant(
        double intensity)
      => new ColorConstant(intensity);

    public IColorFactor CreateColor(
        IScalarValue r,
        IScalarValue g,
        IScalarValue b) => new ColorWrapper(r, g, b);

    public IColorFactor CreateColor(
        IScalarValue intensity) => new ColorWrapper(intensity);


    public IReadOnlyDictionary<TIdentifier, IColorInput<TIdentifier>>
        ColorInputs { get; }

    public IColorInput<TIdentifier> CreateColorInput(
        TIdentifier identifier,
        IColorConstant defaultValue) {
      Asserts.False(this.colorInputs_.ContainsKey(identifier));
      Asserts.False(this.colorOutputs_.ContainsKey(identifier));

      var input = new ColorInput(identifier, defaultValue);
      this.colorInputs_[identifier] = input;
      return input;
    }

    public IReadOnlyDictionary<TIdentifier, IColorOutput<TIdentifier>>
        ColorOutputs { get; }

    public IColorOutput<TIdentifier> CreateColorOutput(
        TIdentifier identifier,
        IColorValue value) {
      Asserts.False(this.colorInputs_.ContainsKey(identifier));
      Asserts.False(this.colorOutputs_.ContainsKey(identifier));

      var output = new ColorOutput(identifier, value);
      this.colorOutputs_[identifier] = output;
      return output;
    }


    private class ColorInput : BColorValue, IColorInput<TIdentifier> {
      public ColorInput(TIdentifier identifier, IColorConstant defaultValue) {
        this.Identifier = identifier;
        this.DefaultValue = defaultValue;
      }

      public TIdentifier Identifier { get; }

      public IColorConstant DefaultValue { get; }
      public IColorConstant? CustomValue { get; set; }

      public IColorValue ColorValue => this.CustomValue ?? this.DefaultValue;

      public override IScalarValue? Intensity
        => throw new NotSupportedException();

      public override IScalarValue R
        => new ColorNamedValueSwizzle(this, ColorSwizzle.R);

      public override IScalarValue G
        => new ColorNamedValueSwizzle(this, ColorSwizzle.G);

      public override IScalarValue B
        => new ColorNamedValueSwizzle(this, ColorSwizzle.B);
    }

    private class ColorOutput : BColorValue, IColorOutput<TIdentifier> {
      public ColorOutput(TIdentifier identifier, IColorValue value) {
        this.Identifier = identifier;
        this.ColorValue = value;
      }

      public TIdentifier Identifier { get; }

      public IColorValue ColorValue { get; }

      public override IScalarValue? Intensity
        => throw new NotSupportedException();

      public override IScalarValue R
        => new ColorNamedValueSwizzle(this, ColorSwizzle.R);

      public override IScalarValue G
        => new ColorNamedValueSwizzle(this, ColorSwizzle.G);

      public override IScalarValue B
        => new ColorNamedValueSwizzle(this, ColorSwizzle.B);
    }


    private class ColorNamedValueSwizzle : BScalarValue,
                                           IColorNamedValueSwizzle<
                                               TIdentifier> {
      public ColorNamedValueSwizzle(
          IColorNamedValue<TIdentifier> source,
          ColorSwizzle swizzleType) {
        this.Source = source;
        this.SwizzleType = swizzleType;
      }

      public IColorNamedValue<TIdentifier> Source { get; }
      public ColorSwizzle SwizzleType { get; }
    }


    private abstract class BColorValue : IColorValue {
      public IColorExpression Add(
          IColorValue term1,
          params IColorValue[] terms)
        => new ColorExpression(ListUtil.ReadonlyFrom(this, term1, terms));

      public IColorExpression Subtract(
          IColorValue term1,
          params IColorValue[] terms)
        => new ColorExpression(
            ListUtil.ReadonlyFrom(
                this,
                this.NegateTerms(term1, terms)));

      public IColorTerm Multiply(
          IColorValue factor1,
          params IColorValue[] factors)
        => new ColorTerm(ListUtil.ReadonlyFrom(this, factor1, factors));

      public IColorTerm Divide(
          IColorValue factor1,
          params IColorValue[] factors)
        => new ColorTerm(ListUtil.ReadonlyFrom(this),
                         ListUtil.ReadonlyFrom(factor1, factors));

      protected IColorValue[] NegateTerms(
          IColorValue term1,
          params IColorValue[] terms)
        => this.NegateTerms(ListUtil.From(term1, terms).ToArray());

      protected IColorValue[] NegateTerms(
          params IColorValue[] terms)
        => terms.Select(
                    term => new ColorTerm(
                        ListUtil.ReadonlyFrom(
                            ColorConstant.NEGATIVE_ONE,
                            term)))
                .ToArray();


      public IColorExpression Add(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ColorExpression(
            ListUtil.ReadonlyFrom(this, this.ToColorValues(term1, terms)));

      public IColorExpression Subtract(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ColorExpression(
            ListUtil.ReadonlyFrom(
                this,
                this.ToColorValues(this.NegateTerms(term1, terms))));

      public IColorTerm Multiply(
          IScalarValue factor1,
          params IScalarValue[] factors)
        => new ColorTerm(
            ListUtil.ReadonlyFrom(this, this.ToColorValues(factor1, factors)));

      public IColorTerm Divide(
          IScalarValue factor1,
          params IScalarValue[] factors)
        => new ColorTerm(ListUtil.ReadonlyFrom(this),
                         new ReadOnlyCollection<IColorValue>(
                             this.ToColorValues(factor1, factors)));

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

      protected IColorValue[] ToColorValues(params IScalarValue[] scalars)
        => scalars.Select(scalar => new ColorWrapper(scalar)).ToArray();

      protected IColorValue[] ToColorValues(
          IScalarValue first,
          params IScalarValue[] scalars)
        => this.ToColorValues(ListUtil.From(first, scalars).ToArray());


      public abstract IScalarValue? Intensity { get; }
      public abstract IScalarValue R { get; }
      public abstract IScalarValue G { get; }
      public abstract IScalarValue B { get; }
    }

    private class ColorExpression : BColorValue, IColorExpression {
      public ColorExpression(IReadOnlyList<IColorValue> terms) {
        this.Terms = terms;
      }

      public IReadOnlyList<IColorValue> Terms { get; }

      public IColorExpression Add(
          IColorValue term1,
          params IColorValue[] terms)
        => new ColorExpression(ListUtil.ReadonlyConcat(this.Terms, terms));

      public IColorExpression Subtract(
          IColorValue term1,
          params IColorValue[] terms)
        => new ColorExpression(
            ListUtil.ReadonlyConcat(this.Terms,
                                    this.NegateTerms(term1, terms)));

      public IColorExpression Add(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ColorExpression(
            ListUtil.ReadonlyConcat(this.Terms, this.ToColorValues(terms)));

      public IColorExpression Subtract(
          IScalarValue term1,
          params IScalarValue[] terms)
        => new ColorExpression(
            ListUtil.ReadonlyConcat(this.Terms,
                                    this.ToColorValues(
                                        this.NegateTerms(term1, terms))));

      public override IScalarValue? Intensity {
        get {
          var numeratorAs =
              this.Terms.Select(factor => factor.Intensity)
                  .ToArray();

          if (numeratorAs.Any(a => a == null)) {
            return null;
          }

          return new ScalarExpression(numeratorAs.Select(a => a!).ToArray());
        }
      }

      public override IScalarValue R
        => new ScalarExpression(this.Terms.Select(factor => factor.R)
                                    .ToArray());

      public override IScalarValue G
        => new ScalarExpression(this.Terms.Select(factor => factor.G)
                                    .ToArray());

      public override IScalarValue B
        => new ScalarExpression(this.Terms.Select(factor => factor.B)
                                    .ToArray());
    }

    private class ColorTerm : BColorValue, IColorTerm {
      public ColorTerm(IReadOnlyList<IColorValue> numeratorFactors) {
        this.NumeratorFactors = numeratorFactors;
      }

      public ColorTerm(
          IReadOnlyList<IColorValue> numeratorFactors,
          IReadOnlyList<IColorValue> denominatorFactors) {
        this.NumeratorFactors = numeratorFactors;
        this.DenominatorFactors = denominatorFactors;
      }

      public IReadOnlyList<IColorValue> NumeratorFactors { get; }
      public IReadOnlyList<IColorValue>? DenominatorFactors { get; }

      public IColorTerm Multiply(
          IColorValue factor1,
          params IColorValue[] factors)
        => new ColorTerm(ListUtil.ReadonlyConcat(
                             this.NumeratorFactors,
                             ListUtil.ReadonlyFrom(factor1, factors)));

      public IColorTerm Divide(
          IColorValue factor1,
          params IColorValue[] factors)
        => new ColorTerm(this.NumeratorFactors,
                         ListUtil.ReadonlyConcat(
                             this.DenominatorFactors,
                             ListUtil.ReadonlyFrom(factor1, factors)));

      public IColorTerm Multiply(
          IScalarValue factor1,
          params IScalarValue[] factors)
        => new ColorTerm(ListUtil.ReadonlyConcat(
                             this.NumeratorFactors,
                             this.ToColorValues(factor1, factors)));

      public IColorTerm Divide(
          IScalarValue factor1,
          params IScalarValue[] factors)
        => new ColorTerm(this.NumeratorFactors,
                         ListUtil.ReadonlyConcat(
                             this.DenominatorFactors,
                             this.ToColorValues(factor1, factors)));

      public override IScalarValue? Intensity {
        get {
          var numeratorAs =
              this.NumeratorFactors.Select(factor => factor.Intensity)
                  .ToArray();
          var denominatorAs =
              this.DenominatorFactors?.Select(factor => factor.Intensity)
                  .ToArray();

          if (numeratorAs.Any(a => a == null) ||
              (denominatorAs?.Any(a => a == null) ?? false)) {
            return null;
          }

          return new ScalarTerm(
              numeratorAs.Select(a => a!).ToArray(),
              denominatorAs?.Select(a => a!).ToArray());
        }
      }

      public override IScalarValue R
        => new ScalarTerm(
            this.NumeratorFactors.Select(factor => factor.R).ToArray(),
            this.DenominatorFactors?.Select(factor => factor.R)
                .ToArray());

      public override IScalarValue G
        => new ScalarTerm(
            this.NumeratorFactors.Select(factor => factor.G).ToArray(),
            this.DenominatorFactors?.Select(factor => factor.G)
                .ToArray());

      public override IScalarValue B
        => new ScalarTerm(
            this.NumeratorFactors.Select(factor => factor.B).ToArray(),
            this.DenominatorFactors?.Select(factor => factor.B)
                .ToArray());
    }

    private class ColorConstant : BColorValue, IColorConstant {
      public static readonly ColorConstant NEGATIVE_ONE = new(-1);

      public ColorConstant(double r, double g, double b) {
        this.RValue = r;
        this.GValue = g;
        this.BValue = b;

        this.R = new ScalarConstant(r);
        this.G = new ScalarConstant(g);
        this.B = new ScalarConstant(b);
      }

      public ColorConstant(double intensity) {
        this.IntensityValue = intensity;
        this.RValue = intensity;
        this.GValue = intensity;
        this.BValue = intensity;

        this.Intensity = new ScalarConstant(intensity);
        this.R = new ScalarConstant(intensity);
        this.G = new ScalarConstant(intensity);
        this.B = new ScalarConstant(intensity);
      }


      public double? IntensityValue { get; }
      public double RValue { get; }
      public double GValue { get; }
      public double BValue { get; }

      public override IScalarValue? Intensity { get; }
      public override IScalarValue R { get; }
      public override IScalarValue G { get; }
      public override IScalarValue B { get; }
    }

    private class ColorWrapper : BColorValue, IColorFactor {
      public ColorWrapper(
          IScalarValue r,
          IScalarValue g,
          IScalarValue b) {
        this.R = r;
        this.G = g;
        this.B = b;
      }

      public ColorWrapper(IScalarValue intensity) {
        this.Intensity = intensity;
        this.R = intensity;
        this.G = intensity;
        this.B = intensity;
      }

      public override IScalarValue? Intensity { get; }
      public override IScalarValue R { get; }
      public override IScalarValue G { get; }
      public override IScalarValue B { get; }
    }
  }
}