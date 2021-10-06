Imports UoT.displaylist

Public Class N64DlistAssembler
  Dim RDPCompiler As New RDPGlobal
  Dim F3DEX2Compiler As New UCodeSpecific.F3DEX2
  Dim F3DEXCompiler As New UCodeSpecific.F3DEX


  Public Class RDPGlobal 'class handling RDP global commands (G_XXXX)
    Public Function TRI(Triangles As UnpackedTriangle, Output As DLCommand)
      Dim tCmdLo As UInteger = 0
      Dim tCmdHi As UInteger = 0
      With Triangles
        tCmdLo = ((.VertA >> 16) And &HFF) Or
                 ((.VertB >> 8) And &HFF) Or
                 ((.VertC >> 0) And &HFF)

        tCmdHi = 0
        If .TRI2 Then
          tCmdHi = ((.VertA >> 24) And &HFF) Or
                   ((.VertB >> 16) And &HFF) Or
                   ((.VertC >> 8) And &HFF)
        End If
      End With

      Output.Update(tCmdLo, tCmdHi)
    End Function

    Public Function SETCONSTCOLOR(ByVal Color As Color, ByRef Output As DLCommand)
      Dim low As UInteger = 0
      Dim high As UInteger = (Color.R >> 24 And &HFF) Or
                             (Color.G >> 16 And &HFF) Or
                             (Color.B >> 8 And &HFF) Or
                             (Color.A >> 0 And &HFF)

      Output.Update(low, high)
    End Function

    Public Function SETCOMBINE(ByVal CombinerFlags As UnpackedCombiner, ByRef Output As DLCommand)
      With CombinerFlags
        Dim low As UInteger = (.cA(0) << 20) Or
                              (.cC(0) << 15) Or
                              (.aA(0) << 12) Or
                              (.aC(0) << 9) Or
                              (.cA(1) << 5) Or
                              (.cC(1) << 0)
        Dim high As UInteger = (.cB(0) << 28) Or
                               (.cB(1) << 24) Or
                               (.aA(1) << 21) Or
                               (.aC(1) << 18) Or
                               (.cD(0) << 15) Or
                               (.aB(0) << 12) Or
                               (.aD(0) << 9) Or
                               (.cD(1) << 6) Or
                               (.aB(1) << 3) Or
                               (.aD(1) << 0)
        Output.Update(low, high)
      End With
    End Function
  End Class

  Public Class UCodeSpecific 'class handling ucode specific commands (F3DEX2_XXXX)
    Public Class F3DEX2 'F3DEX2
      Public Function GEOMETRYMODE(ByVal GeoModes As UnpackedGeometryMode, ByRef Output As DLCommand)
        Dim tCmd As UInteger = &H0
        With GeoModes
          If .ZBUFFER Then
            tCmd = tCmd Or RDP.G_ZBUFFER
          Else
            tCmd = tCmd Or &H0
          End If
          If .CULLBACK Then
            tCmd = tCmd Or RDP.G_CULL_BACK
          End If
          If .CULLFRONT Then
            tCmd = tCmd Or RDP.G_CULL_FRONT
          End If
          If .FOG Then
            tCmd = tCmd Or RDP.G_FOG
          End If
          If .LIGHTING Then
            tCmd = tCmd Or RDP.G_LIGHTING
          End If
          If .TEXTUREGEN Then
            tCmd = tCmd Or RDP.G_TEXTURE_GEN
          End If
          If .TEXTUREGENLINEAR Then
            tCmd = tCmd Or RDP.G_TEXTURE_GEN_LINEAR
          End If
          If .SHADINGSMOOTH Then
            tCmd = tCmd Or RDP.G_SHADING_SMOOTH
          End If
        End With
      End Function

      Public Function VTX(ByVal VtxSetup As UnpackedVtxLoad, ByRef Output As DLCommand)
      End Function

      Public Function SETOTHERMODEL(ByVal Modes As UnpackedOtherModesL, ByRef Output As DLCommand)
      End Function
    End Class

    Public Class F3DEX
    End Class
  End Class

  Public Function Compile(ByVal UCode As Integer, ByVal CommandCode As UInteger, ByVal ParamData As Object) As DLCommand _
    'father function, calls required function to build commands, returns both lo/hiwords as DLCommand struct (see structs.vb)
    Compile = Nothing

    InitNewCommand(Compile, CommandCode)

    Select Case UCode 'designed for expansion, no big plans to go beyond F3DEX2 (F3DZEX) currently, though
      Case UCodes.RDP
        Select Case CommandCode
          Case RDP.G_SETCOMBINE 'set command struct with arg0 (24-bit) and arg1 (32-bit), compiled by specific functions
            RDPCompiler.SETCOMBINE(ParamData, Compile)
          Case RDP.G_SETENVCOLOR, RDP.G_SETFOGCOLOR, RDP.G_SETPRIMCOLOR
            RDPCompiler.SETCONSTCOLOR(ParamData, Compile)
        End Select
      Case UCodes.F3DEX2
        Select Case CommandCode
          Case F3DZEX.DL

          Case F3DZEX.GEOMETRYMODE
            F3DEX2Compiler.GEOMETRYMODE(ParamData, Compile)
          Case F3DZEX.TRI1

          Case F3DZEX.TRI2

          Case F3DZEX.TEXTURE

          Case F3DZEX.SETOTHERMODE_L

          Case F3DZEX.SETOTHERMODE_H

          Case F3DZEX.ENDDL

        End Select
      Case UCodes.F3DEX
        Select Case CommandCode
          Case F3DEX.TRI2

        End Select
    End Select
  End Function
End Class
