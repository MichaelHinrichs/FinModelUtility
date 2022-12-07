using Microsoft.CodeAnalysis;
using System.Collections.Generic;


namespace schema.attributes.size {
  internal class SizeOfMemberInBytesParser {
    public void Parse(IList<Diagnostic> diagnostics,
                      ISymbol memberSymbol) {
      var sizeOfAttribute =
          SymbolTypeUtil.GetAttribute<SizeOfMemberInBytesAttribute>(
              diagnostics, memberSymbol);
      if (sizeOfAttribute == null) {
        return;
      }

      TypeChainUtil.AssertAllNodesInTypeChainUseBinarySchema(
          diagnostics, sizeOfAttribute.TypeChainToOtherMember);

      // TODO: Remember type chain
    }
  }
}