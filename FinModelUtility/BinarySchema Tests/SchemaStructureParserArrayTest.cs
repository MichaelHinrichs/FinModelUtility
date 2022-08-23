using NUnit.Framework;

namespace schema {
  public partial class SchemaStructureParserTest {
    public class Array {
      [Test]
      public void TestMutableArrayWithoutLength() {
        var structure = SchemaTestUtil.Parse(@"
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