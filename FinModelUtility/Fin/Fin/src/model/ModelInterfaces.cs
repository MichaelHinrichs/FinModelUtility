namespace fin.model {
  public interface IModel {
    ISkeleton Skeleton { get; }
    ISkin Skin { get; }
    IMaterialManager MaterialManager { get; }
    IAnimationManager AnimationManager { get; }
    ILighting Lighting { get; }
  }
}