using System;
using System.IO;

namespace zar.schema.cmb {
  public static class DataTypeUtil {
    // TODO: Is there a better way to read arbitrary data types?
    public static float Read(
        EndianBinaryReader r,
        DataType dataType)
      => dataType switch {
          DataType.Byte   => r.ReadSByte(),
          DataType.UByte  => r.ReadByte(),
          DataType.Short  => r.ReadInt16(),
          DataType.UShort => r.ReadUInt16(),
          DataType.Int    => r.ReadInt32(),
          DataType.UInt   => r.ReadUInt32(),
          DataType.Float  => r.ReadSingle(),
          _ => throw new ArgumentOutOfRangeException(
                   nameof(dataType),
                   dataType,
                   null)
      };

    public static float[] Read(
        EndianBinaryReader r,
        int count,
        DataType dataType) {
      var values = new float[count];

      for (var i = 0; i < count; ++i) {
        values[i] = DataTypeUtil.Read(r, dataType);
      }

      return values;
    }

    public static int GetSize(DataType dataType)
      => dataType switch {
          DataType.Byte   => 1,
          DataType.UByte  => 1,
          DataType.Short  => 2,
          DataType.UShort => 2,
          DataType.Int    => 4,
          DataType.UInt   => 4,
          DataType.Float  => 4,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(dataType),
                   dataType,
                   null)
      };
  }
}