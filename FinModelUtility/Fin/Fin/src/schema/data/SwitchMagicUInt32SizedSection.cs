using System;
using System.IO;

using schema.binary;

namespace fin.schema.data {
  /// <summary>
  ///   Schema class that implements a uint32-sized section without needing to
  ///   worry about passing in an instance of the contained data. This should
  ///   be adequate for most cases, except when the data class needs to access
  ///   parent data.
  /// </summary>
  public class SwitchMagicUInt32SizedSection<TMagic, TData>
      : IMagicSection<TMagic, TData>
      where TData : IBinaryConvertible {
    private readonly int tweakSize_;

    private readonly Func<IEndianBinaryReader, TMagic> readMagicHandler_;
    private readonly Action<ISubEndianBinaryWriter, TMagic> writeMagicHandler_;
    private readonly Func<TMagic, TData> createTypeHandler_;

    private readonly PassThruUInt32SizedSection<TData> impl_ = new(default!);

    public SwitchMagicUInt32SizedSection(
        Func<IEndianBinaryReader, TMagic> readMagicHandler,
        Action<ISubEndianBinaryWriter, TMagic> writeMagicHandler,
        Func<TMagic, TData> createTypeHandler) : this(
        0,
        readMagicHandler,
        writeMagicHandler,
        createTypeHandler) { }

    public SwitchMagicUInt32SizedSection(
        int tweakSize,
        Func<IEndianBinaryReader, TMagic> readMagicHandler,
        Action<ISubEndianBinaryWriter, TMagic> writeMagicHandler,
        Func<TMagic, TData> createTypeHandler) {
      this.tweakSize_ = tweakSize;
      this.readMagicHandler_ = readMagicHandler;
      this.writeMagicHandler_ = writeMagicHandler;
      this.createTypeHandler_ = createTypeHandler;
    }

    public int TweakReadSize {
      get => this.impl_.TweakReadSize;
      set => this.impl_.TweakReadSize = value;
    }

    public bool UseLocalSpace {
      get => this.impl_.UseLocalSpace;
      set => this.impl_.UseLocalSpace = value;
    }

    public TMagic Magic { get; private set; }

    public TData Data => this.impl_.Data;

    public void Read(IEndianBinaryReader er) {
      this.Magic = this.readMagicHandler_(er);
      this.impl_.Data = this.createTypeHandler_(this.Magic);
      this.impl_.Read(er);
    }

    public void Write(ISubEndianBinaryWriter ew) {
      this.writeMagicHandler_(ew, this.Magic);
      this.impl_.Write(ew);
    }

    public override string ToString() => $"[{this.Magic}]: {this.Data}";
  }
}