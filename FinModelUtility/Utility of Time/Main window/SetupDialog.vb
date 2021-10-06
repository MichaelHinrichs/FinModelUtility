Imports System.Windows.Forms
Imports System.io

Public Class SetupDialog
    Private bankmt As String
    Private iniwrite As New iniwriter(Application.StartupPath & "/uot.ini")

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If TextBox2.Text <> "" And File.Exists(TextBox2.Text) = True Then
            DefROM = TextBox2.Text
            iniwrite.WriteString("Settings", "DefaultROM", DefROM)
            MainWin.LoadROM.FileName = DefROM
            MainWin.Start(False)
        End If
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Dialog3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DefROM = iniwrite.GetString("Settings", "DefaultROM", Nothing)
        TextBox2.Text = DefROM
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If OpenFileDialog1.ShowDialog = DialogResult.Cancel Then
            OpenFileDialog1.Dispose()
        End If
    End Sub

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        TextBox2.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If OpenFileDialog2.ShowDialog = DialogResult.Cancel Then
            OpenFileDialog2.Dispose()
        End If
    End Sub
End Class
