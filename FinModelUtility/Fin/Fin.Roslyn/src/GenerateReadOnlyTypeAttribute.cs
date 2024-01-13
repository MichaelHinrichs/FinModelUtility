using System;

namespace fin.roslyn {
  [AttributeUsage(validOn: AttributeTargets.Interface)]
  public class GenerateReadOnlyTypeAttribute : Attribute {
  }
}
