using System;
using System.Text;

using Tao.OpenGl;

namespace UoT.displaylist {
  public class DlShaderGenerator {
    // TODO: Lighting still seems a smidge off.
    // TODO: Fix shadows on cheeks, bad conversion of byte to float?
    // TODO: Alpha textures hide what's behind.

    public int CreateShaderProgram(int cycles, UnpackedCombiner combArg) {
      var vertexShader = this.CreateVertexShader_();
      var fragmentShader = this.CreateFragmentShader_(cycles, combArg);

      var program = Gl.glCreateProgram();
      Gl.glAttachShader(program, vertexShader);
      Gl.glAttachShader(program, fragmentShader);
      Gl.glLinkProgram(program);

      Gl.glGetProgramiv(program, Gl.GL_LINK_STATUS, out var linked);
      if (linked == Gl.GL_FALSE) {
        Gl.glGetProgramiv(program, Gl.GL_INFO_LOG_LENGTH, out var logSize);

        var logBuilder = new StringBuilder(logSize);
        Gl.glGetProgramInfoLog(program, logSize, out _, logBuilder);

        throw new Exception(Environment.NewLine + logBuilder);
      }

      return program;
    }

    private string CreateUvMethods() {
      return
          @"vec2 calculateSphericalUv(vec3 p_projectedNormal) {
              return asin(p_projectedNormal.xy) / 3.14159 + 0.5;
            }

            vec2 calculateLinearUv(vec3 p_projectedNormal) {
              return acos(p_projectedNormal.xy) / 3.14159;
            }

            vec2 calculateClampedUv(vec2 p_uv, TextureParams p_texParams) {
              return clamp(p_uv, vec2(0), p_texParams.maxUv);
            }

            vec2 calculateUv(vec2 p_uv, TextureParams p_texParams, vec3 p_projectedNormal) {
              vec2 pOrClampedUv = mix(p_uv, calculateClampedUv(p_uv, p_texParams), p_texParams.clamped);

              vec2 sphericalUv = calculateSphericalUv(p_projectedNormal);
              vec2 linearUv = calculateLinearUv(p_projectedNormal);

              vec2 outUv = pOrClampedUv;
              outUv = mix(outUv, sphericalUv, u_sphericalUvEnabled);
              outUv = mix(outUv, linearUv, u_linearUvEnabled);

              return outUv;
            }" + '\n';
    }

    private int CreateVertexShader_() {
      // TODO: Does this spherical UV need to take camera into account?
      var vertexShaderLines =
          @"#version 130
            uniform float u_sphericalUvEnabled;

            //struct TextureParams {
            //  vec2 clamped;
            //  vec2 mirrored;
            //
            //  vec2 maxUv;
            //};

            //uniform TextureParams textureParams0;
            //uniform TextureParams textureParams1;

            in vec3 in_position;
            in vec2 in_uv0;
            in vec2 in_uv1;
            in vec3 in_normal;
            in vec4 in_color;

            out vec3 v_position;
            out vec2 v_uv0;
            out vec2 v_uv1;
            out vec3 v_normal;
            out vec3 v_projectedNormal;
            out vec4 v_shade;" +
          '\n';

      //vertexShaderLines += this.CreateUvMethods();

      vertexShaderLines += 
          @"void main() {
              gl_Position = gl_ModelViewProjectionMatrix * vec4(in_position, 1f);

              // in_position should already be in world space, since we
              // project the points w/ software matrices.
              v_position = in_position;

              v_uv0 = in_uv0;
              v_uv1 = in_uv1;
              //v_uv0 = calculateUv(in_uv0, textureParams0, projectedNormal);
              //v_uv1 = calculateUv(in_uv1, textureParams1, projectedNormal);

              v_normal = in_normal;
              v_projectedNormal = normalize(gl_ModelViewProjectionMatrix * vec4(in_normal, 0f)).xyz;

              v_shade = in_color;
            }";

      return ShaderCompiler.Compile(Gl.GL_VERTEX_SHADER, vertexShaderLines);
    }

    private int CreateFragmentShader_(
        int cycles,
        UnpackedCombiner combArg) {
      var newLine = '\n';

      // TODO: Shade might need to be an "in" instead?
      var fragmentHeaderLines =
          @"#version 400

            uniform float time;
            uniform vec3 cameraPosition;

            uniform float u_lightingEnabled;
            uniform float u_sphericalUvEnabled;
            uniform float u_linearUvEnabled;

            struct TextureParams {
              vec2 clamped;
              vec2 mirrored;

              vec2 maxUv;
            };

            uniform TextureParams textureParams0;
            uniform TextureParams textureParams1;

            uniform vec4 EnvColor;
            uniform vec4 PrimColor;
            uniform vec4 Blend;
            uniform float PrimColorL;

            uniform sampler2D texture0;
            uniform sampler2D texture1;

            in vec3 v_position;
            in vec2 v_uv0;
            in vec2 v_uv1;
            in vec3 v_normal;
            in vec3 v_projectedNormal;
            in vec4 v_shade;

            out vec4 outColor;" +
          newLine;

      fragmentHeaderLines += this.CreateUvMethods();

      // TODO: Allow changing light position.
      var fragmentMainLines =
          @"vec4 applyLighting(vec4 p_color) {
              vec3 ambientLightColor = vec3(1, 1, 1);
              vec3 diffuseLightColor = vec3(1, 1, 1);
              vec3 specularLightColor = vec3(1, 1, 1);

              // Ambient color
              float ambientLightStrength = .2;
              vec3 ambientColor = ambientLightStrength * ambientLightColor;

              // Diffuse color
              vec3 normal = v_normal;

              //vec3 lightPos = 10000 * vec3(1, 1, 1);
              //vec3 lightDir = normalize(lightPos - v_position);  
              vec3 lightDir = normalize(vec3(-1, 0, 0));

              float diff = max(dot(normal, lightDir), 0);
              //float diff = .5 + .5 * dot(normal, lightDir);
              vec3 diffuseColor = diff * diffuseLightColor;

              // Specular color
              float specularStrength = 0.5;
              
              vec3 mergedColor = ambientColor + diffuseColor;
              vec3 finalColor = mergedColor * p_color.rgb;

              return vec4(finalColor, p_color.a);
            }

            vec4 calculateCcmuxColor(void) {
              vec3 CCRegister_0;
              vec3 CCRegister_1;
              vec3 CCReg;
              float ACRegister_0;
              float ACRegister_1;
              float ACReg;

              vec3 projectedNormal = normalize(v_projectedNormal);
              vec2 uv0 = calculateUv(v_uv0, textureParams0, projectedNormal);
              vec2 uv1 = calculateUv(v_uv1, textureParams1, projectedNormal);

              vec4 Texel0 = texture(texture0, uv0);
              vec4 Texel1 = texture(texture1, uv1);" +
          newLine;

      for (var i = 0; i < cycles; ++i) {
        // Final color = (ColorA [base] - ColorB) * ColorC + ColorD
        fragmentMainLines +=
            DlShaderGenerator.MovC_('a',
                                    "CCRegister_0",
                                    combArg.cA[i]);
        fragmentMainLines +=
            DlShaderGenerator.MovC_('b',
                                    "CCRegister_1",
                                    combArg.cB[i]);
        fragmentMainLines += "CCRegister_0 = CCRegister_0 - CCRegister_1;" +
                             newLine +
                             newLine;

        fragmentMainLines +=
            DlShaderGenerator.MovC_('c',
                                    "CCRegister_1",
                                    combArg.cC[i]);
        fragmentMainLines += "CCRegister_0 = CCRegister_0 * CCRegister_1;" +
                             newLine;

        fragmentMainLines +=
            DlShaderGenerator.MovC_('d',
                                    "CCRegister_1",
                                    combArg.cD[i]);
        fragmentMainLines += "CCRegister_0 = CCRegister_0 + CCRegister_1;" +
                             newLine +
                             newLine;


        fragmentMainLines +=
            DlShaderGenerator.MovA_('a', "ACRegister_0", combArg.aA[i]) +
            newLine;
        fragmentMainLines +=
            DlShaderGenerator.MovA_('b', "ACRegister_1", combArg.aB[i]) +
            newLine;
        fragmentMainLines +=
            "ACRegister_0 = ACRegister_0 - ACRegister_1;" +
            newLine;

        fragmentMainLines +=
            DlShaderGenerator.MovA_('c', "ACRegister_1", combArg.aC[i]) +
            newLine;
        fragmentMainLines +=
            "ACRegister_0 = ACRegister_0 * ACRegister_1;" +
            newLine;

        fragmentMainLines +=
            DlShaderGenerator.MovA_('d', "ACRegister_1", combArg.aD[i]) +
            newLine;
        fragmentMainLines += "ACRegister_0 = ACRegister_0 + ACRegister_1;" +
                             newLine +
                             newLine;

        fragmentMainLines += @"CCReg = CCRegister_0;
                               ACReg = ACRegister_0;" +
                             newLine;
      }

      // TODO: Is this the right shadow calculation?
      fragmentMainLines +=
          @"return vec4(CCReg, ACReg);
          }

          void main(void) {
            vec4 ccmuxColor = calculateCcmuxColor();

            outColor = mix(ccmuxColor, applyLighting(ccmuxColor), u_lightingEnabled);
          }";

      var fragmentShaderLines = fragmentHeaderLines + fragmentMainLines;

      return ShaderCompiler.Compile(Gl.GL_FRAGMENT_SHADER, fragmentShaderLines);
    }

    private static string MovC_(
        char letter,
        string target,
        uint value) {
      return $"{target} = {DlShaderGenerator.CToValue_(letter, value)};" +
             Environment.NewLine;
    }

    private static string CToValue_(char letter, uint value) {
      switch (value) {
        case (uint) RDP.G_CCMUX_COMBINED:
          return "CCReg.rgb";

        case (uint) RDP.G_CCMUX_TEXEL0:
          return "Texel0.rgb";

        case (uint) RDP.G_CCMUX_TEXEL1:
          return "Texel1.rgb";

        case (uint) RDP.G_CCMUX_PRIMITIVE:
          return "PrimColor.rgb";

        case (uint) RDP.G_CCMUX_SHADE:
          return "v_shade.rgb";

        case (uint) RDP.G_CCMUX_ENVIRONMENT:
          return "EnvColor.rgb";
      }

      switch (letter) {
        case 'a':
          // TODO: Support noise mux (white noise)?
          switch (value) {
            case (uint) RDP.G_CCMUX_1:
            case (uint) RDP.G_CCMUX_NOISE:
              return "vec3(1)";
          }
          break;

        case 'b':
          switch (value) {
            case (uint) RDP.G_CCMUX_CENTER:
            case (uint) RDP.G_CCMUX_K4:
              return "vec3(1)";
          }
          break;

        case 'c':
          switch (value) {
            case (uint) RDP.G_CCMUX_COMBINED_ALPHA:
              return "vec3(CCReg.a)";

            case (uint) RDP.G_CCMUX_TEXEL0_ALPHA:
              return "vec3(Texel0.a)";

            case (uint) RDP.G_CCMUX_TEXEL1_ALPHA:
              return "vec3(Texel1.a)";

            case (uint) RDP.G_CCMUX_PRIMITIVE_ALPHA:
              return "vec3(PrimColor.a)";

            case (uint) RDP.G_CCMUX_SHADE_ALPHA:
              return "vec3(v_shade.a)";

            case (uint) RDP.G_CCMUX_ENV_ALPHA:
              return "vec3(EnvColor.a)";

            case (uint) RDP.G_CCMUX_PRIM_LOD_FRAC:
              return "vec3(PrimColorL)";

            case (uint) RDP.G_CCMUX_SCALE:
            case (uint) RDP.G_CCMUX_K5:
              return "vec3(1)";
          }
          break;

        case 'd':
          switch (value) {
            case (uint) RDP.G_CCMUX_1:
              return "vec3(1)";
          }
          break;
      }

      return "vec3(0)";
    }

    private static string MovA_(char letter, string target, uint value) {
      return $"{target} = {DlShaderGenerator.AToValue_(letter, value)};" +
             Environment.NewLine;
    }

    private static string AToValue_(char letter, uint value) {
      switch (value) {
        case (uint) RDP.G_ACMUX_TEXEL0:
          return "Texel0.a";

        case (uint) RDP.G_ACMUX_TEXEL1:
          return "Texel1.a";

        case (uint) RDP.G_ACMUX_PRIMITIVE:
          return "PrimColor.a";

        case (uint) RDP.G_ACMUX_SHADE:
          return "v_shade.a";

        case (uint) RDP.G_ACMUX_ENVIRONMENT:
          return "EnvColor.a";
      }

      if (letter == 'a' || letter == 'b' || letter == 'd') {
        switch (value) {
          case (uint) RDP.G_ACMUX_COMBINED:
            return "ACReg";

          case (uint) RDP.G_ACMUX_1:
            return "1";
        }
      } else {
        switch (value) {
          case (uint) RDP.G_ACMUX_PRIM_LOD_FRAC:
            return "PrimColorL";

          case (uint) RDP.G_ACMUX_LOD_FRACTION:
            return "1";
        }
      }

      return "0";
    }
  }
}