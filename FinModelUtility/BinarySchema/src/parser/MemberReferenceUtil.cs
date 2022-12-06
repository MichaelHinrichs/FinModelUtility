using schema.attributes;
using schema.util;
using static schema.SchemaStructureParser;
using System;

namespace schema.parser {
  internal static class MemberReferenceUtil {
    public static IMemberType WrapTypeInfoWithMemberType(ITypeInfo typeInfo) {
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
            return new StringType { TypeInfo = typeInfo, };
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
                MemberReferenceUtil.WrapTypeInfoWithMemberType(constraintTypeInfo);

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
                                       ? SequenceLengthSourceType.READONLY
                                       : SequenceLengthSourceType.UNSPECIFIED
            };
          }
        default: throw new ArgumentOutOfRangeException(nameof(typeInfo));
      }
    }

    public static SchemaMember WrapMemberReference(IMemberReference memberReference)
      => new() {
        Name = memberReference.Name,
        MemberType = MemberReferenceUtil.WrapTypeInfoWithMemberType(
              memberReference.MemberTypeInfo),
      };
  }
}
