using System.IO;
using System.Text;

using Microsoft.CodeAnalysis;

using schema.binary.parser;
using schema.util.symbols;
using schema.util.text;
using schema.util.types;

namespace fin.roslyn {
  [Generator(LanguageNames.CSharp)]
  internal class ReadOnlyTypeTextGenerator {
    public string GenerateTextFor(INamedTypeSymbol symbol, ITypeV2 typeV2) {
      var sb = new StringBuilder();
      using var cbsb = new CurlyBracketTextWriter(new StringWriter(sb));

      var nSpace = typeV2.FullyQualifiedNamespace;
      if (nSpace != null) {
        cbsb.EnterBlock($"namespace {nSpace}");
      }

      var declaringTypes = symbol.GetDeclaringTypesDownward();
      foreach (var declaringType in declaringTypes) {
        cbsb.EnterBlock(declaringType.GetQualifiersAndNameFor());
      }

      cbsb.EnterBlock(symbol.GetQualifiersAndNameFor());

      {
        var members = new TypeInfoParser().ParseMembers(symbol);

        foreach (var (parseStatus, memberSymbol, _, memberTypeInfo) in members) {
          if (parseStatus != TypeInfoParser.ParseStatus.SUCCESS) {
            continue;
          }

          var typeName = SymbolTypeUtil.GetQualifiedNameFromCurrentSymbol(
              typeV2,
              memberTypeInfo!.TypeV2);

          cbsb.WriteLine($"{typeName} {memberSymbol.Name} {{ get; }}");
        }
      }

      cbsb.ExitBlock();

      foreach (var _ in declaringTypes) {
        cbsb.ExitBlock();
      }

      if (nSpace != null) {
        cbsb.ExitBlock();
      }

      return cbsb.ToString();
    }
  }
}