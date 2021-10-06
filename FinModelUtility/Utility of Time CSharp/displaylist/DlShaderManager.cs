using System;

using Tao.OpenGl;

using UoT.ui.main.viewer;
using UoT.util;

namespace UoT.displaylist {
  /// <summary>
  ///   Helper class for managing shader-specific fields and passing them into
  ///   OpenGL.
  /// </summary>
  public class DlShaderManager {
    // TODO: Clean all this up.
    // TODO: Fix rendering collision boundaries.
    // TODO: Fix lighting.
    // TODO: Support uv bounds mapping in the shader (for repeating+clamped textures).
    // TODO: Add support for reflective surfaces.
    // TODO: Looks like blend/fog colors are totally ignored right now.
    // TODO: Emulate cycle-dependent blender.
    // TODO: Support mirroring & clamping textures.

    private readonly DlShaderGenerator generator_ = new DlShaderGenerator();
    private ShaderCache[] FragShaderCache = new ShaderCache[0];

    /**
     * Important key terms, as sourced from:
     * https://wiki.cloudmodding.com/oot/F3DZEX2#Parameter_Description
     *
     * COMBINED: The current added-up total color.
     * PRIMITIVE: A global color, not sure where this is mainly used.
     * SHADE: The color calculated from the vertex colors that make up a
     *   triangle.
     * ENVIRONMENT: A global color, typically used for coloring in alpha
     *   textures like Link's tunic.
     */
    public DlShaderParams Params { get; set; } = new DlShaderParams();

    private void ResetColor_(float[] color)
      => this.SetColor_(color, 1, 1, 1, .5f);

    private int activeProgram_ = -1;

    private int timeLocation_ = -1;
    private int cameraPositionLocation_ = -1;

    private int lightingEnabledLocation_ = -1;
    private int sphericalUvEnabledLocation_ = -1;
    private int linearUvEnabledLocation_ = -1;

    private int texture0Location_ = -1;
    private int texture1Location_ = -1;

    public TextureParams TextureParams0 { get; } = new TextureParams();
    public TextureParams TextureParams1 { get; } = new TextureParams();

    private int envColorLocation_ = -1;
    private int primColorLocation_ = -1;
    private int blendLocation_ = -1;
    private int primColorLodLocation_ = -1;

    public int Uv0Location { get; private set; } = -1;
    public int Uv1Location { get; private set; } = -1;
    public int NormalLocation { get; private set; } = -1;
    public int ColorLocation { get; private set; } = -1;

    public class TextureParams {
      public bool ClampedU { get; set; }
      public bool ClampedV { get; set; }

      public bool MirroredU { get; set; }
      public bool MirroredV { get; set; }

      public float MinU { get; set; }
      public float MinV { get; set; }
      public float MaxU { get; set; }
      public float MaxV { get; set; }

      public int ClampedLocation { get; set; } = -1;
      public int MirroredLocation { get; set; } = -1;
      public int MinUvLocation { get; set; } = -1;
      public int MaxUvLocation { get; set; } = -1;

      public void GetLocations(int program, string name) {
        this.ClampedLocation =
            Gl.glGetUniformLocation(program, name + ".clamped");
        this.MirroredLocation =
            Gl.glGetUniformLocation(program, name + ".mirrored");
        this.MinUvLocation =
            Gl.glGetUniformLocation(program, name + ".minUv");
        this.MaxUvLocation =
            Gl.glGetUniformLocation(program, name + ".maxUv");
      }

      public void Bind() {
        Gl.glUniform2f(this.ClampedLocation,
                       this.ClampedU ? 1 : 0,
                       this.ClampedV ? 1 : 0);
        Gl.glUniform2f(this.MirroredLocation,
                       this.MirroredU ? 1 : 0,
                       this.MirroredV ? 1 : 0);
        Gl.glUniform2f(this.MinUvLocation, this.MinU, this.MinV);
        Gl.glUniform2f(this.MaxUvLocation, this.MaxU, this.MaxV);
      }
    }

    public void SetCombine(uint w0, uint w1) {
      if (GLExtensions.GLFragProg) {
        var ShaderCachePos = -1;
        this.Params.EnableCombiner = true;

        foreach (var cachedFragShader in this.FragShaderCache) {
          if (w0 == cachedFragShader.MUXS0 && w1 == cachedFragShader.MUXS1) {
            this.activeProgram_ = (int) cachedFragShader.FragShader;

            Gl.glUseProgram(this.activeProgram_);

            this.timeLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_, "time");
            this.cameraPositionLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_, "cameraPosition");

            this.lightingEnabledLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_,
                                        "u_lightingEnabled");
            this.sphericalUvEnabledLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_,
                                        "u_sphericalUvEnabled");
            this.linearUvEnabledLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_,
                                        "u_linearUvEnabled");


            this.texture0Location_ =
                Gl.glGetUniformLocation(this.activeProgram_, "texture0");
            this.texture1Location_ =
                Gl.glGetUniformLocation(this.activeProgram_, "texture1");

            this.TextureParams0.GetLocations(this.activeProgram_,
                                             "textureParams0");
            this.TextureParams1.GetLocations(this.activeProgram_,
                                             "textureParams1");

            this.envColorLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_, "EnvColor");
            this.primColorLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_, "PrimColor");
            this.blendLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_, "Blend");
            this.primColorLodLocation_ =
                Gl.glGetUniformLocation(this.activeProgram_, "PrimColorL");


            this.Uv0Location =
                Gl.glGetAttribLocation(this.activeProgram_, "in_uv0");
            this.Uv1Location =
                Gl.glGetAttribLocation(this.activeProgram_, "in_uv1");
            this.NormalLocation =
                Gl.glGetAttribLocation(this.activeProgram_, "in_normal");
            this.ColorLocation =
                Gl.glGetAttribLocation(this.activeProgram_, "in_color");

            return;
          }
        }

        this.DecodeMUX_(w0,
                        w1,
                        ref this.FragShaderCache,
                        this.FragShaderCache.Length);
      } else {
        Gl.glUseProgram(0);
        this.Params.EnableCombiner = false;
      }
    }

    private void SetColor_(float[] color, float r, float g, float b, float a) {
      color[0] = r;
      color[1] = g;
      color[2] = b;
      color[3] = a;
    }

    public void SetPrimaryColor(
        float primColorM,
        float primColorLOD,
        float r,
        float g,
        float b,
        float a) {
      this.Params.PrimColorM = primColorM;
      this.Params.PrimColorLOD = primColorLOD;
      this.SetColor_(this.Params.PrimColor, r, g, b, a);
    }

    public void SetEnvironmentColor(
        float r,
        float g,
        float b,
        float a)
      => this.SetColor_(this.Params.EnvironmentColor, r, g, b, a);

    public void SetBlendColor(
        float r,
        float g,
        float b,
        float a)
      => this.SetColor_(this.Params.BlendColor, r, g, b, a);

    public void SetFogColor(
        float r,
        float g,
        float b,
        float a)
      => this.SetColor_(this.Params.FogColor, r, g, b, a);

    public void Reset() {
      this.ResetColor_(this.Params.PrimColor);
      this.ResetColor_(this.Params.EnvironmentColor);
      this.ResetColor_(this.Params.BlendColor);
      this.ResetColor_(this.Params.FogColor);

      Gl.glDisable(Gl.GL_FRAGMENT_PROGRAM_ARB);
      Gl.glDisable(Gl.GL_CULL_FACE);
      Gl.glDisable(Gl.GL_TEXTURE_2D);
      Gl.glDisable(Gl.GL_BLEND);
      Gl.glDisable(Gl.GL_ALPHA_TEST);
      Gl.glBlendFunc(Gl.GL_ONE, Gl.GL_ZERO);
      Gl.glAlphaFunc(Gl.GL_GREATER, 0f);

      Gl.glDisable(Gl.GL_LIGHTING);
      Gl.glDisable(Gl.GL_NORMALIZE);

      this.Params.EnableCombiner = false;
      this.Params.EnableLighting = true;

      if (!this.Params.PrecompiledCombiner) {
        this.PrecompileMUXS_(RDP_Defs.G_COMBINERMUX0, RDP_Defs.G_COMBINERMUX1);
      }
    }

    public void PassValuesToShader() {
      if (this.Params.EnableCombiner) {
        Gl.glUniform1f(this.timeLocation_, (float) Time.Current);
        var camera = Asserts.Assert(Camera.Instance);
        Gl.glUniform3f(this.cameraPositionLocation_,
                       (float) camera.X,
                       (float) camera.Y,
                       (float) camera.Z);

        Gl.glUniform1f(this.lightingEnabledLocation_,
                       this.Params.EnableLighting ? 1 : 0);
        Gl.glUniform1f(this.sphericalUvEnabledLocation_,
                       this.Params.EnableSphericalUv ? 1 : 0);
        Gl.glUniform1f(this.linearUvEnabledLocation_,
                       this.Params.EnableLinearUv ? 1 : 0);

        Gl.glUniform4fv(this.envColorLocation_,
                        1,
                        this.Params.EnvironmentColor);
        Gl.glUniform4fv(this.primColorLocation_, 1, this.Params.PrimColor);
        Gl.glUniform4fv(this.blendLocation_, 1, this.Params.BlendColor);
        Gl.glUniform1f(this.primColorLodLocation_, this.Params.PrimColorLOD);

        Gl.glUniform1i(this.texture0Location_, 0);
        Gl.glUniform1i(this.texture1Location_, 1);
        this.TextureParams0.Bind();
        this.TextureParams1.Bind();
      } else {
        Gl.glDisable(Gl.GL_FRAGMENT_PROGRAM_ARB);
        Gl.glEnable(Gl.GL_LIGHTING);
        Gl.glEnable(Gl.GL_NORMALIZE);
        this.Params.MultiTexture = false;
        this.Params.EnableLighting = true;
      }
    }

    public void PrecompileMUXS_(uint[] MUXLIST1, uint[] MUXLIST2) {
      if (MUXLIST1.Length == MUXLIST2.Length) {
        for (int i = 0, loopTo = MUXLIST1.Length - 1; i <= loopTo; i++) {
          this.DecodeMUX_(MUXLIST1[i],
                          MUXLIST2[i],
                          ref this.FragShaderCache,
                          i);
        }
      }

      this.Params.PrecompiledCombiner = true;
    }

    private void DecodeMUX_(
        uint MUXS0,
        uint MUXS1,
        ref ShaderCache[] Cache,
        int CacheEntry) {
      this.UnpackMUX(MUXS0, MUXS1, this.Params.CombArg);
      Array.Resize(ref Cache, CacheEntry + 1);
      Cache[CacheEntry].MUXS0 = MUXS0;
      Cache[CacheEntry].MUXS1 = MUXS1;
      this.CreateShaderProgram_(2, ref Cache, CacheEntry);
    }

    public void UnpackMUX(
        uint MUXS0,
        uint MUXS1,
        UnpackedCombiner CC_Colors) {
      CC_Colors.cA[0] = (uint) (MUXS0 >> 20 & 0xFL);
      CC_Colors.cB[0] = (uint) (MUXS1 >> 28 & 0xFL);
      CC_Colors.cC[0] = (uint) (MUXS0 >> 15 & 0x1FL);
      CC_Colors.cD[0] = (uint) (MUXS1 >> 15 & 0x7L);
      CC_Colors.aA[0] = (uint) (MUXS0 >> 12 & 0x7L);
      CC_Colors.aB[0] = (uint) (MUXS1 >> 12 & 0x7L);
      CC_Colors.aC[0] = (uint) (MUXS0 >> 9 & 0x7L);
      CC_Colors.aD[0] = (uint) (MUXS1 >> 9 & 0x7L);
      CC_Colors.cA[1] = (uint) (MUXS0 >> 5 & 0xFL);
      CC_Colors.cB[1] = (uint) (MUXS1 >> 24 & 0xFL);
      CC_Colors.cC[1] = (uint) (MUXS0 >> 0 & 0x1FL);
      CC_Colors.cD[1] = (uint) (MUXS1 >> 6 & 0x7L);
      CC_Colors.aA[1] = (uint) (MUXS1 >> 21 & 0x7L);
      CC_Colors.aB[1] = (uint) (MUXS1 >> 3 & 0x7L);
      CC_Colors.aC[1] = (uint) (MUXS1 >> 18 & 0x7L);
      CC_Colors.aD[1] = (uint) (MUXS1 >> 0 & 0x7L);
    }

    private void CreateShaderProgram_(
        int cycles,
        ref ShaderCache[] cache,
        int entry)
      => cache[entry].FragShader =
             (uint) this.generator_.CreateShaderProgram(
                 cycles,
                 this.Params.CombArg);

    public void PassInVertexAttribs(IVertex vertex) {
      if (this.Params.EnableLighting) {
        if (!this.Params.EnableCombiner) {
          Gl.glColor4fv(this.Params.PrimColor);
        } else {
          Gl.glColor3f(1, 1, 1);
        }

        Gl.glVertexAttrib4f(this.ColorLocation, 1, 1, 1, 1);
        Gl.glNormal3f(vertex.NormalX, vertex.NormalY, vertex.NormalZ);
        Gl.glVertexAttrib3f(this.NormalLocation,
                            vertex.NormalX,
                            vertex.NormalY,
                            vertex.NormalZ);
      } else {
        Gl.glColor4ub(vertex.R, vertex.G, vertex.B, vertex.A);
        Gl.glVertexAttrib4f(this.ColorLocation,
                            vertex.R / 255.0F,
                            vertex.G / 255.0F,
                            vertex.B / 255.0F,
                            vertex.A / 255.0F);
        // Normal is invalid, but we have to pass a value in to prevent NaNs
        // when normalizing in the shader.
        Gl.glVertexAttrib3f(this.NormalLocation, 1, 1, 1);
      }
    }

    public void BindTextures(Texture? texture0, Texture? texture1) {
      Gl.glEnable(Gl.GL_TEXTURE_2D);
      if (texture0 != null) {
        Gl.glActiveTexture(Gl.GL_TEXTURE0);
        texture0.Bind();

        this.TextureParams0.ClampedU = texture0.GlClampedS;
        this.TextureParams0.ClampedV = texture0.GlClampedT;

        this.TextureParams0.MirroredU = texture0.GlMirroredS;
        this.TextureParams0.MirroredV = texture0.GlMirroredT;

        var tileDescriptor0 = texture0.TileDescriptor;
        this.TextureParams0.MaxU =
            (tileDescriptor0.LRS - tileDescriptor0.ULS + 1) /
            texture0.TileDescriptor.LoadWidth;
        this.TextureParams0.MaxV =
            (tileDescriptor0.LRT - tileDescriptor0.ULT + 1) /
            texture0.TileDescriptor.LoadHeight;

        this.TextureParams0.Bind();
      } else {
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, -1);
      }

      if (this.Params.MultiTexture) {
        Gl.glActiveTexture(Gl.GL_TEXTURE1);
        if (texture1 != null) {
          texture1.Bind();

          this.TextureParams1.ClampedU = texture1.GlClampedS;
          this.TextureParams1.ClampedV = texture1.GlClampedT;

          this.TextureParams1.MirroredU = texture1.GlMirroredS;
          this.TextureParams1.MirroredV = texture1.GlMirroredT;

          var tileDescriptor1 = texture1.TileDescriptor;
          this.TextureParams1.MaxU =
              (tileDescriptor1.LRS - tileDescriptor1.ULS + 1) /
              texture1.TileDescriptor.LoadWidth;
          this.TextureParams1.MaxV =
              (tileDescriptor1.LRT - tileDescriptor1.ULT + 1) /
              texture1.TileDescriptor.LoadHeight;

          this.TextureParams1.Bind();
        } else {
          Gl.glBindTexture(Gl.GL_TEXTURE_2D, -1);
        }
        Gl.glActiveTexture(Gl.GL_TEXTURE0);
      }
      Gl.glDisable(Gl.GL_TEXTURE_2D);
    }

    public void BindTextureUvs(
        IVertex vertex,
        TileDescriptor? tileDescriptor0,
        TileDescriptor? tileDescriptor1) {
      var u = (float) vertex.U;
      var v = (float) vertex.V;

      var u0 = u;
      var v0 = v;
      this.GetUv(tileDescriptor0, ref u0, ref v0);
      Gl.glVertexAttrib2f(this.Uv0Location, u0, v0);

      if (this.Params.MultiTexCoord) {
        var u1 = u;
        var v1 = v;
        this.GetUv(tileDescriptor1, ref u1, ref v1);
        Gl.glVertexAttrib2f(this.Uv1Location, u1, v1);
      }
    }

    public void GetUv(
        TileDescriptor? tileDescriptor,
        ref float u,
        ref float v) {
      /*'u = u*tileDescriptor.TextureWRatio*tileDescriptor.UScaling
      'v = v*tileDescriptor.TextureHRatio*tileDescriptor.VScaling +
           '    AnimatedTextureHacks.GetVOffsetForTexture(tileDescriptor) */
      if (tileDescriptor == null) {
        return;
      }

      u = (float) (u * tileDescriptor.Value.TextureWRatio);
      v = (float) (v * tileDescriptor.Value.TextureHRatio +
                   AnimatedTextureHacks.GetVOffsetForTexture(
                       tileDescriptor.Value));
    }
  }
}