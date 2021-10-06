// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.FragmentShaderApplier
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class FragmentShaderApplier : Form
  {
    private IContainer components = (IContainer) null;
    private FragmentShaderApplier.GlShader g;
    private Bitmap b;
    private string bn;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(419, 341);
      this.Name = nameof (FragmentShaderApplier);
      this.Text = nameof (FragmentShaderApplier);
      this.Load += new EventHandler(this.FragmentShaderApplier_Load);
      this.ResumeLayout(false);
    }

    public FragmentShaderApplier(string file, string b)
    {
      this.g = new FragmentShaderApplier.GlShader(file);
      this.bn = b;
      Bitmap bitmap = new Bitmap(b);
      this.b = (Bitmap) bitmap.Clone();
      bitmap.Dispose();
      this.InitializeComponent();
    }

    private void FragmentShaderApplier_Load(object sender, EventArgs e)
    {
      this.g.ApplyToBitmap(this.b).Save(this.bn + "_new.png", ImageFormat.Png);
    }

    private class GlShader
    {
      private Dictionary<string, object> Variables;
      private string Shader;

      public GlShader(string file)
      {
        this.Shader = File.ReadAllText(file);
      }

      public Bitmap ApplyToBitmap(Bitmap Input)
      {
        Bitmap bitmap = (Bitmap) Input.Clone();
        for (int y = 0; y < bitmap.Height; ++y)
        {
          for (int x = 0; x < bitmap.Width; ++x)
            bitmap.SetPixel(x, y, this.ApplyToColor(bitmap.GetPixel(x, y)));
        }
        return bitmap;
      }

      private Color ApplyToColor(Color Input)
      {
        this.Variables = new Dictionary<string, object>();
        this.Variables.Add("gl_FragColor", (object) new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 1f));
        this.Variables.Add("gl_TexCoord", (object) new FragmentShaderApplier.GlShader.vec4[8]
        {
          new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 0.0f),
          new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 0.0f),
          new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 0.0f),
          new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 0.0f),
          new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 0.0f),
          new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 0.0f),
          new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 0.0f),
          new FragmentShaderApplier.GlShader.vec4(0.0f, 0.0f, 0.0f, 0.0f)
        });
        this.Variables.Add("gl_Color", (object) new FragmentShaderApplier.GlShader.vec4(1f, 1f, 1f, 1f));
        string[] strArray1 = this.Shader.Split(new char[3]
        {
          '\n',
          '\r',
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        List<string> stringList = new List<string>();
        foreach (string str in strArray1)
        {
          if (!str.TrimStart(' ').StartsWith("//"))
            stringList.Add(str);
        }
        foreach (string str in stringList)
        {
          if (str.TrimStart(' ').StartsWith("uniform"))
          {
            string[] strArray2 = str.TrimStart(' ').Split(' ');
            System.Type nestedType = this.GetType().GetNestedType(strArray2[1]);
            this.Variables.Add(strArray2[2], nestedType.GetConstructor(new System.Type[1]
            {
              typeof (Color)
            }).Invoke(new object[1]{ (object) Input }));
          }
          else if (str.TrimStart(' ').Contains("="))
          {
            string[] strArray2 = str.TrimStart(' ').Split('=');
            strArray2[0] = strArray2[0].TrimEnd(' ');
            strArray2[1] = strArray2[1].TrimStart(' ');
            string[] strArray3 = strArray2[0].Split(' ');
            string[] strArray4 = strArray2[1].Split('(', ',', ')');
            int index = 0;
            if (strArray3[0] == "const")
              ++index;
            switch (strArray3[index])
            {
              case "vec4":
                this.Variables.Add(strArray3[index + 1], (object) new FragmentShaderApplier.GlShader.vec4(float.Parse(strArray4[1].Replace('.', ','), NumberStyles.Float), float.Parse(strArray4[2].Replace('.', ','), NumberStyles.Float), float.Parse(strArray4[3].Replace('.', ','), NumberStyles.Float), float.Parse(strArray4[4].Replace('.', ','), NumberStyles.Float)));
                break;
              default:
                int num;
                if (strArray3[0].Split('.').Length > 1)
                  num = !char.IsLetter(strArray3[0].Split('.')[1][0]) ? 1 : 0;
                else
                  num = 1;
                if (num == 0)
                {
                  object obj = !strArray2[0].EndsWith("+") && !strArray2[0].EndsWith("-") && !strArray2[0].EndsWith("*") && !strArray2[0].EndsWith("/") ? this.GetExpressionResult(strArray2[1]) : this.GetExpressionResult(strArray2[0] + strArray2[1]);
                  this.SetPropertyValue(strArray3[0], obj);
                  break;
                }
                break;
            }
          }
        }
        return Color.FromArgb((int) ((double) ((FragmentShaderApplier.GlShader.vec4) this.Variables["gl_FragColor"]).a * (double) byte.MaxValue), (int) ((double) ((FragmentShaderApplier.GlShader.vec4) this.Variables["gl_FragColor"]).r * (double) byte.MaxValue), (int) ((double) ((FragmentShaderApplier.GlShader.vec4) this.Variables["gl_FragColor"]).g * (double) byte.MaxValue), (int) ((double) ((FragmentShaderApplier.GlShader.vec4) this.Variables["gl_FragColor"]).b * (double) byte.MaxValue));
      }

      private object GetPropertyValue(string Expression)
      {
        string[] strArray = Expression.TrimStart(' ').TrimEnd(' ').Split(new char[3]
        {
          '.',
          '[',
          ']'
        }, StringSplitOptions.RemoveEmptyEntries);
        int num;
        if (!this.Variables.ContainsKey(strArray[0]))
        {
          if (!char.IsDigit(Expression.TrimStart(' ')[0]))
            num = !char.IsPunctuation(Expression.TrimStart(' ')[0]) ? 1 : 0;
          else
            num = 0;
        }
        else
          num = 1;
        if (num == 0)
          return (object) float.Parse(Expression.TrimStart(' ').Replace('.', ','), NumberStyles.Float);
        object variable = this.Variables[strArray[0]];
        System.Type type1 = variable.GetType();
        if (strArray.Length == 1)
          return variable;
        int result = -1;
        if (!int.TryParse(strArray[1], out result))
          return type1.GetProperty(strArray[1]).GetValue(variable, (object[]) null);
        object[] objArray = (object[]) variable;
        System.Type type2 = objArray[result].GetType();
        return strArray.Length == 2 ? objArray[result] : type2.GetProperty(strArray[2]).GetValue(objArray[result], (object[]) null);
      }

      private void SetPropertyValue(string Expression, object Value)
      {
        string[] strArray = Expression.Split('.');
        object variable = this.Variables[strArray[0]];
        variable.GetType().GetProperty(strArray[1]).SetValue(variable, Value, (object[]) null);
      }

      private object GetExpressionResult(string Expression)
      {
        string[] strArray = Expression.Split(',');
        Stack<FragmentShaderApplier.GlShader.Method> methodStack = new Stack<FragmentShaderApplier.GlShader.Method>();
        methodStack.Push(new FragmentShaderApplier.GlShader.Method("base"));
        for (int index = 0; index < strArray.Length; ++index)
        {
          string str = strArray[index];
          char[] separator = new char[2]{ '(', ')' };
          foreach (string source in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          {
            int num1;
            if (!this.Variables.ContainsKey(source.TrimStart(' ')) && !source.Contains<char>('.'))
            {
              if (char.IsLetter(source.TrimStart(' ')[0]))
              {
                num1 = char.IsNumber(source.TrimStart(' ')[0]) ? 1 : 0;
                goto label_6;
              }
            }
            num1 = 1;
label_6:
            if (num1 == 0)
            {
              methodStack.Peek().Params.Add((object) new FragmentShaderApplier.GlShader.Method(source.TrimStart(' ')));
              methodStack.Push((FragmentShaderApplier.GlShader.Method) methodStack.Peek().Params.Last<object>());
            }
            else
            {
              int num2;
              if (!char.IsLetter(source.TrimStart(' ')[0]))
              {
                if (!char.IsNumber(source.TrimStart(' ')[0]))
                {
                  num2 = !(methodStack.Peek().Name != "base") ? 1 : 0;
                  goto label_12;
                }
              }
              num2 = 1;
label_12:
              if (num2 == 0)
              {
                methodStack.Peek().Expressions.Add(source.TrimStart(' '));
                methodStack.Pop();
              }
              else if (methodStack.Peek().Name != "base")
                methodStack.Peek().Params.Add((object) source.TrimStart(' '));
              else
                methodStack.Peek().Expressions.Add(source.TrimStart(' '));
            }
          }
          if (strArray[index].TrimEnd(' ').EndsWith(")"))
            methodStack.Pop();
        }
        return methodStack.Peek().Run(this);
      }

      private object GetMathExpressionResult(string Expression, object o = null)
      {
        List<object> objectList = new List<object>();
        if (o != null)
          objectList.Add(o);
        string str = Expression;
        char[] separator = new char[4]{ '+', '-', '*', '/' };
        foreach (string Expression1 in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
          if (!Expression1.StartsWith("."))
            objectList.Add(this.GetPropertyValue(Expression1));
        }
        int index1 = 0;
        foreach (char ch in Expression)
        {
          int num;
          switch (ch)
          {
            case '*':
            case '+':
            case '-':
              num = 0;
              break;
            default:
              num = ch != '/' ? 1 : 0;
              break;
          }
          if (num == 0)
          {
            if (ch == '*' || ch == '/')
            {
              switch (ch)
              {
                case '*':
                  object obj1 = objectList[index1];
                  System.Type type1 = objectList[index1].GetType();
                  object obj2 = objectList[index1 + 1];
                  System.Type type2 = objectList[index1 + 1].GetType();
                  object obj3;
                  if (obj1 is float && obj2 is float)
                    obj3 = (object) (float) ((double) (float) obj1 * (double) (float) obj2);
                  else if (obj2 is float)
                    obj3 = type1.GetMethod("op_Multiply", new System.Type[2]
                    {
                      obj1.GetType(),
                      obj2.GetType()
                    }).Invoke(obj1, new object[2]
                    {
                      obj1,
                      obj2
                    });
                  else
                    obj3 = type2.GetMethod("op_Multiply", new System.Type[2]
                    {
                      obj1.GetType(),
                      obj2.GetType()
                    }).Invoke(obj2, new object[2]
                    {
                      obj1,
                      obj2
                    });
                  objectList.Remove(obj1);
                  objectList.Remove(obj2);
                  objectList.Insert(index1, obj3);
                  break;
              }
            }
            ++index1;
          }
        }
        int index2 = 0;
        foreach (char ch in Expression)
        {
          int num;
          switch (ch)
          {
            case '*':
            case '+':
            case '-':
              num = 0;
              break;
            default:
              num = ch != '/' ? 1 : 0;
              break;
          }
          if (num == 0)
          {
            if (ch == '+' || ch == '-')
            {
              switch (ch)
              {
                case '+':
                  object obj1 = objectList[index2];
                  System.Type type1 = objectList[index2].GetType();
                  object obj2 = objectList[index2 + 1];
                  System.Type type2 = objectList[index2 + 1].GetType();
                  object obj3;
                  if (obj1 is float && obj2 is float)
                    obj3 = (object) (float) ((double) (float) obj1 + (double) (float) obj2);
                  else if (obj2 is float)
                    obj3 = type1.GetMethod("op_Addition", new System.Type[2]
                    {
                      obj1.GetType(),
                      obj2.GetType()
                    }).Invoke(obj1, new object[2]
                    {
                      obj1,
                      obj2
                    });
                  else
                    obj3 = type2.GetMethod("op_Addition", new System.Type[2]
                    {
                      obj1.GetType(),
                      obj2.GetType()
                    }).Invoke(obj2, new object[2]
                    {
                      obj1,
                      obj2
                    });
                  objectList.Remove(obj1);
                  objectList.Remove(obj2);
                  objectList.Insert(index2, obj3);
                  break;
                case '-':
                  object obj4 = objectList[index2];
                  System.Type type3 = objectList[index2].GetType();
                  object obj5 = objectList[index2 + 1];
                  System.Type type4 = objectList[index2 + 1].GetType();
                  object obj6;
                  if (obj4 is float && obj5 is float)
                    obj6 = (object) (float) ((double) (float) obj4 - (double) (float) obj5);
                  else if (obj5 is float)
                    obj6 = type3.GetMethod("op_Subtraction", new System.Type[2]
                    {
                      obj4.GetType(),
                      obj5.GetType()
                    }).Invoke(obj4, new object[2]
                    {
                      obj4,
                      obj5
                    });
                  else
                    obj6 = type4.GetMethod("op_Subtraction", new System.Type[2]
                    {
                      obj4.GetType(),
                      obj5.GetType()
                    }).Invoke(obj5, new object[2]
                    {
                      obj4,
                      obj5
                    });
                  objectList.Remove(obj4);
                  objectList.Remove(obj5);
                  objectList.Insert(index2, obj6);
                  break;
              }
            }
            ++index2;
          }
        }
        return objectList[0];
      }

      public FragmentShaderApplier.GlShader.vec4 texture2D(
        FragmentShaderApplier.GlShader.sampler2D sampler,
        FragmentShaderApplier.GlShader.vec2 P)
      {
        return new FragmentShaderApplier.GlShader.vec4((float) sampler.Color.R / (float) byte.MaxValue, (float) sampler.Color.G / (float) byte.MaxValue, (float) sampler.Color.B / (float) byte.MaxValue, (float) sampler.Color.A / (float) byte.MaxValue);
      }

      public FragmentShaderApplier.GlShader.vec2 mix(
        FragmentShaderApplier.GlShader.vec2 x,
        FragmentShaderApplier.GlShader.vec2 y,
        FragmentShaderApplier.GlShader.vec2 a)
      {
        return new FragmentShaderApplier.GlShader.vec2((float) ((double) x.x * (1.0 - (double) a.x) + (double) y.x * (double) a.x), (float) ((double) x.y * (1.0 - (double) a.y) + (double) y.y * (double) a.y));
      }

      public FragmentShaderApplier.GlShader.vec3 mix(
        FragmentShaderApplier.GlShader.vec3 x,
        FragmentShaderApplier.GlShader.vec3 y,
        FragmentShaderApplier.GlShader.vec3 a)
      {
        return new FragmentShaderApplier.GlShader.vec3((float) ((double) x.x * (1.0 - (double) a.x) + (double) y.x * (double) a.x), (float) ((double) x.y * (1.0 - (double) a.y) + (double) y.y * (double) a.y), (float) ((double) x.z * (1.0 - (double) a.z) + (double) y.z * (double) a.z));
      }

      public FragmentShaderApplier.GlShader.vec4 mix(
        FragmentShaderApplier.GlShader.vec4 x,
        FragmentShaderApplier.GlShader.vec4 y,
        FragmentShaderApplier.GlShader.vec4 a)
      {
        return new FragmentShaderApplier.GlShader.vec4((float) ((double) x.x * (1.0 - (double) a.x) + (double) y.x * (double) a.x), (float) ((double) x.y * (1.0 - (double) a.y) + (double) y.y * (double) a.y), (float) ((double) x.z * (1.0 - (double) a.z) + (double) y.z * (double) a.z), (float) ((double) x.w * (1.0 - (double) a.w) + (double) y.w * (double) a.w));
      }

      public FragmentShaderApplier.GlShader.vec2 mix(
        FragmentShaderApplier.GlShader.vec2 x,
        FragmentShaderApplier.GlShader.vec2 y,
        float a)
      {
        return new FragmentShaderApplier.GlShader.vec2((float) ((double) x.x * (1.0 - (double) a) + (double) y.x * (double) a), (float) ((double) x.y * (1.0 - (double) a) + (double) y.y * (double) a));
      }

      public FragmentShaderApplier.GlShader.vec3 mix(
        FragmentShaderApplier.GlShader.vec3 x,
        FragmentShaderApplier.GlShader.vec3 y,
        float a)
      {
        return new FragmentShaderApplier.GlShader.vec3((float) ((double) x.x * (1.0 - (double) a) + (double) y.x * (double) a), (float) ((double) x.y * (1.0 - (double) a) + (double) y.y * (double) a), (float) ((double) x.z * (1.0 - (double) a) + (double) y.z * (double) a));
      }

      public FragmentShaderApplier.GlShader.vec4 mix(
        FragmentShaderApplier.GlShader.vec4 x,
        FragmentShaderApplier.GlShader.vec4 y,
        float a)
      {
        return new FragmentShaderApplier.GlShader.vec4((float) ((double) x.x * (1.0 - (double) a) + (double) y.x * (double) a), (float) ((double) x.y * (1.0 - (double) a) + (double) y.y * (double) a), (float) ((double) x.z * (1.0 - (double) a) + (double) y.z * (double) a), (float) ((double) x.w * (1.0 - (double) a) + (double) y.w * (double) a));
      }

      public float mix(float x, float y, float a)
      {
        return (float) ((double) x * (1.0 - (double) a) + (double) y * (double) a);
      }

      public FragmentShaderApplier.GlShader.vec2 clamp(
        FragmentShaderApplier.GlShader.vec2 x,
        FragmentShaderApplier.GlShader.vec2 minVal,
        FragmentShaderApplier.GlShader.vec2 maxVal)
      {
        return new FragmentShaderApplier.GlShader.vec2(Math.Min(Math.Max(x.x, minVal.x), maxVal.x), Math.Min(Math.Max(x.y, minVal.y), maxVal.y));
      }

      public FragmentShaderApplier.GlShader.vec3 clamp(
        FragmentShaderApplier.GlShader.vec3 x,
        FragmentShaderApplier.GlShader.vec3 minVal,
        FragmentShaderApplier.GlShader.vec3 maxVal)
      {
        return new FragmentShaderApplier.GlShader.vec3(Math.Min(Math.Max(x.x, minVal.x), maxVal.x), Math.Min(Math.Max(x.y, minVal.y), maxVal.y), Math.Min(Math.Max(x.z, minVal.z), maxVal.z));
      }

      public FragmentShaderApplier.GlShader.vec4 clamp(
        FragmentShaderApplier.GlShader.vec4 x,
        FragmentShaderApplier.GlShader.vec4 minVal,
        FragmentShaderApplier.GlShader.vec4 maxVal)
      {
        return new FragmentShaderApplier.GlShader.vec4(Math.Min(Math.Max(x.x, minVal.x), maxVal.x), Math.Min(Math.Max(x.y, minVal.y), maxVal.y), Math.Min(Math.Max(x.z, minVal.z), maxVal.z), Math.Min(Math.Max(x.w, minVal.w), maxVal.w));
      }

      public FragmentShaderApplier.GlShader.vec2 clamp(
        FragmentShaderApplier.GlShader.vec2 x,
        float minVal,
        float maxVal)
      {
        return new FragmentShaderApplier.GlShader.vec2(Math.Min(Math.Max(x.x, minVal), maxVal), Math.Min(Math.Max(x.y, minVal), maxVal));
      }

      public FragmentShaderApplier.GlShader.vec3 clamp(
        FragmentShaderApplier.GlShader.vec3 x,
        float minVal,
        float maxVal)
      {
        return new FragmentShaderApplier.GlShader.vec3(Math.Min(Math.Max(x.x, minVal), maxVal), Math.Min(Math.Max(x.y, minVal), maxVal), Math.Min(Math.Max(x.z, minVal), maxVal));
      }

      public FragmentShaderApplier.GlShader.vec4 clamp(
        FragmentShaderApplier.GlShader.vec4 x,
        float minVal,
        float maxVal)
      {
        return new FragmentShaderApplier.GlShader.vec4(Math.Min(Math.Max(x.x, minVal), maxVal), Math.Min(Math.Max(x.y, minVal), maxVal), Math.Min(Math.Max(x.z, minVal), maxVal), Math.Min(Math.Max(x.w, minVal), maxVal));
      }

      public float clamp(float x, float minVal, float maxVal)
      {
        return Math.Min(Math.Max(x, minVal), maxVal);
      }

      private class Method
      {
        public string Name = "";
        public List<object> Params = new List<object>();
        public List<string> Expressions = new List<string>();

        public Method(string Name)
        {
          this.Name = Name;
        }

        public override string ToString()
        {
          return this.Name;
        }

        public object Run(FragmentShaderApplier.GlShader Shader)
        {
          List<System.Type> typeList = new List<System.Type>();
          for (int index = 0; index < this.Params.Count; ++index)
          {
            if (this.Params[index] is FragmentShaderApplier.GlShader.Method)
            {
              this.Params[index] = ((FragmentShaderApplier.GlShader.Method) this.Params[index]).Run(Shader);
              typeList.Add(this.Params[index].GetType());
            }
            else
            {
              this.Params[index] = Shader.GetMathExpressionResult((string) this.Params[index], (object) null);
              typeList.Add(this.Params[index].GetType());
            }
          }
          if (this.Name == "base")
          {
            if (this.Params.Count == 1)
              return this.Params[0];
            return this.Params.Count == 0 && this.Expressions[0].StartsWith("-") ? Shader.GetMathExpressionResult(this.Expressions[0], (object) 0.0f) : Shader.GetMathExpressionResult(this.Expressions[0], (object) null);
          }
          if (this.Expressions.Count == 0)
            return typeof (FragmentShaderApplier.GlShader).GetMethod(this.Name, typeList.ToArray()) == (MethodInfo) null ? typeof (FragmentShaderApplier.GlShader).GetNestedType(this.Name).GetConstructor(typeList.ToArray()).Invoke(this.Params.ToArray()) : typeof (FragmentShaderApplier.GlShader).GetMethod(this.Name, typeList.ToArray()).Invoke((object) Shader, this.Params.ToArray());
          object o = typeof (FragmentShaderApplier.GlShader).GetMethod(this.Name, typeList.ToArray()).Invoke((object) Shader, this.Params.ToArray());
          System.Type type = o.GetType();
          if (this.Expressions[0].StartsWith("."))
          {
            o = type.GetProperty(this.Expressions[0].Split(new char[1]
            {
              '.'
            }, StringSplitOptions.RemoveEmptyEntries)[0]).GetValue(o, (object[]) null);
            o.GetType();
          }
          for (int index = 0; index < this.Expressions.Count; ++index)
            o = Shader.GetMathExpressionResult(this.Expressions[index], o);
          return o;
        }
      }

      public class vec2
      {
        public vec2(float x, float y)
        {
          this.x = x;
          this.y = y;
        }

        public float x { get; set; }

        public float y { get; set; }

        public float r
        {
          get
          {
            return this.x;
          }
          set
          {
            this.x = value;
          }
        }

        public float g
        {
          get
          {
            return this.y;
          }
          set
          {
            this.y = value;
          }
        }

        public float s
        {
          get
          {
            return this.x;
          }
          set
          {
            this.x = value;
          }
        }

        public float t
        {
          get
          {
            return this.y;
          }
          set
          {
            this.y = value;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 xy
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 rg
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 st
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public static FragmentShaderApplier.GlShader.vec2 operator +(
          FragmentShaderApplier.GlShader.vec2 a,
          FragmentShaderApplier.GlShader.vec2 b)
        {
          return new FragmentShaderApplier.GlShader.vec2(a.x + b.x, a.y + b.y);
        }

        public static FragmentShaderApplier.GlShader.vec2 operator +(
          FragmentShaderApplier.GlShader.vec2 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec2(a.x + b, a.y + b);
        }

        public static FragmentShaderApplier.GlShader.vec2 operator -(
          FragmentShaderApplier.GlShader.vec2 a,
          FragmentShaderApplier.GlShader.vec2 b)
        {
          return new FragmentShaderApplier.GlShader.vec2(a.x - b.x, a.y - b.y);
        }

        public static FragmentShaderApplier.GlShader.vec2 operator -(
          FragmentShaderApplier.GlShader.vec2 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec2(a.x - b, a.y - b);
        }

        public static FragmentShaderApplier.GlShader.vec2 operator *(
          float a,
          FragmentShaderApplier.GlShader.vec2 b)
        {
          return new FragmentShaderApplier.GlShader.vec2(a * b.x, a * b.y);
        }

        public static FragmentShaderApplier.GlShader.vec2 operator *(
          FragmentShaderApplier.GlShader.vec2 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec2(a.x * b, a.y * b);
        }
      }

      public class vec3
      {
        public vec3(float x, float y, float z)
        {
          this.x = x;
          this.y = y;
          this.z = z;
        }

        public float x { get; set; }

        public float y { get; set; }

        public float z { get; set; }

        public float r
        {
          get
          {
            return this.x;
          }
          set
          {
            this.x = value;
          }
        }

        public float g
        {
          get
          {
            return this.y;
          }
          set
          {
            this.y = value;
          }
        }

        public float b
        {
          get
          {
            return this.z;
          }
          set
          {
            this.z = value;
          }
        }

        public float s
        {
          get
          {
            return this.x;
          }
          set
          {
            this.x = value;
          }
        }

        public float t
        {
          get
          {
            return this.y;
          }
          set
          {
            this.y = value;
          }
        }

        public float p
        {
          get
          {
            return this.z;
          }
          set
          {
            this.z = value;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 xy
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.y);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 rg
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.y);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 st
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.y);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 yz
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.z);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 gb
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.z);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 tp
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.z);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 xz
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.z);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 rb
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.z);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 sp
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.z);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 xyz
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 rgb
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 stp
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
          }
        }

        public static FragmentShaderApplier.GlShader.vec3 operator +(
          FragmentShaderApplier.GlShader.vec3 a,
          FragmentShaderApplier.GlShader.vec3 b)
        {
          return new FragmentShaderApplier.GlShader.vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static FragmentShaderApplier.GlShader.vec3 operator +(
          FragmentShaderApplier.GlShader.vec3 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec3(a.x + b, a.y + b, a.z + b);
        }

        public static FragmentShaderApplier.GlShader.vec3 operator +(
          float a,
          FragmentShaderApplier.GlShader.vec3 b)
        {
          return new FragmentShaderApplier.GlShader.vec3(a + b.x, a + b.y, a + b.z);
        }

        public static FragmentShaderApplier.GlShader.vec3 operator -(
          FragmentShaderApplier.GlShader.vec3 a,
          FragmentShaderApplier.GlShader.vec3 b)
        {
          return new FragmentShaderApplier.GlShader.vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static FragmentShaderApplier.GlShader.vec3 operator -(
          FragmentShaderApplier.GlShader.vec3 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec3(a.x - b, a.y - b, a.z - b);
        }

        public static FragmentShaderApplier.GlShader.vec3 operator -(
          float a,
          FragmentShaderApplier.GlShader.vec3 b)
        {
          return new FragmentShaderApplier.GlShader.vec3(a - b.x, a - b.y, a - b.z);
        }

        public static FragmentShaderApplier.GlShader.vec3 operator *(
          float a,
          FragmentShaderApplier.GlShader.vec3 b)
        {
          return new FragmentShaderApplier.GlShader.vec3(a * b.x, a * b.y, a * b.z);
        }

        public static FragmentShaderApplier.GlShader.vec3 operator *(
          FragmentShaderApplier.GlShader.vec3 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec3(a.x * b, a.y * b, a.z * b);
        }
      }

      public class vec4
      {
        public vec4(float x, float y, float z, float w)
        {
          this.x = x;
          this.y = y;
          this.z = z;
          this.w = w;
        }

        public float x { get; set; }

        public float y { get; set; }

        public float z { get; set; }

        public float w { get; set; }

        public float r
        {
          get
          {
            return this.x;
          }
          set
          {
            this.x = value;
          }
        }

        public float g
        {
          get
          {
            return this.y;
          }
          set
          {
            this.y = value;
          }
        }

        public float b
        {
          get
          {
            return this.z;
          }
          set
          {
            this.z = value;
          }
        }

        public float a
        {
          get
          {
            return this.w;
          }
          set
          {
            this.w = value;
          }
        }

        public float s
        {
          get
          {
            return this.x;
          }
          set
          {
            this.x = value;
          }
        }

        public float t
        {
          get
          {
            return this.y;
          }
          set
          {
            this.y = value;
          }
        }

        public float p
        {
          get
          {
            return this.z;
          }
          set
          {
            this.z = value;
          }
        }

        public float q
        {
          get
          {
            return this.w;
          }
          set
          {
            this.w = value;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 xy
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.y);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 rg
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.y);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 st
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.y);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 yz
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.z);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 gb
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.z);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 tp
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.z);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 zw
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.z, this.w);
          }
          set
          {
            this.z = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 ba
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.z, this.w);
          }
          set
          {
            this.z = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 pq
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.z, this.w);
          }
          set
          {
            this.z = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 xz
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.z);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 rb
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.z);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 sp
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.z);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 xw
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.w);
          }
          set
          {
            this.x = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 ra
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.w);
          }
          set
          {
            this.x = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 sq
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.x, this.w);
          }
          set
          {
            this.x = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 yw
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.w);
          }
          set
          {
            this.y = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 ga
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.w);
          }
          set
          {
            this.y = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec2 tq
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec2(this.y, this.w);
          }
          set
          {
            this.y = value.x;
            this.w = value.y;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 xyz
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.x, this.y, this.z);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 rgb
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.x, this.y, this.z);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 stp
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.x, this.y, this.z);
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 yzw
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.y, this.z, this.w);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
            this.w = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 gba
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.y, this.z, this.w);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
            this.w = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 tpq
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.y, this.z, this.w);
          }
          set
          {
            this.y = value.x;
            this.z = value.y;
            this.w = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 xzw
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.x, this.z, this.w);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
            this.w = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 rba
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.x, this.z, this.w);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
            this.w = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec3 spq
        {
          get
          {
            return new FragmentShaderApplier.GlShader.vec3(this.x, this.z, this.w);
          }
          set
          {
            this.x = value.x;
            this.z = value.y;
            this.w = value.z;
          }
        }

        public FragmentShaderApplier.GlShader.vec4 xyzw
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
            this.w = value.w;
          }
        }

        public FragmentShaderApplier.GlShader.vec4 rgba
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
            this.w = value.w;
          }
        }

        public FragmentShaderApplier.GlShader.vec4 stpq
        {
          get
          {
            return this;
          }
          set
          {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
            this.w = value.w;
          }
        }

        public static FragmentShaderApplier.GlShader.vec4 operator +(
          FragmentShaderApplier.GlShader.vec4 a,
          FragmentShaderApplier.GlShader.vec4 b)
        {
          return new FragmentShaderApplier.GlShader.vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static FragmentShaderApplier.GlShader.vec4 operator +(
          FragmentShaderApplier.GlShader.vec4 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec4(a.x + b, a.y + b, a.z + b, a.w + b);
        }

        public static FragmentShaderApplier.GlShader.vec4 operator -(
          FragmentShaderApplier.GlShader.vec4 a,
          FragmentShaderApplier.GlShader.vec4 b)
        {
          return new FragmentShaderApplier.GlShader.vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static FragmentShaderApplier.GlShader.vec4 operator -(
          FragmentShaderApplier.GlShader.vec4 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec4(a.x - b, a.y - b, a.z - b, a.w - b);
        }

        public static FragmentShaderApplier.GlShader.vec4 operator *(
          float a,
          FragmentShaderApplier.GlShader.vec4 b)
        {
          return new FragmentShaderApplier.GlShader.vec4(a * b.x, a * b.y, a * b.z, a * b.w);
        }

        public static FragmentShaderApplier.GlShader.vec4 operator *(
          FragmentShaderApplier.GlShader.vec4 a,
          float b)
        {
          return new FragmentShaderApplier.GlShader.vec4(a.x * b, a.y * b, a.z * b, a.w * b);
        }
      }

      public class sampler2D
      {
        public Color Color;

        public sampler2D()
        {
          this.Color = Color.Black;
        }

        public sampler2D(Color c)
        {
          this.Color = c;
        }

        public sampler2D(Bitmap b, int X, int Y)
        {
          this.Color = b.GetPixel(X, Y);
        }
      }
    }
  }
}
