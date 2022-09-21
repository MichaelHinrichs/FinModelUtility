using fin.data;
using fin.math;
using fin.model;


namespace uni.ui.gl {
  /// <summary>
  ///   A renderer for a Fin model.
  /// </summary>
  public class ModelRendererV2 : IModelRenderer {
    private readonly GlBufferManager bufferManager_;
    private readonly BoneTransformManager? boneTransformManager_;
    private readonly List<MaterialMeshRendererV2> materialMeshRenderers_ = new();

    public ModelRendererV2(IModel model,
                         BoneTransformManager? boneTransformManager = null) {
      this.Model = model;
      this.boneTransformManager_ = boneTransformManager;

      this.bufferManager_ = new GlBufferManager(model);

      var primitivesByMaterial = new ListDictionary<IMaterial, IPrimitive>();
      foreach (var mesh in model.Skin.Meshes) {
        foreach (var primitive in mesh.Primitives) {
          primitivesByMaterial.Add(primitive.Material, primitive);
        }
      }

      foreach (var (material, primitives) in primitivesByMaterial) {
        materialMeshRenderers_.Add(
            new MaterialMeshRendererV2(
                this.bufferManager_,
                material,
                primitives));
      }
    }

    ~ModelRendererV2() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
        materialMeshRenderer.Dispose();
      }
      materialMeshRenderers_.Clear();
      this.bufferManager_.Dispose();
    }

    public IModel Model { get; }

    private bool useLighting_ = false;

    public bool UseLighting {
      get => this.useLighting_;
      set {
        this.useLighting_ = value;
        foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
          materialMeshRenderer.UseLighting = value;
        }
      }
    }

    private bool valid_ = false;
    public void InvalidateDisplayLists() {
      this.valid_ = false;
    }

    public void Render() {
      if (!this.valid_) {
        this.bufferManager_.UpdateTransforms(this.boneTransformManager_);
        this.valid_ = true;
      }

      foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
        materialMeshRenderer.Render();
      }
    }
  }
}