using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace schema {
  public interface ISchemaStructureParser {
    ISchemaStructure ParseStructure(INamedTypeSymbol symbol);
  }

  public interface ISchemaStructure {
    IList<Diagnostic> Diagnostics { get; }
    INamedTypeSymbol TypeSymbol { get; }
    IReadOnlyList<ISchemaMember> Members { get; }
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

    ENUM,
  }

  public interface ISchemaMember {
    string Name { get; }
    IMemberType MemberType { get; }
  }

  public interface IMemberType {
    ITypeSymbol TypeSymbol { get; }
  }

  /// <summary>
  ///   
  /// </summary>
  public interface IPrimitiveMemberType : IMemberType {
    SchemaPrimitiveType PrimitiveType { get; }
    bool IsConst { get; }

    bool UseAltFormat { get; }
    SchemaNumberType AltFormat { get; }
  }

  public interface IStructureMemberType : IMemberType {
    bool IsReferenceType { get; }
  }

  public enum StringLengthType {
    UNSPECIFIED,
    IMMEDIATE_VALUE,
    OTHER_MEMBER,
    CONST,
  }

  public interface IStringType : IMemberType {
    bool IsConst { get; }

    // TODO: Support char format?
    bool IsNullTerminated { get; }

    /// <summary>
    ///   Whether the string has a set length. This is required for non-null
    ///   terminated strings.
    ///
    ///   If this is set on a null-terminated string, this will function as
    ///   the max length (minus one, since the null-terminator is included).
    /// </summary>
    StringLengthType LengthType { get; }

    IntType ImmediateLengthType { get; }
    ISchemaMember? LengthMember { get; }
    int ConstLength { get; }
  }

  public enum SequenceType {
    ARRAY,
    LIST,
  }

  public enum SequenceLengthType {
    UNSPECIFIED,
    IMMEDIATE_VALUE,
    OTHER_MEMBER,
    CONST,
  }

  public interface ISequenceMemberType : IMemberType {
    SequenceType SequenceType { get; }

    SequenceLengthType LengthType { get; }
    IntType ImmediateLengthType { get; }
    ISchemaMember? LengthMember { get; }

    IMemberType ElementType { get; }
  }

  public class SchemaStructureParser : ISchemaStructureParser {
    public ISchemaStructure ParseStructure(
        INamedTypeSymbol structureSymbol) {
      var diagnostics = new List<Diagnostic>();

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
        bool isMemberReadonly;
        switch (memberSymbol) {
          case IPropertySymbol propertySymbol: {
            isMemberReadonly = propertySymbol.SetMethod == null;
            memberTypeSymbol = propertySymbol.Type;
            break;
          }
          case IFieldSymbol fieldSymbol: {
            isMemberReadonly = fieldSymbol.IsReadOnly;
            memberTypeSymbol = fieldSymbol.Type;
            break;
          }
          default: {
            continue;
          }
        }

        // Get attributes

        IMemberType? memberType = null;
        SequenceBundle? sequenceBundle = null;

        {
          var primitiveType =
              SchemaStructureParser.GetPrimitiveTypeFromType_(memberTypeSymbol);
          if (primitiveType != SchemaPrimitiveType.UNDEFINED) {
            memberType = new PrimitiveMemberType {
                TypeSymbol = memberTypeSymbol,
                IsConst = isMemberReadonly,
                PrimitiveType = primitiveType,
            };
          } else if (memberTypeSymbol.SpecialType ==
                     SpecialType.System_String) {
            memberType = new StringType {
                TypeSymbol = memberTypeSymbol,
                IsConst = isMemberReadonly,
            };
          } else if (memberTypeSymbol.SpecialType is SpecialType
                         .System_Collections_Generic_IReadOnlyList_T) {
            /*isArray = true;
            isPrimitiveConst = true;
            hasConstArrayLength = isFieldReadonly;
            primitiveType =
                SchemaStructureParser.GetPrimitiveTypeFromType_(
                    fieldTypeInfo.GenericTypeArguments[0]);*/
            diagnostics.Add(
                Rules.CreateDiagnostic(memberSymbol, Rules.NotSupported));
          } else if (memberTypeSymbol.TypeKind is TypeKind.Array) {
            var arrayTypeSymbol = memberTypeSymbol as IArrayTypeSymbol;
            sequenceBundle = new SequenceBundle {
                TypeSymbol = memberTypeSymbol,
                IsArray = true,
                IsLengthConst = isMemberReadonly,
                ElementTypeSymbol = arrayTypeSymbol.ElementType,
            };
          } else if (memberTypeSymbol.SpecialType is SpecialType
                         .System_Collections_Generic_IList_T) {
            diagnostics.Add(
                Rules.CreateDiagnostic(memberSymbol, Rules.NotSupported));
            /*isArray = true;
            primitiveType =
                SchemaStructureParser.GetPrimitiveTypeFromType_(
                    fieldTypeInfo.GenericTypeArguments[0]);*/
          } else if
              (memberTypeSymbol is INamedTypeSymbol fieldNamedTypeSymbol &&
               SymbolTypeUtil.Implements(fieldNamedTypeSymbol,
                                         typeof(IDeserializable))) {
            // TODO: Warn if didn't implement deserializable
            memberType = new StructureMemberType {
                TypeSymbol = memberTypeSymbol,
                IsReferenceType = fieldNamedTypeSymbol.IsReferenceType,
            };
          } else {
            diagnostics.Add(
                Rules.CreateDiagnostic(memberSymbol, Rules.NotSupported));
          }
        }

        if (sequenceBundle != null) {
          var elementTypeSymbol = sequenceBundle.ElementTypeSymbol;
          var elementPrimitiveType =
              SchemaStructureParser
                  .GetPrimitiveTypeFromType_(elementTypeSymbol);

          IMemberType? elementMemberType = null;
          if (elementPrimitiveType != SchemaPrimitiveType.UNDEFINED) {
            elementMemberType = new PrimitiveMemberType {
                TypeSymbol = elementTypeSymbol,
                IsConst = sequenceBundle.AreElementsConst,
                PrimitiveType = elementPrimitiveType,
            };
          } else if
              (elementTypeSymbol is INamedTypeSymbol elementNamedTypeSymbol &&
               SymbolTypeUtil.Implements(elementNamedTypeSymbol,
                                         typeof(IDeserializable))) {
            // TODO: Warn if didn't implement deserializable
            elementMemberType = new StructureMemberType {
                TypeSymbol = elementTypeSymbol,
                IsReferenceType = elementNamedTypeSymbol.IsReferenceType,
            };
          } else {
            diagnostics.Add(
                Rules.CreateDiagnostic(memberSymbol,
                                       Rules.UnsupportedArrayType));
          }

          if (elementMemberType != null) {
            memberType = new SequenceMemberType {
                TypeSymbol = sequenceBundle.TypeSymbol,
                SequenceType = sequenceBundle.IsArray
                                   ? SequenceType.ARRAY
                                   : SequenceType.LIST,
                ElementType = elementMemberType,
                LengthType = sequenceBundle.IsLengthConst
                                 ? SequenceLengthType.CONST
                                 : SequenceLengthType.UNSPECIFIED,
            };
          }
        }

        {
          IMemberType? targetMemberType;
          if (memberType is ISequenceMemberType sequenceMember) {
            targetMemberType = sequenceMember.ElementType;
          } else {
            targetMemberType = memberType;
          }
          var targetPrimitiveType = SchemaPrimitiveType.UNDEFINED;
          if (targetMemberType is IPrimitiveMemberType primitiveType) {
            targetPrimitiveType = primitiveType.PrimitiveType;
          }

          // TODO: Apply this to element type as well
          var formatNumberType = SchemaNumberType.UNDEFINED;
          if (targetPrimitiveType == SchemaPrimitiveType.ENUM) {
            var enumDeclaration =
                memberTypeSymbol.DeclaringSyntaxReferences[0].GetSyntax() as
                    EnumDeclarationSyntax;
            var baseList = enumDeclaration.BaseList;
            if (baseList != null) {
              var type = baseList.Types[0].Type as PredefinedTypeSyntax;
              formatNumberType =
                  SchemaStructureParser.ParseNumber(type.Keyword.ValueText);
            }
          }

          var formatAttribute =
              SymbolTypeUtil.GetAttribute<FormatAttribute>(memberSymbol);
          if (formatAttribute != null) {
            formatNumberType = formatAttribute.NumberType;

            var isPrimitiveTypeNumeric =
                SchemaStructureParser.IsPrimitiveTypeNumeric_(
                    targetPrimitiveType);
            if (!(targetMemberType is PrimitiveMemberType &&
                  isPrimitiveTypeNumeric)) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules.UnexpectedAttribute));
            }
          }
          if (targetMemberType is PrimitiveMemberType primitiveMemberType) {
            if (formatNumberType != SchemaNumberType.UNDEFINED) {
              primitiveMemberType.UseAltFormat = true;
              primitiveMemberType.AltFormat = formatNumberType;
            } else if (targetPrimitiveType == SchemaPrimitiveType.ENUM) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol, Rules.EnumNeedsFormat));
            }
          }
        }

        {
          // TODO: Implement this, support strings in arrays?
          var lengthSourceAttribute =
              SymbolTypeUtil.GetAttribute<StringLengthSourceAttribute>(
                  memberSymbol);
          if (memberType is IStringType) {
            if (!isMemberReadonly) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol, Rules.NotSupported));
            }
            // TODO: Use length
          } else if (lengthSourceAttribute != null) {
            diagnostics.Add(
                Rules.CreateDiagnostic(memberSymbol,
                                       Rules.UnexpectedAttribute));
          }
        }

        {
          var lengthSourceAttribute =
              SymbolTypeUtil.GetAttribute<ArrayLengthSourceAttribute>(
                  memberSymbol);
          if (memberType is SequenceMemberType sequenceMemberType) {
            if (sequenceMemberType.LengthType ==
                SequenceLengthType.UNSPECIFIED) {
              if (lengthSourceAttribute != null) {
                sequenceMemberType.LengthType =
                    lengthSourceAttribute.Method;

                switch (sequenceMemberType.LengthType) {
                  case SequenceLengthType.IMMEDIATE_VALUE: {
                    sequenceMemberType.ImmediateLengthType =
                        lengthSourceAttribute.LengthType;
                    break;
                  }
                  case SequenceLengthType.OTHER_MEMBER: {
                    // TODO: Look up variable from name
                    diagnostics.Add(
                        Rules.CreateDiagnostic(
                            memberSymbol,
                            Rules.NotSupported));
                    break;
                  }
                  default:
                    throw new NotImplementedException();
                }
              } else {
                diagnostics.Add(
                    Rules.CreateDiagnostic(
                        memberSymbol,
                        Rules.MutableArrayNeedsLengthSource));
              }
            }
            // Didn't expect attribute b/c length is already specified
            else if (lengthSourceAttribute != null) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules.UnexpectedAttribute));
            }
          }
          // Didn't expect attribute b/c not an array
          else if (lengthSourceAttribute != null) {
            diagnostics.Add(
                Rules.CreateDiagnostic(memberSymbol,
                                       Rules.UnexpectedAttribute));
          }
        }

        if (memberType != null) {
          fields.Add(new SchemaMember {
              Name = memberSymbol.Name,
              MemberType = memberType,
          });
        }
      }

      return new SchemaStructure {
          Diagnostics = diagnostics,
          TypeSymbol = structureSymbol,
          Members = fields,
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

    private static bool IsPrimitiveTypeNumeric_(SchemaPrimitiveType type)
      => type switch {
          SchemaPrimitiveType.SBYTE  => true,
          SchemaPrimitiveType.BYTE   => true,
          SchemaPrimitiveType.INT16  => true,
          SchemaPrimitiveType.UINT16 => true,
          SchemaPrimitiveType.INT32  => true,
          SchemaPrimitiveType.UINT32 => true,
          SchemaPrimitiveType.INT64  => true,
          SchemaPrimitiveType.UINT64 => true,
          SchemaPrimitiveType.SINGLE => true,
          SchemaPrimitiveType.DOUBLE => true,
          SchemaPrimitiveType.SN16   => true,
          SchemaPrimitiveType.UN16   => true,
          SchemaPrimitiveType.ENUM   => true,

          SchemaPrimitiveType.CHAR      => false,
          SchemaPrimitiveType.UNDEFINED => false,
          _                             => throw new NotImplementedException(),
      };

    public static SchemaNumberType ParseNumber(string valueText)
      => valueText switch {
          "byte"   => SchemaNumberType.BYTE,
          "sbyte"  => SchemaNumberType.SBYTE,
          "short"  => SchemaNumberType.INT16,
          "ushort" => SchemaNumberType.UINT16,
          "int"    => SchemaNumberType.INT32,
          "uint"   => SchemaNumberType.UINT32,
          "long"   => SchemaNumberType.INT64,
          "ulong"  => SchemaNumberType.UINT64,
          _        => throw new NotImplementedException(),
      };

    private class SchemaStructure : ISchemaStructure {
      public IList<Diagnostic> Diagnostics { get; set; }
      public INamedTypeSymbol TypeSymbol { get; set; }
      public IReadOnlyList<ISchemaMember> Members { get; set; }
    }


    public class SchemaMember : ISchemaMember {
      public string Name { get; set; }
      public IMemberType MemberType { get; set; }
    }

    public class PrimitiveMemberType : IPrimitiveMemberType {
      public ITypeSymbol TypeSymbol { get; set; }

      public SchemaPrimitiveType PrimitiveType { get; set; }
      public bool IsConst { get; set; }

      public bool UseAltFormat { get; set; }
      public SchemaNumberType AltFormat { get; set; }
    }

    public class StructureMemberType : IStructureMemberType {
      public ITypeSymbol TypeSymbol { get; set; }
      public bool IsReferenceType { get; set; }
    }

    public class StringType : IStringType {
      public ITypeSymbol TypeSymbol { get; set; }

      public bool IsConst { get; set; }

      public bool IsNullTerminated { get; set; }

      public StringLengthType LengthType { get; set; }
      public IntType ImmediateLengthType { get; set; }
      public ISchemaMember? LengthMember { get; set; }
      public int ConstLength { get; set; }
    }

    private class SequenceBundle {
      public ITypeSymbol TypeSymbol { get; set; }
      public bool IsArray { get; set; }
      public bool IsLengthConst { get; set; }
      public bool AreElementsConst { get; set; }
      public ITypeSymbol ElementTypeSymbol { get; set; }
    }

    public class SequenceMemberType : ISequenceMemberType {
      public ITypeSymbol TypeSymbol { get; set; }

      public SequenceType SequenceType { get; set; }

      public SequenceLengthType LengthType { get; set; }
      public IntType ImmediateLengthType { get; set; }
      public ISchemaMember? LengthMember { get; set; }

      public IMemberType ElementType { get; set; }
    }
  }
}