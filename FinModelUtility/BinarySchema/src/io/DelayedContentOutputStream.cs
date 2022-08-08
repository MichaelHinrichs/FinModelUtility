using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace schema.io {
  public interface IDelayedContentOutputStream {
    void Write(byte value);
    void Write(byte[] bytes);
    void WriteDelayed(Task<byte[]> delayedBytes);

    Task CopyTo(Stream stream);
  }

  public class DelayedContentOutputStream : IDelayedContentOutputStream {
    private readonly List<Task<byte[]>> chunks_ = new();
    private IList<byte>? currentChunk_ = null;

    public void Write(byte value) {
      this.CreateCurrentChunkIfNull_();
      this.currentChunk_!.Add(value);
    }

    public void Write(byte[] bytes) {
      this.CreateCurrentChunkIfNull_();
      foreach (var value in bytes) {
        this.currentChunk_!.Add(value);
      }
    }

    public void WriteDelayed(Task<byte[]> delayedStream) {
      this.PushCurrentChunk_();
      this.chunks_.Add(delayedStream);
    }

    public async Task CopyTo(Stream stream) {
      this.PushCurrentChunk_();
      var chunks = await Task.WhenAll(this.chunks_);
      foreach (var chunk in chunks) {
        await stream.WriteAsync(chunk, 0, chunk.Length);
      }
    }

    private void CreateCurrentChunkIfNull_() {
      if (this.currentChunk_ != null) {
        return;
      }

      this.currentChunk_ = new List<byte>();
    }

    private void PushCurrentChunk_() {
      if (this.currentChunk_ == null) {
        return;
      }

      this.chunks_.Add(Task.FromResult(this.currentChunk_.ToArray()));
      this.currentChunk_ = null;
    }
  }
}