using dat.schema.animation;

using fin.model;

namespace dat.api {
  public static class DatBoneTracksHelper {
    public static void AddDatKeyframesToBoneTracks(
        IEnumerable<IDatKeyframes> allDatKeyframes,
        IBoneTracks boneTracks) {
      var positionTrack = boneTracks.UseSeparatePositionAxesTrack();
      var rotationTrack = boneTracks.UseEulerRadiansRotationTrack();
      var scaleTrack = boneTracks.UseScaleTrack();

      foreach (var datKeyframes in allDatKeyframes) {
        var jointTrackType = datKeyframes.JointTrackType;
        switch (jointTrackType) {
          case JointTrackType.HSD_A_J_TRAX:
          case JointTrackType.HSD_A_J_TRAY:
          case JointTrackType.HSD_A_J_TRAZ: {
            var axis = jointTrackType - JointTrackType.HSD_A_J_TRAX;
            foreach (var keyframe in datKeyframes.Keyframes) {
              var (frame, value, tangent) = keyframe;
              positionTrack.Set(frame,
                                axis,
                                value,
                                tangent,
                                tangent);
            }

            break;
          }
          case JointTrackType.HSD_A_J_ROTX:
          case JointTrackType.HSD_A_J_ROTY:
          case JointTrackType.HSD_A_J_ROTZ: {
            var axis = jointTrackType - JointTrackType.HSD_A_J_ROTX;
            foreach (var keyframe in datKeyframes.Keyframes) {
              var (frame, value, tangent) = keyframe;
              rotationTrack.Set(frame,
                                axis,
                                value,
                                tangent,
                                tangent);
            }

            break;
          }
          case JointTrackType.HSD_A_J_SCAX:
          case JointTrackType.HSD_A_J_SCAY:
          case JointTrackType.HSD_A_J_SCAZ: {
            var axis = jointTrackType - JointTrackType.HSD_A_J_SCAX;
            foreach (var keyframe in datKeyframes.Keyframes) {
              var (frame, value, tangent) = keyframe;
              scaleTrack.Set(frame,
                             axis,
                             value,
                             tangent,
                             tangent);
            }

            break;
          }
        }
      }
    }
  }
}