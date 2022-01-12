using System;

namespace schema {
  [AttributeUsage(AttributeTargets.Class)]
  public class SchemaAttribute : Attribute {}


  public enum IntType {
    BYTE,
    SBYTE,
    SHORT,
    USHORT,
    INT,
    UINT,
    LONG,
    ULONG
  }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class NullTerminatedSchemaStringAttribute : Attribute {
    /// <summary>
    ///   Parses a length with the given format immediately before the string/array.
    /// </summary>
    public NullTerminatedSchemaStringAttribute(int maxLength) {
      this.MaxLength = maxLength;
    }

    public int MaxLength { get; }
  }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class LengthSourceAttribute : Attribute {
    /// <summary>
    ///   Parses a length with the given format immediately before the string/array.
    /// </summary>
    public LengthSourceAttribute(IntType countType) {
      this.Method = MethodType.IMMEDIATELY_BEFORE;
      this.CountType = countType;
    }

    /// <summary>
    ///   Uses another field for the length. This separate field will only be used when
    ///   reading/writing.
    /// </summary>
    public LengthSourceAttribute(string otherFieldName) {
      this.Method = MethodType.OTHER_FIELD;
      this.OtherFieldName = otherFieldName;
    }

    public enum MethodType {
      IMMEDIATELY_BEFORE,
      OTHER_FIELD,
    }

    public MethodType Method { get; }

    public IntType CountType { get; }
    public string OtherFieldName { get; }
  }


  public enum SchemaNumberType {
    UNDEFINED,

    SBYTE,
    BYTE,
    INT16,
    UINT16,
    INT32,
    UINT32,
    INT64,
    UINT64,

    SINGLE,
    DOUBLE,

    SN16,
    UN16,
  }


  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class FormatAttribute : Attribute {
    public FormatAttribute(SchemaNumberType numberType) {
      this.NumberType = numberType;
    }

    public SchemaNumberType NumberType { get; }
  }
}