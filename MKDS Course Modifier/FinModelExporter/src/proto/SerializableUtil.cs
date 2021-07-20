using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using fin.util.asserts;

namespace fin.proto {
  public static class SerializableUtil {
    private static readonly BytesPtrView VIEW = new();

    public static int GetSizeOf(Type type) => Marshal.SizeOf(type);

    public static T Deserialize<T>(byte[] bytes)
      => Deserialize<T>(bytes, 0, bytes.Length);

    public static T Deserialize<T>(byte[] bytes, long offset, long length) {
      //STREAM.SetImpl(bytes, offset, length);
      //return Deserialize<T>(STREAM);
      return Deserialize<T>(
          new MemoryStream(bytes, (int) offset, (int) length));
    }

    public static T Deserialize<T>(Stream stream)
      => StreamMarshal.Deserialize<T>(stream);

    public static byte[] Serialize(object obj) {
      var stream = new MemoryStream();
      Serialize(obj, stream);
      return stream.ToArray();
    }

    public static void Serialize(object obj, Stream stream) {
      /*var type = obj.GetType();

      if (type.IsValueType) {*/
      StreamMarshal.Serialize(obj, stream);
      /*  return;
      }

      var serializedFields =
          type.GetFields()
              .Where(field => field.GetCustomAttribute(
                                  typeof(NonSerializedAttribute)) ==
                              null);

      foreach (var field in serializedFields) {
        Serialize(field.GetValue(obj), stream);
      }*/
    }
  }

  public static class StreamMarshal {
    // TODO: Optimize this so it doesn't have to allocate for each object.
    public static T Deserialize<T>(Stream stream) {
      var size = Marshal.SizeOf(typeof(T));

      var bytes = new byte[size];
      stream.Read(bytes, 0, size);

      var ptr = Marshal.AllocHGlobal(size);
      Marshal.Copy(bytes, 0, ptr, size);

      var obj = Marshal.PtrToStructure<T>(ptr);
      Marshal.FreeHGlobal(ptr);

      return obj;
    }

    // TODO: Optimize this so it doesn't have to allocate for each object.
    public static void Serialize(object obj, Stream stream) {
      var size = Marshal.SizeOf(obj);

      var ptr = Marshal.AllocHGlobal(size);
      Marshal.StructureToPtr(obj, ptr, false);

      var bytes = new byte[size];
      Marshal.Copy(ptr, bytes, 0, size);
      Marshal.FreeHGlobal(ptr);

      stream.Write(bytes, 0, size);
    }
  }

  public class BytesPtrView {
    private byte[] impl_;
    private int position_;
    private int length_;

    // TODO: Make sure isn't written outside of.
    public void SetImpl(byte[] bytes) => this.SetImpl(bytes, 0, bytes.Length);

    public void SetImpl(byte[] bytes, int offset, int length) {
      this.impl_ = bytes;
      this.position_ = offset;
      this.length_ = length;
    }
  }

  public class BytesViewStream : Stream {
    private byte[] impl_;
    private long length_;

    public void SetImpl(byte[] bytes) => this.SetImpl(bytes, 0, bytes.Length);

    public void SetImpl(byte[] bytes, long offset, long length) {
      this.impl_ = bytes;
      this.Position = offset;
      this.length_ = length;
    }

    public override void Flush() {}

    public override int Read(byte[] buffer, int offset, int count) {
      this.Position += offset;

      var i = 0;
      for (; i < count && this.Position < this.Length; ++i) {
        buffer[i] = this.impl_[this.Position++];
      }

      return i;
    }

    public override long Seek(long offset, SeekOrigin origin) {
      switch (origin) {
        case SeekOrigin.Begin: {
          this.Position = offset;
          break;
        }
        case SeekOrigin.Current: {
          this.Position += offset;
          break;
        }
        case SeekOrigin.End: {
          throw new NotImplementedException();
          break;
        }
        default:
          throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
      }

      return this.Position;
    }

    public override void SetLength(long value)
      => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) {
      this.Position += offset;

      for (var i = 0; i < count; ++i) {
        this.impl_[this.Position++] = buffer[i];
      }
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => true;

    public override long Length => this.length_;
    public override long Position { get; set; }
  }
}