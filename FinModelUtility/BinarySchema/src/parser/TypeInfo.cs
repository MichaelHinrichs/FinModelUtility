using System;

using Microsoft.CodeAnalysis;


namespace schema.parser {
  public enum SchemaTypeKind {
    BOOL,
    INTEGER,
    FLOAT,
    CHAR,
    STRING,
    ENUM,
    STRUCTURE,
    CONST_LENGTH_CONTAINER,
    VARIABLE_LENGTH_CONTAINER,
  }

  public interface ITypeInfo {
    ITypeSymbol TypeSymbol { get; }
    SchemaTypeKind Kind { get; }
    bool IsReadonly { get; }
    bool IsNullable { get; }
  }

  public interface IPrimitiveTypeInfo : ITypeInfo {
    SchemaPrimitiveType PrimitiveType { get; }
  }

  public interface IBoolTypeInfo : IPrimitiveTypeInfo { }

  public interface INumberTypeInfo : IPrimitiveTypeInfo {
    SchemaNumberType NumberType { get; }
  }

  public interface IIntegerTypeInfo : INumberTypeInfo {
    SchemaIntType IntType { get; }
  }

  public interface IEnumTypeInfo : IPrimitiveTypeInfo { }

  public interface ICharTypeInfo : IPrimitiveTypeInfo { }

  public interface IStringTypeInfo : ITypeInfo { }

  public interface IStructureTypeInfo : ITypeInfo { }

  public interface IContainerTypeInfo : ITypeInfo {
    ITypeInfo ContainedType { get; }
  }

  public class TypeInfoParser {
    public enum ParseStatus {
      SUCCESS,
      NOT_A_FIELD_OR_PROPERTY,
      NOT_SAME_KIND_OF_SERIALIZABLE,
      NOT_IMPLEMENTED,
    }

    public ParseStatus
        ParseMember(ISymbol memberSymbol, out ITypeInfo typeInfo) {
      typeInfo = null;

      if (!GetTypeOfMember_(
              memberSymbol,
              out var memberTypeSymbol,
              out var isReadonly)) {
        return ParseStatus.NOT_A_FIELD_OR_PROPERTY;
      }

      this.ParseNullable_(ref memberTypeSymbol, out var isNullable);

      return this.ParseTypeSymbol_(
          memberTypeSymbol,
          isReadonly,
          isNullable,
          out typeInfo);
    }

    private ParseStatus ParseTypeSymbol_(
        ITypeSymbol typeSymbol,
        bool isReadonly,
        bool isNullable,
        out ITypeInfo typeInfo) {
      var primitiveType =
          SchemaPrimitiveTypesUtil.GetPrimitiveTypeFromTypeSymbol(
              typeSymbol);
      if (primitiveType != SchemaPrimitiveType.UNDEFINED) {
        switch (primitiveType) {
          case SchemaPrimitiveType.BOOLEAN: {
            typeInfo = new BoolTypeInfo(
                typeSymbol,
                isReadonly,
                isNullable);
            return ParseStatus.SUCCESS;
          }
          case SchemaPrimitiveType.BYTE:
          case SchemaPrimitiveType.SBYTE:
          case SchemaPrimitiveType.INT16:
          case SchemaPrimitiveType.UINT16:
          case SchemaPrimitiveType.INT32:
          case SchemaPrimitiveType.UINT32:
          case SchemaPrimitiveType.INT64:
          case SchemaPrimitiveType.UINT64: {
            typeInfo = new IntegerTypeInfo(
                typeSymbol,
                SchemaTypeKind.INTEGER,
                SchemaPrimitiveTypesUtil.ConvertNumberToInt(
                    SchemaPrimitiveTypesUtil
                        .ConvertPrimitiveToNumber(primitiveType)),
                isReadonly,
                isNullable);
            return ParseStatus.SUCCESS;
          }
          case SchemaPrimitiveType.SN8:
          case SchemaPrimitiveType.UN8:
          case SchemaPrimitiveType.SN16:
          case SchemaPrimitiveType.UN16:
          case SchemaPrimitiveType.SINGLE:
          case SchemaPrimitiveType.DOUBLE: {
            typeInfo = new FloatTypeInfo(
                typeSymbol,
                SchemaTypeKind.FLOAT,
                SchemaPrimitiveTypesUtil
                    .ConvertPrimitiveToNumber(primitiveType),
                isReadonly,
                isNullable);
            return ParseStatus.SUCCESS;
          }
          case SchemaPrimitiveType.CHAR: {
            typeInfo = new CharTypeInfo(
                typeSymbol,
                isReadonly,
                isNullable);
            return ParseStatus.SUCCESS;
          }
          case SchemaPrimitiveType.ENUM: {
            typeInfo = new EnumTypeInfo(
                typeSymbol,
                isReadonly,
                isNullable);
            return ParseStatus.SUCCESS;
          }
          default: throw new ArgumentOutOfRangeException();
        }
      }

      if (typeSymbol.SpecialType == SpecialType.System_String) {
        typeInfo = new StringTypeInfo(
            typeSymbol,
            isReadonly,
            isNullable);
        return ParseStatus.SUCCESS;
      }

      if (typeSymbol.SpecialType is SpecialType
              .System_Collections_Generic_IReadOnlyList_T) {
        var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

        var containedTypeSymbol = namedTypeSymbol.TypeArguments[0];
        this.ParseNullable_(ref containedTypeSymbol,
                            out var isContainedNullable);

        var containedParseStatus = this.ParseTypeSymbol_(
            containedTypeSymbol,
            true,
            isContainedNullable,
            out var containedTypeInfo);
        if (containedParseStatus != ParseStatus.SUCCESS) {
          typeInfo = default;
          return containedParseStatus;
        }

        typeInfo = new ContainerTypeInfo(
            typeSymbol,
            SchemaTypeKind.CONST_LENGTH_CONTAINER,
            isReadonly,
            isNullable,
            containedTypeInfo);
        return ParseStatus.SUCCESS;
      } 
      
      if (typeSymbol.TypeKind is TypeKind.Array) {
        var arrayTypeSymbol = typeSymbol as IArrayTypeSymbol;

        var containedTypeSymbol = arrayTypeSymbol.ElementType;
        this.ParseNullable_(ref containedTypeSymbol,
                            out var isContainedNullable);

        var containedParseStatus = this.ParseTypeSymbol_(
            containedTypeSymbol,
            false,
            isContainedNullable,
            out var containedTypeInfo);
        if (containedParseStatus != ParseStatus.SUCCESS) {
          typeInfo = default;
          return containedParseStatus;
        }

        typeInfo = new ContainerTypeInfo(
            typeSymbol,
            SchemaTypeKind.CONST_LENGTH_CONTAINER,
            isReadonly,
            isNullable,
            containedTypeInfo);
        return ParseStatus.SUCCESS;
      } 
      
      if (typeSymbol.SpecialType is SpecialType
                     .System_Collections_Generic_IList_T) {
        var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

        var containedTypeSymbol = namedTypeSymbol.TypeArguments[0];
        this.ParseNullable_(ref containedTypeSymbol,
                            out var isContainedNullable);

        var containedParseStatus = this.ParseTypeSymbol_(
            containedTypeSymbol,
            false,
            isContainedNullable,
            out var containedTypeInfo);
        if (containedParseStatus != ParseStatus.SUCCESS) {
          typeInfo = default;
          return containedParseStatus;
        }

        typeInfo = new ContainerTypeInfo(
            typeSymbol,
            SchemaTypeKind.VARIABLE_LENGTH_CONTAINER,
            isReadonly,
            isNullable,
            containedTypeInfo);
        return ParseStatus.SUCCESS;
      }

      if (typeSymbol is INamedTypeSymbol fieldNamedTypeSymbol) {
        // TODO: Check if implements same kind as parent
        if (SymbolTypeUtil.Implements(fieldNamedTypeSymbol,
                                      typeof(IBiSerializable))) {
          typeInfo = new StructureTypeInfo(
              typeSymbol,
              isReadonly,
              isNullable);
          return ParseStatus.SUCCESS;
        }

        typeInfo = default;
        return ParseStatus.NOT_SAME_KIND_OF_SERIALIZABLE;
      }

      typeInfo = default;
      return ParseStatus.NOT_IMPLEMENTED;
    }

    private bool GetTypeOfMember_(
        ISymbol memberSymbol,
        out ITypeSymbol memberTypeSymbol,
        out bool isMemberReadonly) {
      switch (memberSymbol) {
        case IPropertySymbol propertySymbol: {
          isMemberReadonly = propertySymbol.SetMethod == null;
          memberTypeSymbol = propertySymbol.Type;
          return true;
        }
        case IFieldSymbol fieldSymbol: {
          isMemberReadonly = fieldSymbol.IsReadOnly;
          memberTypeSymbol = fieldSymbol.Type;
          return true;
        }
        default: {
          isMemberReadonly = false;
          memberTypeSymbol = default;
          return false;
        }
      }
    }

    private void ParseNullable_(ref ITypeSymbol typeSymbol,
                                out bool isNullable) {
      isNullable = false;
      if (typeSymbol is INamedTypeSymbol {
              Name: "Nullable"
          } fieldNamedTypeSymbol) {
        typeSymbol = fieldNamedTypeSymbol.TypeArguments[0];
        isNullable = true;
      }
    }

    private record BoolTypeInfo : IBoolTypeInfo {
      public BoolTypeInfo(
          ITypeSymbol typeSymbol,
          bool isReadonly,
          bool isNullable) {
        this.TypeSymbol = typeSymbol;
        this.IsReadonly = isReadonly;
        this.IsNullable = isNullable;
      }

      public ITypeSymbol TypeSymbol { get; }

      public SchemaPrimitiveType PrimitiveType => SchemaPrimitiveType.BOOLEAN;
      public SchemaTypeKind Kind => SchemaTypeKind.BOOL;

      public bool IsReadonly { get; }
      public bool IsNullable { get; }
    }


    private class FloatTypeInfo : INumberTypeInfo {
      public FloatTypeInfo(
          ITypeSymbol typeSymbol,
          SchemaTypeKind kind,
          SchemaNumberType numberType,
          bool isReadonly,
          bool isNullable) {
        this.TypeSymbol = typeSymbol;
        this.Kind = kind;
        this.NumberType = numberType;
        this.IsReadonly = isReadonly;
        this.IsNullable = isNullable;
      }

      public ITypeSymbol TypeSymbol { get; }
      public SchemaTypeKind Kind { get; }
      public SchemaNumberType NumberType { get; }

      public SchemaPrimitiveType PrimitiveType
        => SchemaPrimitiveTypesUtil.ConvertNumberToPrimitive(this.NumberType);

      public bool IsReadonly { get; }
      public bool IsNullable { get; }
    }

    private class IntegerTypeInfo : IIntegerTypeInfo {
      public IntegerTypeInfo(
          ITypeSymbol typeSymbol,
          SchemaTypeKind kind,
          SchemaIntType intType,
          bool isReadonly,
          bool isNullable) {
        this.TypeSymbol = typeSymbol;
        this.Kind = kind;
        this.IntType = intType;
        this.IsReadonly = isReadonly;
        this.IsNullable = isNullable;
      }

      public ITypeSymbol TypeSymbol { get; }
      public SchemaTypeKind Kind { get; }
      public SchemaIntType IntType { get; }

      public SchemaNumberType NumberType
        => SchemaPrimitiveTypesUtil.ConvertIntToNumber(this.IntType);

      public SchemaPrimitiveType PrimitiveType
        => SchemaPrimitiveTypesUtil.ConvertNumberToPrimitive(this.NumberType);

      public bool IsReadonly { get; }
      public bool IsNullable { get; }
    }

    private class CharTypeInfo : ICharTypeInfo {
      public CharTypeInfo(
          ITypeSymbol typeSymbol,
          bool isReadonly,
          bool isNullable) {
        this.TypeSymbol = typeSymbol;
        this.IsReadonly = isReadonly;
        this.IsNullable = isNullable;
      }

      public SchemaPrimitiveType PrimitiveType => SchemaPrimitiveType.CHAR;
      public SchemaTypeKind Kind => SchemaTypeKind.CHAR;

      public ITypeSymbol TypeSymbol { get; }

      public bool IsReadonly { get; }
      public bool IsNullable { get; }
    }

    private class StringTypeInfo : IStringTypeInfo {
      public StringTypeInfo(
          ITypeSymbol typeSymbol,
          bool isReadonly,
          bool isNullable) {
        this.TypeSymbol = typeSymbol;
        this.IsReadonly = isReadonly;
        this.IsNullable = isNullable;
      }

      public SchemaTypeKind Kind => SchemaTypeKind.STRING;

      public ITypeSymbol TypeSymbol { get; }

      public bool IsReadonly { get; }
      public bool IsNullable { get; }
    }

    private class EnumTypeInfo : IEnumTypeInfo {
      public EnumTypeInfo(
          ITypeSymbol typeSymbol,
          bool isReadonly,
          bool isNullable) {
        this.TypeSymbol = typeSymbol;
        this.IsReadonly = isReadonly;
        this.IsNullable = isNullable;
      }

      public SchemaPrimitiveType PrimitiveType => SchemaPrimitiveType.ENUM;
      public SchemaTypeKind Kind => SchemaTypeKind.ENUM;

      public ITypeSymbol TypeSymbol { get; }

      public bool IsReadonly { get; }
      public bool IsNullable { get; }
    }

    private class StructureTypeInfo : IStructureTypeInfo {
      public StructureTypeInfo(
          ITypeSymbol typeSymbol,
          bool isReadonly,
          bool isNullable) {
        this.TypeSymbol = typeSymbol;
        this.IsReadonly = isReadonly;
        this.IsNullable = isNullable;
      }

      public SchemaTypeKind Kind => SchemaTypeKind.STRUCTURE;

      public ITypeSymbol TypeSymbol { get; }

      public bool IsReadonly { get; }
      public bool IsNullable { get; }
    }

    private class ContainerTypeInfo : IContainerTypeInfo {
      public ContainerTypeInfo(
          ITypeSymbol typeSymbol,
          SchemaTypeKind kind,
          bool isReadonly,
          bool isNullable,
          ITypeInfo containedType) {
        this.TypeSymbol = typeSymbol;
        this.Kind = kind;
        this.IsReadonly = isReadonly;
        this.IsNullable = isNullable;
        this.ContainedType = containedType;
      }

      public ITypeSymbol TypeSymbol { get; }
      public SchemaTypeKind Kind { get; }

      public bool IsReadonly { get; }
      public bool IsNullable { get; }

      public ITypeInfo ContainedType { get; }
    }
  }
}