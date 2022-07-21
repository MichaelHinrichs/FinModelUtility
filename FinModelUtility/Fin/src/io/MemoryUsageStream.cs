using System;
using System.Collections.Generic;
using System.IO;

using MathNet.Numerics;


namespace fin.io {
  public class MemoryUsageStream : Stream {
    private readonly MemoryStream impl_;

    public bool[] TouchedBytes { get; }
    public IEnumerable<Range> GetUntouchedRanges() {
      var isRangeUntouched = false;
      var untouchedStartOffset = -1;

      for (var i = 0; i < this.Length; ++i) {
        var wasTouched = this.TouchedBytes[i];

        if (!wasTouched && !isRangeUntouched) {
          isRangeUntouched = true;
          untouchedStartOffset = i;
        }
        if (wasTouched && isRangeUntouched) {
          isRangeUntouched = false;
          yield return new Range(untouchedStartOffset, i - 1);
        }
      }

      if (isRangeUntouched) {
        yield return new Range(untouchedStartOffset, (int) (this.Length - 1));
      }
    }

    public MemoryUsageStream(byte[] data) {
      this.impl_ = new MemoryStream(data);
      this.TouchedBytes = new bool[data.Length];
    }


    public override void Flush() => this.impl_.Flush();

    public override int Read(byte[] buffer, int offset, int count) {
      for (var i = 0; i < count; ++i) {
        this.TouchedBytes[this.Position + i] = true;
      }
      return this.impl_.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
      => this.impl_.Seek(offset, origin);

    public override void SetLength(long value) {
      throw new System.NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count) {
      throw new System.NotImplementedException();
    }

    public override bool CanRead => this.impl_.CanRead;
    public override bool CanSeek => this.impl_.CanSeek;
    public override bool CanWrite => this.impl_.CanWrite;
    public override long Length => this.impl_.Length;

    public override long Position {
      get => this.impl_.Position;
      set => this.impl_.Position = value;
    }
  }
}