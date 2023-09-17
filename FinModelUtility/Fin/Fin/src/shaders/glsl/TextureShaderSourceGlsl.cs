using System.Text;

using fin.model;
using fin.model.extensions;

namespace fin.shaders.glsl {
  public class TextureShaderSourceGlsl : IShaderSourceGlsl {
    public TextureShaderSourceGlsl(IModel model,
                                   IReadOnlyMaterial material,
                                   bool useBoneMatrices) {
      this.VertexShaderSource = GlslUtil.GetVertexSrc(model, useBoneMatrices);

      var hasNormals = model.Skin.HasNormalsForMaterial(material);

      var fragmentSrc = new StringBuilder();
      fragmentSrc.Append("# version 400");

      if (hasNormals) {
        fragmentSrc.Append(
            $"""

             {GlslUtil.GetLightHeader(true)}
             """);
      }

      fragmentSrc.Append(
          $"""


           uniform sampler2D diffuseTexture;
           uniform float {GlslConstants.UNIFORM_USE_LIGHTING_NAME};

           out vec4 fragColor;

           in vec4 vertexColor0;
           """);

      if (hasNormals) {
        fragmentSrc.Append(
            """
            
            in vec3 vertexNormal;
            """);
      }

      fragmentSrc.Append(
          """

          in vec2 uv0;
          """);

      if (hasNormals) {
        fragmentSrc.Append(
            $"""

             {GlslUtil.GetLightFunctions(false)}
             """
        );
      }

      fragmentSrc.Append(
          """


          void main() {
            vec4 diffuseColor = texture(diffuseTexture, uv0);
          
            fragColor = diffuseColor * vertexColor0;
          """);

      if (hasNormals) {
        fragmentSrc.Append(
            $"""
             
               fragColor.rgb =
                   mix(fragColor.rgb, applyLightingColor(fragColor.rgb, vertexNormal),  {GlslConstants.UNIFORM_USE_LIGHTING_NAME});
             """);
      }

      fragmentSrc.Append(
          """

          
            if (fragColor.a < .95) {
              discard;
            }
          }
          """);

      this.FragmentShaderSource = fragmentSrc.ToString();
    }

    public string VertexShaderSource { get; }
    public string FragmentShaderSource { get; }
  }
}