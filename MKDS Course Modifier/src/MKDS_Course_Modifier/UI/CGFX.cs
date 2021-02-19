// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.CGFX
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier._3DS;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI
{
  public class CGFX : Form
  {
    public static float X = 0.0f;
    public static float Y = 0.0f;
    public static float ang = 0.0f;
    public static float dist = 0.0f;
    public static float elev = 0.0f;
    private bool wire = false;
    private bool licht = true;
    private IContainer components = (IContainer) null;
    private MKDS_Course_Modifier._3DS.CGFX cgfx;
    private CGFXShader[] Shaders;
    private SimpleOpenGlControl simpleOpenGlControl1;

    public CGFX(MKDS_Course_Modifier._3DS.CGFX cgfx)
    {
      this.cgfx = cgfx;
      this.InitializeComponent();
      this.simpleOpenGlControl1.MouseWheel += new MouseEventHandler(this.simpleOpenGlControl1_MouseWheel);
    }

    private void simpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
        CGFX.dist += (float) e.Delta / 10f;
      else
        CGFX.dist += (float) e.Delta / 100f;
      this.Render();
    }

    private void CGFX_Load(object sender, EventArgs e)
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
      Gl.glDepthFunc(515);
      Gl.glClearColor(0.2f, 0.2f, 0.2f, 0.0f);
      if (this.cgfx.Data.Textures != null)
      {
        int Nr = 1;
        foreach (MKDS_Course_Modifier._3DS.CGFX.DATA.TXOB texture in this.cgfx.Data.Textures)
        {
          int WrapModeS = 10497;
          int WrapModeT = 10497;
          GlNitro.glNitroTexImage2D(texture.GetBitmap(0), Nr, WrapModeS, WrapModeT, 9729, 9729);
          ++Nr;
        }
      }
      this.Shaders = new CGFXShader[this.cgfx.Data.Models[0].Materials.Length];
      this.Render();
    }

    public void Render()
    {
      if (this.cgfx.Data.Models == null)
        return;
      float num = (float) this.simpleOpenGlControl1.Width / (float) this.simpleOpenGlControl1.Height;
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      Glu.gluPerspective(30.0, (double) num, 0.100000001490116, 20480000.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glBindTexture(3553, 0);
      Gl.glColor3f(1f, 1f, 1f);
      Gl.glClear(16640);
      Gl.glTranslatef(CGFX.X, CGFX.Y, -CGFX.dist);
      Gl.glRotatef(CGFX.elev, 1f, 0.0f, 0.0f);
      Gl.glRotatef(CGFX.ang, 0.0f, 1f, 0.0f);
      Gl.glPushMatrix();
      MKDS_Course_Modifier._3DS.CGFX.DATA.CMDL model = this.cgfx.Data.Models[0];
      foreach (MKDS_Course_Modifier._3DS.CGFX.DATA.CMDL.Mesh mesh in model.Meshes)
      {
        MKDS_Course_Modifier._3DS.CGFX.DATA.CMDL.SeparateDataShapeCtr shape = model.Shapes[(IntPtr) mesh.SeparateShapeReference];
        Polygon vertexData = shape.GetVertexData(model);
        MKDS_Course_Modifier._3DS.CGFX.DATA.CMDL.MTOB material = model.Materials[(IntPtr) mesh.MaterialReference];
        Gl.glMatrixMode(5890);
        if (material.Tex0 != null)
        {
          Gl.glActiveTexture(33984);
          Gl.glBindTexture(3553, this.cgfx.Data.Dictionaries[1].IndexOf(material.Tex0.TextureObject.LinkedTextureName) + 1);
          Gl.glEnable(3553);
          Gl.glLoadIdentity();
          float[] m = new float[16];
          Array.Copy((Array) material.Tex0Matrix, (Array) m, 12);
          m[15] = 1f;
          Gl.glLoadMatrixf(m);
        }
        if (material.Tex1 != null)
        {
          Gl.glActiveTexture(33985);
          Gl.glBindTexture(3553, this.cgfx.Data.Dictionaries[1].IndexOf(material.Tex1.TextureObject.LinkedTextureName) + 1);
          Gl.glEnable(3553);
          Gl.glLoadIdentity();
          float[] m = new float[16];
          Array.Copy((Array) material.Tex1Matrix, (Array) m, 12);
          m[15] = 1f;
          Gl.glLoadMatrixf(m);
        }
        if (material.Tex2 != null)
        {
          Gl.glActiveTexture(33986);
          Gl.glBindTexture(3553, this.cgfx.Data.Dictionaries[1].IndexOf(material.Tex2.TextureObject.LinkedTextureName) + 1);
          Gl.glEnable(3553);
          Gl.glLoadIdentity();
          float[] m = new float[16];
          Array.Copy((Array) material.Tex2Matrix, (Array) m, 12);
          m[15] = 1f;
          Gl.glLoadMatrixf(m);
        }
        if (material.Tex3 != null)
        {
          Gl.glActiveTexture(33987);
          Gl.glBindTexture(3553, this.cgfx.Data.Dictionaries[1].IndexOf(material.Tex3.TextureObject.LinkedTextureName) + 1);
          Gl.glEnable(3553);
          Gl.glLoadIdentity();
        }
        Gl.glMatrixMode(5888);
        if (this.Shaders[(IntPtr) mesh.MaterialReference] == null)
        {
          List<int> intList = new List<int>();
          if (material.Tex0 != null)
            intList.Add(this.cgfx.Data.Dictionaries[1].IndexOf(material.Tex0.TextureObject.LinkedTextureName) + 1);
          else
            intList.Add(0);
          if (material.Tex1 != null)
            intList.Add(this.cgfx.Data.Dictionaries[1].IndexOf(material.Tex1.TextureObject.LinkedTextureName) + 1);
          else
            intList.Add(0);
          if (material.Tex2 != null)
            intList.Add(this.cgfx.Data.Dictionaries[1].IndexOf(material.Tex2.TextureObject.LinkedTextureName) + 1);
          else
            intList.Add(0);
          if (material.Tex3 != null)
            intList.Add(this.cgfx.Data.Dictionaries[1].IndexOf(material.Tex3.TextureObject.LinkedTextureName) + 1);
          else
            intList.Add(0);
          this.Shaders[(IntPtr) mesh.MaterialReference] = new CGFXShader(material, intList.ToArray());
          this.Shaders[(IntPtr) mesh.MaterialReference].Compile();
        }
        this.Shaders[(IntPtr) mesh.MaterialReference].Enable();
        foreach (MKDS_Course_Modifier._3DS.CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr.PrimitiveCtr.IndexStreamCtr indexStream in shape.PrimitiveSets[0].Primitives[0].IndexStreams)
        {
          Vector3[] faceData = indexStream.GetFaceData();
          Gl.glBegin(4);
          foreach (Vector3 vector3 in faceData)
          {
            if (vertexData.Normals != null)
              Gl.glNormal3f(vertexData.Normals[(int) vector3.X].X, vertexData.Normals[(int) vector3.X].Y, vertexData.Normals[(int) vector3.X].Z);
            if (vertexData.Colors != null)
              Gl.glColor4f((float) vertexData.Colors[(int) vector3.X].R / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.X].G / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.X].B / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.X].A / (float) byte.MaxValue);
            else
              Gl.glColor4f(1f, 1f, 1f, 1f);
            if (vertexData.TexCoords != null)
              Gl.glMultiTexCoord2f(33984, vertexData.TexCoords[(int) vector3.X].X, -vertexData.TexCoords[(int) vector3.X].Y);
            if (vertexData.TexCoords2 != null)
              Gl.glMultiTexCoord2f(33985, vertexData.TexCoords2[(int) vector3.X].X, -vertexData.TexCoords2[(int) vector3.X].Y);
            if (vertexData.TexCoords3 != null)
              Gl.glMultiTexCoord2f(33986, vertexData.TexCoords3[(int) vector3.X].X, -vertexData.TexCoords3[(int) vector3.X].Y);
            Gl.glVertex3f(vertexData.Vertex[(int) vector3.X].X, vertexData.Vertex[(int) vector3.X].Y, vertexData.Vertex[(int) vector3.X].Z);
            if (vertexData.Normals != null)
              Gl.glNormal3f(vertexData.Normals[(int) vector3.Y].X, vertexData.Normals[(int) vector3.Y].Y, vertexData.Normals[(int) vector3.Y].Z);
            if (vertexData.Colors != null)
              Gl.glColor4f((float) vertexData.Colors[(int) vector3.Y].R / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.Y].G / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.Y].B / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.Y].A / (float) byte.MaxValue);
            else
              Gl.glColor4f(1f, 1f, 1f, 1f);
            if (vertexData.TexCoords != null)
              Gl.glMultiTexCoord2f(33984, vertexData.TexCoords[(int) vector3.Y].X, -vertexData.TexCoords[(int) vector3.Y].Y);
            if (vertexData.TexCoords2 != null)
              Gl.glMultiTexCoord2f(33985, vertexData.TexCoords2[(int) vector3.Y].X, -vertexData.TexCoords2[(int) vector3.Y].Y);
            if (vertexData.TexCoords3 != null)
              Gl.glMultiTexCoord2f(33986, vertexData.TexCoords3[(int) vector3.Y].X, -vertexData.TexCoords3[(int) vector3.Y].Y);
            Gl.glVertex3f(vertexData.Vertex[(int) vector3.Y].X, vertexData.Vertex[(int) vector3.Y].Y, vertexData.Vertex[(int) vector3.Y].Z);
            if (vertexData.Normals != null)
              Gl.glNormal3f(vertexData.Normals[(int) vector3.Z].X, vertexData.Normals[(int) vector3.Z].Y, vertexData.Normals[(int) vector3.Z].Z);
            if (vertexData.Colors != null)
              Gl.glColor4f((float) vertexData.Colors[(int) vector3.Z].R / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.Z].G / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.Z].B / (float) byte.MaxValue, (float) vertexData.Colors[(int) vector3.Z].A / (float) byte.MaxValue);
            else
              Gl.glColor4f(1f, 1f, 1f, 1f);
            if (vertexData.TexCoords != null)
              Gl.glMultiTexCoord2f(33984, vertexData.TexCoords[(int) vector3.Z].X, -vertexData.TexCoords[(int) vector3.Z].Y);
            if (vertexData.TexCoords2 != null)
              Gl.glMultiTexCoord2f(33985, vertexData.TexCoords2[(int) vector3.Z].X, -vertexData.TexCoords2[(int) vector3.Z].Y);
            if (vertexData.TexCoords3 != null)
              Gl.glMultiTexCoord2f(33986, vertexData.TexCoords3[(int) vector3.Z].X, -vertexData.TexCoords3[(int) vector3.Z].Y);
            Gl.glVertex3f(vertexData.Vertex[(int) vector3.Z].X, vertexData.Vertex[(int) vector3.Z].Y, vertexData.Vertex[(int) vector3.Z].Z);
          }
          Gl.glEnd();
        }
      }
      Gl.glPopMatrix();
      this.simpleOpenGlControl1.Refresh();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      switch (keyData)
      {
        case Keys.Escape:
          CGFX.X = 0.0f;
          CGFX.Y = 0.0f;
          CGFX.ang = 0.0f;
          CGFX.dist = 0.0f;
          CGFX.elev = 0.0f;
          this.Render();
          return true;
        case Keys.Left:
          --CGFX.ang;
          this.Render();
          return true;
        case Keys.Up:
          ++CGFX.elev;
          this.Render();
          return true;
        case Keys.Right:
          ++CGFX.ang;
          this.Render();
          return true;
        case Keys.Down:
          --CGFX.elev;
          this.Render();
          return true;
        case Keys.A:
          CGFX.Y -= 5f;
          this.Render();
          return true;
        case Keys.L:
          this.licht = !this.licht;
          this.Render();
          return true;
        case Keys.S:
          CGFX.Y += 5f;
          this.Render();
          return true;
        case Keys.T:
          CGFX.elev = 90f;
          CGFX.ang = 0.0f;
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
          CGFX.X += 5f;
          this.Render();
          return true;
        case Keys.Z:
          CGFX.X -= 5f;
          this.Render();
          return true;
        default:
          return base.ProcessCmdKey(ref msg, keyData);
      }
    }

    private void CGFX_Resize(object sender, EventArgs e)
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
      this.simpleOpenGlControl1 = new SimpleOpenGlControl();
      this.SuspendLayout();
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
      this.simpleOpenGlControl1.Size = new Size(574, 386);
      this.simpleOpenGlControl1.StencilBits = (byte) 8;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(574, 386);
      this.Controls.Add((Control) this.simpleOpenGlControl1);
      this.Name = nameof (CGFX);
      this.Text = nameof (CGFX);
      this.Load += new EventHandler(this.CGFX_Load);
      this.Resize += new EventHandler(this.CGFX_Resize);
      this.ResumeLayout(false);
    }
  }
}
