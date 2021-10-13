using System.IO;
using System.Linq;
using System.Text;

using fin.util.strings;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.language.equations.fixedFunction {
  [TestClass]
  public class FixedFunctionEquationsTest {
    [TestMethod]
    public void TestInOutScalars() {
      var equations = new FixedFunctionEquations<string>();

      var fooIn =
          equations.CreateScalarInput("fooIn",
                                      equations.CreateScalarConstant(123));
      var fooOut = equations.CreateScalarOutput("fooOut", fooIn);

      this.AssertEquals_(equations,
                         "Scalar inputs:",
                         "fooIn: 123",
                         "",
                         "Color inputs:",
                         "",
                         "",
                         "Scalar outputs:",
                         "fooOut: {fooIn}",
                         "",
                         "Color outputs:"
      );
    }


    [TestMethod]
    public void TestInOut() {
      var equations = new FixedFunctionEquations<string>();

      var scIn =
          equations.CreateScalarInput("scIn",
                                      equations.CreateScalarConstant(123));
      var scOut = equations.CreateScalarOutput("scOut", scIn);

      var colSc =
          equations.CreateColorOutput("colSc", equations.CreateColor(scIn));

      this.AssertEquals_(equations,
                         "Scalar inputs:",
                         "scIn: 123",
                         "",
                         "Color inputs:",
                         "",
                         "",
                         "Scalar outputs:",
                         "scOut: {scIn}",
                         "",
                         "Color outputs:",
                         "colSc: i<{scIn}>"
      );
    }

    [TestMethod]
    public void TestColorMath() {
      var equations = new FixedFunctionEquations<string>();

      var colRgb =
          equations.CreateColorInput("colRgb",
                                     equations.CreateColorConstant(1, 2, 3));
      var colRgba =
          equations.CreateColorInput("colRgba",
                                     equations.CreateColorConstant(1, 2, 3, 4));
      var colI =
          equations.CreateColorInput("colI",
                                     equations.CreateColorConstant(1));
      var colIa =
          equations.CreateColorInput("colIa",
                                     equations.CreateColorConstant(1, 2));

      var colOutput =
          equations.CreateColorOutput("colOutput",
                                      (colRgb.Add(colRgba)).Divide(
                                          colI.Subtract(colIa)));

      this.AssertEquals_(equations,
                         "Scalar inputs:",
                         "",
                         "Color inputs:",
                         "colRgb: rgb<1,2,3>",
                         "colRgba: rgba<1,2,3,4>",
                         "colI: i<1>",
                         "colIa: ia<1,2>",
                         "",
                         "",
                         "Scalar outputs:",
                         "",
                         "Color outputs:",
                         "colOutput: (<colRgb> + <colRgba>)/(<colI> + ia<-1,-1>*<colIa>)"
      );
    }


    private void AssertEquals_<TIdentifier>(
        IFixedFunctionEquations<TIdentifier> equations,
        params string[] expectedLines) {
      var sb = new StringBuilder();

      {
        using var os = new StringWriter(sb);
        new FixedFunctionEquationsPrettyPrinter<TIdentifier>().Print(
            os,
            equations);
      }

      var actualLines = StringUtil.SplitNewlines(sb.ToString());
      actualLines = actualLines.Take(actualLines.Length - 1).ToArray();

      var expectedText = string.Join('\n', expectedLines);
      var actualText = string.Join('\n', actualLines);

      Assert.AreEqual(expectedText, actualText);
    }
  }
}