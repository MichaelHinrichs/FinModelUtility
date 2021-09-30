// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.Shaders
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Text;
using System.Windows.Forms;
using Tao.OpenGl;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  internal class Shaders
  {
    public class Shader
    {
      public int program = 0;
      public int fragment_shader = 0;
      public int vertex_shader = 0;
      private const string vertexShader = "\r\n\tvoid main() \r\n\t{ \r\n\t\tgl_Position = gl_ModelViewProjectionMatrix * gl_Vertex; \r\n\t\tgl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0; \r\n\t\tgl_FrontColor = gl_Color; \r\n\t} \r\n";
      private const string fragmentShader = "\r\n\tuniform sampler1D toonTable;\r\n\tuniform sampler2D tex2d;\r\n\tuniform int hasTexture;\r\n\tuniform int texBlending;\r\n\t\r\n\t\r\n\tvoid main() \r\n\t{ \r\n\t\tvec4 texColor = vec4(1.0, 1.0, 1.0, 1.0); \r\n\t\tvec4 flagColor; \r\n\t\t\r\n\t\tif(hasTexture != 0) \r\n\t\t{ \r\n\t\t\ttexColor = texture2D(tex2d, gl_TexCoord[0].st); \r\n\t\t} \r\n\t\tflagColor = texColor; \r\n\t\tif(texBlending == 0) \r\n\t\t{ \r\n\t\t\tflagColor = gl_Color * texColor; \r\n\t\t} \r\n\t\telse \r\n\t\t\tif(texBlending == 1) \r\n\t\t\t{ \r\n\t\t\t\tif (texColor.a == 0.0 || hasTexture == 0) \r\n\t\t\t\t\tflagColor.rgb = gl_Color.rgb;\r\n\t\t\t\telse \r\n\t\t\t\t\tif (texColor.a == 1.0) \r\n\t\t\t\t\t\tflagColor.rgb = texColor.rgb;\r\n\t\t\t\t\telse \r\n\t\t\t\t\tflagColor.rgb = texColor.rgb * (1.0-texColor.a) + gl_Color.rgb * texColor.a;\r\n\t\t\t\tflagColor.a = gl_Color.a; \r\n\t\t\t} \r\n\t\t\telse \r\n\t\t\t\tif(texBlending == 2) \r\n\t\t\t\t{ \r\n\t\t\t\t\tvec3 toonColor = vec3(texture1D(toonTable, gl_Color.r).rgb); \r\n\t\t\t\t\tflagColor.rgb = texColor.rgb * toonColor.rgb;\r\n\t\t\t\t\tflagColor.a = texColor.a * gl_Color.a;\r\n\t\t\t\t} \r\n\t\t\t\telse \r\n\t\t\t\t\tif(texBlending == 3) \r\n\t\t\t\t\t{ \r\n\t\t\t\t\t\tvec3 toonColor = vec3(texture1D(toonTable, gl_Color.r).rgb); \r\n\t\t\t\t\t\tflagColor.rgb = texColor.rgb * gl_Color.rgb + toonColor.rgb; \r\n\t\t\t\t\t\tflagColor.a = texColor.a * gl_Color.a; \r\n\t\t\t\t\t} \r\n\t\tgl_FragColor = flagColor; \r\n\t} \r\n";

      public void Enable()
      {
        Gl.glUseProgram(this.program);
      }

      public void Compile()
      {
        StringBuilder stringBuilder1 = new StringBuilder();
        stringBuilder1.AppendLine("void main()");
        stringBuilder1.AppendLine("{");
        stringBuilder1.AppendLine("}");
        this.vertex_shader = Gl.glCreateShader(35633);
        string str1 = stringBuilder1.ToString();
        Gl.glShaderSource(this.vertex_shader, 1, new string[1]
        {
          str1
        }, new int[1]{ str1.Length });
        Gl.glCompileShader(this.vertex_shader);
        StringBuilder stringBuilder2 = new StringBuilder();
        stringBuilder2.Append("\r\n\tuniform sampler1D toonTable;\r\n\tuniform sampler2D tex2d;\r\n\tuniform int hasTexture;\r\n\tuniform int texBlending;\r\n\t\r\n\t\r\n\tvoid main() \r\n\t{ \r\n\t\tvec4 texColor = vec4(1.0, 1.0, 1.0, 1.0); \r\n\t\tvec4 flagColor; \r\n\t\t\r\n\t\tif(hasTexture != 0) \r\n\t\t{ \r\n\t\t\ttexColor = texture2D(tex2d, gl_TexCoord[0].st); \r\n\t\t} \r\n\t\tflagColor = texColor; \r\n\t\tif(texBlending == 0) \r\n\t\t{ \r\n\t\t\tflagColor = gl_Color * texColor; \r\n\t\t} \r\n\t\telse \r\n\t\t\tif(texBlending == 1) \r\n\t\t\t{ \r\n\t\t\t\tif (texColor.a == 0.0 || hasTexture == 0) \r\n\t\t\t\t\tflagColor.rgb = gl_Color.rgb;\r\n\t\t\t\telse \r\n\t\t\t\t\tif (texColor.a == 1.0) \r\n\t\t\t\t\t\tflagColor.rgb = texColor.rgb;\r\n\t\t\t\t\telse \r\n\t\t\t\t\tflagColor.rgb = texColor.rgb * (1.0-texColor.a) + gl_Color.rgb * texColor.a;\r\n\t\t\t\tflagColor.a = gl_Color.a; \r\n\t\t\t} \r\n\t\t\telse \r\n\t\t\t\tif(texBlending == 2) \r\n\t\t\t\t{ \r\n\t\t\t\t\tvec3 toonColor = vec3(texture1D(toonTable, gl_Color.r).rgb); \r\n\t\t\t\t\tflagColor.rgb = texColor.rgb * toonColor.rgb;\r\n\t\t\t\t\tflagColor.a = texColor.a * gl_Color.a;\r\n\t\t\t\t} \r\n\t\t\t\telse \r\n\t\t\t\t\tif(texBlending == 3) \r\n\t\t\t\t\t{ \r\n\t\t\t\t\t\tvec3 toonColor = vec3(texture1D(toonTable, gl_Color.r).rgb); \r\n\t\t\t\t\t\tflagColor.rgb = texColor.rgb * gl_Color.rgb + toonColor.rgb; \r\n\t\t\t\t\t\tflagColor.a = texColor.a * gl_Color.a; \r\n\t\t\t\t\t} \r\n\t\tgl_FragColor = flagColor; \r\n\t} \r\n");
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
}
