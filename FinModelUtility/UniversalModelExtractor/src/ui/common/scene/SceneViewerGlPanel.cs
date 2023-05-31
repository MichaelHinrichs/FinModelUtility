﻿using fin.animation.playback;
using fin.gl;
using fin.gl.material;
using fin.gl.model;
using fin.io.bundles;
using fin.model;
using fin.scene;
using fin.ui;

using OpenTK.Graphics.OpenGL;
using uni.config;
using uni.model;
using uni.ui.gl;


namespace uni.ui.common.scene {
  public class SceneViewerGlPanel : BGlPanel, ISceneViewerPanel {
    private readonly Camera camera_ = Camera.NewLookingAt(0, 0, 0, 45, -10, 1.5f);

    private float fovY_ = 30;

    private readonly Color backgroundColor_ = Color.FromArgb(51, 128, 179);

    private BackgroundSphereRenderer backgroundRenderer_ = new();
    private GridRenderer gridRenderer_ = new();

    private float viewerScale_ = 1;

    private IScene? scene_;
    private IFileBundle? fileBundle_;

    public TimeSpan FrameTime { get; private set; }

    public (IFileBundle, IScene)? FileBundleAndScene {
      get {
        var scene = this.scene_;
        return scene != null ? (this.fileBundle_!, scene) : null;
      }
      set {
        if (value == null) {
          this.fileBundle_ = null;
          this.scene_ = null;
          this.viewerScale_ = 1;
        } else {
          this.fileBundle_ = value.Value.Item1;
          this.scene_ = value.Value.Item2;
          this.viewerScale_ = this.scene_.ViewerScale =
              new ScaleSource(Config.Instance.ViewerSettings.ViewerModelScaleSource).GetScale(
                  this.scene_,
                  this.fileBundle_);
        }
      }
    }

    private IScene? Scene => this.FileBundleAndScene?.Item2;

    public ISceneModel? FirstSceneModel
      => this.Scene?.Areas.FirstOrDefault()
             ?.Objects.FirstOrDefault()
             ?.Models.FirstOrDefault();

    public IAnimationPlaybackManager? AnimationPlaybackManager
      => this.FirstSceneModel?.AnimationPlaybackManager;

    public ISkeletonRenderer? SkeletonRenderer
      => this.FirstSceneModel?.SkeletonRenderer;

    public IAnimation? Animation {
      get => this.FirstSceneModel?.Animation;
      set {
        if (this.FirstSceneModel == null) {
          return;
        }

        this.FirstSceneModel.Animation = value;
      }
    }

    private bool isMouseDown_ = false;
    private (int, int)? prevMousePosition_ = null;

    private bool isForwardDown_ = false;
    private bool isBackwardDown_ = false;
    private bool isLeftwardDown_ = false;
    private bool isRightwardDown_ = false;
    private bool isSpeedupActive_ = false;

    public SceneViewerGlPanel() {
      this.impl_.MouseDown += (_, args) => {
        if (args.Button == MouseButtons.Left ||
            args.Button == MouseButtons.Right) {
          isMouseDown_ = true;
          this.prevMousePosition_ = null;
        }
      };
      this.impl_.MouseUp += (_, args) => {
        if (args.Button == MouseButtons.Left ||
            args.Button == MouseButtons.Right) {
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
          case Keys.ShiftKey: {
              this.isSpeedupActive_ = true;
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
          case Keys.ShiftKey: {
              this.isSpeedupActive_ = false;
              break;
            }
        }
      };
    }

    protected override void InitGl() {
      ResetGl_();
    }

    private void ResetGl_() => GlUtil.ResetGl(this.backgroundColor_);

    protected override void RenderGl() {
      var start = DateTime.Now;

      fin.util.time.FrameTime.MarkStartOfFrame();
      this.Scene?.Tick();

      var forwardVector =
          (this.isForwardDown_ ? 1 : 0) - (this.isBackwardDown_ ? 1 : 0);
      var rightwardVector =
          (this.isRightwardDown_ ? 1 : 0) - (this.isLeftwardDown_ ? 1 : 0);
      this.camera_.Move(forwardVector, rightwardVector,
                        DebugFlags.GLOBAL_SCALE *
                        (this.isSpeedupActive_ ? 30 : 15));

      var width = this.Width;
      var height = this.Height;
      GL.Viewport(0, 0, width, height);

      GlUtil.ResetDepth();
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      this.RenderPerspective_();

      var end = DateTime.Now;
      this.FrameTime = end - start;
    }

    private void RenderPerspective_() {
      var width = this.Width;
      var height = this.Height;

      {
        GlTransform.MatrixMode(MatrixMode.Projection);
        GlTransform.LoadIdentity();
        GlTransform.Perspective(this.fovY_, 1.0 * width / height,
                           DebugFlags.NEAR_PLANE, DebugFlags.FAR_PLANE);
        GlTransform.LookAt(this.camera_.X, this.camera_.Y, this.camera_.Z,
                      this.camera_.X + this.camera_.XNormal,
                      this.camera_.Y + this.camera_.YNormal,
                      this.camera_.Z + this.camera_.ZNormal,
                      0, 0, 1);

        GlTransform.MatrixMode(MatrixMode.Modelview);
        GlTransform.LoadIdentity();
      }

      {
        GlTransform.Translate(this.camera_.X, this.camera_.Y, this.camera_.Z * .995f);
        this.backgroundRenderer_.Render();
        GlTransform.LoadIdentity();
      }

      GlTransform.Scale(DebugFlags.GLOBAL_SCALE,
               DebugFlags.GLOBAL_SCALE,
               DebugFlags.GLOBAL_SCALE);

      if (Config.Instance.ViewerSettings.ShowGrid) {
        CommonShaderPrograms.TEXTURELESS_SHADER_PROGRAM.Use();
        this.gridRenderer_.Render();
      }

      {
        GlTransform.Rotate(90, 1, 0, 0);
        GlTransform.Scale(this.viewerScale_, this.viewerScale_, this.viewerScale_);
      }

      this.Scene?.Render();
    }
  }
}