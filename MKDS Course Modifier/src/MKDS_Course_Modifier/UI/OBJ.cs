// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.OBJ
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using OpenTK;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI
{
  public class OBJ : Form
  {
    public static float X = 0.0f;
    public static float Y = 0.0f;
    public static float ang = 0.0f;
    public static float dist = 0.0f;
    public static float elev = 0.0f;
    private MKDS_Course_Modifier._3D_Formats.OBJ Obj = (MKDS_Course_Modifier._3D_Formats.OBJ) null;
    private MLT Mlt = (MLT) null;
    private bool wire = false;
    private bool licht = true;
    private IContainer components = (IContainer) null;
    private MainMenu mainMenu1;
    private MenuItem menuItem1;
    private SimpleOpenGlControl simpleOpenGlControl1;

    public OBJ(MKDS_Course_Modifier._3D_Formats.OBJ Obj, MLT Mlt)
    {
      this.Obj = Obj;
      this.Mlt = Mlt;
      this.InitializeComponent();
      this.simpleOpenGlControl1.MouseWheel += new MouseEventHandler(this.simpleOpenGlControl1_MouseWheel);
    }

    private void simpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
        OBJ.dist += (float) e.Delta / 0.1f;
      else
        OBJ.dist += (float) e.Delta / 1f;
      this.Render();
    }

    private void OBJ_Load(object sender, EventArgs e)
    {
      this.simpleOpenGlControl1.InitializeContexts();
      Gl.ReloadFunctions();
      Gl.glEnable(32826);
      Gl.glEnable(2903);
      Gl.glEnable(2929);
      Gl.glEnable(2977);
      Gl.glDisable(2884);
      Gl.glFrontFace(2305);
      Gl.glEnable(3553);
      Gl.glClearDepth(1.0);
      Gl.glEnable(3008);
      Gl.glAlphaFunc(516, 0.0f);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      Gl.glShadeModel(7425);
      Gl.glClearColor(0.2f, 0.2f, 0.2f, 1f);
      if (this.Mlt != null)
      {
        int Nr = 1;
        foreach (MLT.Material material in this.Mlt.Materials)
        {
          if (material.DiffuseMap != null)
            GlNitro.glNitroTexImage2D(material.DiffuseMap, Nr, 10497, 9728);
          ++Nr;
        }
      }
      this.Render();
    }

    public void Shade()
    {
      Vector3 vector3_1 = new Vector3(0.0f, 400f, 0.0f);
      Vector3 vector3_2 = new Vector3((float) byte.MaxValue, (float) byte.MaxValue, (float) byte.MaxValue);
      Vector3 vector3_3 = new Vector3((float) sbyte.MaxValue, (float) sbyte.MaxValue, (float) sbyte.MaxValue);
      foreach (MKDS_Course_Modifier._3D_Formats.OBJ.Face face in this.Obj.Faces)
      {
        Vector3 vertex1 = this.Obj.Vertices[face.VertexIndieces[0]];
        Vector3 vertex2 = this.Obj.Vertices[face.VertexIndieces[1]];
        Vector3 vertex3 = this.Obj.Vertices[face.VertexIndieces[2]];
        Vector3 right1 = Vector3.Divide(vertex1 - vector3_1, (float) Math.Sqrt(((double) vertex1.X - (double) vector3_1.X) * ((double) vertex1.X - (double) vector3_1.X) + ((double) vertex1.Y - (double) vector3_1.Y) * ((double) vertex1.Y - (double) vector3_1.Y) + ((double) vertex1.Z - (double) vector3_1.Z) * ((double) vertex1.Z - (double) vector3_1.Z)));
        float num1 = Math.Abs(Vector3.Dot(this.Obj.Normals[face.NormalIndieces[0]], right1));
        Vector3 vector3_4 = vector3_2 * num1;
        this.Obj.VertexColors.Add(Color.FromArgb((int) vector3_4.X, (int) vector3_4.Y, (int) vector3_4.Z));
        face.VertexColorIndieces.Add(this.Obj.VertexColors.Count - 1);
        Vector3 right2 = Vector3.Divide(vertex2 - vector3_1, (float) Math.Sqrt(((double) vertex2.X - (double) vector3_1.X) * ((double) vertex2.X - (double) vector3_1.X) + ((double) vertex2.Y - (double) vector3_1.Y) * ((double) vertex2.Y - (double) vector3_1.Y) + ((double) vertex2.Z - (double) vector3_1.Z) * ((double) vertex2.Z - (double) vector3_1.Z)));
        float num2 = Math.Abs(Vector3.Dot(this.Obj.Normals[face.NormalIndieces[1]], right2));
        vector3_4 = vector3_2 * num2;
        vector3_4 += vector3_3;
        if ((double) vector3_4.X > (double) byte.MaxValue)
          vector3_4 = vector3_2;
        this.Obj.VertexColors.Add(Color.FromArgb((int) vector3_4.X, (int) vector3_4.Y, (int) vector3_4.Z));
        face.VertexColorIndieces.Add(this.Obj.VertexColors.Count - 1);
        Vector3 right3 = Vector3.Divide(vertex3 - vector3_1, (float) Math.Sqrt(((double) vertex3.X - (double) vector3_1.X) * ((double) vertex3.X - (double) vector3_1.X) + ((double) vertex3.Y - (double) vector3_1.Y) * ((double) vertex3.Y - (double) vector3_1.Y) + ((double) vertex3.Z - (double) vector3_1.Z) * ((double) vertex3.Z - (double) vector3_1.Z)));
        float num3 = Math.Abs(Vector3.Dot(this.Obj.Normals[face.NormalIndieces[2]], right3));
        vector3_4 = vector3_2 * num3;
        this.Obj.VertexColors.Add(Color.FromArgb((int) vector3_4.X, (int) vector3_4.Y, (int) vector3_4.Z));
        face.VertexColorIndieces.Add(this.Obj.VertexColors.Count - 1);
      }
    }

    public void Render()
    {
      float num1 = (float) this.simpleOpenGlControl1.Width / (float) this.simpleOpenGlControl1.Height;
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      Glu.gluPerspective(30.0, (double) num1, 0.100000001490116, 500000.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glBindTexture(3553, 0);
      Gl.glColor3f(1f, 1f, 1f);
      Gl.glClear(16640);
      Gl.glRotatef(OBJ.elev, 1f, 0.0f, 0.0f);
      Gl.glRotatef(-OBJ.elev, 1f, 0.0f, 0.0f);
      Gl.glTranslatef(OBJ.X, OBJ.Y, -OBJ.dist);
      Gl.glRotatef(OBJ.elev, 1f, 0.0f, 0.0f);
      Gl.glRotatef(OBJ.ang, 0.0f, 1f, 0.0f);
      Gl.glPushMatrix();
      MLT.Material material = (MLT.Material) null;
      foreach (MKDS_Course_Modifier._3D_Formats.OBJ.Face face in this.Obj.Faces)
      {
        float alpha = 1f;
        if ((material == null || material.Name != face.MaterialName) && this.Mlt != null)
        {
          material = this.Mlt.GetMaterialByName(face.MaterialName);
          alpha = material.Alpha;
          Gl.glBindTexture(3553, this.Mlt.Materials.IndexOf(material) + 1);
        }
        if (face.VertexIndieces.Count == 3)
          Gl.glBegin(4);
        else if (face.VertexIndieces.Count == 4)
          Gl.glBegin(7);
        else
          continue;
        for (int index = 0; index < face.VertexIndieces.Count; ++index)
        {
          if (face.TexCoordIndieces.Count != 0)
            Gl.glTexCoord2f(this.Obj.TexCoords[face.TexCoordIndieces[index]].X, -this.Obj.TexCoords[face.TexCoordIndieces[index]].Y);
          if (face.VertexColorIndieces.Count != 0)
          {
            Color vertexColor = this.Obj.VertexColors[face.VertexColorIndieces[index]];
            double num2 = (double) vertexColor.R / (double) byte.MaxValue;
            vertexColor = this.Obj.VertexColors[face.VertexColorIndieces[index]];
            double num3 = (double) vertexColor.G / (double) byte.MaxValue;
            vertexColor = this.Obj.VertexColors[face.VertexColorIndieces[index]];
            double num4 = (double) vertexColor.B / (double) byte.MaxValue;
            double num5 = (double) alpha;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, (float) num5);
          }
          else
            Gl.glColor4f(1f, 1f, 1f, alpha);
          Gl.glVertex3f(this.Obj.Vertices[face.VertexIndieces[index]].X, this.Obj.Vertices[face.VertexIndieces[index]].Y, this.Obj.Vertices[face.VertexIndieces[index]].Z);
        }
        Gl.glEnd();
      }
      Gl.glPopMatrix();
      this.simpleOpenGlControl1.Refresh();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      switch (keyData)
      {
        case Keys.Escape:
          OBJ.X = 0.0f;
          OBJ.Y = 0.0f;
          OBJ.ang = 0.0f;
          OBJ.dist = 0.0f;
          OBJ.elev = 0.0f;
          this.Render();
          return true;
        case Keys.Left:
          --OBJ.ang;
          this.Render();
          return true;
        case Keys.Up:
          ++OBJ.elev;
          this.Render();
          return true;
        case Keys.Right:
          ++OBJ.ang;
          this.Render();
          return true;
        case Keys.Down:
          --OBJ.elev;
          this.Render();
          return true;
        case Keys.A:
          OBJ.Y -= 50f;
          this.Render();
          return true;
        case Keys.L:
          this.licht = !this.licht;
          this.Render();
          return true;
        case Keys.S:
          OBJ.Y += 50f;
          this.Render();
          return true;
        case Keys.T:
          OBJ.elev = 90f;
          OBJ.ang = 0.0f;
          this.Render();
          return true;
        case Keys.W:
          this.wire = !this.wire;
          if (this.wire)
          {
            Gl.glPolygonMode(1032, 6913);
            this.Render();
          }
          else
          {
            Gl.glPolygonMode(1032, 6914);
            this.Render();
          }
          return true;
        case Keys.X:
          OBJ.X += 50f;
          this.Render();
          return true;
        case Keys.Z:
          OBJ.X -= 50f;
          this.Render();
          return true;
        default:
          return base.ProcessCmdKey(ref msg, keyData);
      }
    }

    private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
    {
      this.Render();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.mainMenu1 = new MainMenu(this.components);
      this.menuItem1 = new MenuItem();
      this.simpleOpenGlControl1 = new SimpleOpenGlControl();
      this.SuspendLayout();
      this.mainMenu1.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem1
      });
      this.menuItem1.Index = 0;
      this.menuItem1.Text = nameof (OBJ);
      this.simpleOpenGlControl1.AccumBits = (byte) 0;
      this.simpleOpenGlControl1.AutoCheckErrors = false;
      this.simpleOpenGlControl1.AutoFinish = false;
      this.simpleOpenGlControl1.AutoMakeCurrent = true;
      this.simpleOpenGlControl1.AutoSwapBuffers = true;
      this.simpleOpenGlControl1.BackColor = Color.Black;
      this.simpleOpenGlControl1.ColorBits = (byte) 32;
      this.simpleOpenGlControl1.DepthBits = (byte) 32;
      this.simpleOpenGlControl1.Dock = DockStyle.Fill;
      this.simpleOpenGlControl1.Location = new Point(0, 0);
      this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
      this.simpleOpenGlControl1.Size = new Size(355, 326);
      this.simpleOpenGlControl1.StencilBits = (byte) 8;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.simpleOpenGlControl1.Resize += new EventHandler(this.simpleOpenGlControl1_Resize);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(355, 326);
      this.Controls.Add((Control) this.simpleOpenGlControl1);
      this.Menu = (System.Windows.Forms.MainMenu) this.mainMenu1;
      this.Name = nameof (OBJ);
      this.Text = nameof (OBJ);
      this.Load += new EventHandler(this.OBJ_Load);
      this.ResumeLayout(false);
    }
  }
}
