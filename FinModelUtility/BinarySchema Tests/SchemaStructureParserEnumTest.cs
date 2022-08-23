using Microsoft.CodeAnalysis;

using NUnit.Framework;

namespace schema {
  public partial class SchemaStructureParserTest {
    public class Enum {
      [Test]
      public void TestEnumWithoutFormat() {
        var structure = SchemaTestUtil.Parse(@"
namespace foo.bar {
  public enum ValueType {
    A,
    B,
    C
  }

  [BinarySchema]
  public partial class EnumWrapper {
    public ValueType field;
  }
}");
        SchemaTestUtil.AssertDiagnostics(structure.Diagnostics, Rules.EnumNeedsFormat);
      }

      [Test]
      public void TestEnumArrayWithoutFormat() {
        var structure = SchemaTestUtil.Parse(@"
namespace foo.bar {
  public enum ValueType {
    A,
    B,
    C
  }

  [BinarySchema]
  public partial class EnumWrapper {
    public readonly ValueType[] field = new ValueType[1];
  }
}");
        SchemaTestUtil.AssertDiagnostics(structure.Diagnostics, Rules.EnumNeedsFormat);
      }
    }
  }
}