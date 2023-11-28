using System.Numerics;

using fin.math.matrix.four;
using fin.math.matrix.three;
using fin.math.rotations;
using fin.model;
using fin.shaders.glsl;

using OpenTK.Graphics.OpenGL;

namespace fin.ui.rendering.gl.material {
  public class CachedTextureUniformData {
    public int TextureIndex { get; }
    public ITexture? FinTexture { get; }
    public GlTexture GlTexture { get; }

    public IReadOnlyFinMatrix3x2? Transform2d { get; }
    public IReadOnlyFinMatrix4x4? Transform3d { get; }

    public bool HasFancyData { get; }
    public int SamplerLocation { get; }
    public int ClampMinLocation { get; }
    public int ClampMaxLocation { get; }
    public int Transform2dLocation { get; }
    public int Transform3dLocation { get; }

    public CachedTextureUniformData(
        string textureName,
        int textureIndex,
        ITexture? finTexture,
        GlTexture glTexture,
        GlShaderProgram shaderProgram) {
      this.TextureIndex = textureIndex;
      this.FinTexture = finTexture;
      this.GlTexture = glTexture;

      this.HasFancyData = GlslUtil.RequiresFancyTextureData(finTexture);
      if (!this.HasFancyData) {
        this.SamplerLocation =
            shaderProgram.GetUniformLocation($"{textureName}");
      } else {
        this.SamplerLocation =
            shaderProgram.GetUniformLocation($"{textureName}.sampler");
        this.ClampMinLocation =
            shaderProgram.GetUniformLocation($"{textureName}.clampMin");
        this.ClampMaxLocation =
            shaderProgram.GetUniformLocation($"{textureName}.clampMax");
        this.Transform2dLocation =
            shaderProgram.GetUniformLocation($"{textureName}.transform2d");
        this.Transform3dLocation =
            shaderProgram.GetUniformLocation($"{textureName}.transform3d");
      }

      var isTransform3d = finTexture?.IsTransform3d ?? false;
      if (isTransform3d) {
        this.Transform3d = CalculateTextureTransform3d_(finTexture);
      } else {
        this.Transform2d = CalculateTextureTransform2d_(finTexture);
      }
    }

    public unsafe void BindTextureAndPassInUniforms() {
      this.GlTexture.Bind(this.TextureIndex);
      GL.Uniform1(this.SamplerLocation, this.TextureIndex);

      if (this.HasFancyData) {
        OpenTK.Vector2 clampMin = new(-10000);
        OpenTK.Vector2 clampMax = new(10000);

        if (this.FinTexture?.WrapModeU == WrapMode.MIRROR_CLAMP) {
          clampMin.X = -1;
          clampMax.X = 2;
        }

        if (this.FinTexture?.WrapModeV == WrapMode.MIRROR_CLAMP) {
          clampMin.Y = -1;
          clampMax.Y = 2;
        }

        var clampS = this.FinTexture?.ClampS;
        var clampT = this.FinTexture?.ClampT;

        if (clampS != null) {
          clampMin.X = clampS.X;
          clampMax.X = clampS.Y;
        }

        if (clampT != null) {
          clampMin.Y = clampT.X;
          clampMax.Y = clampT.Y;
        }

        GL.Uniform2(this.ClampMinLocation, clampMin);
        GL.Uniform2(this.ClampMaxLocation, clampMax);

        if (!(this.FinTexture?.IsTransform3d ?? false)) {
          var mat2d = this.Transform2d!.Impl;
          var ptr = (float*) &mat2d;
          GL.UniformMatrix2x3(this.Transform2dLocation, 1, true, ptr);
        } else {
          var mat3d = this.Transform3d!.Impl;
          GlTransform.UniformMatrix4(this.Transform3dLocation, mat3d);
        }
      }
    }

    private static IReadOnlyFinMatrix3x2 CalculateTextureTransform2d_(
    ITexture? texture) {
      if (texture == null) {
        return FinMatrix3x2.IDENTITY;
      }

      var textureOffset = texture.Offset;
      var textureScale = texture.Scale;
      var textureRotationRadians = texture.RotationRadians;

      if (textureOffset == null &&
          textureScale == null &&
          textureRotationRadians == null) {
        return FinMatrix3x2.IDENTITY;
      }

      Vector2? offset = null;
      if (textureOffset != null) {
        offset = new Vector2(textureOffset.X, textureOffset.Y);
      }

      Vector2? scale = null;
      if (textureScale != null) {
        scale = new Vector2(textureScale.X, textureScale.Y);
      }

      return FinMatrix3x2Util.FromTrss(offset,
                                       textureRotationRadians?.Z,
                                       scale,
                                       null);
    }

    private static IReadOnlyFinMatrix4x4 CalculateTextureTransform3d_(
        ITexture? texture) {
      if (texture == null) {
        return FinMatrix4x4.IDENTITY;
      }

      var textureOffset = texture.Offset;
      var textureScale = texture.Scale;
      var textureRotationRadians = texture.RotationRadians;

      if (textureOffset == null &&
          textureScale == null &&
          textureRotationRadians == null) {
        return FinMatrix4x4.IDENTITY;
      }

      Position? offset = null;
      if (textureOffset != null) {
        offset =
            new Position(textureOffset.X, textureOffset.Y, textureOffset.Z);
      }

      Quaternion? rotation = null;
      if (textureRotationRadians != null) {
        rotation = QuaternionUtil.CreateZyx(textureRotationRadians.X,
                                            textureRotationRadians.Y,
                                            textureRotationRadians.Z);
      }

      Scale? scale = null;
      if (textureScale != null) {
        scale = new(textureScale.X, textureScale.Y, textureScale.Z);
      }

      return FinMatrix4x4Util.FromTrs(offset, rotation, scale);
    }
  }
}