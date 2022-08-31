using System;
using System.IO;


namespace schema.attributes.align {
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct |
                  AttributeTargets.Field | AttributeTargets.Property)]
  public class EndiannessAttribute : Attribute {
    public EndiannessAttribute(Endianness endianness) {
      this.Endianness = endianness;
    }

    public Endianness Endianness { get; }
  }
}