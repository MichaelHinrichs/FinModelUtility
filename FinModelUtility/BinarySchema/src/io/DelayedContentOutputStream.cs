using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using schema.util;


namespace schema.io {
  public interface IDelayedContentOutputStream {
    Task<long> GetDelayedPosition();
    Task<long> GetDelayedLength();

    Task<long> EnterBlockAndGetDelayedLength(
        Action<IDelayedContentOutputStream, Task<long>> handler);

    void Align(uint amount);
    void WriteByte(byte value);
    void Write(byte[] bytes, int offset, int count);
    void Write(byte[] bytes);
    void WriteDelayed(Task<byte[]> delayedBytes, Task<long> delayedBytesLength);
    void WriteDelayed(Task<byte[]> delayedBytes);

    Task CompleteAndCopyToDelayed(Stream stream);
  }

  public class DelayedContentOutputStream : IDelayedContentOutputStream {
    private readonly List<Task<IDataChunk>> dataChunks_ = new();
    private readonly List<Task<ISizeChunk>> sizeChunks_ = new();

    private IList<byte>? currentBytes_ = null;
    private readonly TaskCompletionSource<long> lengthTask_ = new();
    private bool isCompleted_ = false;

    public Task<long> GetDelayedPosition() {
      this.AssertNotCompleted_();

      this.PushCurrentBytes_();
      var task = new TaskCompletionSource<long>();
      this.sizeChunks_.Add(
          Task.FromResult<ISizeChunk>(new PositionSizeChunk(task)));
      return task.Task;
    }

    public Task<long> GetDelayedLength() {
      this.AssertNotCompleted_();

      return this.lengthTask_.Task;
    }


    public Task<long> EnterBlockAndGetDelayedLength(
        Action<IDelayedContentOutputStream, Task<long>> handler) {
      this.AssertNotCompleted_();

      var task = new TaskCompletionSource<long>();

      this.PushCurrentBytes_();
      this.sizeChunks_.Add(
          Task.FromResult<ISizeChunk>(new SizeChunkBlockStart(task)));

      handler(this, task.Task);

      this.PushCurrentBytes_();
      this.sizeChunks_.Add(
          Task.FromResult<ISizeChunk>(new SizeChunkBlockEnd(task)));

      return task.Task;
    }


    public void Align(uint amount) {
      this.AssertNotCompleted_();

      this.PushCurrentBytes_();
      this.dataChunks_.Add(
          Task.FromResult<IDataChunk>(new AlignDataChunk(amount)));
      this.sizeChunks_.Add(
          Task.FromResult<ISizeChunk>(new AlignSizeChunk(amount)));
    }

    public void WriteByte(byte value) {
      this.AssertNotCompleted_();

      this.CreateCurrentBytesIfNull_();
      this.currentBytes_!.Add(value);
    }

    public void Write(byte[] bytes) => this.Write(bytes, 0, bytes.Length);

    public void Write(byte[] bytes, int offset, int count) {
      this.AssertNotCompleted_();

      this.CreateCurrentBytesIfNull_();
      for (var i = 0; i < count; ++i) {
        this.currentBytes_!.Add(bytes[offset + i]);
      }
    }


    public void WriteDelayed(Task<byte[]> delayedBytes)
      => WriteDelayed(
          delayedBytes,
          delayedBytes.ContinueWith(bytesTask => bytesTask.Result.LongLength));

    public void WriteDelayed(
        Task<byte[]> delayedBytes,
        Task<long> delayedBytesLength) {
      this.AssertNotCompleted_();

      this.PushCurrentBytes_();
      this.dataChunks_.Add(
          Task.WhenAll(delayedBytes, delayedBytesLength)
              .ContinueWith(
                  _ => {
                    var bytes = delayedBytes.Result;
                    var length = delayedBytesLength.Result;
                    return (IDataChunk) new BytesDataChunk(
                        bytes, 0, (int) length);
                  }));
      this.sizeChunks_.Add(
          delayedBytesLength.ContinueWith(
              lengthTask =>
                  (ISizeChunk) new BytesSizeChunk(lengthTask.Result)));
    }


    public async Task CompleteAndCopyToDelayed(Stream stream) {
      this.AssertNotCompleted_();
      this.isCompleted_ = true;

      this.PushCurrentBytes_();

      // Updates position and length Tasks first.
      var blockStack = new Stack<(SizeChunkBlockStart, long)>();
      var position = 0L;
      var sizeChunks = await Task.WhenAll(this.sizeChunks_);
      foreach (var sizeChunk in sizeChunks) {
        switch (sizeChunk) {
          case PositionSizeChunk positionSizeChunk: {
            positionSizeChunk.Task.SetResult(position);
            break;
          }
          case AlignSizeChunk alignSizeChunk: {
            var pos = position;
            var amt = alignSizeChunk.Amount;
            var align = this.GetAlign_(pos, amt);
            position += align;
            break;
          }
          case BytesSizeChunk bytesSizeChunk: {
            position += bytesSizeChunk.Length;
            break;
          }
          case SizeChunkBlockStart blockStart: {
            blockStack.Push((blockStart, position));
            break;
          }
          case SizeChunkBlockEnd blockEnd: {
            var (blockStart, startPosition) = blockStack.Pop();
            Asserts.Same(blockStart.Task, blockEnd.Task);
            blockStart.Task.SetResult(position - startPosition);
            break;
          }
        }
      }
      this.lengthTask_.SetResult(position);

      // Writes data.
      var dataChunks = await Task.WhenAll(this.dataChunks_);
      foreach (var dataChunk in dataChunks) {
        switch (dataChunk) {
          case AlignDataChunk alignDataChunk: {
            var pos = position;
            var amt = alignDataChunk.Amount;
            var align = this.GetAlign_(pos, amt);
            for (var i = 0; i < align; ++i) {
              stream.WriteByte(0);
            }
            break;
          }
          case BytesDataChunk bytesDataChunk: {
            var bytes = bytesDataChunk.Bytes;
            await stream.WriteAsync(bytes, bytesDataChunk.Offset, bytesDataChunk.Count);
            break;
          }
        }
      }
    }

    private void AssertNotCompleted_() => Asserts.False(this.isCompleted_);

    private void CreateCurrentBytesIfNull_() {
      if (this.currentBytes_ != null) {
        return;
      }

      this.currentBytes_ = new List<byte>();
    }

    private void PushCurrentBytes_() {
      if (this.currentBytes_ == null) {
        return;
      }

      this.dataChunks_.Add(
          Task.FromResult<IDataChunk>(
              new BytesDataChunk(this.currentBytes_.ToArray())));
      this.sizeChunks_.Add(
          Task.FromResult<ISizeChunk>(
              new BytesSizeChunk(this.currentBytes_.Count)));
      this.currentBytes_ = null;
    }


    private long GetAlign_(long pos, long amt)
      => ((~(amt - 1) & (pos + amt - 1)) - pos);


    private interface IDataChunk { }

    private class AlignDataChunk : IDataChunk {
      public AlignDataChunk(uint amount) {
        this.Amount = amount;
      }

      public uint Amount { get; }
    }

    private class BytesDataChunk : IDataChunk {
      public BytesDataChunk(byte[] bytes) : this(bytes, 0, bytes.Length) { }

      public BytesDataChunk(
          byte[] bytes,
          int offset,
          int count) {
        this.Bytes = bytes;
        this.Offset = offset;
        this.Count = count;
      }

      public byte[] Bytes { get; }
      public int Offset { get; }
      public int Count { get; }
    }


    private interface ISizeChunk { }

    private class SizeChunkBlockStart : ISizeChunk {
      public SizeChunkBlockStart(TaskCompletionSource<long> task) {
        this.Task = task;
      }

      public TaskCompletionSource<long> Task { get; }
    }

    private class SizeChunkBlockEnd : ISizeChunk {
      public SizeChunkBlockEnd(TaskCompletionSource<long> task) {
        this.Task = task;
      }

      public TaskCompletionSource<long> Task { get; }
    }

    private class PositionSizeChunk : ISizeChunk {
      public PositionSizeChunk(TaskCompletionSource<long> task) {
        this.Task = task;
      }

      public TaskCompletionSource<long> Task { get; }
    }

    private class BytesSizeChunk : ISizeChunk {
      public BytesSizeChunk(long length) {
        this.Length = length;
      }

      public long Length { get; }
    }

    private class AlignSizeChunk : ISizeChunk {
      public AlignSizeChunk(uint amount) {
        this.Amount = amount;
      }

      public uint Amount { get; }
    }
  }
}