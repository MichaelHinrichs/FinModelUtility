using System.Numerics;

using MKDS_Course_Modifier.GCN;

namespace mkds.exporter {
  public static class JointUtil {
    public static Vector3 GetTranslation(IBcx bcx, int jointIndex, float time) {
      var animatedJoint = bcx.Anx1.Joints[jointIndex];
      var values = animatedJoint.Values;

      var x = animatedJoint.GetAnimValue(values.translationsX, time);
      var y = animatedJoint.GetAnimValue(values.translationsY, time);
      var z = animatedJoint.GetAnimValue(values.translationsZ, time);

      return new Vector3(x, y, z);
    }

    public static Quaternion GetRotation(IBcx bcx, int jointIndex, float time) {
      var animatedJoint = bcx.Anx1.Joints[jointIndex];
      var values = animatedJoint.Values;

      var xRadians = animatedJoint.GetAnimValue(values.rotationsX, time);
      var yRadians = animatedJoint.GetAnimValue(values.rotationsY, time);
      var zRadians = animatedJoint.GetAnimValue(values.rotationsZ, time);

      return JointUtil.CreateQuaternion_(xRadians, yRadians, zRadians);
    }

    public static Vector3 GetScale(IBcx bcx, int jointIndex, float time) {
      var animatedJoint = bcx.Anx1.Joints[jointIndex];
      var values = animatedJoint.Values;

      var x = animatedJoint.GetAnimValue(values.scalesX, time);
      var y = animatedJoint.GetAnimValue(values.scalesY, time);
      var z = animatedJoint.GetAnimValue(values.scalesZ, time);

      return new Vector3(x, y, z);
    }

    private static Quaternion CreateQuaternion_(
        float xRadians,
        float yRadians,
        float zRadians) {
      var qz = Quaternion.CreateFromYawPitchRoll(0, 0, zRadians);
      var qy = Quaternion.CreateFromYawPitchRoll(yRadians, 0, 0);
      var qx = Quaternion.CreateFromYawPitchRoll(0, xRadians, 0);

      return Quaternion.Normalize(qz * qy * qx);
    }
  }
}