using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace schema {
  internal static class SymbolTypeUtil {
    public static ISymbol GetSymbolFromType(SemanticModel semanticModel,
                                            Type type)
      => GetSymbolFromIdentifier(semanticModel, type.FullName);

    public static ISymbol GetSymbolFromIdentifier(
        SemanticModel semanticModel,
        string identifier) {
      var symbol = semanticModel.LookupSymbols(0, null, identifier);
      return symbol.First();
    }

    public static bool ImplementsGeneric(INamedTypeSymbol symbol, Type type)
      => symbol.AllInterfaces.Any(i => SymbolTypeUtil.MatchesGeneric(i, type));

    public static bool Implements(INamedTypeSymbol symbol, Type type)
      => symbol.AllInterfaces.Any(i => SymbolTypeUtil.IsExactlyType(i, type));

    public static string? MergeContainingNamespaces(ISymbol symbol) {
      var namespaceSymbol = symbol.ContainingNamespace;
      if (namespaceSymbol == null) {
        return null;
      }

      var combined = "";
      while (namespaceSymbol != null) {
        if (combined.Length == 0) {
          combined = namespaceSymbol.Name;
        } else if (namespaceSymbol.Name.Length > 0) {
          combined = $"{namespaceSymbol.Name}.{combined}";
        }
        namespaceSymbol = namespaceSymbol.ContainingNamespace;
      }
      return combined;
    }

    public static bool HasEmptyConstructor(INamedTypeSymbol symbol)
      => symbol.InstanceConstructors.Any(c => c.Parameters.Length == 0);

    public static bool IsPartial(TypeDeclarationSyntax syntax)
      => syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

    public static bool MatchesGeneric(INamedTypeSymbol symbol,
                                      Type expectedGenericType) {
      var sameName = symbol.Name ==
                     expectedGenericType.Name.Substring(
                         0, expectedGenericType.Name.IndexOf('`'));
      var sameNamespace =
          SymbolTypeUtil.MergeContainingNamespaces(symbol) ==
          expectedGenericType.Namespace;
      var sameTypeArguments = symbol.TypeArguments.Length ==
                              expectedGenericType.GetTypeInfo().GenericTypeParameters.Length;
      return sameName && sameNamespace && sameTypeArguments;
    }

    public static bool IsExactlyType(ISymbol symbol, Type expectedType)
      => symbol.Name == expectedType.Name &&
         SymbolTypeUtil.MergeContainingNamespaces(symbol) ==
         expectedType.Namespace;

    internal static bool HasAttribute(ISymbol symbol, Type expectedType)
      => symbol.GetAttributes()
               .Any(attributeData
                        => SymbolTypeUtil.IsExactlyType(
                            attributeData.AttributeClass!,
                            expectedType));

    internal static AttributeData?
        GetAttributeData<TAttribute>(ISymbol symbol) {
      var attributeType = typeof(TAttribute);
      return symbol.GetAttributes()
                   .FirstOrDefault(attributeData => {
                     var attributeSymbol = attributeData.AttributeClass;

                     return attributeSymbol.Name == attributeType.Name &&
                            SymbolTypeUtil.MergeContainingNamespaces(
                                attributeSymbol) ==
                            attributeType.Namespace;
                   });
    }

    internal static TAttribute? GetAttribute<TAttribute>(ISymbol symbol)
        where TAttribute : notnull {
      var attributeData = GetAttributeData<TAttribute>(symbol);
      if (attributeData == null) {
        return default;
      }

      var parameters = attributeData.AttributeConstructor.Parameters;
      // TODO: Does this still work w/ optional arguments?
      var attributeType = typeof(TAttribute);
      var constructor =
          attributeType.GetConstructors()
                       .First(c => {
                         var cParameters = c.GetParameters();
                         if (cParameters.Length != parameters.Length) {
                           return false;
                         }

                         for (var i = 0; i < parameters.Length; ++i) {
                           if (parameters[i].Name != cParameters[i].Name) {
                             return false;
                           }
                         }

                         return true;
                       });

      var arguments = attributeData.ConstructorArguments;
      return (TAttribute) constructor.Invoke(
          arguments.Select(a => a.Value).ToArray());
    }

    public static IEnumerable<ISymbol> GetInstanceMembers(
        INamedTypeSymbol structureSymbol) {
      foreach (var memberSymbol in structureSymbol.GetMembers()) {
        // Skips static/const fields
        if (memberSymbol.IsStatic) {
          continue;
        }

        // Skips backing field, these are used internally for properties
        if (memberSymbol.Name.Contains("k__BackingField")) {
          continue;
        }

        yield return memberSymbol;
      }
    }

    public static INamedTypeSymbol[] GetDeclaringTypesDownward(
        ITypeSymbol type) {
      var declaringTypes = new List<INamedTypeSymbol>();

      var declaringType = type.ContainingType;
      while (declaringType != null) {
        declaringTypes.Add(declaringType);
        declaringType = declaringType.ContainingType;
      }
      declaringTypes.Reverse();

      return declaringTypes.ToArray();
    }

    public static string GetSymbolQualifiers(INamedTypeSymbol typeSymbol)
      => (typeSymbol.IsStatic ? "static " : "") +
         SymbolTypeUtil.AccessibilityToModifier(
             typeSymbol.DeclaredAccessibility) +
         " " +
         (typeSymbol.IsAbstract ? "abstract " : "") +
         "partial " +
         (typeSymbol.TypeKind == TypeKind.Class ? "class" : "struct");

    public static string AccessibilityToModifier(
        Accessibility accessibility)
      => accessibility switch {
          Accessibility.Private   => "private",
          Accessibility.Protected => "protected",
          Accessibility.Internal  => "internal",
          Accessibility.Public    => "public",
          _ => throw new ArgumentOutOfRangeException(
                   nameof(accessibility),
                   accessibility,
                   null)
      };

    public static string GetQualifiedName(ITypeSymbol typeSymbol) {
      var mergedNamespace =
          SymbolTypeUtil.MergeContainingNamespaces(typeSymbol);
      var mergedNamespaceText = mergedNamespace == null
                                    ? ""
                                    : $"{mergedNamespace}.";

      var mergedContainersText = "";
      foreach (var container in SymbolTypeUtil.GetDeclaringTypesDownward(
                   typeSymbol)) {
        mergedContainersText += $"{container.Name}.";
      }

      return $"{mergedNamespaceText}{mergedContainersText}{typeSymbol.Name}";
    }

    public static string GetQualifiedNameFromCurrentSymbol(
        ITypeSymbol sourceSymbol,
        ITypeSymbol referencedSymbol) {
      var currentNamespace =
          SymbolTypeUtil.MergeContainingNamespaces(sourceSymbol);
      var referencedNamespace =
          SymbolTypeUtil.MergeContainingNamespaces(referencedSymbol);

      string mergedNamespaceText;
      if (currentNamespace == null && referencedNamespace == null) {
        mergedNamespaceText = "";
      } else if (currentNamespace == null) {
        mergedNamespaceText = $"{referencedNamespace!}.";
      } else if (referencedNamespace == null) {
        mergedNamespaceText = $"{currentNamespace}.";
      } else {
        var mergedNamespaceBuilder = new StringBuilder();
        var matching = true;
        for (var i = 0; i < referencedNamespace.Length; ++i) {
          var prevMatching = matching;

          var referencedC = referencedNamespace[i];
          if (i >= currentNamespace.Length) {
            matching = false;
          } else if (currentNamespace[i] != referencedC) {
            matching = false;
          }

          var newlyDifferent = prevMatching && !matching;
          if (!(newlyDifferent && referencedC == '.') && !matching) {
            mergedNamespaceBuilder.Append(referencedC);
          }
        }

        mergedNamespaceText =
            mergedNamespaceBuilder.Length > 0
                ? $"{mergedNamespaceBuilder}."
                : "";
      }

      var mergedContainersText = "";
      foreach (var container in SymbolTypeUtil.GetDeclaringTypesDownward(
                   referencedSymbol)) {
        mergedContainersText += $"{container.Name}.";
      }

      return
          $"{mergedNamespaceText}{mergedContainersText}{referencedSymbol.Name}";
    }

    public static ITypeSymbol GetTypeFromMember(
        ITypeSymbol structureSymbol,
        string memberName) {
      var periodIndex = memberName.IndexOf('.');
      if (periodIndex != -1) {
        var subStructureName = memberName.Substring(0, periodIndex);
        var subStructureTypeSymbol =
            GetTypeFromMember(structureSymbol, subStructureName);

        var subMemberName = memberName.Substring(periodIndex + 1);

        return GetTypeFromMember(
            subStructureTypeSymbol,
            subMemberName);
      }

      return structureSymbol
             .GetMembers(memberName)
             .Single() switch {
          IPropertySymbol propertySymbol => propertySymbol.Type,
          IFieldSymbol fieldSymbol       => fieldSymbol.Type,
          _                              => throw new NotSupportedException()
      };
    }
  }
}