Imports System.IO
Imports Tao.DevIl
Imports System.Math
Imports System.Runtime.InteropServices

Public Class OBJParser
  Public Function Parse(ByVal fn As String, ByVal SkipMtl As Boolean) As OBJModel
    Try
      If File.Exists(fn) Then
        Il.ilInit()
        Dim tempObj As New OBJModel

        With tempObj
          ReDim .Parts(- 1)
          ReDim .Vertices(- 1)
          ReDim .TexCoords(- 1)
          ReDim .Normals(- 1)
        End With

        Dim objText As New StreamReader(fn)

        Dim CurLine As String = ""
        Dim CurTokens() As String

        Dim CurMtlLine As String = ""
        Dim CurMtlTokens() As String

        Dim vCnt As Integer = 0
        Dim vtCnt As Integer = 0
        Dim fCnt As Integer = 0

        Dim mtlCnt As Integer = 0
        Dim mtlFileCnt As Integer = 0
        Dim texCnt As Integer = 0
        Dim grpCnt As Integer = 0

        Dim fSplit() As Char = {" ", "/"}

        While objText.Peek <> - 1
          CurLine = objText.ReadLine
          CurTokens = CurLine.Split(" ")
          Select Case CurTokens(0)
            Case "v"
              ReDim Preserve tempObj.Vertices(vCnt)
              With tempObj.Vertices(vCnt)
                .x = CDbl(CurTokens(1))
                .y = CDbl(CurTokens(2))
                .z = CDbl(CurTokens(3))
              End With
              vCnt += 1
            Case "vt"
              ReDim Preserve tempObj.TexCoords(vtCnt)
              With tempObj.TexCoords(vtCnt)
                .s = CDbl(CurTokens(1))
                .t = CDbl(CurTokens(2))
              End With
              vtCnt += 1
            Case "g"
              grpCnt += 1
              ReDim Preserve tempObj.Parts(grpCnt)
              tempObj.Parts(grpCnt).grpId = CurTokens(1)
            Case "f"
              ReDim Preserve tempObj.Parts(grpCnt).Faces(fCnt)
              With tempObj.Parts(grpCnt).Faces(fCnt)
                If CurTokens(1).Contains("/") Then
                  CurTokens = CurLine.Split(fSplit)
                  Select Case CurTokens.Length
                    Case 7 ' tex coords
                      .V1 = CInt(CurTokens(1))
                      .V2 = CInt(CurTokens(3))
                      .V3 = CInt(CurTokens(5))

                      .uV1 = CInt(CurTokens(2))
                      .uV2 = CInt(CurTokens(4))
                      .uV3 = CInt(CurTokens(6))
                    Case 10 ' tex coords + normals
                      .V1 = CInt(CurTokens(1))
                      .V2 = CInt(CurTokens(4))
                      .V3 = CInt(CurTokens(7))

                      .uV1 = CInt(CurTokens(2))
                      .uV2 = CInt(CurTokens(5))
                      .uV3 = CInt(CurTokens(8))

                      .nV1 = CInt(CurTokens(3))
                      .nV2 = CInt(CurTokens(6))
                      .nV3 = CInt(CurTokens(9))
                  End Select
                Else ' just geometry
                  .uV1 = CInt(CurTokens(1))
                  .uV2 = CInt(CurTokens(2))
                  .uV3 = CInt(CurTokens(3))
                End If
              End With
              fCnt += 1
            Case "mtllib"
              If Not SkipMtl Then
                tempObj.mtlFile = New StreamReader(GetDir(fn) & "\" & CurTokens(1))
                mtlFileCnt += 1
              End If
            Case "usemtl"
              If Not SkipMtl Then
                If tempObj.mtlFile IsNot Nothing Then
                  While tempObj.mtlFile.Peek <> - 1
                    CurMtlLine = tempObj.mtlFile.ReadLine
                    CurMtlTokens = CurMtlLine.Split(" ")
                    Select Case CurMtlTokens(0)
                      Case "newmtl"
                        ReDim Preserve tempObj.Parts(grpCnt).Materials(mtlCnt)
                        With tempObj.Parts(grpCnt).Materials(mtlCnt)
                          .mtlId = CurMtlTokens(1)
                        End With
                        mtlCnt += 1
                      Case "map_Ka"
                        With tempObj.Parts(grpCnt).Materials(mtlCnt - 1)
                          .texFile = GetDir(fn) & "\" & CurMtlTokens(1)
                          If Not File.Exists(.texFile) Then
                            MsgBox(
                              "Couldn't find material texture " & CurMtlTokens(1) &
                              ". It should be in the same directory as the obj file.")
                            Exit Select
                          End If
                          ReDim Preserve .Textures(texCnt)
                          Dim texData As IntPtr = 0
                          Dim tImg As Integer = 0

                          tImg = Il.ilGenImage()
                          Il.ilBindImage(tImg)
                          Il.ilLoad(Il.IL_PNG, .texFile)
                          texData = Il.ilGetData()
                          Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE)
                          With .Textures(texCnt)
                            .width = FunctionsCs.Pow2(Il.ilGetInteger(Il.IL_IMAGE_WIDTH))
                            .height = FunctionsCs.Pow2(Il.ilGetInteger(Il.IL_IMAGE_HEIGHT))
                            .bpp = Il.ilGetInteger(Il.IL_IMAGE_BPP)
                            ReDim .Data(.width*.height*4 - 1)
                            Marshal.Copy(texData, .Data, 0, .Data.Length - 1)
                          End With
                          Il.ilDeleteImage(tImg)
                        End With
                        texCnt += 1
                    End Select
                  End While
                End If
              End If
          End Select
        End While

        Return tempObj
      Else
        MsgBox("Could not find Wavefrong OBJ file " & fn & "!")
      End If
    Catch ex As Exception
      MsgBox("Error encountered in OBJ Parser." & Environment.NewLine & Environment.NewLine & "Details: " & ex.Message,
             MsgBoxStyle.Critical, "Error while parsing obj model!")
      Return Nothing
    End Try
  End Function
End Class
