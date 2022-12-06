using Microsoft.CodeAnalysis;
using System.Collections.Generic;


namespace schema.attributes.size {
  internal class SizeOfMemberInBytesParser {
    public void Parse(IList<Diagnostic> diagnostics,
                      ISymbol memberSymbol,
                      IMemberType memberType) {
      var sizeOfAttribute =
              SymbolTypeUtil.GetAttribute<SizeOfMemberInBytesAttribute>(
                  diagnostics, memberSymbol);
    }
  }
}
