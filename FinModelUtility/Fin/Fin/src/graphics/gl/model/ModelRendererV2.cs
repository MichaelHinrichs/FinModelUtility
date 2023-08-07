using fin.data;
using fin.math;
using fin.model;
using System;
using System.Collections.Generic;
using System.Linq;


namespace fin.graphics.gl.model {
  /// <summary>
  ///   A renderer for a Fin model.
  ///
  ///   NOTE: This will only be valid in the GL context this was first rendered in!
  /// </summary>
  public class ModelRendererV2 : IModelRenderer {
    // TODO: Require passing in a GL context in the constructor.

    private GlBufferManager? bufferManager_;
    private readonly IBoneTransformManager? boneTransformManager_;

    private readonly ListDictionary<IMesh, MaterialMeshRendererV2>
        materialMeshRenderers_ = new();

    public ModelRendererV2(
        IModel model,
        IBoneTransformManager? boneTransformManager = null) {
      this.Model = model;
      this.boneTransformManager_ = boneTransformManager;
    }

    // Generates buffer manager and model within the current GL context.
    private void GenerateModelIfNull_() {
      if (this.bufferManager_ != null) {
        return;
      }

      this.bufferManager_ = new GlBufferManager(this.Model);

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
          this.materialMeshRenderers_.Add(
              mesh,
              new MaterialMeshRendererV2(
                  this.boneTransformManager_,
                  this.bufferManager_,
                  this.Model,
                  material,
                  primitives.OrderBy(primitive => primitive.InversePriority)
                            .ToArray()) {
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