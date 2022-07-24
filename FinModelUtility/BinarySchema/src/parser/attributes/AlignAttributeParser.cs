using Microsoft.CodeAnalysis;


namespace schema.parser.attributes {
  public class AlignAttributeParser {
    public int GetAlignForMember(ISymbol memberSymbol) {
      var alignAttribute =
          SymbolTypeUtil.GetAttribute<AlignAttribute>(memberSymbol);
      return alignAttribute?.Align ?? 0;
    }
  }
}