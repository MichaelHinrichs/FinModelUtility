using System;

namespace schema {
  [AttributeUsage(AttributeTargets.Class)]
  public class SchemaAttribute : Attribute {
    public string MagicText { get; set; }
  }


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


  public interface ISchemaStringAttribute {}

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class NullTerminatedSchemaStringAttribute : Attribute,
    ISchemaStringAttribute {}

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class SeparateLengthSchemaStringAttribute : Attribute,
    ISchemaStringAttribute {
    public SeparateLengthSchemaStringAttribute(IntType countType) {
      this.CountType = countType;
    }

    public IntType CountType { get; }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class SeparateLengthSchemaArrayAttribute : Attribute {
    public SeparateLengthSchemaArrayAttribute(IntType countType) {
      this.CountType = countType;
    }

    public IntType CountType { get; }
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ConstLengthSchemaArrayAttribute : Attribute {
    public ConstLengthSchemaArrayAttribute(int count) {
      this.Count = count;
    }

    public int Count { get; }
  }
}