using NUnit.Framework;

namespace schema {
  public partial class SchemaStructureParserTests {
    public class Array {
      [Test]
      public void TestMutableArrayWithoutLength() {
        var structure = SchemaTestUtil.ParseFirst(@"
namespace foo.bar {
  [BinarySchema]
  public partial class ArrayWrapper {
    public int[] field;
  }
}");
        SchemaTestUtil.AssertDiagnostics(structure.Diagnostics, Rules.MutableArrayNeedsLengthSource);
      }
    }
  }
}