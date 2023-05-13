using System;
using System.Collections.Generic;

namespace fin.io.bundles {
  public class FileBundleHandlerGatherer<TFileBundle>
      : IFileBundleGatherer<TFileBundle>
      where TFileBundle : IFileBundle {
    private readonly Func<IEnumerable<TFileBundle>> impl_;

    public FileBundleHandlerGatherer(Func<IEnumerable<TFileBundle>> impl) {
      this.impl_ = impl;
    }

    public IEnumerable<TFileBundle> GatherFileBundles(bool assert)
      => this.impl_();
  }

  public class FileBundleHandlerGathererWithInput<TFileBundle, T>
      : IFileBundleGatherer<TFileBundle>
      where TFileBundle : IFileBundle {
    private readonly Func<T, IEnumerable<TFileBundle>> impl_;
    private readonly T input_;

    public FileBundleHandlerGathererWithInput(
        Func<T, IEnumerable<TFileBundle>> impl,
        T input) {
      this.impl_ = impl;
      this.input_ = input;
    }

    public IEnumerable<TFileBundle> GatherFileBundles(bool assert)
      => this.impl_(this.input_);
  }
}