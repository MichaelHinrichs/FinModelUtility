Imports Tao.OpenGl
Imports Tao.DevIl
Imports System.Math
Imports System.IO
Imports System.Xml


Public Class LevelCreator

#Region "app interaction"

  Dim SeparateCollisionMap As Boolean = False

#End Region

#Region "Map creation"

  Enum Commands
    'commands
    SINGLEROOM = &H16
    ENVVARS = &H12
    TIMECONTROL = &H10
    MESHDATA = &HA
    MULTIROOM = &H18
    SCENESTART = &H15
    HEADEREND = &H14
    EXITS = &H13
    SKYBOX = &H11
    ROOMACTORS = &H1
    SCENEACTORS = &HE
    GROUPS = &HB
    LINKS = &H0

    'banks
    MAPBANK = &H3
    SCENEBANK = &H2
  End Enum

  Enum HeaderTypes
    SCENE = 0
    MAP = 1
  End Enum

  Structure Output
    Dim RawHeader() As Byte
    Dim RawData() As Byte
  End Structure

  Structure Header
    Dim Command As Integer
  End Structure

  Structure Actor
    Dim x As Short
    Dim y As Short
    Dim z As Short
    Dim xr As Short
    Dim yr As Short
    Dim zr As Short
    Dim no As UInteger
    Dim var As UInteger
  End Structure

  Structure Group
    Dim val As UInteger
  End Structure

  Structure Pointers
    Dim ActorPtr As UInteger
    Dim GroupPtr As UInteger
    Dim GraphicsPtr As UInteger
    Dim VertStart As UInteger
    Dim CollisionPtr As UInteger
    Dim ScActorPtr As UInteger
    Dim LinkActorPtr As UInteger
  End Structure

  Structure Count
    Dim RmActorCount As Byte
    Dim ScActorCount As Byte
    Dim LinkCount As Byte
    Dim GrCount As Byte
  End Structure

  Structure Env
    Dim EchoLevel As Byte
  End Structure

  Structure ColPoly
    Dim Param As Integer
    Dim A As Integer
    Dim B As Integer
    Dim C As Integer
    Dim nX As Short
    Dim nY As Short
    Dim nZ As Short
    Dim Distance As Short
  End Structure

  Structure ColVert
    Dim x As Short
    Dim y As Short
    Dim z As Short
  End Structure

  Structure ColHead
    Dim MinX As Short
    Dim MinY As Short
    Dim MinZ As Short
    Dim MaxX As Short
    Dim MaxY As Short
    Dim MaxZ As Short
    Dim VertCnt As UInteger
    Dim VertOff As UInteger
    Dim PolyCnt As UInteger
    Dim PolyOff As UInteger
    Dim PolyTypeOff As UInteger
    Dim CamDataOff As UInteger
    Dim WaterBoxCnt As UInteger
    Dim WaterBoxOff As UInteger
  End Structure

  Public TemplateFilename As String = ""
  Public TemplateName As String = ""
  Public CollisionOBJ As New OBJModel
  Public GraphicsOBJ As New OBJModel
  Public Pointer As New Pointers
  Public Counts As New Count
  Public Groups(-1) As Group
  Public RoomActors(-1) As Actor
  Public SceneActors(-1) As Actor
  Public LinkActors(-1) As Actor
  Public Environment As New Env
  Public MapBuff As New Output
  Public SceneBuff As New Output
  Public CollisionHeader As New ColHead
  Public CollisionVertices() As ColVert
  Public CollisionPolies() As ColPoly
  Public objGraphicsFile As String = ""
  Public objCollisionFile As String = ""

#End Region

  Private Sub ObjExport_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    Il.ilInit()
    NewLevel()
  End Sub

  Private Sub AddGroup(ByRef _Groups() As Group, ByVal Count As Integer)
    For i As Integer = 0 To Count - 1
      ReDim Preserve _Groups(Counts.GrCount)
      With _Groups(Counts.GrCount)
        .val = &HB00B
      End With
      Counts.GrCount += 1
    Next
  End Sub

  Private Sub AddRoomActor(ByRef _Actors() As Actor, ByVal count As Integer)
    For i As Integer = 0 To count - 1
      ReDim Preserve _Actors(Counts.RmActorCount)
      With _Actors(Counts.RmActorCount)
        .x = 0
        .y = 0
        .z = 0
        .xr = 0
        .yr = 0
        .zr = 0
        .no = &HDEAD
        .var = &HBEEF
      End With
      Counts.RmActorCount += 1
    Next
  End Sub

  Private Sub AddSceneActor(ByRef _SceneActors() As Actor, ByVal count As Integer)
    For i As Integer = 0 To count - 1
      ReDim Preserve _SceneActors(Counts.ScActorCount)
      With _SceneActors(Counts.ScActorCount)
        .x = 0
        .y = 0
        .z = 0
        .xr = -1
        .yr = 0
        .zr = -1
        .no = &HDEAD
        .var = &HBEEF
      End With
      Counts.ScActorCount += 1
    Next
  End Sub

  Private Sub AddLinkActor(ByRef _LinkActors() As Actor, ByVal count As Integer)
    For i As Integer = 0 To count - 1
      ReDim Preserve _LinkActors(Counts.LinkCount)
      With _LinkActors(Counts.LinkCount)
        .x = 0
        .y = 0
        .z = 0
        .xr = 0
        .yr = 0
        .zr = 0
        .no = &HDEAD
        .var = &HBEEF
      End With
      Counts.LinkCount += 1
    Next
  End Sub

  Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles SeparateCollisionToggle.CheckedChanged
    If SeparateCollisionMap Then
      ObjCollisionText.Text = ""
      objCollisionFile = ""
      ObjCollisionText.Enabled = False
      Button3.Enabled = False
      SeparateCollisionMap = False
      CollisionOBJ = Nothing
    Else
      ObjCollisionText.Enabled = True
      Button3.Enabled = True
      SeparateCollisionMap = True
      CollisionOBJ = New OBJModel
    End If
  End Sub

  Private Function CompileMapHeader() As Byte() 'NEEDS MODULARITY
    Dim tBuffer(0) As Byte
    Dim tPos As Integer = 0

    With Pointer
      .GroupPtr = &H38
      .ActorPtr = .GroupPtr + (Counts.GrCount * 2)
      .GraphicsPtr = .ActorPtr + (Counts.RmActorCount * 16)
      .VertStart = .GraphicsPtr + 32
    End With

    IoUtil.WriteInt32(tBuffer, &H16000000, tPos)
    IoUtil.WriteInt32(tBuffer, 0 Or Environment.EchoLevel, tPos)

    IoUtil.WriteInt32(tBuffer, &H12000000, tPos)
    IoUtil.WriteInt32(tBuffer, 0, tPos)

    IoUtil.WriteInt32(tBuffer, &H10000000, tPos)
    IoUtil.WriteInt32(tBuffer, 0, tPos)

    IoUtil.WriteInt32(tBuffer, &HA000000, tPos)
    IoUtil.WriteInt32(tBuffer, &H3000000 Or Pointer.GraphicsPtr, tPos)

    IoUtil.WriteInt16(tBuffer, tPos, &HB00 Or Counts.GrCount)
    IoUtil.WriteInt32(tBuffer, &H3000000 Or Pointer.GroupPtr, tPos)

    IoUtil.WriteInt16(tBuffer, tPos, &H100 Or Counts.RmActorCount)
    IoUtil.WriteInt32(tBuffer, &H3000000 Or Pointer.ActorPtr, tPos)

    IoUtil.WriteInt32(tBuffer, &H14000000, tPos)
    IoUtil.WriteInt32(tBuffer, 0, tPos)

    Dim tPos1 As Integer = Pointer.GroupPtr

    For i As Integer = 0 To Counts.GrCount - 1
      IoUtil.WriteInt16(tBuffer, tPos1, Groups(i).val)
    Next

    For i As Integer = 0 To Counts.RmActorCount - 1
      IoUtil.WriteInt16(tBuffer, tPos1, RoomActors(i).x)
      IoUtil.WriteInt16(tBuffer, tPos1, RoomActors(i).y)
      IoUtil.WriteInt16(tBuffer, tPos1, RoomActors(i).z)
      IoUtil.WriteInt16(tBuffer, tPos1, RoomActors(i).xr)
      IoUtil.WriteInt16(tBuffer, tPos1, RoomActors(i).yr)
      IoUtil.WriteInt16(tBuffer, tPos1, RoomActors(i).zr)
      IoUtil.WriteInt16(tBuffer, tPos1, RoomActors(i).no)
      IoUtil.WriteInt16(tBuffer, tPos1, RoomActors(i).var)
    Next

    IoUtil.WriteInt16(tBuffer, tPos1, 1)
    IoUtil.WriteInt16(tBuffer, tPos1, 0)

    IoUtil.WriteInt32(tBuffer, &H3000000 Or tPos1 + 8, tPos1)
    IoUtil.WriteInt32(tBuffer, &H3000000 Or tPos1 + 8, tPos1)
    IoUtil.WriteInt32(tBuffer, &H3000000 Or (tPos1 + 16) + (GraphicsOBJ.Vertices.Length * 16), tPos1)
    IoUtil.WriteInt32(tBuffer, &H1000000, tPos1)

    For i As Integer = 0 To 2
      IoUtil.WriteInt32(tBuffer, 0, tPos1)
    Next

    For i As Integer = 0 To GraphicsOBJ.Vertices.Length - 1
      With GraphicsOBJ.Vertices(i)
        IoUtil.WriteInt16(tBuffer, tPos1, .x)
        IoUtil.WriteInt16(tBuffer, tPos1, .y)
        IoUtil.WriteInt16(tBuffer, tPos1, .z)
        IoUtil.WriteInt16(tBuffer, tPos1, 0)
        IoUtil.WriteInt16(tBuffer, tPos1, GraphicsOBJ.TexCoords(i).s)
        IoUtil.WriteInt16(tBuffer, tPos1, GraphicsOBJ.TexCoords(i).t)
        ReDim Preserve tBuffer(tPos1 + 4)
        tBuffer(tPos1) = &HFF
        tBuffer(tPos1 + 1) = &HFF
        tBuffer(tPos1 + 2) = &HFF
        tBuffer(tPos1 + 3) = &HFF
        tPos1 += 4
      End With
    Next

    ReDim Preserve tBuffer(tBuffer.Length - 2)

    File.WriteAllBytes("C:/Test.zmap", tBuffer)

    Return tBuffer
  End Function

  Private Function CompileSceneHeader() As Byte() 'NEEDS MODULARITY
  End Function

  Private Sub LoadTemplate(ByVal fn As String)
    Dim TemplateParser As XmlReader = New XmlTextReader(fn)
    TemplateParser.Read()
    While Not TemplateParser.EOF
      TemplateParser.Read()
      If Not TemplateParser.IsStartElement Then
        Exit While
      End If


    End While
  End Sub

  Private Sub SaveTemplateFile(ByVal fn As String)
    Dim TemplateParser As XmlTextWriter = New XmlTextWriter(fn, System.Text.Encoding.UTF8)

    With TemplateParser
      .WriteStartDocument()

      .WriteStartElement("UoT Level Template")

      .WriteElementString("Title", TemplateName)

      .WriteStartElement("OBJ Models")

      .WriteElementString("Graphics", objGraphicsFile)
      If SeparateCollisionMap Then .WriteElementString("Collision", objCollisionFile) Else _
        .WriteElementString("Collision", objGraphicsFile)

      .WriteEndElement()

      .WriteStartElement("Map Data") '<Map Data>

      If Counts.GrCount > 0 Then
        .WriteStartElement("Groups") '<Groups>

        For i As Integer = 0 To Counts.GrCount - 1
          .WriteElementString("group", Groups(i).val.ToString("X4")) '<group> data </group>
        Next

        .WriteEndElement() '</Groups>
      End If

      If Counts.RmActorCount > 0 Then
        .WriteStartElement("Actors") '<Actors>

        For i As Integer = 0 To Counts.RmActorCount - 1
          .WriteStartElement("actor") '<actor>

          .WriteElementString("xPos", RoomActors(i).x.ToString("X4"))
          .WriteElementString("yPos", RoomActors(i).y.ToString("X4"))
          .WriteElementString("zPos", RoomActors(i).z.ToString("X4"))
          .WriteElementString("xRot", RoomActors(i).xr.ToString("X4"))
          .WriteElementString("yRot", RoomActors(i).yr.ToString("X4"))
          .WriteElementString("zRot", RoomActors(i).zr.ToString("X4"))
          .WriteElementString("Number", RoomActors(i).no.ToString("X4"))
          .WriteElementString("Variable", RoomActors(i).var.ToString("X4"))

          .WriteEndElement() '</actor>
        Next

        .WriteEndElement() '</Actors>
      End If


      .WriteEndElement() '</Map Data>


      'SCENE DATA


      .WriteStartElement("Scene Data") '<Scene Data>

      If Counts.ScActorCount > 0 Then
        .WriteStartElement("Actors") '<Actors>
        For i As Integer = 0 To Counts.ScActorCount - 1
          .WriteStartElement("scactor") '<scactor>

          .WriteElementString("xPos", SceneActors(i).x.ToString("X4"))
          .WriteElementString("yPos", SceneActors(i).y.ToString("X4"))
          .WriteElementString("zPos", SceneActors(i).z.ToString("X4"))
          .WriteElementString("yRot", SceneActors(i).yr.ToString("X4"))
          .WriteElementString("Number", SceneActors(i).no.ToString("X4"))
          .WriteElementString("Variable", SceneActors(i).var.ToString("X4"))

          .WriteEndElement() '</scactor>
        Next
        .WriteEndElement() '</Actors>
      End If

      If Counts.LinkCount > 0 Then
        .WriteStartElement("Links") '<Links>
        For i As Integer = 0 To Counts.LinkCount - 1
          .WriteStartElement("link") '<link>

          .WriteElementString("xPos", LinkActors(i).x.ToString("X4"))
          .WriteElementString("yPos", LinkActors(i).y.ToString("X4"))
          .WriteElementString("zPos", LinkActors(i).z.ToString("X4"))
          .WriteElementString("xRot", LinkActors(i).xr.ToString("X4"))
          .WriteElementString("yRot", LinkActors(i).yr.ToString("X4"))
          .WriteElementString("zRot", LinkActors(i).zr.ToString("X4"))
          .WriteElementString("Number", LinkActors(i).no.ToString("X4"))
          .WriteElementString("Variable", LinkActors(i).var.ToString("X4"))

          .WriteEndElement() '</link>
        Next
        .WriteEndElement() '</Links>
      End If

      .WriteEndElement() '</Scene Data>

      .WriteEndDocument()


      .Close()
    End With
  End Sub

  Private Sub OpenTemplate_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles OpenTemplate.FileOk
    LoadTemplate(OpenTemplate.FileName)
  End Sub

  Private Sub SaveTemplate_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles SaveTemplate.FileOk
    SaveTemplateFile(SaveTemplate.FileName)
  End Sub

  Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    With Environment
      Select Case EchoLevelMenu.SelectedIndex
        Case 0
          .EchoLevel = 0
        Case 1
          .EchoLevel = 1
        Case 2
          .EchoLevel = 4
        Case 3
          .EchoLevel = &HF
      End Select
    End With
  End Sub

  Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
    OpenOBJGraphics.ShowDialog()
  End Sub

  Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
    OpenOBJCollision.ShowDialog()
  End Sub

  Private Sub OpenOBJGraphics_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles OpenOBJGraphics.FileOk
    objGraphicsFile = OpenOBJGraphics.FileName
    GraphicsOBJ = ParseOBJ.Parse(objGraphicsFile, False)
    OBJGraphicsText.Text = objGraphicsFile
  End Sub

  Private Sub OpenOBJCollision_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles OpenOBJCollision.FileOk
    objCollisionFile = OpenOBJCollision.FileName
    CollisionOBJ = ParseOBJ.Parse(objCollisionFile, True)
    ObjCollisionText.Text = objCollisionFile
  End Sub


  Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles UpdateRawDataButton.Click
    ReDim RoomActors(RoomActorCount.Value - 1)
    Counts.RmActorCount = 0
    If RoomActorCount.Value > 0 Then AddRoomActor(RoomActors, RoomActorCount.Value)

    ReDim Groups(RoomGroupCount.Value - 1)
    Counts.GrCount = 0
    If RoomGroupCount.Value > 0 Then AddGroup(Groups, RoomGroupCount.Value)

    MapBuff.RawHeader = CompileMapHeader()

    ReDim SceneActors(SceneActorCount.Value - 1)
    Counts.ScActorCount = 0
    If SceneActorCount.Value > 0 Then AddSceneActor(SceneActors, SceneActorCount.Value)

    ReDim LinkActors(LinkActorCount.Value - 1)
    Counts.LinkCount = 0
    If LinkActorCount.Value > 0 Then AddLinkActor(LinkActors, LinkActorCount.Value)

    'SceneBuff.RawHeader = compilesceneheader()
  End Sub

  Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles SaveToolStripMenuItem.Click
    SaveTemplate.FileName = TemplateName & ".xml"
    SaveTemplate.ShowDialog()
  End Sub

  Private Sub TextBox5_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles LevelNameText.TextChanged
    TemplateName = LevelNameText.Text
    Me.Text = "UoT Level Creator Alpha - " & TemplateName
  End Sub

  Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles OpenToolStripMenuItem.Click
    OpenTemplate.ShowDialog()
  End Sub

  Private Sub AskToSave()
    Dim SaveQuestion As MsgBoxResult = MsgBox("Would you like to save your current level template?", MsgBoxStyle.YesNo,
                                              "Save changes")
    If SaveQuestion = MsgBoxResult.Yes Then
      SaveTemplate.ShowDialog()
    End If
  End Sub

  Private Sub NewLevel()
    RoomActorCount.Value = 0
    SceneActorCount.Value = 0
    RoomGroupCount.Value = 0
    LinkActorCount.Value = 0
    MapRawDataDisplay.Text = ""
    SceneRawDataDisplay.Text = ""
    EchoLevelMenu.SelectedIndex = 1
    SkyboxToggle.Checked = True
    LevelNameText.Text = "Untitled level"
    With Counts
      .RmActorCount = 0
      .ScActorCount = 0
      .LinkCount = 0
      .GrCount = 0
    End With
    ReDim CollisionVertices(-1)
    ReDim CollisionPolies(-1)
    ReDim MapBuff.RawData(-1)
    ReDim MapBuff.RawHeader(-1)
    ReDim SceneBuff.RawData(-1)
    ReDim SceneBuff.RawHeader(-1)
    GraphicsOBJ = New OBJModel
    CollisionOBJ = New OBJModel
  End Sub

  Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles NewToolStripMenuItem.Click
    AskToSave()
    NewLevel()
  End Sub
End Class