// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BMDShader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Text;
using Tao.OpenGl;

namespace bmd.GCN
{
  public class BMDShader
  {
    public int program = 0;
    public int fragment_shader = 0;
    public int vertex_shader = 0;
    public BMD.MAT3Section.TevStageProps[] TevStages;
    public int[] Textures;
    private float[][] g_color_registers;
    private float[][] g_color_consts;
    private float[] MatColor;
    private byte color_matsrc;
    private byte alpha_matsrc;
    private BMD.MAT3Section.AlphaCompare AlphaCompare;

    public BMDShader(
      BMD.MAT3Section.TevStageProps[] Tevs,
      int[] textures,
      System.Drawing.Color[] S10,
      System.Drawing.Color[] Const,
      byte color_matsrc,
      byte alpha_matsrc,
      System.Drawing.Color MatColor,
      BMD.MAT3Section.AlphaCompare alpha)
    {
      this.color_matsrc = color_matsrc;
      this.alpha_matsrc = alpha_matsrc;
      this.AlphaCompare = alpha;
      this.MatColor = new float[4]
      {
        (float) MatColor.R / (float) byte.MaxValue,
        (float) MatColor.G / (float) byte.MaxValue,
        (float) MatColor.B / (float) byte.MaxValue,
        (float) MatColor.A / (float) byte.MaxValue
      };
      this.TevStages = Tevs;
      this.Textures = textures;
      this.g_color_registers = new float[3][];
      this.g_color_registers[0] = new float[4]
      {
        (float) S10[0].R / (float) byte.MaxValue,
        (float) S10[0].G / (float) byte.MaxValue,
        (float) S10[0].B / (float) byte.MaxValue,
        (float) S10[0].A / (float) byte.MaxValue
      };
      this.g_color_registers[1] = new float[4]
      {
        (float) S10[1].R / (float) byte.MaxValue,
        (float) S10[1].G / (float) byte.MaxValue,
        (float) S10[1].B / (float) byte.MaxValue,
        (float) S10[1].A / (float) byte.MaxValue
      };
      this.g_color_registers[2] = new float[4]
      {
        (float) S10[2].R / (float) byte.MaxValue,
        (float) S10[2].G / (float) byte.MaxValue,
        (float) S10[2].B / (float) byte.MaxValue,
        (float) S10[2].A / (float) byte.MaxValue
      };
      this.g_color_consts = new float[4][];
      this.g_color_consts[0] = new float[4]
      {
        (float) Const[0].R / (float) byte.MaxValue,
        (float) Const[0].G / (float) byte.MaxValue,
        (float) Const[0].B / (float) byte.MaxValue,
        (float) Const[0].A / (float) byte.MaxValue
      };
      this.g_color_consts[1] = new float[4]
      {
        (float) Const[1].R / (float) byte.MaxValue,
        (float) Const[1].G / (float) byte.MaxValue,
        (float) Const[1].B / (float) byte.MaxValue,
        (float) Const[1].A / (float) byte.MaxValue
      };
      this.g_color_consts[2] = new float[4]
      {
        (float) Const[2].R / (float) byte.MaxValue,
        (float) Const[2].G / (float) byte.MaxValue,
        (float) Const[2].B / (float) byte.MaxValue,
        (float) Const[2].A / (float) byte.MaxValue
      };
      this.g_color_consts[3] = new float[4]
      {
        (float) Const[3].R / (float) byte.MaxValue,
        (float) Const[3].G / (float) byte.MaxValue,
        (float) Const[3].B / (float) byte.MaxValue,
        (float) Const[3].A / (float) byte.MaxValue
      };
    }

    public void RefreshColors(System.Drawing.Color[] S10, System.Drawing.Color[] Const, System.Drawing.Color MatColor)
    {
      this.MatColor = new float[4]
      {
        (float) MatColor.R / (float) byte.MaxValue,
        (float) MatColor.G / (float) byte.MaxValue,
        (float) MatColor.B / (float) byte.MaxValue,
        (float) MatColor.A / (float) byte.MaxValue
      };
      this.g_color_registers = new float[3][];
      this.g_color_registers[0] = new float[4]
      {
        (float) S10[0].R / (float) byte.MaxValue,
        (float) S10[0].G / (float) byte.MaxValue,
        (float) S10[0].B / (float) byte.MaxValue,
        (float) S10[0].A / (float) byte.MaxValue
      };
      this.g_color_registers[1] = new float[4]
      {
        (float) S10[1].R / (float) byte.MaxValue,
        (float) S10[1].G / (float) byte.MaxValue,
        (float) S10[1].B / (float) byte.MaxValue,
        (float) S10[1].A / (float) byte.MaxValue
      };
      this.g_color_registers[2] = new float[4]
      {
        (float) S10[2].R / (float) byte.MaxValue,
        (float) S10[2].G / (float) byte.MaxValue,
        (float) S10[2].B / (float) byte.MaxValue,
        (float) S10[2].A / (float) byte.MaxValue
      };
      this.g_color_consts = new float[4][];
      this.g_color_consts[0] = new float[4]
      {
        (float) Const[0].R / (float) byte.MaxValue,
        (float) Const[0].G / (float) byte.MaxValue,
        (float) Const[0].B / (float) byte.MaxValue,
        (float) Const[0].A / (float) byte.MaxValue
      };
      this.g_color_consts[1] = new float[4]
      {
        (float) Const[1].R / (float) byte.MaxValue,
        (float) Const[1].G / (float) byte.MaxValue,
        (float) Const[1].B / (float) byte.MaxValue,
        (float) Const[1].A / (float) byte.MaxValue
      };
      this.g_color_consts[2] = new float[4]
      {
        (float) Const[2].R / (float) byte.MaxValue,
        (float) Const[2].G / (float) byte.MaxValue,
        (float) Const[2].B / (float) byte.MaxValue,
        (float) Const[2].A / (float) byte.MaxValue
      };
      this.g_color_consts[3] = new float[4]
      {
        (float) Const[3].R / (float) byte.MaxValue,
        (float) Const[3].G / (float) byte.MaxValue,
        (float) Const[3].B / (float) byte.MaxValue,
        (float) Const[3].A / (float) byte.MaxValue
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
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine("void main()");
      stringBuilder1.AppendLine("{");
      stringBuilder1.AppendLine("gl_FrontColor = gl_Color;");
      stringBuilder1.AppendLine("gl_BackColor = gl_Color;");
      for (uint index = 0; (int) index != (int) length; ++index)
        stringBuilder1.AppendFormat("gl_TexCoord[{0}] = gl_TextureMatrix[{0}] * gl_MultiTexCoord{0};\n", (object) index);
      stringBuilder1.AppendLine("gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;");
      stringBuilder1.AppendLine("}");
      this.vertex_shader = Gl.glCreateShader(35633);
      string str1 = stringBuilder1.ToString();
      Gl.glShaderSource(this.vertex_shader, 1, new string[1]
      {
        str1
      }, new int[1]{ str1.Length });
      Gl.glCompileShader(this.vertex_shader);
      StringBuilder stringBuilder2 = new StringBuilder();
      for (uint index = 0; (int) index != (int) length; ++index)
        stringBuilder2.AppendFormat("uniform sampler2D textures{0};\n", (object) index);
      for (uint index = 0; index < 3U; ++index)
        stringBuilder2.AppendFormat("uniform vec4 color_register{0};\n", (object) index);
      stringBuilder2.AppendFormat("uniform vec4 matColor;\n");
      for (uint index = 0; index < 4U; ++index)
        stringBuilder2.AppendFormat("uniform vec4 color_const{0};\n", (object) index);
      stringBuilder2.AppendLine("vec4 color_constant;");
      stringBuilder2.AppendLine("vec4 rasColor;");
      stringBuilder2.AppendLine("void main()");
      stringBuilder2.AppendLine("{");
      string[] strArray1 = new string[2]
      {
        "matColor",
        "gl_Color"
      };
      stringBuilder2.AppendFormat("rasColor.rgb = {0}.rgb;\n", (object) strArray1[(int) this.color_matsrc]);
      stringBuilder2.AppendFormat("rasColor.a = {0}.a;\n", (object) strArray1[(int) this.alpha_matsrc]);
      stringBuilder2.AppendLine("vec4 color_previous;");
      stringBuilder2.AppendLine("vec4 color_texture;");
      for (uint index = 0; index < 3U; ++index)
        stringBuilder2.AppendFormat("vec4 color_registers{0} = color_register{0};\n", (object) index);
      for (uint index = 0; index < 4U; ++index)
        stringBuilder2.AppendFormat("vec4 color_consts{0} = color_const{0};\n", (object) index);
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
      stringBuilder2.AppendLine("const vec3 comp16 = vec3(1.0, 255.0, 0.0), comp24 = vec3(1.0, 255.0, 255.0 * 255.0);");
      if (this.TevStages.Length != 0 && this.TevStages[0] != null)
      {
        foreach (BMD.MAT3Section.TevStageProps tevStage in this.TevStages)
        {
          if ((uint) tevStage.texcoord < length)
            stringBuilder2.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{1}].st);\n", (object) ((int) tevStage.texmap + 1), (object) (int) tevStage.texcoord);
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
          stringBuilder2.AppendFormat("color_constant = vec4({0}, {1});\n", (object) str2, (object) str3);
          stringBuilder2.AppendLine("{");
          stringBuilder2.AppendFormat("vec4 a = vec4({0}, {1});\n", (object) strArray2[(int) tevStage.color_a], (object) strArray3[(int) tevStage.alpha_a]);
          stringBuilder2.AppendFormat("vec4 b = vec4({0}, {1});\n", (object) strArray2[(int) tevStage.color_b], (object) strArray3[(int) tevStage.alpha_b]);
          stringBuilder2.AppendFormat("vec4 c = vec4({0}, {1});\n", (object) strArray2[(int) tevStage.color_c], (object) strArray3[(int) tevStage.alpha_c]);
          stringBuilder2.AppendFormat("vec4 d = vec4({0}, {1});\n", (object) strArray2[(int) tevStage.color_d], (object) strArray3[(int) tevStage.alpha_d]);
          stringBuilder2.AppendLine("vec4 result;");
          if ((int) tevStage.color_op != (int) tevStage.alpha_op)
          {
            this.write_tevop(tevStage.color_op, ".rgb", ref stringBuilder2);
            this.write_tevop(tevStage.alpha_op, ".a", ref stringBuilder2);
          }
          else
            this.write_tevop(tevStage.color_op, "", ref stringBuilder2);
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
            stringBuilder2.AppendFormat("{0}.rgb = (result.rgb{1}){2};\n", (object) strArray4[(int) tevStage.color_regid], (object) strArray7[(int) tevStage.color_bias], (object) strArray8[(int) tevStage.color_scale]);
          else
            stringBuilder2.AppendFormat("{0}.rgb = result.rgb;\n", (object) strArray4[(int) tevStage.color_regid]);
          if (tevStage.alpha_op < (byte) 2)
            stringBuilder2.AppendFormat("{0}.a = (result.a{1}){2};\n", (object) strArray4[(int) tevStage.alpha_regid], (object) strArray7[(int) tevStage.alpha_bias], (object) strArray8[(int) tevStage.alpha_scale]);
          else
            stringBuilder2.AppendFormat("{0}.a = result.a;\n", (object) strArray4[(int) tevStage.alpha_regid]);
          if (tevStage.color_clamp && tevStage.color_op < (byte) 2)
            stringBuilder2.AppendFormat("{0}.rgb = clamp({0}.rgb,vec3(0.0, 0.0, 0.0),vec3(1.0, 1.0, 1.0));\n", (object) strArray4[(int) tevStage.color_regid]);
          if (tevStage.alpha_clamp && tevStage.alpha_op < (byte) 2)
            stringBuilder2.AppendFormat("{0}.a = clamp({0}.a, 0.0, 1.0);\n", (object) strArray4[(int) tevStage.alpha_regid]);
          stringBuilder2.AppendLine("}");
        }
      }
      else
      {
        for (int index = 0; index < 1; ++index)
        {
          if ((long) index < (long) length)
            stringBuilder2.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{0}].st);\n", (object) index);
          stringBuilder2.AppendLine("{");
          stringBuilder2.AppendFormat("vec4 a = vec4({0}, {1});\n", (object) strArray2[2], (object) strArray3[1]);
          stringBuilder2.AppendFormat("vec4 b = vec4({0}, {1});\n", (object) strArray2[4], (object) strArray3[2]);
          stringBuilder2.AppendFormat("vec4 c = vec4({0}, {1});\n", (object) strArray2[8], (object) strArray3[4]);
          stringBuilder2.AppendFormat("vec4 d = vec4({0}, {1});\n", (object) strArray2[15], (object) strArray3[7]);
          stringBuilder2.AppendLine("vec4 result;");
          this.write_tevop((byte) 0, "", ref stringBuilder2);
          stringBuilder2.AppendFormat("{0}.rgb = result.rgb;\n", (object) strArray4[0]);
          stringBuilder2.AppendFormat("{0}.a = result.a;\n", (object) strArray4[0]);
          stringBuilder2.AppendLine("}");
          if ((long) index < (long) length)
            stringBuilder2.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{0}].st);\n", (object) index);
          stringBuilder2.AppendLine("{");
          stringBuilder2.AppendFormat("vec4 a = vec4({0}, {1});\n", (object) strArray2[15], (object) strArray3[7]);
          stringBuilder2.AppendFormat("vec4 b = vec4({0}, {1});\n", (object) strArray2[0], (object) strArray3[0]);
          stringBuilder2.AppendFormat("vec4 c = vec4({0}, {1});\n", (object) strArray2[10], (object) strArray3[5]);
          stringBuilder2.AppendFormat("vec4 d = vec4({0}, {1});\n", (object) strArray2[15], (object) strArray3[7]);
          stringBuilder2.AppendLine("vec4 result;");
          this.write_tevop((byte) 0, "", ref stringBuilder2);
          stringBuilder2.AppendFormat("{0}.rgb = result.rgb;\n", (object) strArray4[0]);
          stringBuilder2.AppendFormat("{0}.a = result.a;\n", (object) strArray4[0]);
          stringBuilder2.AppendLine("}");
        }
      }
      stringBuilder2.AppendLine("gl_FragColor = color_previous;");
      stringBuilder2.Append("bool a = ");
      this.Alpha_Compare((int) this.AlphaCompare.Ref0, (int) this.AlphaCompare.Func0, ref stringBuilder2);
      stringBuilder2.Append(";\n");
      stringBuilder2.Append("bool b = ");
      this.Alpha_Compare((int) this.AlphaCompare.Ref1, (int) this.AlphaCompare.Func1, ref stringBuilder2);
      stringBuilder2.Append(";\n");
      stringBuilder2.Append("  if(!(");
      switch (this.AlphaCompare.MergeFunc)
      {
        case 0:
          stringBuilder2.Append("all(bvec2(a, b))");
          break;
        case 1:
          stringBuilder2.Append("any(bvec2(a, b))");
          break;
        case 2:
          stringBuilder2.Append("any(bvec2(all(bvec2(!a, b)), all(bvec2(a, !b))))");
          break;
        case 3:
          stringBuilder2.Append("any(bvec2(all(bvec2(!a, !b)), all(bvec2(a, b))))");
          break;
      }
      stringBuilder2.Append("))\n");
      stringBuilder2.AppendLine("    discard;");
      stringBuilder2.AppendLine("}");
      this.fragment_shader = Gl.glCreateShader(35632);
      string str4 = stringBuilder2.ToString();
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

    private void Alpha_Compare(int reference, int id, ref StringBuilder b)
    {
      string str = ((float) reference / (float) byte.MaxValue).ToString().Replace(",", ".");
      switch (id)
      {
        case 0:
          b.Append("gl_FragColor.a <= -10.0");
          break;
        case 1:
          b.Append("gl_FragColor.a < " + str);
          break;
        case 2:
          b.Append("gl_FragColor.a == " + str);
          break;
        case 3:
          b.Append("gl_FragColor.a <= " + str);
          break;
        case 4:
          b.Append("gl_FragColor.a > " + str);
          break;
        case 5:
          b.Append("gl_FragColor.a != " + str);
          break;
        case 6:
          b.Append("gl_FragColor.a >= " + str);
          break;
        case 7:
          b.Append("gl_FragColor.a <= 10.0");
          break;
      }
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
          // TODO: Message box
          //int num1 = (int) MessageBox.Show(tevop.ToString());
          break;
        case 10:
        case 11:
          frag_ss.AppendFormat("((dot(a.gr, comp16) {0} dot(b.gr, comp16)){1}", (object) str2, (object) str1);
          // TODO: Message box
          //int num2 = (int) MessageBox.Show(tevop.ToString());
          break;
        case 12:
        case 13:
          frag_ss.AppendFormat("((dot(a.bgr, comp24) {0} dot(b.bgr, comp24)){1}", (object) str2, (object) str1);
          // TODO: Message box
          //int num3 = (int) MessageBox.Show(tevop.ToString());
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
          // TODO: Message box
          //int num4 = (int) MessageBox.Show(tevop.ToString());
          break;
      }
      frag_ss.AppendLine(";");
    }
  }
}
