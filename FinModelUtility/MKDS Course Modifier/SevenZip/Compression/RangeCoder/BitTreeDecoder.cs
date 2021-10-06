// Decompiled with JetBrains decompiler
// Type: SevenZip.Compression.RangeCoder.BitTreeDecoder
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;

namespace SevenZip.Compression.RangeCoder
{
  internal struct BitTreeDecoder
  {
    private BitDecoder[] Models;
    private int NumBitLevels;

    public BitTreeDecoder(int numBitLevels)
    {
      this.NumBitLevels = numBitLevels;
      this.Models = new BitDecoder[1 << numBitLevels];
    }

    public void Init()
    {
      for (uint index = 1; (long) index < (long) (1 << this.NumBitLevels); ++index)
        this.Models[(IntPtr) index].Init();
    }

    public uint Decode(Decoder rangeDecoder)
    {
      uint num = 1;
      for (int numBitLevels = this.NumBitLevels; numBitLevels > 0; --numBitLevels)
        num = (num << 1) + this.Models[(IntPtr) num].Decode(rangeDecoder);
      return num - (uint) (1 << this.NumBitLevels);
    }

    public uint ReverseDecode(Decoder rangeDecoder)
    {
      uint num1 = 1;
      uint num2 = 0;
      for (int index = 0; index < this.NumBitLevels; ++index)
      {
        uint num3 = this.Models[(IntPtr) num1].Decode(rangeDecoder);
        num1 = (num1 << 1) + num3;
        num2 |= num3 << index;
      }
      return num2;
    }

    public static uint ReverseDecode(
      BitDecoder[] Models,
      uint startIndex,
      Decoder rangeDecoder,
      int NumBitLevels)
    {
      uint num1 = 1;
      uint num2 = 0;
      for (int index = 0; index < NumBitLevels; ++index)
      {
        uint num3 = Models[(IntPtr) (startIndex + num1)].Decode(rangeDecoder);
        num1 = (num1 << 1) + num3;
        num2 |= num3 << index;
      }
      return num2;
    }
  }
}
