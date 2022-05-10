using NUnit.Framework;


namespace schema.text {
  internal class IfBooleanDiagnosticsTests {
    [Test]
    public void TestIfBooleanNonReference() {
      var structure = SchemaTestUtil.Parse(@"
using schema;
namespace foo.bar {
  [Schema]
  public partial class BooleanWrapper : IBiSerializable {
    [IfBoolean(IntType.BYTE)]
    public int field;
  }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.IfBooleanNeedsNullable);
    }

    [Test]
    public void TestIfBooleanNonNullable() {
      var structure = SchemaTestUtil.Parse(@"
using schema;
namespace foo.bar {
  [Schema]
  public partial class BooleanWrapper : IBiSerializable {
    [IfBoolean(IntType.BYTE)]
    public A field;
  }

  [Schema]
  public partial class A : IBiSerializable {
  }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.IfBooleanNeedsNullable);
    }
  }
}