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

  public class FileBundleGathererAccumulatorWithInput<TFileBundle, T>
      : IFileBundleGathererAccumulatorWithInput<TFileBundle, T>
      where TFileBundle : IFileBundle {
    private readonly List<IFileBundleGatherer<TFileBundle>> gatherers_ = new();
    private readonly T input_;

    public FileBundleGathererAccumulatorWithInput(T input) {
      this.input_ = input;
    }

    public IFileBundleGathererAccumulatorWithInput<TFileBundle, T> Add(
        IFileBundleGatherer<TFileBundle> gatherer) {
      this.gatherers_.Add(gatherer);
      return this;
    }

    public IFileBundleGathererAccumulatorWithInput<TFileBundle, T> Add(
        Func<IEnumerable<TFileBundle>> handler)
      => Add(new FileBundleHandlerGatherer<TFileBundle>(handler));

    public IFileBundleGathererAccumulatorWithInput<TFileBundle, T> Add(
        Func<T, IEnumerable<TFileBundle>> handler)
      => Add(new FileBundleHandlerGathererWithInput<TFileBundle, T>(
                 handler,
                 this.input_));

    public IEnumerable<TFileBundle> GatherFileBundles(bool assert)
      => this.gatherers_
             .SelectMany(gatherer => gatherer.GatherFileBundles(assert))
             .ToList();
  }
}