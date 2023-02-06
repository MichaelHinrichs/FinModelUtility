using System.Collections.Generic;
using System.Linq;

namespace System.IO {
  public sealed partial class FinTextReader {
    public byte[] ReadBytes(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertByte_);

    public byte[] ReadHexBytes(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertHexByte_);


    public sbyte[] ReadSBytes(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertSByte_);

    public sbyte[] ReadHexSBytes(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertHexSByte_);


    public short[] ReadInt16s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertInt16_);

    public short[] ReadHexInt16s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertHexInt16_);


    public ushort[] ReadUInt16s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertUInt16_);

    public ushort[] ReadHexUInt16s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertHexUInt16_);


    public int[] ReadInt32s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertInt32_);


    public int[] ReadHexInt32s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertHexInt32_);

    public uint[] ReadUInt32s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertUInt32_);

    public uint[] ReadHexUInt32s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertHexUInt32_);

    public long[] ReadInt64s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertInt64_);

    public long[] ReadHexInt64s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertHexInt64_);


    public ulong[] ReadUInt64s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertUInt64_);


    public ulong[] ReadHexUInt64s(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertHexUInt64_);


    public float[] ReadSingles(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertSingle_);

    public double[] ReadDoubles(string[] separators, string[] terminators)
      => this.ConvertSplitUpToAndPastTerminators_(separators,
                                           terminators,
                                           ConvertDouble_);


    private IEnumerable<string> ReadSplitUpToAndPastTerminators_(
        string[] separators,
        string[] terminators)
      => this.ReadUpToAndPastTerminator(terminators)
             .Split(separators, StringSplitOptions.None);

    private T[] ConvertSplitUpToAndPastTerminators_<T>(
        string[] separators,
        string[] terminators,
        Func<string, T> converter)
      => this.ReadSplitUpToAndPastTerminators_(separators, terminators)
             .Select(converter)
             .ToArray();
  }
}