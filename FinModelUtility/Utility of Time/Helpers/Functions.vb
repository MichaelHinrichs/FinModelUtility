Imports Tao.FreeGlut
Imports Tao.OpenGl

Module Functions
  Public Declare Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteA" _
    (ByVal hWnd As Long, ByVal lpOperation As String, ByVal lpFile As String, ByVal _
      lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Long) As Long
  Public Declare Function GetTickCount Lib "kernel32" () As Long

  Public Sub SetOGLDefaultParams()
    Gl.glDisable(Gl.GL_TEXTURE_2D)
    Gl.glDisable(Gl.GL_FRAGMENT_PROGRAM_ARB)
    Gl.glDisable(Gl.GL_LIGHTING)
    Gl.glDisable(Gl.GL_NORMALIZE)
    Gl.glDisable(Gl.GL_BLEND)
    Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
    Gl.glDisable(Gl.GL_CULL_FACE)
  End Sub

  Public Function RGBA8ColorToColorObject(ByVal Color As Color4UByte) As Color
    Return System.Drawing.Color.FromArgb(Color.a, Color.r,
                                         Color.g, Color.b)
  End Function

  Public Function PushOGLAttribs(ByVal attribs() As Integer)
    For i As Integer = 0 To attribs.Length - 1
      Gl.glPushAttrib(attribs(i))
    Next
  End Function

  Public Function PopOGLAttribls()
    Gl.glPopAttrib()
  End Function

  Public Function AngleToRad(ByVal angle As UShort) As Double
    Return (angle / &HFFFF) * 360.0
  End Function

  Public Function ToggleBoolean(ByRef bool As Boolean, ByRef MenuItem As ToolStripMenuItem)
    If bool Then bool = False Else bool = True
    If MenuItem IsNot Nothing Then
      MenuItem.Checked = bool
    End If
  End Function

  Public Sub InitNewCommand(ByRef Command As DLCommand, ByVal CommandCode As Byte)
    Command = New DLCommand(CommandCode)
  End Sub

  Public Function Flip32(ByVal Flip As ULong) As ULong
    Return ((Flip And &HFF000000) >> 24) Or ((Flip And &HFF0000) >> 8) Or
           ((Flip And &HFF00) << 8) Or ((Flip And &HFF) << 24)
  End Function

  Public Function NoExt(ByVal FullPath _
                         As String) As String
    Return System.IO.Path.GetFileNameWithoutExtension(FullPath)
  End Function

  Public Function HexOnly(ByVal str As String) As Boolean
    If "0123456789ABCDEF".IndexOf(str) = -1 Then
      Return True
    Else
      Return False
    End If
  End Function

  Public Function GetDir(ByVal fn As String) As String
    For i As Integer = fn.Length - 1 To 0 Step -1
      If fn(i) = "\" Or fn(i) = "/" Then
        Return Mid(fn, 1, i)
      End If
    Next
    Return ""
  End Function

  Public Function ConvertHexToSingle(ByVal hexValue As String) As Single
    Try
      Dim iInputIndex As Integer = 0
      Dim iOutputIndex As Integer = 0
      Dim bArray(3) As Byte
      For iInputIndex = 0 To hexValue.Length - 1 Step 2
        bArray(iOutputIndex) = Byte.Parse(hexValue.Chars(iInputIndex) & hexValue.Chars(iInputIndex + 1),
                                          Globalization.NumberStyles.HexNumber)
        iOutputIndex += 1
      Next
      Array.Reverse(bArray)
      Return BitConverter.ToSingle(bArray, 0)
    Catch ex As Exception
      Throw _
        New FormatException(
          "The supplied hex value is either empty or in an incorrect format. Use the following format: 00000000", ex)
    End Try
  End Function

  Public Function CheckAllChildNodes(ByVal treeNode1 As TreeNode, ByVal nodeChecked As Boolean) As Object
    Dim node As TreeNode
    For Each node In treeNode1.Nodes
      node.Checked = nodeChecked
      If node.Nodes.Count > 0 Then
        CheckAllChildNodes(node, nodeChecked)
      End If
    Next
  End Function

  'macros for processing n64 dl commands
  Public Function Hex2(ByVal sHex As String) As Byte()
    Dim n As Long
    Dim nCount As Long
    Dim bArr() As Byte
    nCount = Len(sHex)
    If (nCount And 1) = 1 Then
      sHex = "0" & sHex
      nCount += 1
    End If
    ReDim bArr((nCount \ 2) - 1)
    For n = 1 To nCount Step 2
      bArr((n - 1) \ 2) = CByte("&H" & Mid$(sHex, n, 2))
    Next
    Hex2 = bArr
  End Function

  Public Function GetFileName(ByVal flname As String, ByVal getdir As Boolean) As String
    Dim posn As Integer, i As Integer
    Dim fName As String
    Dim fLen As Integer = flname.Length - 1
    If Not getdir Then
      For i = 0 To fLen
        If flname(i) = "\" Or flname(i) = "/" Then posn = i
      Next
    Else
      For i = fLen To 0 Step -1
        If flname(i) = "\" Or flname(i) = "/" Then
          posn = i
          Exit For
        End If
      Next
    End If
    If getdir Then fName = Mid(flname, 1, posn) Else fName = Right(flname, fLen - posn)
    Return fName
  End Function

  Public Function SearchListbox(ByRef listbx As ListBox, ByVal searchbox As TextBox, ByVal startind As Integer,
                                ByVal nxt As Boolean) As Boolean
    If searchbox.Text <> "" Then
      If Not nxt Then
        For i As Integer = 0 To listbx.Items.Count - 1
          If listbx.Items.Item(i).tolower.contains(searchbox.Text.ToLower) Then
            listbx.SelectedIndex = i
            Return True
          End If
        Next
      Else
        For i As Integer = startind + 1 To listbx.Items.Count - 1
          If listbx.Items.Item(i).tolower.contains(searchbox.Text.ToLower) Then
            listbx.SelectedIndex = i
            Return True
          End If
        Next
      End If
      listbx.SelectedIndex = -1
      Return False
    End If
  End Function

  Public Function GLPrint2D(ByVal Text As String, ByVal Position As Point, ByVal TextColor As Color,
                            ByVal GLUTFONT As IntPtr, ByVal XOffset As Integer, ByVal YOffset As Integer,
                            ByVal Shadow As Boolean)
    Gl.glMatrixMode(Gl.GL_PROJECTION)
    Gl.glPushMatrix()
    Gl.glLoadIdentity()
    Gl.glOrtho(0, winw, 0, winh, 0, 1)

    Gl.glMatrixMode(Gl.GL_MODELVIEW)
    Gl.glPushMatrix()
    Gl.glLoadIdentity()
    Dim XPos As Integer = Position.X
    Dim YPos As Integer = Position.Y
    If Shadow Then
      'shadow (black - 0r, 0g, 0b)
      Gl.glColor3f(0, 0, 0)
      Gl.glRasterPos2f(XPos + XOffset + 1, (winh - YPos) + YOffset - 1)
      For a As Integer = 0 To Text.Length - 1
        Glut.glutBitmapCharacter(GLUTFONT, Asc(Text(a)))
      Next
    End If

    'main text (white - 1r, 1g, 1b)
    Gl.glColor3ub(TextColor.R, TextColor.G, TextColor.B)
    Gl.glRasterPos2f(XPos + XOffset, (winh - YPos) + YOffset)
    For a As Integer = 0 To Text.Length - 1
      Glut.glutBitmapCharacter(GLUTFONT, Asc(Text(a)))
    Next

    Gl.glMatrixMode(Gl.GL_PROJECTION)
    Gl.glPopMatrix()

    Gl.glMatrixMode(Gl.GL_MODELVIEW)
    Gl.glPopMatrix()
  End Function
End Module
