using fin.io;
using fin.io.bundles;
using fin.model;

namespace fin.testing.model {
  public abstract class BGoldenTests<TFileBundle>
      where TFileBundle : IFileBundle {
    public abstract TFileBundle GetFileBundleFromDirectory(
        IFileHierarchyDirectory directory);
  }

  public abstract class BModelGoldenTests<TModelFileBundle, TModelLoader>
      : BGoldenTests<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle
      where TModelLoader : IModelLoader<TModelFileBundle>, new() {
    public void AssertGolden(IFileHierarchyDirectory goldenDirectory)
      => ModelGoldenAssert.AssertGolden(goldenDirectory,
                                        new TModelLoader(),
                                        this.GetFileBundleFromDirectory);
  }
}