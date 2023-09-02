using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.Toolkit.HighPerformance.Helpers;

namespace fin.io.bundles {
  public class ParallelFileBundleGathererAccumulator :
      IFileBundleGathererAccumulator {
    private readonly List<IFileBundleGatherer> gatherers_ = new();

    public IFileBundleGathererAccumulator Add(IFileBundleGatherer gatherer) {
      this.gatherers_.Add(gatherer);
      return this;
    }

    public IFileBundleGathererAccumulator Add(
        Func<IEnumerable<IFileBundle>> handler)
      => this.Add(new FileBundleHandlerGatherer<IFileBundle>(handler));

    public IEnumerable<IFileBundle> GatherFileBundles() {
      var results = new IEnumerable<IFileBundle>[this.gatherers_.Count];
      ParallelHelper.For(0,
                         this.gatherers_.Count,
                         new GathererRunner(this.gatherers_, results));
      return results.SelectMany(result => result);
    }

    private readonly struct GathererRunner(
        IReadOnlyList<IFileBundleGatherer> gatherers,
        IList<IEnumerable<IFileBundle>> results) : IAction {

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Invoke(int i) {
        try {
          results[i] = gatherers[i].GatherFileBundles();
        } catch (Exception e) {
          results[i] = Enumerable.Empty<IFileBundle>();
          Console.Error.WriteLine(e.ToString());
        }
      }
    }
  }
}