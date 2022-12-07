using Microsoft.CodeAnalysis;
using schema.attributes.child_of;
using schema.io;
using schema.parser;
using schema.util;
using System.Collections.Generic;
using System.Linq;


namespace schema {
  public interface ITypeChain {
    ITypeChainNode Root { get; }
    ITypeChainNode Target { get; }

    IReadOnlyList<ITypeChainNode> RootToTarget { get; }
  }

  public interface ITypeChainNode {
    INamedTypeSymbol StructureSymbol { get; }
    ISymbol MemberSymbol { get; }
    ITypeInfo MemberTypeInfo { get; }
  }


  internal static class TypeChainUtil {
    public static ITypeChain GetTypeChainForRelativeMember(
        IList<Diagnostic> diagnostics,
        ITypeSymbol structureSymbol,
        string otherMemberPath,
        string thisMemberName,
        bool assertOrder
    )
      => GetTypeChainForRelativeMemberImpl_(
          diagnostics,
          structureSymbol,
          otherMemberPath,
          thisMemberName,
          assertOrder,
          new UpDownStack<string>(),
          null,
          null
      );

    public static void AssertAllNodesInTypeChainUntilTargetUseBinarySchema(
        IList<Diagnostic> diagnostics,
        ITypeChain typeChain) {
      for (var i = 0; i < typeChain.RootToTarget.Count; ++i) {
        var typeChainNode = typeChain.RootToTarget[i];

        var binarySchemaAttribute =
            SymbolTypeUtil.GetAttribute<BinarySchemaAttribute>(
                diagnostics, typeChainNode.StructureSymbol);
        if (binarySchemaAttribute == null) {
          diagnostics.Add(Rules.CreateDiagnostic(
                              typeChainNode.MemberSymbol,
                              Rules.AllMembersInChainMustUseSchema));
        }
      }
    }

    private static void GetMemberInStructure_(
        ITypeSymbol structureSymbol,
        string memberName,
        out ISymbol memberSymbol,
        out ITypeInfo memberTypeInfo
    ) {
      memberSymbol = structureSymbol.GetMembers(memberName).Single();
      new TypeInfoParser().ParseMember(memberSymbol, out memberTypeInfo);
    }


    private static ITypeChain GetTypeChainForRelativeMemberImpl_(
        IList<Diagnostic> diagnostics,
        ITypeSymbol structureSymbol,
        string otherMemberPath,
        string thisMemberName,
        bool assertOrder,
        IUpDownStack<string> upDownStack,
        TypeChain typeChain,
        string prevMemberName
    ) {
      if (typeChain == null) {
        GetMemberInStructure_(
            structureSymbol,
            thisMemberName,
            out var rootSymbol,
            out var rootTypeInfo);

        typeChain = new TypeChain(new TypeChainNode {
            StructureSymbol = (structureSymbol as INamedTypeSymbol)!,
            MemberSymbol = rootSymbol,
            MemberTypeInfo = rootTypeInfo
        });

        prevMemberName = thisMemberName;
      }

      // Gets next child in chain.
      var periodIndex = otherMemberPath.IndexOf('.');
      var steppingIntoNewStructure = periodIndex != -1;
      var currentMemberName = steppingIntoNewStructure
                                  ? otherMemberPath.Substring(0, periodIndex)
                                  : otherMemberPath;

      GetMemberInStructure_(
          structureSymbol,
          currentMemberName,
          out var memberSymbol,
          out var memberTypeInfo);

      typeChain.AddLinkInChain(new TypeChainNode {
          StructureSymbol = (structureSymbol as INamedTypeSymbol)!,
          MemberSymbol = memberSymbol,
          MemberTypeInfo = memberTypeInfo
      });

      // Asserts that we're not referencing something that comes before the
      // current member.
      if (assertOrder && upDownStack.Count == 0) {
        var members = structureSymbol.GetMembers();
        var membersAndIndices =
            members.Select((member, index) => (member, index)).ToArray();
        var indexOfThisMember = membersAndIndices
                                .Single(memberAndIndex =>
                                            memberAndIndex.member.Name ==
                                            thisMemberName)
                                .index;
        var indexOfOtherMember = membersAndIndices
                                 .Single(memberAndIndex =>
                                             memberAndIndex.member.Name ==
                                             currentMemberName)
                                 .index;

        if (indexOfThisMember < indexOfOtherMember) {
          diagnostics.Add(Rules.CreateDiagnostic(
                              members[indexOfThisMember],
                              Rules.DependentMustComeAfterSource));
        }
      }

      if (currentMemberName == nameof(IChildOf<IBiSerializable>.Parent) &&
          new ChildOfParser(diagnostics).GetParentTypeSymbolOf(
              (structureSymbol as INamedTypeSymbol)!) != null) {
        upDownStack.PushUpFrom(prevMemberName);
      } else {
        upDownStack.PushDownTo(currentMemberName);
      }

      // Steps down into next chain or returns.
      if (steppingIntoNewStructure) {
        var subMemberPath = otherMemberPath.Substring(periodIndex + 1);
        return GetTypeChainForRelativeMemberImpl_(
            diagnostics,
            (memberTypeInfo.TypeSymbol as INamedTypeSymbol)!,
            subMemberPath,
            thisMemberName,
            assertOrder,
            upDownStack,
            typeChain,
            currentMemberName);
      }

      // Asserts that we didn't ultimately reach the same member as this.
      if (upDownStack.Count == 0 &&
          currentMemberName == thisMemberName) {
        Asserts.Fail(
            $"Expected to find '{currentMemberName}' relative to '{thisMemberName}' in '{structureSymbol.Name}', but they're the same!");
      }

      return typeChain;
    }

    private class TypeChain : ITypeChain {
      private readonly List<ITypeChainNode> rootToTarget_ = new();

      public TypeChain(ITypeChainNode root) => this.AddLinkInChain(root);

      public ITypeChainNode Root => this.RootToTarget.First();
      public ITypeChainNode Target => this.RootToTarget.Last();
      public IReadOnlyList<ITypeChainNode> RootToTarget => this.rootToTarget_;

      public void AddLinkInChain(ITypeChainNode node)
        => this.rootToTarget_.Add(node);
    }

    private class TypeChainNode : ITypeChainNode {
      public INamedTypeSymbol StructureSymbol { get; set; }
      public ISymbol MemberSymbol { get; set; }
      public ITypeInfo MemberTypeInfo { get; set; }
    }
  }
}