using Microsoft.CodeAnalysis;
using schema.parser;
using System.Collections.Generic;


namespace schema {
  public interface ITypeChain {
    ITypeChainNode Root { get; }
    ITypeChainNode Target { get; }

    IReadOnlyList<ITypeChainNode> RootToTarget { get; }
  }

  public interface ITypeChainNode {
    ISymbol MemberSymbol { get; }
    ITypeInfo TypeInfo { get; }
  }


  internal static class TypeChainUtil {
    public static ITypeChain GetTypeChainForRelativeMember(
        IList<Diagnostic> diagnostics,
        ITypeSymbol structureSymbol,
        string otherMemberName,
        string thisMemberName)
      => GetTypeChainForRelativeMemberImpl_(
          diagnostics,
          structureSymbol,
          thisMemberName,
          otherMemberName,
          new List<string>()
      );


    private static ITypeChain GetTypeChainForRelativeMemberImpl_(
        IList<Diagnostic> diagnostics,
        ITypeSymbol structureSymbol,
        string thisMemberName,
        string otherMemberName,
        List<string> levels) {
      return null!;
    }
  }
}