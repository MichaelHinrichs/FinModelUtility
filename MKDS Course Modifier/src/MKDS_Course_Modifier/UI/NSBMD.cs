// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.NSBMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using MKDS_Course_Modifier.Language;
using MKDS_Course_Modifier.Misc;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI
{
  public class NSBMD : Form
  {
    public static float X = 0.0f;
    public static float Y = 0.0f;
    public static float ang = 0.0f;
    public static float dist = 0.0f;
    public static float elev = 0.0f;
    private IContainer components = (IContainer) null;
    private int SelMdl = 0;
    private NSBCA Bca = (NSBCA) null;
    private MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA Bta = (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA) null;
    private NSBTP Btp = (NSBTP) null;
    private NSBMA Bma = (NSBMA) null;
    private NSBVA Bva = (NSBVA) null;
    private bool wire = false;
    private bool licht = true;
    private int SelBcaAnim = 0;
    private int BcaFrameNumber = 0;
    private int BcaNumFrames = 0;
    private int SelBtaAnim = 0;
    private int BtaFrameNumber = 0;
    private int BtaNumFrames = 0;
    private int SelBtpAnim = 0;
    private int BtpFrameNumber = 0;
    private int BtpNumFrames = 0;
    private int SelBmaAnim = 0;
    private int BmaFrameNumber = 0;
    private int BmaNumFrames = 0;
    private int SelBvaAnim = 0;
    private int BvaFrameNumber = 0;
    private int BvaNumFrames = 0;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private Timer timer1;
    private MainMenu mainMenu1;
    private MenuItem menuItem1;
    private MenuItem menuItem3;
    private MenuItem menuItem4;
    private MenuItem menuItem2;
    private MenuItem menuItem8;
    private MenuItem menuItem5;
    private MenuItem menuItem7;
    private MenuItem menuItem6;
    private MenuItem menuItem9;
    private MenuItem menuItem10;
    private MenuItem menuItem11;
    private MenuItem menuItem12;
    private MenuItem menuItem13;
    private SaveFileDialog saveFileDialog1;
    private SaveFileDialog saveFileDialog2;
    private MenuItem menuItem14;
    private MenuItem menuItem15;
    private MenuItem menuItem16;
    private SaveFileDialog saveFileDialog3;
    private MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD file;

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
      this.menuItem3 = new MenuItem();
      this.menuItem4 = new MenuItem();
      this.menuItem2 = new MenuItem();
      this.menuItem8 = new MenuItem();
      this.menuItem5 = new MenuItem();
      this.menuItem7 = new MenuItem();
      this.menuItem6 = new MenuItem();
      this.menuItem9 = new MenuItem();
      this.menuItem10 = new MenuItem();
      this.menuItem11 = new MenuItem();
      this.menuItem12 = new MenuItem();
      this.menuItem13 = new MenuItem();
      this.menuItem16 = new MenuItem();
      this.menuItem14 = new MenuItem();
      this.menuItem15 = new MenuItem();
      this.saveFileDialog1 = new SaveFileDialog();
      this.saveFileDialog2 = new SaveFileDialog();
      this.saveFileDialog3 = new SaveFileDialog();
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
      this.simpleOpenGlControl1.StencilBits = (byte) 8;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.timer1.Interval = 5;
      this.timer1.Tick += new EventHandler(this.timer1_Tick);
      this.mainMenu1.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem1,
        this.menuItem9,
        this.menuItem14
      });
      this.menuItem1.Index = 0;
      this.menuItem1.MenuItems.AddRange(new MenuItem[7]
      {
        this.menuItem3,
        this.menuItem4,
        this.menuItem2,
        this.menuItem8,
        this.menuItem5,
        this.menuItem7,
        this.menuItem6
      });
      this.menuItem1.Text = "Animation";
      this.menuItem3.Index = 0;
      this.menuItem3.Text = nameof (NSBMD);
      this.menuItem4.Index = 1;
      this.menuItem4.Text = "-";
      this.menuItem2.Index = 2;
      this.menuItem2.Text = "NSBCA";
      this.menuItem8.Index = 3;
      this.menuItem8.Text = "NSBMA";
      this.menuItem5.Index = 4;
      this.menuItem5.Text = "NSBTA";
      this.menuItem7.Index = 5;
      this.menuItem7.Text = "NSBTP";
      this.menuItem6.Index = 6;
      this.menuItem6.Text = "NSBVA";
      this.menuItem9.Index = 1;
      this.menuItem9.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem10,
        this.menuItem12,
        this.menuItem16
      });
      this.menuItem9.Text = "Export";
      this.menuItem10.Index = 0;
      this.menuItem10.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem11
      });
      this.menuItem10.Text = "Model";
      this.menuItem11.Index = 0;
      this.menuItem11.Text = "OBJ";
      this.menuItem11.Click += new EventHandler(this.menuItem11_Click);
      this.menuItem12.Index = 1;
      this.menuItem12.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem13
      });
      this.menuItem12.Text = "Bones";
      this.menuItem13.Index = 0;
      this.menuItem13.Text = "MA";
      this.menuItem13.Click += new EventHandler(this.menuItem13_Click);
      this.menuItem16.Index = 2;
      this.menuItem16.Text = "Screenshot (Translucent)";
      this.menuItem16.Click += new EventHandler(this.menuItem16_Click);
      this.menuItem14.Index = 2;
      this.menuItem14.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem15
      });
      this.menuItem14.Text = "Nitro Viewer";
      this.menuItem15.Index = 0;
      this.menuItem15.Text = "Connect";
      this.menuItem15.Click += new EventHandler(this.menuItem15_Click);
      this.saveFileDialog1.DefaultExt = "obj";
      this.saveFileDialog1.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
      this.saveFileDialog2.DefaultExt = "ma";
      this.saveFileDialog2.Filter = "Maya Ascii Files(*.ma)|*.ma";
      this.saveFileDialog3.DefaultExt = "png";
      this.saveFileDialog3.Filter = "PNG Images (*.png)|*.png";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(660, 439);
      this.Controls.Add((Control) this.simpleOpenGlControl1);
      this.Menu = (System.Windows.Forms.MainMenu) this.mainMenu1;
      this.Name = nameof (NSBMD);
      this.Text = nameof (NSBMD);
      this.Load += new EventHandler(this.NSBMD_Load);
      this.Resize += new EventHandler(this.NSBMD_Resize);
      this.ResumeLayout(false);
    }

    public NSBMD(MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD file)
    {
      this.file = file;
      this.InitializeComponent();
      this.simpleOpenGlControl1.MouseWheel += new MouseEventHandler(this.simpleOpenGlControl1_MouseWheel);
      this.menuItem1.Text = LanguageHandler.GetString("3d.animation");
      this.menuItem9.Text = LanguageHandler.GetString("base.export");
      this.menuItem10.Text = LanguageHandler.GetString("3d.model");
      this.menuItem12.Text = LanguageHandler.GetString("3d.bones");
    }

    private void simpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
        NSBMD.dist += (float) e.Delta / 100f;
      else
        NSBMD.dist += (float) e.Delta / 1000f;
      this.Render();
    }

    private void NSBMD_Load(object sender, EventArgs e)
    {
      for (int index = 0; index < (int) this.file.modelSet.dict.numEntry; ++index)
      {
        this.menuItem3.MenuItems.Add(this.file.modelSet.dict[index].Key, new EventHandler(this.menuItem3_Click));
        this.menuItem3.MenuItems[index].RadioCheck = true;
      }
      this.menuItem3.MenuItems[0].Checked = true;
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
      Gl.glClearColor(0.2f, 0.2f, 0.2f, 0.0f);
      GlNitro2.glNitroBindTextures(this.file, 1);
      this.Render();
    }

    public void menuItem3_Click(object sender, EventArgs e)
    {
      ((MenuItem) sender).Checked = true;
      this.SelMdl = ((MenuItem) sender).Index;
      foreach (MenuItem menuItem in this.menuItem3.MenuItems)
      {
        if (menuItem != (MenuItem) sender)
          menuItem.Checked = false;
      }
      this.Render();
    }

    public void menuItem2_Click(object sender, EventArgs e)
    {
      ((MenuItem) sender).Checked = true;
      this.SelBcaAnim = ((MenuItem) sender).Index - 2;
      if (this.SelBcaAnim >= 0)
        this.BcaNumFrames = (int) this.Bca.jntAnmSet.jntAnm[this.SelBcaAnim].numFrame;
      this.SetZeroFrame();
      foreach (MenuItem menuItem in this.menuItem2.MenuItems)
      {
        if (menuItem != (MenuItem) sender)
          menuItem.Checked = false;
      }
      this.Render();
    }

    public void menuItem5_Click(object sender, EventArgs e)
    {
      ((MenuItem) sender).Checked = true;
      this.SelBtaAnim = ((MenuItem) sender).Index - 2;
      if (this.SelBtaAnim >= 0)
        this.BtaNumFrames = (int) this.Bta.texSRTAnmSet.texSRTAnm[this.SelBtaAnim].numFrame;
      this.SetZeroFrame();
      foreach (MenuItem menuItem in this.menuItem5.MenuItems)
      {
        if (menuItem != (MenuItem) sender)
          menuItem.Checked = false;
      }
      this.Render();
    }

    public void menuItem7_Click(object sender, EventArgs e)
    {
      ((MenuItem) sender).Checked = true;
      this.SelBtpAnim = ((MenuItem) sender).Index - 2;
      GlNitro2.glNitroBindTextures(this.file, 1);
      if (this.SelBtpAnim >= 0)
        this.BtpNumFrames = (int) this.Btp.texPatAnmSet.texPatAnm[this.SelBtpAnim].numFrame;
      this.SetZeroFrame();
      foreach (MenuItem menuItem in this.menuItem7.MenuItems)
      {
        if (menuItem != (MenuItem) sender)
          menuItem.Checked = false;
      }
      this.Render();
    }

    public void menuItem8_Click(object sender, EventArgs e)
    {
      ((MenuItem) sender).Checked = true;
      this.SelBmaAnim = ((MenuItem) sender).Index - 2;
      if (this.SelBmaAnim >= 0)
        this.BmaNumFrames = (int) this.Bma.matColAnmSet.matColAnm[this.SelBmaAnim].numFrame;
      this.SetZeroFrame();
      foreach (MenuItem menuItem in this.menuItem8.MenuItems)
      {
        if (menuItem != (MenuItem) sender)
          menuItem.Checked = false;
      }
      this.Render();
    }

    public void menuItem6_Click(object sender, EventArgs e)
    {
      ((MenuItem) sender).Checked = true;
      this.SelBvaAnim = ((MenuItem) sender).Index - 2;
      if (this.SelBvaAnim >= 0)
        this.BvaNumFrames = (int) this.Bva.visAnmSet.visAnm[this.SelBvaAnim].numFrame;
      this.SetZeroFrame();
      foreach (MenuItem menuItem in this.menuItem6.MenuItems)
      {
        if (menuItem != (MenuItem) sender)
          menuItem.Checked = false;
      }
      this.Render();
    }

    private void SetZeroFrame()
    {
      this.BtaFrameNumber = 0;
      this.BcaFrameNumber = 0;
      this.BtpFrameNumber = 0;
      this.BmaFrameNumber = 0;
      this.BvaFrameNumber = 0;
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
      Gl.glTranslatef(NSBMD.X, NSBMD.Y, -NSBMD.dist);
      Gl.glRotatef(NSBMD.elev, 1f, 0.0f, 0.0f);
      Gl.glRotatef(NSBMD.ang, 0.0f, 1f, 0.0f);
      Gl.glPushMatrix();
      this.file.modelSet.models[this.SelMdl].ProcessSbc(this.file.TexPlttSet, this.Bca, this.SelBcaAnim, this.BcaFrameNumber, this.Bta, this.SelBtaAnim, this.BtaFrameNumber, this.Btp, this.SelBtpAnim, this.BtpFrameNumber, this.Bma, this.SelBmaAnim, this.BmaFrameNumber, this.Bva, this.SelBvaAnim, this.BvaFrameNumber, NSBMD.X, NSBMD.Y, NSBMD.dist, NSBMD.elev, NSBMD.ang, false, 1);
      Gl.glPopMatrix();
      this.simpleOpenGlControl1.Refresh();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      switch (keyData)
      {
        case Keys.Escape:
          NSBMD.X = 0.0f;
          NSBMD.Y = 0.0f;
          NSBMD.ang = 0.0f;
          NSBMD.dist = 0.0f;
          NSBMD.elev = 0.0f;
          this.Render();
          return true;
        case Keys.Left:
          --NSBMD.ang;
          this.Render();
          return true;
        case Keys.Up:
          ++NSBMD.elev;
          this.Render();
          return true;
        case Keys.Right:
          ++NSBMD.ang;
          this.Render();
          return true;
        case Keys.Down:
          --NSBMD.elev;
          this.Render();
          return true;
        case Keys.A:
          NSBMD.Y -= 0.05f;
          this.Render();
          return true;
        case Keys.L:
          this.licht = !this.licht;
          this.Render();
          return true;
        case Keys.S:
          NSBMD.Y += 0.05f;
          this.Render();
          return true;
        case Keys.T:
          NSBMD.elev = 90f;
          NSBMD.ang = 0.0f;
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
          NSBMD.X += 0.05f;
          this.Render();
          return true;
        case Keys.Z:
          NSBMD.X -= 0.05f;
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

    public void SetNSBCA(NSBCA Bca)
    {
      this.Bca = Bca;
      if (Bca.jntAnmSet.dict.numEntry != (byte) 0)
      {
        this.SelBcaAnim = 0;
        this.BcaNumFrames = (int) Bca.jntAnmSet.jntAnm[this.SelBcaAnim].numFrame;
      }
      else
      {
        this.SelBcaAnim = -1;
        this.BcaNumFrames = 0;
      }
      this.SetZeroFrame();
      this.menuItem2.MenuItems.Clear();
      this.menuItem2.MenuItems.Add(LanguageHandler.GetString("3d.bindpose"), new EventHandler(this.menuItem2_Click));
      this.menuItem2.MenuItems.Add("-");
      for (int index = 0; index < (int) Bca.jntAnmSet.dict.numEntry; ++index)
      {
        this.menuItem2.MenuItems.Add(Bca.jntAnmSet.dict[index].Key, new EventHandler(this.menuItem2_Click));
        this.menuItem2.MenuItems[index].RadioCheck = true;
      }
      if (Bca.jntAnmSet.dict.numEntry != (byte) 0)
        this.menuItem2.MenuItems[2].Checked = true;
      else
        this.menuItem2.MenuItems[0].Checked = true;
      this.timer1.Start();
    }

    public void SetNSBTA(MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA Bta)
    {
      this.Bta = Bta;
      this.SelBtaAnim = 0;
      this.BtaNumFrames = (int) Bta.texSRTAnmSet.texSRTAnm[this.SelBtaAnim].numFrame;
      this.SetZeroFrame();
      this.menuItem5.MenuItems.Clear();
      this.menuItem5.MenuItems.Add(LanguageHandler.GetString("3d.bindpose"), new EventHandler(this.menuItem5_Click));
      this.menuItem5.MenuItems.Add("-");
      for (int index = 0; index < (int) Bta.texSRTAnmSet.dict.numEntry; ++index)
      {
        this.menuItem5.MenuItems.Add(Bta.texSRTAnmSet.dict[index].Key, new EventHandler(this.menuItem5_Click));
        this.menuItem5.MenuItems[index].RadioCheck = true;
      }
      this.menuItem5.MenuItems[2].Checked = true;
      this.timer1.Start();
    }

    public void SetNSBTP(NSBTP Btp)
    {
      this.Btp = Btp;
      this.SelBtpAnim = 0;
      this.BtpNumFrames = (int) Btp.texPatAnmSet.texPatAnm[this.SelBtpAnim].numFrame;
      this.SetZeroFrame();
      this.menuItem7.MenuItems.Clear();
      this.menuItem7.MenuItems.Add(LanguageHandler.GetString("3d.bindpose"), new EventHandler(this.menuItem7_Click));
      this.menuItem7.MenuItems.Add("-");
      for (int index = 0; index < (int) Btp.texPatAnmSet.dict.numEntry; ++index)
      {
        this.menuItem7.MenuItems.Add(Btp.texPatAnmSet.dict[index].Key, new EventHandler(this.menuItem7_Click));
        this.menuItem7.MenuItems[index].RadioCheck = true;
      }
      this.menuItem7.MenuItems[2].Checked = true;
      this.timer1.Start();
    }

    public void SetNSBMA(NSBMA Bma)
    {
      this.Bma = Bma;
      this.SelBmaAnim = 0;
      this.BmaNumFrames = (int) Bma.matColAnmSet.matColAnm[this.SelBmaAnim].numFrame;
      this.SetZeroFrame();
      this.menuItem8.MenuItems.Clear();
      this.menuItem8.MenuItems.Add(LanguageHandler.GetString("3d.bindpose"), new EventHandler(this.menuItem8_Click));
      this.menuItem8.MenuItems.Add("-");
      for (int index = 0; index < (int) Bma.matColAnmSet.dict.numEntry; ++index)
      {
        this.menuItem8.MenuItems.Add(Bma.matColAnmSet.dict[index].Key, new EventHandler(this.menuItem8_Click));
        this.menuItem8.MenuItems[index].RadioCheck = true;
      }
      this.menuItem8.MenuItems[2].Checked = true;
      this.timer1.Start();
    }

    public void SetNSBVA(NSBVA Bva)
    {
      this.Bva = Bva;
      this.SelBvaAnim = 0;
      this.BvaNumFrames = (int) Bva.visAnmSet.visAnm[this.SelBvaAnim].numFrame;
      this.SetZeroFrame();
      this.menuItem6.MenuItems.Clear();
      this.menuItem6.MenuItems.Add(LanguageHandler.GetString("3d.bindpose"), new EventHandler(this.menuItem6_Click));
      this.menuItem6.MenuItems.Add("-");
      for (int index = 0; index < (int) Bva.visAnmSet.dict.numEntry; ++index)
      {
        this.menuItem6.MenuItems.Add(Bva.visAnmSet.dict[index].Key, new EventHandler(this.menuItem6_Click));
        this.menuItem6.MenuItems[index].RadioCheck = true;
      }
      this.menuItem6.MenuItems[2].Checked = true;
      this.timer1.Start();
    }

    public void SetNSBTX(MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX Btx)
    {
      this.file.TexPlttSet = Btx.TexPlttSet;
      GlNitro2.glNitroBindTextures(this.file, 1);
      this.Render();
    }

    public void SetNSBMD(MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD Bmd)
    {
      this.file = Bmd;
      this.timer1.Stop();
      this.Btp = (NSBTP) null;
      this.Bta = (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA) null;
      this.Bca = (NSBCA) null;
      this.Bma = (NSBMA) null;
      this.Bva = (NSBVA) null;
      this.SelBcaAnim = 0;
      this.SelBmaAnim = 0;
      this.SelBtaAnim = 0;
      this.SelBtpAnim = 0;
      this.SelBvaAnim = 0;
      this.SelMdl = 0;
      this.menuItem3.MenuItems.Clear();
      for (int index = 0; index < (int) this.file.modelSet.dict.numEntry; ++index)
      {
        this.menuItem3.MenuItems.Add(this.file.modelSet.dict[index].Key, new EventHandler(this.menuItem3_Click));
        this.menuItem3.MenuItems[index].RadioCheck = true;
      }
      this.menuItem3.MenuItems[0].Checked = true;
      this.menuItem2.MenuItems.Clear();
      this.menuItem5.MenuItems.Clear();
      GlNitro2.glNitroBindTextures(this.file, 1);
      this.Render();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (this.BcaFrameNumber < this.BcaNumFrames - 1)
        ++this.BcaFrameNumber;
      else
        this.BcaFrameNumber = 0;
      if (this.BtaFrameNumber < this.BtaNumFrames - 1)
        ++this.BtaFrameNumber;
      else
        this.BtaFrameNumber = 0;
      if (this.BtpFrameNumber < this.BtpNumFrames - 1)
        ++this.BtpFrameNumber;
      else
        this.BtpFrameNumber = 0;
      if (this.BmaFrameNumber < this.BmaNumFrames - 1)
        ++this.BmaFrameNumber;
      else
        this.BmaFrameNumber = 0;
      if (this.BvaFrameNumber < this.BvaNumFrames - 1)
        ++this.BvaFrameNumber;
      else
        this.BvaFrameNumber = 0;
      this.Render();
    }

    private void menuItem13_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog2.ShowDialog() != DialogResult.OK || this.saveFileDialog2.FileName.Length <= 0)
        return;
      File.Create(this.saveFileDialog2.FileName).Close();
      File.WriteAllBytes(this.saveFileDialog2.FileName, this.file.modelSet.models[this.SelMdl].ExportBones());
    }

    private void menuItem11_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
        return;
      _3DExportSettings obj = new _3DExportSettings();
      int num = (int) obj.ShowDialog();
      this.file.modelSet.models[this.SelMdl].ExportMesh(this.file.TexPlttSet, this.saveFileDialog1.FileName, obj.Format);
    }

    private void menuItem15_Click(object sender, EventArgs e)
    {
      NNSMCS nnsmcs = new NNSMCS();
      string str = Path.GetDirectoryName(Application.ExecutablePath) + "\\NitroPreview.nsbmd";
      File.WriteAllBytes(str, this.file.Write());
      nnsmcs.SendFile(str);
      nnsmcs.Close();
    }

    private void menuItem16_Click(object sender, EventArgs e)
    {
      this.Render();
      System.Drawing.Bitmap bitmap = GlNitro2.ScreenShot(this.simpleOpenGlControl1);
      if (this.saveFileDialog3.ShowDialog() == DialogResult.OK && this.saveFileDialog3.FileName.Length > 0)
        bitmap.Save(this.saveFileDialog3.FileName, ImageFormat.Png);
      bitmap.Dispose();
    }
  }
}
