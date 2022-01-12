using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using schema;
using schema.text;

namespace schema {
  [Generator(LanguageNames.CSharp)]
  internal class SchemaGenerator : ISourceGenerator {
    private readonly Type schemaAttributeType_ = typeof(SchemaAttribute);
    private readonly SchemaStructureParser parser_ = new();

    private readonly SchemaReaderGenerator readerImpl_ = new();

    private void Generate_(ISchemaStructure structure) {
      var generatedCode = this.readerImpl_.Generate(structure);
      this.context_.Value.AddSource(
          SymbolTypeUtil.GetQualifiedName(structure.TypeSymbol),
          generatedCode);
    }

    public void Initialize(GeneratorInitializationContext context) {
      /*if (!Debugger.IsAttached) {
        Debugger.Launch();
      }*/
      context.RegisterForSyntaxNotifications(() => new CustomReceiver(this));
    }

    private class CustomReceiver : ISyntaxContextReceiver {
      private readonly SchemaGenerator g_;

      public CustomReceiver(SchemaGenerator g) {
        this.g_ = g;
      }

      public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
        TypeDeclarationSyntax syntax;
        ISymbol symbol;
        if (context.Node is ClassDeclarationSyntax classDeclarationSyntax) {
          syntax = classDeclarationSyntax;
        } else if (context.Node is StructDeclarationSyntax
                   structDeclarationSyntax) {
          syntax = structDeclarationSyntax;
        } else {
          return;
        }

        symbol = context.SemanticModel.GetDeclaredSymbol(syntax);
        if (symbol is not INamedTypeSymbol namedTypeSymbol) {
          return;
        }

        this.g_.CheckType(context, syntax, namedTypeSymbol);
      }
    }

    public void CheckType(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol symbol) {
      if (!SymbolTypeUtil.HasAttribute(symbol, this.schemaAttributeType_)) {
        return;
      }

      if (!SymbolTypeUtil.IsPartial(syntax)) {
        return;
      }

      var structure = this.parser_.ParseStructure(symbol);
      if (structure.Diagnostics.Count > 0) {
        return;
      }

      this.Enqueue(structure);
    }

    public void Execute(GeneratorExecutionContext context) {
      this.context_ = context;
      foreach (var structure in this.queue_) {
        this.Generate_(structure);
      }
      this.queue_.Clear();
    }

    private GeneratorExecutionContext? context_;
    private List<ISchemaStructure> queue_ = new();

    public void Enqueue(ISchemaStructure structure) {
      if (this.context_ == null) {
        this.queue_.Add(structure);
      } else {
        this.Generate_(structure);
      }
    }
  }
}