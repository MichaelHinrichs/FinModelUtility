using System;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#pragma warning disable CS8604

namespace schema {
  internal static class SchemaTestUtil {
    public static ISchemaStructure Parse(string src) {
      var syntaxTree = CSharpSyntaxTree.ParseText(src);

      var references =
          ((string) AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
          .Split(Path.PathSeparator)
          .Select(path => MetadataReference.CreateFromFile(path));

      var compilation =
          CSharpCompilation.Create("test")
                           .AddReferences(references)
                           .AddSyntaxTrees(syntaxTree);
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var classIndex = src.IndexOf("class");
      var classNameIndex = src.IndexOf(' ', classIndex) + 1;
      var classNameLength = src.IndexOf(' ', classNameIndex) - classNameIndex;
      var typeName = src.Substring(classNameIndex, classNameLength);

      var typeNode = syntaxTree.GetRoot()
                               .DescendantTokens()
                               .Single(t => t.Text == typeName)
                               .Parent;

      var symbol = semanticModel.GetDeclaredSymbol(typeNode);
      var namedTypeSymbol = symbol as INamedTypeSymbol;

      return new SchemaStructureParser().ParseStructure(namedTypeSymbol);
    }
  }
}
