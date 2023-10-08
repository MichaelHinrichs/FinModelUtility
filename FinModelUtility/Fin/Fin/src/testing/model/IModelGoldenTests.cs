using fin.io;
using fin.io.bundles;
using fin.model.io;
using fin.model.io.importers;

namespace fin.testing.model {
  public abstract class BGoldenTests<TFileBundle>
      where TFileBundle : IFileBundle {
    public abstract TFileBundle GetFileBundleFromDirectory(
        IFileHierarchyDirectory directory);
  }

  public abstract class BModelGoldenTests<TModelFileBundle, TModelImporter>
      : BGoldenTests<TModelFileBundle>
      where TModelFileBundle : IModelFileBundle
      where TModelImporter : IModelImporter<TModelFileBundle>, new() {
    public void AssertGolden(IFileHierarchyDirectory goldenDirectory)
      => ModelGoldenAssert.AssertGolden(goldenDirectory,
                                        new TModelImporter(),
                                        this.GetFileBundleFromDirectory);
  }
}