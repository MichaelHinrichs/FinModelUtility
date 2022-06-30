using System.IO;
using System.Linq;
using System.Text;

using fin.model;
using fin.util.strings;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.language.equations.fixedFunction {
  [TestClass]
  public class FixedFunctionEquationsGlslPrinterTest {
    [TestMethod]
    public void TestScalarColor() {
      var equations = new FixedFunctionEquations<FixedFunctionSource>();

      var sc = equations.CreateScalarConstant(1);
      var colSc =
          equations.CreateColorOutput(FixedFunctionSource.OUTPUT_COLOR, equations.CreateColor(sc));

      this.AssertEquals_(equations,
                         @"# version 130

out vec4 fragColor;

void main() {
  fragColor = rgb(1.0,1.0,1.0);

  if (fragColor.a < .95) {
    discard;
  }
}"
      );
    }

    /*[TestMethod]
    public void TestColorMath() {
      var equations = new FixedFunctionEquations<string>();

      var colRgb1 =
          equations.CreateColorInput("colRgb1",
                                     equations.CreateColorConstant(1, 2, 3));
      var colRgb2 =
          equations.CreateColorInput("colRgb2",
                                     equations.CreateColorConstant(2, 3, 4));
      var colI1 =
          equations.CreateColorInput("colI1",
                                     equations.CreateColorConstant(1));
      var colI2 =
          equations.CreateColorInput("colI2",
                                     equations.CreateColorConstant(2));

      var colOutput =
          equations.CreateColorOutput("colOutput",
                                      (colRgb1.Add(colRgb2)).Divide(
                                          colI1.Subtract(colI2)));

      this.AssertEquals_(equations,
                         "Scalar inputs:",
                         "",
                         "Color inputs:",
                         "colRgb1: rgb<1,2,3>",
                         "colRgb2: rgb<2,3,4>",
                         "colI1: i<1>",
                         "colI2: i<2>",
                         "",
                         "",
                         "Scalar outputs:",
                         "",
                         "Color outputs:",
                         "colOutput: (<colRgb1> + <colRgb2>)/(<colI1> + i<-1>*<colI2>)"
      );
    }


    [TestMethod]
    public void TestColorSwizzleOut() {
      var equations = new FixedFunctionEquations<string>();

      var colRgb1 =
          equations.CreateColorInput("colRgb1",
                                     equations.CreateColorConstant(1, 2, 3));
      var colRgb2 =
          equations.CreateColorInput("colRgb2",
                                     equations.CreateColorConstant(2, 3, 4));
      var colI1 =
          equations.CreateColorInput("colI1",
                                     equations.CreateColorConstant(1));
      var colI2 =
          equations.CreateColorInput("colI2",
                                     equations.CreateColorConstant(2));

      var scR1 = equations.CreateScalarOutput("scR1", colI1.R);
      var scR2 = equations.CreateScalarOutput("scR2", colRgb1.R);

      var colOutputExp = (colRgb1.Add(colRgb2)).Divide(
          colI1.Subtract(colI2));
      var colOutput =
          equations.CreateColorOutput("colOutput",
                                      colOutputExp);

      var scBExp = equations.CreateScalarOutput("scBExp", colOutputExp.B);
      var scBVar = equations.CreateScalarOutput("scBVar", colOutput.B);

      this.AssertEquals_(equations,
                         "Scalar inputs:",
                         "",
                         "Color inputs:",
                         "colRgb1: rgb<1,2,3>",
                         "colRgb2: rgb<2,3,4>",
                         "colI1: i<1>",
                         "colI2: i<2>",
                         "",
                         "",
                         "Scalar outputs:",
                         "scR1: <colI1>.R",
                         "scR2: <colRgb1>.R",
                         "scBExp: (<colRgb1>.B + <colRgb2>.B)/(<colI1>.B + -1*<colI2>.B)",
                         "scBVar: <colOutput>.B",
                         "",
                         "Color outputs:",
                         "colOutput: (<colRgb1> + <colRgb2>)/(<colI1> + i<-1>*<colI2>)"
      );
    }*/


    [TestMethod]
    public void TestColorSwizzleIn() {
      var equations = new FixedFunctionEquations<FixedFunctionSource>();

      var colRgb =
          equations.CreateColorInput(FixedFunctionSource.VERTEX_COLOR_0,
                                     equations.CreateColorConstant(1, 2, 3));
      var colGbr =
          equations.CreateColorOutput(FixedFunctionSource.OUTPUT_COLOR,
                                      equations.CreateColor(
                                          colRgb.G,
                                          colRgb.B,
                                          colRgb.R));

      this.AssertEquals_(equations,
                         "Scalar inputs:",
                         "",
                         "Color inputs:",
                         "colRgb: rgb<1,2,3>",
                         "",
                         "",
                         "Scalar outputs:",
                         "",
                         "Color outputs:",
                         "colGbr: rgb<<colRgb>.G,<colRgb>.B,<colRgb>.R>"
      );
    }

    private void AssertEquals_(
        IFixedFunctionEquations<FixedFunctionSource> equations,
        params string[] expectedLines) {
      var actualText =
          new FixedFunctionEquationsGlslPrinter(null).Print(
              null);

      var actualLines = StringUtil.SplitNewlines(actualText);
      actualLines = actualLines.Take(actualLines.Length - 1).ToArray();

      var expectedText = string.Join('\n', expectedLines);
      actualText = string.Join('\n', actualLines);

      Assert.AreEqual(expectedText, actualText);
    }
  }
}