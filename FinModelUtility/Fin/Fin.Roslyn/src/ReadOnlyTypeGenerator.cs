using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using schema.util.asserts;
using schema.util.types;

namespace fin.roslyn {
  [Generator(LanguageNames.CSharp)]
  internal class ReadOnlyTypeGenerator : ISourceGenerator {
    private readonly ReadOnlyTypeTextGenerator textGenerator_ = new();

    private void Generate_(INamedTypeSymbol symbol, ITypeV2 typeV2) {
      this.context_.Value.AddSource(
          $"{typeV2.FullyQualifiedName}_readonly.g",
          textGenerator_.GenerateTextFor(symbol, typeV2));
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
      private readonly ReadOnlyTypeGenerator g_;

      public CustomReceiver(ReadOnlyTypeGenerator g) {
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
      var typeV2 = TypeV2.FromSymbol(symbol);

      try {
        if (!typeV2.HasAttribute<GenerateReadOnlyTypeAttribute>()) {
          return;
        }

        var isPartial = syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        if (!isPartial) {
          return;
        }

        this.EnqueueContainer(symbol, typeV2);
      } catch (Exception exception) {
        if (Debugger.IsAttached) {
          throw;
        }

        this.EnqueueError(symbol, exception);
      }
    }

    public void Execute(GeneratorExecutionContext context) {
      this.context_ = context;

      // Generates code for each container.
      foreach (var (symbol, typeV2) in this.queue_) {
        try {
          this.Generate_(symbol, typeV2);
        } catch (Exception e) {
          ;
        }
      }

      this.queue_.Clear();

      this.errorSymbols_.Clear();
    }

    private GeneratorExecutionContext? context_;
    private readonly List<(INamedTypeSymbol, ITypeV2)> queue_ = new();

    public void EnqueueContainer(INamedTypeSymbol symbol, ITypeV2 typeV2) {
      // If this assertion fails, then it means that syntax nodes are added
      // after the execution started.
      Asserts.Null(this.context_, "Syntax node added after execution!");
      this.queue_.Add((symbol, typeV2));
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