// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3DS.CGFXShader
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Drawing;
using System.Text;
using Tao.OpenGl;

namespace MKDS_Course_Modifier._3DS
{
  public class CGFXShader
  {
    public int program = 0;
    public int fragment_shader = 0;
    public int vertex_shader = 0;
    public int[] Textures;
    private CGFX.DATA.CMDL.MTOB Material;

    public CGFXShader(CGFX.DATA.CMDL.MTOB Material, int[] Textures)
    {
      this.Textures = Textures;
      this.Material = Material;
    }

    public void Enable()
    {
      Gl.glUseProgram(this.program);
    }

    public void Disable()
    {
    }

    public void Compile()
    {
      uint length = (uint) this.Textures.Length;
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendFormat("vec4 diffuse = {0};\n", (object) this.GetVec4(this.Material.Diffuse_2));
      stringBuilder1.AppendFormat("vec4 ambient = {0};\n", (object) this.GetVec4(this.Material.Ambient_1));
      stringBuilder1.AppendFormat("vec4 spec1 = {0};\n", (object) this.GetVec4(this.Material.Specular0_2));
      stringBuilder1.AppendFormat("vec4 spec2 = {0};\n", (object) this.GetVec4(this.Material.Specular1_2));
      stringBuilder1.AppendLine("void main()");
      stringBuilder1.AppendLine("{");
      stringBuilder1.AppendLine("gl_FrontColor = gl_Color;");
      stringBuilder1.AppendLine("gl_BackColor = gl_Color;");
      stringBuilder1.AppendLine("gl_FrontSecondaryColor = gl_Color;");
      stringBuilder1.AppendLine("gl_BackSecondaryColor = gl_Color;");
      if (this.Material.Tex0 != null)
        stringBuilder1.AppendFormat("gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord{0};\n", (object) this.Material.Tex0Coord);
      if (this.Material.Tex1 != null)
        stringBuilder1.AppendFormat("gl_TexCoord[1] = gl_TextureMatrix[1] * gl_MultiTexCoord{0};\n", (object) this.Material.Tex1Coord);
      if (this.Material.Tex2 != null)
        stringBuilder1.AppendFormat("gl_TexCoord[2] = gl_TextureMatrix[2] * gl_MultiTexCoord{0};\n", (object) this.Material.Tex2Coord);
      stringBuilder1.AppendLine("gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;");
      stringBuilder1.AppendLine("}");
      this.vertex_shader = Gl.glCreateShader(35633);
      string str1 = stringBuilder1.ToString();
      Gl.glShaderSource(this.vertex_shader, 1, new string[1]
      {
        str1
      }, new int[1]{ str1.Length });
      Gl.glCompileShader(this.vertex_shader);
      string[] strArray1 = new string[16]
      {
        "gl_Color",
        "gl_Color",
        "gl_SecondaryColor",
        "texture2D(textures0, gl_TexCoord[0].st)",
        "texture2D(textures1, gl_TexCoord[1].st)",
        "texture2D(textures2, gl_TexCoord[2].st)",
        "texture2D(textures3, gl_TexCoord[3].st)",
        "const0",
        "const1",
        "const2",
        "const3",
        "const4",
        "const5",
        "unk3",
        "const{0}",
        "previous"
      };
      string[] strArray2 = new string[16]
      {
        "{0}.rgb",
        "vec3(1.0) - {0}.rgb",
        "{0}.aaa",
        "vec3(1.0) - {0}.aaa",
        "i1.rgb",
        "vec3(1.0) - i1.rgb",
        "i1.aaa",
        "vec3(1.0) - i1.aaa",
        "i2.rgb",
        "vec3(1.0) - i2.rgb",
        "i2.aaa",
        "vec3(1.0) - i2.aaa",
        "i3.rgb",
        "vec3(1.0) - i3.rgb",
        "i3.aaa",
        "vec3(1.0) - i3.aaa"
      };
      string[] strArray3 = new string[16]
      {
        "{0}.a",
        "1.0 - {0}.a",
        "i1.a",
        "1.0 - i1.a",
        "i2.a",
        "1.0 - i2.a",
        "i3.a",
        "1.0 - i3.a",
        "{0}.a",
        "{0}.a",
        "{0}.a",
        "{0}.a",
        "{0}.a",
        "{0}.a",
        "{0}.a",
        "{0}.a"
      };
      string[] strArray4 = new string[10]
      {
        "j1.rgb",
        "j1.rgb * j2.rgb",
        "j1.rgb + j2.rgb",
        "j1.rgb + j2.rgb - vec3(0.5)",
        "j1.rgb * j3.rgb + j2.rgb * (vec3(1.0) - j3.rgb)",
        "j1.rgb - j2.rgb",
        "vec3(4 * ((j1.r - 0.5) * (j1.r - 0.5) + (j1.g - 0.5) * (j2.g - 0.5) + (j1.b - 0.5) * (j2.b - 0.5)))",
        "vec4(4 * ((j1.r - 0.5) * (j1.r - 0.5) + (j1.g - 0.5) * (j2.g - 0.5) + (j1.b - 0.5) * (j2.b - 0.5)))",
        "j1.rgb * j2.rgb + j3.rgb",
        "(j2.rgb + j1.rgb) * j3.rgb"
      };
      string[] strArray5 = new string[10]
      {
        "j1.a",
        "j1.a * j2.a",
        "j1.a + j2.a",
        "j1.a + j2.a - 0.5",
        "j1.a * j3.a + j2.a * (1.0 - j3.a)",
        "j1.a - j2.a",
        "j1.a",
        "j1.a",
        "j1.a * j2.a + j3.a",
        "(j2.a + j1.a) * j3.a"
      };
      StringBuilder stringBuilder2 = new StringBuilder();
      for (uint index = 0; (int) index != (int) length; ++index)
        stringBuilder2.AppendFormat("uniform sampler2D textures{0};\n", (object) index);
      stringBuilder2.AppendFormat("vec4 const0 = {0};\n", (object) this.GetVec4(this.Material.Constant0_2));
      stringBuilder2.AppendFormat("vec4 const1 = {0};\n", (object) this.GetVec4(this.Material.Constant1_2));
      stringBuilder2.AppendFormat("vec4 const2 = {0};\n", (object) this.GetVec4(this.Material.Constant2_2));
      stringBuilder2.AppendFormat("vec4 const3 = {0};\n", (object) this.GetVec4(this.Material.Constant3_2));
      stringBuilder2.AppendFormat("vec4 const4 = {0};\n", (object) this.GetVec4(this.Material.Constant4_2));
      stringBuilder2.AppendFormat("vec4 const5 = {0};\n", (object) this.GetVec4(this.Material.Constant5_2));
      stringBuilder2.AppendFormat("vec4 diffuse = {0};\n", (object) this.GetVec4(this.Material.Diffuse_2));
      stringBuilder2.AppendFormat("vec4 ambient = {0};\n", (object) this.GetVec4(this.Material.Ambient_1));
      stringBuilder2.AppendFormat("vec4 spec1 = {0};\n", (object) this.GetVec4(this.Material.Specular0_2));
      stringBuilder2.AppendFormat("vec4 spec2 = {0};\n", (object) this.GetVec4(this.Material.Specular1_2));
      stringBuilder2.AppendFormat("vec4 unk1 = vec4(1.0);\n");
      stringBuilder2.AppendFormat("vec4 unk2 = vec4(0.0);\n");
      stringBuilder2.AppendFormat("vec4 unk3 = {0};\n", (object) this.GetVec4(this.Material.FragmentShader.BufferColor_2));
      stringBuilder2.AppendLine("void main()");
      stringBuilder2.AppendLine("{");
      stringBuilder2.AppendLine("vec4 previous = vec4(1.0);");
      stringBuilder2.AppendLine("vec4 i1;");
      stringBuilder2.AppendLine("vec4 i2;");
      stringBuilder2.AppendLine("vec4 i3;");
      stringBuilder2.AppendLine("vec4 j1;");
      stringBuilder2.AppendLine("vec4 j2;");
      stringBuilder2.AppendLine("vec4 j3;");
      stringBuilder2.AppendLine("vec4 ConstRgba;");
      for (int index1 = 0; index1 < 6; ++index1)
      {
        stringBuilder2.AppendFormat("ConstRgba = {0};\n", (object) this.GetVec4(this.Material.FragmentShader.TextureCombiners[index1].ConstRgba));
        int index2 = (int) this.Material.FragmentShader.TextureCombiners[index1].SrcRgb & 15;
        int index3 = (int) this.Material.FragmentShader.TextureCombiners[index1].SrcRgb >> 4 & 15;
        int index4 = (int) this.Material.FragmentShader.TextureCombiners[index1].SrcRgb >> 8 & 15;
        int index5 = (int) this.Material.FragmentShader.TextureCombiners[index1].SrcAlpha & 15;
        int index6 = (int) this.Material.FragmentShader.TextureCombiners[index1].SrcAlpha >> 4 & 15;
        int index7 = (int) this.Material.FragmentShader.TextureCombiners[index1].SrcAlpha >> 8 & 15;
        stringBuilder2.AppendFormat("i1 = vec4({0}.rgb, {1}.a);\n", (object) string.Format(strArray1[index2], (object) (uint) ((int) this.Material.FragmentShader.TextureCombiners[index1].Unknown3 & 15)), (object) string.Format(strArray1[index5], (object) (uint) ((int) this.Material.FragmentShader.TextureCombiners[index1].Unknown3 & 15)));
        stringBuilder2.AppendFormat("i2 = vec4({0}.rgb, {1}.a);\n", (object) string.Format(strArray1[index3], (object) (uint) ((int) this.Material.FragmentShader.TextureCombiners[index1].Unknown3 & 15)), (object) string.Format(strArray1[index6], (object) (uint) ((int) this.Material.FragmentShader.TextureCombiners[index1].Unknown3 & 15)));
        stringBuilder2.AppendFormat("i3 = vec4({0}.rgb, {1}.a);\n", (object) string.Format(strArray1[index4], (object) (uint) ((int) this.Material.FragmentShader.TextureCombiners[index1].Unknown3 & 15)), (object) string.Format(strArray1[index7], (object) (uint) ((int) this.Material.FragmentShader.TextureCombiners[index1].Unknown3 & 15)));
        uint num1 = this.Material.FragmentShader.TextureCombiners[index1].Operands & 15U;
        uint num2 = this.Material.FragmentShader.TextureCombiners[index1].Operands >> 4 & 15U;
        uint num3 = this.Material.FragmentShader.TextureCombiners[index1].Operands >> 8 & 15U;
        uint num4 = this.Material.FragmentShader.TextureCombiners[index1].Operands >> 12 & 15U;
        uint num5 = this.Material.FragmentShader.TextureCombiners[index1].Operands >> 16 & 15U;
        uint num6 = this.Material.FragmentShader.TextureCombiners[index1].Operands >> 20 & 15U;
        stringBuilder2.AppendFormat("j1 = vec4({0}, {1});\n", (object) string.Format(strArray2[(IntPtr) num1], (object) "i1"), (object) string.Format(strArray3[(IntPtr) num4], (object) "i1"));
        stringBuilder2.AppendFormat("j2 = vec4({0}, {1});\n", (object) string.Format(strArray2[(IntPtr) num2], (object) "i2"), (object) string.Format(strArray3[(IntPtr) num5], (object) "i2"));
        stringBuilder2.AppendFormat("j3 = vec4({0}, {1});\n", (object) string.Format(strArray2[(IntPtr) num3], (object) "i3"), (object) string.Format(strArray3[(IntPtr) num6], (object) "i3"));
        if (this.Material.FragmentShader.TextureCombiners[index1].CombineRgb == (ushort) 7)
          stringBuilder2.AppendFormat("previous = {0};\n", (object) strArray4[(int) this.Material.FragmentShader.TextureCombiners[index1].CombineRgb]);
        else
          stringBuilder2.AppendFormat("previous = clamp(vec4({0}, {1}), 0.0, 1.0);\n", (object) strArray4[(int) this.Material.FragmentShader.TextureCombiners[index1].CombineRgb], (object) strArray5[(int) this.Material.FragmentShader.TextureCombiners[index1].CombineAlpha]);
      }
      stringBuilder2.AppendLine("gl_FragColor = previous;");
      stringBuilder2.AppendLine("}");
      this.fragment_shader = Gl.glCreateShader(35632);
      string str2 = stringBuilder2.ToString();
      Gl.glShaderSource(this.fragment_shader, 1, new string[1]
      {
        str2
      }, new int[1]{ str2.Length });
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
    }

    private string GetVec4(Color c)
    {
      return string.Format("vec4({0}, {1}, {2}, {3})", (object) ((float) c.R / (float) byte.MaxValue).ToString().Replace(",", "."), (object) ((float) c.G / (float) byte.MaxValue).ToString().Replace(",", "."), (object) ((float) c.B / (float) byte.MaxValue).ToString().Replace(",", "."), (object) ((float) c.A / (float) byte.MaxValue).ToString().Replace(",", "."));
    }
  }
}
