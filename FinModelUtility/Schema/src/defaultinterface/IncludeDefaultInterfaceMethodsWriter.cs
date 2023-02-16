using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using schema.binary;
using schema.binary.text;

namespace schema.defaultinterface {
  public class IncludeDefaultInterfaceMethodsWriter {
    public string Generate(DefaultInterfaceMethodsData data) {
      var typeSymbol = data.StructureSymbol;

      var typeNamespace = SymbolTypeUtil.MergeContainingNamespaces(typeSymbol);

      var declaringTypes =
          SymbolTypeUtil.GetDeclaringTypesDownward(typeSymbol);

      var sb = new StringBuilder();
      var cbsb = new CurlyBracketTextWriter(new StringWriter(sb));

      // Using statements
      if (data.AllUsingDirectives.Count > 0) {
        foreach (var usingDirective in data.AllUsingDirectives) {
          cbsb.WriteLine($"using {usingDirective.Name};");
        }

        cbsb.WriteLine("");
      }

      // TODO: Handle fancier cases here
      if (typeNamespace != null) {
        cbsb.EnterBlock($"namespace {typeNamespace}");
      }

      foreach (var declaringType in declaringTypes) {
        cbsb.EnterBlock(SymbolTypeUtil.GetQualifiersAndNameFor(declaringType));
      }

      cbsb.EnterBlock(SymbolTypeUtil.GetQualifiersAndNameFor(typeSymbol));

      foreach (var member in data.AllMembersToInclude) {
        cbsb.Write(GetNonGenericText_(member)
                   .Replace("\r\n", "\n")
                   .Replace("  ", ""));
      }

      // type
      cbsb.ExitBlock();

      // parent types
      foreach (var _ in declaringTypes) {
        cbsb.ExitBlock();
      }

      // namespace
      if (typeNamespace != null) {
        cbsb.ExitBlock();
      }

      return sb.ToString();
    }

    private string GetNonGenericText_(ISymbol symbol) {
      var sb = new StringBuilder();

      var containingType = symbol.ContainingType;
      var typeParameters = containingType.TypeParameters;
      var typeArguments = containingType.TypeArguments;
      var typeMap = typeParameters
                    .Zip(typeArguments,
                         (p, a) => (p.ToDisplayString(), a.ToDisplayString()))
                    .ToDictionary(t => t.Item1, t => t.Item2);

      var hasPrintedPublic = false;
      var squareBracketIndent = 0;

      var syntax = symbol.DeclaringSyntaxReferences[0].GetSyntax();
      var tokens = syntax.DescendantTokens().ToArray();
      for (var i = 0; i < tokens.Length; ++i) {
        var token = tokens[i];

        if (!hasPrintedPublic) {
          if (token.IsKind(SyntaxKind.OpenBracketToken)) {
            ++squareBracketIndent;
          } else if (token.IsKind(SyntaxKind.CloseBracketToken)) {
            --squareBracketIndent;
          } else if (squareBracketIndent == 0) {
            hasPrintedPublic = true;

            sb.Append("public ");
          }
        }

        var justTokenText = token.ToString();
        var tokenAndSpaces = token.ToFullString();

        var tokenToWrite = typeMap.TryGetValue(justTokenText, out var match)
            ? tokenAndSpaces.Replace(justTokenText, match)
            : tokenAndSpaces;

        sb.Append(tokenToWrite);
      }

      return sb.ToString();
    }
  }
}