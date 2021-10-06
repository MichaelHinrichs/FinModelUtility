using System;
using System.Collections.Generic;

using Tao.OpenGl;

using UoT.animation.playback;

namespace UoT.limbs {
  public static class SkeletonRenderer {
    public static void Render(
        IList<IOldLimb> limbs,
        LimbMatrices limbMatrices,
        IAnimation? animation,
        IAnimationPlaybackManager animationPlaybackManager) {
      if (limbs.Count == 0) {
        return;
      }

      ModelViewMatrixTransformer.Push();

      if (animation != null) {
        var frame = animationPlaybackManager.Frame;
        var totalFrames = animationPlaybackManager.TotalFrames;

        var frameIndex = (int) Math.Floor(frame);
        var frameDelta = frame % 1;

        var startPos = animation.GetPosition(frameIndex);
        var endPos = animation.GetPosition((frameIndex + 1) % totalFrames);

        var f = frameDelta;

        var x = startPos.X * (1 - f) + endPos.X * f;
        var y = startPos.Y * (1 - f) + endPos.Y * f;
        var z = startPos.Z * (1 - f) + endPos.Z * f;

        ModelViewMatrixTransformer.Translate(x, y, z);

        /*If indirectTextureHack IsNot Nothing Then
            Dim face As FacialState = CurrAnimation.GetFacialState(frameIndex)
        indirectTextureHack.EyeState = face.EyeState
        indirectTextureHack.MouthState = face.MouthState
        End If*/

        limbMatrices.UpdateLimbMatrices(limbs,
                                        animation,
                                        (float) animationPlaybackManager.Frame);
      }

      SkeletonRenderer.ForEachLimbRecursively_(
          limbs,
          0,
          (limb, limbIndex) => {
            var xI = 0.0;
            var yI = 0.0;
            var zI = 0.0;
            ModelViewMatrixTransformer.ProjectVertex(ref xI, ref yI, ref zI);

            double xF = limb.x;
            double yF = limb.y;
            double zF = limb.z;
            ModelViewMatrixTransformer.ProjectVertex(ref xF, ref yF, ref zF);

            Gl.glDepthRange(0, 0);
            Gl.glLineWidth(9);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3f(1, 1, 1);
            Gl.glVertex3d(xI, yI, zI);
            Gl.glVertex3d(xF, yF, zF);
            Gl.glEnd();
            Gl.glDepthRange(0, -0.5);
            Gl.glPointSize(11);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glColor3f(0, 0, 0);
            Gl.glVertex3d(xF, yF, zF);
            Gl.glEnd();
            /*Gl.glPointSize(8);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glColor3ub(BoneColorFactor.r,
                          BoneColorFactor.g,
                          BoneColorFactor.b);
            Gl.glVertex3f(xF, yF, zF);
            Gl.glEnd();*/
            Gl.glPointSize(1);
            Gl.glLineWidth(1);
            Gl.glDepthRange(0, 1);

            ModelViewMatrixTransformer.Push();

            var matrix = limbMatrices.GetMatrixForLimb((uint) limbIndex);
            ModelViewMatrixTransformer.Set(matrix);
          },
          (limb, _) => {
            ModelViewMatrixTransformer.Pop();
          });
      ModelViewMatrixTransformer.Pop();
    }

    private static void ForEachLimbRecursively_(
        IList<IOldLimb> limbs,
        sbyte limbIndex,
        Action<IOldLimb, sbyte> beforeChildren,
        Action<IOldLimb, sbyte> afterChildren) {
      var limb = limbs[limbIndex];
      beforeChildren(limb, limbIndex);

      var firstChildIndex = limb.firstChild;
      if (firstChildIndex > -1) {
        SkeletonRenderer.ForEachLimbRecursively_(limbs,
                                                 firstChildIndex,
                                                 beforeChildren,
                                                 afterChildren);
      }

      afterChildren(limb, limbIndex);

      var nextSiblingIndex = limb.nextSibling;
      if (nextSiblingIndex > -1) {
        SkeletonRenderer.ForEachLimbRecursively_(limbs,
                                                 nextSiblingIndex,
                                                 beforeChildren,
                                                 afterChildren);
      }
    }
  }
}