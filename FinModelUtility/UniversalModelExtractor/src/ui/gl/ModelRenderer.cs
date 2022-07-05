using fin.data;
using fin.gl;
using fin.language.equations.fixedFunction;
using fin.math;
using fin.model;
using fin.model.impl;
using fin.util.image;

using Tao.OpenGl;


namespace uni.ui.gl {
  /// <summary>
  ///   A renderer for a Fin model.
  /// </summary>
  public class ModelRenderer : IDisposable {
    private readonly BoneTransformManager boneTransformManager_;
    private readonly List<MaterialMeshRenderer> materialMeshRenderers_ = new();

    public ModelRenderer(IModel model,
                         BoneTransformManager boneTransformManager) {
      this.Model = model;
      this.boneTransformManager_ = boneTransformManager;

      var primitivesByMaterial = new ListDictionary<IMaterial, IPrimitive>();
      foreach (var mesh in model.Skin.Meshes) {
        foreach (var primitive in mesh.Primitives) {
          primitivesByMaterial.Add(primitive.Material, primitive);
        }
      }

      foreach (var (material, primitives) in primitivesByMaterial) {
        materialMeshRenderers_.Add(
            new MaterialMeshRenderer(
                this.boneTransformManager_,
                material,
                primitives));
      }
    }

    ~ModelRenderer() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
        materialMeshRenderer.Dispose();
      }
      materialMeshRenderers_.Clear();
    }

    public IModel Model { get; }

    public void Render() {
      foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
        materialMeshRenderer.Render();
      }
    }
  }

  /// <summary>
  ///   A renderer for all of the primitives of a Fin model with a common material.
  /// </summary>
  public class MaterialMeshRenderer : IDisposable {
    // TODO: Set up shader for material
    // TODO: Use material's textures

    private readonly GlShaderProgram? shaderProgram_;

    private static GlTexture? NULL_TEXTURE_;

    private readonly BoneTransformManager boneTransformManager_;

    private readonly IMaterial material_;
    private readonly IList<GlTexture> textures_;

    private readonly IList<IPrimitive> primitives_;


    public MaterialMeshRenderer(BoneTransformManager boneTransformManager,
                                IMaterial material,
                                IList<IPrimitive> primitives) {
      this.boneTransformManager_ = boneTransformManager;

      this.material_ = material;

      var fixedFunctionMaterial = material as IFixedFunctionMaterial;

      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && fixedFunctionMaterial != null) {
        // TODO: Sometimes vertex colors are passed in from model, and sometimes they
        // represent lighting. How to tell the difference??

        var vertexShaderSrc = @"
# version 120

in vec2 in_uv0;

varying vec3 vertexNormal;
varying vec2 normalUv;
varying vec4 vertexColor0_;
varying vec4 vertexColor1_;
varying vec2 uv0;
varying vec2 uv1;
varying vec2 uv2;
varying vec2 uv3;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex;
    vertexNormal = normalize(gl_ModelViewMatrix * vec4(gl_Normal, 0)).xyz;
    normalUv = normalize(gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(gl_Normal, 0)).xy;
    vertexColor0_ = vec4(0.5, 0.5, 0.5, 1);
    vertexColor1_ = vec4(0, 0, 0, 1);
    uv0 = gl_MultiTexCoord0.st;
    uv1 = gl_MultiTexCoord1.st;
    uv2 = gl_MultiTexCoord2.st;
    uv3 = gl_MultiTexCoord3.st;
}";

        var pretty =
            new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                .Print(fixedFunctionMaterial.Equations);

        var fragmentShaderSrc =
            new FixedFunctionEquationsGlslPrinter(
                    fixedFunctionMaterial.TextureSources)
                .Print(fixedFunctionMaterial);

        this.shaderProgram_ =
            GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);
      }

      if (MaterialMeshRenderer.NULL_TEXTURE_ == null) {
        MaterialMeshRenderer.NULL_TEXTURE_ =
            new GlTexture(BitmapUtil.Create1x1WithColor(Color.White));
      }

      IReadOnlyList<ITexture?> finTextures =
          fixedFunctionMaterial?.TextureSources ?? material.Textures;
      if (DebugFlags.ENABLE_WEIGHT_COLORS) {
        finTextures = Array.Empty<ITexture?>();
      }

      var nSupportedTextures = 8;
      this.textures_ = new List<GlTexture>();
      for (var i = 0; i < nSupportedTextures; ++i) {
        var finTexture =
            !DebugFlags.ENABLE_WEIGHT_COLORS && i < finTextures.Count
                ? finTextures[i]
                : null;

        this.textures_.Add(finTexture != null
                               ? new GlTexture(finTexture)
                               : MaterialMeshRenderer.NULL_TEXTURE_);
      }

      this.primitives_ = primitives;
    }

    ~MaterialMeshRenderer() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      foreach (var texture in this.textures_) {
        if (texture != MaterialMeshRenderer.NULL_TEXTURE_) {
          texture.Dispose();
        }
      }
      this.textures_.Clear();
      this.shaderProgram_?.Dispose();
    }

    public void Render() {
      if (this.shaderProgram_ != null) {
        this.shaderProgram_.Use();

        for (var t = 0; t < 8; ++t) {
          var textureLocation = Gl.glGetUniformLocation(
              this.shaderProgram_.ProgramId,
              $"texture{t}");
          Gl.glUniform1i(textureLocation, t);
        }
      }

      var fixedFunctionMaterial = this.material_ as IFixedFunctionMaterial;
      if (fixedFunctionMaterial != null) {
        GlUtil.SetBlending(fixedFunctionMaterial.BlendMode,
                           fixedFunctionMaterial.SrcFactor,
                           fixedFunctionMaterial.DstFactor,
                           fixedFunctionMaterial.LogicOp);
      }

      GlUtil.SetCulling(this.material_.CullingMode);
      for (var i = 0; i < this.textures_.Count; ++i) {
        this.textures_[i].Bind(i);
      }

      Gl.glBegin(Gl.GL_TRIANGLES);

      foreach (var primitive in this.primitives_) {
        var vertices = primitive.Vertices;
        var pointsCount = vertices.Count;

        switch (primitive.Type) {
          case PrimitiveType.TRIANGLES: {
            for (var v = 0; v < pointsCount; v += 3) {
              this.RenderVertex_(vertices[v + 0]);
              this.RenderVertex_(vertices[v + 2]);
              this.RenderVertex_(vertices[v + 1]);
            }
            break;
          }
          case PrimitiveType.TRIANGLE_STRIP: {
            for (var v = 0; v < pointsCount - 2; ++v) {
              IVertex v1, v2, v3;
              if (v % 2 == 0) {
                v1 = vertices[v + 0];
                v2 = vertices[v + 1];
                v3 = vertices[v + 2];
              } else {
                // Switches drawing order to maintain proper winding:
                // https://www.khronos.org/opengl/wiki/Primitive
                v1 = vertices[v + 1];
                v2 = vertices[v + 0];
                v3 = vertices[v + 2];
              }

              // Intentionally flipped to fix bug where faces were backwards.
              this.RenderVertex_(v1);
              this.RenderVertex_(v3);
              this.RenderVertex_(v2);
            }
            break;
          }
          case PrimitiveType.TRIANGLE_FAN: {
            // https://stackoverflow.com/a/8044252
            var firstVertex = vertices[0];
            for (var v = 2; v < pointsCount; ++v) {
              var v1 = firstVertex;
              var v2 = vertices[v - 1];
              var v3 = vertices[v];

              // Intentionally flipped to fix bug where faces were backwards.
              this.RenderVertex_(v1);
              this.RenderVertex_(v3);
              this.RenderVertex_(v2);
            }
            break;
          }
          case PrimitiveType.QUADS: {
            for (var v = 0; v < pointsCount; v += 4) {
              this.RenderVertex_(vertices[v + 0]);
              this.RenderVertex_(vertices[v + 1]);
              this.RenderVertex_(vertices[v + 2]);
              this.RenderVertex_(vertices[v + 3]);
            }
            break;
          }
        }
      }

      Gl.glEnd();

      for (var i = 0; i < this.textures_.Count; ++i) {
        this.textures_[i].Unbind(i);
      }

      if (fixedFunctionMaterial != null) {
        GlUtil.ResetBlending();
      }
    }

    private readonly IPosition position_ = new ModelImpl.PositionImpl();
    private readonly INormal normal_ = new ModelImpl.NormalImpl();

    private void RenderVertex_(IVertex vertex) {
      // TODO: Load in the matrix instead, so we can perform projection on the GPU.
      this.boneTransformManager_.ProjectVertex(
          vertex, position_, normal_, true);

      var color = vertex.GetColor();
      if (color != null) {
        Gl.glColor4f(color.Rf, color.Gf, color.Bf, color.Af);
      }

      if (DebugFlags.ENABLE_WEIGHT_COLORS) {
        float r = 0, g = 0, b = 0;

        if (vertex.BoneWeights == null) { } else if (vertex.BoneWeights.Weights
                                                         .Count == 0) {
          r = 1;
          g = 1;
        } else if (vertex.BoneWeights.Weights.Count == 1) {
          g = 1;
        } else {
          r = 1;
        }

        if (vertex.BoneWeights?.PreprojectMode == PreprojectMode.NONE) {
          b = 1;
        }

        Gl.glColor4f(r, g, b, 1);
      }

      for (var i = 0; i < 8; ++i) {
        var uvi = vertex.GetUv(i);
        Gl.glMultiTexCoord2f(i, uvi?.U ?? 0, uvi?.V ?? 0);
      }

      Gl.glNormal3f(normal_.X, normal_.Y, normal_.Z);
      Gl.glVertex3f(position_.X, position_.Y, position_.Z);
    }
  }
}