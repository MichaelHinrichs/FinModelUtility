using System;

using Microsoft.CodeAnalysis;


namespace schema.text {
  public class SchemaReaderGenerator {
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
        SchemaReaderGenerator.ReadMember_(cbsb, typeSymbol, member);
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
        ITypeSymbol sourceSymbol,
        ISchemaMember member) {
      if (member.IsPosition) {
        if (member.MemberType.IsReadonly) {
          cbsb.WriteLine($"er.AssertPosition(this.{member.Name});");
        } else {
          cbsb.WriteLine($"this.{member.Name} = er.Position;");
        }
        return;
      }

      SchemaReaderGenerator.Align_(cbsb, member);

      // TODO: How to handle both offset & if boolean together?

      var offset = member.Offset;
      if (offset != null) {
        cbsb.EnterBlock()
            .WriteLine("var tempLocation = er.Position;")
            .WriteLine($"er.Position = this.{offset.StartIndexName.Name} + this.{offset.OffsetName.Name};");
      }

      var ifBoolean = member.IfBoolean;
      var immediateIfBoolean =
          ifBoolean?.SourceType == IfBooleanSourceType.IMMEDIATE_VALUE;
      if (immediateIfBoolean) {
        cbsb.EnterBlock();
      }
      if (ifBoolean != null) {
        if (ifBoolean.SourceType == IfBooleanSourceType.IMMEDIATE_VALUE) {
          var booleanNumberType =
              SchemaPrimitiveTypesUtil.ConvertIntToNumber(
                  ifBoolean.ImmediateBooleanType);
          var booleanPrimitiveType =
              SchemaPrimitiveTypesUtil.ConvertNumberToPrimitive(
                  booleanNumberType);
          var booleanPrimitiveLabel =
              SchemaGeneratorUtil.GetPrimitiveLabel(booleanPrimitiveType);
          cbsb.WriteLine($"var b = er.Read{booleanPrimitiveLabel}() != 0;")
              .EnterBlock("if (b)");
        } else {
          cbsb.EnterBlock($"if (this.{ifBoolean.BooleanMember.Name})");
        }

        if (member.MemberType is not IPrimitiveMemberType) {
          cbsb.WriteLine(
              $"this.{member.Name} = new {SymbolTypeUtil.GetQualifiedNameFromCurrentSymbol(sourceSymbol, member.MemberType.TypeSymbol)}();");
        }
      }

      var memberType = member.MemberType;
      switch (memberType) {
        case IPrimitiveMemberType: {
          SchemaReaderGenerator.ReadPrimitive_(cbsb, sourceSymbol, member);
          break;
        }
        case IStringType: {
          SchemaReaderGenerator.ReadString_(cbsb, member);
          break;
        }
        case IStructureMemberType: {
          SchemaReaderGenerator.ReadStructure_(cbsb, member);
          break;
        }
        case ISequenceMemberType: {
          SchemaReaderGenerator.ReadArray_(cbsb, sourceSymbol, member);
          break;
        }
        default: {
          // Anything that makes it down here probably isn't meant to be read.
          throw new NotImplementedException();
        }
      }

      if (ifBoolean != null) {
        cbsb.ExitBlock()
            .EnterBlock("else")
            .WriteLine($"this.{member.Name} = null;")
            .ExitBlock();
        if (immediateIfBoolean) {
          cbsb.ExitBlock();
        }
      }

      if (offset != null) {
        cbsb.WriteLine("er.Position = tempLocation;")
            .ExitBlock();
      }
    }

    private static void Align_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var align = member.Align;
      if (align != 0) {
        cbsb.WriteLine($"er.Align({align});");
      }
    }

    private static void ReadPrimitive_(
        ICurlyBracketStringBuilder cbsb,
        ITypeSymbol sourceSymbol,
        ISchemaMember member) {
      var primitiveType = member.MemberType as IPrimitiveMemberType;

      if (primitiveType.PrimitiveType == SchemaPrimitiveType.BOOLEAN) {
        SchemaReaderGenerator.ReadBoolean_(cbsb, member);
        return;
      }

      var readType = SchemaGeneratorUtil.GetPrimitiveLabel(
          primitiveType.UseAltFormat
              ? SchemaPrimitiveTypesUtil.ConvertNumberToPrimitive(
                  primitiveType.AltFormat)
              : primitiveType.PrimitiveType);

      var needToCast = primitiveType.UseAltFormat &&
                       primitiveType.PrimitiveType !=
                       SchemaPrimitiveTypesUtil.GetUnderlyingPrimitiveType(
                           SchemaPrimitiveTypesUtil.ConvertNumberToPrimitive(
                               primitiveType.AltFormat));

      if (!primitiveType.IsReadonly) {
        var castText = "";
        if (needToCast) {
          var castType =
              primitiveType.PrimitiveType == SchemaPrimitiveType.ENUM
                  ? SymbolTypeUtil.GetQualifiedNameFromCurrentSymbol(
                      sourceSymbol,
                      primitiveType.TypeSymbol)
                  : primitiveType.TypeSymbol.Name;
          castText = $"({castType}) ";
        }
        cbsb.WriteLine(
            $"this.{member.Name} = {castText}er.Read{readType}();");
      } else {
        var castText = "";
        if (needToCast) {
          var castType =
              SchemaGeneratorUtil.GetTypeName(primitiveType.AltFormat);
          castText = $"({castType}) ";
        }
        cbsb.WriteLine($"er.Assert{readType}({castText}this.{member.Name});");
      }
    }

    private static void ReadBoolean_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var primitiveType = member.MemberType as IPrimitiveMemberType;

      var readType = SchemaGeneratorUtil.GetPrimitiveLabel(
          SchemaPrimitiveTypesUtil.ConvertNumberToPrimitive(
              primitiveType.AltFormat));

      if (!primitiveType.IsReadonly) {
        cbsb.WriteLine(
            $"this.{member.Name} = er.Read{readType}() != 0;");
      } else {
        cbsb.WriteLine(
            $"er.Assert{readType}(this.{member.Name} ? 1 : 0);");
      }
    }

    private static void ReadString_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var stringType = member.MemberType as IStringType;

      if (stringType.IsReadonly) {
        if (stringType.IsNullTerminated) {
          cbsb.WriteLine($"er.AssertStringNT(this.{member.Name});");
        } else if (stringType.IsEndianOrdered) {
          cbsb.WriteLine($"er.AssertStringEndian(this.{member.Name});");
        } else {
          cbsb.WriteLine($"er.AssertString(this.{member.Name});");
        }
        return;
      }

      if (stringType.LengthSourceType == StringLengthSourceType.CONST) {
        if (stringType.IsEndianOrdered) {
          cbsb.WriteLine(
              $"this.{member.Name} = er.ReadStringEndian({stringType.ConstLength});");
        } else {
          cbsb.WriteLine(
              $"this.{member.Name} = er.ReadString({stringType.ConstLength});");
        }
        return;
      }

      // TODO: Handle more cases
      throw new NotImplementedException();
    }

    private static void ReadStructure_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var structureMemberType = member.MemberType as IStructureMemberType;

      // TODO: Do value types need to be handled differently?
      var memberName = member.Name;
      if (structureMemberType.IsChild) {
        cbsb.WriteLine($"this.{memberName}.Parent = this;");
      }
      cbsb.WriteLine($"this.{memberName}.Read(er);");
    }

    private static void ReadArray_(
        ICurlyBracketStringBuilder cbsb,
        ITypeSymbol sourceSymbol,
        ISchemaMember member) {
      var arrayType = member.MemberType as ISequenceMemberType;
      if (arrayType.LengthSourceType != SequenceLengthSourceType.CONST) {
        var isImmediate =
            arrayType.LengthSourceType ==
            SequenceLengthSourceType.IMMEDIATE_VALUE;

        var lengthName =
            isImmediate ? "c" : $"this.{arrayType.LengthMember!.Name}";

        if (isImmediate) {
          var readType = SchemaGeneratorUtil.GetIntLabel(
              arrayType.ImmediateLengthType);
          cbsb.EnterBlock()
              .WriteLine($"var {lengthName} = er.Read{readType}();");
        }

        cbsb.EnterBlock($"if ({lengthName} < 0)")
            .WriteLine(
                $"throw new Exception(\"Expected length to be nonnegative!\");")
            .ExitBlock();

        var qualifiedElementName =
            SymbolTypeUtil.GetQualifiedNameFromCurrentSymbol(
                sourceSymbol,
                arrayType.ElementType.TypeSymbol);
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
                .WriteLine(
                    $"this.{member.Name}[i] = new {qualifiedElementName}();")
                .ExitBlock();
          }
        }

        if (isImmediate) {
          cbsb.ExitBlock();
        }
      }

      SchemaReaderGenerator.ReadIntoArray_(cbsb, sourceSymbol, member);
    }

    private static void ReadIntoArray_(
        ICurlyBracketStringBuilder cbsb,
        ITypeSymbol sourceSymbol,
        ISchemaMember member) {
      var arrayType = member.MemberType as ISequenceMemberType;

      var elementType = arrayType.ElementType;
      if (elementType is IPrimitiveMemberType primitiveElementType) {
        // Primitives that don't need to be cast are the easiest to read.
        if (!primitiveElementType.UseAltFormat) {
          var label =
              SchemaGeneratorUtil.GetPrimitiveLabel(
                  primitiveElementType.PrimitiveType);
          if (!primitiveElementType.IsReadonly) {
            cbsb.WriteLine($"er.Read{label}s(this.{member.Name});");
          } else {
            cbsb.WriteLine($"er.Assert{label}s(this.{member.Name});");
          }
          return;
        }

        // Primitives that *do* need to be cast have to be read individually.
        var readType = SchemaGeneratorUtil.GetPrimitiveLabel(
            SchemaPrimitiveTypesUtil.ConvertNumberToPrimitive(
                primitiveElementType.AltFormat));
        if (!primitiveElementType.IsReadonly) {
          var arrayLengthName = arrayType.SequenceType == SequenceType.ARRAY
                                    ? "Length"
                                    : "Count";
          var castType =
              primitiveElementType.PrimitiveType == SchemaPrimitiveType.ENUM
                  ? SymbolTypeUtil.GetQualifiedNameFromCurrentSymbol(
                      sourceSymbol,
                      primitiveElementType.TypeSymbol)
                  : primitiveElementType.TypeSymbol.Name;
          cbsb.EnterBlock(
                  $"for (var i = 0; i < this.{member.Name}.{arrayLengthName}; ++i)")
              .WriteLine(
                  $"this.{member.Name}[i] = ({castType}) er.Read{readType}();")
              .ExitBlock();
        } else {
          var castType =
              SchemaGeneratorUtil.GetTypeName(primitiveElementType.AltFormat);
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
  }
}