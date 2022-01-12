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
      cbsb.WriteLine("using System.IO;");

      // TODO: Handle fancier cases here
      cbsb.EnterBlock($"namespace {typeNamespace}");
      foreach (var declaringType in declaringTypes) {
        cbsb.EnterBlock(
            $"{SymbolTypeUtil.GetSymbolQualifiers(declaringType)} {typeName}");
      }
      cbsb.EnterBlock(
          $"{SymbolTypeUtil.GetSymbolQualifiers(typeSymbol)} {typeName}");

      cbsb.EnterBlock("public void Read(EndianBinaryReader er)");
      foreach (var field in structure.Fields) {
        var indexName = $"this.{field.Name}[i]";
        var eachName = $"{field.Name}Instance";

        var isValuePrimitive =
            field.PrimitiveType != SchemaPrimitiveType.UNDEFINED;

        var valueName = field.IsArray ? eachName : $"this.{field.Name}";

        if (field.IsArray) {
          if (!field.HasConstLength) {
            var fieldTypeName = field.TypeSymbol.Name;
            var fieldTypeNamespace =
                SymbolTypeUtil.MergeContainingNamespaces(field.TypeSymbol);

            var countField = field.LengthField;

            cbsb.EnterBlock($"if (this.{countField.Name} < 0)");
            cbsb.WriteLine(
                $"throw new Exception(\"Expected {countField.Name} to be nonnegative!\");");
            cbsb.ExitBlock();

            cbsb.EnterBlock(
                $"if (this.{field.Name}.Count < this.{countField.Name})");
            cbsb.WriteLine(
                $"this.{field.Name}.Add(new {fieldTypeNamespace}.{fieldTypeName}());");
            cbsb.ExitBlock();

            cbsb.EnterBlock(
                $"while (this.{field.Name}.Count > this.{countField.Name})");
            cbsb.WriteLine($"this.{field.Name}.RemoveAt(0);");
            cbsb.ExitBlock();
          }

          cbsb.EnterBlock(
              $"for (var i = 0; i < this.{field.Name}.Length; ++i)");
          cbsb.WriteLine($"var {eachName} = {indexName};");
        }

        if (!isValuePrimitive) {
          cbsb.WriteLine($"{valueName}.Read(er);");
        } else {
          var format =
              !field.UseAltFormat
                  ? field.PrimitiveType
                  : SchemaReaderGenerator.ConvertNumberToPrimitive_(
                      field.AltFormat);
          var label =
              SchemaReaderGenerator.GetPrimitiveLabel_(format);

          if (!field.IsPrimitiveConst) {
            var cast = "";
            if (field.UseAltFormat) {
              var castTypeName =
                  field.PrimitiveType == SchemaPrimitiveType.ENUM
                      ? SymbolTypeUtil.GetQualifiedName(field.TypeSymbol)
                      : field.TypeSymbol.Name;
              cast = $"({castTypeName}) ";
            }

            cbsb.WriteLine($"{valueName} = {cast}er.Read{label}();");
          } else {
            var cast = "";
            if (field.UseAltFormat) {
              var castTypeName =
                  SchemaReaderGenerator.GetTypeName(field.AltFormat);
              cast = $"({castTypeName}) ";
            }

            cbsb.WriteLine($"er.Assert{label}({cast}{valueName});");
          }
        }

        if (field.IsArray) {
          cbsb.WriteLine($"{indexName} = {eachName};");
          cbsb.ExitBlock();
        }
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


    private static string GetPrimitiveLabel_(SchemaPrimitiveType type)
      => type switch {
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
          SchemaPrimitiveType.SN16 => "Sn16",
          SchemaPrimitiveType.UN16 => "Un16",
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
          SchemaNumberType.SN16 => SchemaPrimitiveType.SN16,
          SchemaNumberType.UN16 => SchemaPrimitiveType.UN16,
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };
  }
}