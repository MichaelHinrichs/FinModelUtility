Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports System.Math
Imports System.IO
Imports Tao.FreeGlut
Imports UoT.animation
Imports UoT.limbs
Imports UoT.memory.files
Imports UoT.memory.map
Imports UoT.model
Imports UoT.ui.main.viewer

Public Class MainWin

#Region "Necessary for OpenGL Initialization"

  Public Sub New()
    MyBase.New()

    'This call is required by the Windows Form Designer.
    InitializeComponent()
    'Add any initialization after the InitializeComponent() call
    UoTRender.InitializeContexts()
  End Sub
  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()>
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing AndAlso components IsNot Nothing Then
      components.Dispose()
    End If
    MyBase.Dispose(disposing)
    UoTRender.DestroyContexts()
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer
  Friend WithEvents LoadROM As System.Windows.Forms.OpenFileDialog

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()>
  Private Sub InitializeComponent()
    Me.components = New System.ComponentModel.Container()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainWin))
    Me.LoadROM = New System.Windows.Forms.OpenFileDialog()
    Me.UoTStatus = New System.Windows.Forms.StatusStrip()
    Me.ToolStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
    Me.CamXLabel = New System.Windows.Forms.ToolStripStatusLabel()
    Me.CamYLabel = New System.Windows.Forms.ToolStripStatusLabel()
    Me.CamZLabel = New System.Windows.Forms.ToolStripStatusLabel()
    Me.ToolStripStatusLabel5 = New System.Windows.Forms.ToolStripStatusLabel()
    Me.ToolStripStatusLabel4 = New System.Windows.Forms.ToolStripStatusLabel()
    Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
    Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
    Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
    Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
    Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
    Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
    Me.Label12 = New System.Windows.Forms.Label()
    Me.TrackBar4 = New System.Windows.Forms.TrackBar()
    Me.FeaturesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.RenderModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ViewingMeshToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.CollisionMeshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.PrimitiveTypeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.FilledToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.WireframeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
    Me.MouseToolToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.SetupToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ActorInputTimer = New System.Windows.Forms.Timer(Me.components)
    Me.CollisionTab = New System.Windows.Forms.TabPage()
    Me.GroupBox1 = New System.Windows.Forms.GroupBox()
    Me.Button9 = New System.Windows.Forms.Button()
    Me.TriTypeText = New System.Windows.Forms.TextBox()
    Me.Label48 = New System.Windows.Forms.Label()
    Me.Label47 = New System.Windows.Forms.Label()
    Me.ColTriangleBox = New System.Windows.Forms.ComboBox()
    Me.CollisionGroupBox = New System.Windows.Forms.GroupBox()
    Me.Label36 = New System.Windows.Forms.Label()
    Me.ColWalkSound = New System.Windows.Forms.TextBox()
    Me.Label38 = New System.Windows.Forms.Label()
    Me.Label37 = New System.Windows.Forms.Label()
    Me.Label35 = New System.Windows.Forms.Label()
    Me.Label33 = New System.Windows.Forms.Label()
    Me.Label32 = New System.Windows.Forms.Label()
    Me.Label23 = New System.Windows.Forms.Label()
    Me.Label13 = New System.Windows.Forms.Label()
    Me.Label1 = New System.Windows.Forms.Label()
    Me.ColVar4 = New System.Windows.Forms.TextBox()
    Me.ApplyCollisionButton = New System.Windows.Forms.Button()
    Me.ColVar2 = New System.Windows.Forms.TextBox()
    Me.ColVar3 = New System.Windows.Forms.TextBox()
    Me.ColVar1 = New System.Windows.Forms.TextBox()
    Me.CollisionPresetButton = New System.Windows.Forms.Button()
    Me.Label34 = New System.Windows.Forms.Label()
    Me.ColTypeText = New System.Windows.Forms.TextBox()
    Me.ColTypeBox = New System.Windows.Forms.ComboBox()
    Me.Label31 = New System.Windows.Forms.Label()
    Me.ExitTextBox = New System.Windows.Forms.TextBox()
    Me.ExitCombobox = New System.Windows.Forms.ComboBox()
    Me.Label10 = New System.Windows.Forms.Label()
    Me.MiscTab = New System.Windows.Forms.TabPage()
    Me.GroupBox9 = New System.Windows.Forms.GroupBox()
    Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
    Me.Label28 = New System.Windows.Forms.Label()
    Me.Button16 = New System.Windows.Forms.Button()
    Me.Button15 = New System.Windows.Forms.Button()
    Me.Label45 = New System.Windows.Forms.Label()
    Me.AnimStart = New System.Windows.Forms.TextBox()
    Me.Label30 = New System.Windows.Forms.Label()
    Me.LimbStart = New System.Windows.Forms.TextBox()
    Me.GroupBox10 = New System.Windows.Forms.GroupBox()
    Me.CheckBox5 = New System.Windows.Forms.CheckBox()
    Me.CheckBox15 = New System.Windows.Forms.CheckBox()
    Me.CheckBox14 = New System.Windows.Forms.CheckBox()
    Me.CheckBox13 = New System.Windows.Forms.CheckBox()
    Me.GroupBox8 = New System.Windows.Forms.GroupBox()
    Me.MapsCombobox = New System.Windows.Forms.ComboBox()
    Me.Label46 = New System.Windows.Forms.Label()
    Me.LevelFlagsTab = New System.Windows.Forms.TabPage()
    Me.GroupBox6 = New System.Windows.Forms.GroupBox()
    Me.ComboBox6 = New System.Windows.Forms.ComboBox()
    Me.Label2 = New System.Windows.Forms.Label()
    Me.Button11 = New System.Windows.Forms.Button()
    Me.TextBox13 = New System.Windows.Forms.TextBox()
    Me.Label21 = New System.Windows.Forms.Label()
    Me.ActorsTab = New System.Windows.Forms.TabPage()
    Me.Button2 = New System.Windows.Forms.Button()
    Me.GroupBox4 = New System.Windows.Forms.GroupBox()
    Me.Label14 = New System.Windows.Forms.Label()
    Me.Label17 = New System.Windows.Forms.Label()
    Me.TextBox9 = New System.Windows.Forms.TextBox()
    Me.Label18 = New System.Windows.Forms.Label()
    Me.TextBox8 = New System.Windows.Forms.TextBox()
    Me.Label19 = New System.Windows.Forms.Label()
    Me.TextBox7 = New System.Windows.Forms.TextBox()
    Me.TextBox10 = New System.Windows.Forms.TextBox()
    Me.Label16 = New System.Windows.Forms.Label()
    Me.TextBox11 = New System.Windows.Forms.TextBox()
    Me.Label15 = New System.Windows.Forms.Label()
    Me.TextBox12 = New System.Windows.Forms.TextBox()
    Me.GroupBox5 = New System.Windows.Forms.GroupBox()
    Me.Button6 = New System.Windows.Forms.Button()
    Me.SceneActorCombobox = New System.Windows.Forms.ComboBox()
    Me.ActorNumberText = New System.Windows.Forms.TextBox()
    Me.Label7 = New System.Windows.Forms.Label()
    Me.ActorVarText = New System.Windows.Forms.TextBox()
    Me.Label8 = New System.Windows.Forms.Label()
    Me.Label6 = New System.Windows.Forms.Label()
    Me.ActorGroupText = New System.Windows.Forms.TextBox()
    Me.Label22 = New System.Windows.Forms.Label()
    Me.RoomActorCombobox = New System.Windows.Forms.ComboBox()
    Me.Label24 = New System.Windows.Forms.Label()
    Me.EditingTabs = New System.Windows.Forms.TabControl()
    Me.AnimationsTab = New System.Windows.Forms.TabPage()
    Me.animationTab_ = New UoT.ui.main.tabs.animation.AnimationTab()
    Me.DLTab = New System.Windows.Forms.TabPage()
    Me.RadioButton2 = New System.Windows.Forms.RadioButton()
    Me.GroupBox7 = New System.Windows.Forms.GroupBox()
    Me.WholeCommandTxt = New System.Windows.Forms.TextBox()
    Me.Label3 = New System.Windows.Forms.Label()
    Me.Button8 = New System.Windows.Forms.Button()
    Me.HiwordText = New System.Windows.Forms.TextBox()
    Me.Button1 = New System.Windows.Forms.Button()
    Me.LowordText = New System.Windows.Forms.TextBox()
    Me.CommandCodeText = New System.Windows.Forms.TextBox()
    Me.CommandJumpBox = New System.Windows.Forms.ComboBox()
    Me.Label26 = New System.Windows.Forms.Label()
    Me.Label25 = New System.Windows.Forms.Label()
    Me.Label9 = New System.Windows.Forms.Label()
    Me.Button4 = New System.Windows.Forms.Button()
    Me.CommandsListbox = New System.Windows.Forms.ListBox()
    Me.DLEditorContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
    Me.Copy = New System.Windows.Forms.ToolStripMenuItem()
    Me.Paste = New System.Windows.Forms.ToolStripMenuItem()
    Me.Reset = New System.Windows.Forms.ToolStripMenuItem()
    Me.DListSelection = New System.Windows.Forms.ComboBox()
    Me.RadioButton1 = New System.Windows.Forms.RadioButton()
    Me.GroupBox3 = New System.Windows.Forms.GroupBox()
    Me.Button12 = New System.Windows.Forms.Button()
    Me.Label4 = New System.Windows.Forms.Label()
    Me.Button10 = New System.Windows.Forms.Button()
    Me.BackupMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
    Me.RestorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.Label43 = New System.Windows.Forms.Label()
    Me.TrackBar1 = New System.Windows.Forms.TrackBar()
    Me.ActorContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
    Me.DeselectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripSeparator14 = New System.Windows.Forms.ToolStripSeparator()
    Me.EditToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
    Me.CamXRotationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.DegreesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.DegreesToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.CamYRotationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.DegreesToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
    Me.DegreesToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
    Me.CamZRotationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.DegreesToolStripMenuItem4 = New System.Windows.Forms.ToolStripMenuItem()
    Me.DegreesToolStripMenuItem5 = New System.Windows.Forms.ToolStripMenuItem()
    Me.AlignToolItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.XToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
    Me.YToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ZToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripSeparator13 = New System.Windows.Forms.ToolStripSeparator()
    Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.PasteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.PositionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.XToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.YToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ZToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.AllToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.RotationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.XToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
    Me.YToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ZToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.AllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.NumberAndVariableToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ClearClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.RotationTimer = New System.Windows.Forms.Timer(Me.components)
    Me.LoadIndividual = New System.Windows.Forms.OpenFileDialog()
    Me.FileToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem35 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
    Me.CustomLevel = New System.Windows.Forms.ToolStripMenuItem()
    Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
    Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem34 = New System.Windows.Forms.ToolStripMenuItem()
    Me.toolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator()
    Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.EditToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.UndoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.WireframeModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.RenderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.GraphicsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.CollisionOverlayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.MouseToolToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.CameraOnlyMenu = New System.Windows.Forms.ToolStripMenuItem()
    Me.ActorSelectorMenu = New System.Windows.Forms.ToolStripMenuItem()
    Me.CollisionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.CollisionSelectorMenu = New System.Windows.Forms.ToolStripMenuItem()
    Me.EdgeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.TriangleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.VertexToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.DisplayListSelectorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.LockAxesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.XToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.YToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.DisableToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.OptionsToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
    Me.DisableDepthTestToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.OptionsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
    Me.RendererToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.TexturesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.ColorCombinerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.AnisotropicFilteringToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.FullSceneAntialiasingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.UoTMainMenu = New System.Windows.Forms.MenuStrip()
    Me.VertContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
    Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripSeparator15 = New System.Windows.Forms.ToolStripSeparator()
    Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem7 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem8 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem9 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem10 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem11 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem12 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem13 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem14 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem15 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem16 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem17 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem18 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem19 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripSeparator16 = New System.Windows.Forms.ToolStripSeparator()
    Me.ToolStripMenuItem20 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem21 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem22 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem23 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem24 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem25 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem26 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem27 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem28 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem29 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem30 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem31 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem32 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolStripMenuItem33 = New System.Windows.Forms.ToolStripMenuItem()
    Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
    Me.RipDL = New System.Windows.Forms.SaveFileDialog()
    Me.SaveROMAs = New System.Windows.Forms.SaveFileDialog()
    Me.VarContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
    Me.NumContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
    Me.GrpContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
    Me.zFileTreeView_ = New UoT.ui.main.files.ZFileTreeView()
    Me.UoTRender = New UoT.Tao.Platform.Windows.SimpleOpenGlControl()
    Me.UoTStatus.SuspendLayout()
    CType(Me.TrackBar4, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.CollisionTab.SuspendLayout()
    Me.GroupBox1.SuspendLayout()
    Me.CollisionGroupBox.SuspendLayout()
    Me.MiscTab.SuspendLayout()
    Me.GroupBox9.SuspendLayout()
    Me.GroupBox10.SuspendLayout()
    Me.GroupBox8.SuspendLayout()
    Me.LevelFlagsTab.SuspendLayout()
    Me.GroupBox6.SuspendLayout()
    Me.ActorsTab.SuspendLayout()
    Me.GroupBox4.SuspendLayout()
    Me.GroupBox5.SuspendLayout()
    Me.EditingTabs.SuspendLayout()
    Me.AnimationsTab.SuspendLayout()
    Me.DLTab.SuspendLayout()
    Me.GroupBox7.SuspendLayout()
    Me.DLEditorContextMenu.SuspendLayout()
    Me.GroupBox3.SuspendLayout()
    Me.BackupMenuStrip.SuspendLayout()
    CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.ActorContextMenu.SuspendLayout()
    Me.UoTMainMenu.SuspendLayout()
    Me.VertContextMenu.SuspendLayout()
    Me.SuspendLayout()
    '
    'LoadROM
    '
    Me.LoadROM.Filter = "N64 ROMs (*.z64, *.v64, *.n64, *.rom)|*.z64;*.v64;*.n64"
    Me.LoadROM.Title = "Load Ocarina of Time Debug ROM"
    '
    'UoTStatus
    '
    Me.UoTStatus.AutoSize = False
    Me.UoTStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStatusLabel, Me.CamXLabel, Me.CamYLabel, Me.CamZLabel})
    Me.UoTStatus.Location = New System.Drawing.Point(0, 620)
    Me.UoTStatus.Name = "UoTStatus"
    Me.UoTStatus.Size = New System.Drawing.Size(1160, 29)
    Me.UoTStatus.TabIndex = 1
    Me.UoTStatus.Text = "UoTStatusStrip"
    '
    'ToolStatusLabel
    '
    Me.ToolStatusLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.ToolStatusLabel.ForeColor = System.Drawing.Color.Red
    Me.ToolStatusLabel.Name = "ToolStatusLabel"
    Me.ToolStatusLabel.Size = New System.Drawing.Size(95, 24)
    Me.ToolStatusLabel.Text = "Tool: Camera"
    '
    'CamXLabel
    '
    Me.CamXLabel.Name = "CamXLabel"
    Me.CamXLabel.Size = New System.Drawing.Size(45, 24)
    Me.CamXLabel.Text = "Cam X:"
    '
    'CamYLabel
    '
    Me.CamYLabel.Name = "CamYLabel"
    Me.CamYLabel.Size = New System.Drawing.Size(45, 24)
    Me.CamYLabel.Text = "Cam Y:"
    '
    'CamZLabel
    '
    Me.CamZLabel.Name = "CamZLabel"
    Me.CamZLabel.Size = New System.Drawing.Size(45, 24)
    Me.CamZLabel.Text = "Cam Z:"
    '
    'ToolStripStatusLabel5
    '
    Me.ToolStripStatusLabel5.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
    Me.ToolStripStatusLabel5.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
    Me.ToolStripStatusLabel5.Name = "ToolStripStatusLabel5"
    Me.ToolStripStatusLabel5.Size = New System.Drawing.Size(29, 22)
    Me.ToolStripStatusLabel5.Text = "FPS"
    '
    'ToolStripStatusLabel4
    '
    Me.ToolStripStatusLabel4.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
    Me.ToolStripStatusLabel4.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
    Me.ToolStripStatusLabel4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
    Me.ToolStripStatusLabel4.Font = New System.Drawing.Font("Trebuchet MS", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.ToolStripStatusLabel4.Name = "ToolStripStatusLabel4"
    Me.ToolStripStatusLabel4.Size = New System.Drawing.Size(111, 22)
    Me.ToolStripStatusLabel4.Text = "Tool: Camera only"
    '
    'ToolStripSeparator4
    '
    Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
    Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 41)
    '
    'ToolStripSeparator2
    '
    Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
    Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 41)
    '
    'ToolStripSeparator1
    '
    Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
    Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 41)
    '
    'ToolStripSeparator6
    '
    Me.ToolStripSeparator6.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
    Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
    Me.ToolStripSeparator6.RightToLeft = System.Windows.Forms.RightToLeft.No
    Me.ToolStripSeparator6.Size = New System.Drawing.Size(6, 41)
    '
    'ToolStripSeparator5
    '
    Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
    Me.ToolStripSeparator5.Size = New System.Drawing.Size(6, 41)
    '
    'ToolStripSeparator3
    '
    Me.ToolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
    Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
    Me.ToolStripSeparator3.RightToLeft = System.Windows.Forms.RightToLeft.No
    Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 41)
    '
    'Label12
    '
    Me.Label12.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.Label12.AutoSize = True
    Me.Label12.BackColor = System.Drawing.SystemColors.Control
    Me.Label12.Font = New System.Drawing.Font("Trebuchet MS", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.Label12.Location = New System.Drawing.Point(946, 626)
    Me.Label12.Name = "Label12"
    Me.Label12.Size = New System.Drawing.Size(94, 18)
    Me.Label12.TabIndex = 39
    Me.Label12.Text = "Tool Sensitivity:"
    '
    'TrackBar4
    '
    Me.TrackBar4.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.TrackBar4.AutoSize = False
    Me.TrackBar4.Cursor = System.Windows.Forms.Cursors.Hand
    Me.TrackBar4.LargeChange = 1
    Me.TrackBar4.Location = New System.Drawing.Point(1047, 627)
    Me.TrackBar4.Maximum = 15
    Me.TrackBar4.Minimum = 1
    Me.TrackBar4.Name = "TrackBar4"
    Me.TrackBar4.Size = New System.Drawing.Size(90, 15)
    Me.TrackBar4.TabIndex = 99
    Me.TrackBar4.Value = 2
    '
    'FeaturesToolStripMenuItem
    '
    Me.FeaturesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RenderModeToolStripMenuItem, Me.PrimitiveTypeToolStripMenuItem, Me.ToolStripSeparator8, Me.MouseToolToolStripMenuItem, Me.SetupToolStripMenuItem})
    Me.FeaturesToolStripMenuItem.Name = "FeaturesToolStripMenuItem"
    Me.FeaturesToolStripMenuItem.Size = New System.Drawing.Size(81, 17)
    Me.FeaturesToolStripMenuItem.Text = "Preferences"
    '
    'RenderModeToolStripMenuItem
    '
    Me.RenderModeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewingMeshToolStripMenuItem1, Me.CollisionMeshToolStripMenuItem})
    Me.RenderModeToolStripMenuItem.Name = "RenderModeToolStripMenuItem"
    Me.RenderModeToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
    Me.RenderModeToolStripMenuItem.Size = New System.Drawing.Size(174, 22)
    Me.RenderModeToolStripMenuItem.Text = "&Render"
    '
    'ViewingMeshToolStripMenuItem1
    '
    Me.ViewingMeshToolStripMenuItem1.Checked = True
    Me.ViewingMeshToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked
    Me.ViewingMeshToolStripMenuItem1.Name = "ViewingMeshToolStripMenuItem1"
    Me.ViewingMeshToolStripMenuItem1.Size = New System.Drawing.Size(152, 22)
    Me.ViewingMeshToolStripMenuItem1.Text = "Graphics Mesh"
    '
    'CollisionMeshToolStripMenuItem
    '
    Me.CollisionMeshToolStripMenuItem.Name = "CollisionMeshToolStripMenuItem"
    Me.CollisionMeshToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
    Me.CollisionMeshToolStripMenuItem.Text = "Collision Mesh"
    '
    'PrimitiveTypeToolStripMenuItem
    '
    Me.PrimitiveTypeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FilledToolStripMenuItem, Me.WireframeToolStripMenuItem})
    Me.PrimitiveTypeToolStripMenuItem.Name = "PrimitiveTypeToolStripMenuItem"
    Me.PrimitiveTypeToolStripMenuItem.ShortcutKeyDisplayString = "F1"
    Me.PrimitiveTypeToolStripMenuItem.Size = New System.Drawing.Size(174, 22)
    Me.PrimitiveTypeToolStripMenuItem.Text = "&Primitive Mode"
    '
    'FilledToolStripMenuItem
    '
    Me.FilledToolStripMenuItem.AutoSize = False
    Me.FilledToolStripMenuItem.Image = CType(resources.GetObject("FilledToolStripMenuItem.Image"), System.Drawing.Image)
    Me.FilledToolStripMenuItem.Name = "FilledToolStripMenuItem"
    Me.FilledToolStripMenuItem.Size = New System.Drawing.Size(152, 20)
    Me.FilledToolStripMenuItem.Text = "&Filled"
    '
    'WireframeToolStripMenuItem
    '
    Me.WireframeToolStripMenuItem.AutoSize = False
    Me.WireframeToolStripMenuItem.Image = CType(resources.GetObject("WireframeToolStripMenuItem.Image"), System.Drawing.Image)
    Me.WireframeToolStripMenuItem.Name = "WireframeToolStripMenuItem"
    Me.WireframeToolStripMenuItem.Size = New System.Drawing.Size(152, 20)
    Me.WireframeToolStripMenuItem.Text = "&Wireframe"
    '
    'ToolStripSeparator8
    '
    Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
    Me.ToolStripSeparator8.Size = New System.Drawing.Size(171, 6)
    '
    'MouseToolToolStripMenuItem
    '
    Me.MouseToolToolStripMenuItem.Name = "MouseToolToolStripMenuItem"
    Me.MouseToolToolStripMenuItem.Size = New System.Drawing.Size(174, 22)
    Me.MouseToolToolStripMenuItem.Text = "&Mouse Tool..."
    '
    'SetupToolStripMenuItem
    '
    Me.SetupToolStripMenuItem.Name = "SetupToolStripMenuItem"
    Me.SetupToolStripMenuItem.ShortcutKeyDisplayString = ""
    Me.SetupToolStripMenuItem.Size = New System.Drawing.Size(174, 22)
    Me.SetupToolStripMenuItem.Text = "&Setup..."
    '
    'ActorInputTimer
    '
    Me.ActorInputTimer.Interval = 1
    '
    'CollisionTab
    '
    Me.CollisionTab.Controls.Add(Me.GroupBox1)
    Me.CollisionTab.Controls.Add(Me.CollisionGroupBox)
    Me.CollisionTab.Location = New System.Drawing.Point(4, 25)
    Me.CollisionTab.Name = "CollisionTab"
    Me.CollisionTab.Padding = New System.Windows.Forms.Padding(3)
    Me.CollisionTab.Size = New System.Drawing.Size(224, 558)
    Me.CollisionTab.TabIndex = 4
    Me.CollisionTab.Text = "Collision"
    Me.CollisionTab.UseVisualStyleBackColor = True
    '
    'GroupBox1
    '
    Me.GroupBox1.Controls.Add(Me.Button9)
    Me.GroupBox1.Controls.Add(Me.TriTypeText)
    Me.GroupBox1.Controls.Add(Me.Label48)
    Me.GroupBox1.Controls.Add(Me.Label47)
    Me.GroupBox1.Controls.Add(Me.ColTriangleBox)
    Me.GroupBox1.Location = New System.Drawing.Point(3, 9)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(218, 112)
    Me.GroupBox1.TabIndex = 93
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "Triangles"
    '
    'Button9
    '
    Me.Button9.Location = New System.Drawing.Point(155, 83)
    Me.Button9.Name = "Button9"
    Me.Button9.Size = New System.Drawing.Size(56, 23)
    Me.Button9.TabIndex = 4
    Me.Button9.Text = "Apply"
    Me.Button9.UseVisualStyleBackColor = True
    '
    'TriTypeText
    '
    Me.TriTypeText.Location = New System.Drawing.Point(138, 46)
    Me.TriTypeText.Name = "TriTypeText"
    Me.TriTypeText.Size = New System.Drawing.Size(56, 20)
    Me.TriTypeText.TabIndex = 3
    '
    'Label48
    '
    Me.Label48.AutoSize = True
    Me.Label48.Location = New System.Drawing.Point(9, 25)
    Me.Label48.Name = "Label48"
    Me.Label48.Size = New System.Drawing.Size(36, 16)
    Me.Label48.TabIndex = 2
    Me.Label48.Text = "Index"
    '
    'Label47
    '
    Me.Label47.AutoSize = True
    Me.Label47.Location = New System.Drawing.Point(135, 25)
    Me.Label47.Name = "Label47"
    Me.Label47.Size = New System.Drawing.Size(31, 16)
    Me.Label47.TabIndex = 1
    Me.Label47.Text = "Type"
    '
    'ColTriangleBox
    '
    Me.ColTriangleBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.ColTriangleBox.FormattingEnabled = True
    Me.ColTriangleBox.Location = New System.Drawing.Point(11, 44)
    Me.ColTriangleBox.Name = "ColTriangleBox"
    Me.ColTriangleBox.Size = New System.Drawing.Size(100, 24)
    Me.ColTriangleBox.TabIndex = 0
    '
    'CollisionGroupBox
    '
    Me.CollisionGroupBox.Controls.Add(Me.Label36)
    Me.CollisionGroupBox.Controls.Add(Me.ColWalkSound)
    Me.CollisionGroupBox.Controls.Add(Me.Label38)
    Me.CollisionGroupBox.Controls.Add(Me.Label37)
    Me.CollisionGroupBox.Controls.Add(Me.Label35)
    Me.CollisionGroupBox.Controls.Add(Me.Label33)
    Me.CollisionGroupBox.Controls.Add(Me.Label32)
    Me.CollisionGroupBox.Controls.Add(Me.Label23)
    Me.CollisionGroupBox.Controls.Add(Me.Label13)
    Me.CollisionGroupBox.Controls.Add(Me.Label1)
    Me.CollisionGroupBox.Controls.Add(Me.ColVar4)
    Me.CollisionGroupBox.Controls.Add(Me.ApplyCollisionButton)
    Me.CollisionGroupBox.Controls.Add(Me.ColVar2)
    Me.CollisionGroupBox.Controls.Add(Me.ColVar3)
    Me.CollisionGroupBox.Controls.Add(Me.ColVar1)
    Me.CollisionGroupBox.Controls.Add(Me.CollisionPresetButton)
    Me.CollisionGroupBox.Controls.Add(Me.Label34)
    Me.CollisionGroupBox.Controls.Add(Me.ColTypeText)
    Me.CollisionGroupBox.Controls.Add(Me.ColTypeBox)
    Me.CollisionGroupBox.Controls.Add(Me.Label31)
    Me.CollisionGroupBox.Controls.Add(Me.ExitTextBox)
    Me.CollisionGroupBox.Controls.Add(Me.ExitCombobox)
    Me.CollisionGroupBox.Controls.Add(Me.Label10)
    Me.CollisionGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup
    Me.CollisionGroupBox.Location = New System.Drawing.Point(3, 124)
    Me.CollisionGroupBox.Name = "CollisionGroupBox"
    Me.CollisionGroupBox.Size = New System.Drawing.Size(218, 342)
    Me.CollisionGroupBox.TabIndex = 86
    Me.CollisionGroupBox.TabStop = False
    Me.CollisionGroupBox.Text = "Types"
    '
    'Label36
    '
    Me.Label36.AutoSize = True
    Me.Label36.Location = New System.Drawing.Point(115, 135)
    Me.Label36.Name = "Label36"
    Me.Label36.Size = New System.Drawing.Size(20, 16)
    Me.Label36.TabIndex = 107
    Me.Label36.Text = "0x"
    '
    'ColWalkSound
    '
    Me.ColWalkSound.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
    Me.ColWalkSound.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
    Me.ColWalkSound.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ColWalkSound.Location = New System.Drawing.Point(138, 128)
    Me.ColWalkSound.MaxLength = 1
    Me.ColWalkSound.Name = "ColWalkSound"
    Me.ColWalkSound.Size = New System.Drawing.Size(55, 20)
    Me.ColWalkSound.TabIndex = 105
    '
    'Label38
    '
    Me.Label38.AutoSize = True
    Me.Label38.Location = New System.Drawing.Point(121, 109)
    Me.Label38.Name = "Label38"
    Me.Label38.Size = New System.Drawing.Size(90, 16)
    Me.Label38.TabIndex = 106
    Me.Label38.Text = "Walked on sound"
    '
    'Label37
    '
    Me.Label37.AutoSize = True
    Me.Label37.Location = New System.Drawing.Point(9, 263)
    Me.Label37.Name = "Label37"
    Me.Label37.Size = New System.Drawing.Size(20, 16)
    Me.Label37.TabIndex = 104
    Me.Label37.Text = "0x"
    '
    'Label35
    '
    Me.Label35.AutoSize = True
    Me.Label35.Location = New System.Drawing.Point(9, 221)
    Me.Label35.Name = "Label35"
    Me.Label35.Size = New System.Drawing.Size(20, 16)
    Me.Label35.TabIndex = 103
    Me.Label35.Text = "0x"
    '
    'Label33
    '
    Me.Label33.AutoSize = True
    Me.Label33.Location = New System.Drawing.Point(9, 179)
    Me.Label33.Name = "Label33"
    Me.Label33.Size = New System.Drawing.Size(20, 16)
    Me.Label33.TabIndex = 102
    Me.Label33.Text = "0x"
    '
    'Label32
    '
    Me.Label32.AutoSize = True
    Me.Label32.Location = New System.Drawing.Point(9, 135)
    Me.Label32.Name = "Label32"
    Me.Label32.Size = New System.Drawing.Size(20, 16)
    Me.Label32.TabIndex = 101
    Me.Label32.Text = "0x"
    '
    'Label23
    '
    Me.Label23.AutoSize = True
    Me.Label23.Location = New System.Drawing.Point(27, 198)
    Me.Label23.Name = "Label23"
    Me.Label23.Size = New System.Drawing.Size(42, 16)
    Me.Label23.TabIndex = 100
    Me.Label23.Text = "Flags 3"
    '
    'Label13
    '
    Me.Label13.AutoSize = True
    Me.Label13.Location = New System.Drawing.Point(27, 240)
    Me.Label13.Name = "Label13"
    Me.Label13.Size = New System.Drawing.Size(42, 16)
    Me.Label13.TabIndex = 99
    Me.Label13.Text = "Flags 4"
    '
    'Label1
    '
    Me.Label1.AutoSize = True
    Me.Label1.Location = New System.Drawing.Point(26, 156)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(42, 16)
    Me.Label1.TabIndex = 98
    Me.Label1.Text = "Flags 2"
    '
    'ColVar4
    '
    Me.ColVar4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
    Me.ColVar4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
    Me.ColVar4.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ColVar4.Location = New System.Drawing.Point(30, 256)
    Me.ColVar4.MaxLength = 4
    Me.ColVar4.Name = "ColVar4"
    Me.ColVar4.Size = New System.Drawing.Size(55, 20)
    Me.ColVar4.TabIndex = 17
    '
    'ApplyCollisionButton
    '
    Me.ApplyCollisionButton.BackColor = System.Drawing.SystemColors.Control
    Me.ApplyCollisionButton.Location = New System.Drawing.Point(155, 313)
    Me.ApplyCollisionButton.Name = "ApplyCollisionButton"
    Me.ApplyCollisionButton.Size = New System.Drawing.Size(56, 23)
    Me.ApplyCollisionButton.TabIndex = 18
    Me.ApplyCollisionButton.Text = "Apply"
    Me.ApplyCollisionButton.UseVisualStyleBackColor = True
    '
    'ColVar2
    '
    Me.ColVar2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
    Me.ColVar2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
    Me.ColVar2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ColVar2.Location = New System.Drawing.Point(29, 172)
    Me.ColVar2.MaxLength = 4
    Me.ColVar2.Name = "ColVar2"
    Me.ColVar2.Size = New System.Drawing.Size(56, 20)
    Me.ColVar2.TabIndex = 16
    '
    'ColVar3
    '
    Me.ColVar3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
    Me.ColVar3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
    Me.ColVar3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ColVar3.Location = New System.Drawing.Point(30, 214)
    Me.ColVar3.MaxLength = 4
    Me.ColVar3.Name = "ColVar3"
    Me.ColVar3.Size = New System.Drawing.Size(55, 20)
    Me.ColVar3.TabIndex = 15
    '
    'ColVar1
    '
    Me.ColVar1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
    Me.ColVar1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
    Me.ColVar1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ColVar1.Location = New System.Drawing.Point(30, 128)
    Me.ColVar1.MaxLength = 4
    Me.ColVar1.Name = "ColVar1"
    Me.ColVar1.Size = New System.Drawing.Size(55, 20)
    Me.ColVar1.TabIndex = 14
    '
    'CollisionPresetButton
    '
    Me.CollisionPresetButton.BackColor = System.Drawing.SystemColors.Control
    Me.CollisionPresetButton.Location = New System.Drawing.Point(13, 297)
    Me.CollisionPresetButton.Name = "CollisionPresetButton"
    Me.CollisionPresetButton.Size = New System.Drawing.Size(101, 23)
    Me.CollisionPresetButton.TabIndex = 13
    Me.CollisionPresetButton.Text = "Preset Database"
    Me.CollisionPresetButton.UseVisualStyleBackColor = False
    '
    'Label34
    '
    Me.Label34.AutoSize = True
    Me.Label34.Location = New System.Drawing.Point(27, 112)
    Me.Label34.Name = "Label34"
    Me.Label34.Size = New System.Drawing.Size(42, 16)
    Me.Label34.TabIndex = 97
    Me.Label34.Text = "Flags 1"
    '
    'ColTypeText
    '
    Me.ColTypeText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ColTypeText.Location = New System.Drawing.Point(138, 78)
    Me.ColTypeText.MaxLength = 4
    Me.ColTypeText.Name = "ColTypeText"
    Me.ColTypeText.ReadOnly = True
    Me.ColTypeText.Size = New System.Drawing.Size(55, 20)
    Me.ColTypeText.TabIndex = 91
    '
    'ColTypeBox
    '
    Me.ColTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.ColTypeBox.FormattingEnabled = True
    Me.ColTypeBox.Location = New System.Drawing.Point(11, 78)
    Me.ColTypeBox.Name = "ColTypeBox"
    Me.ColTypeBox.Size = New System.Drawing.Size(102, 24)
    Me.ColTypeBox.TabIndex = 12
    '
    'Label31
    '
    Me.Label31.AutoSize = True
    Me.Label31.Location = New System.Drawing.Point(8, 62)
    Me.Label31.Name = "Label31"
    Me.Label31.Size = New System.Drawing.Size(53, 16)
    Me.Label31.TabIndex = 87
    Me.Label31.Text = "Variables"
    '
    'ExitTextBox
    '
    Me.ExitTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ExitTextBox.Location = New System.Drawing.Point(138, 38)
    Me.ExitTextBox.MaxLength = 4
    Me.ExitTextBox.Name = "ExitTextBox"
    Me.ExitTextBox.Size = New System.Drawing.Size(55, 20)
    Me.ExitTextBox.TabIndex = 11
    '
    'ExitCombobox
    '
    Me.ExitCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.ExitCombobox.FormattingEnabled = True
    Me.ExitCombobox.Location = New System.Drawing.Point(11, 38)
    Me.ExitCombobox.Name = "ExitCombobox"
    Me.ExitCombobox.Size = New System.Drawing.Size(102, 24)
    Me.ExitCombobox.TabIndex = 10
    '
    'Label10
    '
    Me.Label10.AutoSize = True
    Me.Label10.Location = New System.Drawing.Point(8, 20)
    Me.Label10.Name = "Label10"
    Me.Label10.Size = New System.Drawing.Size(33, 16)
    Me.Label10.TabIndex = 83
    Me.Label10.Text = "Exits"
    '
    'MiscTab
    '
    Me.MiscTab.Controls.Add(Me.GroupBox9)
    Me.MiscTab.Controls.Add(Me.Label45)
    Me.MiscTab.Controls.Add(Me.AnimStart)
    Me.MiscTab.Controls.Add(Me.Label30)
    Me.MiscTab.Controls.Add(Me.LimbStart)
    Me.MiscTab.Controls.Add(Me.GroupBox10)
    Me.MiscTab.Controls.Add(Me.GroupBox8)
    Me.MiscTab.Location = New System.Drawing.Point(4, 46)
    Me.MiscTab.Name = "MiscTab"
    Me.MiscTab.Size = New System.Drawing.Size(224, 537)
    Me.MiscTab.TabIndex = 3
    Me.MiscTab.Text = "Miscellaneous"
    Me.MiscTab.UseVisualStyleBackColor = True
    '
    'GroupBox9
    '
    Me.GroupBox9.Controls.Add(Me.ProgressBar1)
    Me.GroupBox9.Controls.Add(Me.Label28)
    Me.GroupBox9.Controls.Add(Me.Button16)
    Me.GroupBox9.Controls.Add(Me.Button15)
    Me.GroupBox9.Location = New System.Drawing.Point(28, 120)
    Me.GroupBox9.Name = "GroupBox9"
    Me.GroupBox9.Size = New System.Drawing.Size(173, 100)
    Me.GroupBox9.TabIndex = 97
    Me.GroupBox9.TabStop = False
    Me.GroupBox9.Text = "Collision Matcher"
    '
    'ProgressBar1
    '
    Me.ProgressBar1.Location = New System.Drawing.Point(12, 77)
    Me.ProgressBar1.Name = "ProgressBar1"
    Me.ProgressBar1.Size = New System.Drawing.Size(150, 10)
    Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
    Me.ProgressBar1.TabIndex = 92
    '
    'Label28
    '
    Me.Label28.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.Label28.AutoSize = True
    Me.Label28.BackColor = System.Drawing.SystemColors.Control
    Me.Label28.Location = New System.Drawing.Point(119, 0)
    Me.Label28.Name = "Label28"
    Me.Label28.Size = New System.Drawing.Size(49, 16)
    Me.Label28.TabIndex = 89
    Me.Label28.Text = "Working"
    Me.Label28.Visible = False
    '
    'Button16
    '
    Me.Button16.Location = New System.Drawing.Point(12, 23)
    Me.Button16.Name = "Button16"
    Me.Button16.Size = New System.Drawing.Size(150, 23)
    Me.Button16.TabIndex = 91
    Me.Button16.Text = "Match collision to graphics"
    Me.Button16.UseVisualStyleBackColor = True
    '
    'Button15
    '
    Me.Button15.Location = New System.Drawing.Point(12, 46)
    Me.Button15.Name = "Button15"
    Me.Button15.Size = New System.Drawing.Size(150, 23)
    Me.Button15.TabIndex = 90
    Me.Button15.Text = "Match graphics to collision"
    Me.Button15.UseVisualStyleBackColor = True
    '
    'Label45
    '
    Me.Label45.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
    Me.Label45.AutoSize = True
    Me.Label45.Location = New System.Drawing.Point(12, 421)
    Me.Label45.Name = "Label45"
    Me.Label45.Size = New System.Drawing.Size(71, 16)
    Me.Label45.TabIndex = 94
    Me.Label45.Text = "Animation at"
    Me.Label45.Visible = False
    '
    'AnimStart
    '
    Me.AnimStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
    Me.AnimStart.Location = New System.Drawing.Point(14, 438)
    Me.AnimStart.Name = "AnimStart"
    Me.AnimStart.Size = New System.Drawing.Size(100, 20)
    Me.AnimStart.TabIndex = 93
    Me.AnimStart.Visible = False
    '
    'Label30
    '
    Me.Label30.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
    Me.Label30.AutoSize = True
    Me.Label30.Location = New System.Drawing.Point(12, 379)
    Me.Label30.Name = "Label30"
    Me.Label30.Size = New System.Drawing.Size(72, 16)
    Me.Label30.TabIndex = 92
    Me.Label30.Text = "Heirarchy at"
    Me.Label30.Visible = False
    '
    'LimbStart
    '
    Me.LimbStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
    Me.LimbStart.Location = New System.Drawing.Point(13, 398)
    Me.LimbStart.Name = "LimbStart"
    Me.LimbStart.Size = New System.Drawing.Size(100, 20)
    Me.LimbStart.TabIndex = 91
    Me.LimbStart.Visible = False
    '
    'GroupBox10
    '
    Me.GroupBox10.Controls.Add(Me.CheckBox5)
    Me.GroupBox10.Controls.Add(Me.CheckBox15)
    Me.GroupBox10.Controls.Add(Me.CheckBox14)
    Me.GroupBox10.Controls.Add(Me.CheckBox13)
    Me.GroupBox10.Location = New System.Drawing.Point(28, 9)
    Me.GroupBox10.Name = "GroupBox10"
    Me.GroupBox10.Size = New System.Drawing.Size(173, 105)
    Me.GroupBox10.TabIndex = 90
    Me.GroupBox10.TabStop = False
    Me.GroupBox10.Text = "Hide"
    '
    'CheckBox5
    '
    Me.CheckBox5.AutoSize = True
    Me.CheckBox5.Location = New System.Drawing.Point(12, 59)
    Me.CheckBox5.Name = "CheckBox5"
    Me.CheckBox5.Size = New System.Drawing.Size(111, 20)
    Me.CheckBox5.TabIndex = 3
    Me.CheckBox5.Text = "Link actor cubes"
    Me.CheckBox5.UseVisualStyleBackColor = True
    '
    'CheckBox15
    '
    Me.CheckBox15.AutoSize = True
    Me.CheckBox15.Location = New System.Drawing.Point(12, 77)
    Me.CheckBox15.Name = "CheckBox15"
    Me.CheckBox15.Size = New System.Drawing.Size(82, 20)
    Me.CheckBox15.TabIndex = 2
    Me.CheckBox15.Text = "Axis guides"
    Me.CheckBox15.UseVisualStyleBackColor = True
    '
    'CheckBox14
    '
    Me.CheckBox14.AutoSize = True
    Me.CheckBox14.Location = New System.Drawing.Point(12, 40)
    Me.CheckBox14.Name = "CheckBox14"
    Me.CheckBox14.Size = New System.Drawing.Size(119, 20)
    Me.CheckBox14.TabIndex = 1
    Me.CheckBox14.Text = "Scene actor cubes"
    Me.CheckBox14.UseVisualStyleBackColor = True
    '
    'CheckBox13
    '
    Me.CheckBox13.AutoSize = True
    Me.CheckBox13.Location = New System.Drawing.Point(12, 22)
    Me.CheckBox13.Name = "CheckBox13"
    Me.CheckBox13.Size = New System.Drawing.Size(116, 20)
    Me.CheckBox13.TabIndex = 0
    Me.CheckBox13.Text = "Room actor cubes"
    Me.CheckBox13.UseVisualStyleBackColor = True
    '
    'GroupBox8
    '
    Me.GroupBox8.Controls.Add(Me.MapsCombobox)
    Me.GroupBox8.Controls.Add(Me.Label46)
    Me.GroupBox8.Location = New System.Drawing.Point(28, 227)
    Me.GroupBox8.Name = "GroupBox8"
    Me.GroupBox8.Size = New System.Drawing.Size(173, 85)
    Me.GroupBox8.TabIndex = 98
    Me.GroupBox8.TabStop = False
    Me.GroupBox8.Text = "Individual level"
    '
    'MapsCombobox
    '
    Me.MapsCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.MapsCombobox.FormattingEnabled = True
    Me.MapsCombobox.Location = New System.Drawing.Point(12, 43)
    Me.MapsCombobox.Name = "MapsCombobox"
    Me.MapsCombobox.Size = New System.Drawing.Size(150, 24)
    Me.MapsCombobox.TabIndex = 95
    '
    'Label46
    '
    Me.Label46.AutoSize = True
    Me.Label46.Location = New System.Drawing.Point(9, 24)
    Me.Label46.Name = "Label46"
    Me.Label46.Size = New System.Drawing.Size(93, 16)
    Me.Label46.TabIndex = 96
    Me.Label46.Text = "Referenced maps"
    '
    'LevelFlagsTab
    '
    Me.LevelFlagsTab.BackColor = System.Drawing.Color.Transparent
    Me.LevelFlagsTab.Controls.Add(Me.GroupBox6)
    Me.LevelFlagsTab.Location = New System.Drawing.Point(4, 25)
    Me.LevelFlagsTab.Name = "LevelFlagsTab"
    Me.LevelFlagsTab.Size = New System.Drawing.Size(224, 558)
    Me.LevelFlagsTab.TabIndex = 2
    Me.LevelFlagsTab.Text = "Level Flags"
    Me.LevelFlagsTab.UseVisualStyleBackColor = True
    '
    'GroupBox6
    '
    Me.GroupBox6.Controls.Add(Me.ComboBox6)
    Me.GroupBox6.Controls.Add(Me.Label2)
    Me.GroupBox6.Controls.Add(Me.Button11)
    Me.GroupBox6.Controls.Add(Me.TextBox13)
    Me.GroupBox6.Controls.Add(Me.Label21)
    Me.GroupBox6.Location = New System.Drawing.Point(3, 9)
    Me.GroupBox6.Name = "GroupBox6"
    Me.GroupBox6.Size = New System.Drawing.Size(218, 74)
    Me.GroupBox6.TabIndex = 77
    Me.GroupBox6.TabStop = False
    Me.GroupBox6.Text = "Ambience"
    '
    'ComboBox6
    '
    Me.ComboBox6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.ComboBox6.FormattingEnabled = True
    Me.ComboBox6.Items.AddRange(New Object() {"No Change", "Morning", "Mid Day", "Night"})
    Me.ComboBox6.Location = New System.Drawing.Point(10, 36)
    Me.ComboBox6.Name = "ComboBox6"
    Me.ComboBox6.Size = New System.Drawing.Size(82, 24)
    Me.ComboBox6.TabIndex = 7
    '
    'Label2
    '
    Me.Label2.AutoSize = True
    Me.Label2.Location = New System.Drawing.Point(7, 20)
    Me.Label2.Name = "Label2"
    Me.Label2.Size = New System.Drawing.Size(69, 16)
    Me.Label2.TabIndex = 65
    Me.Label2.Text = "Time of day:"
    '
    'Button11
    '
    Me.Button11.BackColor = System.Drawing.SystemColors.Control
    Me.Button11.FlatStyle = System.Windows.Forms.FlatStyle.System
    Me.Button11.Location = New System.Drawing.Point(137, 37)
    Me.Button11.Name = "Button11"
    Me.Button11.Size = New System.Drawing.Size(23, 23)
    Me.Button11.TabIndex = 9
    Me.Button11.Text = "?"
    Me.Button11.UseVisualStyleBackColor = True
    '
    'TextBox13
    '
    Me.TextBox13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.TextBox13.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.TextBox13.Location = New System.Drawing.Point(96, 38)
    Me.TextBox13.MaxLength = 2
    Me.TextBox13.Name = "TextBox13"
    Me.TextBox13.Size = New System.Drawing.Size(35, 20)
    Me.TextBox13.TabIndex = 8
    '
    'Label21
    '
    Me.Label21.AutoSize = True
    Me.Label21.Location = New System.Drawing.Point(93, 21)
    Me.Label21.Name = "Label21"
    Me.Label21.Size = New System.Drawing.Size(40, 16)
    Me.Label21.TabIndex = 74
    Me.Label21.Text = "Music:"
    '
    'ActorsTab
    '
    Me.ActorsTab.BackColor = System.Drawing.Color.Transparent
    Me.ActorsTab.Controls.Add(Me.Button2)
    Me.ActorsTab.Controls.Add(Me.GroupBox4)
    Me.ActorsTab.Controls.Add(Me.GroupBox5)
    Me.ActorsTab.Location = New System.Drawing.Point(4, 46)
    Me.ActorsTab.Name = "ActorsTab"
    Me.ActorsTab.Padding = New System.Windows.Forms.Padding(3)
    Me.ActorsTab.Size = New System.Drawing.Size(224, 537)
    Me.ActorsTab.TabIndex = 1
    Me.ActorsTab.Text = "Actors"
    Me.ActorsTab.UseVisualStyleBackColor = True
    '
    'Button2
    '
    Me.Button2.BackColor = System.Drawing.SystemColors.Control
    Me.Button2.Location = New System.Drawing.Point(129, 378)
    Me.Button2.Name = "Button2"
    Me.Button2.Size = New System.Drawing.Size(85, 23)
    Me.Button2.TabIndex = 14
    Me.Button2.Text = "Apply"
    Me.Button2.UseVisualStyleBackColor = True
    '
    'GroupBox4
    '
    Me.GroupBox4.Controls.Add(Me.Label14)
    Me.GroupBox4.Controls.Add(Me.Label17)
    Me.GroupBox4.Controls.Add(Me.TextBox9)
    Me.GroupBox4.Controls.Add(Me.Label18)
    Me.GroupBox4.Controls.Add(Me.TextBox8)
    Me.GroupBox4.Controls.Add(Me.Label19)
    Me.GroupBox4.Controls.Add(Me.TextBox7)
    Me.GroupBox4.Controls.Add(Me.TextBox10)
    Me.GroupBox4.Controls.Add(Me.Label16)
    Me.GroupBox4.Controls.Add(Me.TextBox11)
    Me.GroupBox4.Controls.Add(Me.Label15)
    Me.GroupBox4.Controls.Add(Me.TextBox12)
    Me.GroupBox4.Location = New System.Drawing.Point(3, 230)
    Me.GroupBox4.Name = "GroupBox4"
    Me.GroupBox4.Size = New System.Drawing.Size(218, 142)
    Me.GroupBox4.TabIndex = 72
    Me.GroupBox4.TabStop = False
    Me.GroupBox4.Text = "Position"
    '
    'Label14
    '
    Me.Label14.AutoSize = True
    Me.Label14.Location = New System.Drawing.Point(20, 19)
    Me.Label14.Name = "Label14"
    Me.Label14.Size = New System.Drawing.Size(69, 16)
    Me.Label14.TabIndex = 59
    Me.Label14.Text = "Actor X Pos."
    '
    'Label17
    '
    Me.Label17.AutoSize = True
    Me.Label17.Location = New System.Drawing.Point(112, 19)
    Me.Label17.Name = "Label17"
    Me.Label17.Size = New System.Drawing.Size(70, 16)
    Me.Label17.TabIndex = 66
    Me.Label17.Text = "Actor X Rot."
    '
    'TextBox9
    '
    Me.TextBox9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.TextBox9.Location = New System.Drawing.Point(23, 113)
    Me.TextBox9.Name = "TextBox9"
    Me.TextBox9.Size = New System.Drawing.Size(84, 20)
    Me.TextBox9.TabIndex = 10
    '
    'Label18
    '
    Me.Label18.AutoSize = True
    Me.Label18.Location = New System.Drawing.Point(112, 58)
    Me.Label18.Name = "Label18"
    Me.Label18.Size = New System.Drawing.Size(70, 16)
    Me.Label18.TabIndex = 67
    Me.Label18.Text = "Actor Y Rot."
    '
    'TextBox8
    '
    Me.TextBox8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.TextBox8.Location = New System.Drawing.Point(23, 74)
    Me.TextBox8.Name = "TextBox8"
    Me.TextBox8.Size = New System.Drawing.Size(84, 20)
    Me.TextBox8.TabIndex = 9
    '
    'Label19
    '
    Me.Label19.AutoSize = True
    Me.Label19.Location = New System.Drawing.Point(112, 97)
    Me.Label19.Name = "Label19"
    Me.Label19.Size = New System.Drawing.Size(69, 16)
    Me.Label19.TabIndex = 68
    Me.Label19.Text = "Actor Z Rot."
    '
    'TextBox7
    '
    Me.TextBox7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.TextBox7.Location = New System.Drawing.Point(23, 35)
    Me.TextBox7.Name = "TextBox7"
    Me.TextBox7.Size = New System.Drawing.Size(84, 20)
    Me.TextBox7.TabIndex = 8
    '
    'TextBox10
    '
    Me.TextBox10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.TextBox10.Location = New System.Drawing.Point(115, 35)
    Me.TextBox10.Name = "TextBox10"
    Me.TextBox10.Size = New System.Drawing.Size(83, 20)
    Me.TextBox10.TabIndex = 11
    '
    'Label16
    '
    Me.Label16.AutoSize = True
    Me.Label16.Location = New System.Drawing.Point(20, 97)
    Me.Label16.Name = "Label16"
    Me.Label16.Size = New System.Drawing.Size(68, 16)
    Me.Label16.TabIndex = 61
    Me.Label16.Text = "Actor Z Pos."
    '
    'TextBox11
    '
    Me.TextBox11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.TextBox11.Location = New System.Drawing.Point(115, 74)
    Me.TextBox11.Name = "TextBox11"
    Me.TextBox11.Size = New System.Drawing.Size(83, 20)
    Me.TextBox11.TabIndex = 12
    '
    'Label15
    '
    Me.Label15.AutoSize = True
    Me.Label15.Location = New System.Drawing.Point(20, 58)
    Me.Label15.Name = "Label15"
    Me.Label15.Size = New System.Drawing.Size(69, 16)
    Me.Label15.TabIndex = 60
    Me.Label15.Text = "Actor Y Pos."
    '
    'TextBox12
    '
    Me.TextBox12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.TextBox12.Location = New System.Drawing.Point(115, 113)
    Me.TextBox12.Name = "TextBox12"
    Me.TextBox12.Size = New System.Drawing.Size(83, 20)
    Me.TextBox12.TabIndex = 13
    '
    'GroupBox5
    '
    Me.GroupBox5.Controls.Add(Me.Button6)
    Me.GroupBox5.Controls.Add(Me.SceneActorCombobox)
    Me.GroupBox5.Controls.Add(Me.ActorNumberText)
    Me.GroupBox5.Controls.Add(Me.Label7)
    Me.GroupBox5.Controls.Add(Me.ActorVarText)
    Me.GroupBox5.Controls.Add(Me.Label8)
    Me.GroupBox5.Controls.Add(Me.Label6)
    Me.GroupBox5.Controls.Add(Me.ActorGroupText)
    Me.GroupBox5.Controls.Add(Me.Label22)
    Me.GroupBox5.Controls.Add(Me.RoomActorCombobox)
    Me.GroupBox5.Controls.Add(Me.Label24)
    Me.GroupBox5.Location = New System.Drawing.Point(3, 9)
    Me.GroupBox5.Name = "GroupBox5"
    Me.GroupBox5.Size = New System.Drawing.Size(218, 215)
    Me.GroupBox5.TabIndex = 73
    Me.GroupBox5.TabStop = False
    Me.GroupBox5.Text = "Actors"
    '
    'Button6
    '
    Me.Button6.BackColor = System.Drawing.SystemColors.Control
    Me.Button6.Location = New System.Drawing.Point(123, 178)
    Me.Button6.Name = "Button6"
    Me.Button6.Size = New System.Drawing.Size(75, 23)
    Me.Button6.TabIndex = 54
    Me.Button6.Text = "Database"
    Me.Button6.UseVisualStyleBackColor = False
    '
    'SceneActorCombobox
    '
    Me.SceneActorCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.SceneActorCombobox.FormattingEnabled = True
    Me.SceneActorCombobox.Location = New System.Drawing.Point(19, 79)
    Me.SceneActorCombobox.Name = "SceneActorCombobox"
    Me.SceneActorCombobox.Size = New System.Drawing.Size(180, 24)
    Me.SceneActorCombobox.TabIndex = 1
    '
    'ActorNumberText
    '
    Me.ActorNumberText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.ActorNumberText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ActorNumberText.Location = New System.Drawing.Point(20, 128)
    Me.ActorNumberText.MaxLength = 4
    Me.ActorNumberText.Name = "ActorNumberText"
    Me.ActorNumberText.Size = New System.Drawing.Size(83, 20)
    Me.ActorNumberText.TabIndex = 2
    '
    'Label7
    '
    Me.Label7.AutoSize = True
    Me.Label7.Location = New System.Drawing.Point(17, 160)
    Me.Label7.Name = "Label7"
    Me.Label7.Size = New System.Drawing.Size(86, 16)
    Me.Label7.TabIndex = 39
    Me.Label7.Text = "Models (groups)"
    '
    'ActorVarText
    '
    Me.ActorVarText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.ActorVarText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ActorVarText.Location = New System.Drawing.Point(113, 128)
    Me.ActorVarText.MaxLength = 4
    Me.ActorVarText.Name = "ActorVarText"
    Me.ActorVarText.Size = New System.Drawing.Size(85, 20)
    Me.ActorVarText.TabIndex = 3
    '
    'Label8
    '
    Me.Label8.AutoSize = True
    Me.Label8.Location = New System.Drawing.Point(17, 110)
    Me.Label8.Name = "Label8"
    Me.Label8.Size = New System.Drawing.Size(46, 16)
    Me.Label8.TabIndex = 33
    Me.Label8.Text = "Number"
    '
    'Label6
    '
    Me.Label6.AutoSize = True
    Me.Label6.Location = New System.Drawing.Point(111, 111)
    Me.Label6.Name = "Label6"
    Me.Label6.Size = New System.Drawing.Size(48, 16)
    Me.Label6.TabIndex = 34
    Me.Label6.Text = "Variable"
    '
    'ActorGroupText
    '
    Me.ActorGroupText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    Me.ActorGroupText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.ActorGroupText.Location = New System.Drawing.Point(20, 179)
    Me.ActorGroupText.MaxLength = 4
    Me.ActorGroupText.Name = "ActorGroupText"
    Me.ActorGroupText.Size = New System.Drawing.Size(83, 20)
    Me.ActorGroupText.TabIndex = 5
    Me.ActorGroupText.Text = "0001"
    '
    'Label22
    '
    Me.Label22.AutoSize = True
    Me.Label22.Location = New System.Drawing.Point(16, 18)
    Me.Label22.Name = "Label22"
    Me.Label22.Size = New System.Drawing.Size(69, 16)
    Me.Label22.TabIndex = 52
    Me.Label22.Text = "Room Actors"
    '
    'RoomActorCombobox
    '
    Me.RoomActorCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.RoomActorCombobox.FormattingEnabled = True
    Me.RoomActorCombobox.Location = New System.Drawing.Point(19, 34)
    Me.RoomActorCombobox.Name = "RoomActorCombobox"
    Me.RoomActorCombobox.Size = New System.Drawing.Size(180, 24)
    Me.RoomActorCombobox.TabIndex = 0
    '
    'Label24
    '
    Me.Label24.AutoSize = True
    Me.Label24.Location = New System.Drawing.Point(17, 62)
    Me.Label24.Name = "Label24"
    Me.Label24.Size = New System.Drawing.Size(72, 16)
    Me.Label24.TabIndex = 53
    Me.Label24.Text = "Scene Actors"
    '
    'EditingTabs
    '
    Me.EditingTabs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.EditingTabs.Controls.Add(Me.ActorsTab)
    Me.EditingTabs.Controls.Add(Me.LevelFlagsTab)
    Me.EditingTabs.Controls.Add(Me.CollisionTab)
    Me.EditingTabs.Controls.Add(Me.MiscTab)
    Me.EditingTabs.Controls.Add(Me.AnimationsTab)
    Me.EditingTabs.Controls.Add(Me.DLTab)
    Me.EditingTabs.Font = New System.Drawing.Font("Trebuchet MS", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.EditingTabs.HotTrack = True
    Me.EditingTabs.Location = New System.Drawing.Point(930, 33)
    Me.EditingTabs.Multiline = True
    Me.EditingTabs.Name = "EditingTabs"
    Me.EditingTabs.SelectedIndex = 0
    Me.EditingTabs.Size = New System.Drawing.Size(232, 587)
    Me.EditingTabs.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight
    Me.EditingTabs.TabIndex = 3
    '
    'AnimationsTab
    '
    Me.AnimationsTab.Controls.Add(Me.animationTab_)
    Me.AnimationsTab.Location = New System.Drawing.Point(4, 46)
    Me.AnimationsTab.Name = "AnimationsTab"
    Me.AnimationsTab.Size = New System.Drawing.Size(224, 537)
    Me.AnimationsTab.TabIndex = 7
    Me.AnimationsTab.Text = "Animations"
    Me.AnimationsTab.UseVisualStyleBackColor = True
    '
    'animationTab_
    '
    Me.animationTab_.Dock = System.Windows.Forms.DockStyle.Fill
    Me.animationTab_.Location = New System.Drawing.Point(0, 0)
    Me.animationTab_.Name = "animationTab_"
    Me.animationTab_.Size = New System.Drawing.Size(224, 537)
    Me.animationTab_.TabIndex = 0
    '
    'DLTab
    '
    Me.DLTab.Controls.Add(Me.RadioButton2)
    Me.DLTab.Controls.Add(Me.GroupBox7)
    Me.DLTab.Controls.Add(Me.DListSelection)
    Me.DLTab.Controls.Add(Me.RadioButton1)
    Me.DLTab.Controls.Add(Me.GroupBox3)
    Me.DLTab.Location = New System.Drawing.Point(4, 46)
    Me.DLTab.Name = "DLTab"
    Me.DLTab.Size = New System.Drawing.Size(224, 537)
    Me.DLTab.TabIndex = 8
    Me.DLTab.Text = "Graphics"
    Me.DLTab.UseVisualStyleBackColor = True
    '
    'RadioButton2
    '
    Me.RadioButton2.AutoSize = True
    Me.RadioButton2.Location = New System.Drawing.Point(18, 85)
    Me.RadioButton2.Name = "RadioButton2"
    Me.RadioButton2.Size = New System.Drawing.Size(84, 20)
    Me.RadioButton2.TabIndex = 67
    Me.RadioButton2.TabStop = True
    Me.RadioButton2.Text = "Hide others"
    Me.RadioButton2.UseVisualStyleBackColor = True
    '
    'GroupBox7
    '
    Me.GroupBox7.Controls.Add(Me.WholeCommandTxt)
    Me.GroupBox7.Controls.Add(Me.Label3)
    Me.GroupBox7.Controls.Add(Me.Button8)
    Me.GroupBox7.Controls.Add(Me.HiwordText)
    Me.GroupBox7.Controls.Add(Me.Button1)
    Me.GroupBox7.Controls.Add(Me.LowordText)
    Me.GroupBox7.Controls.Add(Me.CommandCodeText)
    Me.GroupBox7.Controls.Add(Me.CommandJumpBox)
    Me.GroupBox7.Controls.Add(Me.Label26)
    Me.GroupBox7.Controls.Add(Me.Label25)
    Me.GroupBox7.Controls.Add(Me.Label9)
    Me.GroupBox7.Controls.Add(Me.Button4)
    Me.GroupBox7.Controls.Add(Me.CommandsListbox)
    Me.GroupBox7.Location = New System.Drawing.Point(10, 176)
    Me.GroupBox7.Name = "GroupBox7"
    Me.GroupBox7.Size = New System.Drawing.Size(206, 358)
    Me.GroupBox7.TabIndex = 69
    Me.GroupBox7.TabStop = False
    Me.GroupBox7.Text = "Commands"
    '
    'WholeCommandTxt
    '
    Me.WholeCommandTxt.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.WholeCommandTxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.WholeCommandTxt.Location = New System.Drawing.Point(7, 321)
    Me.WholeCommandTxt.MaxLength = 32
    Me.WholeCommandTxt.Name = "WholeCommandTxt"
    Me.WholeCommandTxt.Size = New System.Drawing.Size(107, 20)
    Me.WholeCommandTxt.TabIndex = 71
    '
    'Label3
    '
    Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.Label3.AutoSize = True
    Me.Label3.Location = New System.Drawing.Point(3, 238)
    Me.Label3.Name = "Label3"
    Me.Label3.Size = New System.Drawing.Size(47, 16)
    Me.Label3.TabIndex = 69
    Me.Label3.Text = "Jump to"
    '
    'Button8
    '
    Me.Button8.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.Button8.Location = New System.Drawing.Point(57, 234)
    Me.Button8.Name = "Button8"
    Me.Button8.Size = New System.Drawing.Size(67, 23)
    Me.Button8.TabIndex = 68
    Me.Button8.Text = "Previous"
    Me.Button8.UseVisualStyleBackColor = True
    '
    'HiwordText
    '
    Me.HiwordText.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.HiwordText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.HiwordText.Location = New System.Drawing.Point(109, 293)
    Me.HiwordText.MaxLength = 8
    Me.HiwordText.Name = "HiwordText"
    Me.HiwordText.Size = New System.Drawing.Size(90, 20)
    Me.HiwordText.TabIndex = 65
    '
    'Button1
    '
    Me.Button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.Button1.Location = New System.Drawing.Point(130, 234)
    Me.Button1.Name = "Button1"
    Me.Button1.Size = New System.Drawing.Size(67, 23)
    Me.Button1.TabIndex = 67
    Me.Button1.Text = "Next"
    Me.Button1.UseVisualStyleBackColor = True
    '
    'LowordText
    '
    Me.LowordText.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.LowordText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.LowordText.Location = New System.Drawing.Point(37, 293)
    Me.LowordText.MaxLength = 6
    Me.LowordText.Name = "LowordText"
    Me.LowordText.Size = New System.Drawing.Size(66, 20)
    Me.LowordText.TabIndex = 64
    '
    'CommandCodeText
    '
    Me.CommandCodeText.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.CommandCodeText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.CommandCodeText.Location = New System.Drawing.Point(7, 293)
    Me.CommandCodeText.MaxLength = 2
    Me.CommandCodeText.Name = "CommandCodeText"
    Me.CommandCodeText.Size = New System.Drawing.Size(24, 20)
    Me.CommandCodeText.TabIndex = 63
    '
    'CommandJumpBox
    '
    Me.CommandJumpBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.CommandJumpBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.CommandJumpBox.FormattingEnabled = True
    Me.CommandJumpBox.Location = New System.Drawing.Point(6, 204)
    Me.CommandJumpBox.Name = "CommandJumpBox"
    Me.CommandJumpBox.Size = New System.Drawing.Size(192, 24)
    Me.CommandJumpBox.TabIndex = 66
    '
    'Label26
    '
    Me.Label26.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.Label26.AutoSize = True
    Me.Label26.Location = New System.Drawing.Point(132, 274)
    Me.Label26.Name = "Label26"
    Me.Label26.Size = New System.Drawing.Size(44, 16)
    Me.Label26.TabIndex = 65
    Me.Label26.Text = "Param1"
    '
    'Label25
    '
    Me.Label25.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.Label25.AutoSize = True
    Me.Label25.Location = New System.Drawing.Point(48, 274)
    Me.Label25.Name = "Label25"
    Me.Label25.Size = New System.Drawing.Size(44, 16)
    Me.Label25.TabIndex = 64
    Me.Label25.Text = "Param0"
    '
    'Label9
    '
    Me.Label9.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.Label9.AutoSize = True
    Me.Label9.Location = New System.Drawing.Point(5, 274)
    Me.Label9.Name = "Label9"
    Me.Label9.Size = New System.Drawing.Size(29, 16)
    Me.Label9.TabIndex = 63
    Me.Label9.Text = "Cmd"
    '
    'Button4
    '
    Me.Button4.Anchor = System.Windows.Forms.AnchorStyles.Bottom
    Me.Button4.Location = New System.Drawing.Point(127, 319)
    Me.Button4.Name = "Button4"
    Me.Button4.Size = New System.Drawing.Size(70, 23)
    Me.Button4.TabIndex = 62
    Me.Button4.Text = "Set"
    Me.Button4.UseVisualStyleBackColor = True
    '
    'CommandsListbox
    '
    Me.CommandsListbox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
    Me.CommandsListbox.ContextMenuStrip = Me.DLEditorContextMenu
    Me.CommandsListbox.FormattingEnabled = True
    Me.CommandsListbox.ItemHeight = 16
    Me.CommandsListbox.Location = New System.Drawing.Point(6, 22)
    Me.CommandsListbox.Name = "CommandsListbox"
    Me.CommandsListbox.Size = New System.Drawing.Size(192, 340)
    Me.CommandsListbox.TabIndex = 61
    '
    'DLEditorContextMenu
    '
    Me.DLEditorContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Copy, Me.Paste, Me.Reset})
    Me.DLEditorContextMenu.Name = "DLEditorContextMenu"
    Me.DLEditorContextMenu.Size = New System.Drawing.Size(103, 70)
    '
    'Copy
    '
    Me.Copy.Name = "Copy"
    Me.Copy.Size = New System.Drawing.Size(102, 22)
    Me.Copy.Text = "Copy"
    '
    'Paste
    '
    Me.Paste.Name = "Paste"
    Me.Paste.Size = New System.Drawing.Size(102, 22)
    Me.Paste.Text = "Paste"
    Me.Paste.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'Reset
    '
    Me.Reset.Name = "Reset"
    Me.Reset.Size = New System.Drawing.Size(102, 22)
    Me.Reset.Text = "Reset"
    '
    'DListSelection
    '
    Me.DListSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.DListSelection.FormattingEnabled = True
    Me.DListSelection.Location = New System.Drawing.Point(18, 29)
    Me.DListSelection.Name = "DListSelection"
    Me.DListSelection.Size = New System.Drawing.Size(192, 24)
    Me.DListSelection.TabIndex = 58
    '
    'RadioButton1
    '
    Me.RadioButton1.AutoSize = True
    Me.RadioButton1.Checked = True
    Me.RadioButton1.Location = New System.Drawing.Point(18, 59)
    Me.RadioButton1.Name = "RadioButton1"
    Me.RadioButton1.Size = New System.Drawing.Size(132, 20)
    Me.RadioButton1.TabIndex = 66
    Me.RadioButton1.TabStop = True
    Me.RadioButton1.Text = "Highlight and draw all"
    Me.RadioButton1.UseVisualStyleBackColor = True
    '
    'GroupBox3
    '
    Me.GroupBox3.Controls.Add(Me.Button12)
    Me.GroupBox3.Controls.Add(Me.Label4)
    Me.GroupBox3.Controls.Add(Me.Button10)
    Me.GroupBox3.Location = New System.Drawing.Point(10, 4)
    Me.GroupBox3.Name = "GroupBox3"
    Me.GroupBox3.Size = New System.Drawing.Size(206, 166)
    Me.GroupBox3.TabIndex = 68
    Me.GroupBox3.TabStop = False
    Me.GroupBox3.Text = "Display Lists"
    '
    'Button12
    '
    Me.Button12.Location = New System.Drawing.Point(85, 130)
    Me.Button12.Name = "Button12"
    Me.Button12.Size = New System.Drawing.Size(66, 23)
    Me.Button12.TabIndex = 2
    Me.Button12.Text = "All "
    Me.Button12.UseVisualStyleBackColor = True
    '
    'Label4
    '
    Me.Label4.AutoSize = True
    Me.Label4.Location = New System.Drawing.Point(8, 111)
    Me.Label4.Name = "Label4"
    Me.Label4.Size = New System.Drawing.Size(83, 16)
    Me.Label4.TabIndex = 1
    Me.Label4.Text = "Dump raw data"
    '
    'Button10
    '
    Me.Button10.Location = New System.Drawing.Point(9, 130)
    Me.Button10.Name = "Button10"
    Me.Button10.Size = New System.Drawing.Size(68, 23)
    Me.Button10.TabIndex = 0
    Me.Button10.Text = "Selected"
    Me.Button10.UseVisualStyleBackColor = True
    '
    'BackupMenuStrip
    '
    Me.BackupMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RestorToolStripMenuItem})
    Me.BackupMenuStrip.Name = "BackupMenuStrip"
    Me.BackupMenuStrip.Size = New System.Drawing.Size(185, 26)
    '
    'RestorToolStripMenuItem
    '
    Me.RestorToolStripMenuItem.Name = "RestorToolStripMenuItem"
    Me.RestorToolStripMenuItem.Size = New System.Drawing.Size(184, 22)
    Me.RestorToolStripMenuItem.Text = "Restore from backup"
    '
    'Label43
    '
    Me.Label43.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.Label43.AutoSize = True
    Me.Label43.Font = New System.Drawing.Font("Trebuchet MS", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.Label43.Location = New System.Drawing.Point(763, 626)
    Me.Label43.Name = "Label43"
    Me.Label43.Size = New System.Drawing.Size(89, 18)
    Me.Label43.TabIndex = 102
    Me.Label43.Text = "Camera speed:"
    '
    'TrackBar1
    '
    Me.TrackBar1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.TrackBar1.AutoSize = False
    Me.TrackBar1.Cursor = System.Windows.Forms.Cursors.Hand
    Me.TrackBar1.Location = New System.Drawing.Point(850, 628)
    Me.TrackBar1.Maximum = 40
    Me.TrackBar1.Minimum = 1
    Me.TrackBar1.Name = "TrackBar1"
    Me.TrackBar1.Size = New System.Drawing.Size(90, 15)
    Me.TrackBar1.TabIndex = 103
    Me.TrackBar1.Value = 20
    '
    'ActorContextMenu
    '
    Me.ActorContextMenu.BackColor = System.Drawing.SystemColors.Control
    Me.ActorContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeselectToolStripMenuItem, Me.ToolStripSeparator14, Me.EditToolStripMenuItem2, Me.AlignToolItem, Me.ToolStripSeparator13, Me.CopyToolStripMenuItem, Me.PasteToolStripMenuItem, Me.ClearClipboardToolStripMenuItem})
    Me.ActorContextMenu.Name = "ContextMenuStrip4"
    Me.ActorContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
    Me.ActorContextMenu.Size = New System.Drawing.Size(156, 148)
    '
    'DeselectToolStripMenuItem
    '
    Me.DeselectToolStripMenuItem.Name = "DeselectToolStripMenuItem"
    Me.DeselectToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
    Me.DeselectToolStripMenuItem.Text = "Deselect"
    '
    'ToolStripSeparator14
    '
    Me.ToolStripSeparator14.Name = "ToolStripSeparator14"
    Me.ToolStripSeparator14.Size = New System.Drawing.Size(152, 6)
    '
    'EditToolStripMenuItem2
    '
    Me.EditToolStripMenuItem2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CamXRotationToolStripMenuItem, Me.CamYRotationToolStripMenuItem, Me.CamZRotationToolStripMenuItem})
    Me.EditToolStripMenuItem2.Name = "EditToolStripMenuItem2"
    Me.EditToolStripMenuItem2.Size = New System.Drawing.Size(155, 22)
    Me.EditToolStripMenuItem2.Text = "Rotate"
    '
    'CamXRotationToolStripMenuItem
    '
    Me.CamXRotationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DegreesToolStripMenuItem, Me.DegreesToolStripMenuItem1})
    Me.CamXRotationToolStripMenuItem.Name = "CamXRotationToolStripMenuItem"
    Me.CamXRotationToolStripMenuItem.Size = New System.Drawing.Size(81, 22)
    Me.CamXRotationToolStripMenuItem.Text = "X"
    '
    'DegreesToolStripMenuItem
    '
    Me.DegreesToolStripMenuItem.Name = "DegreesToolStripMenuItem"
    Me.DegreesToolStripMenuItem.Size = New System.Drawing.Size(141, 22)
    Me.DegreesToolStripMenuItem.Text = "+ 90 degrees"
    '
    'DegreesToolStripMenuItem1
    '
    Me.DegreesToolStripMenuItem1.Name = "DegreesToolStripMenuItem1"
    Me.DegreesToolStripMenuItem1.Size = New System.Drawing.Size(141, 22)
    Me.DegreesToolStripMenuItem1.Text = "- 90 degrees"
    '
    'CamYRotationToolStripMenuItem
    '
    Me.CamYRotationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DegreesToolStripMenuItem2, Me.DegreesToolStripMenuItem3})
    Me.CamYRotationToolStripMenuItem.Name = "CamYRotationToolStripMenuItem"
    Me.CamYRotationToolStripMenuItem.Size = New System.Drawing.Size(81, 22)
    Me.CamYRotationToolStripMenuItem.Text = "Y"
    '
    'DegreesToolStripMenuItem2
    '
    Me.DegreesToolStripMenuItem2.Name = "DegreesToolStripMenuItem2"
    Me.DegreesToolStripMenuItem2.Size = New System.Drawing.Size(144, 22)
    Me.DegreesToolStripMenuItem2.Text = " + 90 degrees"
    '
    'DegreesToolStripMenuItem3
    '
    Me.DegreesToolStripMenuItem3.Name = "DegreesToolStripMenuItem3"
    Me.DegreesToolStripMenuItem3.Size = New System.Drawing.Size(144, 22)
    Me.DegreesToolStripMenuItem3.Text = "- 90 degrees"
    '
    'CamZRotationToolStripMenuItem
    '
    Me.CamZRotationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DegreesToolStripMenuItem4, Me.DegreesToolStripMenuItem5})
    Me.CamZRotationToolStripMenuItem.Name = "CamZRotationToolStripMenuItem"
    Me.CamZRotationToolStripMenuItem.Size = New System.Drawing.Size(81, 22)
    Me.CamZRotationToolStripMenuItem.Text = "Z"
    '
    'DegreesToolStripMenuItem4
    '
    Me.DegreesToolStripMenuItem4.Name = "DegreesToolStripMenuItem4"
    Me.DegreesToolStripMenuItem4.Size = New System.Drawing.Size(141, 22)
    Me.DegreesToolStripMenuItem4.Text = "+ 90 degrees"
    '
    'DegreesToolStripMenuItem5
    '
    Me.DegreesToolStripMenuItem5.Name = "DegreesToolStripMenuItem5"
    Me.DegreesToolStripMenuItem5.Size = New System.Drawing.Size(141, 22)
    Me.DegreesToolStripMenuItem5.Text = "- 90 degrees"
    '
    'AlignToolItem
    '
    Me.AlignToolItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XToolStripMenuItem3, Me.YToolStripMenuItem3, Me.ZToolStripMenuItem2})
    Me.AlignToolItem.Name = "AlignToolItem"
    Me.AlignToolItem.Size = New System.Drawing.Size(155, 22)
    Me.AlignToolItem.Text = "Align"
    '
    'XToolStripMenuItem3
    '
    Me.XToolStripMenuItem3.Name = "XToolStripMenuItem3"
    Me.XToolStripMenuItem3.Size = New System.Drawing.Size(81, 22)
    Me.XToolStripMenuItem3.Text = "X"
    '
    'YToolStripMenuItem3
    '
    Me.YToolStripMenuItem3.Name = "YToolStripMenuItem3"
    Me.YToolStripMenuItem3.Size = New System.Drawing.Size(81, 22)
    Me.YToolStripMenuItem3.Text = "Y"
    '
    'ZToolStripMenuItem2
    '
    Me.ZToolStripMenuItem2.Name = "ZToolStripMenuItem2"
    Me.ZToolStripMenuItem2.Size = New System.Drawing.Size(81, 22)
    Me.ZToolStripMenuItem2.Text = "Z"
    '
    'ToolStripSeparator13
    '
    Me.ToolStripSeparator13.Name = "ToolStripSeparator13"
    Me.ToolStripSeparator13.Size = New System.Drawing.Size(152, 6)
    '
    'CopyToolStripMenuItem
    '
    Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
    Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
    Me.CopyToolStripMenuItem.Text = "Copy attributes"
    '
    'PasteToolStripMenuItem
    '
    Me.PasteToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PositionToolStripMenuItem, Me.RotationToolStripMenuItem, Me.NumberAndVariableToolStripMenuItem})
    Me.PasteToolStripMenuItem.Enabled = False
    Me.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem"
    Me.PasteToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
    Me.PasteToolStripMenuItem.Text = "Paste attributes"
    '
    'PositionToolStripMenuItem
    '
    Me.PositionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XToolStripMenuItem1, Me.YToolStripMenuItem1, Me.ZToolStripMenuItem, Me.AllToolStripMenuItem1})
    Me.PositionToolStripMenuItem.Name = "PositionToolStripMenuItem"
    Me.PositionToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
    Me.PositionToolStripMenuItem.Text = "Position"
    '
    'XToolStripMenuItem1
    '
    Me.XToolStripMenuItem1.Name = "XToolStripMenuItem1"
    Me.XToolStripMenuItem1.Size = New System.Drawing.Size(88, 22)
    Me.XToolStripMenuItem1.Text = "X"
    '
    'YToolStripMenuItem1
    '
    Me.YToolStripMenuItem1.Name = "YToolStripMenuItem1"
    Me.YToolStripMenuItem1.Size = New System.Drawing.Size(88, 22)
    Me.YToolStripMenuItem1.Text = "Y"
    '
    'ZToolStripMenuItem
    '
    Me.ZToolStripMenuItem.Name = "ZToolStripMenuItem"
    Me.ZToolStripMenuItem.Size = New System.Drawing.Size(88, 22)
    Me.ZToolStripMenuItem.Text = "Z"
    '
    'AllToolStripMenuItem1
    '
    Me.AllToolStripMenuItem1.Name = "AllToolStripMenuItem1"
    Me.AllToolStripMenuItem1.Size = New System.Drawing.Size(88, 22)
    Me.AllToolStripMenuItem1.Text = "All"
    '
    'RotationToolStripMenuItem
    '
    Me.RotationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XToolStripMenuItem2, Me.YToolStripMenuItem2, Me.ZToolStripMenuItem1, Me.AllToolStripMenuItem})
    Me.RotationToolStripMenuItem.Name = "RotationToolStripMenuItem"
    Me.RotationToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
    Me.RotationToolStripMenuItem.Text = "Rotation"
    '
    'XToolStripMenuItem2
    '
    Me.XToolStripMenuItem2.Name = "XToolStripMenuItem2"
    Me.XToolStripMenuItem2.Size = New System.Drawing.Size(88, 22)
    Me.XToolStripMenuItem2.Text = "X"
    '
    'YToolStripMenuItem2
    '
    Me.YToolStripMenuItem2.Name = "YToolStripMenuItem2"
    Me.YToolStripMenuItem2.Size = New System.Drawing.Size(88, 22)
    Me.YToolStripMenuItem2.Text = "Y"
    '
    'ZToolStripMenuItem1
    '
    Me.ZToolStripMenuItem1.Name = "ZToolStripMenuItem1"
    Me.ZToolStripMenuItem1.Size = New System.Drawing.Size(88, 22)
    Me.ZToolStripMenuItem1.Text = "Z"
    '
    'AllToolStripMenuItem
    '
    Me.AllToolStripMenuItem.Name = "AllToolStripMenuItem"
    Me.AllToolStripMenuItem.Size = New System.Drawing.Size(88, 22)
    Me.AllToolStripMenuItem.Text = "All"
    '
    'NumberAndVariableToolStripMenuItem
    '
    Me.NumberAndVariableToolStripMenuItem.Name = "NumberAndVariableToolStripMenuItem"
    Me.NumberAndVariableToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
    Me.NumberAndVariableToolStripMenuItem.Text = "Number and Variable"
    '
    'ClearClipboardToolStripMenuItem
    '
    Me.ClearClipboardToolStripMenuItem.Enabled = False
    Me.ClearClipboardToolStripMenuItem.Name = "ClearClipboardToolStripMenuItem"
    Me.ClearClipboardToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
    Me.ClearClipboardToolStripMenuItem.Text = "Clear clipboard"
    '
    'RotationTimer
    '
    Me.RotationTimer.Interval = 1
    '
    'LoadIndividual
    '
    Me.LoadIndividual.Filter = "Levels (*.scene)|*.zscene|ZOBJ Files (*.zobj)|*.zobj"
    '
    'FileToolStripMenuItem1
    '
    Me.FileToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem35, Me.ToolStripMenuItem2, Me.CustomLevel, Me.toolStripSeparator, Me.SaveToolStripMenuItem, Me.ToolStripMenuItem34, Me.toolStripSeparator12, Me.ExitToolStripMenuItem})
    Me.FileToolStripMenuItem1.Name = "FileToolStripMenuItem1"
    Me.FileToolStripMenuItem1.Size = New System.Drawing.Size(37, 27)
    Me.FileToolStripMenuItem1.Text = "&File"
    '
    'ToolStripMenuItem35
    '
    Me.ToolStripMenuItem35.Image = CType(resources.GetObject("ToolStripMenuItem35.Image"), System.Drawing.Image)
    Me.ToolStripMenuItem35.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.ToolStripMenuItem35.Name = "ToolStripMenuItem35"
    Me.ToolStripMenuItem35.Size = New System.Drawing.Size(153, 22)
    Me.ToolStripMenuItem35.Text = "&Open ROM"
    '
    'ToolStripMenuItem2
    '
    Me.ToolStripMenuItem2.Image = CType(resources.GetObject("ToolStripMenuItem2.Image"), System.Drawing.Image)
    Me.ToolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
    Me.ToolStripMenuItem2.Size = New System.Drawing.Size(153, 22)
    Me.ToolStripMenuItem2.Text = "&Open Individual"
    '
    'CustomLevel
    '
    Me.CustomLevel.Name = "CustomLevel"
    Me.CustomLevel.Size = New System.Drawing.Size(153, 22)
    Me.CustomLevel.Text = "New level..."
    Me.CustomLevel.Visible = False
    '
    'toolStripSeparator
    '
    Me.toolStripSeparator.Name = "toolStripSeparator"
    Me.toolStripSeparator.Size = New System.Drawing.Size(150, 6)
    '
    'SaveToolStripMenuItem
    '
    Me.SaveToolStripMenuItem.Image = CType(resources.GetObject("SaveToolStripMenuItem.Image"), System.Drawing.Image)
    Me.SaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
    Me.SaveToolStripMenuItem.ShowShortcutKeys = False
    Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
    Me.SaveToolStripMenuItem.Text = "&Save ROM"
    '
    'ToolStripMenuItem34
    '
    Me.ToolStripMenuItem34.Name = "ToolStripMenuItem34"
    Me.ToolStripMenuItem34.Size = New System.Drawing.Size(153, 22)
    Me.ToolStripMenuItem34.Text = "Save ROM &As"
    Me.ToolStripMenuItem34.Visible = False
    '
    'toolStripSeparator12
    '
    Me.toolStripSeparator12.Name = "toolStripSeparator12"
    Me.toolStripSeparator12.Size = New System.Drawing.Size(150, 6)
    '
    'ExitToolStripMenuItem
    '
    Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
    Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
    Me.ExitToolStripMenuItem.Text = "E&xit"
    '
    'EditToolStripMenuItem1
    '
    Me.EditToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UndoToolStripMenuItem, Me.WireframeModeToolStripMenuItem, Me.RenderToolStripMenuItem})
    Me.EditToolStripMenuItem1.Name = "EditToolStripMenuItem1"
    Me.EditToolStripMenuItem1.Size = New System.Drawing.Size(44, 27)
    Me.EditToolStripMenuItem1.Text = "&View"
    '
    'UndoToolStripMenuItem
    '
    Me.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem"
    Me.UndoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1
    Me.UndoToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
    Me.UndoToolStripMenuItem.Text = "Editor tabs"
    '
    'WireframeModeToolStripMenuItem
    '
    Me.WireframeModeToolStripMenuItem.CheckOnClick = True
    Me.WireframeModeToolStripMenuItem.Name = "WireframeModeToolStripMenuItem"
    Me.WireframeModeToolStripMenuItem.ShortcutKeyDisplayString = "F4"
    Me.WireframeModeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4
    Me.WireframeModeToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
    Me.WireframeModeToolStripMenuItem.Text = "Wireframe"
    '
    'RenderToolStripMenuItem
    '
    Me.RenderToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GraphicsToolStripMenuItem, Me.CollisionOverlayToolStripMenuItem})
    Me.RenderToolStripMenuItem.Name = "RenderToolStripMenuItem"
    Me.RenderToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+R"
    Me.RenderToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
    Me.RenderToolStripMenuItem.Text = "Render"
    '
    'GraphicsToolStripMenuItem
    '
    Me.GraphicsToolStripMenuItem.Checked = True
    Me.GraphicsToolStripMenuItem.CheckOnClick = True
    Me.GraphicsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
    Me.GraphicsToolStripMenuItem.Name = "GraphicsToolStripMenuItem"
    Me.GraphicsToolStripMenuItem.Size = New System.Drawing.Size(156, 22)
    Me.GraphicsToolStripMenuItem.Text = "Graphics"
    '
    'CollisionOverlayToolStripMenuItem
    '
    Me.CollisionOverlayToolStripMenuItem.Checked = True
    Me.CollisionOverlayToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
    Me.CollisionOverlayToolStripMenuItem.Name = "CollisionOverlayToolStripMenuItem"
    Me.CollisionOverlayToolStripMenuItem.Size = New System.Drawing.Size(156, 22)
    Me.CollisionOverlayToolStripMenuItem.Text = "Collision overlay"
    '
    'ToolsToolStripMenuItem1
    '
    Me.ToolsToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MouseToolToolStripMenuItem1, Me.LockAxesToolStripMenuItem, Me.OptionsToolStripMenuItem2})
    Me.ToolsToolStripMenuItem1.Name = "ToolsToolStripMenuItem1"
    Me.ToolsToolStripMenuItem1.Size = New System.Drawing.Size(44, 27)
    Me.ToolsToolStripMenuItem1.Text = "&Tools"
    '
    'MouseToolToolStripMenuItem1
    '
    Me.MouseToolToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CameraOnlyMenu, Me.ActorSelectorMenu, Me.CollisionToolStripMenuItem, Me.CollisionSelectorMenu, Me.DisplayListSelectorToolStripMenuItem})
    Me.MouseToolToolStripMenuItem1.Name = "MouseToolToolStripMenuItem1"
    Me.MouseToolToolStripMenuItem1.ShortcutKeyDisplayString = ""
    Me.MouseToolToolStripMenuItem1.Size = New System.Drawing.Size(163, 22)
    Me.MouseToolToolStripMenuItem1.Text = "Mouse Tool"
    '
    'CameraOnlyMenu
    '
    Me.CameraOnlyMenu.Name = "CameraOnlyMenu"
    Me.CameraOnlyMenu.ShortcutKeyDisplayString = "1"
    Me.CameraOnlyMenu.Size = New System.Drawing.Size(191, 22)
    Me.CameraOnlyMenu.Text = "Camera only"
    '
    'ActorSelectorMenu
    '
    Me.ActorSelectorMenu.Name = "ActorSelectorMenu"
    Me.ActorSelectorMenu.ShortcutKeyDisplayString = "2"
    Me.ActorSelectorMenu.Size = New System.Drawing.Size(191, 22)
    Me.ActorSelectorMenu.Text = "Actor Selector"
    '
    'CollisionToolStripMenuItem
    '
    Me.CollisionToolStripMenuItem.Name = "CollisionToolStripMenuItem"
    Me.CollisionToolStripMenuItem.ShortcutKeyDisplayString = "3"
    Me.CollisionToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
    Me.CollisionToolStripMenuItem.Text = "Collision Tri Selector"
    '
    'CollisionSelectorMenu
    '
    Me.CollisionSelectorMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EdgeToolStripMenuItem, Me.TriangleToolStripMenuItem, Me.VertexToolStripMenuItem})
    Me.CollisionSelectorMenu.Name = "CollisionSelectorMenu"
    Me.CollisionSelectorMenu.ShortcutKeyDisplayString = ""
    Me.CollisionSelectorMenu.Size = New System.Drawing.Size(191, 22)
    Me.CollisionSelectorMenu.Text = "Geometry Editor"
    Me.CollisionSelectorMenu.Visible = False
    '
    'EdgeToolStripMenuItem
    '
    Me.EdgeToolStripMenuItem.Name = "EdgeToolStripMenuItem"
    Me.EdgeToolStripMenuItem.ShortcutKeyDisplayString = "4"
    Me.EdgeToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
    Me.EdgeToolStripMenuItem.Text = "Edge"
    '
    'TriangleToolStripMenuItem
    '
    Me.TriangleToolStripMenuItem.Name = "TriangleToolStripMenuItem"
    Me.TriangleToolStripMenuItem.ShortcutKeyDisplayString = "5"
    Me.TriangleToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
    Me.TriangleToolStripMenuItem.Text = "Triangle"
    '
    'VertexToolStripMenuItem
    '
    Me.VertexToolStripMenuItem.Name = "VertexToolStripMenuItem"
    Me.VertexToolStripMenuItem.ShortcutKeyDisplayString = "6"
    Me.VertexToolStripMenuItem.Size = New System.Drawing.Size(129, 22)
    Me.VertexToolStripMenuItem.Text = "Vertex"
    '
    'DisplayListSelectorToolStripMenuItem
    '
    Me.DisplayListSelectorToolStripMenuItem.Name = "DisplayListSelectorToolStripMenuItem"
    Me.DisplayListSelectorToolStripMenuItem.ShortcutKeyDisplayString = "7"
    Me.DisplayListSelectorToolStripMenuItem.Size = New System.Drawing.Size(191, 22)
    Me.DisplayListSelectorToolStripMenuItem.Text = "Display List selector"
    '
    'LockAxesToolStripMenuItem
    '
    Me.LockAxesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XToolStripMenuItem, Me.YToolStripMenuItem, Me.DisableToolStripMenuItem})
    Me.LockAxesToolStripMenuItem.Name = "LockAxesToolStripMenuItem"
    Me.LockAxesToolStripMenuItem.ShortcutKeyDisplayString = "Space"
    Me.LockAxesToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
    Me.LockAxesToolStripMenuItem.Text = "Lock axes"
    '
    'XToolStripMenuItem
    '
    Me.XToolStripMenuItem.Name = "XToolStripMenuItem"
    Me.XToolStripMenuItem.ShortcutKeyDisplayString = ""
    Me.XToolStripMenuItem.Size = New System.Drawing.Size(111, 22)
    Me.XToolStripMenuItem.Text = "X"
    '
    'YToolStripMenuItem
    '
    Me.YToolStripMenuItem.Name = "YToolStripMenuItem"
    Me.YToolStripMenuItem.ShortcutKeyDisplayString = ""
    Me.YToolStripMenuItem.Size = New System.Drawing.Size(111, 22)
    Me.YToolStripMenuItem.Text = "Y"
    '
    'DisableToolStripMenuItem
    '
    Me.DisableToolStripMenuItem.Name = "DisableToolStripMenuItem"
    Me.DisableToolStripMenuItem.ShortcutKeyDisplayString = ""
    Me.DisableToolStripMenuItem.Size = New System.Drawing.Size(111, 22)
    Me.DisableToolStripMenuItem.Text = "Disable"
    '
    'OptionsToolStripMenuItem2
    '
    Me.OptionsToolStripMenuItem2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DisableDepthTestToolStripMenuItem})
    Me.OptionsToolStripMenuItem2.Name = "OptionsToolStripMenuItem2"
    Me.OptionsToolStripMenuItem2.Size = New System.Drawing.Size(163, 22)
    Me.OptionsToolStripMenuItem2.Text = "Options"
    '
    'DisableDepthTestToolStripMenuItem
    '
    Me.DisableDepthTestToolStripMenuItem.CheckOnClick = True
    Me.DisableDepthTestToolStripMenuItem.Name = "DisableDepthTestToolStripMenuItem"
    Me.DisableDepthTestToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
    Me.DisableDepthTestToolStripMenuItem.Text = "Select actors behind graphics"
    '
    'ToolsToolStripMenuItem
    '
    Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OptionsToolStripMenuItem1, Me.RendererToolStripMenuItem})
    Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
    Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(80, 27)
    Me.ToolsToolStripMenuItem.Text = "&Preferences"
    '
    'OptionsToolStripMenuItem1
    '
    Me.OptionsToolStripMenuItem1.Name = "OptionsToolStripMenuItem1"
    Me.OptionsToolStripMenuItem1.Size = New System.Drawing.Size(122, 22)
    Me.OptionsToolStripMenuItem1.Text = "&Setup"
    '
    'RendererToolStripMenuItem
    '
    Me.RendererToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TexturesToolStripMenuItem, Me.ColorCombinerToolStripMenuItem, Me.AnisotropicFilteringToolStripMenuItem, Me.FullSceneAntialiasingToolStripMenuItem})
    Me.RendererToolStripMenuItem.Name = "RendererToolStripMenuItem"
    Me.RendererToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
    Me.RendererToolStripMenuItem.Text = "Renderer"
    '
    'TexturesToolStripMenuItem
    '
    Me.TexturesToolStripMenuItem.Checked = True
    Me.TexturesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
    Me.TexturesToolStripMenuItem.Name = "TexturesToolStripMenuItem"
    Me.TexturesToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
    Me.TexturesToolStripMenuItem.Text = "Textures"
    '
    'ColorCombinerToolStripMenuItem
    '
    Me.ColorCombinerToolStripMenuItem.Checked = True
    Me.ColorCombinerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
    Me.ColorCombinerToolStripMenuItem.Name = "ColorCombinerToolStripMenuItem"
    Me.ColorCombinerToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
    Me.ColorCombinerToolStripMenuItem.Text = "Color Combiner"
    '
    'AnisotropicFilteringToolStripMenuItem
    '
    Me.AnisotropicFilteringToolStripMenuItem.Checked = True
    Me.AnisotropicFilteringToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
    Me.AnisotropicFilteringToolStripMenuItem.Name = "AnisotropicFilteringToolStripMenuItem"
    Me.AnisotropicFilteringToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
    Me.AnisotropicFilteringToolStripMenuItem.Text = "Anisotropic Filtering"
    '
    'FullSceneAntialiasingToolStripMenuItem
    '
    Me.FullSceneAntialiasingToolStripMenuItem.Checked = True
    Me.FullSceneAntialiasingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
    Me.FullSceneAntialiasingToolStripMenuItem.Name = "FullSceneAntialiasingToolStripMenuItem"
    Me.FullSceneAntialiasingToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
    Me.FullSceneAntialiasingToolStripMenuItem.Text = "Full Scene Antialiasing"
    '
    'UoTMainMenu
    '
    Me.UoTMainMenu.AllowMerge = False
    Me.UoTMainMenu.AutoSize = False
    Me.UoTMainMenu.BackColor = System.Drawing.SystemColors.ControlLight
    Me.UoTMainMenu.Font = New System.Drawing.Font("Trebuchet MS", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.UoTMainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem1, Me.EditToolStripMenuItem1, Me.ToolsToolStripMenuItem, Me.ToolsToolStripMenuItem1})
    Me.UoTMainMenu.Location = New System.Drawing.Point(0, 0)
    Me.UoTMainMenu.Name = "UoTMainMenu"
    Me.UoTMainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
    Me.UoTMainMenu.Size = New System.Drawing.Size(1160, 31)
    Me.UoTMainMenu.Stretch = False
    Me.UoTMainMenu.TabIndex = 40
    Me.UoTMainMenu.Text = "MenuStrip1"
    '
    'VertContextMenu
    '
    Me.VertContextMenu.BackColor = System.Drawing.SystemColors.Control
    Me.VertContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem5, Me.ToolStripSeparator15, Me.ToolStripMenuItem6, Me.ToolStripMenuItem16, Me.ToolStripSeparator16, Me.ToolStripMenuItem20, Me.ToolStripMenuItem21, Me.ToolStripMenuItem33})
    Me.VertContextMenu.Name = "ContextMenuStrip4"
    Me.VertContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
    Me.VertContextMenu.Size = New System.Drawing.Size(156, 148)
    '
    'ToolStripMenuItem5
    '
    Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
    Me.ToolStripMenuItem5.Size = New System.Drawing.Size(155, 22)
    Me.ToolStripMenuItem5.Text = "Deselect"
    '
    'ToolStripSeparator15
    '
    Me.ToolStripSeparator15.Name = "ToolStripSeparator15"
    Me.ToolStripSeparator15.Size = New System.Drawing.Size(152, 6)
    '
    'ToolStripMenuItem6
    '
    Me.ToolStripMenuItem6.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem7, Me.ToolStripMenuItem10, Me.ToolStripMenuItem13})
    Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
    Me.ToolStripMenuItem6.Size = New System.Drawing.Size(155, 22)
    Me.ToolStripMenuItem6.Text = "Rotate"
    '
    'ToolStripMenuItem7
    '
    Me.ToolStripMenuItem7.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem8, Me.ToolStripMenuItem9})
    Me.ToolStripMenuItem7.Name = "ToolStripMenuItem7"
    Me.ToolStripMenuItem7.Size = New System.Drawing.Size(81, 22)
    Me.ToolStripMenuItem7.Text = "X"
    '
    'ToolStripMenuItem8
    '
    Me.ToolStripMenuItem8.Name = "ToolStripMenuItem8"
    Me.ToolStripMenuItem8.Size = New System.Drawing.Size(141, 22)
    Me.ToolStripMenuItem8.Text = "+ 90 degrees"
    '
    'ToolStripMenuItem9
    '
    Me.ToolStripMenuItem9.Name = "ToolStripMenuItem9"
    Me.ToolStripMenuItem9.Size = New System.Drawing.Size(141, 22)
    Me.ToolStripMenuItem9.Text = "- 90 degrees"
    '
    'ToolStripMenuItem10
    '
    Me.ToolStripMenuItem10.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem11, Me.ToolStripMenuItem12})
    Me.ToolStripMenuItem10.Name = "ToolStripMenuItem10"
    Me.ToolStripMenuItem10.Size = New System.Drawing.Size(81, 22)
    Me.ToolStripMenuItem10.Text = "Y"
    '
    'ToolStripMenuItem11
    '
    Me.ToolStripMenuItem11.Name = "ToolStripMenuItem11"
    Me.ToolStripMenuItem11.Size = New System.Drawing.Size(144, 22)
    Me.ToolStripMenuItem11.Text = " + 90 degrees"
    '
    'ToolStripMenuItem12
    '
    Me.ToolStripMenuItem12.Name = "ToolStripMenuItem12"
    Me.ToolStripMenuItem12.Size = New System.Drawing.Size(144, 22)
    Me.ToolStripMenuItem12.Text = "- 90 degrees"
    '
    'ToolStripMenuItem13
    '
    Me.ToolStripMenuItem13.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem14, Me.ToolStripMenuItem15})
    Me.ToolStripMenuItem13.Name = "ToolStripMenuItem13"
    Me.ToolStripMenuItem13.Size = New System.Drawing.Size(81, 22)
    Me.ToolStripMenuItem13.Text = "Z"
    '
    'ToolStripMenuItem14
    '
    Me.ToolStripMenuItem14.Name = "ToolStripMenuItem14"
    Me.ToolStripMenuItem14.Size = New System.Drawing.Size(141, 22)
    Me.ToolStripMenuItem14.Text = "+ 90 degrees"
    '
    'ToolStripMenuItem15
    '
    Me.ToolStripMenuItem15.Name = "ToolStripMenuItem15"
    Me.ToolStripMenuItem15.Size = New System.Drawing.Size(141, 22)
    Me.ToolStripMenuItem15.Text = "- 90 degrees"
    '
    'ToolStripMenuItem16
    '
    Me.ToolStripMenuItem16.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem17, Me.ToolStripMenuItem18, Me.ToolStripMenuItem19})
    Me.ToolStripMenuItem16.Name = "ToolStripMenuItem16"
    Me.ToolStripMenuItem16.Size = New System.Drawing.Size(155, 22)
    Me.ToolStripMenuItem16.Text = "Align"
    '
    'ToolStripMenuItem17
    '
    Me.ToolStripMenuItem17.Name = "ToolStripMenuItem17"
    Me.ToolStripMenuItem17.Size = New System.Drawing.Size(81, 22)
    Me.ToolStripMenuItem17.Text = "X"
    '
    'ToolStripMenuItem18
    '
    Me.ToolStripMenuItem18.Name = "ToolStripMenuItem18"
    Me.ToolStripMenuItem18.Size = New System.Drawing.Size(81, 22)
    Me.ToolStripMenuItem18.Text = "Y"
    '
    'ToolStripMenuItem19
    '
    Me.ToolStripMenuItem19.Name = "ToolStripMenuItem19"
    Me.ToolStripMenuItem19.Size = New System.Drawing.Size(81, 22)
    Me.ToolStripMenuItem19.Text = "Z"
    '
    'ToolStripSeparator16
    '
    Me.ToolStripSeparator16.Name = "ToolStripSeparator16"
    Me.ToolStripSeparator16.Size = New System.Drawing.Size(152, 6)
    '
    'ToolStripMenuItem20
    '
    Me.ToolStripMenuItem20.Name = "ToolStripMenuItem20"
    Me.ToolStripMenuItem20.Size = New System.Drawing.Size(155, 22)
    Me.ToolStripMenuItem20.Text = "Copy attributes"
    '
    'ToolStripMenuItem21
    '
    Me.ToolStripMenuItem21.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem22, Me.ToolStripMenuItem27, Me.ToolStripMenuItem32})
    Me.ToolStripMenuItem21.Enabled = False
    Me.ToolStripMenuItem21.Name = "ToolStripMenuItem21"
    Me.ToolStripMenuItem21.Size = New System.Drawing.Size(155, 22)
    Me.ToolStripMenuItem21.Text = "Paste attributes"
    '
    'ToolStripMenuItem22
    '
    Me.ToolStripMenuItem22.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem23, Me.ToolStripMenuItem24, Me.ToolStripMenuItem25, Me.ToolStripMenuItem26})
    Me.ToolStripMenuItem22.Name = "ToolStripMenuItem22"
    Me.ToolStripMenuItem22.Size = New System.Drawing.Size(185, 22)
    Me.ToolStripMenuItem22.Text = "Position"
    '
    'ToolStripMenuItem23
    '
    Me.ToolStripMenuItem23.Name = "ToolStripMenuItem23"
    Me.ToolStripMenuItem23.Size = New System.Drawing.Size(88, 22)
    Me.ToolStripMenuItem23.Text = "X"
    '
    'ToolStripMenuItem24
    '
    Me.ToolStripMenuItem24.Name = "ToolStripMenuItem24"
    Me.ToolStripMenuItem24.Size = New System.Drawing.Size(88, 22)
    Me.ToolStripMenuItem24.Text = "Y"
    '
    'ToolStripMenuItem25
    '
    Me.ToolStripMenuItem25.Name = "ToolStripMenuItem25"
    Me.ToolStripMenuItem25.Size = New System.Drawing.Size(88, 22)
    Me.ToolStripMenuItem25.Text = "Z"
    '
    'ToolStripMenuItem26
    '
    Me.ToolStripMenuItem26.Name = "ToolStripMenuItem26"
    Me.ToolStripMenuItem26.Size = New System.Drawing.Size(88, 22)
    Me.ToolStripMenuItem26.Text = "All"
    '
    'ToolStripMenuItem27
    '
    Me.ToolStripMenuItem27.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem28, Me.ToolStripMenuItem29, Me.ToolStripMenuItem30, Me.ToolStripMenuItem31})
    Me.ToolStripMenuItem27.Name = "ToolStripMenuItem27"
    Me.ToolStripMenuItem27.Size = New System.Drawing.Size(185, 22)
    Me.ToolStripMenuItem27.Text = "Rotation"
    '
    'ToolStripMenuItem28
    '
    Me.ToolStripMenuItem28.Name = "ToolStripMenuItem28"
    Me.ToolStripMenuItem28.Size = New System.Drawing.Size(88, 22)
    Me.ToolStripMenuItem28.Text = "X"
    '
    'ToolStripMenuItem29
    '
    Me.ToolStripMenuItem29.Name = "ToolStripMenuItem29"
    Me.ToolStripMenuItem29.Size = New System.Drawing.Size(88, 22)
    Me.ToolStripMenuItem29.Text = "Y"
    '
    'ToolStripMenuItem30
    '
    Me.ToolStripMenuItem30.Name = "ToolStripMenuItem30"
    Me.ToolStripMenuItem30.Size = New System.Drawing.Size(88, 22)
    Me.ToolStripMenuItem30.Text = "Z"
    '
    'ToolStripMenuItem31
    '
    Me.ToolStripMenuItem31.Name = "ToolStripMenuItem31"
    Me.ToolStripMenuItem31.Size = New System.Drawing.Size(88, 22)
    Me.ToolStripMenuItem31.Text = "All"
    '
    'ToolStripMenuItem32
    '
    Me.ToolStripMenuItem32.Name = "ToolStripMenuItem32"
    Me.ToolStripMenuItem32.Size = New System.Drawing.Size(185, 22)
    Me.ToolStripMenuItem32.Text = "Number and Variable"
    '
    'ToolStripMenuItem33
    '
    Me.ToolStripMenuItem33.Enabled = False
    Me.ToolStripMenuItem33.Name = "ToolStripMenuItem33"
    Me.ToolStripMenuItem33.Size = New System.Drawing.Size(155, 22)
    Me.ToolStripMenuItem33.Text = "Clear clipboard"
    '
    'RipDL
    '
    Me.RipDL.Filter = "RAW F3DEX2 Display List (*.f3dex2)|*.f3dex2"
    '
    'SaveROMAs
    '
    Me.SaveROMAs.Filter = "N64 ROMs|*.z64;*.n64;*.v64;*.rom"
    '
    'VarContextMenu
    '
    Me.VarContextMenu.Name = "VarContextMenu"
    Me.VarContextMenu.Size = New System.Drawing.Size(61, 4)
    '
    'NumContextMenu
    '
    Me.NumContextMenu.Name = "NumContextMenu"
    Me.NumContextMenu.Size = New System.Drawing.Size(61, 4)
    '
    'GrpContextMenu
    '
    Me.GrpContextMenu.Name = "GrpContextMenu"
    Me.GrpContextMenu.Size = New System.Drawing.Size(61, 4)
    '
    'zFileTreeView_
    '
    Me.zFileTreeView_.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
    Me.zFileTreeView_.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
    Me.zFileTreeView_.Location = New System.Drawing.Point(5, 33)
    Me.zFileTreeView_.Name = "zFileTreeView_"
    Me.zFileTreeView_.Size = New System.Drawing.Size(218, 583)
    Me.zFileTreeView_.TabIndex = 104
    '
    'UoTRender
    '
    Me.UoTRender.AccumBits = CType(0, Byte)
    Me.UoTRender.AllowDrop = True
    Me.UoTRender.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.UoTRender.AutoCheckErrors = False
    Me.UoTRender.AutoFinish = True
    Me.UoTRender.AutoMakeCurrent = True
    Me.UoTRender.AutoSize = True
    Me.UoTRender.AutoSwapBuffers = False
    Me.UoTRender.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange
    Me.UoTRender.BackColor = System.Drawing.Color.Black
    Me.UoTRender.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
    Me.UoTRender.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
    Me.UoTRender.ColorBits = CType(32, Byte)
    Me.UoTRender.Cursor = System.Windows.Forms.Cursors.Default
    Me.UoTRender.DepthBits = CType(24, Byte)
    Me.UoTRender.Location = New System.Drawing.Point(229, 33)
    Me.UoTRender.Name = "UoTRender"
    Me.UoTRender.Size = New System.Drawing.Size(700, 586)
    Me.UoTRender.StencilBits = CType(0, Byte)
    Me.UoTRender.TabIndex = 0
    '
    'MainWin
    '
    Me.AllowDrop = True
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
    Me.ClientSize = New System.Drawing.Size(1160, 649)
    Me.Controls.Add(Me.zFileTreeView_)
    Me.Controls.Add(Me.Label12)
    Me.Controls.Add(Me.Label43)
    Me.Controls.Add(Me.EditingTabs)
    Me.Controls.Add(Me.TrackBar1)
    Me.Controls.Add(Me.TrackBar4)
    Me.Controls.Add(Me.UoTStatus)
    Me.Controls.Add(Me.UoTMainMenu)
    Me.Controls.Add(Me.UoTRender)
    Me.DoubleBuffered = True
    Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
    Me.MainMenuStrip = Me.UoTMainMenu
    Me.Name = "MainWin"
    Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
    Me.Text = "Utility of Time R8"
    Me.UoTStatus.ResumeLayout(False)
    Me.UoTStatus.PerformLayout()
    CType(Me.TrackBar4, System.ComponentModel.ISupportInitialize).EndInit()
    Me.CollisionTab.ResumeLayout(False)
    Me.GroupBox1.ResumeLayout(False)
    Me.GroupBox1.PerformLayout()
    Me.CollisionGroupBox.ResumeLayout(False)
    Me.CollisionGroupBox.PerformLayout()
    Me.MiscTab.ResumeLayout(False)
    Me.MiscTab.PerformLayout()
    Me.GroupBox9.ResumeLayout(False)
    Me.GroupBox9.PerformLayout()
    Me.GroupBox10.ResumeLayout(False)
    Me.GroupBox10.PerformLayout()
    Me.GroupBox8.ResumeLayout(False)
    Me.GroupBox8.PerformLayout()
    Me.LevelFlagsTab.ResumeLayout(False)
    Me.GroupBox6.ResumeLayout(False)
    Me.GroupBox6.PerformLayout()
    Me.ActorsTab.ResumeLayout(False)
    Me.GroupBox4.ResumeLayout(False)
    Me.GroupBox4.PerformLayout()
    Me.GroupBox5.ResumeLayout(False)
    Me.GroupBox5.PerformLayout()
    Me.EditingTabs.ResumeLayout(False)
    Me.AnimationsTab.ResumeLayout(False)
    Me.DLTab.ResumeLayout(False)
    Me.DLTab.PerformLayout()
    Me.GroupBox7.ResumeLayout(False)
    Me.GroupBox7.PerformLayout()
    Me.DLEditorContextMenu.ResumeLayout(False)
    Me.GroupBox3.ResumeLayout(False)
    Me.GroupBox3.PerformLayout()
    Me.BackupMenuStrip.ResumeLayout(False)
    CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ActorContextMenu.ResumeLayout(False)
    Me.UoTMainMenu.ResumeLayout(False)
    Me.UoTMainMenu.PerformLayout()
    Me.VertContextMenu.ResumeLayout(False)
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub

#End Region


#Region "Global Variables"

#Region "ROM HANDLING RELATED"

  Private ObjectTable() As ObjectTbl
  Private ObjectFilename As String = ""
  Private ObjectDisplayName As String = ""
  Private ROMFiles As ZFiles
  Private Z64Code() As Byte
  Private IndMapFileName As String = ""
  Private IndScFileName As String = ""

#End Region

#Region "ACTOR RELATED"

  Private PrintToolStr As String = ""
  Private PrintTool As Boolean = False
  Private RoomActors() As Actor
  Private SceneActors() As Door
  Private rmActorCount As Integer = 0
  Private scActorCount As Integer = 0
  Private CopyActor() As Integer = {-1, -1, -1, -1, -1, -1, -1, -1} _
  'x pos, y pos, z pos, x rot, y rot, z rot, number, variable
  Private ActorPointer() As UInteger = {0, 0, 0} 'header pos, count, pointer
  Private RelocateActorPtr As Boolean = False
  Private ActorScale As Single = 16.0F
  Private ActorScaleP As Single = 45.0F
  Private ActorScaleW As Single = 2.05F
  Private HideActors(3) As Boolean
  Private RotCoef As Integer = &H4000
  Private ActorDBGroups As New ArrayList
  Private ActorDBNumber As New ArrayList
  Private ActorDBVars As New ArrayList
  Private ActorDBDesc As New ArrayList
  Private UsedGroupIndex() As Integer

#End Region

#Region "ANIMATION & HEIRARCHY"

  Private LimbEntries(-1) As Limb
  Private CurrLimb As Integer = 0
  Private BoneColorFactor As New Color3UByte
  Private CurrAnimation As IAnimation
  Private DlManager As New DlManager

#End Region

#Region "COLLISION RELATED"

  Private CollisionTriColor() As CollisionTriColorSelect
  Private ColA, ColB, ColC As UInt32
  Private CollisionVerts As CollisionVertex
  Private CollisionPolies() As PolygonCollision
  Private SelectedCollisionVert As New ArrayList
  Private ColTypes() As CollisionTypes
  Private ColPresets() As CollisionTypePresets

#End Region

#Region "CURRENT MAP RELATED"

  Private SceneMaps() As MapOffset
  Private SceneExits() As Exits
  Private ScannedObjSet As Boolean = False

#End Region

#Region "EDITING RELATED"

  Enum ToolID
    'identifiers
    CAMERA = 0
    ACTOR = 1
    VERTEX = 2
    EDGE = 3
    FACE = 4
    COLTRI = 5
    DLIST = 6
    NONE = Nothing

    'axis locks
    NOLOCK = 0
    LOCKTOX = 1
    LOCKTOY = 2
    LOCKTOZ = 3
    NOMOVE = 4

    'selected item identifiers
  End Enum

  Private CursorPosOld As Point
  Private HoldCursor As Boolean = False
  Private ToolModes As Tools
  Private AxisStrings() As String = {"(X + Y)", "(X)", "(Y)", "(Z)"}

  Private _
    ToolStrings() As String =
      {"Camera only", "Actor selector", "Vertex selector", "Edge selector", "Face selector",
       "Collision triangle selector", "Display List selector"}

  Private HighlightDL As Boolean = True
  Private EditRotAxis As Integer = 0
  Private EditRotType As Integer = 0
  Private MPick As Boolean = False
  Private ReadPixel(2) As Byte
  Private ChangePosition() As Boolean = {False, False, False, False, False, False}
  Private ToolSensitivity As Integer = 2
  Private ButtonPress As Integer = 0
  Private AxisGuideDList As UInt32 = 0
  Private ActorBoxDList As UInt32 = 0
  Private comb5 As Integer = 0
  Private comb7 As Integer = 0
  Private Objects As New ArrayList
  Private ObjectsDesc As New ArrayList

#End Region

#Region "MISCELLANEOUS"

  Private RipAllDLs As Boolean = False
  Private RawDLFile As FileStream
  Private colTri As Boolean = False
  Private UoTIniFile As New iniwriter(Application.StartupPath & "/uot.ini")
  Private Working As Boolean = False
  Private AppDirectory As String = Application.StartupPath

  Private key_w,
          key_a,
          key_s,
          key_d,
          key_q,
          key_e,
          key_u,
          key_o,
          key_ctrl,
          key_alt As Boolean

  Private MouseLeft As Boolean = False
  Private MouseRight As Boolean = False
  Private MouseMiddle As Boolean = False
  Private CameraCoef As Double = 46.1209812309812
  Private MouseOver As Boolean = False
  Private cam As New Camera
  Private NewMouseX As Double = 0.0
  Private NewMouseY As Double = 0.0
  Private OldMouseX As Double = 0.0
  Private OldMouseY As Double = 0.0
  Private LocalMouse As Point
  Private oldLocalMouse As Point
  Private Dx As Double = 0
  Private Dy As Double = 0
  Private Dz As Double = 0
  Private RenderCollision As Boolean = True
  Private RenderGraphics As Boolean = True
  Private wireframe As Boolean = False
  Private LoadedDataType As Integer = 0
  Private viewport(3) As Integer
  Private AppExit As Boolean = False

#End Region

#Region "EXTERNAL APP PROCESSES"

  Private crc As Process = New Process
  Private cfix As Process = New Process
  Private zlestart16 As Integer = 0
  Private zleend16 As Integer = 0
  Private zlestart6 As Integer = 0
  Private zleend6 As Integer = 0

#End Region

#Region "GENERIC LOOP ITERATORS"

  Private i1 As Integer = 0
  Private i2 As Integer = 0
  Private i3 As Integer = 0
  Private i4 As Integer = 0

#End Region

#End Region

#Region "App Interaction"

  Private Sub Initialize()
    ResetOGL()
    Wgl.wglSwapIntervalEXT(1)
    Reshape()
    FatherLoop()
  End Sub

  Private Sub ResetOGL()
    GlConstants.ResetGl()
  End Sub

  Private Sub FatherLoop()
    While Not AppExit
      MainLoop()
      Application.DoEvents()
      UoTRender.Invalidate()
    End While
    UoTRender.DestroyContexts()
    End
  End Sub

  Private Sub SyncCameraToActor(ByVal ActorType As Integer, ByVal Actor As Integer)
    Select Case ActorType
      Case 0
        cam.X = -RoomActors(Actor).x
        cam.Y = -RoomActors(Actor).y - 1000
        cam.Z = -RoomActors(Actor).z - 1000
      Case 1
        cam.X = -SceneActors(Actor).x
        cam.Y = -SceneActors(Actor).y - 1000
        cam.Z = -SceneActors(Actor).z - 1000
    End Select
    cam.Pitch = 45
  End Sub

  Private Sub MainLoop()
    Try
      Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT Or Gl.GL_COLOR_BUFFER_BIT)

      LocalMouse = UoTRender.PointToClient(Windows.Forms.Cursor.Position)

      NewMouseX = LocalMouse.X
      NewMouseY = LocalMouse.Y

      Dim camXRotd As Double = cam.Pitch / 180 * PI
      Dim camYRotd As Double = cam.Yaw / 180 * PI

      ' TODO: What on earth is going on below?

      Dim MouseChanged As Boolean = False
      If NewMouseX <> OldMouseX Then
        Dx = (NewMouseX - OldMouseX) * ToolSensitivity
        MouseChanged = True
      End If
      If NewMouseY <> OldMouseY Then
        Dy = (NewMouseY - OldMouseY) * ToolSensitivity
        MouseChanged = True
      End If

      If MouseChanged Then
        If MouseLeft Then
          If OldMouseX < NewMouseX Then
            '(MOUSE MOVE RIGHT)
            If ToolModes.SelectedItemType = ToolID.NONE Then
              cam.Yaw += (NewMouseX - OldMouseX) * 0.5
              cam.Yaw = cam.Yaw Mod 360
            End If
            If _
              (ToolModes.Axis = ToolID.NOLOCK Or ToolModes.Axis = ToolID.LOCKTOX) And
              (Not ToolModes.Axis = ToolID.NOMOVE) Then
              If ToolModes.SelectedItemType = ToolID.ACTOR Then
                If OnSceneActor Then
                  For i As Integer = 0 To SelectedSceneActors.Count - 1
                    i1 = SelectedSceneActors(i)
                    SceneActors(i1).x += Cos(camYRotd) * Dx
                    SceneActors(i1).z += Sin(camYRotd) * Dx
                  Next
                Else
                  For i As Integer = 0 To SelectedRoomActors.Count - 1
                    i1 = SelectedRoomActors(i)
                    RoomActors(i1).x += Cos(camYRotd) * Dx
                    RoomActors(i1).z += Sin(camYRotd) * Dx
                  Next
                End If
                cam.X += -Cos(camYRotd) * Dx
                cam.Z += -Sin(camYRotd) * Dx
                UpdateActorPos()
              ElseIf ToolModes.SelectedItemType = ToolID.VERTEX Then
                If RenderGraphics Then

                End If
                If RenderCollision Then
                  For i2 = 0 To SelectedCollisionVert.Count - 1
                    CollisionVerts.x(SelectedCollisionVert(i2)) += Cos(camYRotd) * Dx
                    CollisionVerts.z(SelectedCollisionVert(i2)) += Sin(camYRotd) * Dx
                  Next
                End If
                cam.X += -Cos(camYRotd) * Dx
                cam.Z += -Sin(camYRotd) * Dx
              End If
            End If
          End If
          If OldMouseX > NewMouseX Then
            '(MOUSE MOVE LEFT) 
            If ToolModes.SelectedItemType = ToolID.NONE Then
              cam.Yaw -= (OldMouseX - NewMouseX) * 0.5
              cam.Yaw = cam.Yaw Mod 360
            End If
            If _
              (ToolModes.Axis = ToolID.NOLOCK Or ToolModes.Axis = ToolID.LOCKTOX) And Not ToolModes.Axis = ToolID.NOMOVE _
              Then
              If ToolModes.SelectedItemType = ToolID.ACTOR Then
                If OnSceneActor Then
                  For i As Integer = 0 To SelectedSceneActors.Count - 1
                    i1 = SelectedSceneActors(i)
                    SceneActors(i1).x += Cos(camYRotd) * Dx
                    SceneActors(i1).z += Sin(camYRotd) * Dx
                  Next
                Else
                  For i As Integer = 0 To SelectedRoomActors.Count - 1
                    i1 = SelectedRoomActors(i)
                    RoomActors(i1).x += Cos(camYRotd) * Dx
                    RoomActors(i1).z += Sin(camYRotd) * Dx
                  Next
                End If
                cam.X -= Cos(camYRotd) * Dx
                cam.Z -= Sin(camYRotd) * Dx
                UpdateActorPos()
              ElseIf ToolModes.SelectedItemType = ToolID.VERTEX Then
                If RenderGraphics Then

                End If
                If RenderCollision Then
                  For i2 = 0 To SelectedCollisionVert.Count - 1
                    CollisionVerts.x(SelectedCollisionVert(i2)) += Cos(camYRotd) * Dx
                    CollisionVerts.z(SelectedCollisionVert(i2)) += Sin(camYRotd) * Dx
                  Next
                End If
                cam.X -= Cos(camYRotd) * Dx
                cam.Z -= Sin(camYRotd) * Dx
              End If
            End If
          End If
          If OldMouseY > NewMouseY Then
            '(MOUSE MOVE UP) 
            If ToolModes.SelectedItemType = ToolID.NONE Then
              If cam.Pitch <= -90 Then
                cam.Pitch = -90
              Else
                cam.Pitch += (Dy \ ToolSensitivity) * 0.5
              End If
            End If
            If _
              (ToolModes.Axis = ToolID.NOLOCK Or ToolModes.Axis = ToolID.LOCKTOY) And Not ToolModes.Axis = ToolID.NOMOVE _
              Then
              If ToolModes.SelectedItemType = ToolID.ACTOR Then
                If OnSceneActor Then
                  For i As Integer = 0 To SelectedSceneActors.Count - 1
                    SceneActors(SelectedSceneActors(i)).y -= Dy
                  Next
                Else
                  For i As Integer = 0 To SelectedRoomActors.Count - 1
                    RoomActors(SelectedRoomActors(i)).y -= Dy
                  Next
                End If
                cam.Y += Dy
                UpdateActorPos()
              ElseIf ToolModes.SelectedItemType = ToolID.VERTEX Then
                If RenderGraphics Then

                End If
                If RenderCollision Then
                  For i2 = 0 To SelectedCollisionVert.Count - 1
                    CollisionVerts.y(SelectedCollisionVert(i2)) -= Dy
                  Next
                End If
                cam.Y += Dy
              End If
            End If
          End If
          If OldMouseY < NewMouseY Then
            '(MOUSE MOVE DOWN) 
            If ToolModes.SelectedItemType = ToolID.NONE Then
              If cam.Pitch >= 90 Then
                cam.Pitch = 90
              Else
                cam.Pitch += (Dy \ ToolSensitivity) * 0.5
              End If
            End If
            If _
              (ToolModes.Axis = ToolID.NOLOCK Or ToolModes.Axis = ToolID.LOCKTOY) And Not ToolModes.Axis = ToolID.NOMOVE _
              Then
              If ToolModes.SelectedItemType = ToolID.ACTOR Then
                If OnSceneActor Then
                  For i As Integer = 0 To SelectedSceneActors.Count - 1
                    SceneActors(SelectedSceneActors(i)).y -= Dy
                  Next
                Else
                  For i As Integer = 0 To SelectedRoomActors.Count - 1
                    RoomActors(SelectedRoomActors(i)).y -= Dy
                  Next
                End If
                cam.Y += Dy
                UpdateActorPos()
              ElseIf ToolModes.SelectedItemType = ToolID.VERTEX Then
                If RenderGraphics Then

                End If
                If RenderCollision Then
                  For i2 = 0 To SelectedCollisionVert.Count - 1
                    CollisionVerts.y(SelectedCollisionVert(i2)) -= Dy
                  Next
                End If
                cam.Y += Dy
              End If
            End If
          End If
        ElseIf MouseMiddle Then
          If OldMouseY < NewMouseY Then
            '(MOUSE MOVE DOWN) 
            If ToolModes.SelectedItemType = ToolID.NONE Then
              cam.Y += (OldMouseY - NewMouseY) * (CameraCoef / 8)
            ElseIf ToolModes.SelectedItemType = ToolID.ACTOR Then
              If OnSceneActor Then
                For i As Integer = 0 To SelectedSceneActors.Count - 1
                  i1 = SelectedSceneActors(i)
                  SceneActors(i1).x += Sin(camYRotd) * Dy
                  SceneActors(i1).z -= Cos(camYRotd) * Dy
                Next
              Else
                For i As Integer = 0 To SelectedRoomActors.Count - 1
                  i1 = SelectedRoomActors(i)
                  RoomActors(i1).x += Sin(camYRotd) * Dy
                  RoomActors(i1).z -= Cos(camYRotd) * Dy
                Next
              End If
              cam.X -= Sin(camYRotd) * Dy
              cam.Z += Cos(camYRotd) * Dy
              UpdateActorPos()
            End If
          End If
          If OldMouseY > NewMouseY Then
            '(MOUSE MOVE DOWN) 
            If ToolModes.SelectedItemType = ToolID.NONE Then
              cam.Y -= (NewMouseY - OldMouseY) * (CameraCoef / 8)
            ElseIf ToolModes.SelectedItemType = ToolID.ACTOR Then
              If OnSceneActor Then
                For i As Integer = 0 To SelectedSceneActors.Count - 1
                  i1 = SelectedSceneActors(i)
                  SceneActors(i1).x += Sin(camYRotd) * Dy
                  SceneActors(i1).z -= Cos(camYRotd) * Dy
                Next
              Else
                For i As Integer = 0 To SelectedRoomActors.Count - 1
                  i1 = SelectedRoomActors(i)
                  RoomActors(i1).x += Sin(camYRotd) * Dy
                  RoomActors(i1).z -= Cos(camYRotd) * Dy
                Next
              End If
              cam.X -= Sin(camYRotd) * Dy
              cam.Z += Cos(camYRotd) * Dy
              UpdateActorPos()
            End If
          End If
        End If
      End If

      If HoldCursor Then
        Cursor.Position = CursorPosOld
        Dim curColor As Color = Color.White
        Select Case ToolModes.SelectedItemType
          Case ToolID.ACTOR
            curColor = Color.Aquamarine
          Case ToolID.VERTEX
            curColor = Color.Tomato
          Case ToolID.DLIST
            curColor = Color.Blue
        End Select
        GLPrint2D("+", UoTRender.PointToClient(Cursor.Position), curColor, Glut.GLUT_BITMAP_TIMES_ROMAN_24, -10, -15,
                  True)
      End If
      If PrintTool Then
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_FRAGMENT_PROGRAM_ARB)
        GLPrint2D(PrintToolStr, UoTRender.PointToClient(Cursor.Position), Color.White, Glut.GLUT_BITMAP_HELVETICA_18, 0,
                  0, True)
      End If

      cam.Move(key_w - key_s, key_d - key_a, CameraCoef)

      UpdateCamLabels()

      Gl.glLoadIdentity()
      Gl.glRotated(cam.Pitch, 1.0F, 0.0F, 0.0F)
      Gl.glRotated(cam.Yaw, 0.0F, 1.0F, 0.0F)
      Gl.glTranslated(cam.X, cam.Y, cam.Z)

      ModelViewMatrixTransformer.Push()
      If LoadedDataType = FileTypes.MAP Then DrawActorBoxes(False)
      If RenderGraphics Then DrawDLArray(ToolID.NONE)
      If RenderCollision Then DrawCollision(CollisionPolies, CollisionVerts, False)
      ModelViewMatrixTransformer.Pop()

      UoTRender.SwapBuffers()

      If MouseChanged And Not HoldCursor Then
        MouseOver = True
        If ToolModes.CurrentTool < ToolID.DLIST Then
          PickItem(ToolModes.CurrentTool, Nothing)
        End If
      End If
    Catch err As Exception
      If HandleErrors Then
        GenericCatch(err)
      Else
        Throw
      End If
    End Try
  End Sub

  Private Sub GenericCatch(ByVal err As Exception)
    MsgBox(
      "Error!" & Environment.NewLine & Environment.NewLine & "From routine: " & err.TargetSite.Name & "()" &
      Environment.NewLine & Environment.NewLine & "Details: " & err.Message, MsgBoxStyle.Critical, "Bug")
  End Sub

  Private Sub UpdateCamLabels()
    CamXLabel.Text = "Cam X: " & CStr(cam.X)
    CamYLabel.Text = "Cam Y: " & CStr(cam.Y)
    CamZLabel.Text = "Cam Z: " & CStr(cam.Z)
  End Sub

  Private Sub DrawActorBoxes(ByVal SelectionMode As Boolean)
    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
    SetOGLDefaultParams()

    If RoomActors.Length > 0 And Not HideActors(0) Then
      For i As Integer = 0 To RoomActors.Length - 1
        Gl.glPushMatrix()
        Gl.glTranslatef(RoomActors(i).x, RoomActors(i).y + 16, RoomActors(i).z)
        Gl.glRotatef(RoomActors(i).xr \ 180, 1.0F, 0.0F, 0.0F)
        Gl.glRotatef(RoomActors(i).yr \ 180, 0.0F, 1.0F, 0.0F)
        Gl.glRotatef(RoomActors(i).zr \ 180, 0.0F, 0.0F, 1.0F)
        If SelectionMode Then
          If ToolModes.NoDepthTest Then Gl.glDisable(Gl.GL_DEPTH_TEST)
          Gl.glColor3ub(RoomActors(i).pickR, RoomActors(i).pickG, RoomActors(i).pickB)
          Glut.glutSolidCube(ActorScaleP)
          If ToolModes.NoDepthTest Then Gl.glEnable(Gl.GL_DEPTH_TEST)
        Else
          Gl.glScalef(ActorScale, ActorScale, ActorScale)
          Gl.glCallList(ActorBoxDList)
        End If
        If Not SelectedRoomActors.Contains(i) Then
          Gl.glColor3f(0, 0, 0)
          Gl.glLineWidth(2)
        Else
          If Not HideActors(2) Then
            Gl.glCallList(AxisGuideDList)
          End If
          If SelectedRoomActors.IndexOf(i) > 0 Then Gl.glColor3f(0.0F, 0.6F, 0.2F) Else Gl.glColor3f(1, 0, 0)
          Gl.glLineWidth(3)
        End If
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
        Glut.glutSolidCube(ActorScaleW)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glLineWidth(1)
        Gl.glPopMatrix()
      Next
    End If
    If SceneActors.Length > 0 And Not HideActors(1) Then
      For i As Integer = 0 To SceneActors.Length - 1
        Gl.glPushMatrix()
        Gl.glTranslatef(SceneActors(i).x, SceneActors(i).y + 16, SceneActors(i).z)
        Gl.glRotatef(SceneActors(i).yr \ 180, 0.0F, 1.0F, 0.0F)
        If SelectionMode Then
          Gl.glColor3ub(SceneActors(i).pickR, SceneActors(i).pickG, SceneActors(i).pickB)
          Glut.glutSolidCube(ActorScaleP)
        Else
          Gl.glScalef(ActorScale, ActorScale, ActorScale)
          Gl.glCallList(ActorBoxDList)
        End If
        If Not SelectedSceneActors.Contains(i) Then
          Gl.glColor3f(0, 0, 0)
          Gl.glLineWidth(2)
        Else
          If Not HideActors(2) Then
            Gl.glCallList(AxisGuideDList)
          End If
          If SelectedRoomActors.IndexOf(i) > 0 Then Gl.glColor3f(0.0F, 0.6F, 0.2F) Else Gl.glColor3f(1, 0, 0)
          Gl.glLineWidth(3)
        End If
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
        Glut.glutSolidCube(2.06F)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glLineWidth(1)
        Gl.glPopMatrix()
      Next
    End If
    If wireframe Then
      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
    End If
  End Sub

  Private Sub DrawDLArray(ByVal SelectionMode As Integer)
    ' Used for some hacks.
    Time.UpdateCurrent()

    ' TODO: Don't update this each frame.
    Dim indirectTextureHack As IIndirectTextureHack = IndirectTextureHacks.Get(ObjectFilename)
    DLParser.EnableHacksFor(ObjectFilename, indirectTextureHack)

    animationTab_.AnimationPlaybackManager.Tick()

    'If True Then
    'SkeletonRenderer.Render(LimbEntries, DLParser.LimbMatrices, CurrAnimation, animationTab_.AnimationPlaybackManager)
    'SkeletonRenderer.Render(DlModel.Limbs, DlModel.LimbMatrices, CurrAnimation, animationTab_.AnimationPlaybackManager)
    'Return
    'End If

    If UseStaticDlModel And DlModel.IsComplete Then
      DlModel.Draw(CurrAnimation, animationTab_.AnimationPlaybackManager)
      Return
    End If

    If DlManager.Count = 0 Then
      Return
    End If

    If SelectionMode Then
      DLParser.ParseMode = DLParser.Parse.GEOMETRY
    Else
      DLParser.ParseMode = DLParser.Parse.EVERYTHING
    End If

    ModelViewMatrixTransformer.Push()

    If Not DlManager.HasLimbs Then
      For i As Integer = 0 To DlManager.Count - 1
        DrawDL(i, SelectionMode)
      Next
    Else
      CurrLimb = 0

      If CurrAnimation IsNot Nothing Then
        Dim frame As Double = animationTab_.AnimationPlaybackManager.Frame
        Dim totalFrames As Integer = animationTab_.AnimationPlaybackManager.TotalFrames

        Dim frameIndex As Integer = Math.Floor(frame)
        Dim frameDelta As Double = frame Mod 1

        Dim startPos As Vec3s = CurrAnimation.GetPosition(frameIndex)
        Dim endPos As Vec3s = CurrAnimation.GetPosition((frameIndex + 1) Mod totalFrames)

        Dim f As Double = frameDelta

        ' TODO: Move this out.
        Dim x As Double = startPos.X * (1 - f) + endPos.X * f
        Dim y As Double = startPos.Y * (1 - f) + endPos.Y * f
        Dim z As Double = startPos.Z * (1 - f) + endPos.Z * f

        ModelViewMatrixTransformer.Translate(x, y, z)

        If indirectTextureHack IsNot Nothing Then
          Dim face As FacialState = CurrAnimation.GetFacialState(frameIndex)
          indirectTextureHack.EyeState = face.EyeState
          indirectTextureHack.MouthState = face.MouthState
        End If
      End If

      If False Then
        For CurrLimb = 0 To LimbEntries.Length - 1
          With BoneColorFactor
            .r = 0
            .g = 0
            .b = 0
          End With
          'DrawJoint(CurrLimb)
        Next
      End If

      ' TODO: Precalculate matrices via a helper class.

      DLParser.LimbMatrices.UpdateLimbMatrices(LimbEntries, CurrAnimation, animationTab_.AnimationPlaybackManager.Frame)

      DrawJoint(0)

    End If
    ModelViewMatrixTransformer.Pop()

    If Not DlModel.IsComplete Then
      DlModel.IsComplete = True
      'DlModel.SaveAsGlTf(ObjectDisplayName, animationTab_.AnimationBanks.Animations)

      Dim modelConverter = New ModelConverter()
      modelConverter.Convert()
    End If
  End Sub

  Private Sub DrawJoint(ByVal id As Integer)
    With LimbEntries(id)
      'If id + 1 < LimbEntries.Length - 1 Then
      'CurrLimb = id + 1
      'Else
      ' CurrLimb = id
      'End If

      Dim dlIndex As Integer = - 1
      If .DisplayListAddress > Nothing Then
        dlIndex = DlManager.GetIndexByAddress(.DisplayListAddress)
      End If
      Dim validDl As Boolean = dlIndex > - 1

      If animationTab_.AnimationBanks.ShowBones Then
        Dim xI As Double = 0
        Dim yI As Double = 0
        Dim zI As Double = 0
        ModelViewMatrixTransformer.ProjectVertex(xI, yI, zI)

        Dim xF As Double = .x
        Dim yF As Double = .y
        Dim zF As Double = .z
        ModelViewMatrixTransformer.ProjectVertex(xF, yF, zF)

        Gl.glDepthRange(0, 0)
        Gl.glLineWidth(9)
        Gl.glBegin(Gl.GL_LINES)
        Gl.glColor3f(1, 1, 1)
        Gl.glVertex3f(xI, yI, zI)
        Gl.glVertex3f(xF, yF, zF)
        Gl.glEnd()
        Gl.glDepthRange(0, - 0.5)
        Gl.glPointSize(11)
        Gl.glBegin(Gl.GL_POINTS)
        Gl.glColor3f(0, 0, 0)
        Gl.glVertex3f(xF, yF, zF)
        Gl.glEnd()
        Gl.glPointSize(8)
        Gl.glBegin(Gl.GL_POINTS)
        Gl.glColor3ub(BoneColorFactor.r, BoneColorFactor.g, BoneColorFactor.b)
        Gl.glVertex3f(xF, yF, zF)
        Gl.glEnd()
        Gl.glPointSize(1)
        Gl.glLineWidth(1)
        Gl.glDepthRange(0, 1)
      End If

      ModelViewMatrixTransformer.Push()
      ModelViewMatrixTransformer.Set(DLParser.LimbMatrices.GetMatrixForLimb(id))
      DlModel.SetCurrentLimbByIndex(id)

      If validDl Then
        DrawDL(dlIndex, False)
      End If

      If .firstChild > - 1 Then
        BoneColorFactor.r = 255
        BoneColorFactor.g = 0
        BoneColorFactor.b = 0
        DrawJoint(.firstChild)
      Else
        BoneColorFactor.r = 255
        BoneColorFactor.g = 255
        BoneColorFactor.b = 255
      End If

      ModelViewMatrixTransformer.Pop()

      If .nextSibling > - 1 Then
        BoneColorFactor.r = 0
        BoneColorFactor.g = 0
        BoneColorFactor.b = 255
        DrawJoint(.nextSibling)
      Else
        BoneColorFactor.r = 255
        BoneColorFactor.g = 255
        BoneColorFactor.b = 255
      End If
    End With
  End Sub

  Private Sub DrawDL(index As Integer, SelectionMode As Integer)
    Dim displayList As N64DisplayList = DlManager.GetDisplayListByIndex(index)

    If displayList.Skip Then
      Return
    End If

    If SelectionMode = ToolID.NONE Then
      DLParser.ParseDL(displayList, DlManager)
      If displayList.Highlight Then
        DLParser.ParseMode = DLParser.Parse.GEOMETRY
        Gl.glBindProgramARB(Gl.GL_FRAGMENT_PROGRAM_ARB, HighlightProg)
        Gl.glEnable(Gl.GL_FRAGMENT_PROGRAM_ARB)
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        DLParser.ParseDL(displayList, DlManager)
        DLParser.ParseMode = DLParser.Parse.EVERYTHING
      End If
    ElseIf SelectionMode = ToolID.DLIST Then
      Gl.glColor3ub(displayList.PickCol.r, displayList.PickCol.g, displayList.PickCol.b)
      DLParser.ParseDL(displayList, DlManager)
      ReadPixel = MousePixelRead(NewMouseX, NewMouseY)
      If _
        ReadPixel(0) = displayList.PickCol.r And ReadPixel(1) = displayList.PickCol.g And
        ReadPixel(2) = displayList.PickCol.b Then
        DListSelection.SelectedIndex = index + 1
        EditingTabs.SelectedTab = EditingTabs.TabPages("DLTab")
        ToolModes.SelectedItemType = ToolID.DLIST
        Exit Sub
      End If
      ToolModes.SelectedItemType = ToolID.NONE
    End If
  End Sub

  Private Sub DrawCollision(ByVal Polygons() As PolygonCollision, ByVal Vertices As CollisionVertex,
                            ByVal SelectionMode As Boolean)
    Gl.glDisable(Gl.GL_TEXTURE_2D)
    Gl.glDisable(Gl.GL_FRAGMENT_PROGRAM_ARB)
    Gl.glDisable(Gl.GL_CULL_FACE)
    Gl.glEnable(Gl.GL_BLEND)
    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
    Gl.glAlphaFunc(Gl.GL_GREATER, 0.0)
    If Not SelectionMode Then
      Gl.glEnable(Gl.GL_POLYGON_OFFSET_LINE)
      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)

      Gl.glPolygonOffset(- 5.0, - 5.0)

      Gl.glBegin(Gl.GL_TRIANGLES)
      Gl.glColor3f(0, 0, 0)
      For i As Integer = 0 To Polygons.Length - 1
        With Polygons(i)
          ColA = .A
          ColB = .B
          ColC = .C
        End With
        With Vertices
          Gl.glVertex3i(.x(ColA), .y(ColA), .z(ColA))
          Gl.glVertex3i(.x(ColB), .y(ColB), .z(ColB))
          Gl.glVertex3i(.x(ColC), .y(ColC), .z(ColC))
        End With
      Next
      Gl.glEnd()

      Gl.glDisable(Gl.GL_POLYGON_OFFSET_LINE)
      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

      Gl.glPolygonOffset(- 6, - 6)
      Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
      Gl.glBegin(Gl.GL_TRIANGLES)
      For i As Integer = 0 To Polygons.Length - 1
        With Polygons(i)
          ColA = .A
          ColB = .B
          ColC = .C
        End With
        Gl.glColor4f(1, CollisionTriColor(i).g, CollisionTriColor(i).b, 0.25)
        With Vertices
          Gl.glVertex3i(.x(ColA), .y(ColA), .z(ColA))
          Gl.glVertex3i(.x(ColB), .y(ColB), .z(ColB))
          Gl.glVertex3i(.x(ColC), .y(ColC), .z(ColC))
        End With
      Next
      Gl.glEnd()
      Select Case ToolModes.CurrentTool
        Case ToolID.VERTEX
          Gl.glPointSize(14)
          Gl.glBegin(Gl.GL_POINTS)
          For i As Integer = 0 To Vertices.x.Count - 1
            If SelectedCollisionVert.Contains(i) Then Gl.glColor3f(1, 0, 0) Else Gl.glColor3f(1, 1, 1)
            Gl.glVertex3i(Vertices.x(i), Vertices.y(i), Vertices.z(i))
          Next
          Gl.glEnd()
          Gl.glPointSize(11)
          Gl.glBegin(Gl.GL_POINTS)
          For i As Integer = 0 To Vertices.x.Count - 1
            If SelectedCollisionVert.Contains(i) Then Gl.glColor3f(0, 0, 1) Else Gl.glColor3f(0, 0, 0)
            Gl.glVertex3i(Vertices.x(i), Vertices.y(i), Vertices.z(i))
          Next
          Gl.glEnd()
        Case ToolID.COLTRI

      End Select
    Else
      Select Case ToolModes.CurrentTool
        Case ToolID.VERTEX
          Gl.glDisable(Gl.GL_BLEND)
          Gl.glPointSize(23)
          Gl.glBegin(Gl.GL_POINTS)
          For i As Integer = 0 To Vertices.x.Count - 1
            Gl.glColor3ub(Vertices.VertR(i), Vertices.VertG(i), Vertices.VertB(i))
            Gl.glVertex3i(Vertices.x(i), Vertices.y(i), Vertices.z(i))
          Next
          Gl.glEnd()
        Case ToolID.EDGE
          Dim curedge As Integer = 0
          Gl.glEnable(Gl.GL_POLYGON_OFFSET_LINE)
          Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
          Gl.glPolygonOffset(- 8.0, - 8.0)
          Gl.glLineWidth(10)
          Gl.glBegin(Gl.GL_TRIANGLES)
          For i As Integer = 0 To Vertices.EdgeR.Count - 1
            ColA = Polygons(curedge).A
            ColB = Polygons(curedge + 1).B
            Gl.glColor3ub(Vertices.EdgeR(i), Vertices.EdgeG(i), Vertices.EdgeB(i))
            Gl.glVertex3i(Vertices.x(ColA), Vertices.y(ColA), Vertices.z(ColA))
            Gl.glVertex3i(Vertices.x(ColB), Vertices.y(ColB), Vertices.z(ColB))
            curedge += 1
          Next
          Gl.glEnd()
          Gl.glDisable(Gl.GL_POLYGON_OFFSET_LINE)
          Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Case ToolID.COLTRI
          Gl.glPolygonOffset(- 8, - 8)
          Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
          Gl.glBegin(Gl.GL_TRIANGLES)
          For i As Integer = 0 To Polygons.Length - 1
            With Polygons(i)
              ColA = .A
              ColB = .B
              ColC = .C
              Gl.glColor3ub(.pickR, .pickG, .pickB)
              With Vertices
                Gl.glVertex3i(.x(ColA), .y(ColA), .z(ColA))
                Gl.glVertex3i(.x(ColB), .y(ColB), .z(ColB))
                Gl.glVertex3i(.x(ColC), .y(ColC), .z(ColC))
              End With
            End With
          Next
          Gl.glEnd()
          Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
      End Select
    End If
    SetOGLDefaultParams()
  End Sub

  Private Sub SimpleOpenGlControl1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) _
    Handles UoTRender.KeyDown, UoTRender.KeyDown
    Select Case e.KeyCode
      Case Keys.E
        CameraSensitivity(True)
      Case Keys.Q
        CameraSensitivity(False)
      Case Keys.ControlKey
        If Not key_ctrl Then
          key_ctrl = True
        End If
      Case Keys.I
        ButtonPress = 1
        ActorInputTimer.Start()
      Case Keys.K
        ButtonPress = 2
        ActorInputTimer.Start()
      Case Keys.J
        ButtonPress = 0
        ActorInputTimer.Start()
      Case Keys.L
        ButtonPress = 3
        ActorInputTimer.Start()
      Case Keys.U
        ButtonPress = 4
        ActorInputTimer.Start()
      Case Keys.O
        ButtonPress = 5
        ActorInputTimer.Start()
      Case Keys.Q
        If Not key_q Then key_q = True
      Case Keys.E
        If Not key_e Then key_e = True
      Case Keys.W
        If Not key_w Then key_w = True
      Case Keys.D
        If Not key_d Then key_d = True
      Case Keys.S
        If Not key_s Then key_s = True
      Case Keys.A
        If Not key_a Then key_a = True
      Case Keys.F1
        ToggleWire()
      Case Keys.T
        ButtonPress = 6
        ActorInputTimer.Start()
      Case Keys.Y
        ButtonPress = 7
        ActorInputTimer.Start()
      Case Keys.G
        ButtonPress = 8
        ActorInputTimer.Start()
      Case Keys.H
        ButtonPress = 9
        ActorInputTimer.Start()
      Case Keys.B
        If LoadedDataType = FileTypes.MAP Then
          ResetActors(False)
        End If
      Case Keys.R
        If key_ctrl Then
          If RenderGraphics And Not RenderCollision Then
            RenderGraphics = False
            RenderCollision = True
            ViewingMeshToolStripMenuItem1.Checked = False
            CollisionMeshToolStripMenuItem.Checked = True
          ElseIf RenderGraphics And RenderCollision Then
            RenderGraphics = True
            RenderCollision = False
            ViewingMeshToolStripMenuItem1.Checked = True
            CollisionMeshToolStripMenuItem.Checked = False
          Else
            RenderGraphics = True
            RenderCollision = True
            ViewingMeshToolStripMenuItem1.Checked = True
            CollisionMeshToolStripMenuItem.Checked = True
          End If
        Else
          ResetView()
        End If
      Case Keys.Alt
        key_alt = True
      Case Keys.D1
        SwitchTool(ToolID.CAMERA)
      Case Keys.D2
        SwitchTool(ToolID.ACTOR)
      Case Keys.D3
        SwitchTool(ToolID.VERTEX)
      Case Keys.D4
        SwitchTool(ToolID.EDGE)
      Case Keys.D5
        SwitchTool(ToolID.FACE)
      Case Keys.D6
        SwitchTool(ToolID.COLTRI)
      Case Keys.D7
        SwitchTool(ToolID.DLIST)
      Case Keys.P
        If key_ctrl And LoadedDataType = FileTypes.MAP Then
          ActorPresets.Show()
          ActorPresets.Focus()
          key_ctrl = False
        End If
    End Select
  End Sub

  Sub ToggleWire()
    If Not wireframe Then
      wireframe = True
      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
    Else
      wireframe = False
      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
    End If
  End Sub

  Sub BuildAxis()
    AxisGuideDList = Gl.glGenLists(1)
    Gl.glNewList(AxisGuideDList, Gl.GL_COMPILE)
    Gl.glBegin(Gl.GL_LINES)
    Gl.glColor3f(0, 0, 1)
    Gl.glVertex3f(420, 0.1, 0.1)
    Gl.glVertex3f(- 420, 0.1, 0.1)
    Gl.glEnd()
    Gl.glBegin(Gl.GL_LINES)
    Gl.glColor3f(1, 0, 0)
    Gl.glVertex3f(0.1, 420, 0.1)
    Gl.glVertex3f(0.1, - 420, 0.1)
    Gl.glEnd()
    Gl.glBegin(Gl.GL_LINES)
    Gl.glColor3f(0, 1, 0)
    Gl.glVertex3f(0.1, 0.1, 420)
    Gl.glVertex3f(0.1, 0.1, - 420)
    Gl.glVertex3f(0.1, 0.1, 420)
    Gl.glEnd()
    Gl.glEndList()
    Gl.glLineWidth(1)

    ActorBoxDList = Gl.glGenLists(1)
    Gl.glNewList(ActorBoxDList, Gl.GL_COMPILE)

    Gl.glBegin(Gl.GL_QUADS)
    'top
    Gl.glColor3f(1, 1, 1)
    Gl.glVertex3f(1.0F, 1.0F, - 1.0F)
    Gl.glVertex3f(- 1.0F, 1.0F, - 1.0F)
    Gl.glVertex3f(- 1.0F, 1.0F, 1.0F)
    Gl.glVertex3f(1.0F, 1.0F, 1.0F)
    'bottom
    Gl.glColor3f(0, 0, 0)
    Gl.glVertex3f(1.0F, - 1.0F, 1.0F)
    Gl.glVertex3f(- 1.0F, - 1.0F, 1.0F)
    Gl.glVertex3f(- 1.0F, - 1.0F, - 1.0F)
    Gl.glVertex3f(1.0F, - 1.0F, - 1.0F)
    'front
    Gl.glColor3f(1, 0, 0)
    Gl.glVertex3f(1.0F, 1.0F, 1.0F)
    Gl.glVertex3f(- 1.0F, 1.0F, 1.0F)
    Gl.glVertex3f(- 1.0F, - 1.0F, 1.0F)
    Gl.glVertex3f(1.0F, - 1.0F, 1.0F)
    'back
    Gl.glColor3f(0, 1, 0)
    Gl.glVertex3f(1.0F, - 1.0F, - 1.0F)
    Gl.glVertex3f(- 1.0F, - 1.0F, - 1.0F)
    Gl.glVertex3f(- 1.0F, 1.0F, - 1.0F)
    Gl.glVertex3f(1.0F, 1.0F, - 1.0F)
    'left
    Gl.glColor3f(1, 1, 0)
    Gl.glVertex3f(- 1.0F, 1.0F, 1.0F)
    Gl.glVertex3f(- 1.0F, 1.0F, - 1.0F)
    Gl.glVertex3f(- 1.0F, - 1.0F, - 1.0F)
    Gl.glVertex3f(- 1.0F, - 1.0F, 1.0F)
    'right
    Gl.glColor3f(0, 0, 1)
    Gl.glVertex3f(1.0F, 1.0F, - 1.0F)
    Gl.glVertex3f(1.0F, 1.0F, 1.0F)
    Gl.glVertex3f(1.0F, - 1.0F, 1.0F)
    Gl.glVertex3f(1.0F, - 1.0F, - 1.0F)
    Gl.glEnd()
    Gl.glEndList()
  End Sub

  ' TODO: Here is where the form is initialized!
  Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    Try
      Glut.glutInit()
      Wgl.ReloadFunctions()
      Gl.ReloadFunctions()
      UoTRender.CreateGraphics()

      Dim blank_tex() As Byte = {&HFF, &HFF, &HFF, &HFF}
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 2)
      Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 1, 1, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, blank_tex)

      With CollisionVerts
        .x = New ArrayList
        .y = New ArrayList
        .z = New ArrayList
        .VertR = New ArrayList
        .VertG = New ArrayList
        .VertB = New ArrayList

        .FaceR = New ArrayList
        .FaceG = New ArrayList
        .FaceB = New ArrayList

        .EdgeR = New ArrayList
        .EdgeG = New ArrayList
        .EdgeB = New ArrayList
      End With

      ReDim CollisionPolies(-1)


      If GLExtensions.GLAnisotropic Then
        RenderToggles.Anisotropic = True
      End If


      Dim highlightbytes() As Byte = System.Text.Encoding.ASCII.GetBytes(HighlightShader)
      If GLExtensions.GLFragProg Then
        Gl.glGenProgramsARB(1, HighlightProg)
        Gl.glBindProgramARB(Gl.GL_FRAGMENT_PROGRAM_ARB, HighlightProg)
        Gl.glProgramStringARB(Gl.GL_FRAGMENT_PROGRAM_ARB, Gl.GL_PROGRAM_FORMAT_ASCII_ARB, highlightbytes.Length,
                              highlightbytes)
      End If

      'EditingTabs.TabPages.Remove(GraphicsTab)

      AppDirectory = Application.StartupPath
      If Not File.Exists(AppDirectory & "/uot.ini") Then
        Dim createini As FileStream = File.Create(AppDirectory & "/uot.ini")
        createini.Dispose()
      End If
      UoTIniFile = New iniwriter(AppDirectory & "/uot.ini")

      BuildAxis()

      ToolModes = New Tools
      SwitchTool(ToolID.CAMERA)

      With cfix.StartInfo
        .UseShellExecute = True
        .CreateNoWindow = True
        .WindowStyle = ProcessWindowStyle.Hidden
        .FileName = AppDirectory & "\ext\collision_fixer.exe"
        .WorkingDirectory = AppDirectory & "\ext"
      End With
      With crc.StartInfo
        .UseShellExecute = True
        .CreateNoWindow = True
        .WindowStyle = ProcessWindowStyle.Hidden
        .FileName = AppDirectory & "\ext\rn64crc.exe"
      End With

      Dim winw As String = UoTIniFile.GetString("Settings", "WinResW", Nothing)
      Dim winh As String = UoTIniFile.GetString("Settings", "WinResH", Nothing)
      Dim toolst As String = UoTIniFile.GetString("Settings", "ToolSensitivity", Nothing)
      Dim camst As String = UoTIniFile.GetString("Settings", "CameraSpeed", Nothing)
      DefROM = UoTIniFile.GetString("Settings", "DefaultROM", Nothing)

      If DefROM = "" Or Not File.Exists(DefROM) Then
        LoadROM.ShowDialog()
      End If

      If toolst <> "" Then
        If CInt(toolst) <= 15 And CInt(toolst) >= 1 Then
          TrackBar4.Value = CInt(toolst)
        Else
          TrackBar4.Value = 4
        End If
      Else
        TrackBar4.Value = 4
      End If
      If camst <> "" Then
        If CInt(camst) <= 40 And CInt(camst) >= 1 Then
          TrackBar1.Value = CInt(camst)
        Else
          TrackBar1.Value = 20
        End If
      Else
        TrackBar1.Value = 20
      End If

      If winw <> "" And winh <> "" Then
        Me.Size = New Size(CInt(winw), CInt(winh))
      Else
        Me.Size = New Size(1168, 676)
      End If

      DLParser.Initialize()

      args = Environment.GetCommandLineArgs()
      If args.Length = 2 Then
        LoadROM.FileName = args(1)
        Start(False)
      ElseIf File.Exists(DefROM) Then
        LoadROM.FileName = DefROM
        Start(False)
      End If
      ReDim args(-1)

      Me.Show()
      Me.Focus()
      Initialize()
    Catch err As Exception
      If HandleErrors Then
        MsgBox(err.Message)
      Else
        Throw
      End If
    End Try
  End Sub

  Private Sub ResetView() 'Reset to default view
    cam.Reset()
  End Sub

  Private Function ReadActorDBHuman(ByVal fn As String) As ActorDB()
    If File.Exists(fn) Then
      Dim tDB() As ActorDB
      Dim tReader As StreamReader = New StreamReader(fn)
      Dim Tokens() As String
      Dim nextTokens() As String
      Dim actorCnt As Integer = 0
      Dim varCnt As Integer = 0
      ReDim tDB(- 1)
      readMain: While tReader.Peek <> - 1
        Tokens = tReader.ReadLine.Split(" ")
        If Tokens.Length > 1 Then
          Dim testOne As String = Tokens(0)
          Dim intTest As Integer = 0
          If Int32.TryParse(testOne, intTest) Then
            GoTo readActor
          Else
            GoTo readMain
          End If

          readActor: ReDim Preserve tDB(actorCnt)
          With tDB(actorCnt)
            .no = Int32.Parse(Tokens(0), Globalization.NumberStyles.HexNumber)

            If Int32.TryParse(Tokens(1), intTest) Then
              If Tokens(1).Contains("+") Then
                .grp = Int32.Parse(Mid(Tokens(1), 1, 4), Globalization.NumberStyles.HexNumber)
              Else
                .grp = Int32.Parse(Tokens(1), Globalization.NumberStyles.HexNumber)
              End If
              .desc = ""
              If Tokens.Length > 2 Then
                For i As Integer = 2 To Tokens.Length - 1
                  .desc += Tokens(i) & " "
                Next
              End If
            End If
          End With
          nextTokens = tReader.ReadLine.Split(" ")
          If nextTokens.Length > 1 Then
            readVars: While nextTokens(0) = "" And nextTokens(1) = "-"

              ReDim Preserve tDB(actorCnt).var(varCnt)
              With tDB(actorCnt).var(varCnt)

                Int32.TryParse(nextTokens(2), .var)

                .desc = ""
                If .var > - 1 And nextTokens(3) = "=" Then
                  For I As Integer = 4 To nextTokens.Length - 1
                    .desc += nextTokens(I) & " "
                  Next
                End If
              End With
              varCnt += 1
              nextTokens = tReader.ReadLine.Split(" ")
              If nextTokens.Length <= 1 Then
                Exit While
              End If
            End While
          End If
          varCnt = 0
        Else
          GoTo readMain
        End If
        actorCnt += 1
      End While
      Return tDB
    End If
  End Function

  Private Sub PopulateVarContext(ByVal DB() As ActorDB, ByVal Actor As Integer)
    For I As Integer = 0 To DB(Actor).var.Length - 1
      VarContextMenu.Items.Add(DB(Actor).var(I).desc)
    Next
  End Sub

  Private Sub SwitchGame(ByVal game As Integer)
    Dim curline As String = ""

    Select Case game
      Case 0
        ExtraDataPrefix = "\ext\OoT"
      Case 1
        ExtraDataPrefix = "\ext\MM"
    End Select

    If File.Exists(AppDirectory & ExtraDataPrefix & "\oot_actors_human.txt") Then
      ActorDataBase = ReadActorDBHuman(AppDirectory & ExtraDataPrefix & "\oot_actors_human.txt")
    Else
      ReDim ActorDataBase(- 1)
    End If

    ActorDBGroups.Clear()
    ActorDBNumber.Clear()
    ActorDBVars.Clear()
    ActorDBDesc.Clear()

    Objects.Clear()
    ObjectsDesc.Clear()
    If File.Exists(AppDirectory & ExtraDataPrefix & "\objlist.txt") Then
      Dim objfile As New StreamReader(AppDirectory & ExtraDataPrefix & "\objlist.txt")
      While objfile.Peek <> - 1
        curline = objfile.ReadLine
        Objects.Add(Mid(curline, 1, 4))
        ObjectsDesc.Add(Mid(curline, 28))
      End While
      objfile.Dispose()
    End If
  End Sub

  Private Sub ScanCollisionPresets()
    ReDim ColPresets(- 1)
    Dim curline As String = ""
    If File.Exists(AppDirectory & ExtraDataPrefix & "\CollisionPresets.txt") Then
      Dim presLines(2) As String
      Dim colpres As StreamReader = New StreamReader(AppDirectory & ExtraDataPrefix & "\CollisionPresets.txt")
      Dim endpos As Integer = 0
      Dim no As Integer = 0
      Dim type As String = ""
      While colpres.Peek <> - 1
        curline = colpres.ReadLine
        presLines = curline.Split(ControlChars.Tab)
        If presLines(0) = "g" Then
          type = presLines(1)
          Do
            curline = colpres.ReadLine
            If curline = "" Then
              Exit Do
            End If
            presLines = curline.Split(ControlChars.Tab)
            ReDim Preserve ColPresets(no)
            With ColPresets(no)
              .Type = type
              .Data = presLines(0)
              .Description = presLines(1)
              .Index = presLines(2)
              Select Case .Index
                Case 1
                  ColVar1.AutoCompleteCustomSource.Add(.Data & " - " & .Description)
                Case 2
                  ColVar2.AutoCompleteCustomSource.Add(.Data & " - " & .Description)
                Case 3
                  ColVar3.AutoCompleteCustomSource.Add(.Data & " - " & .Description)
                Case 4
                  ColVar4.AutoCompleteCustomSource.Add(.Data & " - " & .Description)
                Case 5
                  ColWalkSound.AutoCompleteCustomSource.Add(.Data & " - " & .Description)
              End Select
            End With

            no += 1
          Loop
          Exit While
        ElseIf presLines(0) = "" Then

        End If
      End While
      colpres.Dispose()
    End If
  End Sub

  Private Sub ProcessSceneHeader()
    Try
      Dim mscenePos As Integer = 0
      Dim scenePos As Integer = 0
      Dim scActorPos As Integer = 0
      Dim scRoomCnt As Integer = 0
      Dim scRoomPos As Integer = 0
      ReDim CollisionTriColor(-1)
      ReDim SceneActors(-1)
      ReDim ColTypes(-1)
      ReDim SceneExits(-1)
      ReDim CollisionPolies(-1)

      scActorCount = 0

      With CollisionVerts
        .x.Clear()
        .y.Clear()
        .z.Clear()
        .VertR.Clear()
        .VertG.Clear()
        .VertB.Clear()

        .FaceR.Clear()
        .FaceG.Clear()
        .FaceB.Clear()

        .EdgeR.Clear()
        .EdgeG.Clear()
        .EdgeB.Clear()
      End With

      ColVar1.AutoCompleteCustomSource.Clear()
      ColVar2.AutoCompleteCustomSource.Clear()
      ColVar3.AutoCompleteCustomSource.Clear()
      ColVar4.AutoCompleteCustomSource.Clear()
      ColWalkSound.AutoCompleteCustomSource.Clear()

      ExitCombobox.Items.Clear()
      ExitCombobox.Items.Add("Exit Selection")
      ExitCombobox.SelectedIndex = 0
      ExitCombobox.Enabled = False

      ColTypeBox.Items.Clear()
      ColTypeBox.Items.Add("Collision Types")
      ColTypeBox.SelectedIndex = 0
      ColTypeBox.Enabled = False

      SceneActorCombobox.Items.Clear()
      SceneActorCombobox.Items.Add("None selected")
      SceneActorCombobox.SelectedIndex = 0
      SceneActorCombobox.Enabled = False

      ColTriangleBox.Items.Clear()
      ColTriangleBox.Items.Add("Triangles")
      ColTriangleBox.SelectedIndex = 0
      ColTriangleBox.Enabled = False

      If LoadedDataType = FileTypes.MAP Then
        Dim colVar1s As ISet(Of UInteger) = New HashSet(Of UInteger)
        Dim colVar2s As ISet(Of UInteger) = New HashSet(Of UInteger)
        Dim colVar3s As ISet(Of UInteger) = New HashSet(Of UInteger)
        Dim colVar4s As ISet(Of UInteger) = New HashSet(Of UInteger)
        Dim colWalkSounds As ISet(Of Byte) = New HashSet(Of Byte)

        While scenePos < RamBanks.ZSceneBuffer.Count
          mscenePos = scenePos
          Select Case RamBanks.ZSceneBuffer(mscenePos)
            Case &HE
              SceneActorCombobox.Enabled = True
              scActorCount = RamBanks.ZSceneBuffer(mscenePos + 1)
              scActorPos = IoUtil.ReadUInt24(RamBanks.ZSceneBuffer, mscenePos + 5)
              i1 = scActorPos
              ReDim SceneActors(scActorCount - 1)
              For i As Integer = 0 To scActorCount - 1
                With SceneActors(i)
                  .pickR = Rand.Next(0, 255)
                  .pickG = Rand.Next(0, 255)
                  .pickB = Rand.Next(0, 255)

                  .loadMapFront = RamBanks.ZSceneBuffer(i1)

                  .loadMapBack = RamBanks.ZSceneBuffer(i1 + 2)

                  .offset = i1

                  .no = (RamBanks.ZSceneBuffer(i1 + 4) * &H100) + (RamBanks.ZSceneBuffer(i1 + 5))

                  .x = (RamBanks.ZSceneBuffer(i1 + 6) * &H100) + (RamBanks.ZSceneBuffer(i1 + 7))
                  .y = (RamBanks.ZSceneBuffer(i1 + 8) * &H100) + (RamBanks.ZSceneBuffer(i1 + 9))
                  .z = (RamBanks.ZSceneBuffer(i1 + 10) * &H100) + (RamBanks.ZSceneBuffer(i1 + 11))

                  .yr = (RamBanks.ZSceneBuffer(i1 + 12) * &H100) + (RamBanks.ZSceneBuffer(i1 + 13))
                  .var = (RamBanks.ZSceneBuffer(i1 + 14) * &H100) + (RamBanks.ZSceneBuffer(i1 + 15))
                End With
                i1 += 16

                RoomActorCombobox.Items.Add("Scene Actor #" & i.ToString & " - " & IdentifyActor(0, i))
              Next
              scenePos = mscenePos + 8
            Case 4
              Dim cnt As Integer = RamBanks.ZSceneBuffer(scenePos + 1)
              Dim pos As Integer = IoUtil.ReadUInt24(RamBanks.ZSceneBuffer, scenePos + 5)
              Dim i1 As Integer = pos
              Dim curmap As UInt32 = 0
              ReDim SceneMaps(cnt - 1)
              For i As Integer = 0 To cnt - 1
                curmap = IoUtil.ReadUInt32(RamBanks.ZSceneBuffer, i1)
                SceneMaps(i).StartOff = curmap
                curmap = IoUtil.ReadUInt32(RamBanks.ZSceneBuffer, i1 + 4)
                SceneMaps(i).EndOff = curmap
                i1 += 8
              Next
              scenePos = mscenePos + 8
            Case 20
              Exit While
            Case 19
              ExitCombobox.Enabled = True
              ExitTextBox.Enabled = True
              Dim ExitOffset As Integer = IoUtil.ReadUInt24(RamBanks.ZSceneBuffer, scenePos + 5)
              Dim ExitCount As Integer = RamBanks.ZSceneBuffer(scenePos + 8)
              ReDim Preserve SceneExits(ExitCount - 1)
              For i As Integer = 0 To ExitCount - 1
                With SceneExits(i)
                  .Index = IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, ExitOffset)
                  .scOff = ExitOffset
                End With
                ExitCombobox.Items.Add(i)
                ExitOffset += 2
              Next
              Exit While
            Case 3
              ColTypeBox.Enabled = True

              Dim colPtr As UInteger = IoUtil.ReadUInt24(RamBanks.ZSceneBuffer, scenePos + 5) + 12

              Dim VariableOffset As UInteger = IoUtil.ReadUInt24(RamBanks.ZSceneBuffer, colPtr + 17)
              Dim PolygonOffset As UInteger = IoUtil.ReadUInt24(RamBanks.ZSceneBuffer, colPtr + 13)
              Dim VertexOffset As UInteger = IoUtil.ReadUInt24(RamBanks.ZSceneBuffer, colPtr + 5)

              Dim ctCnt As Integer = 0

              While VariableOffset < PolygonOffset

                ReDim Preserve ColTypes(ctCnt)
                With ColTypes(ctCnt)
                  .scOff = VariableOffset
                  .unk1 = IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, VariableOffset)
                  .unk2 = IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, VariableOffset + 2)
                  .unk3 = IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, VariableOffset + 4)
                  .unk4 = IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, VariableOffset + 6) >> 4
                  .WalkOnSound = RamBanks.ZSceneBuffer(VariableOffset + 7) << 4 >> 4

                  colVar1s.Add(.unk1)
                  colVar2s.Add(.unk2)
                  colVar3s.Add(.unk3)
                  colVar4s.Add(.unk4)
                  colWalkSounds.Add(.WalkOnSound)
                End With

                ColTypeBox.Items.Add(ctCnt)
                VariableOffset += 8
                ctCnt += 1
              End While
              ScanCollisionPresets()
              ColTypeBox.SelectedIndex = 0
              zlestart16 = PolygonOffset
              zlestart6 = VertexOffset
              zleend6 = colPtr
              ColTriangleBox.Enabled = True
              Dim triCount As Integer = 0
              While PolygonOffset < VertexOffset
                ReDim Preserve CollisionPolies(triCount)
                With CollisionPolies(triCount)
                  .scOff = PolygonOffset
                  .Param = IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, PolygonOffset)

                  .A = (IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, PolygonOffset + 2) And &HFFF)
                  .B = (IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, PolygonOffset + 4) And &HFFF)
                  .C = (IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, PolygonOffset + 6) And &HFFF)

                  .nX = (IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, PolygonOffset + 8))
                  .nY = (IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, PolygonOffset + 10))
                  .nZ = (IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, PolygonOffset + 12))

                  .pickR = Rand.Next(0, 255)
                  .pickG = Rand.Next(0, 255)
                  .pickB = Rand.Next(0, 255)
                End With
                PolygonOffset += 16
                triCount += 1
                ColTriangleBox.Items.Add(triCount)
              End While

              ReDim CollisionTriColor(triCount)
              Dim edgecnt As Integer = -1

              While VertexOffset < colPtr

                CollisionVerts.x.Add(CShort(IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, VertexOffset)))

                CollisionVerts.y.Add(CShort(IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, VertexOffset + 2)))

                CollisionVerts.z.Add(CShort(IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, VertexOffset + 4)))

                CollisionVerts.VertR.Add(Rand.Next(0, 255))
                CollisionVerts.VertG.Add(Rand.Next(0, 255))
                CollisionVerts.VertB.Add(Rand.Next(0, 255))
                VertexOffset += 6
                edgecnt += 1
                If edgecnt = 2 Then
                  CollisionVerts.EdgeR.Add(Rand.Next(0, 255))
                  CollisionVerts.EdgeG.Add(Rand.Next(0, 255))
                  CollisionVerts.EdgeB.Add(Rand.Next(0, 255))
                  edgecnt = 0
                End If
              End While
              RoomActorCombobox.SelectedIndex = 0
              scenePos += 8
              mscenePos += 8
              ChangeColor()
            Case Else
              mscenePos += 8
              scenePos += 8
          End Select
        End While

        For Each unk1 As UInteger In colVar1s
          ColVar1.AutoCompleteCustomSource.Add(unk1.ToString("X4"))
        Next
        For Each unk2 As UInteger In colVar2s
          ColVar2.AutoCompleteCustomSource.Add(unk2.ToString("X4"))
        Next
        For Each unk3 As UInteger In colVar3s
          ColVar3.AutoCompleteCustomSource.Add(unk3.ToString("X4"))
        Next
        For Each unk4 As UInteger In colVar4s
          ColVar4.AutoCompleteCustomSource.Add(unk4.ToString("X4"))
        Next
        For Each walkSound As UInteger In colWalkSounds
          ColWalkSound.AutoCompleteCustomSource.Add(walkSound.ToString("X0"))
        Next
      End If
    Catch err As Exception
      If HandleErrors Then
        MsgBox("Error parsing scene header: " & Environment.NewLine & Environment.NewLine & "Details: " & err.Message)
      Else
        Throw
      End If
    End Try
  End Sub

  Private Function FindAllDLs(buffer As IBank)
    Dim DLCnt As Integer = 0
    For i As Integer = 0 To buffer.Count - 8 Step 8
      If buffer(i) = &HE7 And buffer(i + 1) = 0 And buffer(i + 2) = 0 _
         And buffer(i + 3) = 0 And buffer(i + 4) = 0 And buffer(i + 5) = 0 _
         And buffer(i + 6) = 0 And buffer(i + 7) = 0 Then

        Dim address As UInteger = IoUtil.MergeAddress(buffer.Segment, i)
        i = DisplayListReader.ReadInDL(DlManager, address, DListSelection)
        DLCnt += 1
      End If
    Next
  End Function

  Private Sub GetEntryPoints()
    Try
      Dim DLCnt As Integer = 0
      Dim FileTreeIndex As Integer = 0
      DlManager.Clear()
      ReDim LimbEntries(-1)

      DListSelection.Items.Clear()
      DListSelection.Items.Add("Render all")

      Select Case LoadedDataType
        Case FileTypes.MAP
          FindAllDLs(RamBanks.ZFileBuffer)
          DlManager.HasLimbs = False
        Case FileTypes.ACTORMODEL
          ' TODO: Determine if model is link to auto-select animations.
          LimbEntries = LimbHierarchyReader.GetHierarchies(
            RamBanks.ZFileBuffer,
            False,
            DlManager,
            DlModel,
            DListSelection)
          If LimbEntries IsNot Nothing Then
            DlManager.HasLimbs = True
            DLParser.LimbMatrices.Retarget(LimbEntries)
            DlModel.LimbMatrices.Retarget(DlModel.Limbs)
          Else
            DlManager.HasLimbs = False
            FindAllDLs(RamBanks.ZFileBuffer)
          End If
      End Select

      animationTab_.AnimationBanks.Reset(ObjectFilename, LimbEntries)
    Catch err As System.Exception
      If HandleErrors Then
        MsgBox(
          "Error in entry point searching: " & Environment.NewLine & Environment.NewLine & "Debug Info: " & err.Message)
      Else
        Throw
      End If
      Exit Sub
    End Try
  End Sub

  Private Function IdentifyActor(ByVal ActorType As UInteger, ByVal Actor As Integer) As String
    If ActorType = 0 Then
      For I As Integer = 0 To ActorDataBase.Length - 1
        With ActorDataBase(I)
          If .no = RoomActors(Actor).no Then
            Return ActorDataBase(I).desc
          End If
        End With
      Next
    Else
      For I As Integer = 0 To ActorDataBase.Length - 1
        With ActorDataBase(I)
          If .no = SceneActors(Actor).no And
             (.grp = 1 Or .grp = 0) Then
            Return ActorDataBase(I).desc
          End If
        End With
      Next
    End If
    Return "?"
  End Function

  Private Sub ProcessMapHeader()
    Try
      Dim HDPos As Integer = objectset
      Dim Identity As String = ""
      Dim GROff As Integer = 0
      Dim CurGr As Integer = 0
      Dim ActorStart As Integer = 0
      Dim ObjSetCnt As Integer = 0
      Dim ObjSetStart As Integer = 0
      Dim CurObjSet As Integer = 0

      ReDim RoomActors(- 1)
      ReDim UsedGroupIndex(- 1)

      ComboBox6.SelectedIndex = 0
      rmActorCount = 0
      ActorVarText.Text = ""
      ActorVarText.Enabled = False
      ActorGroupText.Text = "0001"
      ActorGroupText.Enabled = False
      ActorNumberText.Text = ""
      ActorNumberText.Enabled = False

      RoomActorCombobox.Items.Clear()
      RoomActorCombobox.Items.Add("None selected")
      RoomActorCombobox.SelectedIndex = 0
      RoomActorCombobox.Enabled = False

      While RamBanks.ZFileBuffer(HDPos) <> &H14
        Select Case RamBanks.ZFileBuffer(HDPos)
          Case 1
            rmActorCount = RamBanks.ZFileBuffer(HDPos + 1)
            ActorPointer(0) = HDPos
            ActorPointer(1) = rmActorCount
            If rmActorCount > 0 Then
              RoomActorCombobox.Enabled = True
              ActorVarText.Enabled = True
              ActorNumberText.Enabled = True
              ActorStart = IoUtil.ReadUInt24(RamBanks.ZFileBuffer, HDPos + 5)
              ActorPointer(2) = ActorStart
              i1 = ActorStart
              ReDim RoomActors(rmActorCount - 1)
              ReDim UsedGroupIndex(rmActorCount - 1)
              For i As Integer = 0 To rmActorCount - 1
                With RoomActors(i)
                  .pickR = Rand.Next(0, 255)
                  .pickG = Rand.Next(0, 255)
                  .pickB = Rand.Next(0, 255)

                  .offset = i1

                  .no = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, i1 + 0)

                  .x = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, i1 + 2)
                  .y = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, i1 + 4)
                  .z = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, i1 + 6)

                  .xr = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, i1 + 8)
                  .yr = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, i1 + 10)
                  .zr = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, i1 + 12)

                  .var = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, i1 + 14)

                End With
                i1 += 16
                RoomActorCombobox.Items.Add("Room Actor #" & i.ToString & " - " & IdentifyActor(0, i))
              Next
            End If
          Case &HB
            ActorGroupText.Enabled = True
            Dim Cnt As UInteger = RamBanks.ZFileBuffer(HDPos + 1)
            ActorGroupOffset = IoUtil.ReadUInt24(RamBanks.ZFileBuffer, HDPos + 5)
            CurGr = ActorGroupOffset
            Dim gr As UInteger = 0
            Dim desc As String = "?"
            Dim objind As Integer = 0
            For i As Integer = 0 To Cnt - 1
              gr = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, CurGr)
              ActorGroups.Add(gr)
              objind = Objects.IndexOf(gr.ToString("X4"))
              desc = "?"
              If objind > - 1 Then
                desc = ObjectsDesc(objind)
              End If
              'Group.Items.Add((i + 1).ToString & " - " & desc)
              CurGr += 2
            Next
          Case &H12
            'environment stuff
          Case &H18 And Not ScannedObjSet
            ' TODO: Support object sets.
            If IndMapFileName = "" Then
              'FileTree.SelectedNode.Nodes.Add("Object Sets")
              'FileTree.SelectedNode.Nodes(0).Nodes.Add("1. 0x0")
            End If
            ObjSetStart = IoUtil.ReadUInt24(RamBanks.ZFileBuffer, HDPos + 5)
            ObjSetCnt = RamBanks.ZFileBuffer(HDPos + 15)
            CurObjSet = ObjSetStart
            Dim ObjSetOffset As UInteger = 0
            Dim ObjSetSeg As UInteger = 3
            Dim ObjSetIncr As UInteger = 0
            For i As Integer = 0 To ObjSetCnt - 1
              ObjSetOffset = IoUtil.ReadUInt24(RamBanks.ZFileBuffer, CurObjSet + 1)
              ObjSetSeg = RamBanks.ZFileBuffer(CurObjSet)

              If ObjSetSeg <> &H3 And ObjSetOffset > 0 Then
                Exit For
              End If

              If IndMapFileName = "" And ObjSetOffset > 0 And RamBanks.ZFileBuffer(ObjSetOffset) = &H16 Then
                'FileTree.SelectedNode.Nodes(0).Nodes.Add((ObjSetIncr + 2).ToString & ". 0x" & Hex(ObjSetOffset))
                ObjSetIncr += 1
              End If
              CurObjSet += 4
            Next
        End Select
        HDPos += 8
      End While

    Catch err As Exception
      rmActorCount = 0
      MsgBox("Error in grabbing actors: " & Environment.NewLine & Environment.NewLine & "Debug Info: " & err.Message)
      Exit Sub
    End Try
  End Sub

  Private Sub LoadActorGFX_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles LoadROM.FileOk
    DefROM = LoadROM.FileName
    UoTIniFile.WriteString("Settings", "DefaultROM", DefROM)
    Start(False)
  End Sub

  Public Sub PopulateCommonBanks()
    RamBanks.PopulateFromRomFiles(ROMFiles)
    banks.AnimationBanks.PopulateFromRomFiles(ROMFiles)
    animationTab_.AnimationBanks.Populate(banks.AnimationBanks.Banks)
  End Sub

  Public Sub Start(ByVal individual As Boolean)
    'Try
    DlManager.Clear()
    DLParser.KillTexCache()
    Working = True
    If Not individual Then
      Dim romBytes() As Byte = ZFiles.LoadRomBytes(LoadROM.FileName)
      Dim romMemory As IShardedMemory = ShardedMemory.From(romBytes)

      Dim ROMID As String = ""
      Dim ROMIDBytes(5) As Byte
      Dim BuildDate As String = ""
      Dim BuildDateBytes(16) As Byte
      Dim tSegOff As Integer = 0
      Dim tNameOff As Integer = 0
      Dim ROMType As String = ""
      For i As Integer = 0 To romBytes.Length - 1 Step 16
        For i1 As Integer = 0 To 5
          ROMIDBytes(i1) = romBytes(i + i1)
        Next
        ROMID = System.Text.Encoding.ASCII.GetString(ROMIDBytes)
        If ROMID = "zelda@" Then
          i += &HD
          If romBytes(i) >> 4 <> 3 Then
            Do Until romBytes(i) >> 4 = 3
              i += 1
            Loop
          End If

          For i1 As Integer = 0 To 16
            BuildDateBytes(i1) = romBytes(i + i1)
          Next
          BuildDate = System.Text.Encoding.ASCII.GetString(BuildDateBytes)

          tSegOff = i + &H20

          Dim CodeOff As Integer = 0
          ReDim Z64Code(- 1)
          Select Case BuildDate
            Case "00-07-31 17:04:16"
              tNameOff = - 1
              ROMType = "Majora's Mask (U)"
              SwitchGame(1)
            Case "03-02-21 00:16:31"
              tNameOff = &HBE80
              ROMType = "Master Quest Debug ROM (E)"
              CodeOff = &HA94000
              ReDim Z64Code(&H13AF2F)
              For ii As Integer = 0 To &H13AF30 - 1
                Z64Code(ii) = romBytes(CodeOff + ii)
              Next
              ParseActorTable(&HF9440, &HF5BE0, &H10A6D0, &H10B360)
              SwitchGame(0)
            Case Else
              MsgBox("ROM not recognized, build date: " & BuildDate)
              ExtraDataPrefix = "\ext"
              Working = False
              Exit Sub
          End Select
          Exit For
        End If
      Next

      ROMFiles = ZFiles.GetFiles(romMemory, tSegOff, tNameOff)
      PopulateCommonBanks()
      Reshape()
      zFileTreeView_.Populate(ROMFiles)

      Me.Text = "Utility of Time R8 - " & LoadROM.FileName & " - " & ROMType
      IndMapFileName = ""
      IndScFileName = ""
    Else
      Me.Text = "Utility of Time R8 - " & IndScFileName
      If IndScFileName.Contains(".zscene") Then
        SetVariables(SceneFileType.ZSCENE)
      ElseIf IndScFileName.Contains(".zobj") Then
        SetVariables(SceneFileType.ZOBJ)
      End If
    End If
    'Catch err As Exception
    '  MsgBox("Error reading file: " & err.Message, MsgBoxStyle.Critical, "Error")
    'End Try
  End Sub

  Private Sub ParseActorTable(ByVal ActorTableOff As UInteger, ByVal ActorTableEnd As UInteger,
                              ByVal ObjectTableOff As UInteger, ByVal ObjectTableEnd As UInteger)
    Dim tActorTbl As UInteger = ActorTableOff
    Dim tActorTblEnd As UInteger = ActorTableEnd
    Dim tObjectTbl As UInteger = ObjectTableOff
    Dim tObjectTblEnd As UInteger = ObjectTableEnd
    Dim objtbcnt As Integer = 0
    While tObjectTbl < tObjectTblEnd
      ReDim Preserve ObjectTable(objtbcnt)
      ObjectTable(objtbcnt).Startoff = IoUtil.ReadUInt32(Z64Code, tObjectTbl)
      ObjectTable(objtbcnt).Endoff = IoUtil.ReadUInt32(Z64Code, tObjectTbl + 4)
      objtbcnt += 1
      tObjectTbl += 8
    End While
  End Sub

  Public Sub Reshape()
    winw = UoTRender.Width
    winh = UoTRender.Height
    ogltop = UoTRender.Top
    oglleft = UoTRender.Left
    Gl.glViewport(0, 0, UoTRender.Width, UoTRender.Height)
    Gl.glMatrixMode(Gl.GL_PROJECTION)
    Gl.glLoadIdentity()
    Glu.gluPerspective(45.0, UoTRender.Width/UoTRender.Height, GlConstants.NEAR, GlConstants.FAR)
    Gl.glMatrixMode(Gl.GL_MODELVIEW)
    Gl.glLoadIdentity()
  End Sub

  Private Sub Form1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Enter
    Me.Focus()
  End Sub

  Private Sub KeyCheckUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
    Select Case e.KeyCode
      Case Keys.Alt
        key_alt = False
      Case Keys.ControlKey
        key_ctrl = False
      Case Keys.I, Keys.J, Keys.K, Keys.L, Keys.U, Keys.O, Keys.T, Keys.Y, Keys.G, Keys.H
        ActorInputTimer.Stop()
      Case Keys.W
        key_w = False
      Case Keys.D
        key_d = False
      Case Keys.S
        key_s = False
      Case Keys.A
        key_a = False
      Case Keys.O
        key_o = False
      Case Keys.U
        key_u = False
      Case Keys.Q
        key_q = False
      Case Keys.E
        key_e = False
    End Select
  End Sub

  Private Sub SimpleOpenGlControl1_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) _
    Handles UoTRender.KeyUp, UoTRender.KeyUp
    KeyCheckUp(sender, e)
  End Sub

  Private Sub PickItem(ByVal CurrentTool As Integer, ByVal Button As Windows.Forms.MouseButtons)
    Gl.glPushMatrix()

    Gl.glRotated(cam.Pitch, 1.0F, 0.0F, 0.0F)
    Gl.glRotated(cam.Yaw, 0.0F, 1.0F, 0.0F)
    Gl.glTranslated(cam.X, cam.Y, cam.Z)

    Select Case CurrentTool
      Case ToolID.ACTOR
        If LoadedDataType = FileTypes.MAP Then
          DrawActorBoxes(True)
          ReadPixel = MousePixelRead(NewMouseX, NewMouseY)
          For g As Integer = 0 To RoomActors.Length - 1
            If _
              ReadPixel(0) = RoomActors(g).pickR And ReadPixel(1) = RoomActors(g).pickG And
              ReadPixel(2) = RoomActors(g).pickB Then
              If Not MouseOver Then
                If Button = Windows.Forms.MouseButtons.Right And SelectedRoomActors.Count > 0 Then
                  ActorContextMenu.Show(MousePosition.X, MousePosition.Y)
                Else
                  SceneActorCombobox.SelectedIndex = 0
                  SelectedSceneActors.Clear()
                  If Not key_ctrl Then
                    SelectedRoomActors.Clear()
                    SelectedRoomActors.Add(g)
                    'SyncCameraToActor(0, g)
                    RoomActorCombobox.SelectedIndex = g + 1
                  Else
                    MPick = True
                    If Not SelectedRoomActors.Contains(g) Then
                      SelectedRoomActors.Add(g)
                    End If
                    RoomActorCombobox.SelectedIndex = 0
                  End If
                  ToolModes.SelectedItemType = ToolID.ACTOR
                  UpdateActorPos()
                End If
                EditingTabs.SelectedTab = EditingTabs.TabPages("ActorsTab")
              Else
                PrintTool = True
                PrintToolStr = RoomActorCombobox.Items(g + 1).ToString
              End If
              Exit Select
            End If
          Next
          For g As Integer = 0 To SceneActors.Length - 1
            If _
              ReadPixel(0) = SceneActors(g).pickR And ReadPixel(1) = SceneActors(g).pickG And
              ReadPixel(2) = SceneActors(g).pickB Then
              If Not MouseOver Then
                If Button = Windows.Forms.MouseButtons.Right And SelectedSceneActors.Count > 0 Then
                  ActorContextMenu.Show(MousePosition.X, MousePosition.Y)
                Else
                  MPick = True
                  RoomActorCombobox.SelectedIndex = 0
                  SelectedRoomActors.Clear()

                  If Not key_ctrl Then
                    SelectedSceneActors.Clear()
                    SelectedSceneActors.Add(g)
                    'SyncCameraToActor(1, g)
                    SceneActorCombobox.SelectedIndex = g + 1
                  Else
                    If Not SelectedSceneActors.Contains(g) Then
                      SelectedSceneActors.Add(g)
                    End If
                    SceneActorCombobox.SelectedIndex = 0
                  End If
                  ToolModes.SelectedItemType = ToolID.ACTOR
                  UpdateActorPos()
                End If
                EditingTabs.SelectedTab = EditingTabs.TabPages("ActorsTab")
              Else
                PrintTool = True
                PrintToolStr = SceneActorCombobox.Items(g + 1).ToString
              End If
              Exit Select
            End If
          Next
        End If
        PrintTool = False
      Case ToolID.VERTEX
        Gl.glEnable(Gl.GL_POLYGON_OFFSET_POINT)
        Gl.glPolygonOffset(- 6, - 6)
        If RenderGraphics Then
          DrawDLArray(ToolID.VERTEX)
        End If
        If RenderCollision Then
          DrawCollision(CollisionPolies, CollisionVerts, True)
        End If
        ReadPixel = MousePixelRead(NewMouseX, NewMouseY)
        If Not MouseOver Then
          VertexSelect()
        End If
        Gl.glDisable(Gl.GL_POLYGON_OFFSET_POINT)
      Case ToolID.EDGE
        If Not MouseOver Then
          EdgeSelect()
        End If
      Case ToolID.DLIST
        Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
        Gl.glPolygonOffset(- 7, - 7)
        If RenderGraphics Then
          DrawDLArray(ToolID.DLIST)
        End If
        Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
      Case ToolID.COLTRI
        If RenderCollision Then
          DrawCollision(CollisionPolies, CollisionVerts, True)
          ReadPixel = MousePixelRead(NewMouseX, NewMouseY)
          For i1 = 0 To CollisionPolies.Length - 1
            If _
              ReadPixel(0) = CollisionPolies(i1).pickR And ReadPixel(1) = CollisionPolies(i1).pickG And
              ReadPixel(2) = CollisionPolies(i1).pickB Then
              If Not MouseOver Then
                Dim sel As Integer = CollisionPolies(i1).Param
                For i2 As Integer = 0 To CollisionTriColor.Length - 1
                  CollisionTriColor(i2).g = 1
                  CollisionTriColor(i2).b = 1
                Next
                CollisionTriColor(i1).g = 0
                CollisionTriColor(i1).b = 0
                colTri = True
                ColTriangleBox.SelectedIndex = i1 + 1
                colTri = False
                EditingTabs.SelectedTab = EditingTabs.TabPages("CollisionTab")
              Else
                PrintTool = True
                PrintToolStr = "Triangle " & ColTriangleBox.Items(i1 + 1)
              End If
              Exit Select
            End If
          Next
        End If
        PrintTool = False
    End Select

    Gl.glPopMatrix()
  End Sub

  Private Function MousePixelRead(ByVal x As Integer, ByVal y As Integer) As Byte()
    ReDim MousePixelRead(2)
    Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
    Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, MousePixelRead)
  End Function

  Private Sub EdgeSelect()
    For i As Integer = 0 To CollisionVerts.EdgeR.Count - 1
      If _
        ReadPixel(0) = CollisionVerts.EdgeR(i) And ReadPixel(1) = CollisionVerts.EdgeG(i) And
        ReadPixel(2) = CollisionVerts.EdgeB(i) Then
        MsgBox(i)
      End If
    Next
  End Sub

  Private Sub VertexSelect()
    If RenderCollision Then
      For g As Integer = 0 To CollisionVerts.VertR.Count - 1
        If _
          ReadPixel(0) = CollisionVerts.VertR(g) And ReadPixel(1) = CollisionVerts.VertG(g) And
          ReadPixel(2) = CollisionVerts.VertB(g) Then
          If key_ctrl Then
            If MouseLeft Then
              If SelectedCollisionVert.Contains(g) Then
                SelectedCollisionVert.Remove(g)
              End If
            End If
            If Not SelectedCollisionVert.Contains(g) Then
              SelectedCollisionVert.Add(g)
            End If
          Else
            If MouseRight Then
              VertContextMenu.Show(MousePosition.X, MousePosition.Y)
            End If
            If SelectedCollisionVert.Count > 1 Then
              SelectedCollisionVert.Clear()
              SelectedCollisionVert.Add(g)
            Else
              SelectedCollisionVert.Clear()
              SelectedCollisionVert.Add(g)
            End If
          End If
          ToolModes.SelectedItemType = ToolID.VERTEX
          Exit Sub
        End If
      Next
    End If
  End Sub

  Private Sub SimpleOpenGlControl1_MouseDown(ByVal sender As System.Object,
                                             ByVal e As System.Windows.Forms.MouseEventArgs) Handles UoTRender.MouseDown
    UoTRender.Cursor.Hide()
    MouseDown1(sender, e)
  End Sub

  Private Sub SimpleOpenGlControl1_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
    Handles UoTRender.MouseUp
    UoTRender.Cursor.Show()
    MouseUp1(sender, e)
  End Sub

  Private Sub MouseDown1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
    If Not HoldCursor Then
      HoldCursor = True
      CursorPosOld = New Point(MousePosition.X, MousePosition.Y)
      oldLocalMouse = UoTRender.PointToClient(CursorPosOld)
      OldMouseX = oldLocalMouse.X
      OldMouseY = oldLocalMouse.Y
      MouseOver = False
      If ToolModes.CurrentTool <> ToolID.NONE Then PickItem(ToolModes.CurrentTool, e.Button)
      Select Case e.Button
        Case MouseButtons.Left
          MouseLeft = True
        Case MouseButtons.Right
          MouseRight = True
        Case MouseButtons.Middle
          MouseMiddle = True
      End Select
    Else
      MouseLeft = False
      MouseRight = False
      MouseMiddle = False
    End If
  End Sub

  Private Sub MouseUp1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
    HoldCursor = False
    ToolModes.SelectedItemType = ToolID.NONE
    Select Case e.Button
      Case MouseButtons.Left
        MouseLeft = False
      Case MouseButtons.Right
        MouseRight = False
      Case MouseButtons.Middle
        MouseMiddle = False
    End Select
  End Sub

  Private Sub UpdateActorPos()
    If Not OnSceneActor Then
      If RoomActorCombobox.SelectedIndex > 0 Then
        Dim c7 As Integer = RoomActorCombobox.SelectedIndex - 1
        TextBox7.Text = CInt(RoomActors(c7).x)
        TextBox8.Text = CInt(RoomActors(c7).y)
        TextBox9.Text = CInt(RoomActors(c7).z)
        TextBox10.Text = CInt(RoomActors(c7).xr)
        TextBox11.Text = CInt(RoomActors(c7).yr)
        TextBox12.Text = CInt(RoomActors(c7).zr)
        TextBox10.Enabled = True
        TextBox11.Enabled = True
        TextBox12.Enabled = True
      End If
    Else
      If SceneActorCombobox.SelectedIndex > 0 Then
        Dim c5 As Integer = SceneActorCombobox.SelectedIndex - 1
        TextBox7.Text = CInt(SceneActors(c5).x)
        TextBox8.Text = CInt(SceneActors(c5).y)
        TextBox9.Text = CInt(SceneActors(c5).z)
        TextBox10.Enabled = False
        TextBox11.Enabled = False
        TextBox12.Enabled = False
      End If
    End If
    ChangePosition(0) = False
    ChangePosition(1) = False
    ChangePosition(2) = False
    ChangePosition(3) = False
    ChangePosition(4) = False
  End Sub

  Private Sub RoomActorCombobox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles RoomActorCombobox.SelectedIndexChanged, RoomActorCombobox.SelectedIndexChanged
    Try
      comb7 = RoomActorCombobox.SelectedIndex - 1
      If RoomActorCombobox.SelectedIndex = 0 Then
        ActorVarText.Text = ""
        ActorNumberText.Text = ""
        ActorType = 1
        TextBox7.Text = ""
        TextBox8.Text = ""
        TextBox9.Text = ""
        TextBox10.Text = ""
        TextBox11.Text = ""
        TextBox12.Text = ""
        ToolModes.SelectedItemType = ToolID.NONE
        VarContextMenu.Items.Clear()
        If Not MPick Then
          SelectedRoomActors.Clear()
        End If
      Else
        SceneActorCombobox.SelectedIndex = 0
        OnSceneActor = False
        ActorType = 0
        ActorNumberText.Enabled = True
        ActorVarText.Enabled = True
        ActorVarText.Text = RoomActors(comb7).var.ToString("X4")
        ActorNumberText.Text = RoomActors(comb7).no.ToString("X4")
        VarContextMenu.Items.Clear()
        For I As Integer = 0 To ActorDataBase.Length - 1
          With ActorDataBase(I)
            If .no = RoomActors(comb7).no And ActorGroups.Contains(.grp) Then
              PopulateVarContext(ActorDataBase, I)
              Exit For
            End If
          End With
        Next
        UpdateActorPos()
        If Not MPick Then
          SelectedRoomActors.Clear()
          SelectedRoomActors.Add(comb7)
        End If
      End If
    Catch err As Exception
      MsgBox("Error in room actor selection." & Environment.NewLine & Environment.NewLine & "Debug Info: " & err.Message,
             MsgBoxStyle.Critical)
      RoomActorCombobox.SelectedIndex = 0
      SceneActorCombobox.SelectedIndex = 0
      Exit Sub
    End Try
  End Sub

  Private Sub ComboBox5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles SceneActorCombobox.SelectedIndexChanged, SceneActorCombobox.SelectedIndexChanged
    Try
      comb5 = SceneActorCombobox.SelectedIndex - 1
      If SceneActorCombobox.SelectedIndex = 0 Then
        OnSceneActor = False
        ActorType = 1
        ToolModes.SelectedItemType = ToolID.NONE
        ActorVarText.Text = ""
        ActorNumberText.Text = ""
        TextBox7.Text = ""
        TextBox8.Text = ""
        TextBox9.Text = ""
        TextBox10.Text = ""
        TextBox11.Text = ""
        TextBox12.Text = ""
        If Not MPick Then
          SelectedSceneActors.Clear()
        End If
      Else
        RoomActorCombobox.SelectedIndex = 0
        OnSceneActor = True
        ActorType = 0
        TextBox10.Enabled = False
        TextBox11.Enabled = False
        TextBox12.Enabled = False
        TextBox7.Text = SceneActors(SceneActorCombobox.SelectedIndex - 1).x
        TextBox8.Text = SceneActors(SceneActorCombobox.SelectedIndex - 1).y
        TextBox9.Text = SceneActors(SceneActorCombobox.SelectedIndex - 1).z
        ActorNumberText.Text = SceneActors(SceneActorCombobox.SelectedIndex - 1).no.ToString("X4")
        ActorVarText.Text = SceneActors(SceneActorCombobox.SelectedIndex - 1).var.ToString("X4")
        ActorNumberText.Enabled = True
        ActorVarText.Enabled = True

        SelectedRoomActors.Clear()
        If Not MPick Then
          SelectedSceneActors.Clear()
          SelectedSceneActors.Add(SceneActorCombobox.SelectedIndex - 1)
        Else
          MPick = False
        End If
      End If
    Catch err As Exception
      MsgBox(
        "Error in scene actor selection processing." & Environment.NewLine & Environment.NewLine & "Debug Info: " &
        err.Message, MsgBoxStyle.Critical)
      SceneActorCombobox.SelectedIndex = 0
      RoomActorCombobox.SelectedIndex = 0
      Exit Sub
    End Try
  End Sub

  Public Sub UpdateActors()
    Try
      If OnSceneActor Then
        For i As Integer = 0 To SelectedSceneActors.Count - 1
          i1 = SelectedSceneActors(i)
          If ActorNumberText.Text.Length = 4 Then
            SceneActors(i1).no = Convert.ToUInt16(ActorNumberText.Text, 16)
          End If
          If ActorVarText.Text.Length = 4 Then
            SceneActors(i1).var = Convert.ToUInt16(ActorVarText.Text, 16)
          End If
          If IsNumeric(TextBox7.Text) And ChangePosition(0) Then SceneActors(i1).x = CShort(TextBox7.Text)
          If IsNumeric(TextBox8.Text) And ChangePosition(1) Then SceneActors(i1).y = CShort(TextBox8.Text)
          If IsNumeric(TextBox9.Text) And ChangePosition(2) Then SceneActors(i1).z = CShort(TextBox9.Text)
        Next
      Else
        For i As Integer = 0 To SelectedRoomActors.Count - 1
          i1 = SelectedRoomActors(i)
          If IsNumeric(TextBox7.Text) And ChangePosition(0) Then
            RoomActors(i1).x = CInt(TextBox7.Text)
          End If
          If IsNumeric(TextBox8.Text) And ChangePosition(1) Then
            RoomActors(i1).y = CInt(TextBox8.Text)
          End If
          If IsNumeric(TextBox9.Text) And ChangePosition(2) Then
            RoomActors(i1).z = CInt(TextBox9.Text)
          End If
          If IsNumeric(TextBox10.Text) And ChangePosition(3) Then
            RoomActors(i1).xr = CInt(TextBox10.Text)
          End If
          If IsNumeric(TextBox11.Text) And ChangePosition(4) Then
            RoomActors(i1).yr = CInt(TextBox11.Text)
          End If
          If IsNumeric(TextBox12.Text) And ChangePosition(5) Then
            RoomActors(i1).zr = CInt(TextBox12.Text)
          End If
          If ActorNumberText.Text.Length = 4 Then
            RoomActors(i1).no = Convert.ToUInt16(ActorNumberText.Text, 16)
          End If
          If ActorVarText.Text.Length = 4 Then
            RoomActors(i1).var = Convert.ToUInt16(ActorVarText.Text, 16)
          End If
        Next
      End If
    Catch err As Exception
    End Try
  End Sub

  Private Sub UpdateActorIdents()
    Dim oldint As Integer = 0
    Dim c7 As Integer = RoomActorCombobox.SelectedIndex
    Dim identity As String = ""
    If Not OnSceneActor Then
      oldint = c7
      RoomActorCombobox.Items.Clear()
      RoomActorCombobox.Items.Add("Room Actor Selection")
      For i As Integer = 0 To RoomActors.Length - 1
        RoomActorCombobox.Items.Add((i).ToString & " - " & IdentifyActor(0, i))
      Next
      If SelectedRoomActors.Count = 1 Then
        RoomActorCombobox.SelectedIndex = oldint
      End If
    Else
      oldint = SceneActorCombobox.SelectedIndex
      SceneActorCombobox.Items.Clear()
      SceneActorCombobox.Items.Add("None selected")
      For i As Integer = 0 To SceneActors.Length - 1
        SceneActorCombobox.Items.Add((i + 1).ToString & " - " & IdentifyActor(1, i))
      Next
      If SelectedSceneActors.Count = 1 Then
        SceneActorCombobox.SelectedIndex = oldint
      End If
    End If
  End Sub

  Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
    UpdateActors()
    UpdateActorIdents()
  End Sub

  Private Sub ScrollSensitivity(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
    Handles UoTRender.MouseWheel, UoTRender.MouseWheel
    Dim tAxis As Integer = ToolModes.Axis
    If e.Delta < 0 Then
      If tAxis > 0 Then
        tAxis -= 1
      Else
        tAxis = 2
      End If
    Else
      If tAxis < 2 Then
        tAxis += 1
      Else
        tAxis = 0
      End If
    End If

    LockAxes(tAxis)
  End Sub

  Private Sub CameraSensitivity(ByVal up As Boolean)
    If Not up Then
      If TrackBar1.Value > TrackBar1.Minimum Then
        TrackBar1.Value -= 1
      End If
    Else
      If TrackBar1.Value < TrackBar1.Maximum Then
        TrackBar1.Value += 1
      End If
    End If
    CameraCoef = TrackBar1.Value
  End Sub

  Private Sub MatchCollision(ByVal mm As Integer)
    Working = True
    Label28.Show()
    ProgressBar1.Maximum = CollisionVerts.x.Count
    For i As Integer = 0 To CollisionVerts.x.Count - 1
      ProgressBar1.Value += 1
      ProgressBar1.Refresh()
      'For i0 As Integer = 0 To Vertices.Length - 1
      '    For i1 = 0 To Vertices(i0).x.Count - 1
      '        If Vertices(i0).ox(i1) = CollisionVerts.ox(i) And Vertices(i0).oy(i1) = CollisionVerts.oy(i) And Vertices(i0).oz(i1) = CollisionVerts.oz(i) Then
      '            Select Case mm
      '                Case 0
      '                    CollisionVerts.x(i) = Vertices(i0).x(i1)
      '                    CollisionVerts.y(i) = Vertices(i0).y(i1)
      '                    CollisionVerts.z(i) = Vertices(i0).z(i1)
      '                Case 1
      '                    Vertices(i0).x(i1) = CollisionVerts.x(i)
      '                    Vertices(i0).y(i1) = CollisionVerts.y(i)
      '                    Vertices(i0).z(i1) = CollisionVerts.z(i)
      '            End Select
      '        End If
      '    Next
      'Next
    Next
    ProgressBar1.Value = 0
    Label28.Hide()
    Working = False
  End Sub

  Private Sub FilledToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles FilledToolStripMenuItem.Click
    wireframe = False
    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
  End Sub

  Private Sub WireframeToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles WireframeToolStripMenuItem.Click
    wireframe = True
    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
  End Sub

  Private Sub SimpleOpenGlControl1_MouseMove(ByVal sender As System.Object,
                                             ByVal e As System.Windows.Forms.MouseEventArgs) _
    Handles UoTRender.MouseMove, UoTRender.MouseMove
    oldLocalMouse = UoTRender.PointToClient(Windows.Forms.Cursor.Position)
  End Sub

  Public Sub ToggleTabs()
    If EditingTabs.Visible Then
      EditingTabs.Visible = False
      UoTRender.Width += 235
      UndoToolStripMenuItem.Checked = False
      Reshape()
    Else
      EditingTabs.Visible = True
      UoTRender.Width -= 235
      UndoToolStripMenuItem.Checked = True
      Reshape()
    End If
  End Sub

  Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ActorInputTimer.Tick, ActorInputTimer.Tick

    Dim camYRotD As Double = cam.Yaw/180*PI

    Dim cosYRot As Double = Cos(camYRotD)*ToolSensitivity
    Dim sinYRot As Double = Sin(camYRotD)*ToolSensitivity

    Select Case ToolModes.CurrentTool
      Case ToolID.ACTOR
        Select Case ButtonPress
          Case 0
            If OnSceneActor Then
              For i As Integer = 0 To SelectedSceneActors.Count - 1
                i1 = SelectedSceneActors(i)
                SceneActors(i1).x -= cosYRot
                SceneActors(i1).z -= sinYRot
              Next
            Else
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).x -= cosYRot
                RoomActors(SelectedRoomActors(i)).z -= sinYRot
              Next
            End If
          Case 1
            If OnSceneActor Then
              For i As Integer = 0 To SelectedSceneActors.Count - 1
                SceneActors(SelectedSceneActors(i)).y += ToolSensitivity
              Next
            Else
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).y += ToolSensitivity
              Next
            End If
          Case 2
            If OnSceneActor Then
              For i As Integer = 0 To SelectedSceneActors.Count - 1
                SceneActors(SelectedSceneActors(i)).y -= ToolSensitivity
              Next
            Else
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).y -= ToolSensitivity
              Next
            End If
          Case 3
            If OnSceneActor Then
              For i As Integer = 0 To SelectedSceneActors.Count - 1
                i1 = SelectedSceneActors(i)
                SceneActors(i1).x += cosYRot
                SceneActors(i1).z += sinYRot
              Next

            Else
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).x += cosYRot
                RoomActors(SelectedRoomActors(i)).z += sinYRot
              Next
            End If
          Case 4
            If OnSceneActor Then
              For i As Integer = 0 To SelectedSceneActors.Count - 1
                i1 = SelectedSceneActors(i)
                SceneActors(i1).x -= sinYRot
                SceneActors(i1).z += cosYRot
              Next
            Else
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).x -= cosYRot
                RoomActors(SelectedRoomActors(i)).z += sinYRot
              Next
            End If
          Case 5
            If OnSceneActor Then
              For i As Integer = 0 To SelectedSceneActors.Count - 1
                i1 = SelectedSceneActors(i)
                SceneActors(i1).x += sinYRot
                SceneActors(i1).z -= cosYRot
              Next
            Else
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).x += cosYRot
                RoomActors(SelectedRoomActors(i)).z -= sinYRot
              Next
            End If
        End Select
        UpdateActorPos()
      Case ToolID.VERTEX
        If SelectedCollisionVert.Count > 0 Then
          For i1 = 0 To SelectedCollisionVert.Count - 1
            Select Case ButtonPress
              Case 0
                CollisionVerts.x(SelectedCollisionVert(i1)) -= cosYRot
                CollisionVerts.z(SelectedCollisionVert(i1)) -= sinYRot
              Case 1
                CollisionVerts.y(SelectedCollisionVert(i1)) += ToolSensitivity
              Case 2
                CollisionVerts.y(SelectedCollisionVert(i1)) -= ToolSensitivity
              Case 3
                CollisionVerts.x(SelectedCollisionVert(i1)) += cosYRot
                CollisionVerts.z(SelectedCollisionVert(i1)) += sinYRot
              Case 4
                CollisionVerts.x(SelectedCollisionVert(i1)) -= sinYRot
                CollisionVerts.z(SelectedCollisionVert(i1)) += cosYRot
              Case 5
                CollisionVerts.x(SelectedCollisionVert(i1)) += sinYRot
                CollisionVerts.z(SelectedCollisionVert(i1)) -= cosYRot
            End Select
          Next
        End If
      Case ToolID.EDGE

      Case ToolID.FACE

    End Select
  End Sub

  Private Sub SetupToolStripMenuItem_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles SetupToolStripMenuItem.Click
    SetupDialog.ShowDialog()
  End Sub

  Private Sub CollisionMeshToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CollisionMeshToolStripMenuItem.Click
    If RenderCollision = False Then
      RenderCollision = True
      CollisionMeshToolStripMenuItem.Checked = True
    Else
      RenderCollision = False
      CollisionMeshToolStripMenuItem.Checked = False
    End If
  End Sub

  Private Sub MainWin_ResizeEnd(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
    If Me.Width < 800 Then
      Me.Width = 800
    End If
    If Me.Height < 600 Then
      Me.Height = 600
    End If
    Reshape()
  End Sub

  Private Sub ChangeColor()
    If ColTypeText.Text <> "" Then
      For i As Integer = 0 To CollisionPolies.Length - 1
        If CInt(ColTypeText.Text) = CollisionPolies(i).Param Then
          CollisionTriColor(i).g = 0
          CollisionTriColor(i).b = 0
        Else
          CollisionTriColor(i).g = 1
          CollisionTriColor(i).b = 1
        End If
      Next
    Else
      For i As Integer = 0 To CollisionPolies.Length - 1
        CollisionTriColor(i).g = 1
        CollisionTriColor(i).b = 1
      Next
    End If
  End Sub

  Private Sub ComboBox1_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ColTypeBox.SelectedIndexChanged, ColTypeBox.SelectedIndexChanged
    If ColTypeBox.SelectedIndex > 0 Then
      Dim SelParam As Integer = CInt(CollisionPolies(ColTypeBox.SelectedIndex - 1).Param)
      Dim v1 As String = ColTypes(SelParam).unk1.ToString("X4")
      Dim v2 As String = ColTypes(SelParam).unk2.ToString("X4")
      Dim v3 As String = ColTypes(SelParam).unk3.ToString("X4")
      Dim v4 As String = ColTypes(SelParam).unk4.ToString("X3")
      Dim v5 As String = ColTypes(SelParam).WalkOnSound.ToString("X0")
      ColTypeText.Text = ColTypeBox.SelectedIndex - 1
      ColVar1.Text = v1
      ColVar2.Text = v2
      ColVar3.Text = v3
      ColVar4.Text = v4
      ColWalkSound.Text = v5
      ColWalkSound.Enabled = True
      ColVar1.Enabled = True
      ColVar3.Enabled = True
      ColVar2.Enabled = True
      ColVar4.Enabled = True
      If Not colTri Then
        ChangeColor()
        colTri = False
      End If
    Else
      ColTypeText.Text = ""
      ColVar1.Text = ""
      ColVar3.Text = ""
      ColVar2.Text = ""
      ColVar4.Text = ""
      ColWalkSound.Text = ""
      ColWalkSound.Enabled = False
      ColVar1.Enabled = False
      ColVar3.Enabled = False
      ColVar2.Enabled = False
      ColVar4.Enabled = False
      ChangeColor()
    End If
  End Sub

  Private Sub Button22_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ApplyCollisionButton.Click, ApplyCollisionButton.Click
    If CollisionPolies.Length > 0 Then
      If ExitCombobox.SelectedIndex > 0 Then
        SceneExits(ExitCombobox.SelectedIndex - 1).Index = Convert.ToUInt16(ExitTextBox.Text, 16)
      End If
      ColTypes(ColTypeBox.SelectedIndex - 1).unk1 = Convert.ToUInt32(ColVar1.Text, 16)
      ColTypes(ColTypeBox.SelectedIndex - 1).unk2 = Convert.ToUInt32(ColVar2.Text, 16)
      ColTypes(ColTypeBox.SelectedIndex - 1).unk3 = Convert.ToUInt32(ColVar3.Text, 16)
      ColTypes(ColTypeBox.SelectedIndex - 1).unk4 = Convert.ToUInt32(ColVar4.Text, 16)
      ColTypes(ColTypeBox.SelectedIndex - 1).WalkOnSound = Convert.ToUInt32(ColWalkSound.Text, 16)
    End If
  End Sub

  Private Sub ComboBox8_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ExitCombobox.SelectedIndexChanged, ExitCombobox.SelectedIndexChanged
    If ExitCombobox.SelectedIndex > 0 Then
      ExitTextBox.Enabled = True
      ExitTextBox.Text = SceneExits(ExitCombobox.SelectedIndex - 1).Index.ToString("X4")
    Else
      ExitTextBox.Enabled = False
      ExitTextBox.Text = ""
    End If
  End Sub

  Private Sub ResetActors(ByVal all As Boolean)
    If Not OnSceneActor Then
      For i As Integer = 0 To SelectedRoomActors.Count - 1
        i1 = SelectedRoomActors(i)
        RoomActors(i1).x = RoomActors(i1).x
        RoomActors(i1).y = RoomActors(i1).y
        RoomActors(i1).z = RoomActors(i1).z
        RoomActors(i1).xr = RoomActors(i1).xr
        RoomActors(i1).yr = RoomActors(i1).yr
        RoomActors(i1).zr = RoomActors(i1).zr
      Next
    Else
      For i As Integer = 0 To SelectedSceneActors.Count - 1
        i1 = SelectedSceneActors(i)
        SceneActors(i1).x = SceneActors(i1).x
        SceneActors(i1).y = SceneActors(i1).y
        SceneActors(i1).z = SceneActors(i1).z
      Next
    End If
  End Sub

  Private Sub CheckBox13_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CheckBox13.CheckedChanged
    If HideActors(0) Then
      HideActors(0) = False
    Else
      HideActors(0) = True
    End If
  End Sub

  Private Sub CheckBox14_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CheckBox14.CheckedChanged
    If HideActors(1) Then
      HideActors(1) = False
    Else
      HideActors(1) = True
    End If
  End Sub

  Private Sub CheckBox15_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CheckBox15.CheckedChanged
    If HideActors(2) Then
      HideActors(2) = False
    Else
      HideActors(2) = True
    End If
  End Sub

  Private Sub TextBox7_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TextBox7.TextChanged
    ChangePosition(0) = True
  End Sub

  Private Sub TextBox8_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TextBox8.TextChanged
    ChangePosition(1) = True
  End Sub

  Private Sub TextBox9_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TextBox9.TextChanged
    ChangePosition(2) = True
  End Sub

  Private Sub TextBox10_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TextBox10.TextChanged
    ChangePosition(3) = True
  End Sub

  Private Sub TextBox11_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TextBox11.TextChanged
    ChangePosition(4) = True
  End Sub

  Private Sub TextBox12_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TextBox12.TextChanged
    ChangePosition(5) = True
  End Sub

  Private Sub CheckBox5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CheckBox5.CheckedChanged
    If HideActors(3) Then
      HideActors(3) = False
    Else
      HideActors(3) = True
    End If
  End Sub

  Private Sub ViewingMeshToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ViewingMeshToolStripMenuItem1.Click
    If RenderGraphics Then
      RenderGraphics = False
      ViewingMeshToolStripMenuItem1.Checked = False
    ElseIf Not RenderGraphics Then
      RenderGraphics = True
      ViewingMeshToolStripMenuItem1.Checked = True
    End If
  End Sub

  Private Sub SetVariables(ByVal ftype As SceneFileType)
    DLParser.Initialize()
    Select Case ftype
      Case SceneFileType.ZSCENE
        RamBanks.ZFileBuffer.Segment = &H3
        RenderGraphics = True
        RenderCollision = True
        LoadedDataType = FileTypes.MAP
        RenderModeToolStripMenuItem.Enabled = True
        ViewingMeshToolStripMenuItem1.Checked = True
        CollisionMeshToolStripMenuItem.Checked = True
        EditingTabs.TabPages.Remove(ActorsTab)
        EditingTabs.TabPages.Remove(CollisionTab)
        EditingTabs.TabPages.Remove(LevelFlagsTab)
        EditingTabs.TabPages.Remove(MiscTab)
        EditingTabs.TabPages.Add(ActorsTab)
        EditingTabs.TabPages.Add(CollisionTab)
        EditingTabs.TabPages.Add(LevelFlagsTab)
        EditingTabs.TabPages.Add(MiscTab)
        objectset = 0
        LoadedDataType = FileTypes.MAP
        ProcessMapHeader()
        ProcessSceneHeader()
        GetEntryPoints()
      Case SceneFileType.ZOBJ
        RamBanks.ZFileBuffer.Segment = &H6
        RenderGraphics = True
        RenderCollision = False
        SwitchTool(ToolID.CAMERA)
        LoadedDataType = FileTypes.ACTORMODEL
        EditingTabs.TabPages.Remove(ActorsTab)
        EditingTabs.TabPages.Remove(CollisionTab)
        EditingTabs.TabPages.Remove(LevelFlagsTab)
        EditingTabs.TabPages.Remove(MiscTab)
        GetEntryPoints()
    End Select
  End Sub

  Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ExitToolStripMenuItem.Click
    Me.Close()
  End Sub

  Private Sub OptionsToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles OptionsToolStripMenuItem1.Click
    SetupDialog.ShowDialog()
  End Sub

  Private Sub UndoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles UndoToolStripMenuItem.Click
    ToggleTabs()
  End Sub

  Private Sub ActorGroupText_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ActorGroupText.TextChanged
    If ActorGroupText.Text = "" Then ActorGroupText.Text = "0001"
  End Sub

  Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles SaveToolStripMenuItem.Click
    SaveData(LoadedDataType, False)
    SaveFiles(DefROM)
  End Sub

  Private Sub SaveData(ByVal DataType As Integer, ByVal SaveAs As Boolean)
    If LoadedDataType = FileTypes.MAP Then
      'If IndMapFileName = "" Then
      '    If MsgBox("Would you like to create backups of the current map and scene? (RECOMMENDED)" & Environment.NewLine & Environment.NewLine & _
      '              "You can restore previously created backups to your ROM by right clicking the file in the tree view, and clicking restore backup.", MsgBoxStyle.Question, "Create backups?") Then
      '        If Not Directory.Exists(AppDirectory & "\Backups") Then
      '            Directory.CreateDirectory(AppDirectory & "\Backups")
      '        End If
      '        If Not Directory.Exists(AppDirectory & "\Backups\" & DefROM) Then
      '            Directory.CreateDirectory(AppDirectory & "\Backups\" & DefROM)
      '        End If
      '        Dim backupmap As FileStream = File.Create(MapFileName & "_backup_" & Date.Now.ToShortTimeString.Replace(":", "").Replace(" ", "") & ".zmap")
      '        Dim backupsc As FileStream = File.Create(ScFileName & "_backup_" & Date.Now.ToShortTimeString.Replace(":", "").Replace(" ", "") & ".zscene")
      '        backupmap.Write(ZFileBuffer, 0, ZFileBuffer.Length - 1)
      '        backupsc.Write(ZSceneBuffer, 0, ZSceneBuffer.Length - 1)
      '        backupmap.Dispose()
      '        backupsc.Dispose()
      '    End If
      'End If

      'start saving to room file buffer...

      Dim DLStart As Integer = 0
      For Each displayList As N64DisplayList In DlManager
        DLStart = displayList.StartPos.Offset
        For Each instruction As IDisplayListInstruction In displayList.Commands
          RamBanks.ZFileBuffer(DLStart) = instruction.CMDParams(0)
          DLStart += 1
          IoUtil.WriteInt24(RamBanks.ZFileBuffer, instruction.Low, DLStart)
          IoUtil.WriteInt32(RamBanks.ZFileBuffer, instruction.High, DLStart)
        Next
      Next

      Dim AGrOff As Integer = ActorGroupOffset
      For i As Integer = 0 To ActorGroups.Count - 1
        IoUtil.WriteInt16(RamBanks.ZFileBuffer, AGrOff, ActorGroups(i))
      Next

      If rmActorCount > 0 Then
        Dim ActorStart As Integer = RoomActors(0).offset

        For i As Integer = 0 To RoomActors.Length - 1
          IoUtil.WriteInt16(RamBanks.ZFileBuffer, ActorStart, RoomActors(i).no)

          IoUtil.WriteInt16(RamBanks.ZFileBuffer, ActorStart, RoomActors(i).x)
          IoUtil.WriteInt16(RamBanks.ZFileBuffer, ActorStart, RoomActors(i).y)
          IoUtil.WriteInt16(RamBanks.ZFileBuffer, ActorStart, RoomActors(i).z)

          IoUtil.WriteInt16(RamBanks.ZFileBuffer, ActorStart, RoomActors(i).xr)
          IoUtil.WriteInt16(RamBanks.ZFileBuffer, ActorStart, RoomActors(i).yr)
          IoUtil.WriteInt16(RamBanks.ZFileBuffer, ActorStart, RoomActors(i).zr)

          IoUtil.WriteInt16(RamBanks.ZFileBuffer, ActorStart, RoomActors(i).var)
        Next
      End If
      'start saving to scene file buffer...

      Dim ExitOffset As Integer = 0
      For i As Integer = 0 To SceneExits.Length - 1
        ExitOffset = SceneExits(i).scOff
        IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ExitOffset, SceneExits(i).Index)
      Next

      Dim ColTypeOff As Integer = 0
      For i As Integer = 0 To ColTypes.Length - 1
        ColTypeOff = ColTypes(i).scOff

        IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ColTypeOff, ColTypes(i).unk1)

        IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ColTypeOff, ColTypes(i).unk2)

        IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ColTypeOff, ColTypes(i).unk3)

        RamBanks.ZSceneBuffer(ColTypeOff) = ColTypes(i).unk4 >> 8
        RamBanks.ZSceneBuffer(ColTypeOff + 1) = ((ColTypes(i).unk4 << 8 >> 4) + ColTypes(i).WalkOnSound)
      Next

      If CollisionPolies.Length > 0 Then
        Dim ColParamOff As Integer = 0
        For i As Integer = 0 To CollisionPolies.Length - 1
          ColParamOff = CollisionPolies(i).scOff
          IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ColParamOff, CollisionPolies(i).Param)
        Next
      End If

      If scActorCount > 0 Then
        Dim ActorStart As Integer = SceneActors(0).offset
        For i As Integer = 0 To SceneActors.Length - 1

          RamBanks.ZSceneBuffer(ActorStart + 0) = SceneActors(i).loadMapFront

          RamBanks.ZSceneBuffer(ActorStart + 2) = SceneActors(i).loadMapBack

          ActorStart += 4

          IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ActorStart, SceneActors(i).no)

          IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ActorStart, SceneActors(i).x)
          IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ActorStart, SceneActors(i).y)
          IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ActorStart, SceneActors(i).z)

          IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ActorStart, SceneActors(i).yr)

          IoUtil.WriteInt16(RamBanks.ZSceneBuffer, ActorStart, SceneActors(i).var)
        Next
      End If

    End If
  End Sub

  Private Sub SaveFiles(ByVal fn As String)
    If IndMapFileName = "" Then 'write files to ROM
      If DefROM <> "" Then
        ' TODO: Write files to ROM
        Dim ROMFileStream As New FileStream(fn, FileMode.Open)
        'RamBanks.ZFileBuffer.WriteToStream(ROMFileStream, MapSt)

        If LoadedDataType = FileTypes.MAP Then
          'RamBanks.ZSceneBuffer.WriteToStream(ROMFileStream, SceneSt)
        End If

        ROMFileStream.Dispose()

        crc.StartInfo.Arguments = "-u " & DefROM
        crc.Start()
        crc.WaitForExit()
      End If
      MsgBox("ROM saved!", MsgBoxStyle.Information, "Save")
    Else 'write to individually loaded files
      If RamBanks.ZFileBuffer.Count > 0 Then RamBanks.ZFileBuffer.WriteToFile(IndMapFileName)
      If RamBanks.ZSceneBuffer.Count > 0 Then RamBanks.ZSceneBuffer.WriteToFile(IndScFileName)
      MsgBox("Individual file(s) saved!", MsgBoxStyle.Information, "Save")
    End If
  End Sub

  Private Sub LockAxes(ByVal axis As Integer)
    ToolModes.AxisDisp = AxisStrings(axis)
    Select Case axis
      Case ToolID.NOLOCK
        DisableToolStripMenuItem.Checked = True
        XToolStripMenuItem.Checked = False
        YToolStripMenuItem.Checked = False
      Case ToolID.LOCKTOX
        DisableToolStripMenuItem.Checked = False
        XToolStripMenuItem.Checked = True
        YToolStripMenuItem.Checked = False
      Case ToolID.LOCKTOY
        DisableToolStripMenuItem.Checked = False
        XToolStripMenuItem.Checked = False
        YToolStripMenuItem.Checked = True
    End Select
    If ToolModes.CurrentTool = ToolID.ACTOR Then
      ToolStatusLabel.Text = "Tool: Actor Selector " & ToolModes.AxisDisp
    ElseIf ToolModes.CurrentTool = ToolID.VERTEX Then
      ToolStatusLabel.Text = "Tool: Vertex editor " & ToolModes.AxisDisp
    End If
    ToolModes.Axis = axis
  End Sub

  Private Sub SwitchTool(ByVal tool As Integer)
    ToolModes.CurrentTool = tool
    ToolModes.ToolDisp = ToolStrings(tool)
    ToolStatusLabel.Text = "Tool: " & ToolModes.ToolDisp
    If tool < ToolID.COLTRI Then
      ToolStatusLabel.Text += ToolModes.AxisDisp
    End If
    PrintTool = False
    Select Case tool
      Case ToolID.CAMERA
        ActorSelectorMenu.Checked = False
        CollisionSelectorMenu.Checked = False
        CameraOnlyMenu.Checked = True
        EdgeToolStripMenuItem.Checked = False
        VertexToolStripMenuItem.Checked = False
        TriangleToolStripMenuItem.Checked = False
        DisplayListSelectorToolStripMenuItem.Checked = False
      Case ToolID.ACTOR And LoadedDataType = FileTypes.MAP
        ActorSelectorMenu.Checked = True
        CollisionSelectorMenu.Checked = False
        CameraOnlyMenu.Checked = False
        EdgeToolStripMenuItem.Checked = False
        VertexToolStripMenuItem.Checked = False
        TriangleToolStripMenuItem.Checked = False
        DisplayListSelectorToolStripMenuItem.Checked = False
        EditingTabs.SelectedTab = EditingTabs.TabPages("ActorsTab")
      Case ToolID.VERTEX
        ActorSelectorMenu.Checked = False
        CollisionSelectorMenu.Checked = False
        CameraOnlyMenu.Checked = False
        EdgeToolStripMenuItem.Checked = False
        VertexToolStripMenuItem.Checked = True
        TriangleToolStripMenuItem.Checked = False
        DisplayListSelectorToolStripMenuItem.Checked = False

      Case ToolID.EDGE
        ActorSelectorMenu.Checked = False
        CollisionSelectorMenu.Checked = False
        CameraOnlyMenu.Checked = False
        EdgeToolStripMenuItem.Checked = True
        VertexToolStripMenuItem.Checked = False
        TriangleToolStripMenuItem.Checked = False
        DisplayListSelectorToolStripMenuItem.Checked = False
      Case ToolID.FACE
        ActorSelectorMenu.Checked = False
        CollisionSelectorMenu.Checked = False
        CameraOnlyMenu.Checked = False
        EdgeToolStripMenuItem.Checked = False
        VertexToolStripMenuItem.Checked = False
        TriangleToolStripMenuItem.Checked = True
        DisplayListSelectorToolStripMenuItem.Checked = False
      Case ToolID.COLTRI And LoadedDataType = FileTypes.MAP
        ActorSelectorMenu.Checked = False
        CollisionSelectorMenu.Checked = False
        CameraOnlyMenu.Checked = False
        EdgeToolStripMenuItem.Checked = False
        VertexToolStripMenuItem.Checked = False
        TriangleToolStripMenuItem.Checked = True
        DisplayListSelectorToolStripMenuItem.Checked = False
        EditingTabs.SelectedTab = EditingTabs.TabPages("CollisionTab")
      Case ToolID.DLIST
        ActorSelectorMenu.Checked = False
        CollisionSelectorMenu.Checked = False
        CameraOnlyMenu.Checked = False
        EdgeToolStripMenuItem.Checked = False
        VertexToolStripMenuItem.Checked = False
        TriangleToolStripMenuItem.Checked = False
        DisplayListSelectorToolStripMenuItem.Checked = True
        EditingTabs.SelectedTab = EditingTabs.TabPages("DLTab")
    End Select
  End Sub

  Private Sub XToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles XToolStripMenuItem.Click
    LockAxes(ToolID.LOCKTOX)
  End Sub

  Private Sub YToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles YToolStripMenuItem.Click
    LockAxes(ToolID.LOCKTOY)
  End Sub

  Private Sub DisableToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DisableToolStripMenuItem.Click
    LockAxes(ToolID.NONE)
  End Sub

  Private Sub ActorSelectorToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ActorSelectorMenu.Click
    SwitchTool(ToolID.ACTOR)
  End Sub

  Private Sub CameraOnlyToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CameraOnlyMenu.Click
    SwitchTool(ToolID.CAMERA)
  End Sub

  Private Sub RotationTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RotationTimer.Tick
    RotCoef -= &H800
    If RotCoef = 0 Then
      RotCoef = &H4000
      RotationTimer.Stop()
    End If
    If RoomActors.Length > 0 Then
      Select Case EditRotType
        Case 0
          Select Case EditRotAxis
            Case 0
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).xr += &H800
              Next
            Case 1
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).yr += &H800
              Next
            Case 2
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).zr += &H800
              Next
          End Select
        Case 1
          Select Case EditRotAxis
            Case 0
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).xr -= &H800
              Next
            Case 1
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).yr -= &H800
              Next
            Case 2
              For i As Integer = 0 To SelectedRoomActors.Count - 1
                RoomActors(SelectedRoomActors(i)).zr -= &H800
              Next
          End Select
      End Select
    ElseIf SceneActors.Length > 0 Then
      Select Case EditRotType
        Case 0
          Select Case EditRotAxis
            Case 1
              For i As Integer = 0 To SelectedSceneActors.Count - 1
                SceneActors(SelectedSceneActors(i)).yr += &H800
              Next
          End Select
        Case 1
          Select Case EditRotAxis
            Case 1
              For i As Integer = 0 To SelectedSceneActors.Count - 1
                SceneActors(SelectedSceneActors(i)).yr -= &H800
              Next
          End Select
      End Select
    End If
  End Sub

  Private Sub DegreesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DegreesToolStripMenuItem.Click
    EditRotType = 0
    EditRotAxis = 1
    RotationTimer.Start()
  End Sub

  Private Sub DegreesToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DegreesToolStripMenuItem1.Click
    EditRotType = 1
    EditRotAxis = 1
    RotationTimer.Start()
  End Sub

  Private Sub DegreesToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DegreesToolStripMenuItem2.Click
    EditRotType = 0
    EditRotAxis = 0
    RotationTimer.Start()
  End Sub

  Private Sub DegreesToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DegreesToolStripMenuItem3.Click
    EditRotType = 1
    EditRotAxis = 0
    RotationTimer.Start()
  End Sub

  Private Sub DegreesToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DegreesToolStripMenuItem4.Click
    EditRotType = 0
    EditRotAxis = 2
    RotationTimer.Start()
  End Sub

  Private Sub DegreesToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DegreesToolStripMenuItem5.Click
    EditRotType = 1
    EditRotAxis = 2
    RotationTimer.Start()
  End Sub

  Private Sub AnimationList_SelectedIndexChanged(animation As IAnimation) _
    Handles animationTab_.AnimationSelected

    CurrAnimation = animation

    animationTab_.AnimationPlaybackManager.Reset()
    If animation IsNot Nothing Then
      animationTab_.AnimationPlaybackManager.TotalFrames = animation.FrameCount
    End If
  End Sub

  Private Sub GraphicsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles GraphicsToolStripMenuItem.Click
    If RenderGraphics Then RenderGraphics = False Else RenderGraphics = True
  End Sub

  Private Sub CollisionOverlayToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CollisionOverlayToolStripMenuItem.Click
    If RenderCollision Then RenderCollision = False Else RenderCollision = True
  End Sub

  Private Sub WireframeModeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles WireframeModeToolStripMenuItem.Click
    ToggleWire()
  End Sub

  Private Sub LoadIndividual_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles LoadIndividual.FileOk

    Throw New NotSupportedException()

    If LoadIndividual.FileName.Contains(".zscene") Then
      IndScFileName = LoadIndividual.FileName
      RamBanks.ZSceneBuffer.PopulateFromFile(IndScFileName)
      MapsCombobox.Items.Clear()
      MapsCombobox.Enabled = True
      Dim tScPos As Integer = 0
      While RamBanks.ZSceneBuffer(tScPos) <> &H14
        Select Case RamBanks.ZSceneBuffer(tScPos)
          Case &H4
            Dim MapCount As Integer = RamBanks.ZSceneBuffer(tScPos + 1)
            Dim MapPos As Integer = RamBanks.ZSceneBuffer(tScPos + 5)*&H10000 + RamBanks.ZSceneBuffer(tScPos + 6)*&H100 +
                                    RamBanks.ZSceneBuffer(tScPos + 7)
            For i As Integer = 0 To MapCount - 1
              MapsCombobox.Items.Add(
                RamBanks.ZSceneBuffer(MapPos).ToString("X2") & RamBanks.ZSceneBuffer(MapPos + 1).ToString("X2") &
                RamBanks.ZSceneBuffer(MapPos + 2).ToString("X2") & RamBanks.ZSceneBuffer(MapPos + 3).ToString("X2"))
              MapPos += 8
            Next
        End Select
        tScPos += 8
      End While
      Me.Text = "Utility of Time - " & LoadIndividual.FileName
      If MapsCombobox.Items.Count > 0 Then
        MapsCombobox.SelectedIndex = 0
        EditingTabs.SelectedTab = EditingTabs.TabPages("MiscTab")
      Else
        MapsCombobox.Enabled = False
      End If
    ElseIf LoadIndividual.FileName.Contains(".zobj") Then
      EditingTabs.SelectedTab = EditingTabs.TabPages("DLTab")
      Me.Text = "Utility of Time - " & LoadIndividual.FileName
      IndMapFileName = LoadIndividual.FileName
      RamBanks.ZFileBuffer.PopulateFromFile(IndMapFileName)
      ' TODO: How to reset state here?
      'RamBanks.ZSceneBuffer.Region = null;
      SetVariables(SceneFileType.ZOBJ)
    End If
  End Sub

  Private Sub MapsCombobox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MapsCombobox.SelectedIndexChanged
    Dim Files() As String = Directory.GetFiles(GetFileName(LoadIndividual.FileName, True))
    For i As Integer = 0 To Files.Length - 1
      If Files(i).Contains(MapsCombobox.SelectedItem.ToString) Then
        IndMapFileName = Files(i)
        RamBanks.ZFileBuffer.PopulateFromFile(IndMapFileName)
        Start(True)
        Exit Sub
      End If
    Next
    MsgBox("Map not found in the same directory as the loaded level!", MsgBoxStyle.Critical, "Error")
  End Sub

  Private Sub PasteActor(ByVal Actor_Type As Integer, ByVal x As Boolean, ByVal y As Boolean, ByVal z As Boolean,
                         ByVal xr As Boolean, ByVal yr As Boolean, ByVal zr As Boolean, ByVal no As Boolean,
                         ByVal var As Boolean)
    Select Case Actor_Type
      Case 0
        For i1 As Integer = 0 To SelectedSceneActors.Count - 1
          If x Then SceneActors(SelectedSceneActors(i1)).x = CopyActor(0)
          If y Then SceneActors(SelectedSceneActors(i1)).y = CopyActor(1)
          If z Then SceneActors(SelectedSceneActors(i1)).z = CopyActor(2)
          If yr Then SceneActors(SelectedSceneActors(i1)).yr = CopyActor(4)
          If no Then SceneActors(SelectedSceneActors(i1)).no = CopyActor(6)
          If var Then SceneActors(SelectedSceneActors(i1)).var = CopyActor(7)
        Next
      Case 1
        For i1 As Integer = 0 To SelectedRoomActors.Count - 1
          If x Then RoomActors(SelectedRoomActors(i1)).x = CopyActor(0)
          If y Then RoomActors(SelectedRoomActors(i1)).y = CopyActor(1)
          If z Then RoomActors(SelectedRoomActors(i1)).z = CopyActor(2)
          If xr Then RoomActors(SelectedRoomActors(i1)).xr = CopyActor(3)
          If yr Then RoomActors(SelectedRoomActors(i1)).yr = CopyActor(4)
          If zr Then RoomActors(SelectedRoomActors(i1)).zr = CopyActor(5)
          If no Then RoomActors(SelectedRoomActors(i1)).no = CopyActor(6)
          If var Then RoomActors(SelectedRoomActors(i1)).var = CopyActor(7)
        Next
    End Select
    UpdateActorPos()
    If no Or var Then
      ActorNumberText.Text = CopyActor(6).ToString("X4")
      ActorVarText.Text = CopyActor(7).ToString("X4")
      UpdateActorIdents()
    End If
  End Sub

  Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CopyToolStripMenuItem.Click
    CopyActor(0) = RoomActors(SelectedRoomActors(0)).x
    CopyActor(1) = RoomActors(SelectedRoomActors(0)).y
    CopyActor(2) = RoomActors(SelectedRoomActors(0)).z
    CopyActor(3) = RoomActors(SelectedRoomActors(0)).xr
    CopyActor(4) = RoomActors(SelectedRoomActors(0)).yr
    CopyActor(5) = RoomActors(SelectedRoomActors(0)).zr
    CopyActor(6) = RoomActors(SelectedRoomActors(0)).no
    CopyActor(7) = RoomActors(SelectedRoomActors(0)).var
    PasteToolStripMenuItem.Enabled = True
    PasteToolStripMenuItem.Text = "Paste attributes from actor " & SelectedRoomActors(0).ToString
    ClearClipboardToolStripMenuItem.Enabled = True
  End Sub

  Private Sub XToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles XToolStripMenuItem1.Click
    PasteActor(ActorType, True, False, False, False, False, False, False, False)
  End Sub

  Private Sub YToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles YToolStripMenuItem1.Click
    PasteActor(ActorType, False, True, False, False, False, False, False, False)
  End Sub

  Private Sub ZToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ZToolStripMenuItem.Click
    PasteActor(ActorType, False, False, True, False, False, False, False, False)
  End Sub

  Private Sub AllToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles AllToolStripMenuItem1.Click
    PasteActor(ActorType, True, True, True, False, False, False, False, False)
  End Sub

  Private Sub XToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles XToolStripMenuItem2.Click
    PasteActor(ActorType, False, False, False, True, False, False, False, False)
  End Sub

  Private Sub YToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles YToolStripMenuItem2.Click
    PasteActor(ActorType, False, False, False, False, True, False, False, False)
  End Sub

  Private Sub ZToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ZToolStripMenuItem1.Click
    PasteActor(ActorType, False, False, False, False, False, True, False, False)
  End Sub

  Private Sub AllToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles AllToolStripMenuItem.Click
    PasteActor(ActorType, False, False, False, True, True, True, False, False)
  End Sub

  Private Sub NumberAndVariableToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles NumberAndVariableToolStripMenuItem.Click
    PasteActor(ActorType, False, False, False, False, False, False, True, True)
  End Sub

  Private Sub DeselectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DeselectToolStripMenuItem.Click
    SelectedRoomActors.Clear()
  End Sub

  Private Sub ClearClipboardToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ClearClipboardToolStripMenuItem.Click
    PasteToolStripMenuItem.Enabled = False
    PasteToolStripMenuItem.Text = "Paste attributes"
    For i As Integer = 0 To 7
      CopyActor(i) = - 1
    Next
    ClearClipboardToolStripMenuItem.Enabled = False
  End Sub

  Private Sub XToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles XToolStripMenuItem3.Click
    For i As Integer = 0 To SelectedRoomActors.Count - 1
      RoomActors(SelectedRoomActors(i)).x = RoomActors(SelectedRoomActors(0)).x
    Next
  End Sub

  Private Sub YToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles YToolStripMenuItem3.Click
    For i As Integer = 0 To SelectedRoomActors.Count - 1
      RoomActors(SelectedRoomActors(i)).y = RoomActors(SelectedRoomActors(0)).y
    Next
  End Sub

  Private Sub ZToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ZToolStripMenuItem2.Click
    For i As Integer = 0 To SelectedRoomActors.Count - 1
      RoomActors(SelectedRoomActors(i)).z = RoomActors(SelectedRoomActors(0)).z
    Next
  End Sub

  Private Sub SpawnActor(ByVal x As Short, ByVal y As Short, ByVal z As Short, ByVal CamXRot As Short,
                         ByVal CamYRot As Short, ByVal CamZRot As Short, ByVal Number As UInteger,
                         ByVal Variable As UInteger, ByVal Offset As UInteger)
    ReDim Preserve RoomActors(rmActorCount)
    ReDim Preserve UsedGroupIndex(rmActorCount)

    Dim camYRotD As Double = cam.Yaw/180*PI

    With RoomActors(rmActorCount)
      .y = y + camYRotD
      .x = - x + Sin(camYRotD)*640
      .z = - z - Cos(camYRotD)*640
      .xr = CamXRot
      .yr = CamYRot
      .zr = CamZRot
      .no = Number
      .var = Variable
      .pickR = Rand.Next(0, 255)
      .pickG = Rand.Next(0, 255)
      .pickB = Rand.Next(0, 255)
      .offset = Offset
    End With
    RoomActorCombobox.Items.Add((rmActorCount).ToString & " - " & IdentifyActor(0, rmActorCount))
    RoomActorCombobox.SelectedIndex = rmActorCount + 1
    RamBanks.ZFileBuffer.Resize(RamBanks.ZFileBuffer.Count + (rmActorCount*&H16))
    rmActorCount += 1
  End Sub

  Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    If Not RelocateActorPtr Then
      If MsgBox("Relocate actors to end of file?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
        ActorPointer(2) = RamBanks.ZFileBuffer.Count - 1
        RelocateActorPtr = True
      End If
    End If
    SpawnActor(cam.X, cam.Y, cam.Z, 0, 0, 0, &HDEAD, &HBEEF, 0)
  End Sub

  Private Sub MainWin_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) _
    Handles MyBase.FormClosing
    UoTIniFile.WriteString("Settings", "ToolSensitivity", TrackBar4.Value.ToString)
    UoTIniFile.WriteString("Settings", "CameraSpeed", TrackBar1.Value.ToString)
    UoTIniFile.WriteString("Settings", "WinResW", Me.Width)
    UoTIniFile.WriteString("Settings", "WinResH", Me.Height)
    AppExit = True
  End Sub

  Private Sub DisableDepthTestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DisableDepthTestToolStripMenuItem.Click
    If ToolModes.NoDepthTest Then ToolModes.NoDepthTest = False Else ToolModes.NoDepthTest = True
  End Sub

  Private Sub CollisionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CollisionToolStripMenuItem.Click
    SwitchTool(ToolID.COLTRI)
  End Sub

  Private Sub DisplayListSelectorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DisplayListSelectorToolStripMenuItem.Click
    SwitchTool(ToolID.DLIST)
  End Sub

  Private Sub FileTree_NodeMouseDoubleClick(zFile As IZFile) _
    Handles zFileTreeView_.FileSelected

    Select Case zFile.Type
      Case ZFileType.OBJECT
        ObjectFilename = zFile.FileName
        ObjectDisplayName = zFile.BetterFileName
        RamBanks.ZFileBuffer.Region = zFile.Region

        SetVariables(SceneFileType.ZOBJ)

        ' TODO: Pass the new model in.
        DlManager.DoSomething()

        IndMapFileName = ""
        IndScFileName = ""

        MapsCombobox.Items.Clear()
        MapsCombobox.Enabled = False

      Case ZFileType.CODE
        RamBanks.ZFileBuffer.Region = zFile.Region

        'RSPInterpreter.Parse(ZFileBuffer)

        IndMapFileName = ""
        IndScFileName = ""
        LoadedDataType = FileTypes.ACTORCODE
        MapsCombobox.Items.Clear()
        MapsCombobox.Enabled = False

      Case ZFileType.SCENE
        ' (Do nothing.)

      Case ZFileType.MAP
        Dim map As ZMap = zFile
        Dim scene As ZSc = map.Scene

        RamBanks.ZSceneBuffer.Region = scene.Region
        RamBanks.ZFileBuffer.Region = map.Region

        SetVariables(SceneFileType.ZSCENE)

        IndMapFileName = ""
        IndScFileName = ""
        MapsCombobox.Items.Clear()
        MapsCombobox.Enabled = False
        ScannedObjSet = False

      Case ZFileType.OTHER
        ' TODO: Should we show anything for these?
        ' RamBanks.CurrentBank = 1
        ' RenderGraphics = True
        ' RenderCollision = False
        ' If ToolModes.CurrentTool = ToolID.ACTOR Then
        ' ToolModes.CurrentTool = ToolID.CAMERA
        ' End If

        ' TODO: Handle object sets?
        ' Case ZFileType.OBJECT_SET
        ' objectset = Convert.ToUInt32(Mid(CurrentNodeText, 6), 16)
        ' ScannedObjSet = True
        ' ProcessMapHeader()
    End Select
  End Sub

  Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ToolStripMenuItem2.Click
    LoadIndividual.ShowDialog()
  End Sub

  Private Sub ColTriangleBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ColTriangleBox.SelectedIndexChanged
    If ColTriangleBox.SelectedIndex > 0 Then
      Dim colTriangle As Integer = ColTriangleBox.SelectedIndex - 1
      TriTypeText.Text = CInt(CollisionPolies(colTriangle).Param).ToString("X4")
      ColTypeBox.SelectedIndex = CollisionPolies(colTriangle).Param + 1
    End If
  End Sub

  Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
    If ColTriangleBox.SelectedIndex > 0 Then
      Dim colTriangle As Integer = ColTriangleBox.SelectedIndex - 1
      CollisionPolies(colTriangle).Param = TriTypeText.Text
      ColTypeBox.SelectedIndex = CollisionPolies(colTriangle).Param + 1
    End If
  End Sub

  Private Sub ToolStripMenuItem20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ToolStripMenuItem20.Click
    CopyActor(0) = CollisionVerts.x(SelectedCollisionVert(0))
    CopyActor(1) = CollisionVerts.y(SelectedCollisionVert(0))
    CopyActor(2) = CollisionVerts.z(SelectedCollisionVert(0))
    CopyActor(3) = - 1
    CopyActor(4) = - 1
    CopyActor(5) = - 1
    CopyActor(6) = - 1
    CopyActor(7) = 1

    PasteToolStripMenuItem.Enabled = True
    PasteToolStripMenuItem.Text = "Paste attributes from actor " & SelectedRoomActors(0).ToString
    ClearClipboardToolStripMenuItem.Enabled = True
  End Sub

  Private Sub ToolStripMenuItem35_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ToolStripMenuItem35.Click
    LoadROM.ShowDialog()
  End Sub

  Private Sub ToolStatusLabel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ToolStatusLabel.Click
    If ToolModes.CurrentTool < 6 Then
      SwitchTool(ToolModes.CurrentTool + 1)
    Else
      SwitchTool(ToolID.CAMERA)
    End If
  End Sub

  Private Sub CustomLevel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CustomLevel.Click
    LevelCreator.Show()
    LevelCreator.Focus()
  End Sub

  Private Sub DListSelection_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles DListSelection.SelectedIndexChanged
    CommandsListbox.Items.Clear()
    CommandJumpBox.Items.Clear()
    If DListSelection.SelectedIndex > 0 Then
      If HighlightDL Then
        EnableDLHighlight()
      Else
        DisableDLHighlight()
      End If
      Dim selectedDisplayList As N64DisplayList = DlManager.GetDisplayListByIndex(DListSelection.SelectedIndex - 1)
      For Each instruction As DLCommand In selectedDisplayList.Commands
        CommandsListbox.Items.Add(instruction.Name)
        If Not CommandJumpBox.Items.Contains(instruction.Name) Then
          CommandJumpBox.Items.Add(instruction.Name)
        End If
      Next
    Else
      For Each displayList As N64DisplayList In DlManager
        displayList.Highlight = False
        displayList.Skip = False
      Next
    End If
  End Sub

  Private Sub UpdateCommandDisplay()
    Dim selectedDisplayList As N64DisplayList = DlManager.GetDisplayListByIndex(DListSelection.SelectedIndex - 1)
    Dim selectedInstruction = selectedDisplayList.Commands(CommandsListbox.SelectedIndex)

    With selectedInstruction
      Dim opcode As UInteger = .Opcode
      Dim low As UInteger = .Low
      Dim high As UInteger = .High

      CommandCodeText.Text = opcode.ToString("X2")
      LowordText.Text = low.ToString("X6")
      HiwordText.Text = high.ToString("X8")
    End With

    WholeCommandTxt.Text = CommandCodeText.Text & LowordText.Text & HiwordText.Text
  End Sub

  Private Sub CommandsListbox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CommandsListbox.SelectedIndexChanged
    If CommandsListbox.SelectedIndex >= 0 Then
      UpdateCommandDisplay()
      Select Case CommandsListbox.SelectedItem
        Case "G_SETCOMBINE"
          If Not CombinerEditor.Visible Then
            Dim selectedDisplayList As N64DisplayList = DlManager.GetDisplayListByIndex(DListSelection.SelectedIndex - 1)
            LinkedCommands.EnvColor = RDP_Defs.FindLinkedCommand(selectedDisplayList, RDP.G_SETENVCOLOR,
                                                                 CommandsListbox.SelectedIndex)
            LinkedCommands.PrimColor = RDP_Defs.FindLinkedCommand(selectedDisplayList, RDP.G_SETPRIMCOLOR,
                                                                  CommandsListbox.SelectedIndex)
          Else
            CombinerEditor.Close()
          End If
          CombinerEditor.Show()
          CombinerEditor.Focus()
          Exit Sub
      End Select
    Else
      CommandCodeText.Text = ""
      LowordText.Text = ""
      HiwordText.Text = ""
    End If
    If CombinerEditor.Visible Then
      CombinerEditor.Close()
    End If
  End Sub

  Private Sub CommandsListbox_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CommandsListbox.DoubleClick
    If CommandsListbox.SelectedIndex >= 0 Then

    End If
  End Sub


  Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
    Dim cmd As String = Mid(WholeCommandTxt.Text, 1, 2)
    Dim lo As String = Mid(WholeCommandTxt.Text, 3, 6)
    Dim hi As String = Mid(WholeCommandTxt.Text, 9, 8)

    CommandCodeText.Text = cmd
    LowordText.Text = lo
    HiwordText.Text = hi

    Dim displayList As N64DisplayList = DlManager.GetDisplayListByIndex(DListSelection.SelectedIndex - 1)
    Dim command As DLCommand = displayList.Commands(CommandsListbox.SelectedIndex)
    command.Update(Convert.ToByte(cmd, 16),
                   Convert.ToUInt32(lo, 16),
                   Convert.ToUInt32(hi, 16))
  End Sub

  Private Sub EnableDLHighlight()
    For i As Integer = 0 To DlManager.Count - 1
      Dim displayList As N64DisplayList = DlManager.GetDisplayListByIndex(i)
      If i = DListSelection.SelectedIndex - 1 Then
        displayList.Highlight = True
      Else
        displayList.Highlight = False
      End If
      displayList.Skip = False
    Next
  End Sub

  Private Sub DisableDLHighlight()
    For i As Integer = 0 To DlManager.Count - 1
      Dim displayList As N64DisplayList = DlManager.GetDisplayListByIndex(i)
      If i = DListSelection.SelectedIndex - 1 Then
        displayList.Skip = False
      Else
        displayList.Skip = True
      End If
      displayList.Highlight = False
    Next
  End Sub

  Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles RadioButton1.CheckedChanged
    HighlightDL = True
    EnableDLHighlight()
  End Sub

  Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles RadioButton2.CheckedChanged
    HighlightDL = False
    DisableDLHighlight()
  End Sub

  Private Sub CommandCodeText_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) _
    Handles CommandCodeText.KeyPress, LowordText.KeyPress, HiwordText.KeyPress
    e.Handled = HexOnly(Char.ToUpper(e.KeyChar))
  End Sub

  Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
    Dim st As Integer = 0
    If CommandsListbox.SelectedIndex = CommandsListbox.Items.Count - 1 Then
      st = 0
    Else
      st = CommandsListbox.SelectedIndex + 1
    End If

    For i As Integer = st To CommandsListbox.Items.Count - 1
      If CommandsListbox.Items(i).ToString = CommandJumpBox.SelectedItem.ToString Then
        CommandsListbox.SelectedIndex = i
        Exit Sub
      End If
    Next
  End Sub

  Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
    Dim st As Integer = 0
    If CommandsListbox.SelectedIndex = 0 Then
      st = CommandsListbox.Items.Count - 1
    Else
      st = CommandsListbox.SelectedIndex - 1
    End If

    For i As Integer = st To 0 Step - 1
      If CommandsListbox.Items(i).ToString = CommandJumpBox.SelectedItem.ToString Then
        CommandsListbox.SelectedIndex = i
        Exit Sub
      End If
    Next
  End Sub

  Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
    If DListSelection.SelectedIndex > 0 Then
      RipAllDLs = False
      RipDL.ShowDialog()
    End If
  End Sub

  Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
    RipAllDLs = True
    RipDL.ShowDialog()
  End Sub

  Private Sub WriteDLToFile(ByVal DL As N64DisplayList, ByRef file As FileStream)
    For Each command As DLCommand In DL.Commands
      For ii As Integer = 0 To 7
        file.WriteByte(command.CMDParams(ii))
      Next
    Next
  End Sub

  Private Sub RipDL_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles RipDL.FileOk
    If File.Exists(RipDL.FileName) Then
      File.Delete(RipDL.FileName)
    End If
    RawDLFile = File.Create(RipDL.FileName)
    If RipAllDLs Then
      For Each displayList As N64DisplayList In DlManager
        WriteDLToFile(displayList, RawDLFile)
      Next
    Else
      WriteDLToFile(DlManager.GetDisplayListByIndex(DListSelection.SelectedIndex - 1), RawDLFile)
    End If
    RawDLFile.Dispose()
  End Sub

  Private Sub ToolStripMenuItem34_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ToolStripMenuItem34.Click
    SaveROMAs.ShowDialog()
  End Sub

  Private Sub SaveROMAs_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles SaveROMAs.FileOk
    If DefROM <> "" Then
      Dim romFS As FileStream = New FileStream(SaveROMAs.FileName, FileMode.Create)

    End If
  End Sub

  Private Sub WholeCommandTxt_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles WholeCommandTxt.TextChanged
  End Sub

  Private Sub CommandCodeText_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CommandCodeText.TextChanged, LowordText.TextChanged, HiwordText.TextChanged
    WholeCommandTxt.Text = CommandCodeText.Text & LowordText.Text & HiwordText.Text
  End Sub

  Private Sub TexturesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TexturesToolStripMenuItem.Click
    ToggleBoolean(RenderToggles.Textures, TexturesToolStripMenuItem)
  End Sub

  Private Sub ColorCombinerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles ColorCombinerToolStripMenuItem.Click
    ToggleBoolean(RenderToggles.ColorCombiner, ColorCombinerToolStripMenuItem)
  End Sub

  Private Sub AnisotropicFilteringToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles AnisotropicFilteringToolStripMenuItem.Click
    ToggleBoolean(RenderToggles.Anisotropic, AnisotropicFilteringToolStripMenuItem)
  End Sub

  Private Sub FullSceneAntialiasingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles FullSceneAntialiasingToolStripMenuItem.Click
    ToggleBoolean(RenderToggles.AntiAliasing, FullSceneAntialiasingToolStripMenuItem)
  End Sub

  Private Sub TrackBar1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TrackBar1.ValueChanged
    CameraCoef = TrackBar1.Value*8
  End Sub

  Private Sub TrackBar4_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TrackBar4.ValueChanged
    ToolSensitivity = TrackBar4.Value
  End Sub

#End Region

  Private Sub Button6_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
    ActorPresets.ShowDialog()
  End Sub
End Class
