using System;
using System.Collections.Generic;
using System.Linq;

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

    BOOLEAN,

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

    CHAR,

    ENUM,
  }

  public interface ISchemaMember {
    string Name { get; }
    IMemberType MemberType { get; }
    int Align { get; }
    IIfBoolean? IfBoolean { get; }
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

  public enum IfBooleanSourceType {
    UNSPECIFIED,
    IMMEDIATE_VALUE,
    OTHER_MEMBER,
  }

  public interface IIfBoolean {
    IfBooleanSourceType SourceType { get; }

    IntType ImmediateBooleanType { get; }
    ISchemaMember? BooleanMember { get; }
  }

  public enum StringLengthSourceType {
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
    StringLengthSourceType LengthSourceType { get; }

    IntType ImmediateLengthType { get; }
    ISchemaMember? LengthMember { get; }
    int ConstLength { get; }

    bool IsEndianOrdered { get; }
  }

  public enum SequenceType {
    ARRAY,
    LIST,
  }

  public enum SequenceLengthSourceType {
    UNSPECIFIED,
    IMMEDIATE_VALUE,
    OTHER_MEMBER,
    CONST,
  }

  public interface ISequenceMemberType : IMemberType {
    SequenceType SequenceType { get; }

    SequenceLengthSourceType LengthSourceType { get; }
    IntType ImmediateLengthType { get; }
    ISchemaMember? LengthMember { get; }

    IMemberType ElementType { get; }
  }

  public class SchemaStructureParser : ISchemaStructureParser {
    public ISchemaStructure ParseStructure(
        INamedTypeSymbol structureSymbol) {
      var diagnostics = new List<Diagnostic>();

      // All of the types that contain the structure need to be partial
      {
        var containingType = structureSymbol.ContainingType;
        while (containingType != null) {
          var typeDeclarationSyntax =
              containingType.DeclaringSyntaxReferences[0].GetSyntax() as
                  TypeDeclarationSyntax;

          if (!SymbolTypeUtil.IsPartial(typeDeclarationSyntax!)) {
            diagnostics.Add(Rules.CreateDiagnostic(
                                containingType,
                                Rules.ContainerTypeMustBePartial));
          }

          containingType = containingType.ContainingType;
        }
      }

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

        var align = 0;
        {
          var alignAttribute =
              SymbolTypeUtil.GetAttribute<AlignAttribute>(memberSymbol);
          if (alignAttribute != null) {
            align = alignAttribute.Align;
          }
        }

        IIfBoolean? ifBoolean = null;
        {
          var ifBooleanAttribute =
              SymbolTypeUtil.GetAttribute<IfBooleanAttribute>(memberSymbol);
          if (ifBooleanAttribute != null) {
            if (memberTypeSymbol.NullableAnnotation ==
                NullableAnnotation.Annotated) {
              SchemaMember? booleanMember = null;
              if (ifBooleanAttribute.Method ==
                  IfBooleanSourceType.OTHER_MEMBER) {
                var booleanMemberName = ifBooleanAttribute.OtherMemberName;
                var booleanMemberTypeSymbol =
                    SymbolTypeUtil.GetTypeFromMember(
                        structureSymbol, booleanMemberName!);
                var booleanMemberPrimitiveType =
                    SchemaStructureParser.GetPrimitiveTypeFromType_(
                        booleanMemberTypeSymbol);
                booleanMember =
                    new SchemaMember {
                        Name = booleanMemberName,
                        MemberType = new PrimitiveMemberType {
                            TypeSymbol = booleanMemberTypeSymbol,
                            PrimitiveType = booleanMemberPrimitiveType,
                        }
                    };
              }

              ifBoolean = new IfBoolean {
                  SourceType = ifBooleanAttribute.Method,
                  ImmediateBooleanType = ifBooleanAttribute.BooleanType,
                  BooleanMember = booleanMember,
              };
            } else {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules
                                             .IfBooleanNeedsNullable));
            }
          }
        }

        {
          if (memberTypeSymbol is INamedTypeSymbol {
                  Name: "Nullable"
              } fieldNamedTypeSymbol) {
            memberTypeSymbol = fieldNamedTypeSymbol.TypeArguments[0];
          }
        }

        {
          var primitiveType =
              SchemaStructureParser.GetPrimitiveTypeFromType_(memberTypeSymbol);
          if (primitiveType != SchemaPrimitiveType.UNDEFINED) {
            memberType = new PrimitiveMemberType {
                TypeSymbol = memberTypeSymbol,
                IsConst = isMemberReadonly,
                PrimitiveType = primitiveType,
            };
          } else if
              (memberTypeSymbol.SpecialType ==
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
              (memberTypeSymbol is INamedTypeSymbol fieldNamedTypeSymbol) {
            if (SymbolTypeUtil.Implements(fieldNamedTypeSymbol,
                                          typeof(IBiSerializable))) {
              memberType = new StructureMemberType {
                  TypeSymbol = memberTypeSymbol,
                  IsReferenceType = fieldNamedTypeSymbol.IsReferenceType,
              };
            } else {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules
                                             .StructureMemberNeedsToImplementIBiSerializable));
            }
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
          } else if (elementTypeSymbol is INamedTypeSymbol
                     elementNamedTypeSymbol) {
            if (SymbolTypeUtil.Implements(elementNamedTypeSymbol,
                                          typeof(IBiSerializable))) {
              elementMemberType = new StructureMemberType {
                  TypeSymbol = elementTypeSymbol,
                  IsReferenceType = elementNamedTypeSymbol.IsReferenceType,
              };
            } else {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules
                                             .StructureMemberNeedsToImplementIBiSerializable));
            }
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
                LengthSourceType = sequenceBundle.IsLengthConst
                                       ? SequenceLengthSourceType.CONST
                                       : SequenceLengthSourceType.UNSPECIFIED,
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
                targetMemberType.TypeSymbol.DeclaringSyntaxReferences[0]
                                .GetSyntax() as
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
            } else if (targetPrimitiveType == SchemaPrimitiveType.BOOLEAN) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules.BooleanNeedsFormat));
            }
          }
        }

        {
          var endianOrderedAttribute =
              SymbolTypeUtil.GetAttribute<EndianOrderedAttribute>(memberSymbol);
          if (endianOrderedAttribute != null) {
            if (memberType is StringType stringMemberType) {
              stringMemberType.IsEndianOrdered = true;

              if (stringMemberType.IsNullTerminated) {
                diagnostics.Add(
                    Rules.CreateDiagnostic(memberSymbol,
                                           Rules.UnexpectedAttribute));
              }
            } else {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules.UnexpectedAttribute));
            }
          }
        }

        {
          // TODO: Implement this, support strings in arrays?
          var lengthSourceAttribute =
              SymbolTypeUtil.GetAttribute<StringLengthSourceAttribute>(
                  memberSymbol);
          if (lengthSourceAttribute != null) {
            if (memberType is StringType stringType) {
              if (isMemberReadonly) {
                diagnostics.Add(
                    Rules.CreateDiagnostic(memberSymbol,
                                           Rules.UnexpectedAttribute));
              }

              switch (lengthSourceAttribute.Method) {
                case StringLengthSourceType.CONST: {
                  stringType.LengthSourceType = StringLengthSourceType.CONST;
                  stringType.ConstLength = lengthSourceAttribute.ConstLength;
                  break;
                }
                default: {
                  diagnostics.Add(
                      Rules.CreateDiagnostic(memberSymbol,
                                             Rules.NotSupported));
                  break;
                }
              }
            } else {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules.UnexpectedAttribute));
            }
          }
        }

        {
          var lengthSourceAttribute =
              SymbolTypeUtil.GetAttribute<ArrayLengthSourceAttribute>(
                  memberSymbol);
          if (memberType is SequenceMemberType sequenceMemberType) {
            if (sequenceMemberType.LengthSourceType ==
                SequenceLengthSourceType.UNSPECIFIED) {
              if (lengthSourceAttribute != null) {
                sequenceMemberType.LengthSourceType =
                    lengthSourceAttribute.Method;

                switch (sequenceMemberType.LengthSourceType) {
                  case SequenceLengthSourceType.IMMEDIATE_VALUE: {
                    sequenceMemberType.ImmediateLengthType =
                        lengthSourceAttribute.LengthType;
                    break;
                  }
                  case SequenceLengthSourceType.OTHER_MEMBER: {
                    // TODO: Verify whether it exists, type, stuff
                    var otherLengthMemberSymbolName =
                        lengthSourceAttribute.OtherMemberName;
                    var otherLengthMemberTypeSymbol =
                        SymbolTypeUtil.GetTypeFromMember(
                            structureSymbol,
                            otherLengthMemberSymbolName);

                    ISchemaMember? otherLengthMember = null;
                    var otherLengthPrimitiveType =
                        SchemaStructureParser.GetPrimitiveTypeFromType_(
                            otherLengthMemberTypeSymbol);

                    // TODO: Assert this
                    var isOtherLengthNumeric =
                        SchemaStructureParser.IsPrimitiveTypeNumeric_(
                            otherLengthPrimitiveType);

                    if (otherLengthPrimitiveType !=
                        SchemaPrimitiveType.UNDEFINED) {
                      otherLengthMember = new SchemaMember {
                          Name = otherLengthMemberSymbolName,
                          MemberType = new PrimitiveMemberType {
                              TypeSymbol = otherLengthMemberTypeSymbol,
                              PrimitiveType = otherLengthPrimitiveType,
                          }
                      };
                    }

                    if (otherLengthMember != null) {
                      sequenceMemberType.LengthMember = otherLengthMember;
                    } else {
                      diagnostics.Add(
                          Rules.CreateDiagnostic(
                              memberSymbol,
                              Rules.NotSupported));
                    }
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
              Align = align,
              IfBoolean = ifBoolean,
          });
        }
      }

      {
        var memberByName = new Dictionary<string, ISchemaMember>();
        foreach (var member in fields) {
          memberByName[member.Name] = member;
        }

        foreach (var member in fields) {
          var ifBooleanMemberName = member.IfBoolean?.BooleanMember?.Name;
          if (ifBooleanMemberName != null) {
            var mutableIfBoolean = member.IfBoolean as IfBoolean;
            mutableIfBoolean!.BooleanMember = memberByName[ifBooleanMemberName];
          }
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

    private static bool IsPrimitiveTypeNumeric_(SchemaPrimitiveType type)
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
      public int Align { get; set; }
      public IIfBoolean IfBoolean { get; set; }
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

    public class IfBoolean : IIfBoolean {
      public IfBooleanSourceType SourceType { get; set; }
      public IntType ImmediateBooleanType { get; set; }
      public ISchemaMember? BooleanMember { get; set; }
    }

    public class StringType : IStringType {
      public ITypeSymbol TypeSymbol { get; set; }

      public bool IsConst { get; set; }

      public bool IsNullTerminated { get; set; }

      public StringLengthSourceType LengthSourceType { get; set; }
      public IntType ImmediateLengthType { get; set; }
      public ISchemaMember? LengthMember { get; set; }
      public int ConstLength { get; set; }

      public bool IsEndianOrdered { get; set; }
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

      public SequenceLengthSourceType LengthSourceType { get; set; }
      public IntType ImmediateLengthType { get; set; }
      public ISchemaMember? LengthMember { get; set; }

      public IMemberType ElementType { get; set; }
    }
  }
}