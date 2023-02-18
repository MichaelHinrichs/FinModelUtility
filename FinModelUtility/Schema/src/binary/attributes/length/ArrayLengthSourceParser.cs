using Microsoft.CodeAnalysis;

using static schema.binary.BinarySchemaStructureParser;

using System;
using System.Collections.Generic;

using schema.binary.attributes.array;
using schema.binary.parser;


namespace schema.binary.attributes.length {
  internal class ArrayLengthSourceParser {
    public void Parse(IList<Diagnostic> diagnostics,
                      ISymbol memberSymbol,
                      IMemberType memberType) {
      var lengthSourceAttribute =
          SymbolTypeUtil.GetAttribute<ArrayLengthSourceAttribute>(
              diagnostics,
              memberSymbol);
      var untilEndOfStreamAttribute =
          SymbolTypeUtil.GetAttribute<ArrayUntilEndOfStreamAttribute>(
              diagnostics,
              memberSymbol);

      if (memberType is BinarySchemaStructureParser.SequenceMemberType
          sequenceMemberType) {
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
                    MemberReferenceUtil.WrapMemberReference(
                        lengthSourceAttribute.OtherMember);
                break;
              }
              case SequenceLengthSourceType.CONST_LENGTH: {
                sequenceMemberType.ConstLength =
                    lengthSourceAttribute.ConstLength;
                break;
              }
              default:
                throw new NotImplementedException();
            }
          } else if (untilEndOfStreamAttribute != null) {
            sequenceMemberType.LengthSourceType =
                SequenceLengthSourceType.UNTIL_END_OF_STREAM;
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
  }
}