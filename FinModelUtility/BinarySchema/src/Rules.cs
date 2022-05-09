using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace schema {
  public static class Rules {
    private static int diagnosticId_ = 0;

    private static string GetNextDiagnosticId_() {
      var id = Rules.diagnosticId_++;
      return "SCH" + id.ToString("D3");
    }

    private static DiagnosticDescriptor CreateDiagnostic_(
        string title,
        string messageFormat)
      => new(Rules.GetNextDiagnosticId_(),
             title,
             messageFormat,
             "SchemaAnalyzer",
             DiagnosticSeverity.Error,
             true);


    public static readonly DiagnosticDescriptor SchemaTypeMustBePartial
        = Rules.CreateDiagnostic_(
            "Schema type must be partial",
            "Schema type '{0}' must be partial to accept automatically generated read/write code.");

    public static readonly DiagnosticDescriptor ContainerTypeMustBePartial
        = Rules.CreateDiagnostic_(
            "Container of schema type must be partial",
            "Type '{0}' contains a schema type, must be partial to accept automatically generated code.");

    public static readonly DiagnosticDescriptor MutableStringNeedsLengthSource
        = Rules.CreateDiagnostic_(
            "Schema string must have length source",
            "Mutable string '{0}' is missing a LengthSource attribute.");

    public static readonly DiagnosticDescriptor MutableArrayNeedsLengthSource
        = Rules.CreateDiagnostic_(
            "Mutable array needs length source",
            "Mutable array '{0}' is missing a LengthSource attribute.");

    public static readonly DiagnosticDescriptor FormatOnNonNumber
        = Rules.CreateDiagnostic_(
            "Format attribute on non-numerical member",
            "A Format attribute is applied to the non-numerical member '{0}', which is unsupported.");

    public static DiagnosticDescriptor EnumNeedsFormat { get; }
        = Rules.CreateDiagnostic_(
            "Enum needs format",
            "Enum member '{0}' needs either a valid Format attribute or for its enum type to specify an underlying representation.");

    public static DiagnosticDescriptor BooleanNeedsFormat { get; }
        = Rules.CreateDiagnostic_(
            "Boolean needs format",
            "Boolean member '{0}' needs a valid Format attribute.");

    public static readonly DiagnosticDescriptor ConstUninitialized
        = Rules.CreateDiagnostic_(
            "Const uninitialized",
            "Const member '{0}' must be initialized.");

    public static readonly DiagnosticDescriptor NotSupported
        = Rules.CreateDiagnostic_(
            "Not supported",
            "This feature is not yet supported.");

    public static readonly DiagnosticDescriptor ReadAlreadyDefined
        = Rules.CreateDiagnostic_(
            "Read already defined",
            "A Read method for '{0}' was already defined.");

    public static readonly DiagnosticDescriptor WriteAlreadyDefined
        = Rules.CreateDiagnostic_(
            "Write already defined",
            "A Write method for '{0}' was already defined.");

    public static DiagnosticDescriptor UnexpectedAttribute { get; }
        = Rules.CreateDiagnostic_(
            "Unexpected attribute",
            "Did not expect this attribute on this field.");

    public static readonly DiagnosticDescriptor UnsupportedArrayType
        = Rules.CreateDiagnostic_(
            "Unsupported array type",
            "Array type '{0}' is not currently supported.");

    public static readonly DiagnosticDescriptor Exception
        = Rules.CreateDiagnostic_(
            "Exception",
            "Ran into an exception while parsing.");


    public static Diagnostic CreateDiagnostic(
        ISymbol symbol,
        DiagnosticDescriptor descriptor)
      => Diagnostic.Create(
          descriptor,
          symbol.Locations.First(),
          symbol.Name);

    public static void ReportDiagnostic(
        SyntaxNodeAnalysisContext? context,
        Diagnostic diagnostic)
      => context?.ReportDiagnostic(diagnostic);

    public static void ReportDiagnostic(
        SyntaxNodeAnalysisContext? context,
        ISymbol symbol,
        DiagnosticDescriptor descriptor)
      => context?.ReportDiagnostic(
          Rules.CreateDiagnostic(symbol, descriptor));
  }
}