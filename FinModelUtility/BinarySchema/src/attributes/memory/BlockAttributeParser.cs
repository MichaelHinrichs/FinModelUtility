using Microsoft.CodeAnalysis;


namespace schema.attributes.memory {
  public class BlockAttributeParser {
    public void CheckMember(
        ISymbol memberSymbol,
        ISchemaMember schemaMember) {
      var blockAttribute =
          SymbolTypeUtil.GetAttribute<BlockAttribute>(memberSymbol);
    }
  }
}