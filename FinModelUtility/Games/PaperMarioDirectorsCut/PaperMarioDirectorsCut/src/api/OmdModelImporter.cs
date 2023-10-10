using System.Numerics;

using fin.image;
using fin.io;
using fin.model;
using fin.model.impl;
using fin.model.io;
using fin.model.io.importers;

using pmdc.schema.omd;

namespace pmdc.api {
  public class OmdModelFileBundle : IModelFileBundle {
    public string? GameName { get; }

    public required IReadOnlyTreeFile OmdFile { get; init; }
    public IReadOnlyTreeFile MainFile => this.OmdFile;
  }

  public class OmdModelImporter : IModelImporter<OmdModelFileBundle> {
    public IModel ImportModel(OmdModelFileBundle modelFileBundle) {
      var omdFile = modelFileBundle.OmdFile;

      var omdModel = omdFile.ReadNewFromText<Omd>();
      var finModel = new ModelImpl<NormalUvVertexImpl>(
          (index, position) => new NormalUvVertexImpl(index, position));

      var finSkeleton = finModel.Skeleton;
      var finRoot = finSkeleton.Root;
      finRoot.SetLocalRotationDegrees(-90, 0, 0);

      var finMaterialManager = finModel.MaterialManager;
      var finMaterials =
          omdModel
              .Materials
              .Select(omdMaterial => {
                var texturePath = omdMaterial.TexturePath;

                IMaterial finMaterial;
                if (texturePath.Length == 0 || !omdFile.AssertGetParent()
                        .TryToGetExistingFile(texturePath, out var imageFile)) {
                  finMaterial = finMaterialManager.AddNullMaterial();
                } else {
                  var image = FinImage.FromFile(imageFile);

                  var finTexture = finMaterialManager.CreateTexture(image);
                  finTexture.WrapModeU = WrapMode.REPEAT;
                  finTexture.WrapModeV = WrapMode.REPEAT;

                  finMaterial =
                      finMaterialManager.AddTextureMaterial(finTexture);
                }

                finMaterial.Name = omdMaterial.Name;

                return finMaterial;
              })
              .ToArray();

      var finSkin = finModel.Skin;
      foreach (var omdMesh in omdModel.Meshes) {
        var finMesh = finSkin.AddMesh();
        finMesh.Name = omdMesh.Name;

        var finVertices =
            omdMesh
                .Vertices
                .Where(omdVertex => omdVertex.Something == 8)
                .Select(omdVertex => {
                  var finVertex = finSkin.AddVertex(omdVertex.Position);
                  finVertex.SetLocalNormal(Vector3.Negate(omdVertex.Normal));
                  finVertex.SetUv(omdVertex.Uv);
                  finVertex.SetBoneWeights(
                      finSkin.GetOrCreateBoneWeights(
                          VertexSpace.WORLD,
                          finRoot));

                  return finVertex;
                })
                .ToArray();

        var finPrimitive = finMesh.AddTriangles(finVertices);
        finPrimitive.SetMaterial(finMaterials[omdMesh.MaterialIndex]);
        finPrimitive.SetVertexOrder(VertexOrder.NORMAL);
      }

      return finModel;
    }
  }
}