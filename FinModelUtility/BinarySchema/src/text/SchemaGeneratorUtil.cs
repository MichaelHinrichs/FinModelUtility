using System;

using Microsoft.CodeAnalysis;

namespace schema.text {
  public static class SchemaGeneratorUtil {
    public static string GetPrimitiveLabel(SchemaPrimitiveType type)
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
          SchemaPrimitiveType.SN8 => "Sn8",
          SchemaPrimitiveType.UN8 => "Un8",
          SchemaPrimitiveType.SN16 => "Sn16",
          SchemaPrimitiveType.UN16 => "Un16",
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    public static string GetIntLabel(SchemaIntType type)
      => type switch {
          SchemaIntType.SBYTE => "SByte",
          SchemaIntType.BYTE => "Byte",
          SchemaIntType.INT16 => "Int16",
          SchemaIntType.UINT16 => "UInt16",
          SchemaIntType.INT32 => "Int32",
          SchemaIntType.UINT32 => "UInt32",
          SchemaIntType.INT64 => "Int64",
          SchemaIntType.UINT64 => "UInt64",
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
  }
}