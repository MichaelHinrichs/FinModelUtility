using fin.animation.playback;
using fin.gl;
using fin.math;
using fin.model;
using fin.model.util;
using fin.ui;
using fin.util.optional;
using OpenTK.Graphics.OpenGL;
using uni.config;
using uni.ui.gl;


namespace uni.ui.common.model {
  public class ModelViewerGlPanel : BGlPanel, IModelViewerPanel {
    private readonly Camera camera_ = new();
    private float fovY_ = 30;

    private readonly Color backgroundColor_ = Color.FromArgb(51, 128, 179);

    private GlShaderProgram texturedShaderProgram_;
    private int texture0Location_;

    private int useLightingLocation_;
    private bool hasNormals_;

    private GlShaderProgram texturelessShaderProgram_;

    private BackgroundSphereRenderer backgroundRenderer_ = new();
    private IModelRenderer? modelRenderer_;
    private SkeletonRenderer? skeletonRenderer_;
    private readonly BoneTransformManager boneTransformManager_ = new();

    private GridRenderer gridRenderer_ = new();

    private float scale_ = 1;

    private IModelFileBundle? modelFileBundle_;

    public (IModelFileBundle, IModel)? ModelAndFileBundle {
      get {
        var model = this.modelRenderer_?.Model;
        return model != null ? (this.modelFileBundle_!, model) : null;
      }
      set {
        this.modelFileBundle_ = value?.Item1;
        var model = value?.Item2;

        this.modelRenderer_?.Dispose();
        this.boneTransformManager_.Clear();

        if (model != null) {
          this.modelRenderer_ =
              new ModelRendererV2(model, this.boneTransformManager_);
          this.skeletonRenderer_ =
              new SkeletonRenderer(model.Skeleton, this.boneTransformManager_);
          this.boneTransformManager_.CalculateMatrices(
              model.Skeleton.Root,
              model.Skin.BoneWeights,
              null);
          this.scale_ = 1000 / ModelScaleCalculator.CalculateScale(
                            model, this.boneTransformManager_);

          hasNormals_ = false;
          foreach (var vertex in model.Skin.Vertices) {
            if (vertex.LocalNormal != null) {
              hasNormals_ = true;
              break;
            }
          }
        } else {
          this.modelRenderer_ = null;
          this.skeletonRenderer_ = null;
          this.scale_ = 1;
        }

        this.Animation = model?.AnimationManager.Animations.FirstOrDefault();
      }
    }

    private IModel? Model => this.ModelAndFileBundle?.Item2;

    public IAnimationPlaybackManager AnimationPlaybackManager { get; set; }

    private IAnimation? animation_;

    public IAnimation? Animation {
      get => this.animation_;
      set {
        if (this.animation_ == value) {
          return;
        }

        this.animation_ = value;
        if (this.AnimationPlaybackManager != null) {
          this.AnimationPlaybackManager.Frame = 0;
          this.AnimationPlaybackManager.FrameRate =
              (int)(value?.FrameRate ?? 20);
          this.AnimationPlaybackManager.TotalFrames =
              value?.FrameCount ?? 0;
        }
      }
    }

    private bool isMouseDown_ = false;
    private (int, int)? prevMousePosition_ = null;

    private bool isForwardDown_ = false;
    private bool isBackwardDown_ = false;
    private bool isLeftwardDown_ = false;
    private bool isRightwardDown_ = false;

    public ModelViewerGlPanel() {
      this.impl_.MouseDown += (_, args) => {
        if (args.Button == MouseButtons.Left) {
          isMouseDown_ = true;
          this.prevMousePosition_ = null;
        }
      };
      this.impl_.MouseUp += (_, args) => {
        if (args.Button == MouseButtons.Left) {
          isMouseDown_ = false;
        }
      };
      this.impl_.MouseMove += (_, args) => {
        if (this.isMouseDown_) {
          var mouseLocation = (args.X, args.Y);

          if (this.prevMousePosition_ != null) {
            var (prevMouseX, prevMouseY) = this.prevMousePosition_.Value;
            var (mouseX, mouseY) = mouseLocation;

            var deltaMouseX = mouseX - prevMouseX;
            var deltaMouseY = mouseY - prevMouseY;

            var fovY = this.fovY_;
            var fovX = fovY / this.Height * this.Width;

            var deltaXFrac = 1f * deltaMouseX / this.Width;
            var deltaYFrac = 1f * deltaMouseY / this.Height;

            var mouseSpeed = 3;

            this.camera_.Pitch -= deltaYFrac * fovY * mouseSpeed;
            this.camera_.Yaw -= deltaXFrac * fovX * mouseSpeed;
          }

          this.prevMousePosition_ = mouseLocation;
        }
      };

      this.impl_.KeyDown += (_, args) => {
        switch (args.KeyCode) {
          case Keys.W: {
            this.isForwardDown_ = true;
            break;
          }
          case Keys.S: {
            this.isBackwardDown_ = true;
            break;
          }
          case Keys.A: {
            this.isLeftwardDown_ = true;
            break;
          }
          case Keys.D: {
            this.isRightwardDown_ = true;
            break;
          }
        }
      };

      this.impl_.KeyUp += (_, args) => {
        switch (args.KeyCode) {
          case Keys.W: {
            this.isForwardDown_ = false;
            break;
          }
          case Keys.S: {
            this.isBackwardDown_ = false;
            break;
          }
          case Keys.A: {
            this.isLeftwardDown_ = false;
            break;
          }
          case Keys.D: {
            this.isRightwardDown_ = false;
            break;
          }
        }
      };
    }

    protected override void InitGl() {
      var vertexShaderSrc = @"
# version 120

in vec2 in_uv0;

varying vec4 vertexColor;
varying vec3 vertexNormal;
varying vec2 uv0;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexNormal = normalize(gl_ModelViewMatrix * vec4(gl_Normal, 0)).xyz;
    vertexColor = gl_Color;
    uv0 = gl_MultiTexCoord0.st;
}";

      var fragmentShaderSrc = @$"
# version 130 

uniform sampler2D texture0;
uniform float useLighting;

out vec4 fragColor;

in vec4 vertexColor;
in vec3 vertexNormal;
in vec2 uv0;

void main() {{
    vec4 texColor = texture(texture0, uv0);

    fragColor = texColor * vertexColor;

    vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));
    float diffuseLightAmount = {(DebugFlags.ENABLE_LIGHTING ? "max(-dot(vertexNormal, diffuseLightNormal), 0)" : "1")};

    float ambientLightAmount = .3;

    float lightAmount = min(ambientLightAmount + diffuseLightAmount, 1);

    fragColor.rgb = mix(fragColor.rgb, fragColor.rgb * lightAmount, useLighting);

    if (fragColor.a < .95) {{
      discard;
    }}
}}";

      this.texturedShaderProgram_ =
          GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);

      this.texture0Location_ =
          this.texturedShaderProgram_.GetUniformLocation("texture0");
      this.useLightingLocation_ =
          this.texturedShaderProgram_.GetUniformLocation("useLighting");

      this.texturelessShaderProgram_ =
          GlShaderProgram.FromShaders(@"
# version 120

varying vec4 vertexColor;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexColor = gl_Color;
}", @"
# version 130 

out vec4 fragColor;

in vec4 vertexColor;

void main() {
    fragColor = vertexColor;
}");

      ResetGl_();
    }

    private void ResetGl_() {
      GL.ShadeModel(ShadingModel.Smooth);
      GL.Enable(EnableCap.PointSmooth);
      GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

      GL.ClearDepth(5.0F);

      GL.DepthFunc(DepthFunction.Lequal);
      GL.Enable(EnableCap.DepthTest);
      GL.DepthMask(true);

      GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

      GL.Enable(EnableCap.Texture2D);
      GL.Enable(EnableCap.Normalize);

      GL.Enable(EnableCap.CullFace);
      GL.CullFace(CullFaceMode.Back);

      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      GL.ClearColor(backgroundColor_.R / 255f, backgroundColor_.G / 255f,
                    backgroundColor_.B / 255f, 1);
    }

    protected override void RenderGl() {
      var forwardVector =
          (this.isForwardDown_ ? 1 : 0) - (this.isBackwardDown_ ? 1 : 0);
      var rightwardVector =
          (this.isRightwardDown_ ? 1 : 0) - (this.isLeftwardDown_ ? 1 : 0);
      this.camera_.Move(forwardVector, rightwardVector, 15);

      var width = this.Width;
      var height = this.Height;
      GL.Viewport(0, 0, width, height);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      this.RenderPerspective_();
    }

    private void RenderPerspective_() {
      var width = this.Width;
      var height = this.Height;

      {
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GlUtil.Perspective(this.fovY_, 1.0 * width / height, 10, 10000);
        GlUtil.LookAt(this.camera_.X, this.camera_.Y, this.camera_.Z,
                      this.camera_.X + this.camera_.XNormal,
                      this.camera_.Y + this.camera_.YNormal,
                      this.camera_.Z + this.camera_.ZNormal,
                      0, 0, 1);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
      }

      {
        GL.Translate(this.camera_.X, this.camera_.Y, this.camera_.Z * .995f);
        this.backgroundRenderer_.Render();
        GL.LoadIdentity();
      }

      if (Config.Instance.ShowGrid) {
        this.texturelessShaderProgram_.Use();
        this.gridRenderer_.Render();
      }

      {
        GL.Rotate(90, 1, 0, 0);
        GL.Scale(this.scale_, this.scale_, this.scale_);
      }

      if (this.Animation != null) {
        this.AnimationPlaybackManager.Tick();

        var frame = (float)this.AnimationPlaybackManager.Frame;
        this.boneTransformManager_.CalculateMatrices(
            this.Model.Skeleton.Root,
            this.Model.Skin.BoneWeights,
            (this.Animation, frame),
            this.AnimationPlaybackManager.ShouldLoop);

        this.modelRenderer_?.InvalidateDisplayLists();

        var hiddenMeshes = this.modelRenderer_?.HiddenMeshes;

        hiddenMeshes?.Clear();
        var defaultDisplayState = Optional.Of(MeshDisplayState.VISIBLE);
        foreach (var (mesh, meshTracks) in this.Animation.MeshTracks) {
          var displayState =
              meshTracks.DisplayStates.GetInterpolatedFrame(
                  frame, defaultDisplayState);
          if (displayState.Assert() == MeshDisplayState.HIDDEN) {
            hiddenMeshes?.Add(mesh);
          }
        }
      }

      this.texturedShaderProgram_.Use();
      GL.Uniform1(this.texture0Location_, 0);
      GL.Uniform1(this.useLightingLocation_, hasNormals_ ? 1f : 0f);

      if (this.modelRenderer_ != null) {
        this.modelRenderer_.UseLighting = hasNormals_;
        this.modelRenderer_.Render();
      }

      if (Config.Instance.ShowSkeleton) {
        this.texturelessShaderProgram_.Use();
        this.skeletonRenderer_?.Render();
      }
    }
  }
}