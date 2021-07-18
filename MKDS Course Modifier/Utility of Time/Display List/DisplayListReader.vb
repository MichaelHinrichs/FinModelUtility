Module DisplayListReader
  ' TODO: Remove combo box input.
  Public Function ReadInDL(dlManager As DlManager, address As UInteger, dListSelection As ComboBox) As Integer
    Dim bank As Byte
    Dim offset As UInteger
    IoUtil.SplitAddress(address, bank, offset)

    Dim data As IBank = RamBanks.GetBankByIndex(bank)

    Try
      If offset < data.Count Then
        ' TODO: This jumps into the lowest level DL, but the 0xDE command (DL)
        ' actually allows returning back up and calling more DLs. So this seems
        ' like it will sometimes overlook any DLs that follow.
        ' This should just be deleted and replaced w/ emulating in
        ' F3DEX2_Parser.
        If data(offset) = &HDE Then
          Do Until data(offset) <> &HDE
            offset = IoUtil.ReadUInt24(data, offset + 5)
          Loop
        End If

        Dim index As Integer = dlManager.Count

        Dim displayList As New N64DisplayList
        dlManager.Add(displayList)

        Dim EPLoc As UInteger = offset

        dListSelection.Items.Add((index + 1).ToString & ". " & Hex(offset))

        With displayList
          .StartPos = New ZSegment
          .StartPos.Offset = offset
          .StartPos.Bank = data.Segment
          .Skip = False

          .PickCol = New Color3UByte
          PickerUtil.NextRgb(.PickCol.r, .PickCol.g, .PickCol.b)

          Do
            ReDim Preserve .Commands(.CommandCount)
            .Commands(.CommandCount) = New DLCommand(data, EPLoc)

            If data(EPLoc) = F3DZEX.ENDDL Or EPLoc >= data.Count Then
              EPLoc += 8
              Exit Do
            End If

            EPLoc += 8
            .CommandCount += 1
          Loop
        End With

        Return EPLoc
      End If
    Catch ex As Exception
      MsgBox("Error reading in display list: " & ex.Message, MsgBoxStyle.Critical, "Exception")
      Exit Function
    End Try
  End Function
End Module
