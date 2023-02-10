using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.Toolkit.HighPerformance.Helpers;

namespace fin.io.bundles {
  public class ParallelFileBundleGathererAccumulator
      : IFileBundleGathererAccumulator {
    private readonly List<IFileBundleGatherer> gatherers_ = new();

    public IFileBundleGathererAccumulator Add(IFileBundleGatherer gatherer) {
      this.gatherers_.Add(gatherer);
      return this;
    }

    public IFileBundleGathererAccumulator Add(
        Func<IEnumerable<IFileBundle>> handler)
      => Add(new FileBundleHandlerGatherer<IFileBundle>(handler));

    public IEnumerable<IFileBundle> GatherFileBundles(bool assert) {
      var results = new IEnumerable<IFileBundle>[this.gatherers_.Count];
      ParallelHelper.For(0,
                         this.gatherers_.Count,
                         new GathererRunner(this.gatherers_, results, assert));
      return results.SelectMany(result => result);
    }

    private readonly struct GathererRunner : IAction {
      private readonly IReadOnlyList<IFileBundleGatherer> gatherers_;
      private readonly IList<IEnumerable<IFileBundle>> results_;
      private readonly bool assert_;


      public GathererRunner(IReadOnlyList<IFileBundleGatherer> gatherers,
                            IList<IEnumerable<IFileBundle>> results,
                            bool assert) {
        this.gatherers_ = gatherers;
        this.results_ = results;
        this.assert_ = assert;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Invoke(int i) {
        try {
          this.results_[i] = this.gatherers_[i].GatherFileBundles(this.assert_);
        } catch (Exception e) {
          ;
        }
      }
    }
  }
}