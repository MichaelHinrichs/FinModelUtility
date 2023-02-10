using System;
using System.Collections.Generic;
using System.Linq;

namespace fin.io.bundles {
  public class FileBundleGathererAccumulator : IFileBundleGathererAccumulator {
    private readonly List<IFileBundleGatherer> gatherers_ = new();

    public IFileBundleGathererAccumulator Add(IFileBundleGatherer gatherer) {
      this.gatherers_.Add(gatherer);
      return this;
    }

    public IFileBundleGathererAccumulator Add(
        Func<IEnumerable<IFileBundle>> handler)
      => Add(new FileBundleHandlerGatherer<IFileBundle>(handler));

    public IEnumerable<IFileBundle> GatherFileBundles(bool assert)
      => this.gatherers_
             .SelectMany(gatherer => gatherer.GatherFileBundles(assert))
             .ToList();
  }

  public class FileBundleGathererAccumulator<TFileBundle>
      : IFileBundleGathererAccumulator<TFileBundle>
      where TFileBundle : IFileBundle {
    private readonly List<IFileBundleGatherer<TFileBundle>> gatherers_ = new();

    public IFileBundleGathererAccumulator<TFileBundle> Add(
        IFileBundleGatherer<TFileBundle> gatherer) {
      this.gatherers_.Add(gatherer);
      return this;
    }

    public IFileBundleGathererAccumulator<TFileBundle> Add(
        Func<IEnumerable<TFileBundle>> handler)
      => Add(new FileBundleHandlerGatherer<TFileBundle>(handler));

    public IEnumerable<TFileBundle> GatherFileBundles(bool assert)
      => this.gatherers_
             .SelectMany(gatherer => gatherer.GatherFileBundles(assert))
             .ToList();

  }
}