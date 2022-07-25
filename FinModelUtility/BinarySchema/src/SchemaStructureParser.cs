using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.CodeAnalysis;

using schema.attributes.align;
using schema.parser;
using schema.parser.asserts;


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
    ITypeInfo TypeInfo { get; }
    ITypeSymbol TypeSymbol { get; }
    bool IsReadonly { get; }
  }

  public interface IPrimitiveMemberType : IMemberType {
    SchemaPrimitiveType PrimitiveType { get; }
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

    SchemaIntType ImmediateBooleanType { get; }
    ISchemaMember? BooleanMember { get; }
  }

  public enum StringLengthSourceType {
    UNSPECIFIED,
    IMMEDIATE_VALUE,
    OTHER_MEMBER,
    CONST,
  }

  public interface IStringType : IMemberType {
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

    SchemaIntType ImmediateLengthType { get; }
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
    ISequenceTypeInfo SequenceTypeInfo { get; }

    SequenceType SequenceType { get; }

    SequenceLengthSourceType LengthSourceType { get; }
    SchemaIntType ImmediateLengthType { get; }
    ISchemaMember? LengthMember { get; }

    IMemberType ElementType { get; }
  }

  public class SchemaStructureParser : ISchemaStructureParser {
    public ISchemaStructure ParseStructure(
        INamedTypeSymbol structureSymbol) {
      var diagnostics = new List<Diagnostic>();

      // All of the types that contain the structure need to be partial
      new PartialContainerAsserter(diagnostics).AssertContainersArePartial(
          structureSymbol);

      var typeInfoParser = new TypeInfoParser();

      var fields = new List<ISchemaMember>();
      foreach (var (parseStatus, memberSymbol, memberTypeInfo) in
               typeInfoParser.ParseMembers(structureSymbol)) {
        if (parseStatus == TypeInfoParser.ParseStatus.NOT_A_FIELD_OR_PROPERTY) {
          continue;
        }

        if (parseStatus == TypeInfoParser.ParseStatus.NOT_IMPLEMENTED) {
          diagnostics.Add(
              Rules.CreateDiagnostic(
                  memberSymbol,
                  Rules.NotSupported));
          continue;
        }

        // Makes sure the member is serializable
        {
          if (memberTypeInfo is IStructureTypeInfo structureTypeInfo) {
            // TODO: Check if implements same kind as parent
            if (!SymbolTypeUtil.Implements(
                    structureTypeInfo.NamedTypeSymbol,
                    typeof(IBiSerializable))) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(
                      memberSymbol,
                      Rules.StructureMemberNeedsToImplementIBiSerializable));
              continue;
            }
          }
        }

        // Get attributes
        var memberType = WrapTypeInfoWithMemberType(memberTypeInfo);

        var align = new AlignAttributeParser().GetAlignForMember(memberSymbol);

        IIfBoolean? ifBoolean = null;
        {
          var ifBooleanAttribute =
              SymbolTypeUtil.GetAttribute<IfBooleanAttribute>(memberSymbol);
          if (ifBooleanAttribute != null) {
            if (memberTypeInfo.IsNullable) {
              SchemaMember? booleanMember = null;
              if (ifBooleanAttribute.Method ==
                  IfBooleanSourceType.OTHER_MEMBER) {
                var booleanMemberName = ifBooleanAttribute.OtherMemberName;
                var booleanMemberTypeSymbol =
                    SymbolTypeUtil.GetTypeFromMember(
                        structureSymbol, booleanMemberName!);
                var booleanMemberParseStatus =
                    typeInfoParser.ParseTypeSymbol(
                        booleanMemberTypeSymbol,
                        true,
                        out var booleanMemberTypeInfo);

                // TODO: Handle with better errors
                if (booleanMemberParseStatus !=
                    TypeInfoParser.ParseStatus.SUCCESS
                    || booleanMemberTypeInfo is not IBoolTypeInfo
                        booleanPrimitiveTypeInfo) {
                  diagnostics.Add(
                      Rules.CreateDiagnostic(
                          memberSymbol, Rules.NotSupported));
                  continue;
                }

                booleanMember =
                    new SchemaMember {
                        Name = booleanMemberName,
                        MemberType = new PrimitiveMemberType {
                            PrimitiveTypeInfo = booleanPrimitiveTypeInfo
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
          if (memberType is ISequenceMemberType sequenceMemberType
              && sequenceMemberType.ElementType.TypeInfo is IStructureTypeInfo
                  structureTypeInfo) {
            if (!SymbolTypeUtil.Implements(structureTypeInfo.NamedTypeSymbol,
                                           typeof(IBiSerializable))) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules
                                             .StructureMemberNeedsToImplementIBiSerializable));
              continue;
            }

            // TODO: Make sure array type is actually supported
            /*diagnostics.Add(
                Rules.CreateDiagnostic(memberSymbol,
                                       Rules.UnsupportedArrayType));*/
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
            var enumNamedTypeSymbol =
                targetMemberType.TypeSymbol as INamedTypeSymbol;
            var underlyingType = enumNamedTypeSymbol!.EnumUnderlyingType;

            formatNumberType =
                SchemaPrimitiveTypesUtil.GetNumberTypeFromTypeSymbol(
                    underlyingType);
          }

          var formatAttribute =
              SymbolTypeUtil.GetAttribute<FormatAttribute>(memberSymbol);
          if (formatAttribute != null) {
            formatNumberType = formatAttribute.NumberType;

            var isPrimitiveTypeNumeric =
                SchemaPrimitiveTypesUtil.IsPrimitiveTypeNumeric(
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
              if (memberTypeInfo.IsReadonly) {
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

                    var otherLengthMemberParseStatus =
                        typeInfoParser.ParseTypeSymbol(
                            otherLengthMemberTypeSymbol,
                            true,
                            out var otherLengthMemberTypeInfo);

                    // TODO: Add a better error
                    if (otherLengthMemberParseStatus !=
                        TypeInfoParser.ParseStatus.SUCCESS ||
                        otherLengthMemberTypeInfo is not IIntegerTypeInfo
                            otherLengthMemberIntegerTypeInfo) {
                      diagnostics.Add(
                          Rules.CreateDiagnostic(
                              memberSymbol,
                              Rules.NotSupported));
                      continue;
                    }

                    sequenceMemberType.LengthMember = new SchemaMember {
                        Name = otherLengthMemberSymbolName,
                        MemberType = WrapTypeInfoWithMemberType(
                            otherLengthMemberIntegerTypeInfo),
                    };
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
      public IPrimitiveTypeInfo PrimitiveTypeInfo { get; set; }
      public ITypeInfo TypeInfo => PrimitiveTypeInfo;
      public ITypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
      public bool IsReadonly => this.TypeInfo.IsReadonly;

      public SchemaPrimitiveType PrimitiveType
        => this.PrimitiveTypeInfo.PrimitiveType;

      public bool UseAltFormat { get; set; }
      public SchemaNumberType AltFormat { get; set; }
    }

    public class StructureMemberType : IStructureMemberType {
      public ITypeInfo TypeInfo { get; set; }
      public ITypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
      public bool IsReadonly => this.TypeInfo.IsReadonly;

      public bool IsReferenceType { get; set; }
    }

    public class IfBoolean : IIfBoolean {
      public IfBooleanSourceType SourceType { get; set; }
      public SchemaIntType ImmediateBooleanType { get; set; }
      public ISchemaMember? BooleanMember { get; set; }
    }

    public class StringType : IStringType {
      public ITypeInfo TypeInfo { get; set; }
      public ITypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
      public bool IsReadonly => this.TypeInfo.IsReadonly;

      public bool IsNullTerminated { get; set; }

      public StringLengthSourceType LengthSourceType { get; set; }
      public SchemaIntType ImmediateLengthType { get; set; }
      public ISchemaMember? LengthMember { get; set; }
      public int ConstLength { get; set; }

      public bool IsEndianOrdered { get; set; }
    }

    public class SequenceMemberType : ISequenceMemberType {
      public ISequenceTypeInfo SequenceTypeInfo { get; set; }
      public ITypeInfo TypeInfo => SequenceTypeInfo;
      public ITypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
      public bool IsReadonly => this.TypeInfo.IsReadonly;

      public SequenceType SequenceType { get; set; }

      public SequenceLengthSourceType LengthSourceType { get; set; }
      public SchemaIntType ImmediateLengthType { get; set; }
      public ISchemaMember? LengthMember { get; set; }

      public IMemberType ElementType { get; set; }
    }


    public IMemberType WrapTypeInfoWithMemberType(ITypeInfo typeInfo) {
      switch (typeInfo) {
        case IIntegerTypeInfo integerTypeInfo:
        case INumberTypeInfo numberTypeInfo:
        case IBoolTypeInfo boolTypeInfo:
        case ICharTypeInfo charTypeInfo:
        case IEnumTypeInfo enumTypeInfo: {
          return new PrimitiveMemberType {
              PrimitiveTypeInfo = typeInfo as IPrimitiveTypeInfo,
          };
        }
        case IStringTypeInfo stringTypeInfo: {
          return new StringType {
              TypeInfo = typeInfo,
          };
        }
        case IStructureTypeInfo structureTypeInfo: {
          return new StructureMemberType {
              TypeInfo = typeInfo,
              IsReferenceType =
                  structureTypeInfo.NamedTypeSymbol.IsReferenceType,
          };
        }
        case ISequenceTypeInfo sequenceTypeInfo: {
          return new SequenceMemberType {
              SequenceTypeInfo = sequenceTypeInfo,
              SequenceType = sequenceTypeInfo.IsArray
                                 ? SequenceType.ARRAY
                                 : SequenceType.LIST,
              ElementType =
                  WrapTypeInfoWithMemberType(sequenceTypeInfo.ElementTypeInfo),
              LengthSourceType = sequenceTypeInfo.IsLengthConst
                                     ? SequenceLengthSourceType.CONST
                                     : SequenceLengthSourceType.UNSPECIFIED
          };
        }
        default: throw new ArgumentOutOfRangeException(nameof(typeInfo));
      }
    }
  }
}