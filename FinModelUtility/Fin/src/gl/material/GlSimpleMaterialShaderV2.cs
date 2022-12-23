using System;

using fin.model;
using fin.model.util;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlSimpleMaterialShaderV2 : IGlMaterialShader {
    private static GlShaderProgram impl_;
    private static int diffuseTextureLocation_;
    private static int modelViewMatrixLocation_;
    private static int projectionMatrixLocation_;
    private static int useLightingLocation_;

    private readonly GlTexture primaryGlTexture_;

    public GlSimpleMaterialShaderV2(IMaterial material) {
      this.Material = material;

      if (impl_ == null) {
        var fragmentShaderSrc = @$"
# version 330

uniform sampler2D diffuseTexture;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexColor0;
in vec3 vertexNormal;
in vec2 uv;

void main() {{
    vec4 diffuseColor = texture(diffuseTexture, uv);

    fragColor = diffuseColor * vertexColor0;

    vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));
    float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);

    float ambientLightAmount = .3;

    float lightAmount = min(ambientLightAmount + diffuseLightAmount, 1);

    fragColor.rgb = mix(fragColor.rgb, fragColor.rgb * lightAmount, useLighting);

    if (fragColor.a < .95) {{
      discard;
    }}
}}";

        impl_ =
          GlShaderProgram.FromShaders(CommonShaderPrograms.VERTEX_SRC, fragmentShaderSrc);

        diffuseTextureLocation_ = impl_.GetUniformLocation("diffuseTexture");
        modelViewMatrixLocation_ = impl_.GetUniformLocation("modelViewMatrix");
        projectionMatrixLocation_ = impl_.GetUniformLocation("projectionMatrix");
        useLightingLocation_ = impl_.GetUniformLocation("useLighting");
      }


      var primaryFinTexture = PrimaryTextureFinder.GetFor(material);
      this.primaryGlTexture_ = primaryFinTexture != null
                                   ? new GlTexture(primaryFinTexture)
                                   : GlMaterialConstants.NULL_WHITE_TEXTURE;
    }

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      GlMaterialConstants.DisposeIfNotCommon(this.primaryGlTexture_);
    }


    public IMaterial Material { get; }

    public bool UseLighting { get; set; }

    public void Use() {
      impl_.Use();

      GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 modelViewMatrix);
      GL.UniformMatrix4(modelViewMatrixLocation_,
                        false, ref modelViewMatrix);

      GL.GetFloat(GetPName.ProjectionMatrix, out Matrix4 projectionMatrix);
      GL.UniformMatrix4(projectionMatrixLocation_, false, ref projectionMatrix);

      GL.Uniform1(diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();

      GL.Uniform1(useLightingLocation_, this.UseLighting ? 1f : 0f);
    }
  }
}