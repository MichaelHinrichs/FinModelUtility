using System;
using System.IO;
using System.Threading.Tasks;


namespace schema.attributes.memory {
  /// <summary>
  ///   Pointer that encodes the relative difference between some address and
  ///   the start of the containing stream.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class PointerToAttribute : BMemberAttribute {
    private readonly string otherMemberName_;

    public PointerToAttribute(string otherMemberName) {
      this.otherMemberName_ = otherMemberName;
    }

    protected override void InitFields() {
      this.TypeChainToOtherMember =
          this.GetTypeChainRelativeToStructure(
              this.otherMemberName_, false);
    }

    public ITypeChain TypeChainToOtherMember { get; private set; }
  }
}