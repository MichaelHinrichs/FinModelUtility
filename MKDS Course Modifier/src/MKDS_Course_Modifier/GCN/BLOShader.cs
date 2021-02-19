// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BLOShader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tao.OpenGl;

namespace MKDS_Course_Modifier.GCN
{
  public class BLOShader
  {
    public int program = 0;
    public int fragment_shader = 0;
    public int vertex_shader = 0;
    public BLO.MAT1.TevStageProps[] TevStages;
    public int[] Textures;
    private float[][] g_color_registers;
    private float[][] g_color_consts;
    private float[] MatColor;
    private byte color_matsrc;
    private byte alpha_matsrc;

    public BLOShader(BLO.MAT1.MaterialEntry Material, BLO.MAT1 MAT1, int[] textures)
    {
      this.color_matsrc = (byte) 1;
      this.alpha_matsrc = (byte) 1;
      this.MatColor = new float[4]
      {
        (float) MAT1.Color1[(int) Material.MatColorID].R / (float) byte.MaxValue,
        (float) MAT1.Color1[(int) Material.MatColorID].G / (float) byte.MaxValue,
        (float) MAT1.Color1[(int) Material.MatColorID].B / (float) byte.MaxValue,
        (float) MAT1.Color1[(int) Material.MatColorID].A / (float) byte.MaxValue
      };
      List<BLO.MAT1.TevStageProps> source = new List<BLO.MAT1.TevStageProps>();
      int index = 0;
      foreach (short num in Material.UnknownIndices2)
      {
        if (num == (short) -1)
        {
          ++index;
        }
        else
        {
          source.Add(MAT1.TevStages[(int) num]);
          source.Last<BLO.MAT1.TevStageProps>().alpha_constant_sel = Material.ConstAlphaSel[index];
          source.Last<BLO.MAT1.TevStageProps>().color_constant_sel = Material.ConstColorSel[index];
          try
          {
            source.Last<BLO.MAT1.TevStageProps>().texcoord = MAT1.Tevorders[(int) Material.TevOrderInfo[index]].TexcoordID;
            source.Last<BLO.MAT1.TevStageProps>().texmap = MAT1.Tevorders[(int) Material.TevOrderInfo[index]].TexMap;
          }
          catch
          {
          }
          ++index;
        }
      }
      this.TevStages = source.ToArray();
      this.Textures = textures;
      this.g_color_registers = new float[3][];
      this.g_color_registers[0] = new float[4]
      {
        (float) MAT1.ColorS10[(int) Material.ColorS10[0]].R / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[0]].G / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[0]].B / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[0]].A / (float) byte.MaxValue
      };
      this.g_color_registers[1] = new float[4]
      {
        (float) MAT1.ColorS10[(int) Material.ColorS10[1]].R / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[1]].G / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[1]].B / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[1]].A / (float) byte.MaxValue
      };
      this.g_color_registers[2] = new float[4]
      {
        (float) MAT1.ColorS10[(int) Material.ColorS10[2]].R / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[2]].G / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[2]].B / (float) byte.MaxValue,
        (float) MAT1.ColorS10[(int) Material.ColorS10[2]].A / (float) byte.MaxValue
      };
      this.g_color_consts = new float[4][];
      this.g_color_consts[0] = new float[4]
      {
        (float) MAT1.Color3[(int) Material.Color3[0]].R / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[0]].G / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[0]].B / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[0]].A / (float) byte.MaxValue
      };
      this.g_color_consts[1] = new float[4]
      {
        (float) MAT1.Color3[(int) Material.Color3[1]].R / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[1]].G / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[1]].B / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[1]].A / (float) byte.MaxValue
      };
      this.g_color_consts[2] = new float[4]
      {
        (float) MAT1.Color3[(int) Material.Color3[2]].R / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[2]].G / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[2]].B / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[2]].A / (float) byte.MaxValue
      };
      this.g_color_consts[3] = new float[4]
      {
        (float) MAT1.Color3[(int) Material.Color3[3]].R / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[3]].G / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[3]].B / (float) byte.MaxValue,
        (float) MAT1.Color3[(int) Material.Color3[3]].A / (float) byte.MaxValue
      };
    }

    public void Enable()
    {
      Gl.glUseProgram(this.program);
      for (int index = 0; index < 3; ++index)
        Gl.glUniform4f(Gl.glGetUniformLocation(this.program, "color_register" + (object) index), this.g_color_registers[index][0], this.g_color_registers[index][1], this.g_color_registers[index][2], this.g_color_registers[index][3]);
      for (int index = 0; index < 1; ++index)
        Gl.glUniform4f(Gl.glGetUniformLocation(this.program, "matColor"), this.MatColor[0], this.MatColor[1], this.MatColor[2], this.MatColor[3]);
      for (int index = 0; index < 4; ++index)
        Gl.glUniform4f(Gl.glGetUniformLocation(this.program, "color_const" + (object) index), this.g_color_consts[index][0], this.g_color_consts[index][1], this.g_color_consts[index][2], this.g_color_consts[index][3]);
    }

    public void Disable()
    {
    }

    public void Compile()
    {
      uint length = (uint) this.Textures.Length;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("void main()");
      stringBuilder.AppendLine("{");
      stringBuilder.AppendLine("gl_FrontColor = gl_Color;");
      stringBuilder.AppendLine("gl_BackColor = gl_Color;");
      for (uint index = 0; (int) index != (int) length; ++index)
        stringBuilder.AppendFormat("gl_TexCoord[{0}] = gl_TextureMatrix[{0}] * gl_MultiTexCoord{0};\n", (object) index);
      stringBuilder.AppendLine("gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;");
      stringBuilder.AppendLine("}");
      this.vertex_shader = Gl.glCreateShader(35633);
      string str1 = stringBuilder.ToString();
      Gl.glShaderSource(this.vertex_shader, 1, new string[1]
      {
        str1
      }, new int[1]{ str1.Length });
      Gl.glCompileShader(this.vertex_shader);
      StringBuilder frag_ss = new StringBuilder();
      for (uint index = 0; (int) index != (int) length; ++index)
        frag_ss.AppendFormat("uniform sampler2D textures{0};\n", (object) index);
      for (uint index = 0; index < 3U; ++index)
        frag_ss.AppendFormat("uniform vec4 color_register{0};\n", (object) index);
      frag_ss.AppendFormat("uniform vec4 matColor;\n");
      for (uint index = 0; index < 4U; ++index)
        frag_ss.AppendFormat("uniform vec4 color_const{0};\n", (object) index);
      frag_ss.AppendLine("vec4 color_constant;");
      frag_ss.AppendLine("vec4 rasColor;");
      frag_ss.AppendLine("void main()");
      frag_ss.AppendLine("{");
      string[] strArray1 = new string[2]
      {
        "matColor",
        "gl_Color"
      };
      frag_ss.AppendFormat("rasColor.rgb = {0}.rgb;\n", (object) strArray1[(int) this.color_matsrc]);
      frag_ss.AppendFormat("rasColor.a = {0}.a;\n", (object) strArray1[(int) this.alpha_matsrc]);
      frag_ss.AppendLine("vec4 color_previous;");
      frag_ss.AppendLine("vec4 color_texture;");
      for (uint index = 0; index < 3U; ++index)
        frag_ss.AppendFormat("vec4 color_registers{0} = color_register{0};\n", (object) index);
      for (uint index = 0; index < 4U; ++index)
        frag_ss.AppendFormat("vec4 color_consts{0} = color_const{0};\n", (object) index);
      string[] strArray2 = new string[16]
      {
        "color_previous.rgb",
        "color_previous.aaa",
        "color_registers0.rgb",
        "color_registers0.aaa",
        "color_registers1.rgb",
        "color_registers1.aaa",
        "color_registers2.rgb",
        "color_registers2.aaa",
        "color_texture.rgb",
        "color_texture.aaa",
        "rasColor.rgb",
        "rasColor.aaa",
        "vec3(1.0)",
        "vec3(0.5)",
        "color_constant.rgb",
        "vec3(0.0)"
      };
      string[] strArray3 = new string[8]
      {
        "color_previous.a",
        "color_registers0.a",
        "color_registers1.a",
        "color_registers2.a",
        "color_texture.a",
        "rasColor.a",
        "color_constant.a",
        "0.0"
      };
      string[] strArray4 = new string[4]
      {
        "color_previous",
        "color_registers0",
        "color_registers1",
        "color_registers2"
      };
      frag_ss.AppendLine("const vec3 comp16 = vec3(1.0, 255.0, 0.0), comp24 = vec3(1.0, 255.0, 255.0 * 255.0);");
      if (this.TevStages.Length != 0 && this.TevStages[0] != null)
      {
        foreach (BLO.MAT1.TevStageProps tevStage in this.TevStages)
        {
          if ((uint) tevStage.texcoord < length)
            frag_ss.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{1}].st);\n", (object) (int) tevStage.texcoord, (object) (int) tevStage.texcoord);
          string str2 = "";
          if (tevStage.color_constant_sel <= (byte) 7)
          {
            switch (tevStage.color_constant_sel)
            {
              case 0:
                str2 = "vec3(1.0)";
                break;
              case 1:
                str2 = "vec3(0.875)";
                break;
              case 2:
                str2 = "vec3(0.75)";
                break;
              case 3:
                str2 = "vec3(0.625)";
                break;
              case 4:
                str2 = "vec3(0.5)";
                break;
              case 5:
                str2 = "vec3(0.375)";
                break;
              case 6:
                str2 = "vec3(0.25)";
                break;
              case 7:
                str2 = "vec3(0.125)";
                break;
            }
          }
          else if (tevStage.color_constant_sel < (byte) 12)
          {
            str2 = "vec3(1.0)";
          }
          else
          {
            string[] strArray5 = new string[4]
            {
              "color_consts0",
              "color_consts1",
              "color_consts2",
              "color_consts3"
            };
            string[] strArray6 = new string[5]
            {
              ".rgb",
              ".rrr",
              ".ggg",
              ".bbb",
              ".aaa"
            };
            str2 = strArray5[((int) tevStage.color_constant_sel - 12) % 4] + strArray6[((int) tevStage.color_constant_sel - 12) / 4];
          }
          string str3 = "";
          if (tevStage.alpha_constant_sel <= (byte) 7)
          {
            switch (tevStage.alpha_constant_sel)
            {
              case 0:
                str3 = "vec3(1.0)";
                break;
              case 1:
                str3 = "vec3(0.875)";
                break;
              case 2:
                str3 = "vec3(0.75)";
                break;
              case 3:
                str3 = "vec3(0.625)";
                break;
              case 4:
                str3 = "vec3(0.5)";
                break;
              case 5:
                str3 = "vec3(0.375)";
                break;
              case 6:
                str3 = "vec3(0.25)";
                break;
              case 7:
                str3 = "vec3(0.125)";
                break;
            }
          }
          else if (tevStage.alpha_constant_sel < (byte) 16)
          {
            str2 = "1.0";
          }
          else
          {
            string[] strArray5 = new string[4]
            {
              "color_consts0",
              "color_consts1",
              "color_consts2",
              "color_consts3"
            };
            string[] strArray6 = new string[4]
            {
              ".r",
              ".g",
              ".b",
              ".a"
            };
            str3 = strArray5[((int) tevStage.alpha_constant_sel - 16) % 4] + strArray6[((int) tevStage.alpha_constant_sel - 16) / 4];
          }
          frag_ss.AppendFormat("color_constant = vec4({0}, {1});\n", (object) str2, (object) str3);
          frag_ss.AppendLine("{");
          frag_ss.AppendFormat("vec4 a = vec4({0}, {1});\n", (object) strArray2[(int) tevStage.color_a], (object) strArray3[(int) tevStage.alpha_a]);
          frag_ss.AppendFormat("vec4 b = vec4({0}, {1});\n", (object) strArray2[(int) tevStage.color_b], (object) strArray3[(int) tevStage.alpha_b]);
          frag_ss.AppendFormat("vec4 c = vec4({0}, {1});\n", (object) strArray2[(int) tevStage.color_c], (object) strArray3[(int) tevStage.alpha_c]);
          frag_ss.AppendFormat("vec4 d = vec4({0}, {1});\n", (object) strArray2[(int) tevStage.color_d], (object) strArray3[(int) tevStage.alpha_d]);
          frag_ss.AppendLine("vec4 result;");
          if ((int) tevStage.color_op != (int) tevStage.alpha_op)
          {
            this.write_tevop(tevStage.color_op, ".rgb", ref frag_ss);
            this.write_tevop(tevStage.alpha_op, ".a", ref frag_ss);
          }
          else
            this.write_tevop(tevStage.color_op, "", ref frag_ss);
          string[] strArray7 = new string[3]
          {
            "+0",
            "+0.5",
            "-0.5"
          };
          string[] strArray8 = new string[4]
          {
            "*1",
            "*2",
            "*4",
            "*0.5"
          };
          if (tevStage.color_op < (byte) 2)
            frag_ss.AppendFormat("{0}.rgb = (result.rgb{1}){2};\n", (object) strArray4[(int) tevStage.color_regid], (object) strArray7[(int) tevStage.color_bias], (object) strArray8[(int) tevStage.color_scale]);
          else
            frag_ss.AppendFormat("{0}.rgb = result.rgb;\n", (object) strArray4[(int) tevStage.color_regid]);
          if (tevStage.alpha_op < (byte) 2)
            frag_ss.AppendFormat("{0}.a = (result.a{1}){2};\n", (object) strArray4[(int) tevStage.alpha_regid], (object) strArray7[(int) tevStage.alpha_bias], (object) strArray8[(int) tevStage.alpha_scale]);
          else
            frag_ss.AppendFormat("{0}.a = result.a;\n", (object) strArray4[(int) tevStage.alpha_regid]);
          if (tevStage.color_clamp && tevStage.color_op < (byte) 2)
            frag_ss.AppendFormat("{0}.rgb = clamp({0}.rgb,vec3(0.0, 0.0, 0.0),vec3(1.0, 1.0, 1.0));\n", (object) strArray4[(int) tevStage.color_regid]);
          if (tevStage.alpha_clamp && tevStage.alpha_op < (byte) 2)
            frag_ss.AppendFormat("{0}.a = clamp({0}.a, 0.0, 1.0);\n", (object) strArray4[(int) tevStage.alpha_regid]);
          frag_ss.AppendLine("}");
        }
      }
      else
      {
        for (int index = 0; index < 1; ++index)
        {
          if ((long) index < (long) length)
            frag_ss.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{0}].st);\n", (object) index);
          frag_ss.AppendLine("{");
          frag_ss.AppendFormat("vec4 a = vec4({0}, {1});\n", (object) strArray2[2], (object) strArray3[1]);
          frag_ss.AppendFormat("vec4 b = vec4({0}, {1});\n", (object) strArray2[4], (object) strArray3[2]);
          frag_ss.AppendFormat("vec4 c = vec4({0}, {1});\n", (object) strArray2[8], (object) strArray3[4]);
          frag_ss.AppendFormat("vec4 d = vec4({0}, {1});\n", (object) strArray2[15], (object) strArray3[7]);
          frag_ss.AppendLine("vec4 result;");
          this.write_tevop((byte) 0, "", ref frag_ss);
          frag_ss.AppendFormat("{0}.rgb = result.rgb;\n", (object) strArray4[0]);
          frag_ss.AppendFormat("{0}.a = result.a;\n", (object) strArray4[0]);
          frag_ss.AppendLine("}");
          if ((long) index < (long) length)
            frag_ss.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{0}].st);\n", (object) index);
          frag_ss.AppendLine("{");
          frag_ss.AppendFormat("vec4 a = vec4({0}, {1});\n", (object) strArray2[15], (object) strArray3[7]);
          frag_ss.AppendFormat("vec4 b = vec4({0}, {1});\n", (object) strArray2[0], (object) strArray3[0]);
          frag_ss.AppendFormat("vec4 c = vec4({0}, {1});\n", (object) strArray2[10], (object) strArray3[5]);
          frag_ss.AppendFormat("vec4 d = vec4({0}, {1});\n", (object) strArray2[15], (object) strArray3[7]);
          frag_ss.AppendLine("vec4 result;");
          this.write_tevop((byte) 0, "", ref frag_ss);
          frag_ss.AppendFormat("{0}.rgb = result.rgb;\n", (object) strArray4[0]);
          frag_ss.AppendFormat("{0}.a = result.a;\n", (object) strArray4[0]);
          frag_ss.AppendLine("}");
        }
      }
      frag_ss.AppendLine("gl_FragColor = color_previous;");
      frag_ss.AppendLine("}");
      this.fragment_shader = Gl.glCreateShader(35632);
      string str4 = frag_ss.ToString();
      Gl.glShaderSource(this.fragment_shader, 1, new string[1]
      {
        str4
      }, new int[1]{ str4.Length });
      Gl.glCompileShader(this.fragment_shader);
      int params1 = 0;
      int params2 = 0;
      Gl.glGetShaderiv(this.vertex_shader, 35713, out params1);
      Gl.glGetShaderiv(this.fragment_shader, 35713, out params2);
      if (params1 != 0)
        ;
      if (params2 != 0)
        ;
      this.program = Gl.glCreateProgram();
      Gl.glAttachShader(this.program, this.vertex_shader);
      Gl.glAttachShader(this.program, this.fragment_shader);
      Gl.glLinkProgram(this.program);
      int params3;
      Gl.glGetProgramiv(this.program, 35714, out params3);
      if (params3 != 0)
        ;
      Gl.glUseProgram(this.program);
      for (uint index = 0; (int) index != (int) length; ++index)
        Gl.glUniform1i(Gl.glGetUniformLocation(this.program, "textures" + (object) index), (int) index);
      for (int index = 0; index < 3; ++index)
        Gl.glUniform4f(Gl.glGetUniformLocation(this.program, "color_register" + (object) index), this.g_color_registers[index][0], this.g_color_registers[index][1], this.g_color_registers[index][2], this.g_color_registers[index][3]);
      for (int index = 0; index < 1; ++index)
        Gl.glUniform4f(Gl.glGetUniformLocation(this.program, "matColor"), this.MatColor[0], this.MatColor[1], this.MatColor[2], this.MatColor[3]);
      for (int index = 0; index < 4; ++index)
        Gl.glUniform4f(Gl.glGetUniformLocation(this.program, "color_const" + (object) index), this.g_color_consts[index][0], this.g_color_consts[index][1], this.g_color_consts[index][2], this.g_color_consts[index][3]);
      Gl.glGetProgramInfoLog(this.program, 10240, (int[]) null, new StringBuilder());
    }

    private void write_tevop(byte tevop, string swiz, ref StringBuilder frag_ss)
    {
      string str1 = " ? c : vec4(0.0))" + swiz;
      if (tevop < (byte) 14)
        frag_ss.AppendFormat("result{0} = d{0} {1} ", (object) swiz, (object) (char) (tevop == (byte) 1 ? 45 : 43));
      string str2 = ((int) tevop & 1) != 0 ? "==" : ">";
      switch (tevop)
      {
        case 0:
        case 1:
          frag_ss.AppendFormat("mix(a{0}, b{0}, c{0})", (object) swiz);
          break;
        case 8:
        case 9:
          frag_ss.AppendFormat("((a.r {0} b.r){1}", (object) str2, (object) str1);
          int num1 = (int) MessageBox.Show(tevop.ToString());
          break;
        case 10:
        case 11:
          frag_ss.AppendFormat("((dot(a.gr, comp16) {0} dot(b.gr, comp16)){1}", (object) str2, (object) str1);
          int num2 = (int) MessageBox.Show(tevop.ToString());
          break;
        case 12:
        case 13:
          frag_ss.AppendFormat("((dot(a.bgr, comp24) {0} dot(b.bgr, comp24)){1}", (object) str2, (object) str1);
          int num3 = (int) MessageBox.Show(tevop.ToString());
          break;
        case 14:
        case 15:
          if (swiz == ".rgb")
          {
            frag_ss.AppendFormat("result.r = d.r + ");
            frag_ss.AppendFormat("((a.r {0} b.r) ? c.r : vec4(0.0).r);\n", (object) str2);
            frag_ss.AppendFormat("result.g = d.g + ");
            frag_ss.AppendFormat("((a.g {0} b.g) ? c.g : vec4(0.0).g);\n", (object) str2);
            frag_ss.AppendFormat("result.b = d.b + ");
            frag_ss.AppendFormat("((a.b {0} b.b) ? c.b : vec4(0.0).b);\n", (object) str2);
            break;
          }
          if (swiz == ".a")
          {
            frag_ss.AppendFormat("result.a = d.a + ");
            frag_ss.AppendFormat("((a.a {0} b.a) ? c.a : vec4(0.0).a);", (object) str2);
            break;
          }
          frag_ss.AppendFormat("result.r = d.r + ");
          frag_ss.AppendFormat("((a.r {0} b.r) ? c.r : vec4(0.0).r);\n", (object) str2);
          frag_ss.AppendFormat("result.g = d.g + ");
          frag_ss.AppendFormat("((a.g {0} b.g) ? c.g : vec4(0.0).g);\n", (object) str2);
          frag_ss.AppendFormat("result.b = d.b + ");
          frag_ss.AppendFormat("((a.b {0} b.b) ? c.b : vec4(0.0).b);\n", (object) str2);
          frag_ss.AppendFormat("result.a = d.a + ");
          frag_ss.AppendFormat("((a.a {0} b.a) ? c.a : vec4(0.0).a);", (object) str2);
          break;
        default:
          frag_ss.AppendFormat("mix(a{0}, b{0}, c{0})", (object) swiz);
          int num4 = (int) MessageBox.Show(tevop.ToString());
          break;
      }
      frag_ss.AppendLine(";");
    }
  }
}
