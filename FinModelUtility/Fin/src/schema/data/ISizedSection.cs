﻿using schema;
using schema.attributes.ignore;
using schema.attributes.size;


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

  [BinarySchema]
  public partial class PassThruUint32SizedSection<T> : ISizedSection<T>
      where T : IBiSerializable {
    [SizeOfMemberInBytes(nameof(Data))]
    public uint Size { get; private set; }

    public T Data { get; }

    public PassThruUint32SizedSection(T data) {
      this.Data = data;
    }
  }
}