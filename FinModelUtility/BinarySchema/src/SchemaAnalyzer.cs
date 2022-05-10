using System;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace schema {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class SchemaAnalyzer : DiagnosticAnalyzer {
    private readonly Type schemaAttributeType_ = typeof(SchemaAttribute);
    private readonly SchemaStructureParser parser_ = new();

    public override ImmutableArray<DiagnosticDescriptor>
        SupportedDiagnostics { get; } =
      ImmutableArray.Create(
          Rules.ConstUninitialized,
          Rules.ContainerTypeMustBePartial,
          Rules.EnumNeedsFormat,
          Rules.Exception,
          Rules.FormatOnNonNumber,
          Rules.MutableArrayNeedsLengthSource,
          Rules.MutableStringNeedsLengthSource,
          Rules.NotSupported,
          Rules.SchemaTypeMustBePartial,
          Rules.UnexpectedAttribute,
          Rules.UnsupportedArrayType
      );

    public override void Initialize(AnalysisContext context) {
      /*if (!Debugger.IsAttached) {
        Debugger.Launch();
      }*/
      context.RegisterSyntaxNodeAction(
          syntaxNodeContext => {
            var syntax = syntaxNodeContext.Node as ClassDeclarationSyntax;

            var symbol =
                syntaxNodeContext.SemanticModel.GetDeclaredSymbol(syntax!);
            if (symbol is not INamedTypeSymbol namedTypeSymbol) {
              return;
            }

            this.CheckType(syntaxNodeContext, syntax!, namedTypeSymbol);
          },
          SyntaxKind.ClassDeclaration);

      context.RegisterSyntaxNodeAction(
          syntaxNodeContext => {
            var syntax = syntaxNodeContext.Node as StructDeclarationSyntax;

            var symbol =
                syntaxNodeContext.SemanticModel.GetDeclaredSymbol(syntax!);
            if (symbol is not INamedTypeSymbol namedTypeSymbol) {
              return;
            }

            this.CheckType(syntaxNodeContext, syntax!, namedTypeSymbol);
          },
          SyntaxKind.StructDeclaration);
    }

    public void CheckType(
        SyntaxNodeAnalysisContext context,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol symbol) {
      try {
        if (!SymbolTypeUtil.HasAttribute(symbol, this.schemaAttributeType_)) {
          return;
        }

        if (!SymbolTypeUtil.IsPartial(syntax)) {
          Rules.ReportDiagnostic(
              context,
              symbol,
              Rules.SchemaTypeMustBePartial);
          return;
        }

        var structure = this.parser_.ParseStructure(symbol);
        var diagnostics = structure.Diagnostics;
        if (diagnostics.Count > 0) {
          foreach (var diagnostic in diagnostics) {
            Rules.ReportDiagnostic(context, diagnostic);
          }
        }
      } catch {
        if (Debugger.IsAttached) {
          throw;
        }
        Rules.ReportDiagnostic(context,
                               Rules.CreateDiagnostic(symbol, Rules.Exception));
      }
    }
  }
}