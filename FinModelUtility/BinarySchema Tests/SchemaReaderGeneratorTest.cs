using Microsoft.CodeAnalysis;

using NUnit.Framework;

namespace schema.text {
  public class SchemaReaderGeneratorTest {
    [SetUp]
    public void Setup() {}

    [Test]
    public void TestByte() {
      var structure = SchemaTestUtil.Parse(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public byte field;
  }
}");

      Assert.IsEmpty(structure.Diagnostics);

      Assert.AreEqual("bar", structure.TypeSymbol.ContainingNamespace.Name);
      Assert.AreEqual("ByteWrapper", structure.TypeSymbol.Name);

      Assert.AreEqual(1, structure.Fields.Count);

      var field = structure.Fields[0];
      Assert.AreEqual(SpecialType.System_Byte, field.TypeSymbol.SpecialType);
      Assert.AreEqual(SchemaPrimitiveType.BYTE, field.PrimitiveType);
      Assert.AreEqual("field", field.Name);

      Assert.AreEqual(true, field.IsPrimitive);
      Assert.AreEqual(false, field.IsPrimitiveConst);
      Assert.AreEqual(false, field.IsArray);
      Assert.AreEqual(false, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);
    }

    [Test]
    public void TestSByte() {
      var structure = SchemaTestUtil.Parse(@"
using schema;

namespace foo.bar {
  [Schema]
  public class SByteWrapper {
    public sbyte field;
  }
}");

      Assert.IsEmpty(structure.Diagnostics);

      Assert.AreEqual("bar", structure.TypeSymbol.ContainingNamespace.Name);
      Assert.AreEqual("SByteWrapper", structure.TypeSymbol.Name);

      Assert.AreEqual(1, structure.Fields.Count);

      var field = structure.Fields[0];
      Assert.AreEqual(SpecialType.System_SByte, field.TypeSymbol.SpecialType);
      Assert.AreEqual(SchemaPrimitiveType.SBYTE, field.PrimitiveType);
      Assert.AreEqual("field", field.Name);

      Assert.AreEqual(true, field.IsPrimitive);
      Assert.AreEqual(false, field.IsPrimitiveConst);
      Assert.AreEqual(false, field.IsArray);
      Assert.AreEqual(false, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);

      Assert.AreEqual(false, field.UseAltFormat);
      Assert.AreEqual(SchemaNumberType.UNDEFINED, field.AltFormat);
    }

    [Test]
    public void TestInt16() {
      var structure = SchemaTestUtil.Parse(@"
using schema;

namespace foo.bar {
  [Schema]
  public class Int16Wrapper {
    public short field;
  }
}");

      Assert.IsEmpty(structure.Diagnostics);

      Assert.AreEqual("bar", structure.TypeSymbol.ContainingNamespace.Name);
      Assert.AreEqual("Int16Wrapper", structure.TypeSymbol.Name);

      Assert.AreEqual(1, structure.Fields.Count);

      var field = structure.Fields[0];
      Assert.AreEqual(SpecialType.System_Int16, field.TypeSymbol.SpecialType);
      Assert.AreEqual(SchemaPrimitiveType.INT16, field.PrimitiveType);
      Assert.AreEqual("field", field.Name);

      Assert.AreEqual(true, field.IsPrimitive);
      Assert.AreEqual(false, field.IsPrimitiveConst);
      Assert.AreEqual(false, field.IsArray);
      Assert.AreEqual(false, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);

      Assert.AreEqual(false, field.UseAltFormat);
      Assert.AreEqual(SchemaNumberType.UNDEFINED, field.AltFormat);
    }

    [Test]
    public void TestEnum() {
      var structure = SchemaTestUtil.Parse(@"
using schema;

namespace foo.bar {
  public enum ValueType {
    A,
    B,
    C
  }

  [Schema]
  public class EnumWrapper {
    [Format(SchemaNumberType.UINT16)]
    public ValueType field;
  }
}");

      Assert.IsEmpty(structure.Diagnostics);

      Assert.AreEqual("bar", structure.TypeSymbol.ContainingNamespace.Name);
      Assert.AreEqual("EnumWrapper", structure.TypeSymbol.Name);

      Assert.AreEqual(1, structure.Fields.Count);

      var field = structure.Fields[0];
      Assert.AreEqual("ValueType", field.TypeSymbol.Name);
      Assert.AreEqual(SchemaPrimitiveType.ENUM, field.PrimitiveType);
      Assert.AreEqual("field", field.Name);

      Assert.AreEqual(true, field.IsPrimitive);
      Assert.AreEqual(false, field.IsPrimitiveConst);
      Assert.AreEqual(false, field.IsArray);
      Assert.AreEqual(false, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);

      Assert.AreEqual(true, field.UseAltFormat);
      Assert.AreEqual(SchemaNumberType.UINT16, field.AltFormat);
    }

    [Test]
    public void TestEnumWithoutFormat() {
      var structure = SchemaTestUtil.Parse(@"
namespace foo.bar {
  public enum ValueType {
    A,
    B,
    C
  }

  [Schema]
  public class EnumWrapper {
    public ValueType field;
  }
}");

      Assert.AreEqual(1, structure.Diagnostics.Count);
      Assert.AreEqual(Rules.EnumNeedsFormat,
                      structure.Diagnostics[0].Descriptor);
    }

    [Test]
    public void TestConstArray() {
      var structure = SchemaTestUtil.Parse(@"
namespace foo.bar {
  [Schema]
  public class ArrayWrapper {
    public readonly int[] field;
  }
}");

      Assert.IsEmpty(structure.Diagnostics);

      var field = structure.Fields[0];
      Assert.AreEqual(TypeKind.Array, field.TypeSymbol.TypeKind);
      Assert.AreEqual(SchemaPrimitiveType.INT32, field.PrimitiveType);
      Assert.AreEqual("field", field.Name);

      Assert.AreEqual(false, field.IsPrimitive);
      Assert.AreEqual(false, field.IsPrimitiveConst);
      Assert.AreEqual(true, field.IsArray);
      Assert.AreEqual(true, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);

      Assert.AreEqual(false, field.UseAltFormat);
      Assert.AreEqual(SchemaNumberType.UNDEFINED, field.AltFormat);
    }

    [Test]
    public void TestField() {
      var structure = SchemaTestUtil.Parse(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public byte field;
  }
}");

      Assert.IsEmpty(structure.Diagnostics);

      Assert.AreEqual("bar", structure.TypeSymbol.ContainingNamespace.Name);
      Assert.AreEqual("ByteWrapper", structure.TypeSymbol.Name);

      Assert.AreEqual(1, structure.Fields.Count);

      var field = structure.Fields[0];
      Assert.AreEqual(SpecialType.System_Byte, field.TypeSymbol.SpecialType);
      Assert.AreEqual(SchemaPrimitiveType.BYTE, field.PrimitiveType);
      Assert.AreEqual("field", field.Name);

      Assert.AreEqual(true, field.IsPrimitive);
      Assert.AreEqual(false, field.IsPrimitiveConst);
      Assert.AreEqual(false, field.IsArray);
      Assert.AreEqual(false, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);
    }

    [Test]
    public void TestProperty() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public byte field { get; set; }
  }
}",
                            @"using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Read(EndianBinaryReader er) {
      this.field = er.ReadByte();
    }
  }
}
");
    }

    [Test]
    public void TestReadonlyPrimitiveField() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public readonly byte field;
  }
}",
                            @"using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Read(EndianBinaryReader er) {
      er.AssertByte(this.field);
    }
  }
}
");
    }

    [Test]
    public void TestReadonlyPrimitiveProperty() {
      this.AssertGenerated_(@"
using schema;

namespace foo.bar {
  [Schema]
  public partial class ByteWrapper {
    public byte Field { get; }
  }
}",
                            @"using System.IO;
namespace foo.bar {
  public partial class ByteWrapper {
    public void Read(EndianBinaryReader er) {
      er.AssertByte(this.Field);
    }
  }
}
");
    }

    [Test]
    public void TestEverything() {
      this.AssertGenerated_(@"
using schema;

namespace foo {
  namespace bar {
    public enum IntEnum : int {
      A, B, C
    }

    [Schema]
    public class EverythingWrapper {
      public byte byteField;
      public sbyte sbyteProperty { get; set; }
      public readonly short constShortField;
      public ushort constUshortProperty { get; }
      [Format(SchemaNumberType.INT32)]
      public IntEnum intField;
      [Format(SchemaNumberType.INT32)]
      public readonly IntEnum constIntField;
    }
  }
}",
                            @"using System.IO;
namespace foo.bar {
  public partial class EverythingWrapper {
    public void Read(EndianBinaryReader er) {
      this.byteField = er.ReadByte();
      this.sbyteProperty = er.ReadSByte();
      er.AssertInt16(this.constShortField);
      er.AssertUInt16(this.constUshortProperty);
      this.intField = (foo.bar.IntEnum) er.ReadInt32();
      er.AssertInt32((int) this.constIntField);
    }
  }
}
");
    }

    private void AssertGenerated_(string src, string expectedGenerated) {
      var structure = SchemaTestUtil.Parse(src);
      Assert.IsEmpty(structure.Diagnostics);

      var actualGenerated = new SchemaReaderGenerator().Generate(structure);
      Assert.AreEqual(expectedGenerated, actualGenerated);
    }
  }
}