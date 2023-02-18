using fin.schema.data;
using System;
using System.IO;
using fin.util.strings;
using schema.binary;
using schema.binary.attributes.endianness;
using schema.binary.attributes.ignore;


namespace cmb.schema.cmb {
  [Endianness(Endianness.LittleEndian)]
  public class Cmb : IBinaryDeserializable {
    public long startOffset;

    public readonly CmbHeader header = new();

    private const int TWEAK_AUTO_SIZE = -8;

    public readonly AutoMagicUInt32SizedSection<Skl> skl
        = new("skl" + AsciiUtil.GetChar(0x20), TWEAK_AUTO_SIZE);

    public readonly AutoMagicUInt32SizedSection<Qtrs> qtrs 
        = new("qtrs", TWEAK_AUTO_SIZE);
    public readonly Mat mat = new();

    public readonly AutoMagicUInt32SizedSection<Tex> tex
        = new("tex" + AsciiUtil.GetChar(0x20), TWEAK_AUTO_SIZE);

    public readonly Sklm sklm = new();
    public readonly AutoMagicUInt32SizedSection<Luts> luts 
        = new("luts", TWEAK_AUTO_SIZE);
    public readonly Vatr vatr = new();

    public Cmb(IEndianBinaryReader r) => this.Read(r);

    public void Read(IEndianBinaryReader r) {
      long startOff = 0;

      if (r.ReadString(4) == "ZSI" + AsciiUtil.GetChar(1)) {
        r.Position = 16;

        while (true) {
          var cmd0 = r.ReadUInt32();
          var cmd1 = r.ReadUInt32();

          var cmdType = cmd0 & 0xFF;

          if (cmdType == 0x14) {
            break;
          }
          if (cmdType == 0x0A) {
            r.Position = cmd1 + 20;

            var entryOfs = r.ReadUInt32();
            r.Position = entryOfs + 24;

            var cmbOfs = r.ReadUInt32();
            r.Position = cmbOfs + 16;

            startOff = r.Position;
            break;
          }
        }
      }

      this.startOffset = r.Position = startOff;

      this.header.Read(r);
      this.skl.Read(r);

      if (CmbHeader.Version > CmbVersion.OCARINA_OF_TIME_3D) {
        this.qtrs.Read(r);
      }

      r.Position = startOff + this.header.matsOffset;
      this.mat.Read(r);
      this.tex.Read(r);
      this.sklm.Read(r);

      r.Position = startOff + this.header.lutsOffset;
      this.luts.Read(r);

      r.Position = startOff + this.header.vatrOffset;
      this.vatr.Read(r);

      // Add face indices to primitive sets
      foreach (var shape in this.sklm.shapes.shapes) {
        foreach (var pset in shape.primitiveSets) {
          var primitive = pset.primitive;
          // # Always * 2 even if ubyte is used...
          r.Position = startOff +
                       this.header.faceIndicesOffset +
                       2 * primitive.offset;

          primitive.indices = new uint[primitive.indicesCount];
          for (var i = 0; i < primitive.indicesCount; ++i) {
            switch (primitive.dataType) {
              case DataType.UByte: {
                primitive.indices[i] = r.ReadByte();
                break;
              }
              case DataType.UShort: {
                primitive.indices[i] = r.ReadUInt16();
                break;
              }
              default: {
                throw new NotImplementedException();
              }
            }
          }
        }
      }
    }
  }
}


/*pset.primitive.indices = 
[int(readDataType(f, pset.primitive.dataType)) for _ in
range(pset.primitive.indicesCount)]*/