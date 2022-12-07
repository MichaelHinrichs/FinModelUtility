using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;


namespace schema.attributes.size {
  internal class SizeOfMemberInBytesDependencyFixer {
    public void AddDependenciesForStructure(
        IDictionary<INamedTypeSymbol, ISchemaStructure>
            structureByNamedTypeSymbol,
        ITypeChain typeChain) {
      foreach (var typeChainNode in typeChain.RootToTarget.Skip(1)) {
        if (structureByNamedTypeSymbol.TryGetValue(
                typeChainNode.StructureSymbol, out var structure)) {
          var member = structure.Members.Single(
                               member =>
                                   member.Name ==
                                   typeChainNode.MemberSymbol.Name)
                           as SchemaStructureParser.SchemaMember;
          member.TrackStartAndEnd = true;
        }
      }
    }
  }
}