using System.IO;
using System.Threading.Tasks;

using fin.util.asserts;

using schema;
using schema.attributes.ignore;


namespace fin.schema.data {
  public interface ISizedSection<T> : IBiSerializable
      where T : IBiSerializable {
    T Data { get; }
  }

  /// <summary>
  ///   Schema class that implements a uint32-sized section without needing to
  ///   worry about passing in an instance of the contained data. This should
  ///   be adequate for most cases, except when the data class needs to access
  ///   parent data.
  /// </summary>
  [BinarySchema]
  public partial class AutoUInt32SizedSection<T> : ISizedSection<T>
      where T : IBiSerializable, new() {
    private readonly PassThruUint32SizedSection<T> impl_;

    [Ignore]
    public T Data => this.impl_.Data;

    public AutoUInt32SizedSection() {
      this.impl_ = new(new T());
    }
  }

  public class PassThruUint32SizedSection<T> : ISizedSection<T>
      where T : IBiSerializable {
    public T Data { get; }

    public PassThruUint32SizedSection(T data) {
      this.Data = data;
    }

    public void Read(EndianBinaryReader er) {
      var expectedDataSize = er.ReadUInt32();

      var dataStart = er.Position;
      this.Data.Read(er);
      var actualDataSize = er.Position - dataStart;
      if (expectedDataSize != actualDataSize) {
        Asserts.Fail(
            $"Expected to read {expectedDataSize} bytes in section, but actually read {actualDataSize} bytes.");
      }
    }

    public void Write(EndianBinaryWriter ew) {
      var beforeLengthTask = new TaskCompletionSource<long>();
      ew.WriteUInt32Delayed(
          beforeLengthTask.Task.ContinueWith(
              length => (uint) length.Result));

      {
        var sew = ew.EnterBlock(out var actualLengthTask);
        this.Data.Write(sew);
        actualLengthTask.ContinueWith(
            length =>
                beforeLengthTask.SetResult(length.Result));
      }
    }
  }
}