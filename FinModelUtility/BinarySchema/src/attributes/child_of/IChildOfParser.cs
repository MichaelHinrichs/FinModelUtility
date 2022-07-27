﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using schema.parser;


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
          new TypeInfoParser()
              .ParseMembers(parentNamedTypeSymbol)
              .Any(tuple => {
                var (parseStatus, memberSymbol, memberTypeInfo) = tuple;
                if (parseStatus != TypeInfoParser.ParseStatus.SUCCESS) {
                  return false;
                }

                var elementTypeInfo =
                    (memberTypeInfo is ISequenceTypeInfo sequenceTypeInfo)
                        ? sequenceTypeInfo.ElementTypeInfo
                        : memberTypeInfo;
                var typeSymbol = elementTypeInfo.TypeSymbol;

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