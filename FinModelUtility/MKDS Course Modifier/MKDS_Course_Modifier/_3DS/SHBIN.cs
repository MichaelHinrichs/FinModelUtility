// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3DS.SHBIN
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Exceptions;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MKDS_Course_Modifier._3DS
{
  public class SHBIN
  {
    public SHBIN.DVLB Dvlb;
    public SHBIN.DVLP Dvlp;
    public SHBIN.DVLE[] Dvle;

    public SHBIN(byte[] data)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(data), Endianness.LittleEndian);
      this.Dvlb = new SHBIN.DVLB(er);
      this.Dvlp = new SHBIN.DVLP(er);
      this.Dvle = new SHBIN.DVLE[(IntPtr) this.Dvlb.NrDVLE];
      for (int index = 0; (long) index < (long) this.Dvlb.NrDVLE; ++index)
      {
        er.BaseStream.Position = (long) this.Dvlb.DVLEOffsets[index];
        this.Dvle[index] = new SHBIN.DVLE(er);
      }
      er.Close();
    }

    public void DumpProgram(int Index)
    {
      this.Dump(this.Dvle[Index], 0U, this.Dvlp.ProgramLength);
    }

    private void Dump(SHBIN.DVLE d, uint Offset, uint NrInstructions)
    {
      int offset = (int) Offset * 4;
      for (int index = 0; (long) index < (long) NrInstructions; ++index)
      {
        foreach (SHBIN.DVLE.Label label in d.Labels)
        {
          if ((long) label.LabelProgramOffset == (long) (offset / 4))
            Console.WriteLine(d.SymbolTable[(int) label.SymbolOffset] + ":");
        }
        uint num1 = Bytes.Read4BytesAsUInt32(this.Dvlp.Program, offset);
        offset += 4;
        uint num2 = num1 >> 26;
        uint num3 = num2;
        if (num3 <= 8U)
        {
          switch ((int) num3 - 1)
          {
            case 0:
              SHBIN.DVLP.ExtensionTableEntry extensionTableEntry1 = this.Dvlp.ExtensionTableEntries[(IntPtr) (num1 & 63U)];
              Console.WriteLine("    DP3 - {0}.{1} dot {2}.{3} -> {4}.{5}", (object) this.GetInputName(d, num1 >> 12 & 63U), (object) extensionTableEntry1.GetSource1Mask(), (object) this.GetInputName(d, num1 >> 6 & 63U), (object) extensionTableEntry1.GetSource2Mask(), (object) this.GetOutputName(d, num1 >> 20 & 63U), (object) extensionTableEntry1.GetOutputMask());
              continue;
            case 1:
              SHBIN.DVLP.ExtensionTableEntry extensionTableEntry2 = this.Dvlp.ExtensionTableEntries[(IntPtr) (num1 & 63U)];
              Console.WriteLine("    DP4 - {0}.{1} dot {2}.{3} -> {4}.{5}", (object) this.GetInputName(d, num1 >> 12 & 63U), (object) extensionTableEntry2.GetSource1Mask(), (object) this.GetInputName(d, num1 >> 6 & 63U), (object) extensionTableEntry2.GetSource2Mask(), (object) this.GetOutputName(d, num1 >> 20 & 63U), (object) extensionTableEntry2.GetOutputMask());
              continue;
            default:
              if (num3 == 8U)
              {
                SHBIN.DVLP.ExtensionTableEntry extensionTableEntry3 = this.Dvlp.ExtensionTableEntries[(IntPtr) (num1 & 63U)];
                Console.WriteLine("    MUL - {0}.{1} * {2}.{3} -> {4}.{5}", (object) this.GetInputName(d, num1 >> 12 & 63U), (object) extensionTableEntry3.GetSource1Mask(), (object) this.GetInputName(d, num1 >> 6 & 63U), (object) extensionTableEntry3.GetSource2Mask(), (object) this.GetOutputName(d, num1 >> 20 & 63U), (object) extensionTableEntry3.GetOutputMask());
                continue;
              }
              break;
          }
        }
        else if (num3 != 11U)
        {
          if (num3 != 19U)
          {
            switch ((int) num3 - 33)
            {
              case 0:
                Console.WriteLine("    END21");
                continue;
              case 1:
                Console.WriteLine("    END22");
                continue;
              case 3:
                uint num4 = num1 & 1023U;
                uint num5 = num1 >> 10 & 1023U;
                string str = "";
                foreach (SHBIN.DVLE.Label label in d.Labels)
                {
                  if ((int) label.LabelProgramOffset == (int) num5)
                    str = d.SymbolTable[(int) label.SymbolOffset];
                }
                Console.WriteLine("    CAL - " + str);
                continue;
            }
          }
          else
          {
            SHBIN.DVLP.ExtensionTableEntry extensionTableEntry3 = this.Dvlp.ExtensionTableEntries[(IntPtr) (num1 & 63U)];
            Console.WriteLine("    MOV - {0}.{1} -> {2}.{3}", new object[4]
            {
              (object) this.GetInputName(d, num1 >> 12 & 63U),
              (object) extensionTableEntry3.GetSource1Mask(),
              (object) this.GetOutputName(d, num1 >> 20 & 63U),
              (object) extensionTableEntry3.GetOutputMask()
            });
            continue;
          }
        }
        else
        {
          SHBIN.DVLP.ExtensionTableEntry extensionTableEntry3 = this.Dvlp.ExtensionTableEntries[(IntPtr) (num1 & 63U)];
          Console.WriteLine("    UNKB - {0}.{1}, {2}.{3} -> {4}.{5}", (object) this.GetInputName(d, num1 >> 12 & 63U), (object) extensionTableEntry3.GetSource1Mask(), (object) this.GetInputName(d, num1 >> 6 & 63U), (object) extensionTableEntry3.GetSource2Mask(), (object) this.GetOutputName(d, num1 >> 20 & 63U), (object) extensionTableEntry3.GetOutputMask());
          continue;
        }
        if (num2 < 32U)
        {
          SHBIN.DVLP.ExtensionTableEntry extensionTableEntry3 = this.Dvlp.ExtensionTableEntries[(IntPtr) (num1 & 63U)];
          Console.WriteLine("    {0:X} - {1}.{2}, {3:X}.{4} -> {5}.{6}", (object) num2, (object) this.GetInputName(d, num1 >> 12 & 63U), (object) extensionTableEntry3.GetSource1Mask(), (object) this.GetInputName(d, num1 >> 6 & 63U), (object) extensionTableEntry3.GetSource2Mask(), (object) this.GetOutputName(d, num1 >> 20 & 63U), (object) extensionTableEntry3.GetOutputMask());
        }
        else
          Console.WriteLine("    {0:X}", (object) num2);
      }
      foreach (SHBIN.DVLE.Label label in d.Labels)
      {
        if ((long) label.LabelProgramOffset == (long) (offset / 4))
          Console.WriteLine(d.SymbolTable[(int) label.SymbolOffset] + ":");
      }
    }

    private string GetInputName(SHBIN.DVLE d, uint r)
    {
      if (r >> 4 == 0U)
      {
        for (int index = 0; (long) index < (long) d.NrVariables; ++index)
        {
          if ((int) d.Variables[index].StartRegister == (int) r)
            return d.SymbolTable[(int) d.Variables[index].SymbolOffset];
        }
      }
      else if (r >> 4 == 2U)
      {
        r = (uint) ((int) r & 15 | 16);
        for (int index = 0; (long) index < (long) d.NrVariables; ++index)
        {
          if ((uint) d.Variables[index].StartRegister <= r && (uint) d.Variables[index].EndRegister >= r)
            return d.SymbolTable[(int) d.Variables[index].SymbolOffset] + "[" + (object) (uint) ((int) r - (int) d.Variables[index].StartRegister) + "]";
        }
      }
      return string.Format("{0:X}", (object) r);
    }

    private string GetOutputName(SHBIN.DVLE d, uint r)
    {
      if (r >> 4 == 0U)
      {
        switch (r)
        {
          case 0:
            return "gl_Position";
          case 2:
            return "gl_Color";
          case 4:
            return "gl_TexCoord[0]";
          case 6:
            return "gl_TexCoord[1]";
          case 8:
            return "gl_Texcoord[2]";
        }
      }
      return string.Format("{0:X}", (object) r);
    }

    public class DVLB
    {
      public string Signature;
      public uint NrDVLE;
      public uint[] DVLEOffsets;

      public DVLB(EndianBinaryReader er)
      {
        uint position = (uint) er.BaseStream.Position;
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (DVLB))
          throw new SignatureNotCorrectException(this.Signature, nameof (DVLB), er.BaseStream.Position);
        this.NrDVLE = er.ReadUInt32();
        this.DVLEOffsets = new uint[(IntPtr) this.NrDVLE];
        for (int index = 0; (long) index < (long) this.NrDVLE; ++index)
          this.DVLEOffsets[index] = position + er.ReadUInt32();
      }
    }

    public class DVLP
    {
      public string Signature;
      public uint Flags;
      public uint ProgramOffset;
      public uint ProgramLength;
      public uint ExtensionTableOffset;
      public uint NrExtensionTableEntries;
      public uint FileNameTableOffset;
      public uint FileNameTableLength;
      public byte[] Program;
      public SHBIN.DVLP.ExtensionTableEntry[] ExtensionTableEntries;
      public Dictionary<int, string> FileNameTable;

      public DVLP(EndianBinaryReader er)
      {
        uint position1 = (uint) er.BaseStream.Position;
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (DVLP))
          throw new SignatureNotCorrectException(this.Signature, nameof (DVLP), er.BaseStream.Position);
        this.Flags = er.ReadUInt32();
        this.ProgramOffset = position1 + er.ReadUInt32();
        this.ProgramLength = er.ReadUInt32();
        this.ExtensionTableOffset = position1 + er.ReadUInt32();
        this.NrExtensionTableEntries = er.ReadUInt32();
        this.FileNameTableOffset = position1 + er.ReadUInt32();
        this.FileNameTableLength = er.ReadUInt32();
        uint position2 = (uint) er.BaseStream.Position;
        er.BaseStream.Position = (long) this.ProgramOffset;
        this.Program = er.ReadBytes((int) this.ProgramLength * 4);
        er.BaseStream.Position = (long) this.ExtensionTableOffset;
        this.ExtensionTableEntries = new SHBIN.DVLP.ExtensionTableEntry[(IntPtr) this.NrExtensionTableEntries];
        for (int index = 0; (long) index < (long) this.NrExtensionTableEntries; ++index)
          this.ExtensionTableEntries[index] = new SHBIN.DVLP.ExtensionTableEntry(er);
        er.BaseStream.Position = (long) this.FileNameTableOffset;
        this.FileNameTable = new Dictionary<int, string>();
        int key = 0;
        while (er.BaseStream.Position < (long) (this.FileNameTableOffset + this.FileNameTableLength))
        {
          string str = er.ReadStringNT(Encoding.ASCII);
          this.FileNameTable.Add(key, str);
          key += str.Length + 1;
        }
        while (er.BaseStream.Position % 4L != 0L)
        {
          int num = (int) er.ReadByte();
        }
      }

      public class ExtensionTableEntry
      {
        public uint Descriptor;
        public uint Unknown1;

        public ExtensionTableEntry(EndianBinaryReader er)
        {
          this.Descriptor = er.ReadUInt32();
          this.Unknown1 = er.ReadUInt32();
        }

        public string GetOutputMask()
        {
          string str1 = "";
          string str2 = ((int) this.Descriptor & 8) == 0 ? str1 + "_" : str1 + "x";
          string str3 = ((int) this.Descriptor & 4) == 0 ? str2 + "_" : str2 + "y";
          string str4 = ((int) this.Descriptor & 2) == 0 ? str3 + "_" : str3 + "z";
          return ((int) this.Descriptor & 1) == 0 ? str4 + "_" : str4 + "w";
        }

        public string GetSource1Mask()
        {
          return "" + this.GetComponentString((SHBIN.DVLP.ExtensionTableEntry.Component) ((int) (this.Descriptor >> 11) & 3)) + this.GetComponentString((SHBIN.DVLP.ExtensionTableEntry.Component) ((int) (this.Descriptor >> 9) & 3)) + this.GetComponentString((SHBIN.DVLP.ExtensionTableEntry.Component) ((int) (this.Descriptor >> 7) & 3)) + this.GetComponentString((SHBIN.DVLP.ExtensionTableEntry.Component) ((int) (this.Descriptor >> 5) & 3));
        }

        public string GetSource2Mask()
        {
          return "" + this.GetComponentString((SHBIN.DVLP.ExtensionTableEntry.Component) ((int) (this.Descriptor >> 20) & 3)) + this.GetComponentString((SHBIN.DVLP.ExtensionTableEntry.Component) ((int) (this.Descriptor >> 18) & 3)) + this.GetComponentString((SHBIN.DVLP.ExtensionTableEntry.Component) ((int) (this.Descriptor >> 16) & 3)) + this.GetComponentString((SHBIN.DVLP.ExtensionTableEntry.Component) ((int) (this.Descriptor >> 14) & 3));
        }

        private string GetComponentString(SHBIN.DVLP.ExtensionTableEntry.Component c)
        {
          switch (c)
          {
            case SHBIN.DVLP.ExtensionTableEntry.Component.X:
              return "x";
            case SHBIN.DVLP.ExtensionTableEntry.Component.Y:
              return "y";
            case SHBIN.DVLP.ExtensionTableEntry.Component.Z:
              return "z";
            case SHBIN.DVLP.ExtensionTableEntry.Component.W:
              return "w";
            default:
              return "";
          }
        }

        public enum Component
        {
          X,
          Y,
          Z,
          W,
        }

        [Flags]
        public enum ComponentMask
        {
          X = 8,
          Y = 4,
          Z = 2,
          W = 1,
        }
      }
    }

    public class DVLE
    {
      public string Signature;
      public uint Flags;
      public uint ProgramMainOffset;
      public uint ProgramMainEndOffset;
      public ushort Unknown1;
      public ushort Unknown2;
      public uint Unknown3;
      public uint UniformTableOffset;
      public uint NrUniforms;
      public uint LabelTableOffset;
      public uint NrLabels;
      public uint Table2Offset;
      public uint NrTable2Entries;
      public uint VariableTableOffset;
      public uint NrVariables;
      public uint SymbolTableOffset;
      public uint SymbolTableLength;
      public SHBIN.DVLE.Uniform[] Uniforms;
      public SHBIN.DVLE.Label[] Labels;
      public SHBIN.DVLE.Output[] Outputs;
      public SHBIN.DVLE.Variable[] Variables;
      public Dictionary<int, string> SymbolTable;

      public DVLE(EndianBinaryReader er)
      {
        uint position = (uint) er.BaseStream.Position;
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (DVLE))
          throw new SignatureNotCorrectException(this.Signature, nameof (DVLE), er.BaseStream.Position);
        this.Flags = er.ReadUInt32();
        this.ProgramMainOffset = er.ReadUInt32();
        this.ProgramMainEndOffset = er.ReadUInt32();
        this.Unknown1 = er.ReadUInt16();
        this.Unknown2 = er.ReadUInt16();
        this.Unknown3 = er.ReadUInt32();
        this.UniformTableOffset = position + er.ReadUInt32();
        this.NrUniforms = er.ReadUInt32();
        this.LabelTableOffset = position + er.ReadUInt32();
        this.NrLabels = er.ReadUInt32();
        this.Table2Offset = position + er.ReadUInt32();
        this.NrTable2Entries = er.ReadUInt32();
        this.VariableTableOffset = position + er.ReadUInt32();
        this.NrVariables = er.ReadUInt32();
        this.SymbolTableOffset = position + er.ReadUInt32();
        this.SymbolTableLength = er.ReadUInt32();
        er.BaseStream.Position = (long) this.UniformTableOffset;
        this.Uniforms = new SHBIN.DVLE.Uniform[(IntPtr) this.NrUniforms];
        for (int index = 0; (long) index < (long) this.NrUniforms; ++index)
          this.Uniforms[index] = new SHBIN.DVLE.Uniform(er);
        er.BaseStream.Position = (long) this.LabelTableOffset;
        this.Labels = new SHBIN.DVLE.Label[(IntPtr) this.NrLabels];
        for (int index = 0; (long) index < (long) this.NrLabels; ++index)
          this.Labels[index] = new SHBIN.DVLE.Label(er);
        er.BaseStream.Position = (long) this.Table2Offset;
        this.Outputs = new SHBIN.DVLE.Output[(IntPtr) this.NrTable2Entries];
        for (int index = 0; (long) index < (long) this.NrTable2Entries; ++index)
          this.Outputs[index] = new SHBIN.DVLE.Output(er);
        er.BaseStream.Position = (long) this.VariableTableOffset;
        this.Variables = new SHBIN.DVLE.Variable[(IntPtr) this.NrVariables];
        for (int index = 0; (long) index < (long) this.NrVariables; ++index)
          this.Variables[index] = new SHBIN.DVLE.Variable(er);
        er.BaseStream.Position = (long) this.SymbolTableOffset;
        this.SymbolTable = new Dictionary<int, string>();
        int key = 0;
        while (er.BaseStream.Position < (long) (this.SymbolTableOffset + this.SymbolTableLength))
        {
          string str = er.ReadStringNT(Encoding.ASCII);
          this.SymbolTable.Add(key, str);
          key += str.Length + 1;
        }
        while (er.BaseStream.Position % 4L != 0L)
        {
          int num = (int) er.ReadByte();
        }
      }

      public class Uniform
      {
        public ushort Unknown1;
        public ushort UniformID;
        public Vector4 Value;

        public Uniform(EndianBinaryReader er)
        {
          this.Unknown1 = er.ReadUInt16();
          this.UniformID = er.ReadUInt16();
          this.Value = new Vector4(this.ReadFloat24(er), this.ReadFloat24(er), this.ReadFloat24(er), this.ReadFloat24(er));
        }

        private float ReadFloat24(EndianBinaryReader er)
        {
          uint num = er.ReadUInt32() & 16777215U;
          return num == 0U ? 0.0f : BitConverter.ToSingle(BitConverter.GetBytes((uint) (((int) (num >> 16) & (int) sbyte.MaxValue) + 64 << 23 | ((int) (num >> 23) & 1) << 31 | ((int) num & (int) ushort.MaxValue) << 7)), 0);
        }
      }

      public class Label
      {
        public ushort LabelID;
        public ushort Unknown1;
        public uint LabelProgramOffset;
        public uint Unknown2;
        public uint SymbolOffset;

        public Label(EndianBinaryReader er)
        {
          this.LabelID = er.ReadUInt16();
          this.Unknown1 = er.ReadUInt16();
          this.LabelProgramOffset = er.ReadUInt32();
          this.Unknown2 = er.ReadUInt32();
          this.SymbolOffset = er.ReadUInt32();
        }
      }

      public class Output
      {
        public ushort Unknown1;
        public ushort Unknown2;
        public ushort OutputType;
        public ushort RegisterID;

        public Output(EndianBinaryReader er)
        {
          this.Unknown1 = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.OutputType = er.ReadUInt16();
          this.RegisterID = er.ReadUInt16();
        }
      }

      public class Variable
      {
        public uint SymbolOffset;
        public ushort StartRegister;
        public ushort EndRegister;

        public Variable(EndianBinaryReader er)
        {
          this.SymbolOffset = er.ReadUInt32();
          this.StartRegister = er.ReadUInt16();
          this.EndRegister = er.ReadUInt16();
        }
      }
    }
  }
}
