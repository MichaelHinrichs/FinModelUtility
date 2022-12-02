using Microsoft.CodeAnalysis;


namespace schema {
  public static partial class Rules {
    public static readonly DiagnosticDescriptor DependentMustComeAfterSource
        = Rules.CreateDiagnosticDescriptor_(
            "Field must come after what it is dependent on",
            "Field '{0}' is dependent on another field, and therefore must come after it.");
  }
}