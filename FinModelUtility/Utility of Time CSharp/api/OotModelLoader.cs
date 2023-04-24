using System.Collections.Generic;
using System.IO;
using System.Linq;

using f3dzex2.io;

using fin.data.queue;
using fin.model;
using fin.model.impl;

using UoT.memory;
using UoT.model;


namespace UoT.api {
  public enum OotSegmentIndex : uint {
    GAMEPLAY_KEEP = 4,
    GAMEPLAY_FIELD_KEEP = 5,
    ZOBJECT = 6,
  }

  public class OotModelLoader : IModelLoader<OotModelFileBundle> {
    public IModel LoadModel(OotModelFileBundle modelFileBundle) {
      var n64Memory = new N64Memory(modelFileBundle.OotRom.ReadAllBytes(),
                                    Endianness.BigEndian);

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

      var zFile = modelFileBundle.ZFile;
      n64Memory.AddSegment((uint) OotSegmentIndex.ZOBJECT,
                           zFile.Offset,
                           zFile.Length);

      var finModel = new ModelImpl();

      var isLink = zFile.FileName is "object_link_boy"
                                     or "object_link_child"
                                     or "object_torch2";
      var ootLimbs =
          new LimbHierarchyReader2().GetHierarchies(n64Memory, isLink);
      var finBones = new IBone[ootLimbs.Count];
      if (ootLimbs != null) {
        var ootLimbQueue = new FinTuple2Queue<IBone, int>((finModel.Skeleton.Root, 0));
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


        var animationFiles = new List<IZFile> {zFile};
        if (isLink) {
          animationFiles.Add(
              zSegments.Others.Single(
                  other => other.FileName is "link_animetion"));
        }

        var animationReader = new AnimationReader2();
        var ootAnimations = isLink
            ? null
            : animationReader.GetCommonAnimations(
                n64Memory,
                animationFiles,
                ootLimbs.Count);

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
                                 (ootAnimationTrack.Frames[f] * 360f) / 0xFFFF);
      }
    }
  }
}