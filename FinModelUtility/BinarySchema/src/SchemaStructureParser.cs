using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using schema.attributes;
using schema.attributes.align;
using schema.attributes.child_of;
using schema.attributes.ignore;
using schema.attributes.offset;
using schema.attributes.position;
using schema.parser;
using schema.parser.asserts;
using schema.util;


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

    HALF,
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
    IOffset? Offset { get; }
    public bool IsPosition { get; }
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
    IStructureTypeInfo StructureTypeInfo { get; }
    bool IsReferenceType { get; }
    bool IsChild { get; }
  }

  public interface IGenericMemberType : IMemberType {
    IMemberType ConstraintType { get; }
    IGenericTypeInfo GenericTypeInfo { get; }
  }

  public enum IfBooleanSourceType {
    UNSPECIFIED,
    IMMEDIATE_VALUE,
    OTHER_MEMBER,
  }

  public interface IIfBoolean {
    IfBooleanSourceType SourceType { get; }

    SchemaIntegerType ImmediateBooleanType { get; }
    ISchemaMember? BooleanMember { get; }
  }

  public interface IOffset {
    ISchemaMember StartIndexName { get; }
    ISchemaMember OffsetName { get; }
  }

  public enum StringLengthSourceType {
    UNSPECIFIED,
    IMMEDIATE_VALUE,
    OTHER_MEMBER,
    CONST,
    NULL_TERMINATED,
  }

  public interface IStringType : IMemberType {
    // TODO: Support char format?
    /// <summary>
    ///   Whether the string has a set length. This is required for non-null
    ///   terminated strings.
    ///
    ///   If this is set on a null-terminated string, this will function as
    ///   the max length (minus one, since the null-terminator is included).
    /// </summary>
    StringLengthSourceType LengthSourceType { get; }

    SchemaIntegerType ImmediateLengthType { get; }
    ISchemaMember? LengthMember { get; }
    int ConstLength { get; }
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
    SchemaIntegerType ImmediateLengthType { get; }
    ISchemaMember? LengthMember { get; }

    IMemberType ElementType { get; }
  }

  public class SchemaStructureParser : ISchemaStructureParser {
    public ISchemaStructure ParseStructure(INamedTypeSymbol structureSymbol) {
      var diagnostics = new List<Diagnostic>();

      // All of the types that contain the structure need to be partial
      new PartialContainerAsserter(diagnostics).AssertContainersArePartial(
          structureSymbol);

      var iChildOfParser = new IChildOfParser(diagnostics);
      var parentTypeSymbol =
          iChildOfParser.GetParentTypeSymbolOf(structureSymbol);
      if (parentTypeSymbol != null) {
        iChildOfParser.AssertParentContainsChild(
            parentTypeSymbol, structureSymbol);
      }

      var typeInfoParser = new TypeInfoParser();
      var parsedMembers =
          typeInfoParser.ParseMembers(structureSymbol).ToArray();

      var fields = new List<ISchemaMember>();
      foreach (var (parseStatus, memberSymbol, memberTypeInfo) in
               parsedMembers) {
        if (parseStatus == TypeInfoParser.ParseStatus.NOT_A_FIELD_OR_PROPERTY) {
          continue;
        }

        if (SymbolTypeUtil.GetAttribute<IgnoreAttribute>(memberSymbol) !=
            null) {
          continue;
        }

        // Skips parent field for child types
        if (memberSymbol.Name == nameof(IChildOf<IBiSerializable>.Parent)
            && parentTypeSymbol != null) {
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

        // Gets the type of the current member
        var memberType = WrapTypeInfoWithMemberType(memberTypeInfo);

        // Get attributes
        var align = new AlignAttributeParser().GetAlignForMember(memberSymbol);

        var isPosition = false;
        {
          var positionAttribute =
              SymbolTypeUtil.GetAttribute<PositionAttribute>(memberSymbol);
          if (positionAttribute != null) {
            isPosition = true;
            if (memberTypeInfo is not IIntegerTypeInfo {
                    IntegerType: SchemaIntegerType.INT64
                }) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(
                      memberSymbol,
                      Rules.NotSupported));
            }
          }
        }

        IIfBoolean? ifBoolean = null;
        {
          var ifBooleanAttribute =
              SymbolTypeUtil.GetAttribute<IfBooleanAttribute>(memberSymbol);
          if (ifBooleanAttribute != null) {
            if (memberTypeInfo.IsNullable) {
              SchemaMember? booleanMember = null;
              if (ifBooleanAttribute.Method ==
                  IfBooleanSourceType.OTHER_MEMBER) {
                booleanMember =
                    WrapMemberReference(ifBooleanAttribute.OtherMember!);
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

        IOffset? offset = null;
        {
          var offsetAttribute =
              SymbolTypeUtil.GetAttribute<OffsetAttribute>(memberSymbol);

          if (offsetAttribute != null) {
            var startIndexName = offsetAttribute.StartIndexName;
            var startIndexTypeSymbol =
                SymbolTypeUtil.GetTypeFromMemberRelativeToAnother(
                    structureSymbol, startIndexName, memberSymbol.Name);
            typeInfoParser.ParseTypeSymbol(
                startIndexTypeSymbol,
                true,
                out var startIndexTypeInfo);

            var offsetName = offsetAttribute.OffsetName;
            var offsetTypeSymbol =
                SymbolTypeUtil.GetTypeFromMemberRelativeToAnother(structureSymbol, offsetName, memberSymbol.Name);
            typeInfoParser.ParseTypeSymbol(
                offsetTypeSymbol,
                true,
                out var offsetTypeInfo);

            offset = new Offset {
                StartIndexName = new SchemaMember {
                    Name = startIndexName,
                    MemberType = WrapTypeInfoWithMemberType(startIndexTypeInfo),
                },
                OffsetName = new SchemaMember {
                    Name = offsetName,
                    MemberType = WrapTypeInfoWithMemberType(offsetTypeInfo),
                }
            };
          }
        }

        new SupportedElementTypeAsserter(diagnostics)
            .AssertElementTypesAreSupported(memberSymbol, memberType);

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
          var formatIntegerType = SchemaIntegerType.UNDEFINED;

          if (targetPrimitiveType == SchemaPrimitiveType.ENUM) {
            var enumNamedTypeSymbol =
                targetMemberType.TypeSymbol as INamedTypeSymbol;
            var underlyingType = enumNamedTypeSymbol!.EnumUnderlyingType;

            formatIntegerType =
                SchemaPrimitiveTypesUtil.GetIntegerTypeFromTypeSymbol(
                    underlyingType);
          }

          {
            var numberFormatAttribute =
                SymbolTypeUtil
                    .GetAttribute<NumberFormatAttribute>(memberSymbol);
            if (numberFormatAttribute != null) {
              formatNumberType = numberFormatAttribute.NumberType;

              var canPrimitiveTypeBeReadAsNumber =
                  SchemaPrimitiveTypesUtil.CanPrimitiveTypeBeReadAsNumber(
                      targetPrimitiveType);
              if (!(targetMemberType is PrimitiveMemberType &&
                    canPrimitiveTypeBeReadAsNumber)) {
                diagnostics.Add(
                    Rules.CreateDiagnostic(memberSymbol,
                                           Rules.UnexpectedAttribute));
              }
            }
          }

          {
            var integerFormatAttribute =
                SymbolTypeUtil
                    .GetAttribute<IntegerFormatAttribute>(memberSymbol);
            if (integerFormatAttribute != null) {
              formatIntegerType = integerFormatAttribute.IntegerType;

              var canPrimitiveTypeBeReadAsInteger =
                  SchemaPrimitiveTypesUtil.CanPrimitiveTypeBeReadAsInteger(
                      targetPrimitiveType);
              if (!(targetMemberType is PrimitiveMemberType &&
                    canPrimitiveTypeBeReadAsInteger)) {
                diagnostics.Add(
                    Rules.CreateDiagnostic(memberSymbol,
                                           Rules.UnexpectedAttribute));
              }
            }
          }

          if (formatNumberType == SchemaNumberType.UNDEFINED &&
              formatIntegerType != SchemaIntegerType.UNDEFINED) {
            formatNumberType =
                SchemaPrimitiveTypesUtil
                    .ConvertIntToNumber(formatIntegerType);
          }

          if (targetMemberType is PrimitiveMemberType primitiveMemberType) {
            if (formatNumberType != SchemaNumberType.UNDEFINED) {
              primitiveMemberType.UseAltFormat = true;
              primitiveMemberType.AltFormat = formatNumberType;
            } else if (targetPrimitiveType == SchemaPrimitiveType.ENUM) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules.EnumNeedsIntegerFormat));
            } else if (targetPrimitiveType == SchemaPrimitiveType.BOOLEAN) {
              diagnostics.Add(
                  Rules.CreateDiagnostic(memberSymbol,
                                         Rules.BooleanNeedsIntegerFormat));
            }
          }

          // Checks if the member is a child of the current type
          {
            if (targetMemberType is StructureMemberType structureMemberType) {
              var expectedParentTypeSymbol =
                  iChildOfParser.GetParentTypeSymbolOf(
                      structureMemberType.StructureTypeInfo.NamedTypeSymbol);
              if (expectedParentTypeSymbol != null) {
                if (expectedParentTypeSymbol.Equals(structureSymbol)) {
                  structureMemberType.IsChild = true;
                } else {
                  diagnostics.Add(Rules.CreateDiagnostic(
                                      memberSymbol,
                                      Rules
                                          .ChildTypeCanOnlyBeContainedInParent));
                }
              }
            }
          }
        }

        {
          // TODO: Implement this, support strings in arrays?
          var stringLengthSourceAttribute =
              SymbolTypeUtil.GetAttribute<StringLengthSourceAttribute>(
                  memberSymbol);
          var nullTerminatedStringAttribute =
              SymbolTypeUtil.GetAttribute<NullTerminatedStringAttribute>(
                  memberSymbol);

          if (stringLengthSourceAttribute != null ||
              nullTerminatedStringAttribute != null) {
            if (memberType is StringType stringType) {
              if (stringLengthSourceAttribute != null &&
                  nullTerminatedStringAttribute != null) {
                diagnostics.Add(
                    Rules.CreateDiagnostic(memberSymbol,
                                           Rules.NotSupported));
              }

              if (memberTypeInfo.IsReadonly) {
                diagnostics.Add(
                    Rules.CreateDiagnostic(memberSymbol,
                                           Rules.UnexpectedAttribute));
              }

              var method =
                  stringLengthSourceAttribute?.Method ??
                  StringLengthSourceType.NULL_TERMINATED;

              switch (method) {
                case StringLengthSourceType.CONST: {
                  stringType.LengthSourceType = StringLengthSourceType.CONST;
                  stringType.ConstLength =
                      stringLengthSourceAttribute!.ConstLength;
                  break;
                }
                case StringLengthSourceType.NULL_TERMINATED: {
                  stringType.LengthSourceType =
                      StringLengthSourceType.NULL_TERMINATED;
                  break;
                }
                // TODO: Support int length strings
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
                    sequenceMemberType.LengthMember =
                        WrapMemberReference(lengthSourceAttribute.OtherMember);
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
              Offset = offset,
              IsPosition = isPosition,
          });
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
      public IOffset Offset { get; set; }
      public bool IsPosition { get; set; }
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
      public IStructureTypeInfo StructureTypeInfo { get; set; }
      public ITypeInfo TypeInfo => StructureTypeInfo;
      public ITypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
      public bool IsReadonly => this.TypeInfo.IsReadonly;

      public bool IsReferenceType { get; set; }
      public bool IsChild { get; set; }
    }

    public class GenericMemberType : IGenericMemberType {
      public IMemberType ConstraintType { get; set; }
      public IGenericTypeInfo GenericTypeInfo { get; set; }
      public ITypeInfo TypeInfo => GenericTypeInfo;
      public ITypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
      public bool IsReadonly => this.TypeInfo.IsReadonly;
    }

    public class IfBoolean : IIfBoolean {
      public IfBooleanSourceType SourceType { get; set; }
      public SchemaIntegerType ImmediateBooleanType { get; set; }
      public ISchemaMember? BooleanMember { get; set; }
    }

    public class Offset : IOffset {
      public ISchemaMember StartIndexName { get; set; }
      public ISchemaMember OffsetName { get; set; }
    }

    public class StringType : IStringType {
      public ITypeInfo TypeInfo { get; set; }
      public ITypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
      public bool IsReadonly => this.TypeInfo.IsReadonly;

      public StringLengthSourceType LengthSourceType { get; set; }
      public SchemaIntegerType ImmediateLengthType { get; set; }
      public ISchemaMember? LengthMember { get; set; }
      public int ConstLength { get; set; }
    }

    public class SequenceMemberType : ISequenceMemberType {
      public ISequenceTypeInfo SequenceTypeInfo { get; set; }
      public ITypeInfo TypeInfo => SequenceTypeInfo;
      public ITypeSymbol TypeSymbol => TypeInfo.TypeSymbol;
      public bool IsReadonly => this.TypeInfo.IsReadonly;

      public SequenceType SequenceType { get; set; }

      public SequenceLengthSourceType LengthSourceType { get; set; }
      public SchemaIntegerType ImmediateLengthType { get; set; }
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
              PrimitiveTypeInfo =
                  Asserts.CastNonnull(typeInfo as IPrimitiveTypeInfo),
          };
        }
        case IStringTypeInfo stringTypeInfo: {
          return new StringType {TypeInfo = typeInfo,};
        }
        case IStructureTypeInfo structureTypeInfo: {
          return new StructureMemberType {
              StructureTypeInfo = structureTypeInfo,
              IsReferenceType =
                  structureTypeInfo.NamedTypeSymbol.IsReferenceType,
          };
        }
        case IGenericTypeInfo genericTypeInfo: {
          // TODO: Figure out how to find the best constraint
          var constraintTypeInfo = genericTypeInfo.ConstraintTypeInfos[0];
          var constraintMemberType =
              this.WrapTypeInfoWithMemberType(constraintTypeInfo);

          return new GenericMemberType {
              ConstraintType = constraintMemberType,
              GenericTypeInfo = genericTypeInfo,
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

    public SchemaMember WrapMemberReference(IMemberReference memberReference)
      => new() {
          Name = memberReference.Name,
          MemberType = WrapTypeInfoWithMemberType(
              memberReference.MemberTypeInfo),
      };
  }
}