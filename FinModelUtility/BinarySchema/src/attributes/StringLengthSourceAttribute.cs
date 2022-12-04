using System;


namespace schema.attributes {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class StringLengthSourceAttribute : BMemberAttribute<string> {
    private string? otherMemberName_;

    /// <summary>
    ///   Parses a length with the given format immediately before the string.
    /// </summary>
    public StringLengthSourceAttribute(SchemaIntegerType lengthType) {
      this.Method = StringLengthSourceType.IMMEDIATE_VALUE;
      this.LengthType = lengthType;
    }

    /// <summary>
    ///   Uses another field for the length. This separate field will only be used when
    ///   reading/writing.
    /// </summary>
    public StringLengthSourceAttribute(string otherMemberName) {
      this.Method = StringLengthSourceType.OTHER_MEMBER;
      this.otherMemberName_ = otherMemberName;
    }

    public StringLengthSourceAttribute(int constLength) {
      this.Method = StringLengthSourceType.CONST;
      this.ConstLength = constLength;
    }

    protected override void InitFields() {
      if (this.otherMemberName_ != null) {
        this.OtherMember =
            this.GetSourceRelativeToStructure(this.otherMemberName_)
                .AssertIsInteger();
      }
    }

    public StringLengthSourceType Method { get; }

    public SchemaIntegerType LengthType { get; }
    public IMemberReference? OtherMember { get; private set; }
    public int ConstLength { get; }
  }
}