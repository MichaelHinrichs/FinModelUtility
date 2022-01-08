using System;
using System.Collections.Generic;

using schema.text;

namespace schema {
  internal class SchemaReaderGenerator {
    public void Generate(ISchemaStructure structure) {
      /*var typeSymbol = structure.TypeSymbol;

      var declaringTypes =
          SchemaReaderGenerator.GetDeclaringTypesDownward_(type);
      var symbolType = (type.IsAbstract ? "abstract " : "") +
                       "partial " +
                       (type.IsClass ? "class" : "struct");

      var cbsb = new CurlyBracketStringBuilder();
      cbsb.WriteLine("import System.IO;");

      cbsb.EnterBlock($"namespace {type.Namespace}");
      foreach (var declaringType in declaringTypes) {
        cbsb.EnterBlock($"{SchemaReaderGenerator.GetSymbolQualifiers_(declaringType)} {type.Name}");
      }
      cbsb.EnterBlock($"{SchemaReaderGenerator.GetSymbolQualifiers_(type)} {type.Name}");

      cbsb.EnterBlock("public void Read(EndianBinaryReader er)");
      foreach (var field in structure.Fields) {
        var valueName = field.IsArray
                            ? $"{field.Name}Instance"
                            : $"this.{field.Name}";

        if (field.IsArray) {
          if (!field.HasConstLength) {
            var countField = field.LengthField;

            cbsb.EnterBlock($"if (this.{countField.Name} < 0)");
            cbsb.WriteLine(
                $"throw new Exception(\"Expected {countField.Name} to be nonnegative!\");");
            cbsb.ExitBlock();

            cbsb.EnterBlock(
                $"if (this.{field.Name}.Count < this.{countField.Name})");
            cbsb.WriteLine(
                $"this.{field.Name}.Add(new {field.Type.Namespace}.{field.Type.Name}());");
            cbsb.ExitBlock();

            cbsb.EnterBlock(
                $"while (this.{field.Name}.Count > this.{countField.Name})");
            cbsb.WriteLine($"this.{field.Name}.RemoveAt(0);");
            cbsb.ExitBlock();
          }

          cbsb.EnterBlock($"foreach (var {valueName} in this.{field.Name})");
        }

        if (!field.IsPrimitive) {
          cbsb.WriteLine($"{valueName}.Read(er);");
        } else {
          var label =
              SchemaReaderGenerator.GetPrimitiveLabel_(field.PrimitiveType);

          cbsb.WriteLine(!field.IsPrimitiveConst
                             ? $"{valueName} = er.Read{label}();"
                             : $"er.Assert{label}({valueName});");
        }

        if (field.IsArray) {
          cbsb.ExitBlock();
        }
      }
      cbsb.ExitBlock();

      // type
      cbsb.ExitBlock();

      // parent types
      foreach (var declaringType in declaringTypes) {
        cbsb.ExitBlock();
      }

      // namespace
      cbsb.ExitBlock();

      var generatedCode = cbsb.ToString();
      ;*/
    }

    private static Type[] GetDeclaringTypesDownward_(Type type) {
      var declaringTypes = new List<Type>();

      var declaringType = type.DeclaringType;
      while (declaringType != null) {
        declaringTypes.Add(declaringType);
        declaringType = declaringType.DeclaringType;
      }
      declaringTypes.Reverse();

      return declaringTypes.ToArray();
    }

    private static string GetSymbolQualifiers_(Type type)
      => "public " +
         (type.IsAbstract ? "abstract " : "") +
         "partial " +
         (type.IsClass ? "class" : "struct");

    private static string GetPrimitiveLabel_(SchemaPrimitiveType type)
      => type switch {
          SchemaPrimitiveType.SBYTE => "Sbyte",
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
  }
}