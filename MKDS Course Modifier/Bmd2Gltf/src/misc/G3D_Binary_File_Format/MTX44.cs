// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.MTX44
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class MTX44
  {
    private float[] _array = new float[16];

    public float this[int x, int y]
    {
      get
      {
        return this._array[x + y * 4];
      }
      set
      {
        this._array[x + y * 4] = value;
      }
    }

    public void SetValues(float[] array)
    {
      this._array = array;
    }

    public float this[int index]
    {
      get
      {
        return this._array[index];
      }
      set
      {
        this._array[index] = value;
      }
    }

    public float[] Floats
    {
      get
      {
        return this._array;
      }
    }

    public MTX44 Clone()
    {
      MTX44 mtX44 = new MTX44();
      for (int index = 0; index < 16; ++index)
        mtX44._array[index] = this._array[index];
      return mtX44;
    }

    public void translate(float x, float y, float z)
    {
      MTX44 b = new MTX44();
      b.LoadIdentity();
      b[12] = x;
      b[13] = y;
      b[14] = z;
      this.MultMatrix(b).CopyValuesTo(this);
    }

    public void rotate(float x, float y, float z)
    {
      MTX44 b = new MTX44();
      b.LoadIdentity();
      float num1 = (float) Math.Cos((double) x);
      float num2 = (float) Math.Sin((double) x);
      float num3 = (float) Math.Cos((double) y);
      float num4 = (float) Math.Sin((double) y);
      float num5 = (float) Math.Cos((double) z);
      float num6 = (float) Math.Sin((double) z);
      b[0] = num3 * num5;
      b[1] = num3 * num6;
      b[2] = -num4;
      b[4] = (float) ((double) num5 * (double) num2 * (double) num4 - (double) num6 * (double) num1);
      b[5] = (float) ((double) num2 * (double) num4 + (double) num1 * (double) num5);
      b[6] = num2 * num3;
      b[8] = (float) ((double) num1 * (double) num5 * (double) num4 + (double) num2 * (double) num6);
      b[9] = (float) ((double) num1 * (double) num4 * (double) num6 - (double) num2 * (double) num5);
      b[10] = num1 * num3;
      this.MultMatrix(b).CopyValuesTo(this);
    }

    public void LoadIdentity()
    {
      this.Zero();
      this[0, 0] = this[1, 1] = this[2, 2] = this[3, 3] = 1f;
    }

    public MTX44 MultMatrix(MTX44 b)
    {
      MTX44 mtX44_1 = new MTX44();
      MTX44 mtX44_2 = this;
      for (int index1 = 0; index1 < 4; ++index1)
      {
        for (int index2 = 0; index2 < 4; ++index2)
        {
          mtX44_1._array[(index1 << 2) + index2] = 0.0f;
          for (int index3 = 0; index3 < 4; ++index3)
            mtX44_1._array[(index1 << 2) + index2] += mtX44_2._array[(index3 << 2) + index2] * b._array[(index1 << 2) + index3];
        }
      }
      return mtX44_1;
    }

    public static MTX44 mtx_Rotate(int pivot, int neg, float a, float b)
    {
      float[] numArray = new float[16];
      numArray[15] = 1f;
      float num1 = 1f;
      float num2 = a;
      float num3 = b;
      switch (neg)
      {
        case 1:
        case 3:
        case 5:
        case 7:
        case 9:
        case 11:
        case 13:
        case 15:
          num1 = -1f;
          break;
      }
      switch (neg - 2)
      {
        case 0:
        case 1:
        case 4:
        case 5:
        case 8:
        case 9:
        case 12:
        case 13:
          num3 = -num3;
          break;
      }
      switch (neg - 4)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 8:
        case 9:
        case 10:
        case 11:
          num2 = -num2;
          break;
      }
      switch (pivot)
      {
        case 0:
          numArray[0] = num1;
          numArray[5] = a;
          numArray[6] = b;
          numArray[9] = num3;
          numArray[10] = num2;
          break;
        case 1:
          numArray[1] = num1;
          numArray[4] = a;
          numArray[6] = b;
          numArray[8] = num3;
          numArray[10] = num2;
          break;
        case 2:
          numArray[2] = num1;
          numArray[4] = a;
          numArray[5] = b;
          numArray[8] = num3;
          numArray[9] = num2;
          break;
        case 3:
          numArray[4] = num1;
          numArray[1] = a;
          numArray[2] = b;
          numArray[9] = num3;
          numArray[10] = num2;
          break;
        case 4:
          numArray[5] = num1;
          numArray[0] = a;
          numArray[2] = b;
          numArray[8] = num3;
          numArray[10] = num2;
          break;
        case 5:
          numArray[6] = num1;
          numArray[0] = a;
          numArray[1] = b;
          numArray[8] = num3;
          numArray[9] = num2;
          break;
        case 6:
          numArray[8] = num1;
          numArray[1] = a;
          numArray[2] = b;
          numArray[5] = num3;
          numArray[6] = num2;
          break;
        case 7:
          numArray[9] = num1;
          numArray[0] = a;
          numArray[2] = b;
          numArray[4] = num3;
          numArray[6] = num2;
          break;
        case 8:
          numArray[10] = num1;
          numArray[0] = a;
          numArray[1] = b;
          numArray[4] = num3;
          numArray[5] = num2;
          break;
        case 9:
          numArray[0] = -a;
          break;
      }
      return new MTX44() { _array = numArray };
    }

    public Vector3 MultVector(Vector3 v)
    {
      float[] numArray = this.MultVector(new float[3]
      {
        v.X,
        v.Y,
        v.Z
      });
      return new Vector3(numArray[0], numArray[1], numArray[2]);
    }

    public float[] MultVector(float[] v)
    {
      float[] numArray = new float[3];
      for (int index = 0; index < 3; ++index)
      {
        float num1 = v[0] * this[index];
        float num2 = v[1] * this[4 + index];
        float num3 = v[2] * this[8 + index];
        float num4 = this[12 + index];
        numArray[index] = num1 + num2 + num3 + num4;
      }
      return numArray;
    }

    public static MTX44 operator +(MTX44 a, MTX44 b)
    {
      a[0] += b[0];
      a[1] += b[1];
      a[2] += b[2];
      a[3] += b[3];
      a[4] += b[4];
      a[5] += b[5];
      a[6] += b[6];
      a[7] += b[7];
      a[8] += b[8];
      a[9] += b[9];
      a[10] += b[10];
      a[11] += b[11];
      a[12] += b[12];
      a[13] += b[13];
      a[14] += b[14];
      a[15] += b[15];
      return a;
    }

    public static MTX44 operator -(MTX44 a, MTX44 b)
    {
      a[0] -= b[0];
      a[1] -= b[1];
      a[2] -= b[2];
      a[3] -= b[3];
      a[4] -= b[4];
      a[5] -= b[5];
      a[6] -= b[6];
      a[7] -= b[7];
      a[8] -= b[8];
      a[9] -= b[9];
      a[10] -= b[10];
      a[11] -= b[11];
      a[12] -= b[12];
      a[13] -= b[13];
      a[14] -= b[14];
      a[15] -= b[15];
      return a;
    }

    public static MTX44 operator *(MTX44 a, float b)
    {
      a[0] *= b;
      a[1] *= b;
      a[2] *= b;
      a[3] *= b;
      a[4] *= b;
      a[5] *= b;
      a[6] *= b;
      a[7] *= b;
      a[8] *= b;
      a[9] *= b;
      a[10] *= b;
      a[11] *= b;
      a[12] *= b;
      a[13] *= b;
      a[14] *= b;
      a[15] *= b;
      return a;
    }

    public static MTX44 operator *(float b, MTX44 a)
    {
      a[0] *= b;
      a[1] *= b;
      a[2] *= b;
      a[3] *= b;
      a[4] *= b;
      a[5] *= b;
      a[6] *= b;
      a[7] *= b;
      a[8] *= b;
      a[9] *= b;
      a[10] *= b;
      a[11] *= b;
      a[12] *= b;
      a[13] *= b;
      a[14] *= b;
      a[15] *= b;
      return a;
    }

    public static MTX44 operator /(float b, MTX44 a)
    {
      a[0] = b / a[0];
      a[1] = b / a[1];
      a[2] = b / a[2];
      a[3] = b / a[3];
      a[4] = b / a[4];
      a[5] = b / a[5];
      a[6] = b / a[6];
      a[7] = b / a[7];
      a[8] = b / a[8];
      a[9] = b / a[9];
      a[10] = b / a[10];
      a[11] = b / a[11];
      a[12] = b / a[12];
      a[13] = b / a[13];
      a[14] = b / a[14];
      a[15] = b / a[15];
      return a;
    }

    public MTX44()
    {
      this.LoadIdentity();
    }

    public void Scale(float x, float y, float z)
    {
      MTX44 b = new MTX44();
      b.LoadIdentity();
      b[0] = x;
      b[5] = y;
      b[10] = z;
      this.MultMatrix(b).CopyValuesTo(this);
    }

    public void Zero()
    {
      for (int index = 0; index < 16; ++index)
        this._array[index] = 0.0f;
      this._array = new float[16];
    }

    public void CopyValuesTo(MTX44 m)
    {
      for (int index = 0; index < 16; ++index)
        m._array[index] = this[index];
    }

    public static implicit operator float[](MTX44 a)
    {
      return a.Floats;
    }

    public static implicit operator MTX44(float[] a)
    {
      return new MTX44() { _array = a };
    }
  }
}
