using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace schema {
  internal static class SymbolTypeUtil {
    public static Type GetTypeFromSymbol(ISymbol symbol) {
      // TODO: Factor in declaration types
      var fieldName = symbol.Name;
      var fullyQualifiedTypeName =
          $"{symbol.ContainingNamespace}.{symbol.Name}";
      return Type.GetType(fullyQualifiedTypeName);
    }

    public static bool Implements(INamedTypeSymbol symbol, Type type)
      => symbol.AllInterfaces.Any(i => SymbolTypeUtil.IsExactlyType(i, type));

    public static string MergeContainingNamespaces(ISymbol symbol) {
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


    public static bool IsExactlyType(ISymbol symbol, Type expectedType)
      => symbol.Name == expectedType.Name &&
         SymbolTypeUtil.MergeContainingNamespaces(symbol) ==
         expectedType.Namespace;

    internal static bool HasAttribute(ISymbol symbol, Type expectedType)
      => symbol.GetAttributes()
               .Any(attributeData
                        => SymbolTypeUtil.IsExactlyType(
                            attributeData.AttributeClass,
                            expectedType));

    internal static TAttribute? GetAttribute<TAttribute>(ISymbol symbol)
        where TAttribute : notnull {
      var attributeType = typeof(TAttribute);
      var attributeData =
          symbol.GetAttributes()
                .FirstOrDefault(attributeData => {
                  var attributeSymbol = attributeData.AttributeClass;

                  return attributeSymbol.Name == attributeType.Name &&
                         SymbolTypeUtil.MergeContainingNamespaces(
                             attributeSymbol) ==
                         attributeType.Namespace;
                });

      if (attributeData == null) {
        return default;
      }

      var parameters = attributeData.AttributeConstructor.Parameters;
      // TODO: Does this still work w/ optional arguments?
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

    public static INamedTypeSymbol[] GetDeclaringTypesDownward(
        INamedTypeSymbol type) {
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

      return mergedNamespace == null
                 ? typeSymbol.Name
                 : $"{mergedNamespace}.{typeSymbol.Name}";
    }
  }
}