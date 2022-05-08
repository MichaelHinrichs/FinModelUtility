using System;

using Microsoft.CodeAnalysis;


namespace schema.text {
  public class SchemaWriterGenerator {
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

      cbsb.EnterBlock("public void Write(EndianBinaryWriter ew)");
      foreach (var member in structure.Members) {
        SchemaWriterGenerator.WriteMember_(cbsb, member);
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

    private static void WriteMember_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var memberType = member.MemberType;
      switch (memberType) {
        case IPrimitiveMemberType: {
          SchemaWriterGenerator.WritePrimitive_(cbsb, member);
          return;
        }
        case IStringType: {
          SchemaWriterGenerator.WriteString_(cbsb, member);
          return;
        }
        case IStructureMemberType: {
          SchemaWriterGenerator.WriteStructure_(cbsb, member);
          return;
        }
        case ISequenceMemberType: {
          SchemaWriterGenerator.WriteArray_(cbsb, member);
          return;
        }
      }

      // Anything that makes it down here probably isn't meant to be read.
      throw new NotImplementedException();
    }

    private static void WritePrimitive_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var primitiveType = member.MemberType as IPrimitiveMemberType;

      var readType = SchemaGeneratorUtil.GetPrimitiveLabel(
          primitiveType.UseAltFormat
              ? SchemaGeneratorUtil.ConvertNumberToPrimitive(
                  primitiveType.AltFormat)
              : primitiveType.PrimitiveType);

      var needToCast = primitiveType.UseAltFormat &&
                       primitiveType.PrimitiveType !=
                       SchemaGeneratorUtil.GetUnderlyingPrimitiveType(
                           SchemaGeneratorUtil.ConvertNumberToPrimitive(
                               primitiveType.AltFormat));

      var castText = "";
      if (needToCast) {
        var castType =
            SchemaGeneratorUtil.GetTypeName(primitiveType.AltFormat);
        castText = $"({castType}) ";
      }

      cbsb.WriteLine(
          $"ew.Write{readType}({castText}this.{member.Name});");
    }

    private static void WriteString_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var stringType = member.MemberType as IStringType;

      if (!stringType.IsNullTerminated) {
        cbsb.WriteLine($"ew.WriteString(this.{member.Name});");
      } else {
        cbsb.WriteLine($"ew.WriteStringNT(this.{member.Name});");
      }
      return;

      // TODO: Handle more cases
      throw new NotImplementedException();
    }

    private static void WriteStructure_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      // TODO: Do value types need to be handled differently?
      cbsb.WriteLine($"this.{member.Name}.Write(ew);");
    }

    private static void WriteArray_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var arrayType = member.MemberType as ISequenceMemberType;
      if (arrayType.LengthType != SequenceLengthType.CONST) {
        var isImmediate =
            arrayType.LengthType == SequenceLengthType.IMMEDIATE_VALUE;

        if (isImmediate) {
          var writeType = SchemaGeneratorUtil.GetIntLabel(
              arrayType.ImmediateLengthType);

          var castType = SchemaGeneratorUtil.GetTypeName(
              SchemaGeneratorUtil.ConvertIntToNumber(
                  arrayType.ImmediateLengthType));

          var arrayLengthName = arrayType.SequenceType == SequenceType.ARRAY
                                    ? "Length"
                                    : "Count";
          var arrayLengthAccessor = $"this.{member.Name}.{arrayLengthName}";

          cbsb.WriteLine(
              $"ew.Write{writeType}(({castType}) {arrayLengthAccessor});");
        }
      }

      SchemaWriterGenerator.WriteIntoArray_(cbsb, member);
    }

    private static void WriteIntoArray_(
        ICurlyBracketStringBuilder cbsb,
        ISchemaMember member) {
      var arrayType = member.MemberType as ISequenceMemberType;

      var elementType = arrayType.ElementType;
      if (elementType is IPrimitiveMemberType primitiveElementType) {
        // Primitives that don't need to be cast are the easiest to write.
        if (!primitiveElementType.UseAltFormat) {
          var label =
              SchemaGeneratorUtil.GetPrimitiveLabel(
                  primitiveElementType.PrimitiveType);
          cbsb.WriteLine($"ew.Write{label}s(this.{member.Name});");
          return;
        }

        // Primitives that *do* need to be cast have to be written individually.
        var writeType = SchemaGeneratorUtil.GetPrimitiveLabel(
            SchemaGeneratorUtil.ConvertNumberToPrimitive(
                primitiveElementType.AltFormat));
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
                $"ew.Write{writeType}(({castType}) this.{member.Name}[i]);")
            .ExitBlock();
        return;
      }

      if (elementType is IStructureMemberType structureElementType) {
        //if (structureElementType.IsReferenceType) {
        cbsb.EnterBlock($"foreach (var e in this.{member.Name})")
            .WriteLine("e.Write(ew);")
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
              .WriteLine("e.Read(ew);")
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