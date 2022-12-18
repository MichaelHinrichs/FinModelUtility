using fin.data;
using fin.image;
using fin.model;
using fin.model.impl;
using OpenTK.Graphics.OpenGL;
using Quad64;
using Quad64.src.Scripts;


namespace sm64.api {
  public static class Sm64ModelConverter {
    public static IModel ConvertModels(params Model3D[] sm64Models) {
      var finModel = new ModelImpl();

      var lazyTextureDictionary = new LazyDictionary<Texture2D, ITexture>(
          sm64Texture => {
            var finTexture = finModel.MaterialManager.CreateTexture(
                FinImage.FromBitmap(sm64Texture.Bmp));

            finTexture.WrapModeU =
                ConvertFromGlWrap_((TextureWrapMode)sm64Texture.TextureParamS);
            finTexture.WrapModeV =
                ConvertFromGlWrap_((TextureWrapMode)sm64Texture.TextureParamT);

            return finTexture;
          });
      var lazyMaterialDictionary =
          new LazyDictionary<(ModelBuilder.ModelBuilderMaterial, Texture2D),
              IMaterial>(
              sm64MaterialAndTexture => {
                var (sm64Material, sm64Texture) = sm64MaterialAndTexture;

                var geometryMode = sm64Material.GeometryMode;

                var cullFront =
                    geometryMode.HasFlag(RspGeometryMode.G_CULL_FRONT);
                var cullBack =
                    geometryMode.HasFlag(RspGeometryMode.G_CULL_BACK);
                var finCullingMode = cullFront switch {
                    false => cullBack switch {
                        true  => CullingMode.SHOW_FRONT_ONLY,
                        false => CullingMode.SHOW_BOTH,
                    },
                    true => cullBack switch {
                        false => CullingMode.SHOW_BACK_ONLY,
                        true  => CullingMode.SHOW_NEITHER,
                    },
                };

                var finMaterial = finModel.MaterialManager.AddTextureMaterial(
                    lazyTextureDictionary[sm64Texture]);
                finMaterial.CullingMode = finCullingMode;

                return finMaterial;
              });

      foreach (var sm64Model in sm64Models) {
        foreach (var sm64Mesh in sm64Model.meshes) {
          var finMaterial =
              lazyMaterialDictionary[(sm64Mesh.Material, sm64Mesh.texture)];

          var indices = sm64Mesh.indices;
          var colors = sm64Mesh.colors;
          var vertices = sm64Mesh.vertices;
          var uvs = sm64Mesh.texCoord;

          var finVertices = new List<IVertex>();
          foreach (var vertexIndex in indices) {
            var uv = uvs[vertexIndex];
            var color = colors[vertexIndex];
            var vertex = vertices[vertexIndex];

            finVertices.Add(
                finModel.Skin.AddVertex(vertex.X, vertex.Y, vertex.Z)
                        .SetUv(uv.X, uv.Y)
                        .SetColorBytes(
                            (byte)(255 * color.X),
                            (byte)(255 * color.Y),
                            (byte)(255 * color.Z),
                            (byte)(255 * color.W)));
          }

          var finMesh = finModel.Skin.AddMesh();
          finMesh.AddTriangles(finVertices.ToArray())
                 .SetMaterial(finMaterial)
                 .SetVertexOrder(VertexOrder.NORMAL);
        }
      }

      return finModel;
    }

    private static WrapMode ConvertFromGlWrap_(
        TextureWrapMode wrapMode) =>
        wrapMode switch {
            TextureWrapMode.ClampToEdge    => WrapMode.CLAMP,
            TextureWrapMode.Repeat         => WrapMode.REPEAT,
            TextureWrapMode.MirroredRepeat => WrapMode.MIRROR_REPEAT,
            _ => throw new ArgumentOutOfRangeException(
                     nameof(wrapMode), wrapMode, null)
        };
  }
}