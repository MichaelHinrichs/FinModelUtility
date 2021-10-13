using System.IO;
using System.Linq;
using System.Text;

using fin.util.asserts;
using fin.util.strings;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.language.equations.scalar {
  [TestClass]
  public class ScalarEquationsTest {
    [TestMethod]
    public void TestInOut() {
      var equations = new ScalarEquations<string>();

      var fooIn =
          equations.CreateScalarInput("fooIn",
                                      equations.CreateScalarConstant(123));
      var fooOut = equations.CreateScalarOutput("fooOut", fooIn);

      this.AssertEquals_(equations,
                         "Inputs:",
                         "fooIn: 123",
                         "",
                         "Outputs:",
                         "fooOut: {fooIn}");
    }

    [TestMethod]
    public void TestEquation() {
      var equations = new ScalarEquations<string>();

      var a =
          equations.CreateScalarInput("a",
                                      equations.CreateScalarConstant(0));
      var b =
          equations.CreateScalarInput("b",
                                      equations.CreateScalarConstant(1));
      var c =
          equations.CreateScalarInput("c",
                                      equations.CreateScalarConstant(2));
      var d =
          equations.CreateScalarInput("d",
                                      equations.CreateScalarConstant(3));

      var output =
          equations.CreateScalarOutput("output",
                                       a.Multiply(b).Divide(c).Add(d));

      this.AssertEquals_(equations,
                         "Inputs:",
                         "a: 0",
                         "b: 1",
                         "c: 2",
                         "d: 3",
                         "",
                         "Outputs:",
                         "output: {a}*{b}/{c} + {d}");
    }

    [TestMethod]
    public void TestComplexDivision() {
      var equations = new ScalarEquations<string>();

      var a =
          equations.CreateScalarInput("a",
                                      equations.CreateScalarConstant(0));
      var b =
          equations.CreateScalarInput("b",
                                      equations.CreateScalarConstant(1));
      var c =
          equations.CreateScalarInput("c",
                                      equations.CreateScalarConstant(2));
      var d =
          equations.CreateScalarInput("d",
                                      equations.CreateScalarConstant(3));

      var output =
          equations.CreateScalarOutput("output",
                                       (a.Add(b)).Divide(c.Subtract(d)));

      this.AssertEquals_(equations,
                         "Inputs:",
                         "a: 0",
                         "b: 1",
                         "c: 2",
                         "d: 3",
                         "",
                         "Outputs:",
                         "output: ({a} + {b})/({c} + -1*{d})");
    }


    private void AssertEquals_<TIdentifier>(
        IScalarEquations<TIdentifier> equations,
        params string[] expectedLines) {
      var sb = new StringBuilder();

      {
        using var os = new StringWriter(sb);
        new ScalarEquationsPrettyPrinter<TIdentifier>().Print(os, equations);
      }

      var actualLines = StringUtil.SplitNewlines(sb.ToString());
      actualLines = actualLines.Take(actualLines.Length - 1).ToArray();

      var expectedText = string.Join('\n', expectedLines);
      var actualText = string.Join('\n', actualLines);

      Assert.AreEqual(expectedText, actualText);
    }
  }
}