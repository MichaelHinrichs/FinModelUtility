using System;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

using schema;

#pragma warning disable CS8604

namespace schema {
  public class SchemaStructureParserTest {
    [SetUp]
    public void Setup() {}

    [Test]
    public void TestByte() {
      var structure = this.Parse_(@"
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
      var structure = this.Parse_(@"
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
      var structure = this.Parse_(@"
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
      var structure = this.Parse_(@"
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
      var structure = this.Parse_(@"
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
      var structure = this.Parse_(@"
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
      var structure = this.Parse_(@"
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
      var structure = this.Parse_(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public byte Field { get; set; }
  }
}");

      Assert.IsEmpty(structure.Diagnostics);

      Assert.AreEqual("bar", structure.TypeSymbol.ContainingNamespace.Name);
      Assert.AreEqual("ByteWrapper", structure.TypeSymbol.Name);

      Assert.AreEqual(1, structure.Fields.Count);

      var field = structure.Fields[0];
      Assert.AreEqual(SpecialType.System_Byte, field.TypeSymbol.SpecialType);
      Assert.AreEqual(SchemaPrimitiveType.BYTE, field.PrimitiveType);
      Assert.AreEqual("Field", field.Name);

      Assert.AreEqual(true, field.IsPrimitive);
      Assert.AreEqual(false, field.IsPrimitiveConst);
      Assert.AreEqual(false, field.IsArray);
      Assert.AreEqual(false, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);
    }

    [Test]
    public void TestReadonlyPrimitiveField() {
      var structure = this.Parse_(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public readonly byte field;
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
      Assert.AreEqual(true, field.IsPrimitiveConst);
      Assert.AreEqual(false, field.IsArray);
      Assert.AreEqual(false, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);
    }

    [Test]
    public void TestReadonlyPrimitiveProperty() {
      var structure = this.Parse_(@"
using schema;

namespace foo.bar {
  [Schema]
  public class ByteWrapper {
    public byte Field { get; }
  }
}");

      Assert.IsEmpty(structure.Diagnostics);

      Assert.AreEqual("bar", structure.TypeSymbol.ContainingNamespace.Name);
      Assert.AreEqual("ByteWrapper", structure.TypeSymbol.Name);

      Assert.AreEqual(1, structure.Fields.Count);

      var field = structure.Fields[0];
      Assert.AreEqual(SpecialType.System_Byte, field.TypeSymbol.SpecialType);
      Assert.AreEqual(SchemaPrimitiveType.BYTE, field.PrimitiveType);
      Assert.AreEqual("Field", field.Name);

      Assert.AreEqual(true, field.IsPrimitive);
      Assert.AreEqual(true, field.IsPrimitiveConst);
      Assert.AreEqual(false, field.IsArray);
      Assert.AreEqual(false, field.HasConstLength);
      Assert.AreEqual(null, field.LengthField);
    }

    private ISchemaStructure Parse_(string src) {
      var syntaxTree = CSharpSyntaxTree.ParseText(src);

      var references =
          ((string) AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
          .Split(Path.PathSeparator)
          .Select(path => MetadataReference.CreateFromFile(path));

      var compilation =
          CSharpCompilation.Create("test")
                           .AddReferences(references)
                           .AddSyntaxTrees(syntaxTree);
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var classIndex = src.IndexOf("class");
      var classNameIndex = src.IndexOf(' ', classIndex) + 1;
      var classNameLength = src.IndexOf(' ', classNameIndex) - classNameIndex;
      var typeName = src.Substring(classNameIndex, classNameLength);

      var typeNode = syntaxTree.GetRoot()
                               .DescendantTokens()
                               .Single(t => t.Text == typeName)
                               .Parent;

      var symbol = semanticModel.GetDeclaredSymbol(typeNode);
      var namedTypeSymbol = symbol as INamedTypeSymbol;

      return new SchemaStructureParser().ParseStructure(namedTypeSymbol);
    }
  }

  [Schema]
  public class NestedClass {}
}