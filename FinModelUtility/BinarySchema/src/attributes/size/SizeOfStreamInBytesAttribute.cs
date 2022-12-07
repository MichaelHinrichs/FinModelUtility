using System;


namespace schema.attributes.size {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class SizeOfStreamInBytesAttribute : Attribute { }
}