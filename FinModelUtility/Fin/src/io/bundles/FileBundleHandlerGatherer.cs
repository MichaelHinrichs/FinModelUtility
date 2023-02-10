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
}