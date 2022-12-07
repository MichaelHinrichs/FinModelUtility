using NUnit.Framework;


namespace schema {
  public class BooleanDiagnosticTests {
    [Test]
    public void TestBooleanWithoutAltFormat() {
      var structure = SchemaTestUtil.ParseFirst(@"
namespace foo.bar {
  [BinarySchema]
  public partial class BooleanWrapper {
    public bool field;
  }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.BooleanNeedsIntegerFormat);
    }
  }
}