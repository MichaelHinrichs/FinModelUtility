using System;
using System.IO;

using schema.binary;
using schema.binary.attributes;

namespace fin.schema.data {
  /// <summary>
  ///   Schema class that implements a uint32-sized section without needing to
  ///   worry about passing in an instance of the contained data. This should
  ///   be adequate for most cases, except when the data class needs to access
  ///   parent data.
  /// </summary>
  [BinarySchema]
  public partial class SwitchMagicStringUInt32SizedSection<T> : IMagicSection<T>
      where T : IBinaryConvertible {
    [Ignore]
    private readonly int magicLength_;

    [Ignore]
    private readonly Func<string, T> createTypeHandler_;

    private readonly PassThruMagicUInt32SizedSection<T> impl_ =
        new(default, default!);

    public SwitchMagicStringUInt32SizedSection(
        int magicLength,
        Func<string, T> createTypeHandler) {
      this.magicLength_ = magicLength;
      this.createTypeHandler_ = createTypeHandler;
    }

    [Ignore]
    public int TweakReadSize {
      get => this.impl_.TweakReadSize;
      set => this.impl_.TweakReadSize = value;
    }

    [Ignore]
    public string Magic => this.impl_.Magic;

    [Ignore]
    public T Data => this.impl_.Data;

    public void Read(IEndianBinaryReader er) {
      var baseOffset = er.Position;

      var magic = er.ReadString(this.magicLength_);
      this.impl_.Magic = magic;
      this.impl_.Data = this.createTypeHandler_(magic);

      er.Position = baseOffset;
      this.impl_.Read(er);
    }
  }
}