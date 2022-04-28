using System;

using Microsoft.CodeAnalysis;

namespace schema.text {
  public class SchemaReaderGenerator {
    private readonly Type schemaAttributeType_ = typeof(SchemaAttribute);
    private readonly SchemaStructureParser parser_ = new();

    public string Generate(ISchemaStructure structure) {
      var typeSymbol = structure.TypeSymbol;

      var typeName = typeSymbol.Name;
      var typeNamespace = SymbolTypeUtil.MergeContainingNamespaces(typeSymbol);

      var isTypeClass = typeSymbol.TypeKind == TypeKind.Class;
      var isTypeAbstract = typeSymbol.IsAbstract;

      var declaringTypes =
          SymbolTypeUtil.GetDeclaringTypesDownward(typeSymbol);
      var symbolType = (isTypeAbstract ? "abstract " : "") +
                       "partial " +
                       (isTypeClass ? "class" : "struct");

      var cbsb = new CurlyBracketStringBuilder();
      cbsb.WriteLine("using System;")
          .WriteLine("using System.IO;");

      // TODO: Handle fancier cases here
      cbsb.EnterBlock($"namespace {typeNamespace}");
      foreach (var declaringType in declaringTypes) {
        cbsb.EnterBlock(
            $"{SymbolTypeUtil.GetSymbolQualifiers(declaringType)} {declaringType.Name}");
      }
      cbsb.EnterBlock(
          $"{SymbolTypeUtil.GetSymbolQualifiers(typeSymbol)} {typeName}");

      cbsb.EnterBlock("public void Read(EndianBinaryReader er)");
      foreach (var member in structure.Members) {
        SchemaReaderGenerator.ReadMember_(cbsb, member);
      }
      cbsb.ExitBlock();

      // TODO: Handle fancier cases here

      // type
      cbsb.ExitBlock();

      // parent types
      foreach (var declaringType in declaringTypes) {
        cbsb.ExitBlock();
      }

      // namespace
      cbsb.ExitBlock();

      var generatedCode = cbsb.ToString();
      return generatedCode;
    }

    private static void ReadMember_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var memberType = member.MemberType;
      switch (memberType) {
        case IPrimitiveMemberType: {
          SchemaReaderGenerator.ReadPrimitive_(cbsb, member);
          return;
        }
        case IStringType: {
          SchemaReaderGenerator.ReadString_(cbsb, member);
          return;
        }
        case IStructureMemberType: {
          SchemaReaderGenerator.ReadStructure_(cbsb, member);
          return;
        }
        case ISequenceMemberType: {
          SchemaReaderGenerator.ReadArray_(cbsb, member);
          return;
        }
      }

      // Anything that makes it down here probably isn't meant to be read.
      throw new NotImplementedException();
    }

    private static void ReadPrimitive_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var primitiveType = member.MemberType as IPrimitiveMemberType;

      var readType = SchemaReaderGenerator.GetPrimitiveLabel_(
          primitiveType.UseAltFormat
              ? SchemaReaderGenerator.ConvertNumberToPrimitive_(
                  primitiveType.AltFormat)
              : primitiveType.PrimitiveType);
      if (!primitiveType.IsConst) {
        var castText = "";
        if (primitiveType.UseAltFormat) {
          var castType =
              primitiveType.PrimitiveType == SchemaPrimitiveType.ENUM
                  ? SymbolTypeUtil.GetQualifiedName(
                      primitiveType.TypeSymbol)
                  : primitiveType.TypeSymbol.Name;
          castText = $"({castType}) ";
        }
        cbsb.WriteLine(
            $"this.{member.Name} = {castText}er.Read{readType}();");
      } else {
        var castText = "";
        if (primitiveType.UseAltFormat) {
          var castType =
              SchemaReaderGenerator.GetTypeName(primitiveType.AltFormat);
          castText = $"({castType}) ";
        }
        cbsb.WriteLine($"er.Assert{readType}({castText}this.{member.Name});");
      }
    }

    private static void ReadString_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var stringType = member.MemberType as IStringType;

      if (stringType.IsConst) {
        if (!stringType.IsNullTerminated) {
          cbsb.WriteLine($"er.AssertString(this.{member.Name});");
        } else {
          cbsb.WriteLine($"er.AssertStringNT(this.{member.Name});");
        }
        return;
      }

      // TODO: Handle more cases
      throw new NotImplementedException();
    }

    private static void ReadStructure_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      // TODO: Do value types need to be handled differently?
      cbsb.WriteLine($"this.{member.Name}.Read(er);");
    }

    private static void ReadArray_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var arrayType = member.MemberType as ISequenceMemberType;
      if (arrayType.LengthType != SequenceLengthType.CONST) {
        var isImmediate =
            arrayType.LengthType == SequenceLengthType.IMMEDIATE_VALUE;

        var lengthName =
            isImmediate ? "c" : $"this.{arrayType.LengthMember!.Name}";

        if (isImmediate) {
          var readType = SchemaReaderGenerator.GetIntLabel_(
              arrayType.ImmediateLengthType);
          cbsb.EnterBlock()
              .WriteLine($"var {lengthName} = er.Read{readType}();");
        }

        cbsb.EnterBlock($"if ({lengthName} < 0)")
            .WriteLine(
                $"throw new Exception(\"Expected length to be nonnegative!\");")
            .ExitBlock();

        var qualifiedElementName =
            SymbolTypeUtil.GetQualifiedName(arrayType.ElementType.TypeSymbol);
        var hasReferenceElements =
            arrayType.ElementType is IStructureMemberType {
                IsReferenceType: true
            };

        // TODO: Handle readonly lists, can't be expanded like this!
        if (arrayType.SequenceType == SequenceType.LIST) {
          cbsb.EnterBlock($"if (this.{member.Name}.Count < {lengthName})");
          if (hasReferenceElements) {
            cbsb.WriteLine(
                $"this.{member.Name}.Add(new {qualifiedElementName}());");
          } else {
            cbsb.WriteLine($"this.{member.Name}.Add(default);");
          }
          cbsb.ExitBlock();

          cbsb.EnterBlock(
                  $"while (this.{member.Name}.Count > {lengthName})")
              .WriteLine($"this.{member.Name}.RemoveAt(0);")
              .ExitBlock();
        } else {
          cbsb.WriteLine(
              $"this.{member.Name} = new {qualifiedElementName}[{lengthName}];");

          if (hasReferenceElements) {
            cbsb.EnterBlock($"for (var i = 0; i < {lengthName}; ++i)")
                .WriteLine($"this.{member.Name}[i] = new {qualifiedElementName}();")
                .ExitBlock();
          }
        }

        if (isImmediate) {
          cbsb.ExitBlock();
        }
      }

      SchemaReaderGenerator.ReadIntoArray_(cbsb, member);
    }

    private static void ReadIntoArray_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var arrayType = member.MemberType as ISequenceMemberType;

      var elementType = arrayType.ElementType;
      if (elementType is IPrimitiveMemberType primitiveElementType) {
        // Primitives that don't need to be cast are the easiest to read.
        if (!primitiveElementType.UseAltFormat) {
          var label =
              SchemaReaderGenerator.GetPrimitiveLabel_(
                  primitiveElementType.PrimitiveType);
          if (!primitiveElementType.IsConst) {
            cbsb.WriteLine($"er.Read{label}s(this.{member.Name});");
          } else {
            cbsb.WriteLine($"er.Assert{label}s(this.{member.Name});");
          }
          return;
        }

        // Primitives that *do* need to be cast have to be read individually.
        var readType = SchemaReaderGenerator.GetPrimitiveLabel_(
            SchemaReaderGenerator.ConvertNumberToPrimitive_(
                primitiveElementType.AltFormat));
        if (!primitiveElementType.IsConst) {
          var arrayLengthName = arrayType.SequenceType == SequenceType.ARRAY
                                    ? "Length"
                                    : "Count";
          var castType =
              primitiveElementType.PrimitiveType == SchemaPrimitiveType.ENUM
                  ? SymbolTypeUtil.GetQualifiedName(
                      primitiveElementType.TypeSymbol)
                  : primitiveElementType.TypeSymbol.Name;
          cbsb.EnterBlock(
                  $"for (var i = 0; i < this.{member.Name}.{arrayLengthName}; ++i)")
              .WriteLine(
                  $"this.{member.Name}[i] = ({castType}) er.Read{readType}();")
              .ExitBlock();
        } else {
          var castType =
              SchemaReaderGenerator.GetTypeName(primitiveElementType.AltFormat);
          cbsb.EnterBlock($"foreach (var e in this.{member.Name})")
              .WriteLine($"er.Assert{readType}(({castType}) e);")
              .ExitBlock();
        }
        return;
      }

      if (elementType is IStructureMemberType structureElementType) {
        //if (structureElementType.IsReferenceType) {
        cbsb.EnterBlock($"foreach (var e in this.{member.Name})")
            .WriteLine("e.Read(er);")
            .ExitBlock();
        // TODO: Do value types need to be read like below?
        /*}
        // Value types (mainly structs) have to be pulled out, read, then put
        // back in.
        else {
          var arrayLengthName = arrayType.SequenceType == SequenceType.ARRAY
                                    ? "Length"
                                    : "Count";
          cbsb.EnterBlock(
                  $"for (var i = 0; i < this.{member.Name}.{arrayLengthName}; ++i)")
              .WriteLine($"var e = this.{member.Name}[i];")
              .WriteLine("e.Read(er);")
              .WriteLine($"this.{member.Name}[i] = e;")
              .ExitBlock();
        }*/
        return;
      }

// Anything that makes it down here probably isn't meant to be read.
      throw new NotImplementedException();
    }

    private static string GetPrimitiveLabel_(SchemaPrimitiveType type)
      => type switch {
          SchemaPrimitiveType.CHAR => "Char",
          SchemaPrimitiveType.SBYTE => "SByte",
          SchemaPrimitiveType.BYTE => "Byte",
          SchemaPrimitiveType.INT16 => "Int16",
          SchemaPrimitiveType.UINT16 => "UInt16",
          SchemaPrimitiveType.INT32 => "Int32",
          SchemaPrimitiveType.UINT32 => "UInt32",
          SchemaPrimitiveType.INT64 => "Int64",
          SchemaPrimitiveType.UINT64 => "UInt64",
          SchemaPrimitiveType.SINGLE => "Single",
          SchemaPrimitiveType.DOUBLE => "Double",
          SchemaPrimitiveType.UN8 => "Un8",
          SchemaPrimitiveType.SN16 => "Sn16",
          SchemaPrimitiveType.UN16 => "Un16",
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    private static string GetIntLabel_(IntType type)
      => type switch {
          IntType.SBYTE => "SByte",
          IntType.BYTE => "Byte",
          IntType.INT16 => "Int16",
          IntType.UINT16 => "UInt16",
          IntType.INT32 => "Int32",
          IntType.UINT32 => "UInt32",
          IntType.INT64 => "Int64",
          IntType.UINT64 => "UInt64",
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    public static string GetTypeName(SchemaNumberType type)
      => type switch {
          SchemaNumberType.SBYTE => "sbyte",
          SchemaNumberType.BYTE => "byte",
          SchemaNumberType.INT16 => "short",
          SchemaNumberType.UINT16 => "ushort",
          SchemaNumberType.INT32 => "int",
          SchemaNumberType.UINT32 => "uint",
          SchemaNumberType.INT64 => "long",
          SchemaNumberType.UINT64 => "ulong",
          SchemaNumberType.SINGLE => "float",
          SchemaNumberType.DOUBLE => "double",
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    private static SchemaPrimitiveType ConvertNumberToPrimitive_(
        SchemaNumberType type)
      => type switch {
          SchemaNumberType.SBYTE => SchemaPrimitiveType.SBYTE,
          SchemaNumberType.BYTE => SchemaPrimitiveType.BYTE,
          SchemaNumberType.INT16 => SchemaPrimitiveType.INT16,
          SchemaNumberType.UINT16 => SchemaPrimitiveType.UINT16,
          SchemaNumberType.INT32 => SchemaPrimitiveType.INT32,
          SchemaNumberType.UINT32 => SchemaPrimitiveType.UINT32,
          SchemaNumberType.INT64 => SchemaPrimitiveType.INT64,
          SchemaNumberType.UINT64 => SchemaPrimitiveType.UINT64,
          SchemaNumberType.SINGLE => SchemaPrimitiveType.SINGLE,
          SchemaNumberType.DOUBLE => SchemaPrimitiveType.DOUBLE,
          SchemaNumberType.UN8 => SchemaPrimitiveType.UN8,
          SchemaNumberType.SN16 => SchemaPrimitiveType.SN16,
          SchemaNumberType.UN16 => SchemaPrimitiveType.UN16,
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };
  }
}