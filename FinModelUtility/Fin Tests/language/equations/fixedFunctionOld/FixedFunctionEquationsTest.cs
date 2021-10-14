using System.Linq;

using fin.util.strings;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.language.equations.fixedFunctionOld {
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


    [TestMethod]
    public void TestColorSwizzleOut() {
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

      var scR = equations.CreateScalarOutput("scR", colI.R);
      var scA = equations.CreateScalarOutput("scA", colRgb.A);

      var colOutputExp = (colRgb.Add(colRgba)).Divide(
          colI.Subtract(colIa));
      var colOutput =
          equations.CreateColorOutput("colOutput",
                                      colOutputExp);

      var scBExp = equations.CreateScalarOutput("scBExp", colOutputExp.B);
      var scBVar = equations.CreateScalarOutput("scBVar", colOutput.B);

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
                         "scR: <colI>.R",
                         "scA: <colRgb>.A",
                         "scBExp: (<colRgb>.B + <colRgba>.B)/(<colI>.B + -1*<colIa>.B)",
                         "scBVar: <colOutput>.B",
                         "",
                         "Color outputs:",
                         "colOutput: (<colRgb> + <colRgba>)/(<colI> + ia<-1,-1>*<colIa>)"
      );
    }


    [TestMethod]
    public void TestColorSwizzleIn() {
      var equations = new FixedFunctionEquations<string>();

      var colRgba =
          equations.CreateColorInput("colRgba",
                                     equations.CreateColorConstant(1, 2, 3, 4));
      var colArgb =
          equations.CreateColorOutput("colArgb",
                                      equations.CreateColor(
                                          colRgba.A,
                                          colRgba.R,
                                          colRgba.G,
                                          colRgba.B));

      this.AssertEquals_(equations,
                         "Scalar inputs:",
                         "",
                         "Color inputs:",
                         "colRgba: rgba<1,2,3,4>",
                         "",
                         "",
                         "Scalar outputs:",
                         "",
                         "Color outputs:",
                         "colArgb: rgba<<colRgba>.A,<colRgba>.R,<colRgba>.G,<colRgba>.B>"
      );
    }

    private void AssertEquals_<TIdentifier>(
        IFixedFunctionEquations<TIdentifier> equations,
        params string[] expectedLines) {
      var actualText =
          new FixedFunctionEquationsPrettyPrinter<TIdentifier>().Print(
              equations);

      var actualLines = StringUtil.SplitNewlines(actualText);
      actualLines = actualLines.Take(actualLines.Length - 1).ToArray();

      var expectedText = string.Join('\n', expectedLines);
      actualText = string.Join('\n', actualLines);

      Assert.AreEqual(expectedText, actualText);
    }
  }
}