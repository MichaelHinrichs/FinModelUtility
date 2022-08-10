using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

using schema.text;

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

      var attributeSyntax = syntaxTree.GetRoot()
                                      .DescendantTokens()
                                      .First(t => {
                                        if (t.Text == "Schema" &&
                                            t.Parent
                                             ?.Parent is AttributeSyntax) {
                                          return true;
                                        }
                                        return false;
                                      })
                                      .Parent?.Parent as AttributeSyntax;
      var attributeSpan = attributeSyntax!.FullSpan;

      var classIndex =
          src.IndexOf("class", attributeSpan.Start + attributeSpan.Length);
      var classNameIndex = src.IndexOf(' ', classIndex) + 1;
      var classNameLength = src.IndexOf(' ', classNameIndex) - classNameIndex;

      var typeName = src.Substring(classNameIndex, classNameLength);
      var angleBracketIndex = typeName.IndexOf('<');
      if (angleBracketIndex > -1) {
        typeName = typeName.Substring(0, angleBracketIndex);
      }

      var typeNode = syntaxTree.GetRoot()
                               .DescendantTokens()
                               .Single(t =>
                                           t.Text == typeName &&
                                           t.Parent is ClassDeclarationSyntax
                                               or StructDeclarationSyntax
                               )
                               .Parent;

      var symbol = semanticModel.GetDeclaredSymbol(typeNode);
      var namedTypeSymbol = symbol as INamedTypeSymbol;

      return new SchemaStructureParser().ParseStructure(namedTypeSymbol);
    }

    public static void AssertDiagnostics(
        IList<Diagnostic> actualDiagnostics,
        params DiagnosticDescriptor[] expectedDiagnostics) {
      var message = "";

      if (actualDiagnostics.Count != expectedDiagnostics.Length) {
        message +=
            $"Expected {expectedDiagnostics.Length} diagnostics but got {actualDiagnostics.Count}.\n";
      }

      var issues = 0;
      for (var i = 0; i < actualDiagnostics.Count; ++i) {
        var actualDiagnostic = actualDiagnostics[i];
        var expectedDiagnostic = expectedDiagnostics[i];

        if (!actualDiagnostic.Descriptor.Equals(expectedDiagnostic)) {
          message +=
              $"{++issues}) Expected '{expectedDiagnostic.MessageFormat.ToString()}' but was '{actualDiagnostic.GetMessage()}'.\n";
        }
      }

      if (message.Length != 0) {
        Assert.Fail(message);
      }
    }

    public static void AssertGenerated(string src,
                                       string expectedReader,
                                       string expectedWriter) {
      var structure = SchemaTestUtil.Parse(src);
      Assert.IsEmpty(structure.Diagnostics);

      var actualReader = new SchemaReaderGenerator().Generate(structure);
      var actualWriter = new SchemaWriterGenerator().Generate(structure);

      Assert.AreEqual(expectedReader, actualReader.ReplaceLineEndings());
      Assert.AreEqual(expectedWriter, actualWriter.ReplaceLineEndings());
    }
  }
}