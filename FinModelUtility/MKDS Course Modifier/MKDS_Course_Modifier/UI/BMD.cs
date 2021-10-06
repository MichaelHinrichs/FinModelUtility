// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.BMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using MKDS_Course_Modifier.SM64DS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI
{
  public class BMD : Form
  {
    public static float X = 0.0f;
    public static float Y = 0.0f;
    public static float ang = 0.0f;
    public static float dist = 0.0f;
    public static float elev = 0.0f;
    private BCA Bca = (BCA) null;
    private bool wire = false;
    private bool licht = true;
    private int SelBcaAnim = 0;
    private int BcaFrameNumber = 0;
    private int BcaNumFrames = 0;
    private IContainer components = (IContainer) null;
    private MKDS_Course_Modifier.SM64DS.BMD file;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private Timer timer1;
    private MainMenu mainMenu1;
    private MenuItem menuItem1;
    private MenuItem menuItem2;
    private MenuItem menuItem8;
    private MenuItem menuItem5;
    private MenuItem menuItem7;
    private MenuItem menuItem6;

    public BMD(MKDS_Course_Modifier.SM64DS.BMD file)
    {
      this.file = file;
      this.InitializeComponent();
      this.simpleOpenGlControl1.MouseWheel += new MouseEventHandler(this.simpleOpenGlControl1_MouseWheel);
      this.menuItem1.Text = LanguageHandler.GetString("3d.animation");
    }

    private void simpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
        BMD.dist += (float) e.Delta / 100f;
      else
        BMD.dist += (float) e.Delta / 1000f;
      this.Render();
    }

    private void NSBMD_Load(object sender, EventArgs e)
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
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      Gl.glShadeModel(7425);
      Gl.glAlphaFunc(516, 0.0f);
      Gl.glClearColor(0.2f, 0.2f, 0.2f, 1f);
      GlNitro2.glNitroBindTextures(this.file, 1);
      this.Render();
    }

    public void menuItem2_Click(object sender, EventArgs e)
    {
      ((MenuItem) sender).Checked = true;
      this.SelBcaAnim = ((MenuItem) sender).Index - 2;
      if (this.SelBcaAnim >= 0)
        this.BcaNumFrames = (int) this.Bca.Header.NrFrames;
      this.BcaFrameNumber = 0;
      foreach (MenuItem menuItem in this.menuItem2.MenuItems)
      {
        if (menuItem != (MenuItem) sender)
          menuItem.Checked = false;
      }
      this.Render();
    }

    public void Render()
    {
      float num = (float) this.simpleOpenGlControl1.Width / (float) this.simpleOpenGlControl1.Height;
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      Glu.gluPerspective(30.0, (double) num, 0.100000001490116, 2048.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glBindTexture(3553, 0);
      Gl.glColor3f(1f, 1f, 1f);
      Gl.glClear(16640);
      Gl.glTranslatef(BMD.X, BMD.Y, -BMD.dist);
      Gl.glRotatef(BMD.elev, 1f, 0.0f, 0.0f);
      Gl.glRotatef(BMD.ang, 0.0f, 1f, 0.0f);
      Gl.glPushMatrix();
      this.file.Render(this.Bca, this.BcaFrameNumber);
      Gl.glPopMatrix();
      this.simpleOpenGlControl1.Refresh();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      switch (keyData)
      {
        case Keys.Escape:
          BMD.X = 0.0f;
          BMD.Y = 0.0f;
          BMD.ang = 0.0f;
          BMD.dist = 0.0f;
          BMD.elev = 0.0f;
          this.Render();
          return true;
        case Keys.Left:
          --BMD.ang;
          this.Render();
          return true;
        case Keys.Up:
          ++BMD.elev;
          this.Render();
          return true;
        case Keys.Right:
          ++BMD.ang;
          this.Render();
          return true;
        case Keys.Down:
          --BMD.elev;
          this.Render();
          return true;
        case Keys.A:
          BMD.Y -= 0.05f;
          this.Render();
          return true;
        case Keys.L:
          this.licht = !this.licht;
          this.Render();
          return true;
        case Keys.S:
          BMD.Y += 0.05f;
          this.Render();
          return true;
        case Keys.T:
          BMD.elev = 90f;
          BMD.ang = 0.0f;
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
          BMD.X += 0.05f;
          this.Render();
          return true;
        case Keys.Z:
          BMD.X -= 0.05f;
          this.Render();
          return true;
        default:
          return base.ProcessCmdKey(ref msg, keyData);
      }
    }

    private void NSBMD_Resize(object sender, EventArgs e)
    {
      this.Render();
    }

    public void SetBCA(BCA Bca)
    {
      this.Bca = Bca;
      this.SelBcaAnim = 0;
      this.BcaNumFrames = (int) Bca.Header.NrFrames;
      this.BcaFrameNumber = 0;
      this.menuItem2.MenuItems.Clear();
      this.menuItem2.MenuItems.Add(LanguageHandler.GetString("3d.bindpose"), new EventHandler(this.menuItem2_Click));
      this.menuItem2.MenuItems.Add("-");
      this.menuItem2.MenuItems.Add("Animation", new EventHandler(this.menuItem2_Click));
      this.menuItem2.MenuItems[0].RadioCheck = true;
      this.menuItem2.MenuItems[2].Checked = true;
      this.timer1.Start();
    }

    public void SetBMD(MKDS_Course_Modifier.SM64DS.BMD Bmd)
    {
      this.file = Bmd;
      this.timer1.Stop();
      this.Bca = (BCA) null;
      this.SelBcaAnim = 0;
      this.menuItem2.MenuItems.Clear();
      GlNitro2.glNitroBindTextures(this.file, 1);
      this.Render();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (this.BcaFrameNumber < this.BcaNumFrames - 1)
        ++this.BcaFrameNumber;
      else
        this.BcaFrameNumber = 0;
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
      this.simpleOpenGlControl1 = new SimpleOpenGlControl();
      this.timer1 = new Timer(this.components);
      this.mainMenu1 = new MainMenu(this.components);
      this.menuItem1 = new MenuItem();
      this.menuItem2 = new MenuItem();
      this.menuItem8 = new MenuItem();
      this.menuItem5 = new MenuItem();
      this.menuItem7 = new MenuItem();
      this.menuItem6 = new MenuItem();
      this.SuspendLayout();
      this.simpleOpenGlControl1.AccumBits = (byte) 0;
      this.simpleOpenGlControl1.AutoCheckErrors = false;
      this.simpleOpenGlControl1.AutoFinish = false;
      this.simpleOpenGlControl1.AutoMakeCurrent = true;
      this.simpleOpenGlControl1.AutoSwapBuffers = true;
      this.simpleOpenGlControl1.BackColor = Color.Black;
      this.simpleOpenGlControl1.ColorBits = (byte) 32;
      this.simpleOpenGlControl1.DepthBits = (byte) 24;
      this.simpleOpenGlControl1.Dock = DockStyle.Fill;
      this.simpleOpenGlControl1.Location = new Point(0, 0);
      this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
      this.simpleOpenGlControl1.Size = new Size(660, 439);
      this.simpleOpenGlControl1.StencilBits = (byte) 0;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.timer1.Interval = 5;
      this.timer1.Tick += new EventHandler(this.timer1_Tick);
      this.mainMenu1.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem1
      });
      this.menuItem1.Index = 0;
      this.menuItem1.MenuItems.AddRange(new MenuItem[5]
      {
        this.menuItem2,
        this.menuItem8,
        this.menuItem5,
        this.menuItem7,
        this.menuItem6
      });
      this.menuItem1.Text = "Animation";
      this.menuItem2.Index = 0;
      this.menuItem2.Text = "BCA";
      this.menuItem8.Index = 1;
      this.menuItem8.Text = "NSBMA";
      this.menuItem5.Index = 2;
      this.menuItem5.Text = "NSBTA";
      this.menuItem7.Index = 3;
      this.menuItem7.Text = "NSBTP";
      this.menuItem6.Index = 4;
      this.menuItem6.Text = "NSBVA";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(660, 439);
      this.Controls.Add((Control) this.simpleOpenGlControl1);
      this.Menu = (System.Windows.Forms.MainMenu) this.mainMenu1;
      this.Name = nameof (BMD);
      this.Text = nameof (BMD);
      this.Load += new EventHandler(this.NSBMD_Load);
      this.Resize += new EventHandler(this.NSBMD_Resize);
      this.ResumeLayout(false);
    }
  }
}
