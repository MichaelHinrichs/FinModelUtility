// Decompiled with JetBrains decompiler
// Type: SevenZip.Compression.LZ.BinTree
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
  public class BinTree : InWindow, IMatchFinder, IInWindowStream
  {
    private uint _cyclicBufferSize = 0;
    private uint _cutValue = (uint) byte.MaxValue;
    private uint _hashSizeSum = 0;
    private bool HASH_ARRAY = true;
    private uint kNumHashDirectBytes = 0;
    private uint kMinMatchCheck = 4;
    private uint kFixHashSize = 66560;
    private const uint kHash2Size = 1024;
    private const uint kHash3Size = 65536;
    private const uint kBT2HashSize = 65536;
    private const uint kStartMaxLen = 1;
    private const uint kHash3Offset = 1024;
    private const uint kEmptyHashValue = 0;
    private const uint kMaxValForNormalize = 2147483647;
    private uint _cyclicBufferPos;
    private uint _matchMaxLen;
    private uint[] _son;
    private uint[] _hash;
    private uint _hashMask;

    public void SetType(int numHashBytes)
    {
      this.HASH_ARRAY = numHashBytes > 2;
      if (this.HASH_ARRAY)
      {
        this.kNumHashDirectBytes = 0U;
        this.kMinMatchCheck = 4U;
        this.kFixHashSize = 66560U;
      }
      else
      {
        this.kNumHashDirectBytes = 2U;
        this.kMinMatchCheck = 3U;
        this.kFixHashSize = 0U;
      }
    }

    public new void SetStream(Stream stream)
    {
      base.SetStream(stream);
    }

    public new void ReleaseStream()
    {
      base.ReleaseStream();
    }

    public new void Init()
    {
      base.Init();
      for (uint index = 0; index < this._hashSizeSum; ++index)
        this._hash[(IntPtr) index] = 0U;
      this._cyclicBufferPos = 0U;
      this.ReduceOffsets(-1);
    }

    public new void MovePos()
    {
      if (++this._cyclicBufferPos >= this._cyclicBufferSize)
        this._cyclicBufferPos = 0U;
      base.MovePos();
      if (this._pos != (uint) int.MaxValue)
        return;
      this.Normalize();
    }

    public new byte GetIndexByte(int index)
    {
      return base.GetIndexByte(index);
    }

    public new uint GetMatchLen(int index, uint distance, uint limit)
    {
      return base.GetMatchLen(index, distance, limit);
    }

    public new uint GetNumAvailableBytes()
    {
      return base.GetNumAvailableBytes();
    }

    public void Create(
      uint historySize,
      uint keepAddBufferBefore,
      uint matchMaxLen,
      uint keepAddBufferAfter)
    {
      if (historySize > 2147483391U)
        throw new Exception();
      this._cutValue = 16U + (matchMaxLen >> 1);
      uint keepSizeReserv = (historySize + keepAddBufferBefore + matchMaxLen + keepAddBufferAfter) / 2U + 256U;
      this.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, keepSizeReserv);
      this._matchMaxLen = matchMaxLen;
      uint num1 = historySize + 1U;
      if ((int) this._cyclicBufferSize != (int) num1)
        this._son = new uint[(IntPtr) ((this._cyclicBufferSize = num1) * 2U)];
      uint num2 = 65536;
      if (this.HASH_ARRAY)
      {
        uint num3 = historySize - 1U;
        uint num4 = num3 | num3 >> 1;
        uint num5 = num4 | num4 >> 2;
        uint num6 = num5 | num5 >> 4;
        uint num7 = (num6 | num6 >> 8) >> 1 | (uint) ushort.MaxValue;
        if (num7 > 16777216U)
          num7 >>= 1;
        this._hashMask = num7;
        num2 = num7 + 1U + this.kFixHashSize;
      }
      if ((int) num2 == (int) this._hashSizeSum)
        return;
      this._hash = new uint[(IntPtr) (this._hashSizeSum = num2)];
    }

    public uint GetMatches(uint[] distances)
    {
      uint num1;
      if (this._pos + this._matchMaxLen <= this._streamPos)
      {
        num1 = this._matchMaxLen;
      }
      else
      {
        num1 = this._streamPos - this._pos;
        if (num1 < this.kMinMatchCheck)
        {
          this.MovePos();
          return 0;
        }
      }
      uint num2 = 0;
      uint num3 = this._pos > this._cyclicBufferSize ? this._pos - this._cyclicBufferSize : 0U;
      uint num4 = this._bufferOffset + this._pos;
      uint num5 = 1;
      uint num6 = 0;
      uint num7 = 0;
      uint num8;
      if (this.HASH_ARRAY)
      {
        uint num9 = CRC.Table[(int) this._bufferBase[(IntPtr) num4]] ^ (uint) this._bufferBase[(IntPtr) (num4 + 1U)];
        num6 = num9 & 1023U;
        uint num10 = num9 ^ (uint) this._bufferBase[(IntPtr) (num4 + 2U)] << 8;
        num7 = num10 & (uint) ushort.MaxValue;
        num8 = (num10 ^ CRC.Table[(int) this._bufferBase[(IntPtr) (num4 + 3U)]] << 5) & this._hashMask;
      }
      else
        num8 = (uint) this._bufferBase[(IntPtr) num4] ^ (uint) this._bufferBase[(IntPtr) (num4 + 1U)] << 8;
      uint num11 = this._hash[(IntPtr) (this.kFixHashSize + num8)];
      if (this.HASH_ARRAY)
      {
        uint num9 = this._hash[(IntPtr) num6];
        uint num10 = this._hash[(IntPtr) (1024U + num7)];
        this._hash[(IntPtr) num6] = this._pos;
        this._hash[(IntPtr) (1024U + num7)] = this._pos;
        if (num9 > num3 && (int) this._bufferBase[(IntPtr) (this._bufferOffset + num9)] == (int) this._bufferBase[(IntPtr) num4])
        {
          uint[] numArray1 = distances;
          int num12 = (int) num2;
          uint num13 = (uint) (num12 + 1);
          uint num14 = (uint) num12;
          int num15;
          num5 = (uint) (num15 = 2);
          numArray1[(IntPtr) num14] = (uint) num15;
          uint[] numArray2 = distances;
          int num16 = (int) num13;
          num2 = (uint) (num16 + 1);
          uint num17 = (uint) num16;
          int num18 = (int) this._pos - (int) num9 - 1;
          numArray2[(IntPtr) num17] = (uint) num18;
        }
        if (num10 > num3 && (int) this._bufferBase[(IntPtr) (this._bufferOffset + num10)] == (int) this._bufferBase[(IntPtr) num4])
        {
          if ((int) num10 == (int) num9)
            num2 -= 2U;
          uint[] numArray1 = distances;
          int num12 = (int) num2;
          uint num13 = (uint) (num12 + 1);
          uint num14 = (uint) num12;
          int num15;
          num5 = (uint) (num15 = 3);
          numArray1[(IntPtr) num14] = (uint) num15;
          uint[] numArray2 = distances;
          int num16 = (int) num13;
          num2 = (uint) (num16 + 1);
          uint num17 = (uint) num16;
          int num18 = (int) this._pos - (int) num10 - 1;
          numArray2[(IntPtr) num17] = (uint) num18;
          num9 = num10;
        }
        if (num2 != 0U && (int) num9 == (int) num11)
        {
          num2 -= 2U;
          num5 = 1U;
        }
      }
      this._hash[(IntPtr) (this.kFixHashSize + num8)] = this._pos;
      uint num19 = (uint) (((int) this._cyclicBufferPos << 1) + 1);
      uint num20 = this._cyclicBufferPos << 1;
      uint val2;
      uint val1 = val2 = this.kNumHashDirectBytes;
      if (this.kNumHashDirectBytes != 0U && num11 > num3 && (int) this._bufferBase[(IntPtr) (this._bufferOffset + num11 + this.kNumHashDirectBytes)] != (int) this._bufferBase[(IntPtr) (num4 + this.kNumHashDirectBytes)])
      {
        uint[] numArray1 = distances;
        int num9 = (int) num2;
        uint num10 = (uint) (num9 + 1);
        uint num12 = (uint) num9;
        int numHashDirectBytes;
        num5 = (uint) (numHashDirectBytes = (int) this.kNumHashDirectBytes);
        numArray1[(IntPtr) num12] = (uint) numHashDirectBytes;
        uint[] numArray2 = distances;
        int num13 = (int) num10;
        num2 = (uint) (num13 + 1);
        uint num14 = (uint) num13;
        int num15 = (int) this._pos - (int) num11 - 1;
        numArray2[(IntPtr) num14] = (uint) num15;
      }
      uint cutValue = this._cutValue;
      uint num21;
      while (true)
      {
        if (num11 > num3 && cutValue-- != 0U)
        {
          uint num9 = this._pos - num11;
          num21 = (uint) ((num9 <= this._cyclicBufferPos ? (int) this._cyclicBufferPos - (int) num9 : (int) this._cyclicBufferPos - (int) num9 + (int) this._cyclicBufferSize) << 1);
          uint num10 = this._bufferOffset + num11;
          uint num12 = Math.Min(val1, val2);
          if ((int) this._bufferBase[(IntPtr) (num10 + num12)] == (int) this._bufferBase[(IntPtr) (num4 + num12)])
          {
            do
              ;
            while ((int) ++num12 != (int) num1 && (int) this._bufferBase[(IntPtr) (num10 + num12)] == (int) this._bufferBase[(IntPtr) (num4 + num12)]);
            if (num5 < num12)
            {
              uint[] numArray1 = distances;
              int num13 = (int) num2;
              uint num14 = (uint) (num13 + 1);
              uint num15 = (uint) num13;
              int num16;
              num5 = (uint) (num16 = (int) num12);
              numArray1[(IntPtr) num15] = (uint) num16;
              uint[] numArray2 = distances;
              int num17 = (int) num14;
              num2 = (uint) (num17 + 1);
              uint num18 = (uint) num17;
              int num22 = (int) num9 - 1;
              numArray2[(IntPtr) num18] = (uint) num22;
              if ((int) num12 == (int) num1)
                goto label_25;
            }
          }
          if ((int) this._bufferBase[(IntPtr) (num10 + num12)] < (int) this._bufferBase[(IntPtr) (num4 + num12)])
          {
            this._son[(IntPtr) num20] = num11;
            num20 = num21 + 1U;
            num11 = this._son[(IntPtr) num20];
            val2 = num12;
          }
          else
          {
            this._son[(IntPtr) num19] = num11;
            num19 = num21;
            num11 = this._son[(IntPtr) num19];
            val1 = num12;
          }
        }
        else
          break;
      }
      this._son[(IntPtr) num19] = this._son[(IntPtr) num20] = 0U;
      goto label_32;
label_25:
      this._son[(IntPtr) num20] = this._son[(IntPtr) num21];
      this._son[(IntPtr) num19] = this._son[(IntPtr) (num21 + 1U)];
label_32:
      this.MovePos();
      return num2;
    }

    public void Skip(uint num)
    {
      do
      {
        uint num1;
        if (this._pos + this._matchMaxLen <= this._streamPos)
        {
          num1 = this._matchMaxLen;
        }
        else
        {
          num1 = this._streamPos - this._pos;
          if (num1 < this.kMinMatchCheck)
          {
            this.MovePos();
            goto label_21;
          }
        }
        uint num2 = this._pos > this._cyclicBufferSize ? this._pos - this._cyclicBufferSize : 0U;
        uint num3 = this._bufferOffset + this._pos;
        uint num4;
        if (this.HASH_ARRAY)
        {
          uint num5 = CRC.Table[(int) this._bufferBase[(IntPtr) num3]] ^ (uint) this._bufferBase[(IntPtr) (num3 + 1U)];
          this._hash[(IntPtr) (num5 & 1023U)] = this._pos;
          uint num6 = num5 ^ (uint) this._bufferBase[(IntPtr) (num3 + 2U)] << 8;
          this._hash[(IntPtr) (1024U + (num6 & (uint) ushort.MaxValue))] = this._pos;
          num4 = (num6 ^ CRC.Table[(int) this._bufferBase[(IntPtr) (num3 + 3U)]] << 5) & this._hashMask;
        }
        else
          num4 = (uint) this._bufferBase[(IntPtr) num3] ^ (uint) this._bufferBase[(IntPtr) (num3 + 1U)] << 8;
        uint num7 = this._hash[(IntPtr) (this.kFixHashSize + num4)];
        this._hash[(IntPtr) (this.kFixHashSize + num4)] = this._pos;
        uint num8 = (uint) (((int) this._cyclicBufferPos << 1) + 1);
        uint num9 = this._cyclicBufferPos << 1;
        uint val2;
        uint val1 = val2 = this.kNumHashDirectBytes;
        uint cutValue = this._cutValue;
        uint num10;
        while (true)
        {
          if (num7 > num2 && cutValue-- != 0U)
          {
            uint num5 = this._pos - num7;
            num10 = (uint) ((num5 <= this._cyclicBufferPos ? (int) this._cyclicBufferPos - (int) num5 : (int) this._cyclicBufferPos - (int) num5 + (int) this._cyclicBufferSize) << 1);
            uint num6 = this._bufferOffset + num7;
            uint num11 = Math.Min(val1, val2);
            if ((int) this._bufferBase[(IntPtr) (num6 + num11)] == (int) this._bufferBase[(IntPtr) (num3 + num11)])
            {
              do
                ;
              while ((int) ++num11 != (int) num1 && (int) this._bufferBase[(IntPtr) (num6 + num11)] == (int) this._bufferBase[(IntPtr) (num3 + num11)]);
              if ((int) num11 == (int) num1)
                goto label_14;
            }
            if ((int) this._bufferBase[(IntPtr) (num6 + num11)] < (int) this._bufferBase[(IntPtr) (num3 + num11)])
            {
              this._son[(IntPtr) num9] = num7;
              num9 = num10 + 1U;
              num7 = this._son[(IntPtr) num9];
              val2 = num11;
            }
            else
            {
              this._son[(IntPtr) num8] = num7;
              num8 = num10;
              num7 = this._son[(IntPtr) num8];
              val1 = num11;
            }
          }
          else
            break;
        }
        this._son[(IntPtr) num8] = this._son[(IntPtr) num9] = 0U;
        goto label_20;
label_14:
        this._son[(IntPtr) num9] = this._son[(IntPtr) num10];
        this._son[(IntPtr) num8] = this._son[(IntPtr) (num10 + 1U)];
label_20:
        this.MovePos();
label_21:;
      }
      while (--num != 0U);
    }

    private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
    {
      for (uint index = 0; index < numItems; ++index)
      {
        uint num1 = items[(IntPtr) index];
        uint num2 = num1 > subValue ? num1 - subValue : 0U;
        items[(IntPtr) index] = num2;
      }
    }

    private void Normalize()
    {
      uint subValue = this._pos - this._cyclicBufferSize;
      this.NormalizeLinks(this._son, this._cyclicBufferSize * 2U, subValue);
      this.NormalizeLinks(this._hash, this._hashSizeSum, subValue);
      this.ReduceOffsets((int) subValue);
    }

    public void SetCutValue(uint cutValue)
    {
      this._cutValue = cutValue;
    }
  }
}
