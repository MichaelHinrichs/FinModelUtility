// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.IO.ARM9
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace MKDS_Course_Modifier.IO
{
  public class ARM9
  {
    private int SettingsOffset;
    public ARM9.ARM9Section[] Sections;

    public ARM9(byte[] data)
    {
      byte[] numArray = data;
      this.SettingsOffset = (int) Extensions.IndexOf(numArray, new byte[8]
      {
        (byte) 33,
        (byte) 6,
        (byte) 192,
        (byte) 222,
        (byte) 222,
        (byte) 192,
        (byte) 6,
        (byte) 33
      });
      this.SettingsOffset -= 28;
      if (Bytes.Read4BytesAsUInt32(numArray, this.SettingsOffset + 20) != 0U)
        numArray = this.DecompressARM9(numArray);
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(numArray), Endianness.LittleEndian);
      er.BaseStream.Position = (long) this.SettingsOffset;
      uint num1 = er.ReadUInt32() - 33554432U;
      uint num2 = er.ReadUInt32() - 33554432U;
      uint DataStart = er.ReadUInt32() - 33554432U;
      er.BaseStream.Position = (long) num1;
      this.Sections = new ARM9.ARM9Section[(IntPtr) ((num2 - num1) / 12U + 1U)];
      this.Sections[0] = new ARM9.ARM9Section(er, DataStart);
      for (int index = 0; (long) index < (long) ((num2 - num1) / 12U); ++index)
        this.Sections[index + 1] = new ARM9.ARM9Section(er, ref DataStart);
      er.Close();
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      for (int index = 0; index < this.Sections.Length; ++index)
      {
        er.Write(this.Sections[index].Data, 0, this.Sections[index].Data.Length);
        while (er.BaseStream.Position % 4L != 0L)
          er.Write((byte) 0);
      }
      er.BaseStream.Position = 3584L;
      for (int index = 1; index < this.Sections.Length; ++index)
      {
        if (this.Sections[index].Size != 0U)
          this.Sections[index].Write(er);
      }
      for (int index = 1; index < this.Sections.Length; ++index)
      {
        if (this.Sections[index].Size == 0U)
          this.Sections[index].Write(er);
      }
      er.BaseStream.Position = (long) this.SettingsOffset;
      er.Write(33558016U);
      er.Write((uint) (33558016 + (this.Sections.Length - 1) * 12));
      er.Write(this.Sections[0].Size + 33554432U);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public static byte[] Decompress(byte[] Data)
    {
      return new ARM9(Data).Write();
    }

    private byte[] DecompressARM9(byte[] Data)
    {
      try
      {
        uint num1 = Bytes.Read4BytesAsUInt32(Data, this.SettingsOffset + 20) - 33554432U;
        uint num2 = Bytes.Read4BytesAsUInt32(Data, (int) num1 - 8) & 16777215U;
        uint num3 = num1 - num2;
        byte[] sourcedata = new byte[(IntPtr) num2];
        Array.Copy((Array) Data, (long) num3, (Array) sourcedata, 0L, (long) num2);
        byte[] numArray1 = Compression.OverlayDecompress(sourcedata);
        byte[] numArray2 = new byte[Data.Length - sourcedata.Length + numArray1.Length];
        Array.Copy((Array) Data, (Array) numArray2, Data.Length);
        Array.Copy((Array) numArray1, 0L, (Array) numArray2, (long) num3, (long) numArray1.Length);
        numArray2[this.SettingsOffset + 20] = numArray2[this.SettingsOffset + 21] = numArray2[this.SettingsOffset + 22] = numArray2[this.SettingsOffset + 23] = (byte) 0;
        return numArray2;
      }
      catch
      {
        int num = (int) System.Windows.Forms.MessageBox.Show("Error: The file isn't an arm9 binary, isn't compressed or isn't compatible with the currently used decompressing method.");
        return (byte[]) null;
      }
    }

    public uint ReadUInt32(uint Offset)
    {
      for (int index = 0; index < this.Sections.Length; ++index)
      {
        if (Offset >= this.Sections[index].Start && Offset < this.Sections[index].Start + this.Sections[index].Size)
          return Bytes.Read4BytesAsUInt32(this.Sections[index].Data, (int) Offset - (int) this.Sections[index].Start);
      }
      return uint.MaxValue;
    }

    public bool WriteUInt32(uint Value, uint Offset)
    {
      for (int index = 0; index < this.Sections.Length; ++index)
      {
        if (Offset >= this.Sections[index].Start && Offset < this.Sections[index].Start + this.Sections[index].Size)
        {
          this.Sections[index].Data[(IntPtr) (Offset - this.Sections[index].Start)] = (byte) Value;
          this.Sections[index].Data[(IntPtr) (uint) ((int) Offset - (int) this.Sections[index].Start + 1)] = (byte) (Value >> 8);
          this.Sections[index].Data[(IntPtr) (uint) ((int) Offset - (int) this.Sections[index].Start + 2)] = (byte) (Value >> 16);
          this.Sections[index].Data[(IntPtr) (uint) ((int) Offset - (int) this.Sections[index].Start + 3)] = (byte) (Value >> 24);
          return true;
        }
      }
      return false;
    }

    public bool WriteUInt16(ushort Value, uint Offset)
    {
      for (int index = 0; index < this.Sections.Length; ++index)
      {
        if (Offset >= this.Sections[index].Start && Offset < this.Sections[index].Start + this.Sections[index].Size)
        {
          this.Sections[index].Data[(IntPtr) (Offset - this.Sections[index].Start)] = (byte) Value;
          this.Sections[index].Data[(IntPtr) (uint) ((int) Offset - (int) this.Sections[index].Start + 1)] = (byte) ((uint) Value >> 8);
          return true;
        }
      }
      return false;
    }

    public void AddCustomCode(string CodeDir)
    {
      uint Offset1 = uint.Parse(System.IO.File.ReadAllText(CodeDir + "\\arenaoffs.txt"), NumberStyles.HexNumber);
      uint num1 = this.ReadUInt32(Offset1);
      this.Compile(CodeDir, num1);
      byte[] Data = System.IO.File.ReadAllBytes(CodeDir + "\\newcode.bin");
      StreamReader streamReader = new StreamReader(CodeDir + "\\newcode.sym");
      string str;
      while ((str = streamReader.ReadLine()) != null)
      {
        string[] strArray = str.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 4 && strArray[3].Length >= 7)
        {
          switch (strArray[3].Remove(6))
          {
            case "arepl_":
              uint Offset2 = uint.Parse(strArray[3].Replace("arepl_", ""), NumberStyles.HexNumber);
              if (!this.WriteUInt32(3942645760U | (uint) ((int) (uint.Parse(strArray[0], NumberStyles.HexNumber) / 4U) - (int) (Offset2 / 4U) - 2) & 16777215U, Offset2))
              {
                int num2 = (int) System.Windows.MessageBox.Show("The offset of function " + strArray[3] + " is invalid. Maybe your code is inside an overlay or you wrote the wrong offset.");
                break;
              }
              break;
            case "ansub_":
              uint Offset3 = uint.Parse(strArray[3].Replace("ansub_", ""), NumberStyles.HexNumber);
              if (!this.WriteUInt32(3925868544U | (uint) ((int) (uint.Parse(strArray[0], NumberStyles.HexNumber) / 4U) - (int) (Offset3 / 4U) - 2) & 16777215U, Offset3))
              {
                int num2 = (int) System.Windows.MessageBox.Show("The offset of function " + strArray[3] + " is invalid. Maybe your code is inside an overlay or you wrote the wrong offset.");
                break;
              }
              break;
            case "trepl_":
              uint Offset4 = uint.Parse(strArray[3].Replace("trepl_", ""), NumberStyles.HexNumber);
              ushort num3 = 61440;
              ushort num4 = 59392;
              uint num5 = (uint) ((int) uint.Parse(strArray[0], NumberStyles.HexNumber) - (int) Offset4 - 2) >> 1 & 4194303U;
              ushort num6 = (ushort) ((uint) num3 | (uint) (ushort) (num5 >> 11 & 2047U));
              ushort num7 = (ushort) ((uint) num4 | (uint) (ushort) (num5 & 2046U));
              if (!this.WriteUInt16(num6, Offset4))
              {
                int num2 = (int) System.Windows.MessageBox.Show("The offset of function " + strArray[3] + " is invalid. Maybe your code is inside an overlay or you wrote the wrong offset.\r\nIf your code is inside an overlay, this is an action replay code to let your asm hack still work:\r\n1" + string.Format("{0:X7}", (object) Offset4) + " 0000" + string.Format("{0:X4}", (object) num6) + "\r\n1" + string.Format("{0:X7}", (object) (uint) ((int) Offset4 + 2)) + " 0000" + string.Format("{0:X4}", (object) num7));
                break;
              }
              this.WriteUInt16(num7, Offset4 + 2U);
              break;
          }
        }
      }
      streamReader.Close();
      this.WriteUInt32((uint) ((ulong) num1 + (ulong) Data.Length), Offset1);
      Array.Resize<ARM9.ARM9Section>(ref this.Sections, this.Sections.Length + 1);
      this.Sections[this.Sections.Length - 1] = new ARM9.ARM9Section(num1, Data);
      System.IO.File.Delete(CodeDir + "\\newcode.bin");
      System.IO.File.Delete(CodeDir + "\\newcode.elf");
      System.IO.File.Delete(CodeDir + "\\newcode.sym");
      System.IO.Directory.Delete(CodeDir + "\\build", true);
    }

    private void Compile(string CodeDir, uint ArenaLo)
    {
      Process process = new Process();
      process.StartInfo.FileName = "cmd";
      process.StartInfo.Arguments = "/C make CODEADDR=0x" + ArenaLo.ToString("X8") + " || pause";
      process.StartInfo.WorkingDirectory = CodeDir;
      process.Start();
      process.WaitForExit();
    }

    public class ARM9Section
    {
      public uint Start;
      public uint Size;
      public uint BlockStartedBySymbolSize;
      public byte[] Data;

      public ARM9Section(EndianBinaryReader er, ref uint DataStart)
      {
        this.Start = er.ReadUInt32();
        this.Size = er.ReadUInt32();
        this.BlockStartedBySymbolSize = er.ReadUInt32();
        long position = er.BaseStream.Position;
        er.BaseStream.Position = (long) DataStart;
        this.Data = er.ReadBytes((int) this.Size);
        er.BaseStream.Position = position;
        DataStart += this.Size;
      }

      public ARM9Section(EndianBinaryReader er, uint DataStart)
      {
        this.Start = 33554432U;
        this.Size = DataStart;
        this.BlockStartedBySymbolSize = 0U;
        long position = er.BaseStream.Position;
        er.BaseStream.Position = 0L;
        this.Data = er.ReadBytes((int) this.Size);
        er.BaseStream.Position = position;
      }

      public ARM9Section(uint Start, byte[] Data)
      {
        this.Start = Start;
        this.Data = Data;
        this.Size = (uint) Data.Length;
        this.BlockStartedBySymbolSize = 0U;
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.Start);
        er.Write(this.Size);
        er.Write(this.BlockStartedBySymbolSize);
      }
    }

    public class ArmArchitecture
    {
      public static void RunCode(NDS Rom)
      {
        ARM9.ArmArchitecture.ArmContext Context = new ARM9.ArmArchitecture.ArmContext();
        Array.Copy((Array) Rom.Arm9, 0L, (Array) Context.Memory, (long) Rom.ARM9LoadAddress, (long) Rom.Arm9.Length);
        Context.Registers[15] = Rom.ARM9EntryAddress;
        while (true)
        {
          if (!Context.T)
          {
            uint Instruction = Bytes.Read4BytesAsUInt32(Context.Memory, (int) Context.Registers[15]);
            uint num = Instruction >> 28;
            uint Op = Instruction >> 25 & 7U;
            switch (Op)
            {
              case 0:
              case 1:
                ARM9.ArmArchitecture.DataProc(Instruction, Op, ref Context);
                Context.Registers[15] += 4U;
                break;
              case 2:
              case 3:
                ARM9.ArmArchitecture.SingleDataTrans(Instruction, Op, ref Context);
                Context.Registers[15] += 4U;
                break;
              case 5:
                ARM9.ArmArchitecture.Branch(Instruction, Op, ref Context);
                break;
              case 7:
                Context.Registers[15] += 4U;
                break;
              default:
                Context.Registers[15] += 4U;
                break;
            }
          }
        }
      }

      private static uint Shift(uint ShiftType, uint Value, uint NrBits)
      {
        switch (ShiftType)
        {
          case 0:
            return Value << (int) NrBits;
          case 1:
            return Value >> (int) NrBits;
          case 2:
            return Value << (int) NrBits;
          case 3:
            return Value >> (int) NrBits | Value << 32 - (int) NrBits;
          default:
            return uint.MaxValue;
        }
      }

      private static void DataProc(
        uint Instruction,
        uint Op,
        ref ARM9.ArmArchitecture.ArmContext Context)
      {
        bool flag1 = ((int) Op & 1) == 1;
        uint num1 = Instruction >> 21 & 15U;
        bool flag2 = ((int) (Instruction >> 20) & 1) == 1;
        uint num2 = Instruction >> 16 & 15U;
        uint num3 = Instruction >> 12 & 15U;
        uint num4;
        if (flag1)
        {
          uint num5 = Instruction >> 8 & 15U;
          uint num6 = Instruction & (uint) byte.MaxValue;
          num4 = num6 >> (int) num5 * 2 | num6 << 32 - (int) num5 * 2;
        }
        else
        {
          uint ShiftType = Instruction >> 5 & 3U;
          bool flag3 = ((int) (Instruction >> 4) & 1) == 1;
          uint num5 = Instruction & 15U;
          if (flag3)
          {
            uint NrBits = Instruction >> 7 & 31U;
            num4 = ARM9.ArmArchitecture.Shift(ShiftType, Context.Registers[(IntPtr) num5], NrBits);
          }
          else
          {
            uint num6 = Instruction >> 8 & 15U;
            uint num7 = Instruction >> 7 & 1U;
            num4 = ARM9.ArmArchitecture.Shift(ShiftType, Context.Registers[(IntPtr) num5], Context.Registers[(IntPtr) num6] & (uint) byte.MaxValue);
          }
        }
        switch (num1)
        {
          case 12:
            Context.Registers[(IntPtr) num3] = Context.Registers[(IntPtr) num2] | num4;
            break;
          case 13:
            Context.Registers[(IntPtr) num3] = num4;
            break;
          case 14:
            Context.Registers[(IntPtr) num3] = Context.Registers[(IntPtr) num2] & ~num4;
            break;
        }
        if (!flag2)
          return;
        bool flag4 = Context.Registers[(IntPtr) num3] == 0U;
        bool flag5 = Context.Registers[(IntPtr) num3] >> 31 == 1U;
        if (num3 != 15U)
        {
          switch (num1)
          {
          }
        }
      }

      private static void SingleDataTrans(
        uint Instruction,
        uint Op,
        ref ARM9.ArmArchitecture.ArmContext Context)
      {
        bool flag1 = ((int) Op & 1) == 1;
        bool flag2 = ((int) (Instruction >> 24) & 1) == 1;
        bool flag3 = ((int) (Instruction >> 23) & 1) == 1;
        bool flag4 = ((int) (Instruction >> 22) & 1) == 1;
        bool flag5 = false;
        bool flag6 = false;
        if (flag2)
          flag6 = ((int) (Instruction >> 21) & 1) == 1;
        else
          flag5 = ((int) (Instruction >> 21) & 1) == 1;
        bool flag7 = ((int) (Instruction >> 20) & 1) == 1;
        uint num1 = Instruction >> 16 & 15U;
        uint num2 = Instruction >> 12 & 15U;
        uint num3;
        if (flag1)
        {
          uint NrBits = Instruction >> 7 & 31U;
          uint ShiftType = Instruction >> 5 & 3U;
          uint num4 = Instruction >> 4 & 1U;
          uint num5 = Instruction & 15U;
          num3 = ARM9.ArmArchitecture.Shift(ShiftType, Context.Registers[(IntPtr) num5], NrBits);
        }
        else
          num3 = Instruction & 4095U;
        uint register = Context.Registers[(IntPtr) num1];
        if (num1 == 15U)
          register += 8U;
        if (flag2)
        {
          if (flag3)
            register += num3;
          else
            register -= num3;
          if (flag6)
            Context.Registers[(IntPtr) num1] = register;
        }
        if (flag7)
          Context.Registers[(IntPtr) num2] = !flag4 ? Bytes.Read4BytesAsUInt32(Context.Memory, (int) register) : (uint) Context.Memory[(IntPtr) register];
        else if (num2 == 15U)
        {
          if (flag4)
            Context.Memory[(IntPtr) register] = (byte) ((int) Context.Registers[(IntPtr) num2] + 12 & (int) byte.MaxValue);
          else
            Array.Copy((Array) BitConverter.GetBytes(Context.Registers[(IntPtr) num2] + 12U), 0L, (Array) Context.Memory, (long) register, 4L);
        }
        else if (flag4)
          Context.Memory[(IntPtr) register] = (byte) (Context.Registers[(IntPtr) num2] & (uint) byte.MaxValue);
        else
          Array.Copy((Array) BitConverter.GetBytes(Context.Registers[(IntPtr) num2]), 0L, (Array) Context.Memory, (long) register, 4L);
        if (flag2)
          return;
        uint num6 = !flag3 ? register - num3 : register + num3;
        Context.Registers[(IntPtr) num1] = num6;
      }

      private static void Branch(
        uint Instruction,
        uint Op,
        ref ARM9.ArmArchitecture.ArmContext Context)
      {
        uint num1 = Instruction >> 24 & 1U;
        int num2 = ((int) Instruction & 16777215) << 8 >> 8;
        if (num1 == 1U)
          Context.Registers[14] = Context.Registers[15] + 4U;
        Context.Registers[15] = (uint) ((int) Context.Registers[15] + 8 + num2 * 4);
      }

      public class ArmContext
      {
        public byte[] Memory = new byte[134217728];
        public uint[] Registers = new uint[16];
        public bool T = false;
        public uint CPSR;
        public uint SPSR;
      }
    }
  }
}
