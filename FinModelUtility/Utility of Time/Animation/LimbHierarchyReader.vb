Imports System.Numerics
Imports UoT.animation.playback

Public Class LimbHierarchyReader

  ''' <summary>
  '''   Parses a limb hierarchy according to the following spec:
  '''   https://wiki.cloudmodding.com/oot/Animation_Format#Hierarchy
  ''' </summary>
  Public Shared Function GetHierarchies(Data As IBank, isLink As Boolean, dlManager As DlManager, model As StaticDlModel) _
      As Limb()
    Dim limbIndexAddress As UInteger
    Dim limbIndexBank As UInteger
    Dim limbIndexOffset As UInteger

    model.Reset()

    Dim j As Integer = 0
    For i As Integer = 0 To Data.Count - 8 Step 4
      limbIndexAddress = IoUtil.ReadUInt32(Data, i)
      IoUtil.SplitAddress(limbIndexAddress, limbIndexBank, limbIndexOffset)

      Dim limbCount As UInteger = Data(i + 4)

      Dim limbAddress As UInteger
      Dim limbBank As UInteger
      Dim limbOffset As UInteger
      Dim limbBankBuffer As IBank

      ' Link has an extra set of values for each limb that define LOD model
      ' display lists.
      Dim limbSize As UInteger
      If isLink Then
        limbSize = 16
      Else
        limbSize = 12
      End If

      If RamBanks.IsValidBank(limbIndexBank) And limbCount > 0 Then
        Dim limbIndexBankBuffer As IBank = RamBanks.GetBankByIndex(limbIndexBank)

        If limbIndexOffset + 4 * limbCount < limbIndexBankBuffer.Count Then
          Dim firstChild As Byte
          Dim nextSibling As Byte

          Dim isValid As Boolean = True
          Dim somethingVisible As Boolean = False

          For j = 0 To limbCount - 1
            limbAddress = IoUtil.ReadUInt32(limbIndexBankBuffer, limbIndexOffset + j * 4)
            IoUtil.SplitAddress(limbAddress, limbBank, limbOffset)

            If Not RamBanks.IsValidBank(limbBank) Then
              isValid = False
              GoTo badLimbIndexOffset
            End If

            limbBankBuffer = RamBanks.GetBankByIndex(limbBank)

            If limbOffset + limbSize >= limbBankBuffer.Count Then
              isValid = False
              GoTo badLimbIndexOffset
            End If

            firstChild = limbBankBuffer(limbOffset + 6)
            nextSibling = limbBankBuffer(limbOffset + 7)

            If firstChild = j Or nextSibling = j Then
              isValid = False
              GoTo badLimbIndexOffset
            End If

            Dim displayListAddress As UInteger = IoUtil.ReadUInt32(limbBankBuffer, limbOffset + 8)
            Dim displayListBank As UInteger
            Dim displayListOffset As UInteger
            IoUtil.SplitAddress(displayListAddress, displayListBank, displayListOffset)

            If displayListBank <> 0 Then
              somethingVisible = True
            End If

            If displayListBank <> 0 And Not RamBanks.IsValidBank(displayListBank) Then
              isValid = False
              GoTo badLimbIndexOffset
            End If
          Next

badLimbIndexOffset:

          If isValid Then
            Dim tmpHierarchy(limbCount - 1) As Limb
            For k As Integer = 0 To limbCount - 1
              limbAddress = IoUtil.ReadUInt32(limbIndexBankBuffer, limbIndexOffset + 4 * k)
              IoUtil.SplitAddress(limbAddress, limbBank, limbOffset)
              limbBankBuffer = RamBanks.GetBankByIndex(limbBank)

              tmpHierarchy(k) = New Limb
              With tmpHierarchy(k)
                .x = IoUtil.ReadUInt16(limbBankBuffer, limbOffset + 0)
                .y = IoUtil.ReadUInt16(limbBankBuffer, limbOffset + 2)
                .z = IoUtil.ReadUInt16(limbBankBuffer, limbOffset + 4)
                .firstChild = CSByte(limbBankBuffer(limbOffset + 6))
                .nextSibling = CSByte(limbBankBuffer(limbOffset + 7))

                model.AddLimb(.x, .y, .z, .firstChild, .nextSibling)

                Dim displayListAddress As UInteger = IoUtil.ReadUInt32(limbBankBuffer, limbOffset + 8)
                Dim displayListBank As UInteger
                Dim displayListOffset As UInteger
                IoUtil.SplitAddress(displayListAddress, displayListBank, displayListOffset)

                If displayListBank <> 0 Then
                  Dim displayListBankBuffer As IBank = RamBanks.GetBankByIndex(displayListBank)
                  .DisplayListAddress = displayListAddress

                  DisplayListReader.ReadInDL(dlManager, displayListAddress, MainWin.DListSelection)
                ElseIf Not somethingVisible Then
                  .DisplayListAddress = displayListAddress
                Else
                  .DisplayListAddress = displayListAddress
                End If

                ' Far model display list (i.e. LOD model). Only used for Link.
                'If Data(tmpLimbOff + 12) = Bank Then
                '    .DisplayListLow = ReadUInt24(Data, tmpLimbOff + 13)
                '    ReDim Preserve N64DList(N64DList.Length)
                '    ReadInDL(Data, N64DList, .DisplayListLow, N64DList.Length - 1)
                'Else
                .DisplayListLow = Nothing

                'End If
                .r = Rand.NextDouble
                .g = Rand.NextDouble
                .b = Rand.NextDouble
              End With
            Next

            If isValid And Not somethingVisible Then
              Throw New NotSupportedException("model format is not rendering a valid model!")
            End If

            Return tmpHierarchy
          End If
        End If
      End If
    Next
    Return Nothing
  End Function
End Class
