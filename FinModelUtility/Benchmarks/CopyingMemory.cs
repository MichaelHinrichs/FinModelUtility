using BenchmarkDotNet.Attributes;


namespace benchmarks {
  public class CopyingMemory {
    private const int SIZE = 10;
    private readonly int n_ = 100000;
    private readonly byte[] src_ = new byte[CopyingMemory.SIZE];
    private readonly byte[] dst_ = new byte[CopyingMemory.SIZE];



    [Benchmark]
    public void UsingBufferBlockCopy() {
      for (var i = 0; i < n_; ++i) {
        Buffer.BlockCopy(this.src_, 0, this.dst_, 0, CopyingMemory.SIZE);
      }
    }

    [Benchmark]
    public void UsingArrayCopy() {
      for (var i = 0; i < n_; ++i) {
        Array.Copy(this.src_, this.dst_, SIZE);
      }
    }

    [Benchmark]
    public void UsingSpanCopy() {
      for (var i = 0; i < n_; ++i) {
        this.src_.AsSpan().CopyTo(this.dst_.AsSpan());
      }
    }

    [Benchmark]
    public void UsingForLoop() {
      for (var i = 0; i < n_; ++i) {
        for (var b = 0; b < CopyingMemory.SIZE; ++b) {
          this.dst_[b] = this.src_[b];
        }
      }
    }
  }
}