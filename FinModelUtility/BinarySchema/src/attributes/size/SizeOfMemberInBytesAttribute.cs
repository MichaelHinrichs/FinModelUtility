using System;


namespace schema.attributes.size {
  /// <summary>
  ///   Attribute for specifying that an integer represents the size of some
  ///   structure.
  ///
  ///   <para>
  ///     Used at write-time to substitute that length in instead of the raw
  ///     value of this field.
  ///   </para>
  ///   <para>
  ///     If included within an IDeserializable, this will result in a
  ///     compile-time error since this is only used at write-time.
  ///   </para>
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class SizeOfMemberInBytesAttribute : BMemberAttribute {
    private string otherMemberName_;

    public SizeOfMemberInBytesAttribute(string otherMemberName) {
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