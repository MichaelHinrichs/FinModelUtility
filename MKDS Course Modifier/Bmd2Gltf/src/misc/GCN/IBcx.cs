using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MKDS_Course_Modifier._3D_Formats;

namespace MKDS_Course_Modifier.GCN {
  public static class BcxConstants {
    // TODO: Pass this in via a flag?
    public static float MIN_TIME_UNIT = 1 / 30f;
  }

  public interface IBcx {
    IAnx1 Anx1 { get; }
  }

  public interface IAnx1 {
    int FrameCount { get; }
    IAnimatedJoint[] Joints { get; }
  }

  public interface IAnimatedJoint {
    IJointAnim Values { get; }
    float GetAnimValue(IJointAnimKey[] keys, float time);
  }

  public interface IJointAnim {
    IJointAnimKey[] scalesX { get; }
    IJointAnimKey[] scalesY { get; }
    IJointAnimKey[] scalesZ { get; }

    IJointAnimKey[] rotationsX { get; }
    IJointAnimKey[] rotationsY { get; }
    IJointAnimKey[] rotationsZ { get; }
    
    IJointAnimKey[] translationsX { get; }
    IJointAnimKey[] translationsY { get; }
    IJointAnimKey[] translationsZ { get; }
  }

  public interface IJointAnimKey {
    float Time { get; }
    float Value { get; }
    float IncomingTangent { get; }
    float OutgoingTangent { get; }
  }
}