using System;


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
  public class SchemaAttribute : Attribute { }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ArrayLengthSourceAttribute : Attribute {
    /// <summary>
    ///   Parses an integer length with the given format immediately before the array.
    /// </summary>
    public ArrayLengthSourceAttribute(SchemaIntType lengthType) {
      this.Method = SequenceLengthSourceType.IMMEDIATE_VALUE;
      this.LengthType = lengthType;
    }

    /// <summary>
    ///   Uses another integer field for the length. This separate field will only be used when
    ///   reading/writing.
    /// </summary>
    public ArrayLengthSourceAttribute(string otherMemberName) {
      this.Method = SequenceLengthSourceType.OTHER_MEMBER;
      this.OtherMemberName = otherMemberName;
    }

    public SequenceLengthSourceType Method { get; }

    public SchemaIntType LengthType { get; }
    public string OtherMemberName { get; }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class NullTerminatedStringAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class StringLengthSourceAttribute : Attribute {
    /// <summary>
    ///   Parses a length with the given format immediately before the string.
    /// </summary>
    public StringLengthSourceAttribute(SchemaIntType lengthType) {
      this.Method = StringLengthSourceType.IMMEDIATE_VALUE;
      this.LengthType = lengthType;
    }

    /// <summary>
    ///   Uses another field for the length. This separate field will only be used when
    ///   reading/writing.
    /// </summary>
    public StringLengthSourceAttribute(string otherMemberName) {
      this.Method = StringLengthSourceType.OTHER_MEMBER;
      this.OtherMemberName = otherMemberName;
    }

    public StringLengthSourceAttribute(int constLength) {
      this.Method = StringLengthSourceType.CONST;
      this.ConstLength = constLength;
    }

    public StringLengthSourceType Method { get; }

    public SchemaIntType LengthType { get; }
    public string? OtherMemberName { get; }
    public int ConstLength { get; }
  }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class FormatAttribute : Attribute {
    public FormatAttribute(SchemaNumberType numberType) {
      this.NumberType = numberType;
    }

    public SchemaNumberType NumberType { get; }
  }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class IfBooleanAttribute : Attribute {
    public IfBooleanAttribute(SchemaIntType lengthType) {
      this.Method = IfBooleanSourceType.IMMEDIATE_VALUE;
      this.BooleanType = lengthType;
    }

    public IfBooleanAttribute(string otherMemberName) {
      this.Method = IfBooleanSourceType.OTHER_MEMBER;
      this.OtherMemberName = otherMemberName;
    }

    public IfBooleanSourceType Method { get; }

    public SchemaIntType BooleanType { get; }
    public string? OtherMemberName { get; }
  }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class EndianOrderedAttribute : Attribute { }
}