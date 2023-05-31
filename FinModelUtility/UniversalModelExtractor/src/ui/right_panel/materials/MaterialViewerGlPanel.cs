using fin.gl;
using fin.gl.material;
using fin.model;
using fin.model.impl;

using OpenTK.Graphics.OpenGL;

using uni.ui.common;

namespace uni.ui.right_panel.materials {
  public class MaterialViewerGlPanel : BGlPanel, IMaterialViewerPanel {
    private readonly Color backgroundColor_ = Color.FromArgb(51, 128, 179);

    private IMaterial? material_;
    private IGlMaterialShader? materialShader_;

    public IMaterial? Material {
      get => this.material_;
      set {
        this.material_ = value;
        this.materialShader_?.Dispose();
        this.materialShader_ = null;
      }
    }

    private ModelImpl viewerModel_;
    private GlBufferManager bufferManager_;
    private GlBufferManager.GlBufferRenderer bufferRenderer_;

    protected override void InitGl() {
      this.ResetGl_();

      this.viewerModel_ = new ModelImpl(4);

      var ul = this.viewerModel_.Skin.AddVertex(0, 0, 0);
      ul.SetUv(0, 0);

      var ur = this.viewerModel_.Skin.AddVertex(1, 0, 0);
      ur.SetUv(1, 0);

      var ll = this.viewerModel_.Skin.AddVertex(0, 1, 0);
      ll.SetUv(0, 1);

      var lr = this.viewerModel_.Skin.AddVertex(1, 1, 0);
      lr.SetUv(1, 1);

      this.bufferManager_ = new GlBufferManager(this.viewerModel_);
      this.bufferRenderer_ = this.bufferManager_.CreateRenderer(fin.model.PrimitiveType.TRIANGLES, new[] {
          ll,
          ur,
          ul,

          ll,
          lr,
          ur
      });
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

      GL.Enable(EnableCap.Normalize);

      GL.Enable(EnableCap.CullFace);
      GL.CullFace(CullFaceMode.Back);

      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      GL.ClearColor(this.backgroundColor_.R / 255f, this.backgroundColor_.G / 255f,
                    this.backgroundColor_.B / 255f, 1);
    }

    protected override void RenderGl() {
      var width = this.Width;
      var height = this.Height;
      GL.Viewport(0, 0, width, height);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      if (this.Material == null) {
        this.materialShader_?.Dispose();
        this.materialShader_ = null;
      } else {
        if (this.materialShader_ == null) {
          this.materialShader_ =
              GlMaterialShader.FromMaterial(this.viewerModel_, this.Material);
          this.materialShader_.DisposeTextures = false;
        }

        this.RenderOrtho_();
      }
    }

    private void RenderOrtho_() {
      var width = this.Width;
      var height = this.Height;

      {
        GlTransform.MatrixMode(MatrixMode.Projection);
        GlTransform.LoadIdentity();
        GlTransform.Ortho2d(0, width, height, 0);

        GlTransform.MatrixMode(MatrixMode.Modelview);
        GlTransform.LoadIdentity();
      }

      var size = MathF.Min(width, height);
      GlTransform.Translate(width / 2.0, height / 2.0, 0);
      GlTransform.Scale(size, size, 1);
      GlTransform.Translate(-.5, -.5, 0);
      GlTransform.PassMatricesIntoGl();
      
      this.materialShader_.Use();

      this.bufferRenderer_.Render();
    }
  }
}