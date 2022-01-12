using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace schema {
  public interface ISchemaStructureParser {
    ISchemaStructure ParseStructure(
        SyntaxNodeAnalysisContext? context,
        INamedTypeSymbol symbol);
  }

  public interface ISchemaStructure {
    bool Error { get; }
    INamedTypeSymbol TypeSymbol { get; }
    IReadOnlyList<ISchemaMember> Fields { get; }
  }

  public enum SchemaPrimitiveType {
    UNDEFINED,

    SBYTE,
    BYTE,
    INT16,
    UINT16,
    INT32,
    UINT32,
    INT64,
    UINT64,

    SINGLE,
    DOUBLE,

    SN16,
    UN16,

    CHAR,
    STRING,
    STRING_NT,

    ENUM,
  }

  public interface ISchemaMember {
    string Name { get; }
    ISymbol Symbol { get; }

    ITypeSymbol TypeSymbol { get; }

    bool HasConstLength { get; }
    ISchemaMember LengthField { get; }

    bool IsArray { get; }

    bool IsPrimitive { get; }
    SchemaPrimitiveType PrimitiveType { get; }
    bool IsPrimitiveConst { get; }

    bool UseAltFormat { get; }
    SchemaNumberType AltFormat { get; }
  }

  public class SchemaStructureParser : ISchemaStructureParser {
    public ISchemaStructure ParseStructure(
        SyntaxNodeAnalysisContext? context,
        INamedTypeSymbol structureSymbol) {
      var error = false;

      var members = structureSymbol.GetMembers();

      var fields = new List<ISchemaMember>();
      foreach (var memberSymbol in members) {
        if (memberSymbol.IsStatic) {
          continue;
        }

        if (memberSymbol.Name.Contains("k__BackingField")) {
          continue;
        }

        ITypeSymbol memberTypeSymbol;
        bool isFieldReadonly;
        switch (memberSymbol) {
          case IPropertySymbol propertySymbol: {
            isFieldReadonly = propertySymbol.SetMethod == null;
            memberTypeSymbol = propertySymbol.Type;
            break;
          }
          case IFieldSymbol fieldSymbol: {
            isFieldReadonly = fieldSymbol.IsReadOnly;
            memberTypeSymbol = fieldSymbol.Type;
            break;
          }
          default: {
            continue;
          }
        }

        var isPrimitive = false;
        var isArray = false;
        var isPrimitiveConst = false;
        var hasConstArrayLength = false;

        var primitiveType =
            SchemaStructureParser.GetPrimitiveTypeFromType_(memberTypeSymbol);
        if (primitiveType != SchemaPrimitiveType.UNDEFINED) {
          isPrimitive = true;
          isPrimitiveConst = isFieldReadonly;
        } else if (memberTypeSymbol.SpecialType is SpecialType
                       .System_Collections_Generic_IReadOnlyList_T) {
          /*isArray = true;
          isPrimitiveConst = true;
          hasConstArrayLength = isFieldReadonly;
          primitiveType =
              SchemaStructureParser.GetPrimitiveTypeFromType_(
                  fieldTypeInfo.GenericTypeArguments[0]);*/
          Rules.ReportDiagnostic(context, memberSymbol, Rules.NotSupported);
          error = true;
        } else if (memberTypeSymbol.TypeKind is TypeKind.Array) {
          isArray = true;
          hasConstArrayLength = isFieldReadonly;

          var arrayTypeSymbol = memberTypeSymbol as IArrayTypeSymbol;
          primitiveType = SchemaStructureParser.GetPrimitiveTypeFromType_(
              arrayTypeSymbol.ElementType);
        } else if (memberTypeSymbol.SpecialType is SpecialType
                       .System_Collections_Generic_IList_T) {
          Rules.ReportDiagnostic(context, memberSymbol, Rules.NotSupported);
          error = true;
          /*isArray = true;
          primitiveType =
              SchemaStructureParser.GetPrimitiveTypeFromType_(
                  fieldTypeInfo.GenericTypeArguments[0]);*/
        } else if (memberTypeSymbol is INamedTypeSymbol fieldNamedTypeSymbol &&
                   SymbolTypeUtil.Implements(fieldNamedTypeSymbol,
                                             typeof(IDeserializable))) {
          // TODO: Handle unsupported types.
          Rules.ReportDiagnostic(context, memberSymbol, Rules.NotSupported);
          error = true;
        } else {
          Rules.ReportDiagnostic(context, memberSymbol, Rules.NotSupported);
          error = true;
        }

        var formatAttribute =
            SymbolTypeUtil.GetAttribute<FormatAttribute>(memberSymbol);
        var formatNumberType =
            formatAttribute?.NumberType ?? SchemaNumberType.UNDEFINED;
        var useAltFormat = formatNumberType != SchemaNumberType.UNDEFINED;

        if (primitiveType == SchemaPrimitiveType.ENUM && !useAltFormat) {
          Rules.ReportDiagnostic(context, memberSymbol, Rules.EnumNeedsFormat);
          error = true;
        }

        var field = new SchemaMember {
            Name = memberSymbol.Name,
            Symbol = memberSymbol,
            TypeSymbol = memberTypeSymbol,
            IsArray = isArray,
            HasConstLength = hasConstArrayLength,
            LengthField = null,
            IsPrimitive = isPrimitive,
            PrimitiveType = primitiveType,
            IsPrimitiveConst = isPrimitiveConst,
            UseAltFormat = useAltFormat,
            AltFormat = formatNumberType,
        };
        fields.Add(field);
      }

      return new SchemaStructure {
          Error = error,
          TypeSymbol = structureSymbol,
          Fields = fields,
      };
    }

    private static SchemaPrimitiveType GetPrimitiveTypeFromType_(
        ITypeSymbol typeSymbol) {
      if (typeSymbol.TypeKind == TypeKind.Enum) {
        return SchemaPrimitiveType.ENUM;
      }

      return typeSymbol.SpecialType switch {
          SpecialType.System_Char   => SchemaPrimitiveType.CHAR,
          SpecialType.System_SByte  => SchemaPrimitiveType.SBYTE,
          SpecialType.System_Byte   => SchemaPrimitiveType.BYTE,
          SpecialType.System_Int16  => SchemaPrimitiveType.INT16,
          SpecialType.System_UInt16 => SchemaPrimitiveType.UINT16,
          SpecialType.System_Int32  => SchemaPrimitiveType.INT32,
          SpecialType.System_UInt32 => SchemaPrimitiveType.UINT32,
          SpecialType.System_Int64  => SchemaPrimitiveType.INT64,
          SpecialType.System_UInt64 => SchemaPrimitiveType.UINT64,
          SpecialType.System_Single => SchemaPrimitiveType.SINGLE,
          SpecialType.System_Double => SchemaPrimitiveType.DOUBLE,
          _                         => SchemaPrimitiveType.UNDEFINED
      };
    }

    private static SchemaPrimitiveType GetPrimitiveTypeFromType_(Type type) {
      // TODO: Support SN16/UN16
      if (type == typeof(byte)) {
        return SchemaPrimitiveType.BYTE;
      }
      if (type == typeof(sbyte)) {
        return SchemaPrimitiveType.SBYTE;
      }
      if (type == typeof(short)) {
        return SchemaPrimitiveType.INT16;
      }
      if (type == typeof(ushort)) {
        return SchemaPrimitiveType.UINT16;
      }
      if (type == typeof(int)) {
        return SchemaPrimitiveType.INT32;
      }
      if (type == typeof(uint)) {
        return SchemaPrimitiveType.UINT32;
      }
      if (type == typeof(float)) {
        return SchemaPrimitiveType.SINGLE;
      }
      if (type == typeof(double)) {
        return SchemaPrimitiveType.DOUBLE;
      }
      if (type == typeof(long)) {
        return SchemaPrimitiveType.INT64;
      }
      if (type == typeof(ulong)) {
        return SchemaPrimitiveType.UINT64;
      }
      if (type == typeof(char)) {
        return SchemaPrimitiveType.CHAR;
      }

      throw new ArgumentOutOfRangeException();
    }

    private class SchemaStructure : ISchemaStructure {
      public bool Error { get; set; }
      public INamedTypeSymbol TypeSymbol { get; set; }
      public IReadOnlyList<ISchemaMember> Fields { get; set; }
    }


    private class SchemaMember : ISchemaMember {
      public string Name { get; set; }
      public ISymbol Symbol { get; set; }

      public ITypeSymbol TypeSymbol { get; set; }
      public bool IsArray { get; set; }
      public bool HasConstLength { get; set; }
      public ISchemaMember LengthField { get; set; }
      public bool IsPrimitive { get; set; }
      public SchemaPrimitiveType PrimitiveType { get; set; }
      public bool IsPrimitiveConst { get; set; }
      public bool UseAltFormat { get; set; }
      public SchemaNumberType AltFormat { get; set; }
    }
  }
}