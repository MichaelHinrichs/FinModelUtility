using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using f3dzex2.displaylist.opcodes.f3dzex2;
using f3dzex2.image;
using f3dzex2.io;
using f3dzex2.model;

using fin.data.queue;
using fin.model;

using UoT.memory;
using UoT.model;


namespace UoT.api {
  public enum OotSegmentIndex : uint {
    GAMEPLAY_KEEP = 4,
    GAMEPLAY_FIELD_KEEP = 5,
    ZOBJECT = 6,
    LINK_ANIMETION = 7,
  }

  public class OotModelLoader : IModelLoader<OotModelFileBundle> {
    public IModel LoadModel(OotModelFileBundle modelFileBundle) {
      var zFile = modelFileBundle.ZFile;
      var isLink = zFile.FileName is "object_link_boy"
                                     or "object_link_child"
                                     or "object_torch2";

      var n64Memory = new N64Memory(modelFileBundle.OotRom.ReadAllBytes(),
                                    Endianness.BigEndian);


      var n64Hardware = new N64Hardware<N64Memory>();
      n64Hardware.Memory = n64Memory;
      n64Hardware.Rdp = new Rdp { Tmem = new JankTmem(n64Hardware) };
      n64Hardware.Rsp = new Rsp();

      var zSegments = ZSegments.Instance;

      var gameplayKeep =
          zSegments.Others.Single(other => other.FileName is "gameplay_keep");
      n64Memory.AddSegment((uint) OotSegmentIndex.GAMEPLAY_KEEP,
                           gameplayKeep.Offset,
                           gameplayKeep.Length);

      // TODO: Use "gameplay dangeon keep" when applicable
      var gameplayFieldKeep =
          zSegments.Others.Single(
              other => other.FileName is "gameplay_field_keep");
      n64Memory.AddSegment((uint) OotSegmentIndex.GAMEPLAY_FIELD_KEEP,
                           gameplayFieldKeep.Offset,
                           gameplayFieldKeep.Length);

      n64Memory.AddSegment((uint) OotSegmentIndex.ZOBJECT,
                           zFile.Offset,
                           zFile.Length);

      var linkAnimetion =
          zSegments.Others.SingleOrDefault(
              other => other.FileName is "link_animetion");
      if (isLink) {
        n64Memory.AddSegment((uint) OotSegmentIndex.LINK_ANIMETION,
                             linkAnimetion.Offset,
                             linkAnimetion.Length);
      }

      var dlModelBuilder = new DlModelBuilder(n64Hardware);
      var finModel = dlModelBuilder.Model;

      var ootLimbs =
          new LimbHierarchyReader2().GetHierarchies(n64Memory, isLink);
      if (ootLimbs != null) {
        var finBones = new IBone[ootLimbs.Count];
        var ootLimbQueue =
            new FinTuple2Queue<IBone, int>((finModel.Skeleton.Root, 0));
        while (ootLimbQueue.TryDequeue(out var parentFinBone,
                                       out var ootLimbIndex)) {
          var ootLimb = ootLimbs[ootLimbIndex];
          var finBone = parentFinBone.AddChild(ootLimb.X, ootLimb.Y, ootLimb.Z);
          finBones[ootLimbIndex] = finBone;

          // TODO: Handle DLs
          // TODO: Handle animations

          var firstChildIndex = ootLimb.FirstChildIndex;
          if (firstChildIndex != -1) {
            ootLimbQueue.Enqueue((finBone, firstChildIndex));
          }

          var nextSiblingIndex = ootLimb.NextSiblingIndex;
          if (nextSiblingIndex != -1) {
            ootLimbQueue.Enqueue((parentFinBone, nextSiblingIndex));
          }
        }

        var finBonesWithDisplayLists = new List<IBone>();
        for (var i = 0; i < finBones.Length; ++i) {
          var ootLimb = ootLimbs[i];
          IoUtils.SplitSegmentedAddress(ootLimb.DisplayListSegmentedAddress,
                                        out var dlSegmentIndex,
                                        out _);

          if (dlSegmentIndex == 0) {
            continue;
          }

          var finBone = finBones[i];
          finBonesWithDisplayLists.Add(finBone);

          var displayList =
              new f3dzex2.displaylist.DisplayListReader().ReadDisplayList(
                  n64Memory,
                  new F3dzex2OpcodeParser(),
                  ootLimb.DisplayListSegmentedAddress);
          dlModelBuilder.AddDl(displayList);
        }

        var animationReader = new AnimationReader2();
        IList<IAnimation>? ootAnimations;
        if (isLink) {
          ootAnimations = animationReader.GetLinkAnimations(n64Memory,
            gameplayKeep,
            ootLimbs.Count);
        } else {
          var animationFiles = new List<IZFile> {zFile};
          ootAnimations = animationReader.GetCommonAnimations(
              n64Memory,
              animationFiles,
              ootLimbs.Count);
        }

        var animationIndex = 0;
        if (ootAnimations != null) {
          var rootBone = finBones[0];

          foreach (var ootAnimation in ootAnimations) {
            var finAnimation = finModel.AnimationManager.AddAnimation();
            finAnimation.FrameRate = 20;
            var frameCount = finAnimation.FrameCount = ootAnimation.FrameCount;

            finAnimation.Name = $"Animation {animationIndex++}";

            var rootAnimationTracks = finAnimation.AddBoneTracks(rootBone);
            for (var f = 0; f < frameCount; ++f) {
              var pos = ootAnimation.GetPosition(f);

              rootAnimationTracks.Positions.Set(f, 0, pos.X);
              rootAnimationTracks.Positions.Set(f, 1, pos.Y);
              rootAnimationTracks.Positions.Set(f, 2, pos.Z);
            }

            for (var i = 0; i < ootLimbs.Count; ++i) {
              var finBone = finBones[i];
              var animationTracks = i == 0
                  ? rootAnimationTracks
                  : finAnimation.AddBoneTracks(finBone);

              animationTracks.Rotations.ConvertRadiansToQuaternionImpl =
                  ConvertRadiansToQuaternionOot_;

              for (var a = 0; a < 3; ++a) {
                AddOotAnimationTrackToFin_(ootAnimation.GetTrack(i * 3 + a),
                                           a,
                                           animationTracks);
              }
            }
          }
        }
      }

      return finModel;
    }

    private void AddOotAnimationTrackToFin_(IAnimationTrack ootAnimationTrack,
                                            int axis,
                                            IBoneTracks boneTracks) {
      for (var f = 0; f < ootAnimationTrack.Frames.Count; ++f) {
        boneTracks.Rotations.Set(f,
                                 axis,
                                 (float) ((ootAnimationTrack.Frames[f] *
                                           360.0) / 0xFFFF));
      }
    }

    private static Quaternion ConvertRadiansToQuaternionOot_(
        float xRadians,
        float yRadians,
        float zRadians) {
      var r2d = MathF.PI / 180;
      var xDegrees = xRadians * r2d;
      var yDegrees = yRadians * r2d;
      var zDegrees = zRadians * r2d;

      var qz = Quaternion.CreateFromYawPitchRoll(0, 0, zDegrees);
      var qy = Quaternion.CreateFromYawPitchRoll(yDegrees, 0, 0);
      var qx = Quaternion.CreateFromYawPitchRoll(0, xDegrees, 0);

      return Quaternion.Normalize(qz * qy * qx);
    }
  }
}