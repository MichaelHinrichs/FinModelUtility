using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using schema.attributes.size;
using schema.text;

#pragma warning disable CS8604


namespace schema {
  internal static class SchemaTestUtil {
    public static ISchemaStructure ParseFirst(string src)
      => ParseAll(src).First();

    public static IReadOnlyList<ISchemaStructure> ParseAll(string src) {
      var syntaxTree = CSharpSyntaxTree.ParseText(src);

      var references =
          ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
          .Split(Path.PathSeparator)
          .Select(path => MetadataReference.CreateFromFile(path));

      var compilation =
          CSharpCompilation.Create("test")
                           .AddReferences(references)
                           .AddSyntaxTrees(syntaxTree);
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var structures = syntaxTree
                       .GetRoot()
                       .DescendantTokens()
                       .Where(t => {
                         if (t.Text == "BinarySchema" &&
                             t.Parent
                              ?.Parent is AttributeSyntax) {
                           return true;
                         }
                         return false;
                       })
                       .Select(t => t.Parent?.Parent as AttributeSyntax)
                       .Select(attributeSyntax => {
                         var attributeSpan = attributeSyntax!.FullSpan;

                         var classIndex =
                             src.IndexOf("class",
                                         attributeSpan.Start +
                                         attributeSpan.Length);
                         var classNameIndex = src.IndexOf(' ', classIndex) + 1;
                         var classNameLength =
                             src.IndexOf(' ', classNameIndex) - classNameIndex;

                         var typeName =
                             src.Substring(classNameIndex, classNameLength);
                         var angleBracketIndex = typeName.IndexOf('<');
                         if (angleBracketIndex > -1) {
                           typeName = typeName.Substring(0, angleBracketIndex);
                         }

                         var typeNode = syntaxTree.GetRoot()
                                                  .DescendantTokens()
                                                  .Single(t =>
                                                      t.Text ==
                                                      typeName &&
                                                      t.Parent is
                                                          ClassDeclarationSyntax
                                                          or StructDeclarationSyntax
                                                  )
                                                  .Parent;

                         var symbol = semanticModel.GetDeclaredSymbol(typeNode);
                         var namedTypeSymbol = symbol as INamedTypeSymbol;

                         return new SchemaStructureParser().ParseStructure(
                             namedTypeSymbol);
                       })
                       .ToArray();

      var structureByNamedTypeSymbol =
          new Dictionary<INamedTypeSymbol, ISchemaStructure>();
      foreach (var structure in structures) {
        structureByNamedTypeSymbol[structure.TypeSymbol] = structure;
      }

      var sizeOfMemberInBytesDependencyFixer =
          new SizeOfMemberInBytesDependencyFixer();
      foreach (var structure in structures) {
        foreach (var member in structure.Members) {
          if (member.MemberType is IPrimitiveMemberType primitiveMemberType) {
            if (primitiveMemberType.TypeChainToSizeOf != null) {
              sizeOfMemberInBytesDependencyFixer.AddDependenciesForStructure(
                  structureByNamedTypeSymbol,
                  primitiveMemberType.TypeChainToSizeOf);
            }
          }
        }
      }

      return structures;
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
      var structure = SchemaTestUtil.ParseFirst(src);
      Assert.IsEmpty(structure.Diagnostics);

      var actualReader = new SchemaReaderGenerator().Generate(structure);
      var actualWriter = new SchemaWriterGenerator().Generate(structure);

      Assert.AreEqual(expectedReader, actualReader.ReplaceLineEndings());
      Assert.AreEqual(expectedWriter, actualWriter.ReplaceLineEndings());
    }

    public static void AssertGeneratedForAll(
        string src,
        params (string, string)[] expectedReadersAndWriters) {
      var structures = SchemaTestUtil.ParseAll(src).ToArray();
      Assert.AreEqual(expectedReadersAndWriters.Length, structures.Length);
      for (var i = 0; i < structures.Length; ++i) {
        var (expectedReader, expectedWriter) = expectedReadersAndWriters[i];
        var structure = structures[i];

        Assert.IsEmpty(structure.Diagnostics);

        var actualReader = new SchemaReaderGenerator().Generate(structure);
        var actualWriter = new SchemaWriterGenerator().Generate(structure);

        Assert.AreEqual(expectedReader, actualReader.ReplaceLineEndings());
        Assert.AreEqual(expectedWriter, actualWriter.ReplaceLineEndings());
      }
    }
  }
}