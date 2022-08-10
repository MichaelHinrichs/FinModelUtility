using System.IO;
using System.Threading.Tasks;

using fin.util.asserts;

using schema;
using schema.attributes.ignore;


namespace fin.schema.data {
  public interface IMagicSection<T> : IBiSerializable
      where T : IBiSerializable {
    string Magic { get; }
    T Data { get; }
  }

  /// <summary>
  ///   Schema class that implements a uint32-sized section without needing to
  ///   worry about passing in an instance of the contained data. This should
  ///   be adequate for most cases, except when the data class needs to access
  ///   parent data.
  /// </summary>
  [Schema]
  public partial class AutoMagicUInt32SizedSection<T> : IMagicSection<T>
      where T : IBiSerializable, new() {
    private readonly PassThruMagicUint32SizedSection<T> impl_;

    [Ignore]
    public string Magic => this.impl_.Magic;

    [Ignore]
    public T Data => this.impl_.Data;

    public AutoMagicUInt32SizedSection(string magic) {
      this.impl_ = new(magic, new T());
    }
  }

  /// <summary>
  ///   Type for 
  /// </summary>
  public class PassThruMagicUint32SizedSection<T> : IMagicSection<T>
      where T : IBiSerializable {
    public string Magic { get; }
    public T Data { get; }

    public PassThruMagicUint32SizedSection(
        string magic,
        T data) {
      this.Magic = magic;
      this.Data = data;
    }

    public void Read(EndianBinaryReader er) {
      er.AssertString(this.Magic);

      var expectedDataSize = er.ReadUInt32();

      var dataStart = er.Position;
      this.Data.Read(er);
      var actualDataSize = er.Position - dataStart;
      if (expectedDataSize != actualDataSize) {
        Asserts.Fail(
            $"Expected to read {expectedDataSize} bytes in section '{this.Magic}', but actually read {actualDataSize} bytes.");
      }
    }

    public void Write(EndianBinaryWriter ew) {
      ew.WriteString(this.Magic);

      var beforeLengthTask = new TaskCompletionSource<long>();
      ew.WriteUInt32Delayed(
          beforeLengthTask.Task.ContinueWith(
              length => (uint) length.Result));

      var actualLengthTask = ew.EnterBlockAndGetDelayedLength((_, _) => {
        this.Data.Write(ew);
      });
      actualLengthTask.ContinueWith(
          length =>
              beforeLengthTask.SetResult(length.Result));
    }
  }
}