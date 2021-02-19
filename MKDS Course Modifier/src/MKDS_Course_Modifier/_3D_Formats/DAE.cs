// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3D_Formats.DAE
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MKDS_Course_Modifier._3D_Formats
{
  public class DAE
  {
    public class accessor
    {
      [XmlAttribute]
      public ulong count;
      [DefaultValue(0)]
      [XmlAttribute]
      public ulong offset;
      [XmlAttribute]
      public string source;
      [XmlAttribute]
      public ulong stride;
      [XmlElement("param")]
      public List<DAE.param> param;

      public accessor()
      {
        this.param = new List<DAE.param>();
      }
    }

    public class asset
    {
      [DefaultValue(DAE.UpAxisType.Y_UP)]
      public DAE.UpAxisType up_axis = DAE.UpAxisType.Y_UP;
      [XmlElement("contributor")]
      public List<DAE.asset._contributor> contributor;
      [XmlElement(IsNullable = false)]
      public DateTime created;
      public string keywords;
      [XmlElement(IsNullable = false)]
      public DateTime modified;
      public string revision;
      public string subject;
      public string title;
      public DAE.asset._unit unit;

      public asset()
      {
        this.contributor = new List<DAE.asset._contributor>();
        this.unit = new DAE.asset._unit("meter", 1f);
      }

      public class _contributor
      {
        public string author;
        public string authoring_tool;
        public string comments;
        public string copyright;
        public string source_data;
      }

      public class _unit
      {
        [XmlAttribute]
        public double meter;
        [XmlAttribute]
        public string name;

        public _unit()
        {
        }

        public _unit(string name, float meter)
        {
          this.name = name;
          this.meter = (double) meter;
        }
      }
    }

    public class bind_material
    {
      [XmlElement("param")]
      public List<DAE.param> param;
      [XmlElement("technique_common")]
      public DAE.bind_material._technique_common technique_common;
      [XmlElement("technique")]
      public List<DAE.technique> technique;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public bind_material()
      {
        this.param = new List<DAE.param>();
        this.technique = new List<DAE.technique>();
        this.extra = new List<DAE.extra>();
      }

      [XmlType("technique_commonA")]
      public class _technique_common
      {
        [XmlElement("instance_material")]
        public List<DAE.instance_material> instance_material;

        public _technique_common()
        {
          this.instance_material = new List<DAE.instance_material>();
        }
      }
    }

    [Serializable]
    public class COLLADA
    {
      [XmlElement(IsNullable = false)]
      public DAE.asset asset;
      public DAE.library_controllers library_controllers;
      public DAE.library_geometries library_geometries;
      public DAE.library_effects library_effects;
      public DAE.library_images library_images;
      public DAE.library_materials library_materials;
      public DAE.library_visual_scenes library_visual_scenes;
      public DAE.COLLADA._scene scene;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public COLLADA()
      {
        this.asset = new DAE.asset();
        this.asset.created = DateTime.Now;
        this.extra = new List<DAE.extra>();
      }

      public void Write(string FileName)
      {
        this.asset.modified = DateTime.Now;
        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", "");
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (DAE.COLLADA));
        TextWriter textWriter = (TextWriter) new StreamWriter(FileName);
        xmlSerializer.Serialize(textWriter, (object) this, namespaces);
        textWriter.Close();
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(FileName);
        xmlDocument.DocumentElement.SetAttribute("xmlns", "http://www.collada.org/2005/11/COLLADASchema");
        xmlDocument.DocumentElement.SetAttribute("version", "1.4.1");
        xmlDocument.Save(FileName);
      }

      public static DAE.COLLADA FromFile(string FileName)
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (DAE.COLLADA));
        TextReader textReader = (TextReader) new StringReader(File.ReadAllText(FileName).Replace("<COLLADA xmlns=\"http://www.collada.org/2005/11/COLLADASchema\" version=\"1.4.1\">", "<COLLADA>").Replace("<COLLADA version=\"1.4.1\" xmlns=\"http://www.collada.org/2008/03/COLLADASchema\">", "<COLLADA>"));
        textReader.ReadLine();
        object obj = xmlSerializer.Deserialize(textReader);
        textReader.Close();
        return (DAE.COLLADA) obj;
      }

      public class _scene
      {
        [XmlElement("instance_physics_scene")]
        public List<DAE.InstanceWithExtra> instance_physics_scene;
        public DAE.InstanceWithExtra instance_visual_scene;
        [XmlElement("extra")]
        public List<DAE.extra> extra;

        public _scene()
        {
          this.instance_physics_scene = new List<DAE.InstanceWithExtra>();
          this.extra = new List<DAE.extra>();
        }
      }
    }

    public class common_color_or_texture_type
    {
      public DAE.common_color_or_texture_type._color color;
      public DAE.common_color_or_texture_type._param param;
      public DAE.common_color_or_texture_type._texture texture;

      public class _color : IXmlSerializable
      {
        [XmlAttribute]
        public string sid;
        public Color content;

        public XmlSchema GetSchema()
        {
          throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
          this.sid = reader.GetAttribute("sid");
          string[] strArray = reader.ReadInnerXml().Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Split(new char[1]
          {
            ' '
          }, StringSplitOptions.RemoveEmptyEntries);
          this.content = Color.FromArgb((int) (double.Parse(strArray[3]) * (double) byte.MaxValue), (int) (double.Parse(strArray[0]) * (double) byte.MaxValue), (int) (double.Parse(strArray[1]) * (double) byte.MaxValue), (int) (double.Parse(strArray[2]) * (double) byte.MaxValue));
        }

        public void WriteXml(XmlWriter writer)
        {
          if (this.sid != null && this.sid != "")
            writer.WriteAttributeString("sid", this.sid);
          string[] strArray1 = new string[7];
          string[] strArray2 = strArray1;
          float num = (float) this.content.R / (float) byte.MaxValue;
          string str1 = num.ToString();
          strArray2[0] = str1;
          strArray1[1] = " ";
          string[] strArray3 = strArray1;
          num = (float) this.content.G / (float) byte.MaxValue;
          string str2 = num.ToString();
          strArray3[2] = str2;
          strArray1[3] = " ";
          string[] strArray4 = strArray1;
          num = (float) this.content.B / (float) byte.MaxValue;
          string str3 = num.ToString();
          strArray4[4] = str3;
          strArray1[5] = " ";
          string[] strArray5 = strArray1;
          num = (float) this.content.A / (float) byte.MaxValue;
          string str4 = num.ToString();
          strArray5[6] = str4;
          string data = string.Concat(strArray1).Replace(",", ".");
          writer.WriteRaw(data);
        }
      }

      [XmlType("paramA")]
      public class _param
      {
        [XmlAttribute]
        public string @ref;
      }

      public class _texture
      {
        [XmlAttribute]
        public string texture;
        [XmlAttribute]
        public string texcoord;
        public DAE.extra extra;
      }
    }

    public class common_float_or_param_type
    {
      public DAE.common_float_or_param_type._float @float;
      public DAE.common_float_or_param_type._param param;

      public class _float : IXmlSerializable
      {
        [XmlAttribute]
        public string sid;
        public float content;

        public XmlSchema GetSchema()
        {
          throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
          this.sid = reader.GetAttribute("sid");
          this.content = float.Parse(reader.ReadInnerXml());
        }

        public void WriteXml(XmlWriter writer)
        {
          if (this.sid != null && this.sid != "")
            writer.WriteAttributeString("sid", this.sid);
          writer.WriteRaw(this.content.ToString());
        }
      }

      [XmlType("paramB")]
      public class _param
      {
        [XmlAttribute]
        public string @ref;
      }
    }

    public class common_newparam_type
    {
      [XmlAttribute]
      public string sid;
      public string semantic;
      [XmlElement("surface", typeof (DAE.fx_surface_common))]
      [XmlElement("float", typeof (double))]
      [XmlElement("sampler2D", typeof (DAE.fx_sampler2D_common))]
      public object choice;
    }

    public class common_transparent_type : DAE.common_color_or_texture_type
    {
      [XmlAttribute]
      [DefaultValue(DAE.fx_opaque_enum.A_ONE)]
      public DAE.fx_opaque_enum opaque;
    }

    public class controller
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      public DAE.skin skin;
      [XmlElement("extra")]
      public List<DAE.extra> extra;
    }

    public class effect
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      [XmlElement("annotate")]
      public List<DAE.fx_annotate_common> annotate;
      [XmlElement("image")]
      public List<DAE.image> image;
      [XmlElement("newparam")]
      public List<DAE.fx_newparam_common> newparam;
      public DAE.profile_COMMON profile_COMMON;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public effect()
      {
        this.annotate = new List<DAE.fx_annotate_common>();
        this.image = new List<DAE.image>();
        this.newparam = new List<DAE.fx_newparam_common>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class extra
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      [XmlAttribute]
      public string type;
      public DAE.asset asset;
    }

    public class float4x4 : IXmlSerializable
    {
      public double[][] content;

      public float4x4()
      {
        this.content = new double[4][];
        this.content[0] = new double[4];
        this.content[1] = new double[4];
        this.content[2] = new double[4];
        this.content[3] = new double[4];
        this.content[0][0] = 1.0;
        this.content[1][1] = 1.0;
        this.content[2][2] = 1.0;
        this.content[3][3] = 1.0;
      }

      public XmlSchema GetSchema()
      {
        throw new NotImplementedException();
      }

      public void ReadXml(XmlReader reader)
      {
        string[] strArray = reader.ReadInnerXml().Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        int num = 0;
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
            this.content[index2][index1] = double.Parse(strArray[num++]);
        }
      }

      public void WriteXml(XmlWriter writer)
      {
        string str = "";
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
            str = str + (object) this.content[index2][index1] + " ";
        }
        str.Trim();
        string data = str.Replace(",", ".");
        writer.WriteRaw(data);
      }
    }

    public class fx_annotate_common
    {
      [XmlAttribute]
      public string name;
      [XmlElement("string", typeof (string))]
      [XmlElement("float", typeof (double))]
      public object choice;
    }

    public class fx_newparam_common
    {
      [XmlAttribute]
      public string sid;
      [XmlElement("annotate")]
      public List<DAE.fx_annotate_common> annotate;
      public string semantic;
      public DAE.fx_modifier_enum_common modifier;
      [XmlElement("float", typeof (double))]
      [XmlElement("surface", typeof (DAE.fx_surface_common))]
      [XmlElement("sampler2D", typeof (DAE.fx_sampler2D_common))]
      public object choice;

      public fx_newparam_common()
      {
        this.annotate = new List<DAE.fx_annotate_common>();
      }
    }

    public class fx_sampler2D_common
    {
      [DefaultValue(DAE.fx_sampler_wrap_common.WRAP)]
      public DAE.fx_sampler_wrap_common wrap_s = DAE.fx_sampler_wrap_common.WRAP;
      [DefaultValue(DAE.fx_sampler_wrap_common.WRAP)]
      public DAE.fx_sampler_wrap_common wrap_t = DAE.fx_sampler_wrap_common.WRAP;
      [DefaultValue(255)]
      public byte mipmap_maxlevel = byte.MaxValue;
      public string source;
      [DefaultValue(DAE.fx_sampler_filter_common.NONE)]
      public DAE.fx_sampler_filter_common minfilter;
      [DefaultValue(DAE.fx_sampler_filter_common.NONE)]
      public DAE.fx_sampler_filter_common magfilter;
      [DefaultValue(DAE.fx_sampler_filter_common.NONE)]
      public DAE.fx_sampler_filter_common mipfilter;
      [DefaultValue(0)]
      public float mipmap_bias;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public fx_sampler2D_common()
      {
        this.extra = new List<DAE.extra>();
      }
    }

    public class fx_surface_common
    {
      [XmlAttribute]
      public DAE.fx_surface_type_enum type;
      [XmlElement("init_from")]
      public List<DAE.fx_surface_init_from_common> init_from;
      public string format;
      public DAE.fx_surface_common format_hint;
      [DefaultValue(0)]
      public uint mip_levels;
      [DefaultValue(false)]
      public bool mipmap_generate;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public fx_surface_common()
      {
        this.init_from = new List<DAE.fx_surface_init_from_common>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class fx_surface_format_hint_common
    {
      public DAE.fx_surface_format_hint_channels_enum channels;
      public DAE.fx_surface_format_hint_range_enum range;
      public DAE.fx_surface_format_hint_precision_enum precision;
      public DAE.fx_surface_format_hint_option_enum option;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public fx_surface_format_hint_common()
      {
        this.extra = new List<DAE.extra>();
      }
    }

    public class fx_surface_init_from_common
    {
      [DefaultValue(0)]
      [XmlAttribute]
      public uint mip;
      [DefaultValue(0)]
      [XmlAttribute]
      public uint slice;
      [XmlAttribute]
      [DefaultValue(DAE.fx_surface_face_enum.POSITIVE_X)]
      public DAE.fx_surface_face_enum face;
      [XmlText]
      public string content;
    }

    public class geometry
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      public DAE.mesh mesh;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public geometry()
      {
        this.extra = new List<DAE.extra>();
      }
    }

    public class image
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      [XmlAttribute]
      public string format;
      [XmlAttribute]
      [DefaultValue(0)]
      public ulong height;
      [DefaultValue(0)]
      [XmlAttribute]
      public ulong width;
      [DefaultValue(0)]
      [XmlAttribute]
      public ulong depth;
      public DAE.asset asset;
      public string init_from;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public image()
      {
        this.extra = new List<DAE.extra>();
      }
    }

    public class InputLocal
    {
      [XmlAttribute]
      public string semantic;
      [XmlAttribute]
      public string source;
    }

    public class InputLocalOffset
    {
      [XmlAttribute]
      public ulong offset;
      [XmlAttribute]
      public string semantic;
      [XmlAttribute]
      public string source;
      [DefaultValue(0)]
      [XmlAttribute]
      public ulong set;
    }

    public class instance_controller
    {
      [XmlAttribute]
      public string url;
      [XmlAttribute]
      public string sid;
      [XmlAttribute]
      public string name;
      [XmlElement("skeleton")]
      public List<string> skeleton;
      public DAE.bind_material bind_material;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public instance_controller()
      {
        this.skeleton = new List<string>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class instance_effect
    {
      [XmlAttribute]
      public string url;
      [XmlAttribute]
      public string sid;
      [XmlAttribute]
      public string name;
      [XmlElement("technique_hint")]
      public List<DAE.instance_effect._technique_hint> technique_hint;
      [XmlElement("setparam")]
      public List<DAE.instance_effect._setparam> setparam;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public class _technique_hint
      {
        [XmlAttribute]
        public string platform;
        [XmlAttribute]
        public string profile;
        [XmlAttribute]
        public string @ref;
      }

      public class _setparam
      {
        public string @ref;
      }
    }

    public class instance_geometry
    {
      [XmlAttribute]
      public string url;
      [XmlAttribute]
      public string sid;
      [XmlAttribute]
      public string name;
      public DAE.bind_material bind_material;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public instance_geometry()
      {
        this.extra = new List<DAE.extra>();
      }
    }

    public class instance_material
    {
      [XmlAttribute]
      public string symbol;
      [XmlAttribute]
      public string target;
      [XmlAttribute]
      public string sid;
      [XmlAttribute]
      public string name;
      [XmlElement("bind")]
      public List<DAE.instance_material._bind> bind;
      [XmlElement("bind_vertex_input")]
      public List<DAE.instance_material._bind_vertex_input> bind_vertex_input;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public instance_material()
      {
        this.bind = new List<DAE.instance_material._bind>();
        this.bind_vertex_input = new List<DAE.instance_material._bind_vertex_input>();
        this.extra = new List<DAE.extra>();
      }

      public class _bind
      {
        [XmlAttribute]
        public string semantic;
        [XmlAttribute]
        public string target;
      }

      public class _bind_vertex_input
      {
        [XmlAttribute]
        public string semantic;
        [XmlAttribute]
        public string input_semantic;
        [XmlAttribute]
        public ulong input_set;
      }
    }

    public class InstanceWithExtra
    {
      [XmlAttribute]
      public string url;
      [XmlAttribute]
      public string sid;
      [XmlAttribute]
      public string name;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public InstanceWithExtra()
      {
        this.extra = new List<DAE.extra>();
      }
    }

    public class library_controllers
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      [XmlElement("controller")]
      public List<DAE.controller> controller;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public library_controllers()
      {
        this.controller = new List<DAE.controller>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class library_effects
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      [XmlElement("effect")]
      public List<DAE.effect> effect;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public library_effects()
      {
        this.effect = new List<DAE.effect>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class library_geometries
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      [XmlElement("geometry")]
      public List<DAE.geometry> geometry;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public library_geometries()
      {
        this.geometry = new List<DAE.geometry>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class library_images
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      [XmlElement("image")]
      public List<DAE.image> image;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public library_images()
      {
        this.image = new List<DAE.image>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class library_materials
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      [XmlElement("material")]
      public List<DAE.material> material;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public library_materials()
      {
        this.material = new List<DAE.material>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class library_visual_scenes
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      [XmlElement("visual_scene")]
      public List<DAE.visual_scene> visual_scene;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public library_visual_scenes()
      {
        this.visual_scene = new List<DAE.visual_scene>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class material
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      public DAE.instance_effect instance_effect;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public material()
      {
        this.extra = new List<DAE.extra>();
      }
    }

    public class matrix : IXmlSerializable
    {
      [XmlAttribute]
      public string sid;
      public double[][] content;

      public matrix()
      {
        this.content = new double[4][];
        this.content[0] = new double[4];
        this.content[1] = new double[4];
        this.content[2] = new double[4];
        this.content[3] = new double[4];
        this.content[0][0] = 1.0;
        this.content[1][1] = 1.0;
        this.content[2][2] = 1.0;
        this.content[3][3] = 1.0;
      }

      public XmlSchema GetSchema()
      {
        throw new NotImplementedException();
      }

      public void ReadXml(XmlReader reader)
      {
        this.sid = reader.GetAttribute("sid");
        string[] strArray = reader.ReadInnerXml().Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator).Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        int num = 0;
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
            this.content[index2][index1] = double.Parse(strArray[num++]);
        }
      }

      public void WriteXml(XmlWriter writer)
      {
        if (this.sid != null && this.sid != "")
          writer.WriteAttributeString("sid", this.sid);
        string str = "";
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
            str = str + (object) this.content[index2][index1] + " ";
        }
        str.Trim();
        string data = str.Replace(",", ".");
        writer.WriteRaw(data);
      }
    }

    public class mesh
    {
      [XmlElement("source")]
      public List<DAE.source> source;
      public DAE.vertices vertices;
      [XmlElement("triangles")]
      public List<DAE.triangles> triangles;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public mesh()
      {
        this.source = new List<DAE.source>();
        this.triangles = new List<DAE.triangles>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class node
    {
      [XmlAttribute]
      [DefaultValue(DAE.NodeType.NODE)]
      public DAE.NodeType type = DAE.NodeType.NODE;
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      [XmlAttribute]
      public string sid;
      [DefaultValue(null)]
      [XmlAttribute]
      public List<string> layer;
      public DAE.asset asset;
      public DAE.matrix matrix;
      [XmlElement("instance_controller")]
      public List<DAE.instance_controller> instance_controller;
      [XmlElement("instance_geometry")]
      public List<DAE.instance_geometry> instance_geometry;
      [XmlElement("node")]
      public List<DAE.node> _node;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public node()
      {
        this.instance_geometry = new List<DAE.instance_geometry>();
        this._node = new List<DAE.node>();
        this.extra = new List<DAE.extra>();
      }

      public node(string name)
        : this()
      {
        this.name = name;
      }
    }

    public class p : IXmlSerializable
    {
      public List<ulong> content;

      public p()
      {
        this.content = new List<ulong>();
      }

      public XmlSchema GetSchema()
      {
        throw new NotImplementedException();
      }

      public void ReadXml(XmlReader reader)
      {
        string str = reader.ReadInnerXml();
        char[] separator = new char[1]{ ' ' };
        foreach (string s in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          this.content.Add(ulong.Parse(s));
      }

      public void WriteXml(XmlWriter writer)
      {
        string data = "";
        foreach (ulong num in this.content)
          data = data + (object) num + " ";
        data.Trim();
        writer.WriteRaw(data);
      }
    }

    public class param
    {
      [XmlAttribute]
      public string name;
      [XmlAttribute]
      public string sid;
      [XmlAttribute]
      public string semantic;
      [XmlAttribute]
      public string type;
      [XmlText]
      public string content;
    }

    public class profile_COMMON
    {
      [XmlAttribute]
      public string id;
      public DAE.asset asset;
      [XmlElement("image")]
      public List<DAE.image> image;
      [XmlElement("newparam")]
      public List<DAE.common_newparam_type> newparam;
      public DAE.profile_COMMON._technique technique;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public profile_COMMON()
      {
        this.extra = new List<DAE.extra>();
      }

      public class _technique
      {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string sid;
        public DAE.asset asset;
        public DAE.image image;
        public DAE.common_newparam_type newparam;
        public DAE.profile_COMMON._technique._lambert lambert;
        public DAE.profile_COMMON._technique._phong phong;
        [XmlElement("extra")]
        public List<DAE.extra> extra;

        public _technique()
        {
          this.extra = new List<DAE.extra>();
        }

        public class _lambert
        {
          public DAE.common_color_or_texture_type emission;
          public DAE.common_color_or_texture_type ambient;
          public DAE.common_color_or_texture_type diffuse;
          public DAE.common_color_or_texture_type reflective;
          public DAE.common_float_or_param_type reflectivity;
          public DAE.common_transparent_type transparent;
          public DAE.common_float_or_param_type transparency;
          public DAE.common_float_or_param_type index_of_refraction;
        }

        public class _phong
        {
          public DAE.common_color_or_texture_type emission;
          public DAE.common_color_or_texture_type ambient;
          public DAE.common_color_or_texture_type diffuse;
          public DAE.common_color_or_texture_type specular;
          public DAE.common_float_or_param_type shininess;
          public DAE.common_color_or_texture_type reflective;
          public DAE.common_float_or_param_type reflectivity;
          public DAE.common_transparent_type transparent;
          public DAE.common_float_or_param_type transparency;
          public DAE.common_float_or_param_type index_of_refraction;
        }
      }
    }

    public class skin
    {
      [XmlAttribute]
      public string source;
      public DAE.float4x4 bind_shape_matrix;
      [XmlElement("source")]
      public List<DAE.source> _source;
      public DAE.skin._joints joints;
      public DAE.skin._vertex_weights vertex_weights;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public skin()
      {
        this._source = new List<DAE.source>();
        this.extra = new List<DAE.extra>();
      }

      public class _joints
      {
        [XmlElement("input")]
        public List<DAE.InputLocal> input;
        [XmlElement("extra")]
        public List<DAE.extra> extra;

        public _joints()
        {
          this.input = new List<DAE.InputLocal>();
          this.extra = new List<DAE.extra>();
        }
      }

      public class _vertex_weights
      {
        [XmlAttribute]
        public ulong count;
        [XmlElement("input")]
        public List<DAE.InputLocalOffset> input;
        public DAE.skin._vertex_weights._vcount vount;
        public DAE.skin._vertex_weights._v v;
        [XmlElement("extra")]
        public List<DAE.extra> extra;

        public _vertex_weights()
        {
          this.input = new List<DAE.InputLocalOffset>();
          this.extra = new List<DAE.extra>();
        }

        public class _vcount
        {
          public List<ulong> content;

          public _vcount()
          {
            this.content = new List<ulong>();
          }

          public XmlSchema GetSchema()
          {
            throw new NotImplementedException();
          }

          public void ReadXml(XmlReader reader)
          {
            string str = reader.ReadInnerXml();
            char[] separator = new char[1]{ ' ' };
            foreach (string s in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
              this.content.Add(ulong.Parse(s));
          }

          public void WriteXml(XmlWriter writer)
          {
            string data = "";
            foreach (ulong num in this.content)
              data = data + (object) num + " ";
            data.Trim();
            writer.WriteRaw(data);
          }
        }

        public class _v
        {
          public List<ulong> content;

          public _v()
          {
            this.content = new List<ulong>();
          }

          public XmlSchema GetSchema()
          {
            throw new NotImplementedException();
          }

          public void ReadXml(XmlReader reader)
          {
            string str = reader.ReadInnerXml();
            char[] separator = new char[1]{ ' ' };
            foreach (string s in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
              this.content.Add(ulong.Parse(s));
          }

          public void WriteXml(XmlWriter writer)
          {
            string data = "";
            foreach (ulong num in this.content)
              data = data + (object) num + " ";
            data.Trim();
            writer.WriteRaw(data);
          }
        }
      }
    }

    public class source
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      public DAE.source._Name_array Name_array;
      public DAE.source._float_array float_array;
      [XmlElement("technique_common")]
      public DAE.source._technique_common technique_common;
      [XmlElement("technique")]
      public List<DAE.technique> technique;

      public source()
      {
        this.technique = new List<DAE.technique>();
      }

      public class _Name_array : IXmlSerializable
      {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public ulong count;
        public List<string> content;

        public _Name_array()
        {
          this.content = new List<string>();
        }

        public XmlSchema GetSchema()
        {
          throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
          this.id = reader.GetAttribute("id");
          this.name = reader.GetAttribute("name");
          string attribute = reader.GetAttribute("count");
          if (attribute != null)
            this.count = ulong.Parse(attribute);
          this.content.AddRange((IEnumerable<string>) reader.ReadInnerXml().Split(new char[1]
          {
            ' '
          }, StringSplitOptions.RemoveEmptyEntries));
        }

        public void WriteXml(XmlWriter writer)
        {
          if (this.id != null && this.id != "")
            writer.WriteAttributeString("id", this.id);
          if (this.name != null && this.name != "")
            writer.WriteAttributeString("name", this.name);
          writer.WriteAttributeString("count", this.count.ToString());
          if (this.content.Count == 0)
            return;
          string data = "";
          foreach (string str in this.content)
            data = data + str + " ";
          data.Trim();
          writer.WriteRaw(data);
        }
      }

      public class _float_array : IXmlSerializable
      {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public ulong count;
        [XmlAttribute]
        public short digits;
        [XmlAttribute]
        public short magnitude;
        public List<double> content;

        public _float_array()
        {
          this.content = new List<double>();
        }

        public XmlSchema GetSchema()
        {
          throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
          this.id = reader.GetAttribute("id");
          this.name = reader.GetAttribute("name");
          string attribute1 = reader.GetAttribute("count");
          if (attribute1 != null)
            this.count = ulong.Parse(attribute1);
          string attribute2 = reader.GetAttribute("digits");
          if (attribute2 != null)
            this.digits = short.Parse(attribute2);
          string attribute3 = reader.GetAttribute("magnitude");
          if (attribute3 != null)
            this.magnitude = short.Parse(attribute3);
          string str = reader.ReadInnerXml().Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
          char[] separator = new char[1]{ ' ' };
          foreach (string s in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            this.content.Add(double.Parse(s));
        }

        public void WriteXml(XmlWriter writer)
        {
          this.count = (ulong) this.content.Count;
          if (this.id != null && this.id != "")
            writer.WriteAttributeString("id", this.id);
          if (this.name != null && this.name != "")
            writer.WriteAttributeString("name", this.name);
          writer.WriteAttributeString("count", this.count.ToString());
          if (this.digits != (short) 0)
            writer.WriteAttributeString("digits", this.digits.ToString());
          if (this.magnitude != (short) 0)
            writer.WriteAttributeString("magnitude", this.magnitude.ToString());
          if (this.content.Count == 0)
            return;
          string str = "";
          foreach (double num in this.content)
            str = str + (object) num + " ";
          str.Trim();
          string data = str.Replace(",", ".");
          writer.WriteRaw(data);
        }
      }

      [XmlType("technique_commonB")]
      public class _technique_common
      {
        public DAE.accessor accessor;
      }
    }

    public class technique
    {
      [XmlAttribute]
      public string profile;
    }

    public class triangles
    {
      [XmlAttribute]
      public string name;
      [XmlAttribute]
      public ulong count;
      [XmlAttribute]
      public string material;
      [XmlElement("input")]
      public List<DAE.InputLocalOffset> input;
      [XmlElement("p")]
      public List<DAE.p> p;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public triangles()
      {
        this.input = new List<DAE.InputLocalOffset>();
        this.p = new List<DAE.p>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class vertices
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      [XmlElement("input")]
      public List<DAE.InputLocal> input;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public vertices()
      {
        this.input = new List<DAE.InputLocal>();
        this.extra = new List<DAE.extra>();
      }
    }

    public class visual_scene
    {
      [XmlAttribute]
      public string id;
      [XmlAttribute]
      public string name;
      public DAE.asset asset;
      [XmlElement("node")]
      public List<DAE.node> node;
      [XmlElement("evaluate_scene")]
      public List<DAE.visual_scene._evaluate_scene> evaluate_scene;
      [XmlElement("extra")]
      public List<DAE.extra> extra;

      public visual_scene()
      {
        this.node = new List<DAE.node>();
        this.evaluate_scene = new List<DAE.visual_scene._evaluate_scene>();
        this.extra = new List<DAE.extra>();
      }

      public visual_scene(string id)
        : this()
      {
        this.id = id;
      }

      public class _evaluate_scene
      {
        [XmlAttribute]
        public string name;
        public List<DAE.visual_scene._evaluate_scene._render> render;

        public _evaluate_scene()
        {
          this.render = new List<DAE.visual_scene._evaluate_scene._render>();
        }

        public class _render
        {
          [XmlAttribute]
          public string camera_node;
          [XmlElement("layer")]
          public List<string> layer;
          public DAE.instance_effect instance_effect;

          public _render()
          {
            this.layer = new List<string>();
          }
        }
      }
    }

    public enum fx_modifier_enum_common
    {
      CONST,
      UNIFORM,
      VARYING,
      STATIC,
      VOLATILE,
      EXTERN,
      SHARED,
    }

    public enum fx_opaque_enum
    {
      A_ONE,
      RGB_ZERO,
    }

    public enum fx_sampler_filter_common
    {
      NONE,
      NEAREST,
      LINEAR,
      NEAREST_MIPMAP_NEAREST,
      LINEAR_MIPMAP_NEAREST,
      NEAREST_MIPMAP_LINEAR,
      LINEAR_MIPMAP_LINEAR,
    }

    public enum fx_sampler_wrap_common
    {
      NONE,
      WRAP,
      MIRROR,
      CLAMP,
      BORDER,
    }

    public enum fx_surface_face_enum
    {
      POSITIVE_X,
      NEGATIVE_X,
      POSITIVE_Y,
      NEGATIVE_Y,
      POSITIVE_Z,
      NEGATIVE_Z,
    }

    public enum fx_surface_format_hint_channels_enum
    {
      RGB,
      RGBA,
      L,
      LA,
      D,
      XYZ,
      XYZW,
    }

    public enum fx_surface_format_hint_precision_enum
    {
      LOW,
      MID,
      HIGH,
    }

    public enum fx_surface_format_hint_range_enum
    {
      SNORM,
      UNORM,
      SINT,
      UINT,
      FLOAT,
    }

    public enum fx_surface_format_hint_option_enum
    {
      SRGB_GAMMA,
      NORMALIZED3,
      NORMALIZED4,
      COMPRESSABLE,
    }

    public enum fx_surface_type_enum
    {
      UNTYPED,
      [XmlEnum("1D")] _1D,
      [XmlEnum("2D")] _2D,
      [XmlEnum("3D")] _3D,
      RECT,
      CUBE,
      DEPTH,
    }

    public enum NodeType
    {
      JOINT,
      NODE,
    }

    public enum UpAxisType
    {
      X_UP,
      Y_UP,
      Z_UP,
    }
  }
}
