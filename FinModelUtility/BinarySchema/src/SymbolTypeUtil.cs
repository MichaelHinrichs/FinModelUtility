using System;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace schema {
  internal static class SymbolTypeUtil {
    public static Type GetTypeFromSymbol(ISymbol symbol) {
      // TODO: Factor in declaration types
      var fieldName = symbol.Name;
      var fullyQualifiedTypeName =
          $"{symbol.ContainingNamespace}.{symbol.Name}";
      return Type.GetType(fullyQualifiedTypeName);
    }

    public static bool IsExactlyType(ISymbol symbol, Type expectedType)
      => SymbolTypeUtil.GetTypeFromSymbol(symbol) == expectedType;

    public static bool HasAttribute(ISymbol symbol, Type expectedType)
      => symbol.GetAttributes()
               .Any(attributeData
                        => SymbolTypeUtil.IsExactlyType(
                            attributeData.AttributeClass,
                            expectedType));

  }
}
