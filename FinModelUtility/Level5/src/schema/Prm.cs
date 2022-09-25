using OpenTK;

namespace level5.schema {
  public class Prm {
    public string Name { get; private set; }

    public string MaterialName { get; private set; }

    private float[] nodeTable_;

    public List<uint> Triangles { get; set; }
    public List<GenericVertex> Vertices { get; set; }

    public Prm(byte[] data) {
      using (var r = new EndianBinaryReader(new System.IO.MemoryStream(data), Endianness.LittleEndian)) {
        Open(r);
      }
    }

    public void Open(EndianBinaryReader r) {
      r.Position = 4;
      var prmOffset = r.ReadUInt32();

      r.Position = prmOffset + 4;

      // buffers-------------------------------------------

      uint pvbOffset = r.ReadUInt32();
      int pvbSize = r.ReadInt32();
      uint pviOffset = r.ReadUInt32();
      int pviSize = r.ReadInt32();

      var polygonVertexBuffer = r.ReadBytesAtOffset(pvbOffset + prmOffset, pvbSize);
      var polygonVertexIndexBuffer = r.ReadBytesAtOffset(pviOffset + prmOffset, pviSize);

      // node table-------------------------------------------

      r.Position = 0x28;
      uint noOffset = r.ReadUInt32();
      int noSize = r.ReadInt32() / 4 + 1;

      this.nodeTable_ = new float[noSize];
      r.Position = noOffset;
      for (int i = 0; i < noSize; i++) {
        this.nodeTable_[i] = r.ReadSingle();
      }

      // name and material-------------------------------------------
      r.Position = 0x30;
      string name = r.ReadStringAtOffset(r.ReadUInt32(), r.ReadInt32());
      MaterialName = r.ReadStringAtOffset(r.ReadUInt32(), r.ReadInt32());
      Name = name;

      Triangles = this.ParseIndexBuffer_(polygonVertexIndexBuffer);
      Vertices = this.ParseBuffer_(polygonVertexBuffer);
    }


    private List<uint> ParseIndexBuffer_(byte[] buffer) {
      List<uint> indices = new List<uint>();
      int primitiveType = 0;
      int faceCount = 0;

      var endianness = Endianness.LittleEndian;

      using (var r = new EndianBinaryReader(new MemoryStream(buffer), endianness)) {
        r.Position = 0x04;
        primitiveType = r.ReadInt16();
        uint faceOffset = r.ReadUInt16();
        faceCount = r.ReadInt32();

        buffer = Decompress.Level5Decom(r.ReadBytesAtOffset(faceOffset, (int)(r.Length - faceOffset)));
      }

      if (primitiveType != 2 && primitiveType != 0)
        throw new NotSupportedException("Primitve Type no implemented");

      if (primitiveType == 0)
        using (var r = new EndianBinaryReader(new MemoryStream(buffer), endianness)) {
          r.Position = 0;
          for (int i = 0; i < faceCount / 2; i++)
            indices.Add(r.ReadUInt16());
        }
      if (primitiveType == 2)
        using (var r = new EndianBinaryReader(new System.IO.MemoryStream(buffer), endianness)) {
          //Console.WriteLine(PrimitiveType + " " + FaceCount + " " + r.BaseStream.Length / 2);
          r.Position = 0;
          int f1 = r.ReadInt16();
          int f2 = r.ReadInt16();
          int f3;
          int dir = -1;
          int startdir = -1;
          for (int i = 0; i < faceCount - 2; i++) {
            if (r.Position + 2 > r.Length)
              break;
            //if (r.Position + 2 > r.Length)
            //    break;
            f3 = r.ReadInt16();
            if (f3 == 0xFFFF || f3 == -1) {
              f1 = r.ReadInt16();
              f2 = r.ReadInt16();
              dir = startdir;
            } else {
              dir *= -1;
              if (f1 != f2 && f2 != f3 && f3 != f1) {
                /*if (f1 > vCount || f2 > vCount || f3 > vCount)
                {
                    f1 = 0;
                }*/
                if (dir > 0) {
                  indices.Add((uint)f1);
                  indices.Add((uint)f2);
                  indices.Add((uint)f3);
                } else {
                  indices.Add((uint)f1);
                  indices.Add((uint)f3);
                  indices.Add((uint)f2);
                }
              }
              f1 = f2;
              f2 = f3;
            }
          }
        }

      return indices;
    }

    private List<GenericVertex> ParseBuffer_(byte[] buffer) {
      List<GenericVertex> vertices = new List<GenericVertex>();
      byte[] attributeBuffer = new byte[0];
      int stride = 0;
      int vertexCount = 0;

      var endianness = Endianness.LittleEndian;

      using (var r = new EndianBinaryReader(new System.IO.MemoryStream(buffer), endianness)) {
        r.Position = 0x4;
        uint attOffset = r.ReadUInt16();
        int attSomething = r.ReadInt16();
        uint verOffset = r.ReadUInt16();
        stride = r.ReadInt16();
        vertexCount = r.ReadInt32();

        attributeBuffer = Decompress.Level5Decom(r.ReadBytesAtOffset(attOffset, attSomething));
        buffer = Decompress.Level5Decom(r.ReadBytesAtOffset(verOffset, (int)(r.Length - verOffset)));
      }

      int[] aCount = new int[10];
      int[] aOffset = new int[10];
      int[] aSize = new int[10];
      int[] aType = new int[10];
      using (var r = new EndianBinaryReader(new System.IO.MemoryStream(attributeBuffer), endianness)) {
        for (int i = 0; i < 10; i++) {
          aCount[i] = r.ReadByte();
          aOffset[i] = r.ReadByte();
          aSize[i] = r.ReadByte();
          aType[i] = r.ReadByte();

          if (aCount[i] > 0 && i != 0 && i != 1 && i != 2 && i != 4 && i != 7 && i != 8 && i != 9) {
            Console.WriteLine(i + " " + aCount[i] + " " + aOffset[i] + " " + aSize[i] + " " + aType[i]);
          }
        }
      }

      using (var r = new EndianBinaryReader(new System.IO.MemoryStream(buffer), endianness)) {
        for (int i = 0; i < vertexCount; i++) {
          GenericVertex vert = new GenericVertex();
          vert.Clr = new Vector4(1, 1, 1, 1);
          for (int j = 0; j < 10; j++) {
            r.Position = (uint)(i * stride + aOffset[j]);
            switch (j) {
              case 0: //Position
                vert.Pos = ReadAttribute(r, aType[j], aCount[j]).Xyz;
                break;
              case 1: //Tangent
                break;
              case 2: //Normal
                vert.Nrm = ReadAttribute(r, aType[j], aCount[j]).Xyz;
                break;
              case 4: //Uv0
                vert.Uv0 = ReadAttribute(r, aType[j], aCount[j]).Xy;
                break;
              case 7: //Bone Weight
                vert.Weights = ReadAttribute(r, aType[j], aCount[j]);
                break;
              case 8: //Bone Index
                Vector4 vn = ReadAttribute(r, aType[j], aCount[j]);
                if (this.nodeTable_.Length > 0 && this.nodeTable_.Length != 1)
                  vert.Bones = new Vector4(this.nodeTable_[(int)vn.X], this.nodeTable_[(int)vn.Y], this.nodeTable_[(int)vn.Z], this.nodeTable_[(int)vn.W]);
                break;
              case 9: // Color
                vert.Clr = ReadAttribute(r, aType[j], aCount[j]).Yzwx;
                break;
            }
          }
          vertices.Add(vert);
        }
      }

      //
      return vertices;
    }

    public Vector4 ReadAttribute(EndianBinaryReader f, int type, int count) {
      Vector4 o = new Vector4();
      switch (type) {
        case 0://nothing
          break;
        case 1: //Vec3
          break;
        case 2: //Vec4
          if (count > 0 && f.Position + 4 < f.Length)
            o.X = f.ReadSingle();
          if (count > 1 && f.Position + 4 < f.Length)
            o.Y = f.ReadSingle();
          if (count > 2 && f.Position + 4 < f.Length)
            o.Z = f.ReadSingle();
          if (count > 3 && f.Position + 4 < f.Length)
            o.W = f.ReadSingle();
          break;
        default:
          throw new Exception("Unknown Type 0x" + type.ToString("x") + " " + f.ReadInt32().ToString("X") + f.ReadInt32().ToString("X"));
      }
      return o;
    }
  }

  public class GenericVertex {
    public Vector3 Pos { get; set; }
    public Vector3 Nrm { get; set; }
    public Vector2 Uv0 { get; set; }
    public Vector4 Weights { get; set; }
    public Vector4 Clr { get; set; }
    public Vector4 Bones { get; set; }
  }
}
