using System;
using schema.attributes;


namespace schema {
  /// <summary>
  ///   Attribute for automatically generating Read/Write methods on
  ///   classes/structs. These are generated at compile-time, so the field
  ///   order will be 1:1 to the original class/struct and there should be no
  ///   performance cost compared to manually defined logic.
  ///
  ///   For any types that have this attribute, DO NOT modify or move around
  ///   the fields unless you know what you're doing!
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public class BinarySchemaAttribute : Attribute { }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ArrayLengthSourceAttribute : BMemberAttribute {
    private string? otherMemberName_;

    /// <summary>
    ///   Parses an integer length with the given format immediately before the array.
    /// </summary>
    public ArrayLengthSourceAttribute(SchemaIntegerType lengthType) {
      this.Method = SequenceLengthSourceType.IMMEDIATE_VALUE;
      this.LengthType = lengthType;
    }

    /// <summary>
    ///   Uses another integer field for the length. This separate field will only be used when
    ///   reading/writing.
    /// </summary>
    public ArrayLengthSourceAttribute(string otherMemberName) {
      this.Method = SequenceLengthSourceType.OTHER_MEMBER;
      this.otherMemberName_ = otherMemberName;
    }

    /// <summary>
    ///   Uses a constant integer for the length.
    /// </summary>
    public ArrayLengthSourceAttribute(uint constLength) {
      this.Method = SequenceLengthSourceType.CONST_LENGTH;
      this.ConstLength = constLength;
    }

    protected override void InitFields() {
      if (this.otherMemberName_ != null) {
        this.OtherMember =
            this.GetSourceRelativeToStructure(this.otherMemberName_)
                .AssertIsInteger();
      }
    }

    public SequenceLengthSourceType Method { get; }

    public SchemaIntegerType LengthType { get; }
    public IMemberReference OtherMember { get; private set; }
    public uint ConstLength { get; private set; }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class NullTerminatedStringAttribute : BMemberAttribute<string> {
    protected override void InitFields() { }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class NumberFormatAttribute : Attribute {
    public NumberFormatAttribute(SchemaNumberType numberType) {
      this.NumberType = numberType;
    }

    public SchemaNumberType NumberType { get; }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class IntegerFormatAttribute : Attribute {
    public IntegerFormatAttribute(SchemaIntegerType integerType) {
      this.IntegerType = integerType;
    }

    public SchemaIntegerType IntegerType { get; }
  }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class IfBooleanAttribute : BMemberAttribute {
    private readonly string? otherMemberName_;

    public IfBooleanAttribute(SchemaIntegerType lengthType) {
      this.Method = IfBooleanSourceType.IMMEDIATE_VALUE;
      this.BooleanType = lengthType;
    }

    public IfBooleanAttribute(string otherMemberName) {
      this.Method = IfBooleanSourceType.OTHER_MEMBER;
      this.otherMemberName_ = otherMemberName;
    }

    protected override void InitFields() {
      if (this.otherMemberName_ != null) {
        this.OtherMember =
            this.GetSourceRelativeToStructure(this.otherMemberName_)
                .AssertIsBool();
      }
    }

    public IfBooleanSourceType Method { get; }

    public SchemaIntegerType BooleanType { get; }
    public IMemberReference? OtherMember { get; private set; }
  }
}