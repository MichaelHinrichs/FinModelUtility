using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.VisualBasic;

using UoT.util;

namespace UoT {
  public class AnimationReader {
    /// <summary>
    ///   Parses a set of animations according to the spec at:
    ///   https://wiki.cloudmodding.com/oot/Animation_Format#Normal_Animations
    /// </summary>
    public IList<IAnimation>? GetCommonAnimations(
        IBank bank,
        int limbCount,
        ListBox animationList) {
      animationList.Items.Clear();

      uint trackCount = (uint) (limbCount * 3);
      var animations = new List<IAnimation>();

      // Guesstimating the index by looking for an spot where the header's angle
      // address and track address have the same bank as the param at the top.
      for (var i = 4; i < bank.Count - 12; i += 4) {
        var attemptOffset = (uint) (i - 4);

        // Verifies the frame count is positive.
        var frameCount = IoUtil.ReadUInt16(bank, attemptOffset);
        if (frameCount == 0) {
          continue;
        }

        var rotationValuesAddress = IoUtil.ReadUInt32(
            bank,
            attemptOffset + 4);
        IoUtil.SplitAddress(rotationValuesAddress,
                            out var rotationValuesBank,
                            out var rotationValuesOffset);

        // Verifies the rotation values address has a valid bank.
        if (!RamBanks.IsValidBank(rotationValuesBank)) {
          continue;
        }

        // Verifies the rotation indices address has a valid bank.
        var rotationIndicesAddress = IoUtil.ReadUInt32(
            bank,
            attemptOffset + 8);
        IoUtil.SplitAddress(rotationIndicesAddress,
                            out var rotationIndicesBank,
                            out var rotationIndicesOffset);
        if (!RamBanks.IsValidBank(rotationIndicesBank)) {
          continue;
        }

        // Obtains the specified banks.
        var rotationValuesBuffer =
            Asserts.Assert(RamBanks.GetBankByIndex(rotationValuesBank));
        var rotationIndicesBuffer =
            Asserts.Assert(RamBanks.GetBankByIndex(rotationIndicesBank));

        // Offsets should be within bounds of the bank.
        var validRotationValuesOffset =
            rotationValuesOffset < rotationValuesBuffer.Count;
        var validRotationIndicesOffset =
            rotationIndicesOffset < rotationIndicesBuffer.Count;

        if (!validRotationValuesOffset || !validRotationIndicesOffset) {
          continue;
        }

        // Angle count should be greater than 0.
        var angleCount =
            (uint) ((rotationIndicesOffset - rotationValuesOffset) / 2L);
        var validAngleCount = rotationIndicesOffset > rotationValuesOffset &&
                              angleCount > 0L;
        if (!validAngleCount) {
          continue;
        }

        // Should have zeroes present in two spots of the animation header. 
        var hasZeroes =
            IoUtil.ReadUInt16(bank, attemptOffset + 2) == 0 &&
            IoUtil.ReadUInt16(bank, attemptOffset + 14) == 0;
        if (!hasZeroes) {
          continue;
        }

        // All values of "tTrack" should be within the bounds of .Angles.
        var validTTracks = true;
        var limit = IoUtil.ReadUInt16(bank, attemptOffset + 12);
        for (var i1 = 0; i1 < trackCount; i1++) {
          var tTrack = IoUtil.ReadUInt16(
              rotationIndicesBuffer,
              (uint) (rotationIndicesOffset + 6L + 2 * i1));
          if (tTrack < limit) {
            if (tTrack >= angleCount) {
              validTTracks = false;
              goto badTTracks;
            }
          } else if ((uint) (tTrack + frameCount) > angleCount) {
            validTTracks = false;
            goto badTTracks;
          }
        }

        badTTracks:
        if (!validTTracks) {
          continue;
        }

        var animation = new NormalAnimation {
            FrameCount = frameCount,
            TrackOffset = rotationIndicesOffset,
            AngleCount = angleCount
        };

        animation.Angles = new ushort[animation.AngleCount];
        for (var i1 = 0; i1 < animation.AngleCount; ++i1) {
          animation.Angles[i1] =
              IoUtil.ReadUInt16(rotationValuesBuffer,
                                rotationValuesOffset);
          rotationValuesOffset = (uint) (rotationValuesOffset + 2L);
        }

        animation.Position = new Vec3s {
            X = IoUtil.ReadInt16(rotationIndicesBuffer, animation.TrackOffset),
            Y = IoUtil.ReadInt16(rotationIndicesBuffer,
                                 animation.TrackOffset + 2),
            Z = IoUtil.ReadInt16(rotationIndicesBuffer,
                                 animation.TrackOffset + 4),
        };

        animation.Tracks = new NormalAnimationTrack[trackCount];

        var tTrackOffset = (int) (animation.TrackOffset + 6L);
        for (var i1 = 0; i1 < trackCount; ++i1) {
          var track = animation.Tracks[i1] = new NormalAnimationTrack();

          var tTrack =
              IoUtil.ReadUInt16(rotationIndicesBuffer,
                                (uint) tTrackOffset);
          if (tTrack < limit) {
            // Constant (single value)
            track.Type = 0;
            track.Frames = new ushort[1];
            track.Frames[0] = animation.Angles[tTrack];
          } else {
            // Keyframes
            track.Type = 1;
            track.Frames = new ushort[animation.FrameCount];
            for (var i2 = 0; i2 < animation.FrameCount; ++i2) {
              try {
                track.Frames[i2] = animation.Angles[tTrack + i2];
              } catch {
                return null;
              }
            }
          }

          tTrackOffset += 2;
        }

        animations.Add(animation);

        animationList.Items.Add("0x" + Conversion.Hex(i));
      }

      return animations.Count > 0 ? animations : null;
    }

    /// <summary>
    ///   Parses a set of animations according to the spec at:
    ///   https://wiki.cloudmodding.com/oot/Animation_Format#C_code
    /// </summary>
    public IList<IAnimation>? GetLinkAnimations(
        IBank HeaderData,
        int LimbCount,
        IBank animationData,
        ListBox animationList) {
      animationList.Items.Clear();
      var animations = new List<IAnimation>();

      var trackCount = (uint) (LimbCount * 3);
      var frameSize = 2 * (3 + trackCount) + 2;
      for (uint i = 0x2310; i <= 0x34F8; i += 4) {
        // Verifies the frame count is positive.
        var frameCount = IoUtil.ReadUInt16(HeaderData, i);
        if (frameCount == 0) {
          continue;
        }

        var animationAddress = IoUtil.ReadUInt32(HeaderData, i + 4);
        IoUtil.SplitAddress(animationAddress,
                            out var animationBank,
                            out var animationOffset);

        // Should use link_animetion bank.
        var validAnimationBank = animationBank == 7;
        if (!validAnimationBank) {
          continue;
        }

        // Should have zeroes in the expected bytes of the header.
        var hasZeroes = IoUtil.ReadUInt16(HeaderData, i + 2) == 0;
        if (!hasZeroes) {
          continue;
        }

        // Should be within the bounds of the bank.
        var validOffset = animationOffset + frameSize * frameCount <
                          animationData.Count;
        if (!validOffset) {
          continue;
        }

        // Everything looks good with this animation location!

        // Starts parsing animation from this spot.
        var tracks = new LinkAnimetionTrack[(int) (trackCount - 1L + 1)];
        var positions = new Vec3s[frameCount];
        var facialStates = new FacialState[frameCount];

        for (int t = 0, loopTo = (int) (trackCount - 1L);
             t <= loopTo;
             t++) {
          tracks[t] = new LinkAnimetionTrack(1, new ushort[frameCount]);
        }

        for (int f = 0, loopTo1 = frameCount - 1; f <= loopTo1; f++) {
          var frameOffset = (uint) (animationOffset + f * frameSize);

          // TODO: This should be ReadInt16() instead.
          positions[f] = new Vec3s {
              X = (short) IoUtil.ReadUInt16(animationData, frameOffset),
              Y = (short) IoUtil.ReadUInt16(animationData, frameOffset + 2),
              Z = (short) IoUtil.ReadUInt16(animationData, frameOffset + 4),
          };
          for (int t = 0, loopTo2 = (int) (trackCount - 1L);
               t <= loopTo2;
               t++) {
            var trackOffset = (uint) (frameOffset + 2 * (3 + t));
            tracks[t].Frames[f] = IoUtil.ReadUInt16(animationData, trackOffset);
          }

          var facialStateOffset =
              (int) (frameOffset + 2 * (3 + trackCount));
          var facialState = animationData[facialStateOffset + 1];
          var mouthState = IoUtil.ShiftR(facialState, 4, 4);
          var eyeState = IoUtil.ShiftR(facialState, 0, 4);

          facialStates[f] = new FacialState((EyeState) eyeState,
                                            (MouthState) mouthState);
        }

        var animation =
            new LinkAnimetion(frameCount, tracks, positions, facialStates);
        animations.Add(animation);

        animationList.Items.Add("0x" + Conversion.Hex(i));
      }

      return animations.Count > 0 ? animations : null;
    }
  }
}