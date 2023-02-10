using System.Collections.Generic;
using System.Linq;

namespace fin.io.bundles {
  public class FileBundleGathererAccumulator : IFileBundleGathererAccumulator {
    private readonly List<IFileBundleGatherer> gatherers_ = new();

    public IFileBundleGathererAccumulator Add(IFileBundleGatherer gatherer) {
      this.gatherers_.Add(gatherer);
      return this;
    }

    public IEnumerable<IFileBundle> GatherFileBundles(bool assert)
      => this.gatherers_
             .SelectMany(gatherer => gatherer.GatherFileBundles(assert))
             .ToList();
  }
}