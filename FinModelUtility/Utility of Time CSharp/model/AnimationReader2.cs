using System;
using System.Collections.Generic;
using System.IO;

using f3dzex2.io;

using UoT.memory;

#pragma warning disable CS8603


namespace UoT.model {
  public class AnimationReader2 {
    /// <summary>
    ///   Parses a set of animations according to the spec at:
    ///   https://wiki.cloudmodding.com/oot/Animation_Format#Normal_Animations
    /// </summary>
    // TODO: Some jank still slips through, is there a proper list of these
    // addresses somewhere in the file?
    public IList<IAnimation>? GetCommonAnimations(
        IN64Memory n64Memory,
        IReadOnlyList<IZFile> animationFiles,
        int limbCount) {
      uint trackCount = (uint) (limbCount * 3);
      var animations = new List<IAnimation>();

      foreach (var animationFile in animationFiles) {
        using var entryEr = new EndianBinaryReader(
            new MemoryStream(n64Memory.Bytes,
                             (int) animationFile.Offset,
                             (int) animationFile.Length),
            n64Memory.Endianness);

        // Guesstimating the index by looking for an spot where the header's angle
        // address and track address have the same bank as the param at the top.
        for (var i = 0; i < entryEr.Length - 16; ++i) {
          entryEr.Position = i;

          var frameCount = entryEr.ReadUInt16();
          var pad0 = entryEr.ReadUInt16();
          var rotationValuesAddress = entryEr.ReadUInt32();
          var rotationIndicesAddress = entryEr.ReadUInt32();
          var limit = entryEr.ReadUInt16();
          var pad1 = entryEr.ReadUInt16();

          if (pad0 != 0 || pad1 != 0) {
            continue;
          }

          // Verifies the frame count is positive.
          if (frameCount == 0) {
            continue;
          }

          if (!n64Memory.IsValidSegmentedAddress(rotationValuesAddress)) {
            continue;
          }

          // Verifies the rotation indices address has a valid bank.
          if (!n64Memory.IsValidSegmentedAddress(rotationIndicesAddress)) {
            continue;
          }

          // Obtains the specified banks.
          using var rotationValuesEr =
              n64Memory.OpenAtSegmentedAddress(rotationValuesAddress);
          using var rotationIndicesEr =
              n64Memory.OpenAtSegmentedAddress(rotationIndicesAddress);
          var originalRotationIndicesOffset = rotationIndicesEr.Position;

          // Angle count should be greater than 0.
          var angleCount =
              (rotationIndicesEr.Position - rotationValuesEr.Position) / 2L;
          if (angleCount <= 0) {
            continue;
          }

          // All values of "tTrack" should be within the bounds of .Angles.
          var validTTracks = true;
          for (var i1 = 0; i1 < 3 + (trackCount + 1); i1++) {
            var tTrack = rotationIndicesEr.ReadUInt16();
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
              TrackOffset = (uint) originalRotationIndicesOffset,
              AngleCount = (uint) angleCount
          };

          animation.Angles = rotationValuesEr.ReadUInt16s(animation.AngleCount);

          // Translation is at the start.
          rotationIndicesEr.Position = originalRotationIndicesOffset;
          var xList =
              ReadFrames_(
                  rotationIndicesEr.ReadUInt16(),
                  limit,
                  animation);
          var yList =
              ReadFrames_(
                  rotationIndicesEr.ReadUInt16(),
                  limit,
                  animation);
          var zList =
              ReadFrames_(
                  rotationIndicesEr.ReadUInt16(),
                  limit,
                  animation);

          animation.Positions = new Vec3s[animation.FrameCount];
          for (var pi = 0; pi < animation.FrameCount; ++pi) {
            animation.Positions[pi] = new Vec3s {
                X = (short) xList[Math.Min(pi, xList.Length - 1)],
                Y = (short) yList[Math.Min(pi, yList.Length - 1)],
                Z = (short) zList[Math.Min(pi, zList.Length - 1)],
            };
          }

          animation.Tracks = new NormalAnimationTrack[trackCount];

          for (var i1 = 0; i1 < trackCount; ++i1) {
            var track = animation.Tracks[i1] = new NormalAnimationTrack();
            track.Frames =
                ReadFrames_(rotationIndicesEr.ReadUInt16(), limit, animation);
          }

          animations.Add(animation);
        }
      }

      return animations.Count > 0 ? animations : null;
    }

    private static ushort[] ReadFrames_(
        ushort tTrack,
        ushort limit,
        NormalAnimation animation) {
      ushort[] frames;

      // Constant
      if (tTrack < limit) {
        frames = new ushort[1];
        frames[0] = animation.Angles[tTrack];
      } else {
        // Keyframes
        frames = new ushort[animation.FrameCount];
        for (var i2 = 0; i2 < animation.FrameCount; ++i2) {
          try {
            frames[i2] = animation.Angles[tTrack + i2];
          } catch {
            return null;
          }
        }
      }

      return frames;
    }

    /// <summary>
    ///   Parses a set of animations according to the spec at:
    ///   https://wiki.cloudmodding.com/oot/Animation_Format#C_code
    /// </summary>
    /*public IList<IAnimation>? GetLinkAnimations(
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
        IoUtils.SplitSegmentedAddress(animationAddress,
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
    }*/
  }
}