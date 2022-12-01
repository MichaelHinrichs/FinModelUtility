using Microsoft.CodeAnalysis;
using System.IO;


namespace schema.attributes.endianness {
  public class EndiannessParser {
    public Endianness? GetEndianness(ISymbol symbol) {
      var endiannessAttribute =
          SymbolTypeUtil.GetAttribute<EndiannessAttribute>(symbol);
      return endiannessAttribute?.Endianness;
    }
  }
}