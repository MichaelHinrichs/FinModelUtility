Imports System.Windows.Forms
Imports System.IO
Public Class ActorPresets
    Private ind As Integer = 0
    Private ActorDBReader As New PresetReader

    Private Sub ActorPresets_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If ActorDataBase.Length > 0 Then
            Dim curParent As Integer = 0
            For i As Integer = 0 To ActorDataBase.Length - 1
                ActorSelection.Nodes.Add(ActorDataBase(i).desc)
                If ActorDataBase(i).var IsNot Nothing Then
                    For i1 As Integer = 0 To ActorDataBase(i).var.Length - 1
                        ActorSelection.Nodes(curParent).Nodes.Add(ActorDataBase(i).var(i1).desc)
                    Next
                End If
                curParent += 1
            Next
        End If
    End Sub

    Private Sub ActorSelection_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles ActorSelection.AfterSelect

    End Sub
End Class
