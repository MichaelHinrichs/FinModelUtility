using Microsoft.CodeAnalysis;
using System.Collections.Generic;


namespace schema.attributes.memory {
  public class BlockAttributeParser {
    public void CheckMember(
        IList<Diagnostic> diagnostics,
        ISymbol memberSymbol,
        ISchemaMember schemaMember) {
      var blockAttribute =
          SymbolTypeUtil.GetAttribute<BlockAttribute>(
              diagnostics, memberSymbol);
    }
  }
}