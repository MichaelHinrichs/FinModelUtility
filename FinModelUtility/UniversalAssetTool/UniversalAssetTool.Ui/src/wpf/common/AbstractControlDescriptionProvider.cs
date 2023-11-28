using System.ComponentModel;

using fin.util.asserts;

namespace uni.ui.wpf.common {
  /// <summary>
  ///   https://stackoverflow.com/a/17661386
  /// </summary>
  public class AbstractControlDescriptionProvider<TAbstract, TBase>
      : TypeDescriptionProvider {
    public AbstractControlDescriptionProvider()
        : base(TypeDescriptor.GetProvider(typeof(TAbstract))) { }

    public override Type GetReflectionType(Type objectType, object instance) {
      if (objectType == typeof(TAbstract))
        return typeof(TBase);

      return base.GetReflectionType(objectType, instance);
    }

    public override object CreateInstance(IServiceProvider provider,
                                          Type objectType,
                                          Type[] argTypes,
                                          object[] args) {
      if (objectType == typeof(TAbstract))
        objectType = typeof(TBase);

      return Asserts.Nonnull(
          base.CreateInstance(provider, objectType, argTypes, args));
    }
  }
}