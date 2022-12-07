using NUnit.Framework;


namespace schema.text {
  internal class IfBooleanDiagnosticsTests {
    [Test]
    public void TestIfBooleanNonReference() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;
namespace foo.bar {
  [BinarySchema]
  public partial class BooleanWrapper : IBiSerializable {
    [IfBoolean(SchemaIntegerType.BYTE)]
    public int field;
  }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.IfBooleanNeedsNullable);
    }

    [Test]
    public void TestIfBooleanNonNullable() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;
namespace foo.bar {
  [BinarySchema]
  public partial class BooleanWrapper : IBiSerializable {
    [IfBoolean(SchemaIntegerType.BYTE)]
    public A field;
  }

  [BinarySchema]
  public partial class A : IBiSerializable {
  }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.IfBooleanNeedsNullable);
    }

    [Test]
    public void TestOutOfOrder() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ByteWrapper : IBiSerializable {
    [IfBoolean(nameof(Field))]
    public int? OtherValue { get; set; }

    [IntegerFormat(SchemaIntegerType.BYTE)]
    private bool Field { get; set; }
  }

  public class A : IBiSerializable { }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.DependentMustComeAfterSource);
    }

    [Test]
    public void TestPublicPropertySource() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ByteWrapper : IBiSerializable {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool Field { get; set; }

    [IfBoolean(nameof(Field))]
    public int? OtherValue { get; set; }
  }

  public class A : IBiSerializable { }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.SourceMustBePrivate);
    }

    [Test]
    public void TestProtectedPropertySource() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ByteWrapper : IBiSerializable {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    protected bool Field { get; set; }

    [IfBoolean(nameof(Field))]
    public int? OtherValue { get; set; }
  }

  public class A : IBiSerializable { }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.SourceMustBePrivate);
    }

    [Test]
    public void TestInternalPropertySource() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ByteWrapper : IBiSerializable {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    internal bool Field { get; set; }

    [IfBoolean(nameof(Field))]
    public int? OtherValue { get; set; }
  }

  public class A : IBiSerializable { }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.SourceMustBePrivate);
    }

    [Test]
    public void TestPublicFieldSource() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ByteWrapper : IBiSerializable {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool Field;

    [IfBoolean(nameof(Field))]
    public int? OtherValue { get; set; }
  }

  public class A : IBiSerializable { }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.SourceMustBePrivate);
    }

    [Test]
    public void TestProtectedFieldSource() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ByteWrapper : IBiSerializable {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    protected bool Field;

    [IfBoolean(nameof(Field))]
    public int? OtherValue { get; set; }
  }

  public class A : IBiSerializable { }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.SourceMustBePrivate);
    }

    [Test]
    public void TestInternalFieldSource() {
      var structure = SchemaTestUtil.ParseFirst(@"
using schema;

namespace foo.bar {
  [BinarySchema]
  public partial class ByteWrapper : IBiSerializable {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    internal bool Field;

    [IfBoolean(nameof(Field))]
    public int? OtherValue { get; set; }
  }

  public class A : IBiSerializable { }
}");
      SchemaTestUtil.AssertDiagnostics(structure.Diagnostics,
                                       Rules.SourceMustBePrivate);
    }
  }
}