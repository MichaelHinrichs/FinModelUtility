// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.ObjectDbWikiTextGenerator
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.MKDS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class ObjectDbWikiTextGenerator : Form
  {
    private IContainer components = (IContainer) null;
    private ObjectDb.Object Object;
    private Label label1;
    private TextBox textBox1;
    private Label label2;
    private Panel panel1;
    private Panel panel2;
    private Panel panel3;
    private TextBox textBox2;

    public ObjectDbWikiTextGenerator(ObjectDb.Object o)
    {
      this.Object = o;
      this.InitializeComponent();
    }

    private void ObjectDbWikiTextGenerator_Load(object sender, EventArgs e)
    {
      this.textBox1.Text = "NKM (File Format)/Object ID/" + BitConverter.ToString(BitConverter.GetBytes(this.Object.ObjectId)).Replace("-", "");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("{{MKDSCourseModifierGenerated}}");
      stringBuilder.AppendLine("{| class=\"wikitable\" style=\"float:right\" width=\"250px\"");
      stringBuilder.AppendLine("! colspan=2 | " + this.Object.Name);
      if (this.Object.GotPicture())
      {
        stringBuilder.AppendLine("|-");
        stringBuilder.AppendLine("| colspan=2 align=center | [[File:" + BitConverter.ToString(BitConverter.GetBytes(this.Object.ObjectId)).Replace("-", "") + ".png]]");
      }
      stringBuilder.AppendLine("|-");
      stringBuilder.AppendLine("! Route Required");
      stringBuilder.AppendLine("| " + this.Object.RouteRequired.ToString());
      stringBuilder.AppendLine("|}");
      stringBuilder.AppendLine("This page describes the object '''" + this.Object.Name + "'''.");
      stringBuilder.AppendLine("<br><br><br><br><br><br><br><br><br><br>");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("== Description ==");
      stringBuilder.AppendLine(this.Object.Description);
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("== Required Files ==");
      if (this.Object.RequiredFiles.Count == 0)
      {
        stringBuilder.AppendLine("No additional files are required for this object.");
      }
      else
      {
        stringBuilder.AppendLine("{| class=\"wikitable\"");
        stringBuilder.AppendLine("!Filename");
        foreach (ObjectDb.Object.File requiredFile in this.Object.RequiredFiles)
        {
          stringBuilder.AppendLine("|-");
          stringBuilder.AppendLine("|" + requiredFile.FileName);
        }
        stringBuilder.AppendLine("|}");
      }
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("== Properties ==");
      stringBuilder.Append("{{Object|");
      if (this.Object.Setting1.Count == 1)
        stringBuilder.Append(this.Object.Setting1[0].Name + "||");
      else
        stringBuilder.Append(this.Object.Setting1[0].Name + "|" + this.Object.Setting1[1].Name + "|");
      if (this.Object.Setting2.Count == 1)
        stringBuilder.Append(this.Object.Setting2[0].Name + "||");
      else
        stringBuilder.Append(this.Object.Setting2[0].Name + "|" + this.Object.Setting2[1].Name + "|");
      if (this.Object.Setting3.Count == 1)
        stringBuilder.Append(this.Object.Setting3[0].Name + "||");
      else
        stringBuilder.Append(this.Object.Setting3[0].Name + "|" + this.Object.Setting3[1].Name + "|");
      if (this.Object.Setting4.Count == 1)
        stringBuilder.Append(this.Object.Setting4[0].Name + "||");
      else
        stringBuilder.Append(this.Object.Setting4[0].Name + "|" + this.Object.Setting4[1].Name + "|");
      stringBuilder.AppendLine("}}");
      this.textBox2.Text = stringBuilder.ToString();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.label1 = new Label();
      this.textBox1 = new TextBox();
      this.label2 = new Label();
      this.panel1 = new Panel();
      this.panel2 = new Panel();
      this.panel3 = new Panel();
      this.textBox2 = new TextBox();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel3.SuspendLayout();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Dock = DockStyle.Left;
      this.label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label1.Location = new Point(3, 3);
      this.label1.Name = "label1";
      this.label1.Padding = new Padding(0, 3, 0, 0);
      this.label1.Size = new Size(76, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Page Name:";
      this.textBox1.Dock = DockStyle.Fill;
      this.textBox1.Location = new Point(79, 3);
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new Size(420, 20);
      this.textBox1.TabIndex = 1;
      this.label2.AutoSize = true;
      this.label2.Dock = DockStyle.Left;
      this.label2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label2.Location = new Point(3, 3);
      this.label2.Name = "label2";
      this.label2.Size = new Size(55, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Content:";
      this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add((Control) this.textBox1);
      this.panel1.Controls.Add((Control) this.label1);
      this.panel1.Dock = DockStyle.Top;
      this.panel1.Location = new Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new Padding(3);
      this.panel1.Size = new Size(502, 26);
      this.panel1.TabIndex = 3;
      this.panel2.Controls.Add((Control) this.label2);
      this.panel2.Dock = DockStyle.Top;
      this.panel2.Location = new Point(0, 26);
      this.panel2.Name = "panel2";
      this.panel2.Padding = new Padding(3);
      this.panel2.Size = new Size(502, 19);
      this.panel2.TabIndex = 4;
      this.panel3.Controls.Add((Control) this.textBox2);
      this.panel3.Dock = DockStyle.Fill;
      this.panel3.Location = new Point(0, 45);
      this.panel3.Name = "panel3";
      this.panel3.Size = new Size(502, 333);
      this.panel3.TabIndex = 5;
      this.textBox2.Dock = DockStyle.Fill;
      this.textBox2.Location = new Point(0, 0);
      this.textBox2.Multiline = true;
      this.textBox2.Name = "textBox2";
      this.textBox2.ReadOnly = true;
      this.textBox2.ScrollBars = ScrollBars.Vertical;
      this.textBox2.Size = new Size(502, 333);
      this.textBox2.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(502, 378);
      this.Controls.Add((Control) this.panel3);
      this.Controls.Add((Control) this.panel2);
      this.Controls.Add((Control) this.panel1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (ObjectDbWikiTextGenerator);
      this.Text = nameof (ObjectDbWikiTextGenerator);
      this.Load += new EventHandler(this.ObjectDbWikiTextGenerator_Load);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel3.ResumeLayout(false);
      this.panel3.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
