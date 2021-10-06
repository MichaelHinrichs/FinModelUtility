Imports UoT.displaylist

Public Class CombinerEditor
  Private CombinerColors As UnpackedCombiner
  Private CombinerColorsCopy As UnpackedCombiner
  Private CompiledCmb As New DLCommand

  Private Sub Decode(ByVal MUXS0 As UInteger, ByVal MUXS1 As UInteger)
    DLParser.ShaderManager.UnpackMUX(MUXS0, MUXS1, CombinerColors)
    With CombinerColors
      cA0.SelectedIndex = .cA(0)
      cA1.SelectedIndex = .cA(1)

      cB0.SelectedIndex = .cB(0)
      cB1.SelectedIndex = .cB(1)

      cC0.SelectedIndex = .cC(0)
      cC1.SelectedIndex = .cC(1)

      cD0.SelectedIndex = .cD(0)
      cD1.SelectedIndex = .cD(1)

      aA0.SelectedIndex = .aA(0)
      aA1.SelectedIndex = .aA(1)

      aB0.SelectedIndex = .aB(0)
      aB1.SelectedIndex = .aB(1)

      aC0.SelectedIndex = .aC(0)
      aC1.SelectedIndex = .aC(1)

      aD0.SelectedIndex = .aD(0)
      aD1.SelectedIndex = .aD(1)
    End With
  End Sub


  Private Sub UpdateEnvColor(ByVal Cmd As DLCommand)
    If Cmd.High > 0 Then
      Dim tempEnv As New Color4UByte
      tempEnv.r = Cmd.CMDParams(4)
      tempEnv.g = Cmd.CMDParams(5)
      tempEnv.b = Cmd.CMDParams(6)
      tempEnv.a = &HFF
      EnvR.BackColor = RGBA8ColorToColorObject(tempEnv)
      tempEnv.r = &HFF
      tempEnv.g = &HFF
      tempEnv.b = &HFF
      tempEnv.a = Cmd.CMDParams(7)
      EnvA.BackColor = RGBA8ColorToColorObject(tempEnv)
      EnvR.Enabled = True
      EnvA.Enabled = True
    Else
      EnvR.Enabled = False
      EnvA.Enabled = False
    End If
  End Sub

  Private Sub UpdatePrimColor(ByVal Cmd As DLCommand)
    If Cmd.High > 0 Then
      Dim tempPrim As New Color4UByte
      tempPrim.r = Cmd.CMDParams(4)
      tempPrim.g = Cmd.CMDParams(5)
      tempPrim.b = Cmd.CMDParams(6)

      tempPrim.a = &HFF
      PrimR.BackColor = RGBA8ColorToColorObject(tempPrim)

      tempPrim.r = &HFF
      tempPrim.g = &HFF
      tempPrim.b = &HFF
      tempPrim.a = Cmd.CMDParams(7)
      PrimA.BackColor = RGBA8ColorToColorObject(tempPrim)

      PrimA.Enabled = True
      PrimR.Enabled = True

    Else
      PrimA.Enabled = False
      PrimR.Enabled = False
    End If
  End Sub

  Private Sub Dialog1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    For i As Integer = 0 To RDP_Defs.ColorAStr.Length - 1
      cA0.Items.Add(RDP_Defs.ColorAStr(i))
      cA1.Items.Add(RDP_Defs.ColorAStr(i))
    Next
    For i As Integer = 0 To RDP_Defs.ColorBStr.Length - 1
      cB0.Items.Add(RDP_Defs.ColorBStr(i))
      cB1.Items.Add(RDP_Defs.ColorBStr(i))
    Next
    For i As Integer = 0 To RDP_Defs.ColorCStr.Length - 1
      cC0.Items.Add(RDP_Defs.ColorCStr(i))
      cC1.Items.Add(RDP_Defs.ColorCStr(i))
    Next
    For i As Integer = 0 To RDP_Defs.ColorDStr.Length - 1
      cD0.Items.Add(RDP_Defs.ColorDStr(i))
      cD1.Items.Add(RDP_Defs.ColorDStr(i))
    Next

    For i As Integer = 0 To RDP_Defs.AlphaAStr.Length - 1
      aA0.Items.Add(RDP_Defs.AlphaAStr(i))
      aA1.Items.Add(RDP_Defs.AlphaAStr(i))
    Next
    For i As Integer = 0 To RDP_Defs.AlphaBStr.Length - 1
      aB0.Items.Add(RDP_Defs.AlphaBStr(i))
      aB1.Items.Add(RDP_Defs.AlphaBStr(i))
    Next
    For i As Integer = 0 To RDP_Defs.AlphaCStr.Length - 1
      aC0.Items.Add(RDP_Defs.AlphaCStr(i))
      aC1.Items.Add(RDP_Defs.AlphaCStr(i))
    Next
    For i As Integer = 0 To RDP_Defs.AlphaDStr.Length - 1
      aD0.Items.Add(RDP_Defs.AlphaDStr(i))
      aD1.Items.Add(RDP_Defs.AlphaDStr(i))
    Next

    UpdateEnvColor(LinkedCommands.EnvColor)

    UpdatePrimColor(LinkedCommands.PrimColor)

    Decode(Convert.ToUInt32(MainWin.LowordText.Text, 16), Convert.ToUInt32(MainWin.HiwordText.Text, 16))
  End Sub

  Private Sub Compile()
    CompiledCmb = CompileDL.Compile(UCodes.RDP, RDP.G_SETCOMBINE, CombinerColors)
    CompiledCmbCmd.Text = CompiledCmb.CMDParams(0).ToString("X2") & CompiledCmb.Low.ToString("X6") &
                          CompiledCmb.High.ToString("X8")
  End Sub
End Class
