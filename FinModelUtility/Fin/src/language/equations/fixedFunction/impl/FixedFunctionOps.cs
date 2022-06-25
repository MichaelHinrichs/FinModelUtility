using fin.model;


namespace fin.language.equations.fixedFunction.impl {
  public abstract class BFixedFunctionOps<T> where T : notnull {
    public abstract T Zero { get; }
    public abstract T One { get; }

    public bool IsZero(T? value)
      => value == null || value.Equals(this.Zero);

    public abstract T? Add(T? lhs, T? rhs);
    public abstract T? AddWithScalar(T? lhs, IScalarValue? rhs);

    public abstract T? Subtract(T? lhs, T? rhs);

    public abstract T? Multiply(T? lhs, T? rhs);
    public abstract T? MultiplyWithScalar(T? lhs, IScalarValue? rhs);

    public T? AddOrSubtractOp(
        bool isAdd,
        T? a,
        T? b,
        T? c,
        T? d,
        IScalarValue? bias,
        IScalarValue? scale) {
      var aTimesOneMinusC = this.Multiply(
          a, this.Subtract(this.One, c));

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

  public class ColorFixedFunctionOps : BFixedFunctionOps<IColorValue> {
    private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;

    private readonly IScalarValue scZero_;
    private readonly IScalarValue scOne_;
    private readonly IScalarValue scMinusOne_;

    public ColorFixedFunctionOps(
        IFixedFunctionEquations<FixedFunctionSource> equations) {
      this.equations_ = equations;

      this.Zero = equations.CreateColorConstant(0);
      this.One = equations.CreateColorConstant(1);

      this.scZero_ = equations.CreateScalarConstant(0);
      this.scOne_ = equations.CreateScalarConstant(1);
      this.scMinusOne_ = equations.CreateScalarConstant(-1);
    }

    private bool IsZero(IScalarValue? value)
      => value == null || value == this.scZero_;

    public override IColorValue Zero { get; }
    public override IColorValue One { get; }

    public override IColorValue? Add(IColorValue? lhs, IColorValue? rhs) {
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

      return lhs!.Add(rhs!);
    }

    public override IColorValue? AddWithScalar(IColorValue? lhs,
                                               IScalarValue? rhs) {
      var lhsIsZero = this.IsZero(lhs);
      var rhsIsZero = this.IsZero(rhs);

      if (lhsIsZero && rhsIsZero) {
        return null;
      }
      if (lhsIsZero) {
        return this.equations_.CreateColor(rhs!);
      }
      if (rhsIsZero) {
        return lhs;
      }

      return lhs!.Add(rhs!);
    }

    public override IColorValue? Subtract(IColorValue? lhs, IColorValue? rhs) {
      var lhsIsZero = this.IsZero(lhs);
      var rhsIsZero = this.IsZero(rhs);

      if ((lhsIsZero && rhsIsZero) || lhs == rhs) {
        return null;
      }
      if (lhsIsZero) {
        return rhs?.Multiply(this.scMinusOne_);
      }
      if (rhsIsZero) {
        return lhs;
      }

      return lhs!.Subtract(rhs!);
    }

    public override IColorValue? Multiply(IColorValue? lhs, IColorValue? rhs) {
      if (this.IsZero(lhs) || this.IsZero(rhs)) {
        return null;
      }

      var lhsIsOne = lhs == this.One;
      var rhsIsOne = rhs == this.One;

      if (lhsIsOne && rhsIsOne) {
        return this.One;
      }
      if (lhsIsOne) {
        return rhs;
      }
      if (rhsIsOne) {
        return lhs;
      }

      return lhs!.Multiply(rhs!);
    }

    public override IColorValue? MultiplyWithScalar(
        IColorValue? lhs,
        IScalarValue? rhs) {
      if (this.IsZero(lhs) || this.IsZero(rhs)) {
        return null;
      }

      var lhsIsOne = lhs == this.One;
      var rhsIsOne = rhs == this.One;

      if (lhsIsOne && rhsIsOne) {
        return this.One;
      }
      if (lhsIsOne) {
        return this.equations_.CreateColor(rhs!);
      }
      if (rhsIsOne) {
        return lhs;
      }

      return lhs!.Multiply(rhs!);
    }
  }

  public class ScalarFixedFunctionOps : BFixedFunctionOps<IScalarValue> {
    private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;

    private readonly IScalarValue scMinusOne_;

    public ScalarFixedFunctionOps(
        IFixedFunctionEquations<FixedFunctionSource> equations) {
      this.equations_ = equations;

      this.Zero = equations.CreateScalarConstant(0);
      this.One = equations.CreateScalarConstant(1);
      this.scMinusOne_ = equations.CreateScalarConstant(-1);
    }

    public override IScalarValue Zero { get; }
    public override IScalarValue One { get; }

    public override IScalarValue? AddWithScalar(
        IScalarValue? lhs,
        IScalarValue? rhs)
      => this.Add(lhs, rhs);

    public override IScalarValue? Add(IScalarValue? lhs, IScalarValue? rhs) {
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

      return lhs!.Add(rhs!);
    }

    public override IScalarValue?
        Subtract(IScalarValue? lhs, IScalarValue? rhs) {
      var lhsIsZero = this.IsZero(lhs);
      var rhsIsZero = this.IsZero(rhs);

      if ((lhsIsZero && rhsIsZero) || lhs == rhs) {
        return null;
      }
      if (lhsIsZero) {
        return rhs?.Multiply(this.scMinusOne_);
      }
      if (rhsIsZero) {
        return lhs;
      }

      return lhs!.Subtract(rhs!);
    }

    public override IScalarValue? MultiplyWithScalar(
        IScalarValue? lhs,
        IScalarValue? rhs)
      => this.Multiply(lhs, rhs);

    public override IScalarValue?
        Multiply(IScalarValue? lhs, IScalarValue? rhs) {
      if (this.IsZero(lhs) || this.IsZero(rhs)) {
        return null;
      }

      var lhsIsOne = lhs == this.One;
      var rhsIsOne = rhs == this.One;

      if (lhsIsOne && rhsIsOne) {
        return this.One;
      }
      if (lhsIsOne) {
        return rhs;
      }
      if (rhsIsOne) {
        return lhs;
      }

      return lhs!.Multiply(rhs!);
    }
  }
}