// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.NKMProperties
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Flobbster.Windows.Forms;
using MKDS_Course_Modifier.Language;
using OpenTK;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class NKMProperties
  {
    public class OBJI : PropertyTable
    {
      public ushort OldObjectID;

      [MLCategory("nkm.position")]
      [DisplayName("X")]
      public float Tx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.position")]
      public float Ty { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Z")]
      public float Tz { get; set; }

      [MLCategory("nkm.rotation")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [DisplayName("X")]
      public float Rx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.rotation")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      public float Ry { get; set; }

      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [MLCategory("nkm.rotation")]
      [DisplayName("Z")]
      public float Rz { get; set; }

      [DisplayName("X")]
      [MLCategory("nkm.scale")]
      public float Sx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.scale")]
      public float Sy { get; set; }

      [DisplayName("Z")]
      [MLCategory("nkm.scale")]
      public float Sz { get; set; }

      [MLCategory("nkm.obji.object")]
      [Editor(typeof (System.ObjectSelector), typeof (UITypeEditor))]
      [TypeConverter(typeof (ObjectIDTypeConverter))]
      [MLDisplayName("nkm.obji.objectid")]
      public ushort ObjectID { get; set; }

      [MLCategory("nkm.obji.object")]
      [MLDisplayName("nkm.obji.routeid")]
      public short RouteID { get; set; }

      [MLDisplayName("nkm.obji.showtt")]
      [MLCategory("nkm.obji.object")]
      public bool ShowTT { get; set; }
    }

    public class PATH
    {
      [MLDisplayName("nkm.index")]
      [MLCategory("nkm.path.path")]
      public byte Index { get; set; }

      [MLCategory("nkm.path.path")]
      [MLDisplayName("nkm.path.loop")]
      public bool Loop { get; set; }

      [MLDisplayName("nkm.path.nrpoint")]
      [MLCategory("nkm.path.path")]
      public short NrPoit { get; set; }
    }

    public class POIT
    {
      [MLCategory("nkm.position")]
      [DisplayName("X")]
      public float Tx { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Y")]
      public float Ty { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Z")]
      public float Tz { get; set; }

      [MLDisplayName("nkm.index")]
      [MLCategory("nkm.poit.point")]
      public short Index { get; set; }

      [MLCategory("nkm.poit.point")]
      [MLDisplayName("nkm.duration")]
      public short Duration { get; set; }

      [MLCategory("nkm.unknown")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLDisplayName("nkm.unknown1")]
      public uint Unknown1 { get; set; }
    }

    public class KTPS
    {
      [MLCategory("nkm.position")]
      [DisplayName("X")]
      public float Tx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.position")]
      public float Ty { get; set; }

      [DisplayName("Z")]
      [MLCategory("nkm.position")]
      public float Tz { get; set; }

      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [DisplayName("X")]
      [MLCategory("nkm.rotation")]
      public float Rx { get; set; }

      [MLCategory("nkm.rotation")]
      [DisplayName("Y")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      public float Ry { get; set; }

      [DisplayName("Z")]
      [MLCategory("nkm.rotation")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      public float Rz { get; set; }

      [MLDisplayName("nkm.index")]
      [MLCategory("nkm.start")]
      public short Index { get; set; }

      [TypeConverter(typeof (UInt16HexTypeConverter))]
      [MLDisplayName("nkm.padding")]
      [MLCategory("nkm.unknown")]
      public ushort Padding { get; set; }
    }

    public class KTPJ
    {
      [DisplayName("X")]
      [MLCategory("nkm.position")]
      public float Tx { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Y")]
      public float Ty { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Z")]
      public float Tz { get; set; }

      [MLCategory("nkm.rotation")]
      [DisplayName("X")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      public float Rx { get; set; }

      [DisplayName("Y")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [MLCategory("nkm.rotation")]
      public float Ry { get; set; }

      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [MLCategory("nkm.rotation")]
      [DisplayName("Z")]
      public float Rz { get; set; }

      [MLCategory("nkm.respawn")]
      [MLDisplayName("nkm.ktpj.enemy")]
      public short EnemyPositionID { get; set; }

      [MLCategory("nkm.respawn")]
      [MLDisplayName("nkm.ktpj.item")]
      public short ItemPositionID { get; set; }

      [MLDisplayName("nkm.index")]
      [MLCategory("nkm.respawn")]
      public int Index { get; set; }
    }

    public class KTP2
    {
      [DisplayName("X")]
      [MLCategory("nkm.position")]
      public float Tx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.position")]
      public float Ty { get; set; }

      [DisplayName("Z")]
      [MLCategory("nkm.position")]
      public float Tz { get; set; }

      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [MLCategory("nkm.rotation")]
      [DisplayName("X")]
      public float Rx { get; set; }

      [MLCategory("nkm.rotation")]
      [DisplayName("Y")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      public float Ry { get; set; }

      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [DisplayName("Z")]
      [MLCategory("nkm.rotation")]
      public float Rz { get; set; }

      [MLCategory("nkm.start")]
      [MLDisplayName("nkm.index")]
      public short Index { get; set; }

      [TypeConverter(typeof (UInt16HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.padding")]
      public ushort Padding { get; set; }
    }

    public class KTPC
    {
      [DisplayName("X")]
      [MLCategory("nkm.position")]
      public float Tx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.position")]
      public float Ty { get; set; }

      [DisplayName("Z")]
      [MLCategory("nkm.position")]
      public float Tz { get; set; }

      [MLCategory("nkm.rotation")]
      [DisplayName("X")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      public float Rx { get; set; }

      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [DisplayName("Y")]
      [MLCategory("nkm.rotation")]
      public float Ry { get; set; }

      [DisplayName("Z")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [MLCategory("nkm.rotation")]
      public float Rz { get; set; }

      [MLDisplayName("nkm.index")]
      [MLCategory("nkm.cannon")]
      public short Index { get; set; }

      [MLDisplayName("nkm.unknown1")]
      [TypeConverter(typeof (UInt16HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      public ushort Unknown1 { get; set; }
    }

    public class KTPM
    {
      [MLCategory("nkm.position")]
      [DisplayName("X")]
      public float Tx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.position")]
      public float Ty { get; set; }

      [DisplayName("Z")]
      [MLCategory("nkm.position")]
      public float Tz { get; set; }

      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [DisplayName("X")]
      [MLCategory("nkm.rotation")]
      public float Rx { get; set; }

      [DisplayName("Y")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [MLCategory("nkm.rotation")]
      public float Ry { get; set; }

      [MLCategory("nkm.rotation")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [DisplayName("Z")]
      public float Rz { get; set; }

      [MLDisplayName("nkm.index")]
      [MLCategory("nkm.missionfinish")]
      public short Index { get; set; }

      [MLDisplayName("nkm.padding")]
      [TypeConverter(typeof (UInt16HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      public ushort Padding { get; set; }
    }

    public class CPOI
    {
      private float tx1;
      private float tz1;
      private float tx2;
      private float tz2;

      private double AngleBetween(Vector2 a, Vector2 b)
      {
        double num1 = (double) b.X - (double) a.X;
        double num2 = (double) b.Y - (double) a.Y;
        return num1 != 0.0 ? (num2 != 0.0 ? (num1 >= 0.0 ? (num2 >= 0.0 ? Math.Atan(num2 / num1) : Math.Atan(num2 / num1) + 2.0 * Math.PI) : Math.Atan(num2 / num1) + Math.PI) : (num1 <= 0.0 ? Math.PI : 0.0)) : (num1 != 0.0 ? (num2 <= 0.0 ? 3.0 * Math.PI / 2.0 : Math.PI / 2.0) : 0.0);
      }

      private void CalculateSinCos()
      {
        double num = Math.Atan(((double) this.Tz1 - (double) this.Tz2) / ((double) this.Tx1 - (double) this.Tx2));
        this.Sinus = (float) Math.Sin(Math.Abs(num));
        this.Cosinus = (float) Math.Cos(Math.Abs(num));
        if ((double) this.Tz1 - (double) this.Tz2 > 0.0)
          this.Sinus = 0.0f - this.Sinus;
        if ((double) this.Tx1 - (double) this.Tx2 >= 0.0)
          return;
        this.Cosinus = 0.0f - this.Cosinus;
      }

      [RefreshProperties(RefreshProperties.All)]
      [DisplayName("X")]
      [MLCategory("nkm.position1")]
      public float Tx1
      {
        get
        {
          return this.tx1;
        }
        set
        {
          this.tx1 = value;
          this.CalculateSinCos();
        }
      }

      [DisplayName("Z")]
      [RefreshProperties(RefreshProperties.All)]
      [MLCategory("nkm.position1")]
      public float Tz1
      {
        get
        {
          return this.tz1;
        }
        set
        {
          this.tz1 = value;
          this.CalculateSinCos();
        }
      }

      [MLCategory("nkm.position2")]
      [DisplayName("X")]
      [RefreshProperties(RefreshProperties.All)]
      public float Tx2
      {
        get
        {
          return this.tx2;
        }
        set
        {
          this.tx2 = value;
          this.CalculateSinCos();
        }
      }

      [RefreshProperties(RefreshProperties.All)]
      [DisplayName("Z")]
      [MLCategory("nkm.position2")]
      public float Tz2
      {
        get
        {
          return this.tz2;
        }
        set
        {
          this.tz2 = value;
          this.CalculateSinCos();
        }
      }

      [DisplayName("Sinus")]
      [MLCategory("nkm.checkpoint")]
      public float Sinus { get; private set; }

      [DisplayName("Cosinus")]
      [MLCategory("nkm.checkpoint")]
      public float Cosinus { get; private set; }

      [MLDisplayName("nkm.cpoi.distance")]
      [MLCategory("nkm.checkpoint")]
      public float Distance { get; set; }

      [MLDisplayName("nkm.cpoi.sectiondata")]
      [MLCategory("nkm.checkpoint")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      public uint SectionData { get; set; }

      [MLCategory("nkm.checkpoint")]
      [MLDisplayName("nkm.cpoi.keypoint")]
      public short KeyPoint { get; set; }

      [MLCategory("nkm.checkpoint")]
      [MLDisplayName("nkm.cpoi.respid")]
      public short RespawnID { get; set; }
    }

    public class CPAT
    {
      [MLDisplayName("nkm.cpat.startidx")]
      [MLCategory("nkm.cpat")]
      public short StartIdx { get; set; }

      [MLDisplayName("nkm.cpat.length")]
      [MLCategory("nkm.cpat")]
      public short Length { get; set; }

      [MLDisplayName("nkm.cpat.sectionorder")]
      [MLCategory("nkm.cpat")]
      public short SectionOrder { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom1")]
      public sbyte ComesFrom1 { get; set; }

      [MLDisplayName("nkm.cpat.comesfrom2")]
      [MLCategory("nkm.cpat.comesfrom")]
      public sbyte ComesFrom2 { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom3")]
      public sbyte ComesFrom3 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto1")]
      public sbyte GoesTo1 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto2")]
      public sbyte GoesTo2 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto3")]
      public sbyte GoesTo3 { get; set; }
    }

    public class IPOI
    {
      [MLCategory("nkm.position")]
      [DisplayName("X")]
      public float Tx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.position")]
      public float Ty { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Z")]
      public float Tz { get; set; }

      [MLCategory("nkm.unknown")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLDisplayName("nkm.unknown1")]
      public uint Unknown1 { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown2")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      public uint Unknown2 { get; set; }
    }

    public class IPAT
    {
      [MLCategory("nkm.cpat")]
      [MLDisplayName("nkm.cpat.startidx")]
      public short StartIdx { get; set; }

      [MLDisplayName("nkm.cpat.length")]
      [MLCategory("nkm.cpat")]
      public short Length { get; set; }

      [MLDisplayName("nkm.cpat.sectionorder")]
      [MLCategory("nkm.cpat")]
      public short SectionOrder { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom1")]
      public sbyte ComesFrom1 { get; set; }

      [MLDisplayName("nkm.cpat.comesfrom2")]
      [MLCategory("nkm.cpat.comesfrom")]
      public sbyte ComesFrom2 { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom3")]
      public sbyte ComesFrom3 { get; set; }

      [MLDisplayName("nkm.cpat.goesto1")]
      [MLCategory("nkm.cpat.goesto")]
      public sbyte GoesTo1 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto2")]
      public sbyte GoesTo2 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto3")]
      public sbyte GoesTo3 { get; set; }
    }

    public class EPOI
    {
      [MLCategory("nkm.position")]
      [DisplayName("X")]
      public float Tx { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Y")]
      public float Ty { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Z")]
      public float Tz { get; set; }

      [MLDisplayName("nkm.epoi.pointsize")]
      [MLCategory("nkm.epoi")]
      public float PointSize { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown1")]
      [TypeConverter(typeof (UInt16HexTypeConverter))]
      public ushort Unknown1 { get; set; }

      [MLDisplayName("nkm.unknown2")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      public uint Unknown2 { get; set; }

      [MLCategory("nkm.epoi")]
      [MLDisplayName("nkm.epoi.drifting")]
      public short Drifting { get; set; }
    }

    public class EPAT
    {
      [MLCategory("nkm.cpat")]
      [MLDisplayName("nkm.cpat.startidx")]
      public short StartIdx { get; set; }

      [MLCategory("nkm.cpat")]
      [MLDisplayName("nkm.cpat.length")]
      public short Length { get; set; }

      [MLDisplayName("nkm.cpat.sectionorder")]
      [MLCategory("nkm.cpat")]
      public short SectionOrder { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom1")]
      public sbyte ComesFrom1 { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom2")]
      public sbyte ComesFrom2 { get; set; }

      [MLDisplayName("nkm.cpat.comesfrom3")]
      [MLCategory("nkm.cpat.comesfrom")]
      public sbyte ComesFrom3 { get; set; }

      [MLDisplayName("nkm.cpat.goesto1")]
      [MLCategory("nkm.cpat.goesto")]
      public sbyte GoesTo1 { get; set; }

      [MLDisplayName("nkm.cpat.goesto2")]
      [MLCategory("nkm.cpat.goesto")]
      public sbyte GoesTo2 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto3")]
      public sbyte GoesTo3 { get; set; }
    }

    public class MEPO
    {
      [MLCategory("nkm.position")]
      [DisplayName("X")]
      public float Tx { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Y")]
      public float Ty { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Z")]
      public float Tz { get; set; }

      [MLDisplayName("nkm.epoi.pointsize")]
      [MLCategory("nkm.epoi")]
      public float PointSize { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown1")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      public uint Unknown1 { get; set; }

      [MLDisplayName("nkm.epoi.drifting")]
      [MLCategory("nkm.epoi")]
      public int Drifting { get; set; }
    }

    public class MEPA
    {
      [MLCategory("nkm.cpat")]
      [MLDisplayName("nkm.cpat.startidx")]
      public short StartIdx { get; set; }

      [MLCategory("nkm.cpat")]
      [MLDisplayName("nkm.cpat.length")]
      public short Length { get; set; }

      [MLDisplayName("nkm.cpat.comesfrom1")]
      [MLCategory("nkm.cpat.comesfrom")]
      public sbyte ComesFrom1 { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom2")]
      public sbyte ComesFrom2 { get; set; }

      [MLDisplayName("nkm.cpat.comesfrom3")]
      [MLCategory("nkm.cpat.comesfrom")]
      public sbyte ComesFrom3 { get; set; }

      [MLDisplayName("nkm.cpat.comesfrom4")]
      [MLCategory("nkm.cpat.comesfrom")]
      public sbyte ComesFrom4 { get; set; }

      [MLDisplayName("nkm.cpat.comesfrom5")]
      [MLCategory("nkm.cpat.comesfrom")]
      public sbyte ComesFrom5 { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom6")]
      public sbyte ComesFrom6 { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom7")]
      public sbyte ComesFrom7 { get; set; }

      [MLCategory("nkm.cpat.comesfrom")]
      [MLDisplayName("nkm.cpat.comesfrom8")]
      public sbyte ComesFrom8 { get; set; }

      [MLDisplayName("nkm.cpat.goesto1")]
      [MLCategory("nkm.cpat.goesto")]
      public sbyte GoesTo1 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto2")]
      public sbyte GoesTo2 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto3")]
      public sbyte GoesTo3 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto4")]
      public sbyte GoesTo4 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto5")]
      public sbyte GoesTo5 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto6")]
      public sbyte GoesTo6 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto7")]
      public sbyte GoesTo7 { get; set; }

      [MLCategory("nkm.cpat.goesto")]
      [MLDisplayName("nkm.cpat.goesto8")]
      public sbyte GoesTo8 { get; set; }
    }

    public class AREA
    {
      [DisplayName("X")]
      [MLCategory("nkm.position")]
      public float Tx { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Y")]
      public float Ty { get; set; }

      [MLCategory("nkm.position")]
      [DisplayName("Z")]
      public float Tz { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown1")]
      public float Unknown1 { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown2")]
      public float Unknown2 { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown3")]
      public float Unknown3 { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown4")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      public uint Unknown4 { get; set; }

      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown5")]
      public uint Unknown5 { get; set; }

      [MLDisplayName("nkm.unknown6")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      public uint Unknown6 { get; set; }

      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLDisplayName("nkm.unknown7")]
      [MLCategory("nkm.unknown")]
      public uint Unknown7 { get; set; }

      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown8")]
      public uint Unknown8 { get; set; }

      [MLDisplayName("nkm.unknown9")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      public uint Unknown9 { get; set; }

      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLDisplayName("nkm.unknown10")]
      [MLCategory("nkm.unknown")]
      public uint Unknown10 { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown11")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      public uint Unknown11 { get; set; }

      [MLDisplayName("nkm.unknown12")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      public uint Unknown12 { get; set; }

      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown13")]
      public uint Unknown13 { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown14")]
      public short Unknown14 { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown15")]
      public byte Unknown15 { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown16")]
      public int Unknown16 { get; set; }

      [MLCategory("nkm.area")]
      [MLDisplayName("nkm.area.linkedcame")]
      public sbyte LinkedCame { get; set; }
    }

    public class CAME
    {
      [DisplayName("X")]
      [MLCategory("nkm.position1")]
      public float Tx1 { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.position1")]
      public float Ty1 { get; set; }

      [MLCategory("nkm.position1")]
      [DisplayName("Z")]
      public float Tz1 { get; set; }

      [MLCategory("nkm.position2")]
      [DisplayName("X")]
      public float Tx2 { get; set; }

      [MLCategory("nkm.position2")]
      [DisplayName("Y")]
      public float Ty2 { get; set; }

      [MLCategory("nkm.position2")]
      [DisplayName("Z")]
      public float Tz2 { get; set; }

      [MLCategory("nkm.position3")]
      [DisplayName("X")]
      public float Tx3 { get; set; }

      [MLCategory("nkm.position3")]
      [DisplayName("Y")]
      public float Ty3 { get; set; }

      [MLCategory("nkm.position3")]
      [DisplayName("Z")]
      public float Tz3 { get; set; }

      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [MLCategory("nkm.rotation")]
      [DisplayName("X")]
      public float Rx { get; set; }

      [DisplayName("Y")]
      [MLCategory("nkm.rotation")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      public float Ry { get; set; }

      [MLCategory("nkm.rotation")]
      [Editor(typeof (RotationSelector), typeof (UITypeEditor))]
      [DisplayName("Z")]
      public float Rz { get; set; }

      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown1")]
      [TypeConverter(typeof (UInt32HexTypeConverter))]
      public uint Unknown1 { get; set; }

      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown2")]
      public uint Unknown2 { get; set; }

      [TypeConverter(typeof (UInt32HexTypeConverter))]
      [MLCategory("nkm.unknown")]
      [MLDisplayName("nkm.unknown3")]
      public uint Unknown3 { get; set; }

      [MLDisplayName("nkm.unknown4")]
      [MLCategory("nkm.unknown")]
      [TypeConverter(typeof (UInt16HexTypeConverter))]
      public ushort Unknown4 { get; set; }

      [MLCategory("nkm.came")]
      [MLDisplayName("nkm.came.linkedroute")]
      public short LinkedRoute { get; set; }

      [MLDisplayName("nkm.came.routespeed")]
      [MLCategory("nkm.came")]
      public short RouteSpeed { get; set; }

      [MLDisplayName("nkm.came.pointspeed")]
      [MLCategory("nkm.came")]
      public short PointSpeed { get; set; }

      [MLDisplayName("nkm.came.camerazoom")]
      [MLCategory("nkm.came")]
      public short CameraZoom { get; set; }

      [MLCategory("nkm.came")]
      [MLDisplayName("nkm.came.cameratype")]
      public short CameraType { get; set; }

      [MLCategory("nkm.came")]
      [MLDisplayName("nkm.came.nextcame")]
      public short NextCame { get; set; }

      [MLCategory("nkm.came")]
      [MLDisplayName("nkm.came.totallength")]
      public TimeSpan TotalLength { get; set; }
    }
  }
}
