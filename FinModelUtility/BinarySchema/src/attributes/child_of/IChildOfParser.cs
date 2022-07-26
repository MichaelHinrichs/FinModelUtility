using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;


namespace schema.attributes.child_of {
  public class IChildOfParser {
    private readonly IList<Diagnostic> diagnostics_;

    public IChildOfParser(IList<Diagnostic> diagnostics) {
      this.diagnostics_ = diagnostics;
    }

    public INamedTypeSymbol? GetParentTypeSymbolOf(
        INamedTypeSymbol childNamedTypeSymbol) {
      if (!SymbolTypeUtil.ImplementsGeneric(childNamedTypeSymbol,
                                            typeof(IChildOf<>))) {
        return null;
      }

      var parentSymbol = childNamedTypeSymbol
                         .GetMembers(nameof(IChildOf<IBiSerializable>.Parent))
                         .Single();
      return parentSymbol switch {
          IPropertySymbol propertySymbol => propertySymbol.Type,
          IFieldSymbol fieldSymbol       => fieldSymbol.Type,
      } as INamedTypeSymbol;
    }

    public void AssertParentContainsChild(
        INamedTypeSymbol parentNamedTypeSymbol,
        INamedTypeSymbol childNamedTypeSymbol) {
      var containedInClass =
          SymbolTypeUtil
              .GetInstanceMembers(parentNamedTypeSymbol!)
              .Any(memberSymbol => {
                var typeSymbol = memberSymbol switch {
                    IPropertySymbol propertySymbol => propertySymbol.Type,
                    IFieldSymbol fieldSymbol       => fieldSymbol.Type,
                    _ =>
                        throw new NotSupportedException()
                };

                var hasSameName =
                    typeSymbol.Name == childNamedTypeSymbol.Name;
                var hasSameNamespace =
                    SymbolTypeUtil
                        .MergeContainingNamespaces(typeSymbol) ==
                    SymbolTypeUtil
                        .MergeContainingNamespaces(
                            childNamedTypeSymbol);
                var hasSameAssembly =
                    typeSymbol.ContainingAssembly ==
                    childNamedTypeSymbol.ContainingAssembly;

                if (hasSameName && hasSameNamespace &&
                    hasSameAssembly) {
                  return true;
                }

                return false;
              });

      if (!containedInClass) {
        diagnostics_.Add(
            Rules.CreateDiagnostic(childNamedTypeSymbol,
                                   Rules.ChildTypeMustBeContainedInParent));
      }
    }
  }
}