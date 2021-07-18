Public Class ZAnimation
  ''' <summary>
  '''   Parses a set of animations according to the spec at:
  '''   https://wiki.cloudmodding.com/oot/Animation_Format#Normal_Animations
  ''' </summary>
  Public Function GetCommonAnimations(Data As IBank, ByVal LimbCount As Integer, animationList As ListBox) _
    As IList(Of IAnimation)
    Dim trackCount As UInteger = LimbCount * 3

    Dim animCnt As Integer = -1
    Dim tAnimation(-1) As NormalAnimation
    MainWin.AnimationList.Items.Clear()

    ' Guesstimating the index by looking for an spot where the header's angle
    ' address and track address have the same bank as the param at the top.
    ' TODO: Is this robust enough?
    For i As Integer = 4 To Data.Count - 12 - 1 Step 4
      Dim attemptOffset = i - 4

      Dim frameCount As UShort = IoUtil.ReadUInt16(Data, attemptOffset)

      If frameCount = 0 Then
        Continue For
      End If

      Dim rotationValuesAddress As UInt32 = IoUtil.ReadUInt32(Data, attemptOffset + 4)
      Dim rotationIndicesAddress As UInt32 = IoUtil.ReadUInt32(Data, attemptOffset + 8)

      Dim rotationValuesBank As Byte
      Dim rotationValuesOffset As UInt32
      IoUtil.SplitAddress(rotationValuesAddress, rotationValuesBank, rotationValuesOffset)

      Dim rotationIndicesBank As Byte
      Dim rotationIndicesOffset As UInt32
      IoUtil.SplitAddress(rotationIndicesAddress, rotationIndicesBank, rotationIndicesOffset)

      Dim limit As UShort = IoUtil.ReadUInt16(Data, attemptOffset + 12)

      Dim validAttemptOffset As Boolean = RamBanks.IsValidBank(rotationValuesBank) And
                                          RamBanks.IsValidBank(rotationIndicesBank)

      Dim rotationValuesBuffer As IBank
      Dim rotationIndicesBuffer As IBank
      Dim validRotationOffsets As Boolean = False
      If validAttemptOffset Then
        ' 6 is "current file", which is whatever was passed into "Data".
        If rotationValuesBank = 6 Then
          rotationValuesBuffer = Data
        Else
          rotationValuesBuffer = RamBanks.GetBankByIndex(rotationValuesBank)
        End If
        If rotationIndicesBank = 6 Then
          rotationIndicesBuffer = Data
        Else
          rotationIndicesBuffer = RamBanks.GetBankByIndex(rotationIndicesBank)
        End If

        ' Offsets should be within bounds of the bank.
        Dim validRotationValuesOffset As Boolean = rotationValuesOffset < rotationValuesBuffer.Count
        Dim validRotationIndicesOffset As Boolean = rotationIndicesOffset < rotationIndicesBuffer.Count
        validRotationOffsets = validRotationValuesOffset And validRotationIndicesOffset
      End If

      ' Angle count should be greater than 0.
      Dim angleCount As UInteger = (rotationIndicesOffset - rotationValuesOffset) \ 2
      Dim validAngleCount As Boolean = rotationIndicesOffset > rotationValuesOffset And angleCount > 0

      If validAttemptOffset And validRotationOffsets And validAngleCount Then
        ' Should have zeroes present in two spots of the animation header. 
        Dim hasZeroes As Boolean = IoUtil.ReadUInt16(Data, attemptOffset + 2) = 0 And
                                   IoUtil.ReadUInt16(Data, attemptOffset + 14) = 0

        ' TODO: Assumes 0 is one of the angles, is this valid?
        'Dim validAngles As Boolean = IoUtil.ReadUInt16(Data, rotationValuesOffset) = 0 And IoUtil.ReadUInt16(Data, rotationValuesOffset + 2) > 0

        ' All values of "tTrack" should be within the bounds of .Angles.
        Dim validTTracks As Boolean = True
        For i1 As Integer = 0 To trackCount - 1
          Dim tTrack As UShort = IoUtil.ReadUInt16(rotationIndicesBuffer, rotationIndicesOffset + 6 + 2 * i1)

          If tTrack < limit Then
            If tTrack >= angleCount Then
              validTTracks = False
              GoTo badTTracks
            End If
          ElseIf tTrack + frameCount > angleCount Then
            validTTracks = False
            GoTo badTTracks
          End If
        Next
badTTracks:

        If hasZeroes And validTTracks Then
          animCnt += 1
          ReDim Preserve tAnimation(animCnt)
          With tAnimation(animCnt)
            .FrameCount = frameCount
            .TrackOffset = rotationIndicesOffset

            .AngleCount = angleCount

            If .FrameCount > 0 Then
              ReDim .Angles(.AngleCount - 1)
              ReDim .Tracks(trackCount - 1)

              animationList.Items.Add("0x" & Hex(i))

              For i1 As Integer = 0 To .AngleCount - 1
                .Angles(i1) = IoUtil.ReadUInt16(rotationValuesBuffer, rotationValuesOffset)
                rotationValuesOffset += 2
              Next

              .Position.X = IoUtil.ReadInt16(rotationIndicesBuffer, .TrackOffset + 0)
              .Position.Y = IoUtil.ReadInt16(rotationIndicesBuffer, .TrackOffset + 2)
              .Position.Z = IoUtil.ReadInt16(rotationIndicesBuffer, .TrackOffset + 4)

              Dim tTrackOffset As Integer = .TrackOffset + 6

              For i1 As Integer = 0 To trackCount - 1
                Dim tTrack As UShort = IoUtil.ReadUInt16(rotationIndicesBuffer, tTrackOffset)

                If tTrack < limit Then
                  ' Constant (single value)
                  .Tracks(i1).Type = 0
                  ReDim .Tracks(i1).Frames(0)
                  .Tracks(i1).Frames(0) = .Angles(tTrack)
                Else
                  ' Keyframes
                  .Tracks(i1).Type = 1
                  ReDim .Tracks(i1).Frames(.FrameCount - 1)
                  For i2 As Integer = 0 To .FrameCount - 1
                    Try
                      .Tracks(i1).Frames(i2) = .Angles(tTrack + i2)
                    Catch
                      Return Nothing
                    End Try
                  Next
                End If

                tTrackOffset += 2
              Next
            Else
              ReDim Preserve tAnimation(animCnt - 1)
            End If
          End With
        End If
      End If
    Next
    If tAnimation.Length > 0 Then
      Dim outList As New List(Of IAnimation)
      For Each animation As NormalAnimation In tAnimation
        outList.Add(animation)
      Next
      Return outList
    End If
    Return Nothing
  End Function

  ''' <summary>
  '''   Parses a set of animations according to the spec at:
  '''   https://wiki.cloudmodding.com/oot/Animation_Format#C_code
  ''' </summary>
  Public Function GetLinkAnimations(HeaderData As IBank, ByVal LimbCount As Integer, animationData As IBank, animationList As ListBox) _
    As IList(Of IAnimation)
    Dim animCnt As Integer = -1
    Dim animations(-1) As LinkAnimetion
    MainWin.AnimationList.Items.Clear()

    Dim trackCount As UInteger = LimbCount * 3
    Dim frameSize = 2 * (3 + trackCount) + 2

    For i As Integer = &H2310 To &H34F8 Step 4
      Dim frameCount As UShort = IoUtil.ReadUInt16(HeaderData, i)
      Dim animationAddress As UInt32 = IoUtil.ReadUInt32(HeaderData, i + 4)

      Dim animationBank As Byte
      Dim animationOffset As UInt32
      IoUtil.SplitAddress(animationAddress, animationBank, animationOffset)

      Dim validAnimationBank As Boolean = animationBank = 7 ' Corresponds to link_animetions.
      Dim hasZeroes As Boolean = IoUtil.ReadUInt16(HeaderData, i + 2) = 0

      ' TODO: Is this really needed?
      Dim validOffset As Boolean = animationOffset + frameSize * frameCount < animationData.Count

      If validAnimationBank And hasZeroes And validOffset Then
        animCnt += 1
        ReDim Preserve animations(animCnt)
        With animations(animCnt)
          .FrameCount = frameCount

          If frameCount > 0 Then
            ReDim .Positions(frameCount - 1)

            ReDim .Tracks(trackCount - 1)
            For t As Integer = 0 To trackCount - 1
              .Tracks(t).Type = 1
              ReDim .Tracks(t).Frames(frameCount - 1)
            Next

            ReDim .FacialStates(frameCount - 1)

            For f As Integer = 0 To frameCount - 1
              Dim frameOffset As UInteger = animationOffset + f * frameSize

              ' TODO: This should be ReadInt16() instead.
              Dim position As Vec3s
              .Positions(f).X = IoUtil.ReadUInt16(animationData, frameOffset + 0)
              .Positions(f).Y = IoUtil.ReadUInt16(animationData, frameOffset + 2)
              .Positions(f).Z = IoUtil.ReadUInt16(animationData, frameOffset + 4)

              For t As Integer = 0 To trackCount - 1
                Dim trackOffset As UInteger = frameOffset + 2 * (3 + t)

                .Tracks(t).Frames(f) = IoUtil.ReadUInt16(animationData, trackOffset)
              Next

              Dim facialStateOffset As UInteger = frameOffset + 2 * (3 + trackCount)

              Dim facialState = animationData(facialStateOffset + 1)
              Dim mouthState As Byte = IoUtil.ShiftR(facialState, 4, 4)
              Dim eyeState As Byte = IoUtil.ShiftR(facialState, 0, 4)

              .FacialStates(f).EyeState = eyeState
              .FacialStates(f).MouthState = mouthState
            Next

            animationList.Items.Add("0x" & Hex(i))
          Else
            ReDim Preserve animations(animCnt - 1)
          End If
        End With
      End If
    Next
    If animations.Length > 0 Then
      Dim outList As New List(Of IAnimation)
      For Each animation As LinkAnimetion In animations
        outList.Add(animation)
      Next
      Return outList
    End If
    Return Nothing
  End Function
End Class
