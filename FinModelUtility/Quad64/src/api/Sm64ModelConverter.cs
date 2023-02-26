using fin.data.lazy;
using fin.image;
using fin.model;
using fin.model.impl;
using fin.util.enums;
using OpenTK.Graphics.OpenGL;
using Quad64;
using Quad64.src.Scripts;


namespace sm64.api {
  public static class Sm64ModelConverter {
    public static IModel ConvertModels(params Model3D[] sm64Models) {
      var finModel = new ModelImpl();

      var lazyTextureDictionary = new LazyDictionary<(Texture2D, UvType), ITexture>(
          sm64TextureAndUvType => {
            var (sm64Texture, uvType) = sm64TextureAndUvType;
            var finTexture = finModel.MaterialManager.CreateTexture(
                FinImage.FromBitmap(sm64Texture.Bmp));

            finTexture.UvType = uvType;

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
                    geometryMode.CheckFlag(RspGeometryMode.G_CULL_FRONT);
                var cullBack =
                    geometryMode.CheckFlag(RspGeometryMode.G_CULL_BACK);
                var finCullingMode = cullFront switch {
                  false => cullBack switch {
                    true => CullingMode.SHOW_FRONT_ONLY,
                    false => CullingMode.SHOW_BOTH,
                  },
                  true => cullBack switch {
                    false => CullingMode.SHOW_BACK_ONLY,
                    true => CullingMode.SHOW_NEITHER,
                  },
                };

                var uvType =
                  geometryMode.CheckFlag(RspGeometryMode.G_TEXTURE_GEN_LINEAR)
                    ? UvType.LINEAR
                    : UvType.NORMAL;

                var finMaterial =
                  finModel.MaterialManager.AddFixedFunctionMaterial();

                var equations = finMaterial.Equations;

                finMaterial.SetTextureSource(0, lazyTextureDictionary[(sm64Texture, uvType)]);
                finMaterial.CullingMode = finCullingMode;

                var color0 = equations.CreateColorConstant(0);
                var scalar1 = equations.CreateScalarConstant(1);

                var vertexColor0 = equations.CreateColorInput(
                  FixedFunctionSource.VERTEX_COLOR_0,
                  color0);
                var textureColor0 = equations.CreateColorInput(
                  FixedFunctionSource.TEXTURE_COLOR_0,
                  color0);

                var vertexAlpha0 =
                  equations.CreateScalarInput(FixedFunctionSource.VERTEX_ALPHA_0,
                    scalar1);
                var textureAlpha0 = equations.CreateScalarInput(
                  FixedFunctionSource.TEXTURE_ALPHA_0,
                  scalar1);

                equations.CreateColorOutput(
                  FixedFunctionSource.OUTPUT_COLOR,
                  vertexColor0.Multiply(textureColor0));
                equations.CreateScalarOutput(FixedFunctionSource.OUTPUT_ALPHA,
                  vertexAlpha0.Multiply(textureAlpha0));

                finMaterial.SetAlphaCompare(AlphaOp.Or, AlphaCompareType.Greater, .5f, AlphaCompareType.Never, 0);

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
          TextureWrapMode.ClampToEdge => WrapMode.CLAMP,
          TextureWrapMode.Repeat => WrapMode.REPEAT,
          TextureWrapMode.MirroredRepeat => WrapMode.MIRROR_REPEAT,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(wrapMode), wrapMode, null)
        };
  }
}