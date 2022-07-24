using Microsoft.CodeAnalysis;


namespace schema.attributes.align {
  public class AlignAttributeParser {
    public int GetAlignForMember(ISymbol memberSymbol) {
      var alignAttribute =
          SymbolTypeUtil.GetAttribute<AlignAttribute>(memberSymbol);
      return alignAttribute?.Align ?? 0;
    }
  }
}