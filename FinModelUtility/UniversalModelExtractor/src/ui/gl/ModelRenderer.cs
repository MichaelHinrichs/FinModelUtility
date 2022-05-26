using fin.data;
using fin.math;
using fin.model;
using fin.model.impl;

using Tao.OpenGl;


namespace uni.ui.gl {
  /// <summary>
  ///   A renderer for a Fin model.
  /// </summary>
  public class ModelRenderer {
    private readonly BoneTransformManager boneTransformManager_ = new();
    public readonly List<MaterialMeshRenderer> materialMeshRenderers_ = new();

    public ModelRenderer(IModel model) {
      this.Model = model;

      var primitivesByMaterial = new ListDictionary<IMaterial, IPrimitive>();
      foreach (var mesh in model.Skin.Meshes) {
        foreach (var primitive in mesh.Primitives) {
          primitivesByMaterial.Add(primitive.Material, primitive);
        }
      }

      foreach (var (material, primitives) in primitivesByMaterial) {
        materialMeshRenderers_.Add(
            new MaterialMeshRenderer(
                this.boneTransformManager_,
                material,
                primitives));
      }

      this.boneTransformManager_.CalculateMatrices(
          this.Model.Skeleton.Root, null);
    }

    public IModel Model { get; }

    public void Render() {
      foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
        materialMeshRenderer.Render();
      }
    }
  }

  /// <summary>
  ///   A renderer for all of the primitives of a Fin model with a common material.
  /// </summary>
  public class MaterialMeshRenderer {
    // TODO: Set up shader for material
    // TODO: Use material's textures

    private readonly BoneTransformManager boneTransformManager_;
    private readonly IMaterial material_;
    private readonly IList<IPrimitive> primitives_;

    public MaterialMeshRenderer(BoneTransformManager boneTransformManager,
                                IMaterial material,
                                IList<IPrimitive> primitives) {
      this.boneTransformManager_ = boneTransformManager;
      this.material_ = material;
      this.primitives_ = primitives;
    }

    public void Render() {
      Gl.glBegin(Gl.GL_TRIANGLES);

      IPosition position = new ModelImpl.PositionImpl();
      INormal normal = new ModelImpl.NormalImpl();

      foreach (var primitive in this.primitives_) {
        switch (primitive.Type) {
          case PrimitiveType.TRIANGLES: {
            foreach (var vertex in primitive.Vertices) {
              this.boneTransformManager_.ProjectVertex(
                  vertex, position, normal);

              Gl.glNormal3f(normal.X, normal.Y, normal.Z);
              Gl.glVertex3f(position.X, position.Y, position.Z);
            }
            break;
          }
        }
      }
      Gl.glEnd();
    }
  }
}