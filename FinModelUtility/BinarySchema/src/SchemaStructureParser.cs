using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace schema {
  public interface ISchemaStructureParser {
    ISchemaStructure ParseStructure(INamedTypeSymbol symbol);
  }

  public interface ISchemaStructure {
    ITypeSymbol TypeSymbol { get; }
    IReadOnlyList<ISchemaField> Fields { get; }
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
  }

  public interface ISchemaField {
    string Name { get; }
    ITypeSymbol TypeSymbol { get; }

    bool HasConstLength { get; }
    ISchemaField LengthField { get; }

    bool IsArray { get; }

    bool IsPrimitive { get; }
    SchemaPrimitiveType PrimitiveType { get; }
    bool IsPrimitiveConst { get; }
  }

  public class SchemaStructureParser : ISchemaStructureParser {
    public ISchemaStructure ParseStructure(INamedTypeSymbol structureSymbol) {
      var members = structureSymbol.GetMembers();

      var fields = new List<ISchemaField>();
      foreach (var memberSymbol in members) {
        if (memberSymbol.IsStatic) {
          continue;
        }

        if (memberSymbol.Name.Contains("k__BackingField")) {
          continue;
        }

        ITypeSymbol fieldTypeSymbol;
        bool isFieldReadonly;
        switch (memberSymbol) {
          case IPropertySymbol propertySymbol: {
            isFieldReadonly = propertySymbol.SetMethod == null;
            fieldTypeSymbol = propertySymbol.Type;
            break;
          }
          case IFieldSymbol fieldSymbol: {
            isFieldReadonly = fieldSymbol.IsReadOnly;
            fieldTypeSymbol = fieldSymbol.Type;
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

        var primitiveType = SchemaStructureParser.GetPrimitiveTypeFromType_(fieldTypeSymbol);
        if (primitiveType != SchemaPrimitiveType.UNDEFINED) {
          isPrimitive = true;
          isPrimitiveConst = isFieldReadonly;
        } else if (fieldTypeSymbol.TypeKind is TypeKind.Array) {
          isArray = true;
          hasConstArrayLength = isFieldReadonly;
          /*primitiveTypeInfo = fieldTypeInfo.
              SchemaStructureParser.GetPrimitiveTypeFromType_(
                  fieldTypeInfo.GetElementType());*/
        } /*else if (fieldTypeInfo.IsAssignableFrom(typeof(IReadOnlyList<>))) {
          isArray = true;
          isPrimitiveConst = true;
          hasConstArrayLength = isFieldReadonly;
          primitiveType =
              SchemaStructureParser.GetPrimitiveTypeFromType_(
                  fieldTypeInfo.GenericTypeArguments[0]);
        } else if (fieldTypeInfo.IsAssignableFrom(typeof(IList<>))) {
          isArray = true;
          primitiveType =
              SchemaStructureParser.GetPrimitiveTypeFromType_(
                  fieldTypeInfo.GenericTypeArguments[0]);
        } else if (fieldTypeInfo.IsAssignableFrom(typeof(IDeserializable))) {
          // TODO: Handle unsupported types.
        }*/

        var field = new SchemaField {
            Name = memberSymbol.Name,
            TypeSymbol = fieldTypeSymbol,
            IsArray = isArray,
            HasConstLength = hasConstArrayLength,
            LengthField = null,
            IsPrimitive = isPrimitive,
            PrimitiveType = primitiveType,
            IsPrimitiveConst = isPrimitiveConst,
        };
        fields.Add(field);
      }

      return new SchemaStructure {
          TypeSymbol = structureSymbol,
          Fields = fields,
      };
    }

    private static SchemaPrimitiveType GetPrimitiveTypeFromType_(
        ITypeSymbol typeSymbol) {
      // TODO: Support SN16/UN16
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
      public ITypeSymbol TypeSymbol { get; set; }
      public IReadOnlyList<ISchemaField> Fields { get; set; }
    }


    private class SchemaField : ISchemaField {
      public string Name { get; set; }
      public ITypeSymbol TypeSymbol { get; set; }
      public bool IsArray { get; set; }
      public bool HasConstLength { get; set; }
      public ISchemaField LengthField { get; set; }
      public bool IsPrimitive { get; set; }
      public SchemaPrimitiveType PrimitiveType { get; set; }
      public bool IsPrimitiveConst { get; set; }
    }
  }
}