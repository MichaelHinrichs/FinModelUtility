// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.BLO
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.GCN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI
{
  public class BLO : Form
  {
    private IContainer components = (IContainer) null;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private MKDS_Course_Modifier.GCN.BLO Layout;

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
      this.simpleOpenGlControl1.BackColor = System.Drawing.Color.Black;
      this.simpleOpenGlControl1.ColorBits = (byte) 32;
      this.simpleOpenGlControl1.DepthBits = (byte) 24;
      this.simpleOpenGlControl1.Dock = DockStyle.Fill;
      this.simpleOpenGlControl1.Location = new Point(0, 0);
      this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
      this.simpleOpenGlControl1.Size = new Size(672, 401);
      this.simpleOpenGlControl1.StencilBits = (byte) 8;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.simpleOpenGlControl1.Resize += new EventHandler(this.simpleOpenGlControl1_Resize);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(672, 401);
      this.Controls.Add((Control) this.simpleOpenGlControl1);
      this.Name = nameof (BLO);
      this.Text = nameof (BLO);
      this.Load += new EventHandler(this.BLO_Load);
      this.ResumeLayout(false);
    }

    public BLO(MKDS_Course_Modifier.GCN.BLO Layout)
    {
      this.Layout = Layout;
      this.InitializeComponent();
      this.simpleOpenGlControl1.InitializeContexts();
      this.ClientSize = new Size((int) Layout.Inf1.Width, (int) Layout.Inf1.Height);
    }

    private void BLO_Load(object sender, EventArgs e)
    {
      this.InitOpenGl();
      this.Render(false);
    }

    public void InitOpenGl()
    {
      Gl.glEnable(2903);
      Gl.glDisable(2929);
      Gl.glDepthFunc(519);
      Gl.glEnable(3057);
      Gl.glDisable(2884);
      Gl.glEnable(3553);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      this.Layout.BindTextures();
    }

    public void Render(bool picking = false)
    {
      Gl.glEnable(2903);
      Gl.glDisable(2929);
      Gl.glDepthFunc(519);
      Gl.glEnable(3057);
      Gl.glDisable(2884);
      Gl.glEnable(3553);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      Gl.glOrtho(0.0, (double) this.Layout.Inf1.Width, (double) this.Layout.Inf1.Height, 0.0, -1000.0, 1000.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      if (!picking)
        Gl.glClearColor(1f, 1f, 1f, 0.0f);
      else
        Gl.glClearColor(0.0f, 0.0f, 0.0f, 1f);
      Gl.glClear(16640);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glEnable(3553);
      Gl.glBindTexture(3553, 0);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glDisable(2884);
      Gl.glEnable(3008);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      Gl.glAlphaFunc(519, 0.0f);
      Gl.glLoadIdentity();
      Gl.glPushMatrix();
      Console.WriteLine("-------------------------");
      this.Layout.ROOT.Render(this.Layout);
      Console.WriteLine("-------------------------");
      Gl.glPopMatrix();
      this.simpleOpenGlControl1.Refresh();
    }

    public void DrawTest(float X, float Y, int MaterialID)
    {
      Gl.glPushMatrix();
      MKDS_Course_Modifier.GCN.BLO.MAT1.MaterialEntry materialEntry = this.Layout.Mat1.MaterialEntries[(int) this.Layout.Mat1.MaterialEntryIndieces[MaterialID]];
      this.Layout.Mat1.BlendFunctions[(int) materialEntry.BlendModeIdx].ApplyBlendMode();
      Gl.glEnable(3008);
      Gl.glAlphaFunc(519, 0.0f);
      List<int> intList = new List<int>();
      for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
      {
        if (materialEntry.TextureIndices[index] != (short) -1)
        {
          Gl.glActiveTexture(33984 + index);
          Gl.glBindTexture(3553, MaterialID * 8 + index + 1);
          Gl.glEnable(3553);
          intList.Add(MaterialID * 8 + index + 1);
        }
      }
      if (materialEntry.TextureIndices[0] == (short) -1)
      {
        Gl.glActiveTexture(33984);
        Gl.glColor4f(1f, 1f, 1f, 1f);
        Gl.glBindTexture(3553, MaterialID * 8 + 1);
        Gl.glEnable(3553);
        intList.Add(MaterialID * 8 + 1);
      }
      Gl.glMatrixMode(5890);
      for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
      {
        Gl.glActiveTexture(33984 + index);
        Gl.glLoadIdentity();
        if (materialEntry.TextureIndices[index] != (short) -1)
          ;
      }
      if (materialEntry.TextureIndices[0] == (short) -1)
      {
        Gl.glActiveTexture(33984);
        Gl.glLoadIdentity();
      }
      Gl.glMatrixMode(5888);
      if (materialEntry.Shader == null)
      {
        materialEntry.Shader = new BLOShader(materialEntry, this.Layout.Mat1, intList.ToArray());
        materialEntry.Shader.Compile();
      }
      materialEntry.Shader.Enable();
      Gl.glTranslatef(X, Y, 0.0f);
      Gl.glPushMatrix();
      float[,] numArray = new float[4, 2]
      {
        {
          0.0f,
          0.0f
        },
        {
          (float) this.Layout.Tex1.Textures[(int) this.Layout.Mat1.TextureIndieces[(int) materialEntry.TextureIndices[0]]].Header.Width,
          0.0f
        },
        {
          (float) this.Layout.Tex1.Textures[(int) this.Layout.Mat1.TextureIndieces[(int) materialEntry.TextureIndices[0]]].Header.Width,
          0.0f
        },
        {
          0.0f,
          0.0f
        }
      };
      numArray[0, 1] = 0.0f;
      numArray[1, 1] = 0.0f;
      numArray[2, 1] = (float) this.Layout.Tex1.Textures[(int) this.Layout.Mat1.TextureIndieces[(int) materialEntry.TextureIndices[0]]].Header.Height;
      numArray[3, 1] = (float) this.Layout.Tex1.Textures[(int) this.Layout.Mat1.TextureIndieces[(int) materialEntry.TextureIndices[0]]].Header.Height;
      Gl.glBegin(7);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
      {
        if (materialEntry.TextureIndices[index] != (short) -1)
          Gl.glMultiTexCoord2f(33984 + index, 0.0f, 0.0f);
      }
      if (materialEntry.TextureIndices[0] == (short) -1)
        Gl.glMultiTexCoord2f(33984, 0.0f, 0.0f);
      Gl.glVertex3f(numArray[0, 0], numArray[0, 1], 0.0f);
      for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
      {
        if (materialEntry.TextureIndices[index] != (short) -1)
          Gl.glMultiTexCoord2f(33984 + index, 1f, 0.0f);
      }
      if (materialEntry.TextureIndices[0] == (short) -1)
        Gl.glMultiTexCoord2f(33984, 1f, 0.0f);
      Gl.glVertex3f(numArray[1, 0], numArray[1, 1], 0.0f);
      for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
        Gl.glMultiTexCoord2f(33984 + index, 1f, 1f);
      if (materialEntry.TextureIndices[0] == (short) -1)
        Gl.glMultiTexCoord2f(33984, 1f, 1f);
      Gl.glVertex3f(numArray[2, 0], numArray[2, 1], 0.0f);
      for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
      {
        if (materialEntry.TextureIndices[index] != (short) -1)
          Gl.glMultiTexCoord2f(33984 + index, 0.0f, 1f);
      }
      if (materialEntry.TextureIndices[0] == (short) -1)
        Gl.glMultiTexCoord2f(33984, 0.0f, 1f);
      Gl.glVertex3f(numArray[3, 0], numArray[3, 1], 0.0f);
      Gl.glEnd();
      Gl.glPopMatrix();
      Gl.glPopMatrix();
    }

    private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
    {
      this.Render(false);
    }
  }
}
