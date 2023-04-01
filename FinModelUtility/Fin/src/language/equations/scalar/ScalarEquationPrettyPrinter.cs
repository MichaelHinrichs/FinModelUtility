using System.IO;

using asserts;

namespace fin.language.equations.scalar {
  public class ScalarEquationsPrettyPrinter<TIdentifier> {
    public void Print(
        StringWriter os,
        IScalarEquations<TIdentifier> equations) {
      os.WriteLine("Inputs:");
      foreach (var (id, input) in equations.Inputs) {
        os.Write(id);
        os.Write(": ");
        this.PrintConstant_(
            os,
            input.CustomValue ?? input.DefaultValue);
        os.Write("\n");
      }

      os.WriteLine();
      os.WriteLine("Outputs:");
      foreach (var (id, output) in equations.Outputs) {
        os.Write(id);
        os.Write(": ");
        this.PrintValue_(
            os,
            output.Value);
        os.Write("\n");
      }
    }

    private void PrintValue_(
        StringWriter os,
        IScalarValue value,
        bool wrapExpressions = false) {
      if (value is IScalarExpression expression) {
        if (wrapExpressions) {
          os.Write("(");
        }
        this.PrintExpression_(os, expression);
        if (wrapExpressions) {
          os.Write(")");
        }
      } else if (value is IScalarTerm term) {
        this.PrintTerm_(os, term);
      } else if (value is IScalarFactor factor) {
        this.PrintFactor_(os, factor);
      } else {
        Asserts.Fail("Unsupported value type!");
      }
    }

    private void PrintExpression_(
        StringWriter os,
        IScalarExpression expression) {
      var terms = expression.Terms;

      for (var i = 0; i < terms.Count; ++i) {
        var term = terms[i];

        if (i > 0) {
          os.Write(" + ");
        }
        this.PrintValue_(os, term);
      }
    }

    private void PrintTerm_(
        StringWriter os,
        IScalarTerm scalarTerm) {
      var numerators = scalarTerm.NumeratorFactors;
      var denominators = scalarTerm.DenominatorFactors;

      if (numerators.Count > 0) {
        for (var i = 0; i < numerators.Count; ++i) {
          var numerator = numerators[i];

          if (i > 0) {
            os.Write("*");
          }

          this.PrintValue_(os, numerator, true);
        }
      } else {
        os.Write(1);
      }

      if (denominators != null) {
        for (var i = 0; i < denominators.Count; ++i) {
          var denominator = denominators[i];

          os.Write("/");

          this.PrintValue_(os, denominator, true);
        }
      }
    }

    private void PrintFactor_(
        StringWriter os,
        IScalarFactor factor) {
      if (factor is INamedValue<TIdentifier> namedValue) {
        this.PrintNamedValue_(os, namedValue);
      } else if (factor is IScalarConstant constant) {
        this.PrintConstant_(os, constant);
      } else {
        Asserts.Fail("Unsupported factor type!");
      }
    }

    private void PrintNamedValue_(
        StringWriter os,
        INamedValue<TIdentifier> namedValue)
      => os.Write("{" + namedValue.Identifier + "}");

    private void PrintConstant_(
        StringWriter os,
        IScalarConstant constant)
      => os.Write(constant.Value);
  }
}