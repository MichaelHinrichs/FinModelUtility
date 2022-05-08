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
          SchemaPrimitiveType.UN8 => "Un8",
          SchemaPrimitiveType.SN16 => "Sn16",
          SchemaPrimitiveType.UN16 => "Un16",
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    public static string GetIntLabel(IntType type)
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

    public static SchemaNumberType ConvertIntToNumber(IntType type)
      => type switch {
          IntType.SBYTE => SchemaNumberType.SBYTE,
          IntType.BYTE => SchemaNumberType.BYTE,
          IntType.INT16 => SchemaNumberType.INT16,
          IntType.UINT16 => SchemaNumberType.UINT16,
          IntType.INT32 => SchemaNumberType.INT32,
          IntType.UINT32 => SchemaNumberType.UINT32,
          IntType.INT64 => SchemaNumberType.INT64,
          IntType.UINT64 => SchemaNumberType.UINT64,
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

    public static SchemaPrimitiveType GetUnderlyingPrimitiveType(SchemaPrimitiveType type)
      => type switch {
          SchemaPrimitiveType.UN8  => SchemaPrimitiveType.SINGLE,
          SchemaPrimitiveType.UN16 => SchemaPrimitiveType.SINGLE,
          SchemaPrimitiveType.SN16 => SchemaPrimitiveType.SINGLE,
          _                        => type
      };

    public static SchemaPrimitiveType ConvertNumberToPrimitive(
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