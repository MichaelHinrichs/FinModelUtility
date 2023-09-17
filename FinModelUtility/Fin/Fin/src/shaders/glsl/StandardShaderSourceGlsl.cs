using System.Text;

using fin.model;
using fin.model.extensions;

namespace fin.shaders.glsl {
  public class StandardShaderSourceGlsl : IShaderSourceGlsl {
    public StandardShaderSourceGlsl(
        IModel model,
        IStandardMaterial material,
        bool useBoneMatrices) {
      this.VertexShaderSource = GlslUtil.GetVertexSrc(model, useBoneMatrices);

      var hasNormalTexture = material.NormalTexture != null;
      var hasNormals = hasNormalTexture ||
                       model.Skin.HasNormalsForMaterial(material);

      var fragmentShaderSrc = new StringBuilder();
      fragmentShaderSrc.Append("# version 400");

      if (hasNormals) {
        fragmentShaderSrc.Append(
            $"""


             {GlslUtil.GetLightHeader(true)}
             """);
      }

      fragmentShaderSrc.Append(
          """


          uniform sampler2D diffuseTexture;
          """);

      if (hasNormalTexture) {
        fragmentShaderSrc.Append(
            """

            uniform sampler2D normalTexture;
            """);
      }

      fragmentShaderSrc.Append(
          $"""

           uniform sampler2D ambientOcclusionTexture;
           uniform sampler2D emissiveTexture;
           uniform float {GlslConstants.UNIFORM_USE_LIGHTING_NAME};

           out vec4 fragColor;

           in vec4 vertexColor0;
           """);

      if (hasNormals) {
        fragmentShaderSrc.Append(
            """

            in vec3 vertexNormal;
            in vec3 tangent;
            in vec3 binormal;
            """);
      }

      fragmentShaderSrc.Append(
          """

          in vec2 uv0;
          """);

      if (hasNormals) {
        fragmentShaderSrc.Append(
            $"""


             {GlslUtil.GetLightFunctions(true)}
             """);
      }

      fragmentShaderSrc.Append(
          """

          
          void main() {
            vec4 diffuseColor = texture(diffuseTexture, uv0);
            vec4 ambientOcclusionColor = texture(ambientOcclusionTexture, uv0);
            vec4 emissiveColor = texture(emissiveTexture, uv0);
          
            fragColor = diffuseColor * vertexColor0;
          """);

      if (hasNormals) {
        if (!hasNormalTexture) {
          fragmentShaderSrc.Append(
              """

                            
                vec3 fragNormal = vertexNormal;
              """);
        } else {
          fragmentShaderSrc.Append(
              """

              
                vec3 textureNormal = texture(normalTexture, uv0).xyz * 2 - 1;
                vec3 fragNormal = normalize(mat3(tangent, binormal, vertexNormal) * textureNormal);
              """);
        }

        // TODO: Is this right?
        fragmentShaderSrc.Append(
            $"""
             
             
               fragColor.rgb = mix(fragColor.rgb, applyLightingColor(fragColor.rgb, ambientOcclusionColor.r, fragNormal), {GlslConstants.UNIFORM_USE_LIGHTING_NAME});
             """);
      }

      // TODO: Is this right?
      fragmentShaderSrc.Append(
          """
          
            fragColor.rgb += emissiveColor.rgb;
            fragColor.rgb = min(fragColor.rgb, 1);
          
            if (fragColor.a < .95) {
              discard;
            }
          }
          """);

      this.FragmentShaderSource = fragmentShaderSrc.ToString();
    }

    public string VertexShaderSource { get; }
    public string FragmentShaderSource { get; set; }
  }
}