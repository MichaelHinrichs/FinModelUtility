using fin.data.dictionaries;
using fin.math;
using fin.model;

namespace fin.ui.rendering.gl.model {
  /// <summary>
  ///   A renderer for a Fin model.
  ///
  ///   NOTE: This will only be valid in the GL context this was first rendered in!
  /// </summary>
  public class ModelRendererV2 : IModelRenderer {
    // TODO: Require passing in a GL context in the constructor.

    private GlBufferManager? bufferManager_;
    private readonly ILighting? lighting_;
    private readonly IBoneTransformManager? boneTransformManager_;

    private readonly ListDictionary<IMesh, MergedPrimitivesRenderer>
        materialMeshRenderers_ = new();

    public ModelRendererV2(
        IModel model,
        ILighting? lighting,
        IBoneTransformManager? boneTransformManager = null) {
      this.Model = model;
      this.lighting_ = lighting;
      this.boneTransformManager_ = boneTransformManager;
    }

    // Generates buffer manager and model within the current GL context.
    private void GenerateModelIfNull_() {
      if (this.bufferManager_ != null) {
        return;
      }

      this.bufferManager_ = new GlBufferManager(this.Model);

      var primitiveMerger = new PrimitiveMerger();
      foreach (var mesh in this.Model.Skin.Meshes) {
        var primitivesByMaterial = new ListDictionary<IMaterial, IPrimitive>();
        var prioritiesByMaterial = new SetDictionary<IMaterial, uint>();
        foreach (var primitive in mesh.Primitives) {
          primitivesByMaterial.Add(primitive.Material, primitive);
          prioritiesByMaterial.Add(primitive.Material,
                                   primitive.InversePriority);
        }

        var orderedMaterials =
            prioritiesByMaterial.OrderBy(pair => pair.Value.Order().First())
                                .Select(pair => pair.Key)
                                .ToArray();

        foreach (var material in orderedMaterials) {
          var primitives = primitivesByMaterial[material];
          if (!primitiveMerger.TryToMergePrimitives(
                  primitives,
                  out var mergedPrimitives)) {
            continue;
          }

          this.materialMeshRenderers_.Add(
              mesh,
              new MergedPrimitivesRenderer(
                  this.boneTransformManager_,
                  this.bufferManager_,
                  this.Model,
                  material,
                  this.lighting_,
                  mergedPrimitives) {
                  UseLighting = UseLighting
              });
        }
      }
    }

    ~ModelRendererV2() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      foreach (var (_, materialMeshRenderers) in this.materialMeshRenderers_) {
        foreach (var materialMeshRenderer in materialMeshRenderers) {
          materialMeshRenderer.Dispose();
        }
      }

      materialMeshRenderers_.Clear();
      this.bufferManager_?.Dispose();
    }

    public IModel Model { get; }
    public IReadOnlyDictionary<IMesh, IMeshTracks> MeshTracks { get; set; }
    public ISet<IMesh> HiddenMeshes { get; } = new HashSet<IMesh>();

    private bool useLighting_ = false;

    public bool UseLighting {
      get => this.useLighting_;
      set {
        this.useLighting_ = value;
        foreach (var (_, materialMeshRenderers) in
                 this.materialMeshRenderers_) {
          foreach (var materialMeshRenderer in materialMeshRenderers) {
            materialMeshRenderer.UseLighting = value;
          }
        }
      }
    }

    public void Render() {
      this.GenerateModelIfNull_();

      foreach (var (mesh, materialMeshRenderers) in
               this.materialMeshRenderers_) {
        if (this.HiddenMeshes?.Contains(mesh) ?? false) {
          continue;
        }

        foreach (var materialMeshRenderer in materialMeshRenderers) {
          materialMeshRenderer.Render();
        }
      }
    }
  }
}