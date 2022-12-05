using System;


namespace schema.attributes {
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
      this.OtherMember =
          this.GetSourceRelativeToStructure<string>(this.otherMemberName_);
    }

    public IMemberReference<string>? OtherMember { get; private set; }
  }
}