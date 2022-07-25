using System;

using Microsoft.CodeAnalysis;


namespace schema {
  public enum SchemaIntType {
    BYTE,
    SBYTE,
    INT16,
    UINT16,
    INT32,
    UINT32,
    INT64,
    UINT64
  }

  public enum SchemaNumberType {
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

    SN8,
    UN8,

    SN16,
    UN16,
  }

  public static class SchemaPrimitiveTypesUtil {
    public static SchemaPrimitiveType GetPrimitiveTypeFromTypeSymbol(
        ITypeSymbol typeSymbol) {
      if (typeSymbol.TypeKind == TypeKind.Enum) {
        return SchemaPrimitiveType.ENUM;
      }

      return typeSymbol.SpecialType switch {
          SpecialType.System_Boolean => SchemaPrimitiveType.BOOLEAN,
          SpecialType.System_Char    => SchemaPrimitiveType.CHAR,
          SpecialType.System_SByte   => SchemaPrimitiveType.SBYTE,
          SpecialType.System_Byte    => SchemaPrimitiveType.BYTE,
          SpecialType.System_Int16   => SchemaPrimitiveType.INT16,
          SpecialType.System_UInt16  => SchemaPrimitiveType.UINT16,
          SpecialType.System_Int32   => SchemaPrimitiveType.INT32,
          SpecialType.System_UInt32  => SchemaPrimitiveType.UINT32,
          SpecialType.System_Int64   => SchemaPrimitiveType.INT64,
          SpecialType.System_UInt64  => SchemaPrimitiveType.UINT64,
          SpecialType.System_Single  => SchemaPrimitiveType.SINGLE,
          SpecialType.System_Double  => SchemaPrimitiveType.DOUBLE,
          _                          => SchemaPrimitiveType.UNDEFINED
      };
    }

    public static bool IsPrimitiveTypeNumeric(SchemaPrimitiveType type)
      => type switch {
          SchemaPrimitiveType.BOOLEAN => true,
          SchemaPrimitiveType.SBYTE   => true,
          SchemaPrimitiveType.BYTE    => true,
          SchemaPrimitiveType.INT16   => true,
          SchemaPrimitiveType.UINT16  => true,
          SchemaPrimitiveType.INT32   => true,
          SchemaPrimitiveType.UINT32  => true,
          SchemaPrimitiveType.INT64   => true,
          SchemaPrimitiveType.UINT64  => true,
          SchemaPrimitiveType.SINGLE  => true,
          SchemaPrimitiveType.DOUBLE  => true,
          SchemaPrimitiveType.SN8     => true,
          SchemaPrimitiveType.UN8     => true,
          SchemaPrimitiveType.SN16    => true,
          SchemaPrimitiveType.UN16    => true,
          SchemaPrimitiveType.ENUM    => true,

          SchemaPrimitiveType.CHAR      => false,
          SchemaPrimitiveType.UNDEFINED => false,
          _                             => throw new NotImplementedException(),
      };

    public static SchemaNumberType GetNumberTypeFromTypeSymbol(
        ITypeSymbol? typeSymbol)
      => typeSymbol?.SpecialType switch {
          SpecialType.System_Byte   => SchemaNumberType.BYTE,
          SpecialType.System_SByte  => SchemaNumberType.SBYTE,
          SpecialType.System_Int16  => SchemaNumberType.INT16,
          SpecialType.System_UInt16 => SchemaNumberType.UINT16,
          SpecialType.System_Int32  => SchemaNumberType.INT32,
          SpecialType.System_UInt32 => SchemaNumberType.UINT32,
          SpecialType.System_Int64  => SchemaNumberType.INT64,
          SpecialType.System_UInt64 => SchemaNumberType.UINT64,
          _                         => SchemaNumberType.UNDEFINED,
      };

    public static SchemaNumberType ConvertIntToNumber(SchemaIntType type)
      => type switch {
          SchemaIntType.SBYTE => SchemaNumberType.SBYTE,
          SchemaIntType.BYTE => SchemaNumberType.BYTE,
          SchemaIntType.INT16 => SchemaNumberType.INT16,
          SchemaIntType.UINT16 => SchemaNumberType.UINT16,
          SchemaIntType.INT32 => SchemaNumberType.INT32,
          SchemaIntType.UINT32 => SchemaNumberType.UINT32,
          SchemaIntType.INT64 => SchemaNumberType.INT64,
          SchemaIntType.UINT64 => SchemaNumberType.UINT64,
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    public static SchemaPrimitiveType GetUnderlyingPrimitiveType(
        SchemaPrimitiveType type)
      => type switch {
          SchemaPrimitiveType.SN8  => SchemaPrimitiveType.SINGLE,
          SchemaPrimitiveType.UN8  => SchemaPrimitiveType.SINGLE,
          SchemaPrimitiveType.UN16 => SchemaPrimitiveType.SINGLE,
          SchemaPrimitiveType.SN16 => SchemaPrimitiveType.SINGLE,
          _                        => type
      };

    public static SchemaIntType ConvertNumberToInt(
        SchemaNumberType type)
      => type switch {
          SchemaNumberType.SBYTE => SchemaIntType.SBYTE,
          SchemaNumberType.BYTE => SchemaIntType.BYTE,
          SchemaNumberType.INT16 => SchemaIntType.INT16,
          SchemaNumberType.UINT16 => SchemaIntType.UINT16,
          SchemaNumberType.INT32 => SchemaIntType.INT32,
          SchemaNumberType.UINT32 => SchemaIntType.UINT32,
          SchemaNumberType.INT64 => SchemaIntType.INT64,
          SchemaNumberType.UINT64 => SchemaIntType.UINT64,
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
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
          SchemaNumberType.SN8 => SchemaPrimitiveType.SN8,
          SchemaNumberType.UN8 => SchemaPrimitiveType.UN8,
          SchemaNumberType.SN16 => SchemaPrimitiveType.SN16,
          SchemaNumberType.UN16 => SchemaPrimitiveType.UN16,
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    public static SchemaNumberType ConvertPrimitiveToNumber(
        SchemaPrimitiveType type)
      => type switch {
          SchemaPrimitiveType.SBYTE => SchemaNumberType.SBYTE,
          SchemaPrimitiveType.BYTE => SchemaNumberType.BYTE,
          SchemaPrimitiveType.INT16 => SchemaNumberType.INT16,
          SchemaPrimitiveType.UINT16 => SchemaNumberType.UINT16,
          SchemaPrimitiveType.INT32 => SchemaNumberType.INT32,
          SchemaPrimitiveType.UINT32 => SchemaNumberType.UINT32,
          SchemaPrimitiveType.INT64 => SchemaNumberType.INT64,
          SchemaPrimitiveType.UINT64 => SchemaNumberType.UINT64,
          SchemaPrimitiveType.SINGLE => SchemaNumberType.SINGLE,
          SchemaPrimitiveType.DOUBLE => SchemaNumberType.DOUBLE,
          SchemaPrimitiveType.SN8 => SchemaNumberType.SN8,
          SchemaPrimitiveType.UN8 => SchemaNumberType.UN8,
          SchemaPrimitiveType.SN16 => SchemaNumberType.SN16,
          SchemaPrimitiveType.UN16 => SchemaNumberType.UN16,
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };
  }
}