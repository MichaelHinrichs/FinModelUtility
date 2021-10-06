Imports System.Math
Imports MathNet.Numerics.LinearAlgebra
Imports MathNet.Numerics.LinearAlgebra.Double
Imports Tao.OpenGl
Imports UoT.displaylist
Imports UoT.limbs

Public Class F3DEX2_Parser
  Public Sub New(ShaderManager As DlShaderManager)
    Me.ShaderManager = ShaderManager
  End Sub

#Region "VARIABLES"

  Public Enum Parse
    GEOMETRY = 1
    EVERYTHING = 0
  End Enum

  Public ParseMode As Integer = - 1

#Region "SHADERS & TEXTURE RELATED"

  Private N64GeometryMode As UInt32

  ''' <summary>
  '''   All 8 tile descriptors available to the given display list.
  '''
  '''   At most, two can be bound at once.
  ''' </summary>
  Private TileDescriptors(TILE_DESCRIPTOR_MAX) As TileDescriptor

  Private Const TILE_DESCRIPTOR_MAX = 7
  Private Cache As New TextureCache
  Private Tmem As New Tmem(Cache)
  Private SelectedTileDescriptor As Integer = 0

  Private UseJank As Boolean = True

  Private JankTileDescriptors(-1) As TileDescriptor
  Private JankCache As New TextureCache
  Private JankTextures(1) As Texture

  ' TODO: Support the second texture being a mipmap in LOD mode.
  ' TODO: Figure out and document how tile selectors are selected.
  ''' <summary>
  '''   Tile descriptors for the textures currently selected (AKA in use) by
  '''   the display list.
  '''
  '''   Although the RDP supports indexing 8 different tile descriptors, only
  '''   two can be bound at once! Fancier texture effects limit this down even
  '''   further, to a single bound texture.
  '''
  '''   Judging from GLideN64's source, these are selected in TEXTURE() as tile
  '''   and tile+1.
  ''' </summary>
  Private SelectedTileDescriptors(-1) As Integer

  ' TODO: Delete this field?
  Private CurrentSelectedTileDescriptor As Integer

#End Region

#Region "GEOMETRY RELATED"

  Private VertBufferOff As Integer = 0
  Private Polygons(5) As UInteger
  Private v0 As Byte = 0
  Private n0 As Byte = 0

  Private vertexCache As New VertexCache

  Private FullAlphaCombiner As Boolean = False
  Private ModColorWithAlpha As Boolean = False

  Public ShaderManager As DlShaderManager
  Public LimbMatrices As New LimbMatrices

  Structure RSPMatrix
    Dim N64Mat() As Byte
    Dim OGLMat(,) As Single
  End Structure

#End Region

#Region "COMMON Z64 DATA"

#End Region

#End Region

#Region "Hacks"

  Private FaceHack_ As IIndirectTextureHack
  Private HackEnvColor() As Byte

  Public Sub EnableHacksFor(filename As String, faceHack As IIndirectTextureHack)
    FaceHack_ = faceHack

    Dim envColor() As Byte = EnvironmentColorHacks.GetColorForObject(filename)
    HackEnvColor = envColor
  End Sub

#End Region

#Region "F3DEX2 TO OPENGL DISPLAY LIST"

  Private MatrixMap As New Dictionary(Of UInteger, Matrix(Of Double))

  Public Sub ParseDL(DL As N64DisplayList, dlManager As DlManager)
    If HackEnvColor IsNot Nothing Then
      ShaderManager.SetEnvironmentColor(HackEnvColor(0) / 255.0F,
                                        HackEnvColor(1) / 255.0F,
                                        HackEnvColor(2) / 255.0F,
                                        HackEnvColor(3) / 255.0F)
    End If

    For i As Integer = 0 To DL.Commands.Length - 1
      With DL.Commands(i)
        If ParseMode = Parse.EVERYTHING Then
          Select Case .Opcode
            Case F3DZEX.POPMTX
popmatrix:
              Debug.NotImplemented()

            Case RDP.G_SETENVCOLOR
setenvironmentcolor:
              ENVCOLOR(.CMDParams)

            Case RDP.G_SETPRIMCOLOR
setprimitivecolor:
              SETPRIMCOLOR(.CMDParams)

            Case RDP.G_SETTIMG
settextureimg:
              Dim paletteMode As Boolean = (DL.Commands(i + 1).Opcode = RDP.G_RDPTILESYNC)

              If DL.Commands(i - 1).Opcode = RDP.G_SETTILESIZE Then
                CurrentSelectedTileDescriptor = 1
                If GLExtensions.GLMultiTexture And GLExtensions.GLFragProg Then
                  ShaderManager.Params.MultiTexCoord = True
                Else
                  ShaderManager.Params.MultiTexCoord = False
                End If
                ShaderManager.Params.MultiTexture = True
              Else
                ShaderManager.Params.MultiTexture = False
                ShaderManager.Params.MultiTexCoord = False
                CurrentSelectedTileDescriptor = 0
              End If

              SETTIMG(.Low, .High, paletteMode)

            Case RDP.G_LOADTLUT
loadtexturelut:
              LOADTLUT(.High)

            Case RDP.G_LOADBLOCK
loadtexblock:
              LOADBLOCK(.Low, .High)

            Case RDP.G_LOADTILE
              LOADTILE(.Low, .High)

            Case RDP.G_SETTILESIZE
settilesize:
              SETTILESIZE(.Low, .High)

            Case RDP.G_SETTILE
settile:
              If .CMDParams(1) > 0 Then SETTILE(.Low, .High)

            Case RDP.G_SETCOMBINE
setcombiner:
              SETCOMBINE(.Low, .High)

            Case F3DZEX.TEXTURE
texture:
              TEXTURE(.Low, .High)

            Case F3DZEX.GEOMETRYMODE
geometrymode:
              GEOMETRYMODE(.Low, .High)

            Case F3DZEX.SETOTHERMODE_H
setothermodehigh:
              SETOTHERMODE_H(.Low, .High)

            Case F3DZEX.SETOTHERMODE_L
seothtermodelow:
              SETOTHERMODE_L(.Low, .High)

            Case F3DZEX.MTX
matrix:
              ' MTX(.CMDLow, .CMDHigh)
              Dim targetAddress As UInteger = .High

              Dim m As Matrix = LimbMatrices.GetMatrixAtAddress(targetAddress)
              ModelViewMatrixTransformer.Set(m)
              DlModel.SetCurrentLimbByMatrixAddress(targetAddress)

            Case F3DZEX.VTX
vertex:
              Dim nextIsNotCulling As Boolean = DL.Commands(i + 1).Opcode <> F3DZEX.CULLDL
              If nextIsNotCulling Then
                VTX(.Low, .High)
              End If

            Case F3DZEX.MODIFYVTX
modifyvertex:
              MODIFYVTX(.CMDParams)

            Case F3DZEX.TRI1
onetriangle:
              TRI1(.CMDParams)

            Case F3DZEX.TRI2
twotriangles:
              TRI2(.CMDParams)

            Case F3DZEX.QUAD
quad:
              Debug.NotImplemented()

            Case F3DZEX.DL
dl:
              ' TODO: Support jumping to another DL.
              ' TODO: Decide to continue or quit depending on pp from w0. 
              Dim address As UInteger = .High

              Dim pushDl As Boolean = IoUtil.ShiftR(.Low, 16, 8) = 0

              Dim bank As Byte
              Dim offset As UInteger
              IoUtil.SplitAddress(address, bank, offset)

              ' TODO: Attempting to jump to addresses w/ bank 0x00 and 0x0c.
              ' One of these is probably held items!
              If bank <> 0 Then
                Dim something = 0
              Else
                Debug.NotImplemented("Tried to jump to display list with address: " & address.ToString("X8"))
              End If

            Case F3DZEX.BRANCH_Z
branchz:
              Debug.NotImplemented()

            Case F3DZEX.ENDDL
enddisplaylist:
              Reset()
              Exit Sub
          End Select
        ElseIf ParseMode = Parse.GEOMETRY Then
          Select Case .Opcode
            Case F3DZEX.VTX
              GoTo vertex

            Case F3DZEX.GEOMETRYMODE
              GoTo geometrymode

            Case F3DZEX.MODIFYVTX
              GoTo modifyvertex

            Case F3DZEX.TRI1
              GoTo onetriangle

            Case F3DZEX.TRI2
              GoTo twotriangles

            Case F3DZEX.QUAD
              GoTo quad

            Case F3DZEX.DL
              GoTo dl

            Case F3DZEX.BRANCH_Z
              GoTo branchz

            Case F3DZEX.ENDDL
              GoTo enddisplaylist
          End Select
        End If
      End With
    Next
  End Sub

#Region "GEOMETRY HANDLING"

  Private Sub MTX(ByVal w0 As UInt32, ByVal w1 As UInt32)
    Dim MtxSegment As Byte = (w1 >> 24)
    Dim Address As UInteger = (w1 << 8 >> 8)
    Dim Param As Byte = (w1 And &HFF) Xor F3DZEX.MTX_PUSH

    Dim TempRSPMatrix As New RSPMatrix

    With TempRSPMatrix
      ReDim .N64Mat(&H3F)
      ReDim .OGLMat(3, 3)
    End With

    Dim MatValue(1) As Short
    Dim MtxPos As UInteger

    Select Case MtxSegment
      Case RamBanks.CurrentBank
        For i As Integer = 0 To &H3F
          TempRSPMatrix.N64Mat(i) = RamBanks.ZFileBuffer(Address + i)
        Next
      Case 2
        For i As Integer = 0 To &H3F
          TempRSPMatrix.N64Mat(i) = RamBanks.ZSceneBuffer(Address + i)
        Next
      Case &H80
        ModelViewMatrixTransformer.Pop()
        Exit Sub
      Case Else
        Exit Sub
    End Select


    For i As Integer = 0 To 3
      For j As Integer = 0 To 3
        MatValue(0) = IoUtil.ReadUInt16(TempRSPMatrix.N64Mat, MtxPos + 0)
        MatValue(1) = IoUtil.ReadUInt16(TempRSPMatrix.N64Mat, MtxPos + 32)
        TempRSPMatrix.OGLMat(i, j) = ((MatValue(0) << 16) Or MatValue(1)) * 1.0F / 65536.0F
        MtxPos += 2
      Next
    Next

    Dim gch As Runtime.InteropServices.GCHandle = Runtime.InteropServices.GCHandle.Alloc(TempRSPMatrix.OGLMat,
                                                                                         Runtime.InteropServices.
                                                                                          GCHandleType.Pinned)
    Dim mtxPtr As IntPtr = gch.AddrOfPinnedObject()

    'If Param And F3DZEX.MTX_PROJECTION > 0 Then
    '    Gl.glMatrixMode(Gl.GL_PROJECTION)
    'Else
    '    Gl.glMatrixMode(Gl.GL_MODELVIEW)
    '    If Param And F3DZEX.MTX_PUSH > 0 Then
    '        Gl.glPushMatrix()
    '    End If
    'End If
    ModelViewMatrixTransformer.Push()

    'If Param And F3DZEX.MTX_LOAD > 0 Then
    '    Gl.glLoadMatrixf(mtxPtr)
    'Else
    ' TODO Should be able to multiply this in.
    Gl.glMultMatrixf(mtxPtr)
    'End If

    gch.Free()
  End Sub

  Private Sub MODIFYVTX(ByVal CMDParams() As Byte)
    Dim i As Integer = (IoUtil.ReadUInt16(CMDParams, 2) And &HFFF) / 2
    Dim Target As Integer = CMDParams(1)

    Dim vertex As Vertex = vertexCache(i)

    Select Case Target
      Case &H10
        vertex.R = CMDParams(4)
        vertex.G = CMDParams(5)
        vertex.B = CMDParams(6)
        vertex.A = CMDParams(7)

        Dim rawNormalX As Byte = vertex.R
        Dim rawNormalY As Byte = vertex.G
        Dim rawNormalZ As Byte = vertex.B

        DlModel.UpdateVertex(i, Function(vtxParams) As VertexParams
                                  vtxParams.R = vertex.R
                                  vtxParams.G = vertex.G
                                  vtxParams.B = vertex.B
                                  vtxParams.A = vertex.A

                                  vtxParams.NormalX = ConvertByteToFloat_(rawNormalX)
                                  vtxParams.NormalY = ConvertByteToFloat_(rawNormalY)
                                  vtxParams.NormalZ = ConvertByteToFloat_(rawNormalZ)

                                  Return vtxParams
                                End Function)

        Dim normalX As Single = ConvertByteToJankFloat_(rawNormalX)
        Dim normalY As Single = ConvertByteToJankFloat_(rawNormalY)
        Dim normalZ As Single = ConvertByteToJankFloat_(rawNormalZ)
        ModelViewMatrixTransformer.ProjectNormal(normalX, normalY, normalZ)

        vertex.NormalX = normalX
        vertex.NormalY = normalY
        vertex.NormalZ = normalZ
      Case &H14
        vertex.U = CShort(IoUtil.ReadUInt16(CMDParams, 4))
        vertex.V = CShort(IoUtil.ReadUInt16(CMDParams, 6))

        DlModel.UpdateVertex(i, Function(vtxParams) As VertexParams
                                  vtxParams.U = vertex.U
                                  vtxParams.V = vertex.V

                                  Return vtxParams
                                End Function)
    End Select

    vertexCache(i) = vertex
  End Sub

  Private Function ConvertByteToJankFloat_(value As Byte) As Single
    Dim total As Byte = 255
    Dim upper As Byte = 128
    Dim lower As Byte = total - upper

    If value > lower Then
      Return (value - lower) / upper
    Else
      Return -value / lower
    End If
  End Function

  Private Function ConvertByteToFloat_(value As Byte) As Single
    Return ConvertByteToJankFloat_(value)
    Return -1 + 2 * (value / 255.0F)
  End Function


  Private Sub GEOMETRYMODE(ByVal w0 As UInt32, ByVal w1 As UInt32)
    Dim MCLEAR As UInt32 = w0
    Dim MSET As UInt32 = w1 And &HFFFFFF

    N64GeometryMode = N64GeometryMode And MCLEAR
    N64GeometryMode = N64GeometryMode Or MSET

    If (N64GeometryMode And RDP.G_TEXTURE_GEN) Then
      ShaderManager.Params.EnableSphericalUv = True
    Else
      ShaderManager.Params.EnableSphericalUv = False
    End If

    If (N64GeometryMode And RDP.G_TEXTURE_GEN_LINEAR) Then
      ShaderManager.Params.EnableLinearUv = True
    Else
      ShaderManager.Params.EnableLinearUv = False
    End If

    If N64GeometryMode And RDP.G_CULL_BOTH Then
      Gl.glEnable(Gl.GL_CULL_FACE)
      If N64GeometryMode And RDP.G_CULL_BACK Then
        Gl.glCullFace(Gl.GL_BACK)
      Else
        Gl.glDisable(Gl.GL_CULL_FACE)
      End If
    Else
      Gl.glDisable(Gl.GL_CULL_FACE)
    End If

    If ParseMode = Parse.EVERYTHING Then
      If N64GeometryMode And RDP.G_LIGHTING Then
        ShaderManager.Params.EnableLighting = True
        Gl.glEnable(Gl.GL_NORMALIZE)
        Gl.glEnable(Gl.GL_LIGHTING)
      Else
        ShaderManager.Params.EnableLighting = False
        Gl.glDisable(Gl.GL_NORMALIZE)
        Gl.glDisable(Gl.GL_LIGHTING)
      End If
    End If
  End Sub

  Private Sub SETOTHERMODE_H(ByVal w0 As UInt32, ByVal w1 As UInt32)
    Dim MDSFT As Byte = (32 - (w0 << 4 >> 4) - 1)
    Select Case MDSFT
      Case RDP.G_MDSFT_CYCLETYPE
        Gdp.CycleMode = w1 >> RDP.G_MDSFT_CYCLETYPE
      Case Else
    End Select
  End Sub

  Private Sub SETOTHERMODE_L(ByVal w0 As UInt32, ByVal w1 As UInt32)
    Dim AA_EN As Boolean = (w1 And &H8) > 0
    Dim Z_CMP As Boolean = (w1 And &H10) > 0
    Dim Z_UPD As Boolean = (w1 And &H20) > 0
    Dim IM_RD As Boolean = (w1 And &H40) > 0
    Dim CLR_ON_CVG As Boolean = (w1 And &H80) > 0
    Dim CVG_DST_WRAP As Boolean = (w1 And &H100) > 0
    Dim CVG_DST_FULL As Boolean = (w1 And &H200) > 0
    Dim CVG_DST_SAVE As Boolean = (w1 And &H300) > 0
    Dim ZMODE_INTER As Boolean = (w1 And &H400) > 0
    Dim ZMODE_XLU As Boolean = (w1 And &H800) > 0
    Dim ZMODE_DEC As Boolean = (w1 And &HC00) > 0
    Dim CVG_X_ALPHA As Boolean = (w1 And &H1000) > 0
    Dim ALPHA_CVG_SEL As Boolean = (w1 And &H2000) > 0
    Dim FORCE_BL As Boolean = (w1 And &H4000) > 0

    Dim MDSFT As Byte = (32 - (w0 << 4 >> 4) - 1)

    ' TODO: Handle other ZMODEs, as these are probably why translucent
    ' materials are broken.
    Select Case MDSFT
      Case 3 'rendermode
        If ZMODE_DEC Then
          Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
          Gl.glPolygonOffset(-7, -7)
        Else
          Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
        End If
        If CVG_X_ALPHA Or ALPHA_CVG_SEL Then
          Gl.glAlphaFunc(Gl.GL_GEQUAL, 0.2F)
          Gl.glEnable(Gl.GL_ALPHA_TEST)
          Gl.glDisable(Gl.GL_BLEND)
        ElseIf FORCE_BL Then
          ForceBlending(w0, w1)
        End If
        If Z_CMP Then
          Gl.glEnable(Gl.GL_DEPTH_TEST)
        Else
          Gl.glDisable(Gl.GL_DEPTH_TEST)
        End If
      Case Else
        MsgBox("Unhandled SETOTHERMODE_L MDSFT: 0x" & MDSFT.ToString & "?")
    End Select
  End Sub

  Private Function GetSelectedTileDescriptor(index As Integer) As TileDescriptor
    If UseJank Then
      Return JankTileDescriptors(SelectedTileDescriptors(index))
    Else
      Return TileDescriptors((SelectedTileDescriptor + index) Mod 8)
    End If
  End Function

  Private Sub SetSelectedTileDescriptor(index As Integer, ByVal tileDescriptor As TileDescriptor)
    If UseJank Then
      JankTileDescriptors(SelectedTileDescriptors(index)) = tileDescriptor
    Else
      TileDescriptors((SelectedTileDescriptor + index) Mod 8) = tileDescriptor
    End If
  End Sub

  Private Function GetTexture(index As Integer) As Texture
    If UseJank Then
      Dim texture As Texture = JankTextures(index)
      Dim tileDescriptor As TileDescriptor = JankTileDescriptors(index)

      If texture IsNot Nothing Then
        If texture.TileDescriptor.Uuid = tileDescriptor.Uuid Then
          Return texture
        End If
      End If

      texture = JankCache(tileDescriptor)
      JankTextures(index) = texture

      Return texture
    End If
    Throw New NotImplementedException()
  End Function

  Private Function SearchTexCache(ByVal tileDescriptor As TileDescriptor) As Texture
    If UseJank Then
      Return JankCache(tileDescriptor)
    Else
      Return Cache(tileDescriptor)
    End If
  End Function

  Private Sub VTX(ByVal w0 As UInt32, ByVal w1 As UInt32)
    Dim n0 As UInteger = IoUtil.ShiftR(w0, 12, 8)
    Dim v0 As UInteger = (IoUtil.ShiftR(w0, 0, 8) >> 1) - n0

    Dim VertBufferOff As UInteger
    Dim VertexSeg As UInteger
    IoUtil.SplitAddress(w1, VertexSeg, VertBufferOff)

    Select Case VertexSeg
      Case RamBanks.CurrentBank
        FillVertexCache(RamBanks.ZFileBuffer, VertexSeg, VertBufferOff, n0, v0)

      Case 2
        FillVertexCache(RamBanks.ZSceneBuffer, VertexSeg, VertBufferOff, n0, v0)
    End Select
  End Sub

  Private Function FillVertexCache(ByVal Data As IList(Of Byte), ByVal DataSource As Byte,
                                   ByVal Offset As Integer, ByVal n0 As Integer, ByVal v0 As Integer)
    Select Case DataSource
      Case RamBanks.CurrentBank
        For i2 As Integer = v0 To (v0 + n0) - 1
          Dim rawX As Double = IoUtil.ReadInt16(Data, Offset)
          Dim rawY As Double = IoUtil.ReadInt16(Data, Offset + 2)
          Dim rawZ As Double = IoUtil.ReadInt16(Data, Offset + 4)

          Dim x As Double = rawX
          Dim y As Double = rawY
          Dim z As Double = rawZ

          ModelViewMatrixTransformer.ProjectVertex(x, y, z)

          Dim u As Short = CShort(IoUtil.ReadUInt16(Data, Offset + 8))
          Dim v As Short = CShort(IoUtil.ReadUInt16(Data, Offset + 10))
          Dim r As Byte = Data(Offset + 12)
          Dim g As Byte = Data(Offset + 13)
          Dim b As Byte = Data(Offset + 14)
          Dim a As Byte = Data(Offset + 15)

          Dim rawNormalX As Byte = r
          Dim rawNormalY As Byte = g
          Dim rawNormalZ As Byte = b

          Dim normalX As Single = ConvertByteToJankFloat_(rawNormalX)
          Dim normalY As Single = ConvertByteToJankFloat_(rawNormalY)
          Dim normalZ As Single = ConvertByteToJankFloat_(rawNormalZ)
          ModelViewMatrixTransformer.ProjectNormal(normalX, normalY, normalZ)

          Dim newVertex As Vertex

          newVertex.Populated = True

          'Vertex x(l)-y(w)-z(d) coordinates
          newVertex.X = x
          newVertex.Y = y
          newVertex.Z = z

          'Texture coordinates
          newVertex.U = u
          newVertex.V = v

          'Vertex colors
          newVertex.R = r
          newVertex.G = g
          newVertex.B = b
          newVertex.A = a

          newVertex.NormalX = normalX
          newVertex.NormalY = normalY
          newVertex.NormalZ = normalZ

          vertexCache(i2) = newVertex

          DlModel.UpdateVertex(i2, Function(vertex) As VertexParams
                                     vertex.X = rawX
                                     vertex.Y = rawY
                                     vertex.Z = rawZ

                                     vertex.U = u
                                     vertex.V = v

                                     vertex.NormalX = ConvertByteToFloat_(rawNormalX)
                                     vertex.NormalY = ConvertByteToFloat_(rawNormalY)
                                     vertex.NormalZ = ConvertByteToFloat_(rawNormalZ)

                                     vertex.R = r
                                     vertex.G = g
                                     vertex.B = b
                                     vertex.A = a

                                     Return vertex
                                   End Function)
          Offset += 16
        Next
      Case Else
        MsgBox("Trying to load vertices from bank 0x" & Hex(DataSource) & "?")
    End Select
  End Function

  ''' <summary>
  '''   Prepares to draw a triangle by setting GL params and loading any
  '''   pending textures. This was previously called in VTX(), but it turns out
  '''   it MUST be called before the TRI() methods to fix textures!
  '''
  '''   (This idea was shamelessly taken from GLideN64.)
  ''' </summary>
  Private Sub PrepareDrawTriangle_()
    If ParseMode = Parse.EVERYTHING Then
      Gl.glEnable(Gl.GL_TEXTURE_2D)
      Dim texture0 As Texture = GetTexture(0)
      Dim texture1 As Texture = Nothing

      If texture0 Is Nothing Then
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Dim tileDescriptor0 As TileDescriptor = GetSelectedTileDescriptor(0)
        Dim targetBuffer0 As IBank = RamBanks.GetBankByIndex(tileDescriptor0.ImageBank)
        If targetBuffer0 IsNot Nothing Then
          LoadTex(targetBuffer0, 0)

          texture0 = SearchTexCache(tileDescriptor0)
        End If
      End If

      If ShaderManager.Params.MultiTexture Then
        Gl.glActiveTexture(Gl.GL_TEXTURE1)
        texture1 = GetTexture(1)

        If texture1 Is Nothing Then
          Dim tileDescriptor1 As TileDescriptor = GetSelectedTileDescriptor(1)
          Select Case tileDescriptor1.ImageBank
            Case RamBanks.CurrentBank
              LoadTex(RamBanks.ZFileBuffer, 1)
            Case 2
              LoadTex(RamBanks.ZSceneBuffer, 1)
            Case 4
              LoadTex(RamBanks.GetBankByIndex(4), 0)
            Case 5
              LoadTex(RamBanks.GetBankByIndex(5), 0)
            Case Else
              ' TODO: Should throw an error for unsupported banks.
          End Select

          texture1 = SearchTexCache(tileDescriptor1)
        End If
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
      End If
      Gl.glDisable(Gl.GL_TEXTURE_2D)

      ShaderManager.PassValuesToShader()
      ShaderManager.BindTextures(texture0, texture1)
    End If
  End Sub


  ''' <summary>
  '''   Loads a subset of a texture into memory. This *seems* to be specified
  '''   as a region starting at an upper-left coordinate and going on for n
  '''   "texels".
  '''
  '''   A rate param dxt can also be provided; this specifies the speed at
  '''   which a counter is increased per 64 bits read. Every time this counter
  '''   turns over to a new integer, the scanline increments.
  ''' </summary>
  Private Sub LOADBLOCK(w0 As UInt32, w1 As UInt32)
    Dim uls As UShort = IoUtil.ShiftR(w0, 12, 12)
    Dim ult As UShort = IoUtil.ShiftR(w0, 0, 12)
    Dim tileDescriptorIndex As Byte = IoUtil.ShiftR(w1, 24, 3)
    Dim texelsMinusOne As UShort = IoUtil.ShiftR(w1, 12, 12)
    Dim dxt As UShort = IoUtil.ShiftR(w1, 0, 12)

    Dim tileDescriptor As TileDescriptor = TileDescriptors(tileDescriptorIndex)
    Tmem.LoadBlock(tileDescriptor, uls, ult, texelsMinusOne, dxt, TimgArgs)
    TileDescriptors(tileDescriptorIndex) = tileDescriptor
  End Sub

  ''' <summary>
  '''   Loads a subset of a texture into memory, specified as a region between
  '''   an upper-left coordinate and lower-right coordinate.
  ''' </summary>
  Private Sub LOADTILE(w0 As UInt32, w1 As UInt32)
    Dim uls As UShort = IoUtil.ShiftR(w0, 12, 12) >> 2
    Dim ult As UShort = IoUtil.ShiftR(w0, 0, 12) >> 2
    Dim tileDescriptorIndex As Byte = IoUtil.ShiftR(w1, 24, 3)
    Dim lrs As UShort = IoUtil.ShiftR(w1, 12, 12) >> 2
    Dim lrt As UShort = IoUtil.ShiftR(w1, 0, 12) >> 2

    Dim tileDescriptor As TileDescriptor = TileDescriptors(tileDescriptorIndex)
    Tmem.LoadTile(tileDescriptor, uls, ult, lrs, lrt, TimgArgs)
    TileDescriptors(tileDescriptorIndex) = tileDescriptor
  End Sub

  Private Sub BindTextureUvs(ByRef vertex As IVertex)
    ' TODO: These lookups are slow, cache these.
    Dim texture0 As Texture = GetTexture(0)
    Dim tileDescriptor0 As TileDescriptor = Nothing
    If texture0 IsNot Nothing Then
      tileDescriptor0 = texture0.TileDescriptor
    End If

    Dim texture1 As Texture = GetTexture(1)
    Dim tileDescriptor1 As TileDescriptor = Nothing
    If texture1 IsNot Nothing Then
      tileDescriptor1 = texture1.TileDescriptor
    End If

    ShaderManager.BindTextureUvs(vertex, tileDescriptor0, tileDescriptor1)
  End Sub


  Private Sub TRI1(ByVal CMDParams() As Byte)
    PrepareDrawTriangle_()

    Try
      ' TODO: Handle reordering based on value of flag.
      Polygons(0) = CMDParams(1) >> 1
      Polygons(1) = CMDParams(2) >> 1
      Polygons(2) = CMDParams(3) >> 1

      For Each polygon As Integer In Polygons
        Dim vertex As Vertex = vertexCache(polygon)
        If Not vertex.Populated Then
          Return
        End If
      Next

      Dim texture0 As Texture = GetTexture(0)
      Dim texture1 As Texture = GetTexture(1)
      DlModel.AddTriangle(ShaderManager.Params, Polygons(0), Polygons(1), Polygons(2), texture0, texture1)

      If ParseMode = Parse.EVERYTHING Then
        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As Integer = 0 To 2
          Dim vertex As Vertex = vertexCache(Polygons(i))
          BindTextureUvs(vertex)

          ShaderManager.PassInVertexAttribs(vertex)
          Gl.glVertex3d(vertex.X, vertex.Y, vertex.Z)
        Next
        Gl.glEnd()
      Else
        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As Integer = 0 To 2
          Dim vertex As Vertex = vertexCache(Polygons(i))
          Gl.glVertex3d(vertex.X, vertex.Y, vertex.Z)
        Next
        Gl.glEnd()
      End If
    Catch ex As Exception
      MsgBox("Error TRI1 - out of bounds!" & Environment.NewLine & Environment.NewLine & ex.Message,
             MsgBoxStyle.Critical, "Error")
    End Try
  End Sub

  Private Sub TRI2(ByVal CMDParams() As Byte)
    PrepareDrawTriangle_()

    Try
      ' TODO: Handle reordering based on value of flag.
      Polygons(0) = CMDParams(1) >> 1
      Polygons(1) = CMDParams(2) >> 1
      Polygons(2) = CMDParams(3) >> 1
      Polygons(3) = CMDParams(5) >> 1
      Polygons(4) = CMDParams(6) >> 1
      Polygons(5) = CMDParams(7) >> 1

      For Each polygon As Integer In Polygons
        Dim vertex As Vertex = vertexCache(polygon)
        If Not vertex.Populated Then
          Return
        End If
      Next

      Dim texture0 As Texture = GetTexture(0)
      Dim texture1 As Texture = GetTexture(1)
      DlModel.AddTriangle(ShaderManager.Params, Polygons(0), Polygons(1), Polygons(2), texture0, texture1)
      DlModel.AddTriangle(ShaderManager.Params, Polygons(3), Polygons(4), Polygons(5), texture0, texture1)

      If ParseMode = Parse.EVERYTHING Then
        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As Integer = 0 To 5
          Dim vertex As Vertex = vertexCache(Polygons(i))
          BindTextureUvs(vertex)

          ShaderManager.PassInVertexAttribs(vertex)
          Gl.glVertex3d(vertex.X, vertex.Y, vertex.Z)
        Next
        Gl.glEnd()
      Else
        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As Integer = 0 To 5
          Dim vertex As Vertex = vertexCache(Polygons(i))
          Gl.glVertex3d(vertex.X, vertex.Y, vertex.Z)
        Next
        Gl.glEnd()
      End If
    Catch ex As Exception
      MsgBox("Error TRI2 - out of bounds!" & Environment.NewLine & Environment.NewLine & ex.Message,
             MsgBoxStyle.Critical, "Error")
    End Try
  End Sub

#End Region

#Region "TEXTURE HANDLING"

  Private TimgArgs As New TimgArgs

  Private Sub SETTIMG(w0 As UInt32, w1 As UInt32, ByVal paletteMode As Boolean)
    Dim address As UInt32 = w1

    If FaceHack_ IsNot Nothing Then
      address = FaceHack_.MapTextureAddress(address)
    End If

    Dim tmpBank As Integer
    Dim tmpOff As Integer
    IoUtil.SplitAddress(address, tmpBank, tmpOff)

    TimgArgs.ColorFormat = ColorFormatUtil.Parse(IoUtil.ShiftR(w0, 21, 3))
    TimgArgs.BitSize = BitSizeUtil.Parse(IoUtil.ShiftR(w0, 19, 2))
    TimgArgs.Width = IoUtil.ShiftR(w0, 0, 12) + 1
    TimgArgs.Address = address

    ' TODO: Delete the below logic.
    If UseJank Then
      If paletteMode Then
        Dim tileDescriptor As TileDescriptor = GetSelectedTileDescriptor(0)
        tileDescriptor.PaletteOffset = tmpOff
        tileDescriptor.PaletteBank = tmpBank
        SetSelectedTileDescriptor(0, tileDescriptor)
      Else
        Dim tileDescriptor As TileDescriptor = GetSelectedTileDescriptor(CurrentSelectedTileDescriptor)
        tileDescriptor.Address = address
        tileDescriptor.Offset = tmpOff
        tileDescriptor.ImageBank = tmpBank
        SetSelectedTileDescriptor(CurrentSelectedTileDescriptor, tileDescriptor)
      End If
    End If
  End Sub


  Private Function SETTILE(ByVal w0 As UInt32, ByVal w1 As UInt32)
    Dim tileDescriptorIndex As Integer = IoUtil.ShiftR(w1, 24, 3)
    Dim tileDescriptor As TileDescriptor = TileDescriptors(tileDescriptorIndex)
    SetTile_(tileDescriptor, w0, w1)

    ' TODO: Remove this struct logic.
    TileDescriptors(tileDescriptorIndex) = tileDescriptor

    ' TODO: Delete this logic.
    If UseJank Then
      Dim jankTileDescriptor As TileDescriptor = GetSelectedTileDescriptor(CurrentSelectedTileDescriptor)
      SetTile_(jankTileDescriptor, w0, w1)

      ' TODO: Remove this struct logic.
      SetSelectedTileDescriptor(CurrentSelectedTileDescriptor, jankTileDescriptor)
    End If
  End Function

  Private Sub SetTile_(ByRef tileDescriptor As TileDescriptor, ByVal w0 As UInt32, ByVal w1 As UInt32)
    With tileDescriptor
      ' TODO: Delete this.
      .JankFormat = w0 >> 16
      .ColorFormat = ColorFormatUtil.Parse(IoUtil.ShiftR(w0, 21, 3))
      .BitSize = BitSizeUtil.Parse(IoUtil.ShiftR(w0, 19, 2))
      .LineSize = IoUtil.ShiftR(w0, 9, 9)
      .TmemOffset = IoUtil.ShiftR(w0, 0, 9)
      .Palette = IoUtil.ShiftR(w1, 20, 4)
      .CMT = IoUtil.ShiftR(w1, 18, 2)
      .CMS = IoUtil.ShiftR(w1, 8, 2)

      Dim maskS As Integer = IoUtil.ShiftR(w1, 4, 4)
      .MaskS = maskS
      .OriginalMaskS = maskS

      Dim maskT As Integer = IoUtil.ShiftR(w1, 14, 4)
      .MaskT = maskT
      .OriginalMaskT = maskT

      .TShiftS = IoUtil.ShiftR(w1, 0, 4)
      .TShiftT = IoUtil.ShiftR(w1, 10, 4)
    End With

    ' TODO: GLideN64 does a lookup into the selected tiles here?
  End Sub


  Private Sub SETTILESIZE(ByVal w0 As UInt32, ByVal w1 As UInt32)
    Dim tileDescriptorIndex As Integer = IoUtil.ShiftR(w1, 24, 3)
    Dim tileDescriptor As TileDescriptor = TileDescriptors(tileDescriptorIndex)
    SetTileSize_(tileDescriptor, w0, w1, False)

    ' TODO: Remove this struct logic.
    TileDescriptors(tileDescriptorIndex) = tileDescriptor

    ' TODO: Delete this logic.
    If UseJank Then
      Dim jankTileDescriptor As TileDescriptor = GetSelectedTileDescriptor(CurrentSelectedTileDescriptor)
      SetTileSize_(jankTileDescriptor, w0, w1, True)

      ' TODO: Remove this struct logic.
      SetSelectedTileDescriptor(CurrentSelectedTileDescriptor, jankTileDescriptor)
    End If
  End Sub

  Private Sub SetTileSize_(ByRef tileDescriptor As TileDescriptor, w0 As UInt32, w1 As UInt32, calcSize As Boolean)
    With tileDescriptor
      .ULS = (w0 And &HFFF000) >> 14
      .ULT = (w0 And &HFFF) >> 2
      .LRS = (w1 And &HFFF000) >> 14
      .LRT = (w1 And &HFFF) >> 2
      .Width = ((.LRS - .ULS) + 1)
      .Height = ((.LRT - .ULT) + 1)
      .TexBytes = (.Width * .Height) * 2
      If .TexBytes >> 16 = &HFFFF Then
        .TexBytes = (.TexBytes << 16 >> 16) * 2
      End If
    End With

    ' TODO: Delete this logic.
    If calcSize Then
      CalculateTexSize_(tileDescriptor)
    End If
  End Sub

  Private Sub CalculateTexSize_(ByRef tileDescriptor As TileDescriptor)
    With tileDescriptor
      Dim MaxTexel As UInteger = 0
      Dim Line_Shift As UInteger = 0
      Select Case .JankFormat
        Case 0, &H40
          MaxTexel = 4096
          Line_Shift = 4
        Case &H60, &H80
          MaxTexel = 8192
          Line_Shift = 4
        Case &H8, &H48
          MaxTexel = 2048
          Line_Shift = 3
        Case &H68, &H88
          MaxTexel = 4096
          Line_Shift = 3
        Case &H10, &H70
          MaxTexel = 2048
          Line_Shift = 2
        Case &H50, &H90
          MaxTexel = 2048
          Line_Shift = 0
        Case &H18
          MaxTexel = 1024
          Line_Shift = 2
      End Select

      Dim Line_Width As UInteger = .LineSize << Line_Shift

      Dim Tile_Width As UInteger = .LRS - .ULS + 1
      Dim Tile_Height As UInteger = .LRT - .ULT + 1

      Dim Mask_Width As UInteger = 1 << .MaskS
      Dim Mask_Height As UInteger = 1 << .MaskT

      Dim Line_Height As UInteger = 0
      If Line_Width > 0 Then Line_Height = Min(MaxTexel / Line_Width, Tile_Height)

      If .MaskS > 0 And ((Mask_Width * Mask_Height) <= MaxTexel) Then
        .Width = Mask_Width
      ElseIf ((Tile_Width * Tile_Height) <= MaxTexel) Then
        .Width = Tile_Width
      Else
        .Width = Line_Width
      End If
      If .MaskT > 0 And ((Mask_Width * Mask_Height) <= MaxTexel) Then
        .Height = Mask_Height
      ElseIf ((Tile_Width * Tile_Height) <= MaxTexel) Then
        .Height = Tile_Height
      Else
        .Height = Line_Height
      End If

      Dim Clamp_Width As UInteger = 0
      Dim Clamp_Height As UInteger = 0
      If .CMS = 1 Then
        Clamp_Width = Tile_Width
      Else
        Clamp_Width = .Width
      End If
      If .CMT = 1 Then
        Clamp_Height = Tile_Height
      Else
        Clamp_Height = .Height
      End If

      If Mask_Width > .Width Then
        .MaskS = FunctionsCs.PowOf(.Width)
        Mask_Width = 1 << .MaskS
      End If
      If Mask_Height > .Height Then
        .MaskT = FunctionsCs.PowOf(.Height)
        Mask_Height = 1 << .MaskT
      End If

      If .CMS = 2 Or .CMS = 3 Then
        .LoadWidth = FunctionsCs.Pow2(Clamp_Width)
      ElseIf .CMS = 1 Then
        .LoadWidth = FunctionsCs.Pow2(Mask_Width)
      Else
        .LoadWidth = FunctionsCs.Pow2(.Width)
      End If

      If .CMT = 2 Or .CMT = 3 Then
        .LoadHeight = FunctionsCs.Pow2(Clamp_Height)
      ElseIf .CMT = 1 Then
        .LoadHeight = FunctionsCs.Pow2(Mask_Height)
      Else
        .LoadHeight = FunctionsCs.Pow2(.Height)
      End If

      .ShiftS = 1.0F
      .ShiftT = 1.0F

      If (.TShiftS > 10) Then
        .ShiftS = (1 << (16 - .TShiftS))
      ElseIf (.TShiftS > 0) Then
        .ShiftS /= (1 << .TShiftS)
      End If

      If (.TShiftT > 10) Then
        .ShiftT = (1 << (16 - .TShiftT))
      ElseIf (.TShiftT > 0) Then
        .ShiftT /= (1 << .TShiftT)
      End If

      .TextureHRatio = ((.T_Scale * .ShiftT) / 32 / .LoadHeight)
      .TextureWRatio = ((.S_Scale * .ShiftS) / 32 / .LoadWidth)
    End With
  End Sub


  Private Sub LOADTLUT(ByVal w1 As UInt32)
    Dim tileDescriptorIndex As Integer = IoUtil.ShiftR(w1, 24, 3)
    Dim tileDescriptor As TileDescriptor = TileDescriptors(tileDescriptorIndex)
    LoadTlut_(tileDescriptor, w1)

    ' TODO: Remove this struct logic.
    TileDescriptors(tileDescriptorIndex) = tileDescriptor

    ' TODO: Delete this logic.
    If UseJank Then
      ' TODO: Delete this logic.
      Dim jankTileDescriptor As TileDescriptor = GetSelectedTileDescriptor(0)
      LoadTlut_(jankTileDescriptor, w1)

      ' TODO: Remove this struct logic.
      SetSelectedTileDescriptor(0, jankTileDescriptor)
    End If
  End Sub

  Private Sub LoadTlut_(ByRef tileDescriptor As TileDescriptor, w1 As UInt32)
    With tileDescriptor
      Dim paletteSizeMinus1 As Integer = IoUtil.ShiftR(w1, 12, 12) >> 2

      Dim palette16(paletteSizeMinus1) As UShort
      Select Case .PaletteBank
        Case RamBanks.CurrentBank
          For i As Integer = 0 To paletteSizeMinus1
            palette16(i) = IoUtil.ReadUInt16(RamBanks.ZFileBuffer, .PaletteOffset + 2 * i)
          Next
        Case 2
          For i As Integer = 0 To paletteSizeMinus1
            palette16(i) = IoUtil.ReadUInt16(RamBanks.ZSceneBuffer, .PaletteOffset + 2 * i)
          Next
      End Select

      ReDim .Palette32(paletteSizeMinus1)
      For i As Integer = 0 To paletteSizeMinus1
        Dim RGBA5551 As UShort = palette16(i)
        .Palette32(i).r = (RGBA5551 And &HF800) >> 8
        .Palette32(i).g = ((RGBA5551 And &H7C0) << 5) >> 8
        .Palette32(i).b = ((RGBA5551 And &H3E) << 18) >> 16
        If RGBA5551 And 1 Then .Palette32(i).a = 255 Else .Palette32(i).a = 0
      Next
    End With
  End Sub


  Private Function LoadTex(Data As IList(Of Byte), ByVal ID As UInteger) As Integer
    Dim tileDescriptor As TileDescriptor = GetSelectedTileDescriptor(ID)

    If UseJank Then
      Dim generator As New OglTextureConverter
      generator.GenerateAndAddToCache(Data, tileDescriptor.Offset, tileDescriptor,
                                      GetSelectedTileDescriptor(0).Palette32, JankCache, True)
      JankTextures(ID) = JankCache(tileDescriptor)
    Else
      Tmem.LoadTexture(tileDescriptor)
    End If

    SetSelectedTileDescriptor(ID, tileDescriptor)
  End Function

  Private Sub TEXTURE(w0 As UInt32, w1 As UInt32)
    ' TODO: Support setting max # of mipmap levels.

    Dim tileDescriptorIndex As Integer = IoUtil.ShiftR(w0, 8, 3)
    Dim tileDescriptor As TileDescriptor = TileDescriptors(tileDescriptorIndex)
    With tileDescriptor
      .Enabled = IoUtil.ShiftR(w0, 1, 7) <> 0

      If .Enabled Then
        .S_Scale = IoUtil.ShiftR(w1, 16, 16)
        .T_Scale = IoUtil.ShiftR(w1, 0, 16)
      End If
    End With

    TileDescriptors(tileDescriptorIndex) = tileDescriptor

    SelectedTileDescriptor = tileDescriptorIndex

    ' TODO: Delete this logic.
    If UseJank Then
      For i As Integer = 0 To 1
        Dim jankTileDescriptor As TileDescriptor = GetSelectedTileDescriptor(i)
        If IoUtil.ShiftR(w1, 16, 16) < &HFFFF Then _
          jankTileDescriptor.S_Scale = IoUtil.Fixed2Float(IoUtil.ShiftR(w1, 16, 16), 16) Else _
          jankTileDescriptor.S_Scale = 1.0F
        If IoUtil.ShiftR(w1, 0, 16) < &HFFFF Then _
          jankTileDescriptor.T_Scale = IoUtil.Fixed2Float(IoUtil.ShiftR(w1, 0, 16), 16) Else _
          jankTileDescriptor.T_Scale = 1.0F
        SetSelectedTileDescriptor(i, jankTileDescriptor)
      Next
    End If
  End Sub

#End Region

#Region "Color Combiner"

  Private Sub SETCOMBINE(ByVal w0 As UInt32, ByVal w1 As UInt32)
    ShaderManager.SetCombine(w0, w1)
  End Sub

  Private Sub ForceBlending(ByVal c1 As UInteger, ByVal c2 As UInteger)
    Gl.glEnable(Gl.GL_BLEND)
    Gl.glDisable(Gl.GL_ALPHA_TEST)

    Dim GLsrcFactor As Integer = Gl.GL_SRC_ALPHA
    Dim GLdstFactor As Integer = Gl.GL_ONE_MINUS_SRC_ALPHA

    Gl.glBlendFunc(GLsrcFactor, GLdstFactor)
    Gl.glAlphaFunc(Gl.GL_GREATER, 0.5F)
  End Sub

  Private Sub SETFOGCOLOR(ByVal CMDParams() As Byte)
    ShaderManager.SetFogColor(CMDParams(4)/255,
                              CMDParams(5)/255,
                              CMDParams(6)/255,
                              CMDParams(7)/255)
  End Sub

  Private Sub ENVCOLOR(ByVal CMDParams() As Byte)
    ShaderManager.SetEnvironmentColor(CMDParams(4)/255,
                                      CMDParams(5)/255,
                                      CMDParams(6)/255,
                                      CMDParams(7)/255)
  End Sub

  Private Sub SETPRIMCOLOR(ByVal CMDParams() As Byte)
    ShaderManager.SetPrimaryColor(CMDParams(2)/255,
                                  CMDParams(3)/255,
                                  CMDParams(4)/255,
                                  CMDParams(5)/255,
                                  CMDParams(6)/255,
                                  CMDParams(7)/255)
  End Sub

  Private Sub SETBLENDCOLOR(ByVal CMDParams() As Byte)
    ShaderManager.SetBlendColor(CMDParams(4)/255,
                                CMDParams(5)/255,
                                CMDParams(6)/255,
                                CMDParams(7)/255)
  End Sub

#End Region

#Region "STATE CHANGES"

  Public Sub Initialize()
    Reset()
    KillTexCache()
    MatrixMap.Clear()
    vertexCache.Reset()
    ShaderManager.Reset()
  End Sub

  Public Sub KillTexCache()
    Cache.Clear()
    JankCache.Clear()
  End Sub

  Public Sub Reset()
    Gl.glFinish()

    ReDim TileDescriptors(TILE_DESCRIPTOR_MAX)
    For i As Integer = 0 To TILE_DESCRIPTOR_MAX
      TileDescriptors(i).S_Scale = 1.0F
      TileDescriptors(i).T_Scale = 1.0F
    Next

    ReDim JankTileDescriptors(1)
    ReDim SelectedTileDescriptors(1)
    For i As Integer = 0 To 1
      JankTileDescriptors(i).S_Scale = 1.0F
      JankTileDescriptors(i).T_Scale = 1.0F
      SelectedTileDescriptors(i) = i
    Next
  End Sub

#End Region

#End Region
End Class
