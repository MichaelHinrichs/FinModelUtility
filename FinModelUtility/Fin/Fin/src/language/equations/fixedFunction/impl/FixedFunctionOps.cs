using fin.model;

namespace fin.language.equations.fixedFunction.impl {
  public class FixedFunctionOpsConstants {
    public const bool SIMPLIFY = true;
  }

  public interface IFixedFunctionOps<TValue, TTerm, TExpression>
      where TValue : IValue<TValue, TTerm, TExpression>
      where TTerm : ITerm<TValue, TTerm, TExpression>
      where TExpression : IExpression<TValue, TTerm, TExpression> {
    TValue Zero { get; }
    TValue Half { get; }
    TValue One { get; }

    bool IsZero(TValue? value) => value == null || value.Equals(this.Zero);

    TValue? Add(TValue? lhs, TValue? rhs);
    TValue? Subtract(TValue? lhs, TValue? rhs);
    TValue? Multiply(TValue? lhs, TValue? rhs);

    TValue? AddWithConstant(TValue? lhs, double constant);
    TValue? MultiplyWithConstant(TValue? lhs, double constant);
  }

  public abstract class BFixedFunctionOps<TValue, TTerm, TExpression>
      : IFixedFunctionOps<TValue, TTerm, TExpression>
      where TValue : IValue<TValue, TTerm, TExpression>
      where TTerm : ITerm<TValue, TTerm, TExpression>
      where TExpression : IExpression<TValue, TTerm, TExpression> {
    public abstract TValue Zero { get; }
    public abstract TValue Half { get; }
    public abstract TValue One { get; }

    public bool IsZero(TValue? value) => value?.Equals(this.Zero) ?? true;

    public abstract TValue? Add(TValue? lhs, TValue? rhs);
    public abstract TValue? AddWithScalar(TValue? lhs, IScalarValue? rhs);

    public abstract TValue? Subtract(TValue? lhs, TValue? rhs);

    public abstract TValue? Multiply(TValue? lhs, TValue? rhs);
    public abstract TValue? MultiplyWithScalar(TValue? lhs, IScalarValue? rhs);

    public TValue? AddWithConstant(TValue? lhs, double constant)
      => this.AddWithScalar(lhs, new ScalarConstant(constant));

    public TValue? MultiplyWithConstant(TValue? lhs, double constant)
      => this.MultiplyWithScalar(lhs, new ScalarConstant(constant));

    public TValue? AddOrSubtractOp(
        bool isAdd,
        TValue? a,
        TValue? b,
        TValue? c,
        TValue? d,
        IScalarValue? bias,
        IScalarValue? scale) {
      var aTimesOneMinusC = this.Multiply(
          a,
          this.Subtract(this.One, c));

      var bTimesC = this.Multiply(b, c);

      var rest = this.Add(aTimesOneMinusC, bTimesC);

      var value = isAdd
          ? this.Add(d, rest)
          : this.Subtract(d, rest);

      value = this.AddWithScalar(value, bias);
      value = this.MultiplyWithScalar(value, scale);

      return value;
    }
  }

  public class ColorFixedFunctionOps
      : BFixedFunctionOps<IColorValue, IColorTerm, IColorExpression> {
    private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;

    private readonly IScalarValue scZero_;
    private readonly IScalarValue scOne_;
    private readonly IScalarValue scMinusOne_;

    public ColorFixedFunctionOps(
        IFixedFunctionEquations<FixedFunctionSource> equations) {
      this.equations_ = equations;

      this.Zero = equations.CreateColorConstant(0);
      this.Half = equations.CreateColorConstant(.5f);
      this.One = equations.CreateColorConstant(1);

      this.scZero_ = equations.CreateScalarConstant(0);
      this.scOne_ = equations.CreateScalarConstant(1);
      this.scMinusOne_ = equations.CreateScalarConstant(-1);
    }

    private bool IsZero_(IScalarValue? value)
      => value?.Equals(this.scZero_) ?? true;

    public override IColorValue Zero { get; }
    public override IColorValue Half { get; }
    public override IColorValue One { get; }

    public override IColorValue? Add(IColorValue? lhs, IColorValue? rhs) {
      if (!FixedFunctionOpsConstants.SIMPLIFY) {
        lhs ??= this.Zero;
        rhs ??= this.Zero;
      } else {
        var lhsIsZero = this.IsZero(lhs);
        var rhsIsZero = this.IsZero(rhs);

        if (lhsIsZero && rhsIsZero) {
          return null;
        }

        if (lhsIsZero) {
          return rhs;
        }

        if (rhsIsZero) {
          return lhs;
        }
      }

      return lhs!.Add(rhs!);
    }

    public override IColorValue? AddWithScalar(IColorValue? lhs,
                                               IScalarValue? rhs) {
      if (!FixedFunctionOpsConstants.SIMPLIFY) {
        lhs ??= this.Zero;
        rhs ??= this.scZero_;
      } else {
        var lhsIsZero = this.IsZero(lhs);
        var rhsIsZero = this.IsZero_(rhs);

        if (lhsIsZero && rhsIsZero) {
          return null;
        }

        if (lhsIsZero) {
          return this.equations_.CreateColor(rhs!);
        }

        if (rhsIsZero) {
          return lhs;
        }
      }

      return lhs!.Add(rhs!);
    }

    public override IColorValue? Subtract(IColorValue? lhs, IColorValue? rhs) {
      if (!FixedFunctionOpsConstants.SIMPLIFY) {
        lhs ??= this.Zero;
        rhs ??= this.Zero;
      } else {
        var lhsIsZero = this.IsZero(lhs);
        var rhsIsZero = this.IsZero(rhs);

        if ((lhsIsZero && rhsIsZero) || (lhs?.Equals(rhs) ?? false)) {
          return null;
        }

        if (lhsIsZero) {
          return rhs?.Multiply(this.scMinusOne_);
        }

        if (rhsIsZero) {
          return lhs;
        }
      }

      return lhs!.Subtract(rhs!);
    }

    public override IColorValue? Multiply(IColorValue? lhs, IColorValue? rhs) {
      if (!FixedFunctionOpsConstants.SIMPLIFY) {
        lhs ??= this.Zero;
        rhs ??= this.Zero;
      } else {
        if (this.IsZero(lhs) || this.IsZero(rhs)) {
          return null;
        }

        var lhsIsOne = lhs?.Equals(this.One) ?? false;
        var rhsIsOne = rhs?.Equals(this.One) ?? false;

        if (lhsIsOne && rhsIsOne) {
          return this.One;
        }

        if (lhsIsOne) {
          return rhs;
        }

        if (rhsIsOne) {
          return lhs;
        }
      }

      return lhs!.Multiply(rhs!);
    }

    public override IColorValue? MultiplyWithScalar(
        IColorValue? lhs,
        IScalarValue? rhs) {
      if (!FixedFunctionOpsConstants.SIMPLIFY) {
        lhs ??= this.Zero;
        rhs ??= this.scZero_;
      } else {
        if (this.IsZero(lhs) || this.IsZero_(rhs)) {
          return null;
        }

        var lhsIsOne = lhs?.Equals(this.One) ?? false;
        var rhsIsOne = rhs?.Equals(this.scOne_) ?? false;

        if (lhsIsOne && rhsIsOne) {
          return this.One;
        }

        if (lhsIsOne) {
          return this.equations_.CreateColor(rhs!);
        }

        if (rhsIsOne) {
          return lhs;
        }
      }

      return lhs!.Multiply(rhs!);
    }
  }

  public class ScalarFixedFunctionOps
      : BFixedFunctionOps<IScalarValue, IScalarTerm, IScalarExpression> {
    private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;

    private readonly IScalarValue scMinusOne_;

    public ScalarFixedFunctionOps(
        IFixedFunctionEquations<FixedFunctionSource> equations) {
      this.equations_ = equations;

      this.Zero = equations.CreateScalarConstant(0);
      this.Half = equations.CreateScalarConstant(.5f);
      this.One = equations.CreateScalarConstant(1);
      this.scMinusOne_ = equations.CreateScalarConstant(-1);
    }

    public override IScalarValue Zero { get; }
    public override IScalarValue Half { get; }
    public override IScalarValue One { get; }

    public override IScalarValue? AddWithScalar(
        IScalarValue? lhs,
        IScalarValue? rhs)
      => this.Add(lhs, rhs);

    public override IScalarValue? Add(IScalarValue? lhs, IScalarValue? rhs) {
      if (!FixedFunctionOpsConstants.SIMPLIFY) {
        lhs ??= this.Zero;
        rhs ??= this.Zero;
      } else {
        var lhsIsZero = this.IsZero(lhs);
        var rhsIsZero = this.IsZero(rhs);

        if (lhsIsZero && rhsIsZero) {
          return null;
        }

        if (lhsIsZero) {
          return rhs;
        }

        if (rhsIsZero) {
          return lhs;
        }
      }

      return lhs!.Add(rhs!);
    }

    public override IScalarValue?
        Subtract(IScalarValue? lhs, IScalarValue? rhs) {
      if (!FixedFunctionOpsConstants.SIMPLIFY) {
        lhs ??= this.Zero;
        rhs ??= this.Zero;
      } else {
        var lhsIsZero = this.IsZero(lhs);
        var rhsIsZero = this.IsZero(rhs);

        if ((lhsIsZero && rhsIsZero) || (lhs?.Equals(rhs) ?? false)) {
          return null;
        }

        if (lhsIsZero) {
          return rhs?.Multiply(this.scMinusOne_);
        }

        if (rhsIsZero) {
          return lhs;
        }
      }

      return lhs!.Subtract(rhs!);
    }

    public override IScalarValue? MultiplyWithScalar(
        IScalarValue? lhs,
        IScalarValue? rhs)
      => this.Multiply(lhs, rhs);

    public override IScalarValue?
        Multiply(IScalarValue? lhs, IScalarValue? rhs) {
      if (!FixedFunctionOpsConstants.SIMPLIFY) {
        lhs ??= this.Zero;
        rhs ??= this.Zero;
      } else {
        if (this.IsZero(lhs) || this.IsZero(rhs)) {
          return null;
        }

        var lhsIsOne = lhs?.Equals(this.One) ?? false;
        var rhsIsOne = rhs?.Equals(this.One) ?? false;

        if (lhsIsOne && rhsIsOne) {
          return this.One;
        }

        if (lhsIsOne) {
          return rhs;
        }

        if (rhsIsOne) {
          return lhs;
        }
      }

      return lhs!.Multiply(rhs!);
    }
  }
}