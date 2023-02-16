using System.IO;
using System.Text;

using Microsoft.CodeAnalysis;

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
        if (member is IMethodSymbol method) {
          var syntax = method.DeclaringSyntaxReferences[0];
          var methodText = syntax.GetSyntax().ToString();
          cbsb.WriteLine($"public {methodText}"
                         .Replace("\r\n", "\n")
                         .Replace("  ", ""));
        }
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
  }
}