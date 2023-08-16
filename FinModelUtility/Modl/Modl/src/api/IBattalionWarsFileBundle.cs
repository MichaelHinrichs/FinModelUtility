using fin.io.bundles;
using fin.model;

namespace modl.api {
  public interface IBattalionWarsFileBundle : IFileBundle { }

  public interface IBattalionWarsModelFileBundle : IBattalionWarsFileBundle,
      IModelFileBundle { }
}