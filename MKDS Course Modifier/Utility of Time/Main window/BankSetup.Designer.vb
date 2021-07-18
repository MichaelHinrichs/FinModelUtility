<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BankSetup
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ROM\Gameplay_Keep.zdata")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("0x04", New System.Windows.Forms.TreeNode() {TreeNode1})
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ROM\Gameplay_Field_Keep")
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ROM\Gameplay_Dangeon_Keep.zdata")
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("0x05", New System.Windows.Forms.TreeNode() {TreeNode3, TreeNode4})
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ROM\ObjectFile")
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ROM\object_oE_anime.zobj")
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ROM\object_o_anime.zobj")
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Animation", New System.Windows.Forms.TreeNode() {TreeNode6, TreeNode7, TreeNode8})
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Banks", New System.Windows.Forms.TreeNode() {TreeNode2, TreeNode5, TreeNode9})
        Me.TreeView1 = New System.Windows.Forms.TreeView
        Me.SuspendLayout()
        '
        'TreeView1
        '
        Me.TreeView1.Location = New System.Drawing.Point(12, 12)
        Me.TreeView1.Name = "TreeView1"
        TreeNode1.Name = "Node4"
        TreeNode1.Text = "ROM\Gameplay_Keep.zdata"
        TreeNode2.Name = "Node1"
        TreeNode2.Text = "0x04"
        TreeNode3.Name = "Node5"
        TreeNode3.Text = "ROM\Gameplay_Field_Keep"
        TreeNode4.Name = "Node7"
        TreeNode4.Text = "ROM\Gameplay_Dangeon_Keep.zdata"
        TreeNode5.Name = "Node2"
        TreeNode5.Text = "0x05"
        TreeNode6.Name = "Node8"
        TreeNode6.Text = "ROM\ObjectFile"
        TreeNode7.Name = "Node9"
        TreeNode7.Text = "ROM\object_oE_anime.zobj"
        TreeNode8.Name = "Node10"
        TreeNode8.Text = "ROM\object_o_anime.zobj"
        TreeNode9.Name = "Node3"
        TreeNode9.Text = "Animation"
        TreeNode10.Name = "Node0"
        TreeNode10.Text = "Banks"
        Me.TreeView1.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode10})
        Me.TreeView1.Size = New System.Drawing.Size(202, 332)
        Me.TreeView1.TabIndex = 0
        '
        'ObjectExchange
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(225, 423)
        Me.Controls.Add(Me.TreeView1)
        Me.Name = "ObjectExchange"
        Me.Text = "Bank setup"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents TreeView1 As System.Windows.Forms.TreeView
End Class
