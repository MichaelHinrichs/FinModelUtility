// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G2D_Binary_File_Format.NCER
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using bmd.G3D_Binary_File_Format;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace bmd.G2D_Binary_File_Format
{
  public class NCER
  {
    public const string Signature = "RECN";
    public FileHeader.HeaderInfo Header;
    public NCER.cellBankBlock CellBankBlock;
    public NCER.labelBlock LabelBlock;

    public NCER(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, "RECN", out OK);
      if (!OK)
      {
        // TODO: Message box
        // int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.CellBankBlock = new NCER.cellBankBlock(er, out OK);
        if (!OK)
        {
          // TODO: Message box
          //int num2 = (int) MessageBox.Show("Error 1");
        }
        else if (this.Header.dataBlocks > (ushort) 1)
        {
          er.BaseStream.Position = (long) (this.CellBankBlock.Header.size + (uint) this.Header.headerSize);
          this.LabelBlock = new NCER.labelBlock(er, (int) this.CellBankBlock.CellDataBank.numCells, out OK);
          if (OK)
            ;
        }
      }
      er.Close();
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.dataBlocks = (ushort) 1;
      this.Header.Write(er);
      this.CellBankBlock.Write(er);
      er.BaseStream.Position = 8L;
      er.WriteUInt32((uint) er.BaseStream.Length);
      er.BaseStream.Position = 20L;
      er.WriteUInt32((uint) ((ulong) er.BaseStream.Length - 16UL));
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class cellBankBlock
    {
      public const string Signature = "KBEC";
      public DataBlockHeader Header;
      public NCER.cellBankBlock.cellDataBank CellDataBank;

      public cellBankBlock(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "KBEC", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.CellDataBank = new NCER.cellBankBlock.cellDataBank(er);
          OK = true;
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        long num = er.BaseStream.Position + 4L;
        this.Header.Write(er, 0);
        this.CellDataBank.Write(er);
        long position = er.BaseStream.Position;
        er.BaseStream.Position = num;
        er.WriteUInt32((uint) (position - num));
        er.BaseStream.Position = position;
      }

      public class cellDataBank
      {
        public ushort numCells;
        public ushort cellBankAttr;
        public uint pCellDataArrayHead;
        public NCER.cellBankBlock.cellDataBank.CharacterDataMapingType mappingMode;
        public uint pVramTransferData;
        public uint pStringBank;
        public uint pExtendedData;
        public NCER.cellBankBlock.cellDataBank.cellData[] CellData;

        public cellDataBank(EndianBinaryReader er)
        {
          this.numCells = er.ReadUInt16();
          this.cellBankAttr = er.ReadUInt16();
          this.pCellDataArrayHead = er.ReadUInt32();
          this.mappingMode = (NCER.cellBankBlock.cellDataBank.CharacterDataMapingType) er.ReadUInt32();
          this.pVramTransferData = er.ReadUInt32();
          this.pStringBank = er.ReadUInt32();
          this.pExtendedData = er.ReadUInt32();
          this.CellData = new NCER.cellBankBlock.cellDataBank.cellData[(int) this.numCells];
          for (int index = 0; index < (int) this.numCells; ++index)
            this.CellData[index] = new NCER.cellBankBlock.cellDataBank.cellData(er, (int) this.cellBankAttr, (int) this.pCellDataArrayHead + 24 + (int) this.numCells * (this.cellBankAttr == (ushort) 1 ? 16 : 8));
        }

        public void Write(EndianBinaryWriter er)
        {
          er.WriteUInt16(this.numCells);
          er.WriteUInt16(this.cellBankAttr);
          er.WriteUInt32(24U);
          er.WriteUInt32((uint) this.mappingMode);
          er.WriteUInt32(this.pVramTransferData);
          er.WriteUInt32(this.pStringBank);
          er.WriteUInt32(this.pExtendedData);
          int Offset = 0;
          for (int index = 0; index < (int) this.numCells; ++index)
            this.CellData[index].Write(er, ref Offset);
          for (int index1 = 0; index1 < (int) this.numCells; ++index1)
          {
            for (int index2 = 0; index2 < (int) this.CellData[index1].numOAMAttrs; ++index2)
              this.CellData[index1].CellOAMAttrData[index2].Write(er);
          }
        }

        public Bitmap GetBitmap(int Index, NCGR Graphic, NCLR Palette)
        {
          int Width = 0;
          int Height = 0;
          switch (this.mappingMode)
          {
            case NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_1D_32:
              Width = 32;
              Height = Graphic.CharacterData.Data.Length * (Graphic.CharacterData.pixelFmt == bmd.Converters.Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 32;
              break;
            case NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_1D_64:
              Width = 64;
              Height = Graphic.CharacterData.Data.Length * (Graphic.CharacterData.pixelFmt == bmd.Converters.Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 64;
              break;
            case NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_1D_128:
              Width = 128;
              Height = Graphic.CharacterData.Data.Length * (Graphic.CharacterData.pixelFmt == bmd.Converters.Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 128;
              break;
            case NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_1D_256:
              Width = 256;
              Height = Graphic.CharacterData.Data.Length * (Graphic.CharacterData.pixelFmt == bmd.Converters.Graphic.GXTexFmt.GX_TEXFMT_PLTT16 ? 2 : 1) / 256;
              break;
            case NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D:
              Width = (int) Graphic.CharacterData.W * 8;
              Height = (int) Graphic.CharacterData.H * 8;
              break;
          }
          return bmd.Converters.Graphic.ConvertData(Graphic.CharacterData.Data, Palette.PaletteData.Data, Width, Height, this.CellData[Index], this.mappingMode, Graphic.CharacterData.pixelFmt, bmd.Converters.Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR);
        }

        public enum CharacterDataMapingType
        {
          NNS_G2D_CHARACTERMAPING_1D_32,
          NNS_G2D_CHARACTERMAPING_1D_64,
          NNS_G2D_CHARACTERMAPING_1D_128,
          NNS_G2D_CHARACTERMAPING_1D_256,
          NNS_G2D_CHARACTERMAPING_2D,
          NNS_G2D_CHARACTERMAPING_MAX,
        }

        public class cellData
        {
          public ushort numOAMAttrs;
          public ushort cellAttr;
          public uint pOamAttrArray;
          public NCER.cellBankBlock.cellDataBank.cellData.CellBoundingRectS16 boundingRect;
          public NCER.cellBankBlock.cellDataBank.cellData.cellOAMAttrData[] CellOAMAttrData;

          public cellData(EndianBinaryReader er, int Format, int offset)
          {
            this.numOAMAttrs = er.ReadUInt16();
            this.cellAttr = er.ReadUInt16();
            this.pOamAttrArray = er.ReadUInt32();
            if (Format == 1)
              this.boundingRect = new NCER.cellBankBlock.cellDataBank.cellData.CellBoundingRectS16(er);
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.pOamAttrArray + (long) offset;
            this.CellOAMAttrData = new NCER.cellBankBlock.cellDataBank.cellData.cellOAMAttrData[(int) this.numOAMAttrs];
            for (int index = 0; index < (int) this.numOAMAttrs; ++index)
              this.CellOAMAttrData[index] = new NCER.cellBankBlock.cellDataBank.cellData.cellOAMAttrData(er);
            er.BaseStream.Position = position;
          }

          public cellData()
          {
          }

          public void Write(EndianBinaryWriter er, ref int Offset)
          {
            er.WriteUInt16(this.numOAMAttrs);
            er.WriteUInt16(this.cellAttr);
            er.WriteUInt32((uint) Offset);
            Offset += 6 * (int) this.numOAMAttrs;
            if (this.boundingRect == null)
              return;
            this.boundingRect.Write(er);
          }

          public class CellBoundingRectS16
          {
            public short maxX;
            public short maxY;
            public short minX;
            public short minY;

            public CellBoundingRectS16(EndianBinaryReader er)
            {
              this.maxX = er.ReadInt16();
              this.maxY = er.ReadInt16();
              this.minX = er.ReadInt16();
              this.minY = er.ReadInt16();
            }

            public CellBoundingRectS16(short Width, short Height)
            {
              this.minX = (short) -((int) Width / 2);
              this.minY = (short) -((int) Height / 2);
              this.maxX = (short) ((int) Width / 2);
              this.maxY = (short) ((int) Height / 2);
            }

            public void Write(EndianBinaryWriter er)
            {
              er.WriteInt16(this.maxX);
              er.WriteInt16(this.maxY);
              er.WriteInt16(this.minX);
              er.WriteInt16(this.minY);
            }
          }

          public class cellOAMAttrData
          {
            public bool FlipY = false;
            public bool FlipX = false;
            public ushort attr0;
            public byte YCoord;
            public bool AffineTransformation;
            public bool DoubleSizeAffineTransformation;
            public byte OBJMode;
            public bool Mosaic;
            public bool ColorMode;
            public byte OBJShape;
            public ushort attr1;
            public short XCoord;
            public byte AffineTransformationParameterSelection;
            public byte OBJSize;
            public ushort attr2;
            public short StartingCharacterName;
            public byte DisplayPriority;
            public byte ColorParameter;

            public cellOAMAttrData(EndianBinaryReader er)
            {
              this.attr0 = er.ReadUInt16();
              this.attr1 = er.ReadUInt16();
              this.attr2 = er.ReadUInt16();
              this.YCoord = (byte) ((uint) this.attr0 & (uint) byte.MaxValue);
              this.AffineTransformation = ((int) this.attr0 >> 8 & 1) == 1;
              this.DoubleSizeAffineTransformation = ((int) this.attr0 >> 9 & 1) == 1;
              this.OBJMode = (byte) ((int) this.attr0 >> 10 & 3);
              this.Mosaic = ((int) this.attr0 >> 12 & 1) == 1;
              this.ColorMode = ((int) this.attr0 >> 13 & 1) == 1;
              this.OBJShape = (byte) ((int) this.attr0 >> 14 & 3);
              this.XCoord = (short) ((int) this.attr1 & 511);
              this.AffineTransformationParameterSelection = (byte) ((int) this.attr1 >> 9 & 31);
              if (!this.AffineTransformation)
              {
                this.FlipX = ((int) this.AffineTransformationParameterSelection >> 3 & 1) == 1;
                this.FlipY = ((int) this.AffineTransformationParameterSelection >> 4 & 1) == 1;
              }
              this.OBJSize = (byte) ((int) this.attr1 >> 14 & 3);
              this.StartingCharacterName = (short) ((int) this.attr2 & 1023);
              this.DisplayPriority = (byte) ((int) this.attr2 >> 10 & 3);
              this.ColorParameter = (byte) ((int) this.attr2 >> 12 & 15);
            }

            public cellOAMAttrData(
              short XCoord,
              sbyte YCoord,
              byte OBJSize,
              byte OBJShape,
              byte ColorParameter)
            {
              this.XCoord = XCoord;
              this.YCoord = (byte) YCoord;
              if (YCoord < (sbyte) 0)
                this.YCoord |= (byte) 128;
              this.OBJShape = OBJShape;
              this.OBJSize = OBJSize;
              this.ColorParameter = ColorParameter;
              this.AffineTransformation = false;
              this.DoubleSizeAffineTransformation = false;
              this.Mosaic = false;
              this.ColorMode = false;
              this.FlipX = false;
              this.FlipY = false;
              this.DisplayPriority = (byte) 1;
              this.AffineTransformationParameterSelection = (byte) 0;
              this.OBJMode = (byte) 0;
            }

            public void Write(EndianBinaryWriter er)
            {
              this.attr0 = (ushort) 0;
              this.attr0 |= (ushort) ((uint) this.OBJShape << 14);
              this.attr0 |= (ushort) ((this.ColorMode ? 1 : 0) << 13);
              this.attr0 |= (ushort) ((this.Mosaic ? 1 : 0) << 12);
              this.attr0 |= (ushort) ((uint) this.OBJMode << 10);
              this.attr0 |= (ushort) ((this.DoubleSizeAffineTransformation ? 1 : 0) << 9);
              this.attr0 |= (ushort) ((this.AffineTransformation ? 1 : 0) << 8);
              this.attr0 |= (ushort) this.YCoord;
              er.WriteUInt16(this.attr0);
              this.attr1 = (ushort) 0;
              this.attr1 |= (ushort) ((uint) this.OBJSize << 14);
              if (!this.AffineTransformation)
              {
                this.AffineTransformationParameterSelection = (byte) 0;
                this.AffineTransformationParameterSelection |= (byte) ((this.FlipY ? 1 : 0) << 4);
                this.AffineTransformationParameterSelection |= (byte) ((this.FlipX ? 1 : 0) << 3);
              }
              this.attr1 |= (ushort) ((uint) this.AffineTransformationParameterSelection << 9);
              this.attr1 |= (ushort) ((uint) this.XCoord & 511U);
              er.WriteUInt16(this.attr1);
              this.attr2 = (ushort) 0;
              this.attr2 |= (ushort) ((uint) this.ColorParameter << 12);
              this.attr2 |= (ushort) ((uint) this.DisplayPriority << 10);
              this.attr2 |= (ushort) this.StartingCharacterName;
              er.WriteUInt16(this.attr2);
            }

            public Size GetSize()
            {
              switch (this.OBJShape)
              {
                case 0:
                  switch (this.OBJSize)
                  {
                    case 0:
                      return new Size(8, 8);
                    case 1:
                      return new Size(16, 16);
                    case 2:
                      return new Size(32, 32);
                    case 3:
                      return new Size(64, 64);
                  }
                  break;
                case 1:
                  switch (this.OBJSize)
                  {
                    case 0:
                      return new Size(16, 8);
                    case 1:
                      return new Size(32, 8);
                    case 2:
                      return new Size(32, 16);
                    case 3:
                      return new Size(64, 32);
                  }
                  break;
                case 2:
                  switch (this.OBJSize)
                  {
                    case 0:
                      return new Size(8, 16);
                    case 1:
                      return new Size(8, 32);
                    case 2:
                      return new Size(16, 32);
                    case 3:
                      return new Size(32, 64);
                  }
                  break;
                case 3:
                  throw new NotSupportedException("Prohibited Cell setting");
              }
              return new Size(0, 0);
            }
          }
        }
      }
    }

    public class labelBlock
    {
      public const string Signature = "LBAL";
      public DataBlockHeader Header;
      public uint[] Offsets;
      public string[] Names;

      public labelBlock(EndianBinaryReader er, int NrCells, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "LBAL", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Offsets = new uint[NrCells];
          int length = NrCells;
          for (int newSize = 0; newSize < NrCells; ++newSize)
          {
            this.Offsets[newSize] = er.ReadUInt32();
            if (this.Offsets[newSize] > this.Header.size)
            {
              length = newSize;
              Array.Resize<uint>(ref this.Offsets, newSize);
              er.BaseStream.Position -= 4L;
              break;
            }
          }
          this.Names = new string[length];
          long position = er.BaseStream.Position;
          for (int index = 0; index < length; ++index)
          {
            er.BaseStream.Position = position + (long) this.Offsets[index];
            this.Names[index] = er.ReadStringNT();
          }
          OK = true;
        }
      }
    }
  }
}
