using System;


namespace schema.attributes.ignore {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class IgnoreAttribute : Attribute { }
}