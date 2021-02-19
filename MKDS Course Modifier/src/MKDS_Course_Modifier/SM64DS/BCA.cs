// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.SM64DS.BCA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;
using System.IO;

namespace MKDS_Course_Modifier.SM64DS
{
  public class BCA
  {
    public BCA.BCAHeader Header;
    public BCA.BCAAnimation[] Animations;

    public BCA(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.Header = new BCA.BCAHeader(er);
      er.BaseStream.Position = (long) this.Header.OffsetAnimationSection;
      this.Animations = new BCA.BCAAnimation[(int) this.Header.NrBones];
      for (int index = 0; index < (int) this.Header.NrBones; ++index)
        this.Animations[index] = new BCA.BCAAnimation(er, this.Header);
      er.Close();
    }

    public Matrix4[] GetMatrices(BMD.BMDBone[] Bones, int FrameNr, int Posscale)
    {
      Matrix4[] Matrices = new Matrix4[Bones.Length];
      for (int Idx = 0; Idx < Bones.Length; ++Idx)
        Matrices[Idx] = this.Animations[Idx].GetMatrix(FrameNr, Posscale, Matrices, Bones[Idx], Idx);
      return Matrices;
    }

    public class BCAHeader
    {
      public ushort NrBones;
      public ushort NrFrames;
      public uint Unknown;
      public uint OffsetScale;
      public uint OffsetRotation;
      public uint OffsetTranslation;
      public uint OffsetAnimationSection;

      public BCAHeader(EndianBinaryReader er)
      {
        this.NrBones = er.ReadUInt16();
        this.NrFrames = er.ReadUInt16();
        this.Unknown = er.ReadUInt32();
        this.OffsetScale = er.ReadUInt32();
        this.OffsetRotation = er.ReadUInt32();
        this.OffsetTranslation = er.ReadUInt32();
        this.OffsetAnimationSection = er.ReadUInt32();
      }
    }

    public class BCAAnimation
    {
      public BCA.BCAAnimation.AnimationInfo Sx;
      public BCA.BCAAnimation.AnimationInfo Sy;
      public BCA.BCAAnimation.AnimationInfo Sz;
      public BCA.BCAAnimation.AnimationInfo Rx;
      public BCA.BCAAnimation.AnimationInfo Ry;
      public BCA.BCAAnimation.AnimationInfo Rz;
      public BCA.BCAAnimation.AnimationInfo Tx;
      public BCA.BCAAnimation.AnimationInfo Ty;
      public BCA.BCAAnimation.AnimationInfo Tz;

      public BCAAnimation(EndianBinaryReader er, BCA.BCAHeader Header)
      {
        this.Sx = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetScale, (uint) Header.NrFrames, false);
        this.Sy = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetScale, (uint) Header.NrFrames, false);
        this.Sz = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetScale, (uint) Header.NrFrames, false);
        this.Rx = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetRotation, (uint) Header.NrFrames, true);
        this.Ry = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetRotation, (uint) Header.NrFrames, true);
        this.Rz = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetRotation, (uint) Header.NrFrames, true);
        this.Tx = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetTranslation, (uint) Header.NrFrames, false);
        this.Ty = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetTranslation, (uint) Header.NrFrames, false);
        this.Tz = new BCA.BCAAnimation.AnimationInfo(er, Header.OffsetTranslation, (uint) Header.NrFrames, false);
      }

      public Matrix4 GetMatrix(
        int FrameNr,
        int Posscale,
        Matrix4[] Matrices,
        BMD.BMDBone Bone,
        int Idx)
      {
        Matrix4 right = Matrix4.Scale(this.Sx.GetValue(FrameNr, Bone.Sx), this.Sy.GetValue(FrameNr, Bone.Sy), this.Sz.GetValue(FrameNr, Bone.Sz));
        Matrix4 rotationX = Matrix4.CreateRotationX(this.Rx.GetValue(FrameNr, (float) ((double) Bone.Rx * 3.14159274101257 / 2048.0)));
        Matrix4 rotationY = Matrix4.CreateRotationY(this.Ry.GetValue(FrameNr, (float) ((double) Bone.Ry * 3.14159274101257 / 2048.0)));
        Matrix4 rotationZ = Matrix4.CreateRotationZ(this.Rz.GetValue(FrameNr, (float) ((double) Bone.Rz * 3.14159274101257 / 2048.0)));
        Matrix4 translation = Matrix4.CreateTranslation(this.Tx.GetValue(FrameNr, Bone.Tx) / (float) Posscale, this.Ty.GetValue(FrameNr, Bone.Ty) / (float) Posscale, this.Tz.GetValue(FrameNr, Bone.Tz) / (float) Posscale);
        Matrix4 result = Matrix4.Identity;
        Matrix4.Mult(ref result, ref right, out result);
        Matrix4.Mult(ref result, ref rotationX, out result);
        Matrix4.Mult(ref result, ref rotationY, out result);
        Matrix4.Mult(ref result, ref rotationZ, out result);
        Matrix4.Mult(ref result, ref translation, out result);
        if (Bone.OffsetParent < (short) 0)
          Matrix4.Mult(ref result, ref Matrices[Idx + (int) Bone.OffsetParent], out result);
        return result;
      }

      public class AnimationInfo
      {
        public byte Interpolate;
        public bool ConstantValue;
        public byte StartOffset;
        public byte DD;
        public float[] Values;

        public AnimationInfo(EndianBinaryReader er, uint Offset, uint NrFrames, bool Rotation)
        {
          this.Interpolate = er.ReadByte();
          this.ConstantValue = er.ReadByte() != (byte) 1;
          this.StartOffset = er.ReadByte();
          this.DD = er.ReadByte();
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) Offset + (long) ((int) this.StartOffset * (Rotation ? 2 : 4));
          this.Values = !this.ConstantValue ? (Rotation ? Array.ConvertAll<short, float>(er.ReadInt16s((int) NrFrames / (this.Interpolate == (byte) 1 ? 2 : 1) + (int) this.Interpolate), new Converter<short, float>(this.ShortToFloat)) : er.ReadSingleInt32Exp12s((int) NrFrames / (this.Interpolate == (byte) 1 ? 2 : 1) + (int) this.Interpolate)) : (Rotation ? Array.ConvertAll<short, float>(er.ReadInt16s(1), new Converter<short, float>(this.ShortToFloat)) : er.ReadSingleInt32Exp12s(1));
          er.BaseStream.Position = position;
        }

        private float ShortToFloat(short i)
        {
          return (float) ((double) i * 3.14159274101257 / 2048.0);
        }

        public float GetValue(int FrameNr, float BoneValue)
        {
          if (this.ConstantValue)
            return this.Values[0];
          if (this.Interpolate != (byte) 1)
            return this.Values[FrameNr];
          return (FrameNr & 1) != 0 && (FrameNr >> 1) + 1 <= this.Values.Length - 1 ? (float) (((double) this.Values[FrameNr >> 1] + (double) this.Values[(FrameNr >> 1) + 1]) / 2.0) : this.Values[FrameNr >> 1];
        }
      }
    }
  }
}
