using System.Runtime.CompilerServices;

using BenchmarkDotNet.Attributes;

using FastMath;

namespace benchmarks {
  public class TrigMethodVsInline {
    private const int n = 100000;
    private const float value = 1;

    [Benchmark]
    public void CallDirectly() {
      for (var i = 0; i < n; i++) {
        var x = Math.Atan2(value, value);
      }
    }

    [Benchmark]
    public void CallViaMethod() {
      for (var i = 0; i < n; i++) {
        var x = AcosWrapper(value);
      }
    }

    private double AcosWrapper(double d) => Math.Atan2(d, d);

    [Benchmark]
    public void CallViaMethodGroup() {
      for (var i = 0; i < n; i++) {
        var x = this.methodGroup(value, value);
      }
    }

    public Func<double, double, double> methodGroup = Math.Atan2;

    [Benchmark]
    public void CallViaLambda() {
      for (var i = 0; i < n; i++) {
        var x = this.lambda(value);
      }
    }

    public Func<double, double> lambda = d => Math.Atan2(d, d);

    [Benchmark]
    public void CallViaInline() {
      for (var i = 0; i < n; i++) {
        var x = InlineWrapper(value);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double InlineWrapper(double d) => Math.Atan2(d, d);

    [Benchmark]
    public void CallViaOptimizedInline() {
      for (var i = 0; i < n; i++) {
        var x = OptimizedInlineWrapper(value);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining |
                MethodImplOptions.AggressiveOptimization)]
    private double OptimizedInlineWrapper(double d) => Math.Atan2(d, d);



    [Benchmark]
    public void CallViaMemoizedLowError() {
      for (var i = 0; i < n; i++) {
        var x = memoizedLowError.Calculate(value, value);
      }
    }

    private MemoizedAtan2 memoizedLowError =
        MemoizedAtan2.ConstructByMaxError(.01f);


    [Benchmark]
    public void CallViaMemoizedHighError() {
      for (var i = 0; i < n; i++) {
        var x = memoizedHighError.Calculate(value, value);
      }
    }

    private MemoizedAtan2 memoizedHighError =
        MemoizedAtan2.ConstructByMaxError(.1f);

  }
}