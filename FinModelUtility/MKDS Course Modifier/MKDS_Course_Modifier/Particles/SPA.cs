// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Particles.SPA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using OpenTK;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Particles
{
  public class SPA
  {
    public const string Signature = " APS";
    public string Type;
    public string Version;
    public ushort NrParticles;
    public ushort NrParticleTextures;
    public uint Unknown3;
    public uint Unknown4;
    public uint Unknown5;
    public uint FirstParticleTextureOffset;
    public uint Padding;
    public SPA.Particle[] Particles;
    public SPA.ParticleTexture[] ParticleTextures;

    public SPA(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.Type = er.ReadString(Encoding.ASCII, 4);
      if (this.Type != " APS")
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Version = er.ReadString(Encoding.ASCII, 4);
        this.NrParticles = er.ReadUInt16();
        this.NrParticleTextures = er.ReadUInt16();
        this.Unknown3 = er.ReadUInt32();
        this.Unknown4 = er.ReadUInt32();
        this.Unknown5 = er.ReadUInt32();
        this.FirstParticleTextureOffset = er.ReadUInt32();
        this.Padding = er.ReadUInt32();
        if (this.Version == "12_1")
        {
          this.Particles = new SPA.Particle[(int) this.NrParticles];
          for (int index = 0; index < (int) this.NrParticles; ++index)
            this.Particles[index] = new SPA.Particle(er);
        }
        er.BaseStream.Position = (long) this.FirstParticleTextureOffset;
        this.ParticleTextures = new SPA.ParticleTexture[(int) this.NrParticleTextures];
        for (int index = 0; index < (int) this.NrParticleTextures; ++index)
        {
          bool OK;
          this.ParticleTextures[index] = new SPA.ParticleTexture(er, out OK);
          if (this.Type != " APS")
          {
            int num2 = (int) MessageBox.Show("Error " + (object) (index + 2));
            break;
          }
        }
      }
      er.Close();
    }

    public class Particle
    {
      public SPA.Particle.ParticleFlags Flag;
      public Vector3 Position;
      public float Unknown1;
      public float Unknown2;
      public float Unknown3;
      public ushort Unknown4;
      public ushort Unknown5;
      public ushort Unknown6;
      public ushort Unknown7;
      public uint Unknown8;
      public uint Unknown9;
      public uint Unknown10;
      public ushort Unknown11;
      public byte[] Unknown12;
      public ushort Unknown13;
      public ushort Unknown14;
      public ushort Unknown15;
      public ushort Unknown16;
      public byte[] Unknown17;
      public byte Unknown18;
      public byte Unknown19;
      public byte Unknown20;
      public byte TextureId;
      public uint Unknown21;
      public uint Unknown22;
      public ushort Unknown23;
      public ushort Unknown24;
      public uint Unknown25;
      public byte[] Bit8;
      public SPA.Particle.Bit_9 Bit9;
      public byte[] Bit10;
      public SPA.Particle.TextureAnimation TexAnim;
      public byte[] Bit16;
      public byte[] Bit24;
      public byte[] Bit25;
      public byte[] Bit26;
      public byte[] Bit27;
      public byte[] Bit28;
      public byte[] Bit29;

      public Particle(EndianBinaryReader er)
      {
        this.Flag = (SPA.Particle.ParticleFlags) er.ReadUInt32();
        this.Position = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
        this.Unknown1 = er.ReadSingleInt32Exp12();
        this.Unknown2 = er.ReadSingleInt32Exp12();
        this.Unknown3 = er.ReadSingleInt32Exp12();
        this.Unknown4 = er.ReadUInt16();
        this.Unknown5 = er.ReadUInt16();
        this.Unknown6 = er.ReadUInt16();
        this.Unknown7 = er.ReadUInt16();
        this.Unknown8 = er.ReadUInt32();
        this.Unknown9 = er.ReadUInt32();
        this.Unknown10 = er.ReadUInt32();
        this.Unknown11 = er.ReadUInt16();
        this.Unknown12 = er.ReadBytes(6);
        this.Unknown13 = er.ReadUInt16();
        this.Unknown14 = er.ReadUInt16();
        this.Unknown15 = er.ReadUInt16();
        this.Unknown16 = er.ReadUInt16();
        this.Unknown17 = er.ReadBytes(4);
        this.Unknown18 = er.ReadByte();
        this.Unknown19 = er.ReadByte();
        this.Unknown20 = er.ReadByte();
        this.TextureId = er.ReadByte();
        this.Unknown21 = er.ReadUInt32();
        this.Unknown22 = er.ReadUInt32();
        this.Unknown23 = er.ReadUInt16();
        this.Unknown24 = er.ReadUInt16();
        this.Unknown25 = er.ReadUInt32();
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit8) != SPA.Particle.ParticleFlags.Type0)
          this.Bit8 = er.ReadBytes(12);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit9) != SPA.Particle.ParticleFlags.Type0)
          this.Bit9 = new SPA.Particle.Bit_9(er);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit10) != SPA.Particle.ParticleFlags.Type0)
          this.Bit10 = er.ReadBytes(8);
        if ((this.Flag & SPA.Particle.ParticleFlags.TextureAnimation) != SPA.Particle.ParticleFlags.Type0)
          this.TexAnim = new SPA.Particle.TextureAnimation(er);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit16) != SPA.Particle.ParticleFlags.Type0)
          this.Bit16 = er.ReadBytes(20);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit24) != SPA.Particle.ParticleFlags.Type0)
          this.Bit24 = er.ReadBytes(8);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit25) != SPA.Particle.ParticleFlags.Type0)
          this.Bit25 = er.ReadBytes(8);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit26) != SPA.Particle.ParticleFlags.Type0)
          this.Bit26 = er.ReadBytes(16);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit27) != SPA.Particle.ParticleFlags.Type0)
          this.Bit27 = er.ReadBytes(4);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit28) != SPA.Particle.ParticleFlags.Type0)
          this.Bit28 = er.ReadBytes(8);
        if ((this.Flag & SPA.Particle.ParticleFlags.Bit29) == SPA.Particle.ParticleFlags.Type0)
          return;
        this.Bit29 = er.ReadBytes(16);
      }

      [Flags]
      public enum ParticleFlags : uint
      {
        Type0 = 0,
        Type1 = 16, // 0x00000010
        Type2 = 32, // 0x00000020
        Type3 = Type2 | Type1, // 0x00000030
        Bit8 = 256, // 0x00000100
        Bit9 = 512, // 0x00000200
        Bit10 = 1024, // 0x00000400
        TextureAnimation = 2048, // 0x00000800
        Bit16 = 65536, // 0x00010000
        Bit21 = 2097152, // 0x00200000
        Bit22 = 4194304, // 0x00400000
        Bit23 = 8388608, // 0x00800000
        Bit24 = 16777216, // 0x01000000
        Bit25 = 33554432, // 0x02000000
        Bit26 = 67108864, // 0x04000000
        Bit27 = 134217728, // 0x08000000
        Bit28 = 268435456, // 0x10000000
        Bit29 = 536870912, // 0x20000000
      }

      public class Bit_9
      {
        public ushort Unknown1;
        public ushort Unknown2;
        public ushort Unknown3;
        public ushort Unknown4;
        public uint Unknown5;

        public Bit_9(EndianBinaryReader er)
        {
          this.Unknown1 = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.Unknown3 = er.ReadUInt16();
          this.Unknown4 = er.ReadUInt16();
          this.Unknown5 = er.ReadUInt32();
        }
      }

      public class TextureAnimation
      {
        public byte[] Textures;
        public byte NrFrames;
        public byte Unknown1;
        public ushort Unknown2;

        public TextureAnimation(EndianBinaryReader er)
        {
          this.Textures = er.ReadBytes(8);
          this.NrFrames = er.ReadByte();
          this.Unknown1 = er.ReadByte();
          this.Unknown2 = er.ReadUInt16();
        }
      }
    }

    public class ParticleTexture
    {
      public const string Signature = " TPS";
      public string Type;
      public ushort TextureInfo;
      public ushort Width;
      public ushort Height;
      public Graphic.GXTexFmt TextureFormat;
      public bool RepeatS;
      public bool RepeatT;
      public bool FlipS;
      public bool FlipT;
      public ushort Unknown1;
      public uint TextureDataLength;
      public uint PaletteOffset;
      public uint PaletteDataLength;
      public uint Unknown2;
      public uint Unknown3;
      public uint Unknown4;
      public byte[] ImageData;
      public byte[] PaletteData;

      public ParticleTexture(EndianBinaryReader er, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 4);
        if (this.Type != " TPS")
        {
          OK = false;
        }
        else
        {
          this.TextureInfo = er.ReadUInt16();
          this.Width = (ushort) (8 << ((int) this.TextureInfo >> 4 & 15));
          this.Height = (ushort) (8 << ((int) this.TextureInfo >> 8 & 15));
          this.TextureFormat = (Graphic.GXTexFmt) ((int) this.TextureInfo & 7);
          this.RepeatS = ((int) this.TextureInfo >> 12 & 1) == 1;
          this.RepeatT = ((int) this.TextureInfo >> 13 & 1) == 1;
          this.FlipS = ((int) this.TextureInfo >> 14 & 1) == 1;
          this.FlipT = ((int) this.TextureInfo >> 15 & 1) == 1;
          this.Unknown1 = er.ReadUInt16();
          this.TextureDataLength = er.ReadUInt32();
          this.PaletteOffset = er.ReadUInt32();
          this.PaletteDataLength = er.ReadUInt32();
          this.Unknown2 = er.ReadUInt32();
          this.Unknown3 = er.ReadUInt32();
          this.Unknown4 = er.ReadUInt32();
          this.ImageData = er.ReadBytes((int) this.TextureDataLength);
          this.PaletteData = er.ReadBytes((int) this.PaletteDataLength);
          OK = true;
        }
      }

      public Bitmap ToBitmap()
      {
        return Graphic.ConvertData(this.ImageData, this.PaletteData, (byte[]) null, 0, (int) this.Width, (int) this.Height, this.TextureFormat, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, Convert.ToBoolean(this.Unknown1), true);
      }
    }
  }
}
