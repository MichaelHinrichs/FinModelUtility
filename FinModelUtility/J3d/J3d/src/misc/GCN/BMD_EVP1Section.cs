using System;

using fin.schema.matrix;

using j3d.G3D_Binary_File_Format;

using schema.binary;

#pragma warning disable CS8604

namespace j3d.GCN {
  public partial class BMD {
    public class EVP1Section {
      public const string Signature = "EVP1";
      public DataBlockHeader Header;
      public ushort Count;
      public ushort Padding;
      public uint[] Offsets;
      public byte[] Counts;
      public Matrix3x4f[] InverseBindMatrices { get; set; }
      public MultiMatrix[] WeightedIndices;

      public EVP1Section(IEndianBinaryReader er, out bool OK) {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "EVP1", out OK1);
        if (!OK1) {
          OK = false;
        } else {
          this.Count = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.Offsets = er.ReadUInt32s(4);
          long position2 = er.Position;
          er.Position = position1 + (long) this.Offsets[0];
          this.Counts = er.ReadBytes((int) this.Count);

          er.Position = position1 + (long) this.Offsets[1];
          this.WeightedIndices = new MultiMatrix[(int) this.Count];
          int val1 = 0;
          for (int index1 = 0; index1 < (int) this.Count; ++index1) {
            this.WeightedIndices[index1] = new MultiMatrix();
            this.WeightedIndices[index1].Indices =
                new ushort[(int) this.Counts[index1]];
            for (int index2 = 0; index2 < (int) this.Counts[index1]; ++index2) {
              this.WeightedIndices[index1].Indices[index2] = er.ReadUInt16();
              val1 = Math.Max(val1,
                              (int) this.WeightedIndices[index1]
                                        .Indices[index2] + 1);
            }
          }

          er.Position = position1 + (long) this.Offsets[2];
          for (int index1 = 0; index1 < (int) this.Count; ++index1) {
            this.WeightedIndices[index1].Weights =
                new float[(int) this.Counts[index1]];
            for (int index2 = 0; index2 < (int) this.Counts[index1]; ++index2)
              this.WeightedIndices[index1].Weights[index2] = er.ReadSingle();
          }

          er.Position = position1 + (long) this.Offsets[3];
          this.InverseBindMatrices = new Matrix3x4f[val1];
          for (int index = 0; index < val1; ++index) {
            this.InverseBindMatrices[index] = er.ReadNew<Matrix3x4f>();
          }

          er.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      public class MultiMatrix {
        public float[] Weights;
        public ushort[] Indices;
      }
    }
  }
}