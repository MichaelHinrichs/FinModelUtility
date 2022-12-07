using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using schema.attributes.size;
using schema.text;
using schema.util;


namespace schema {
  [Generator(LanguageNames.CSharp)]
  internal class SchemaGenerator : ISourceGenerator {
    private readonly Type schemaAttributeType_ = typeof(BinarySchemaAttribute);
    private readonly SchemaStructureParser parser_ = new();

    private readonly SchemaReaderGenerator readerImpl_ = new();
    private readonly SchemaWriterGenerator writerImpl_ = new();

    private void Generate_(ISchemaStructure structure) {
      var readerCode = this.readerImpl_.Generate(structure);
      this.context_.Value.AddSource(
          SymbolTypeUtil.GetQualifiedName(structure.TypeSymbol) + "_reader.g",
          readerCode);

      var writerCode = this.writerImpl_.Generate(structure);
      this.context_.Value.AddSource(
          SymbolTypeUtil.GetQualifiedName(structure.TypeSymbol) + "_writer.g",
          writerCode);
    }

    public void Initialize(GeneratorInitializationContext context) {
#if DEBUG
      /*if (!Debugger.IsAttached) {
        Debugger.Launch();
      }*/
#endif

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
      try {
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

        this.EnqueueStructure(structure);
      } catch (Exception exception) {
        if (Debugger.IsAttached) {
          throw;
        }

        this.EnqueueError(symbol, exception);
      }
    }

    public void Execute(GeneratorExecutionContext context) {
      this.context_ = context;

      // Gathers up a map of all structures by named type symbol.
      var structureByNamedTypeSymbol =
          new Dictionary<INamedTypeSymbol, ISchemaStructure>();
      foreach (var structure in this.queue_) {
        structureByNamedTypeSymbol[structure.TypeSymbol] = structure;
      }

      // Hooks up size of dependencies.
      {
        var sizeOfMemberInBytesDependencyFixer =
            new SizeOfMemberInBytesDependencyFixer();
        foreach (var structure in this.queue_) {
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
      }

      // Generates code for each structure.
      foreach (var structure in this.queue_) {
        try {
          this.Generate_(structure);
        } catch (Exception e) {
          ;
        }
      }
      this.queue_.Clear();

      foreach (var (errorSymbol, exception) in this.errorSymbols_) {
        this.context_.Value.ReportDiagnostic(
            Rules.CreateExceptionDiagnostic(errorSymbol, exception));
      }
      this.errorSymbols_.Clear();
    }

    private GeneratorExecutionContext? context_;
    private readonly List<ISchemaStructure> queue_ = new();

    public void EnqueueStructure(ISchemaStructure structure) {
      // If this assertion fails, then it means that syntax nodes are added
      // after the execution started.
      Asserts.Null(this.context_, "Syntax node added after execution!");
      this.queue_.Add(structure);
    }

    private readonly List<(ISymbol, Exception)> errorSymbols_ = new();

    public void EnqueueError(ISymbol errorSymbol, Exception exception) {
      // If this assertion fails, then it means that syntax nodes are added
      // after the execution started.
      Asserts.Null(this.context_, "Syntax node added after execution!");
      this.errorSymbols_.Add((errorSymbol, exception));
    }
  }
}