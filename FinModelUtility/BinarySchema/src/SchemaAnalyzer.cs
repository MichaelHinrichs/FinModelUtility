using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace schema {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class SchemaAnalyzer : DiagnosticAnalyzer {
    public override void Initialize(AnalysisContext context) {
      context.RegisterSymbolAction(symbolAnalysisContext => {
        var symbol = symbolAnalysisContext.Symbol;
        var namedType = symbol as INamedTypeSymbol;

        var attributes = namedType.GetAttributes();
        //if (attributes.Contains())

        // TODO: Check each field, make sure types are annotated

        /*var diagnostic = Diagnostic.Create(Rule, statement.GetFirstToken().GetLocation());
        symbolAnalysisContext.ReportDiagnostic(diagnostic);*/
      }, SymbolKind.NamedType);
    }

    // Metadata of the analyzer
    public const string DIAGNOSTIC_ID_ = "SampleAnalyzer";

    // You could use LocalizedString but it's a little more complicated for this sample
    private static readonly string TITLE_ =
        "Specify StringComparison in String.Equals";

    private static readonly string
        MESSAGE_FORMAT_ = "StringComparison is missing";

    private static readonly string DESCRIPTION_ =
        "Ensure you compare strings the way it is expected";

    private const string CATEGORY_ = "Usage";

    private static readonly DiagnosticDescriptor RULE_ =
        new(DIAGNOSTIC_ID_,
            TITLE_,
            MESSAGE_FORMAT_,
            CATEGORY_,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DESCRIPTION_);

    // Register the list of rules this DiagnosticAnalizer supports
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
      => ImmutableArray.Create(RULE_);
  }
}