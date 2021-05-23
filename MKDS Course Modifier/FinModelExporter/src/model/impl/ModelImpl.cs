namespace fin.model.impl {
  // TODO: Add logic for optimizing the model.
  public partial class ModelImpl : IModel {
    public IMaterialManager MaterialManager { get; }
    public IAnimationManager AnimationManager { get; }
  }
}