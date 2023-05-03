using System.Linq;

using cmb.schema.cmb;

using fin.data.lazy;
using fin.image;
using fin.model;

using CmbTextureMinFilter = cmb.schema.cmb.TextureMinFilter;
using CmbTextureMagFilter = cmb.schema.cmb.TextureMagFilter;
using FinTextureMinFilter = fin.model.TextureMinFilter;
using FinTextureMagFilter = fin.model.TextureMagFilter;

namespace cmb.material {
  public class CmbFixedFunctionMaterial {
    private const bool USE_FIXED_FUNCTION = false;

    public CmbFixedFunctionMaterial(
        IModel finModel,
        Cmb cmb,
        Material cmbMaterial,
        int materialIndex,
        ILazyArray<IImage> textureImages) {
      // Get associated texture
      var finTextures = cmbMaterial.texMappers.Select(texMapper => {
        var textureId = texMapper.textureId;

        ITexture? finTexture = null;
        if (textureId != -1) {
          var cmbTexture = cmb.tex.Data.textures[textureId];
          var textureImage = textureImages[textureId];

          finTexture = finModel.MaterialManager.CreateTexture(textureImage);
          finTexture.Name = cmbTexture.name;
          finTexture.WrapModeU = this.CmbToFinWrapMode(texMapper.wrapS);
          finTexture.WrapModeV = this.CmbToFinWrapMode(texMapper.wrapT);
          finTexture.MinFilter = texMapper.minFilter switch {
            CmbTextureMinFilter.Nearest => FinTextureMinFilter.NEAR,
            CmbTextureMinFilter.Linear => FinTextureMinFilter.LINEAR,
            CmbTextureMinFilter.NearestMipmapNearest => FinTextureMinFilter
                .NEAR_MIPMAP_NEAR,
            CmbTextureMinFilter.LinearMipmapNearest => FinTextureMinFilter
                .LINEAR_MIPMAP_NEAR,
            CmbTextureMinFilter.NearestMipmapLinear => FinTextureMinFilter
                .NEAR_MIPMAP_LINEAR,
            CmbTextureMinFilter.LinearMipmapLinear => FinTextureMinFilter
                .LINEAR_MIPMAP_LINEAR,
          };
          finTexture.MagFilter = texMapper.magFilter switch {
            CmbTextureMagFilter.Nearest => FinTextureMagFilter.NEAR,
            CmbTextureMagFilter.Linear => FinTextureMagFilter.LINEAR,
          };

          var cmbBorderColor = texMapper.BorderColor;
          finTexture.BorderColor = cmbBorderColor;
        }

        return finTexture;
      }).ToArray();

      // Create material
      if (!USE_FIXED_FUNCTION) {
        var firstTexture = finTextures.FirstOrDefault();
        var finMaterial = firstTexture != null
            ? (IMaterial) finModel.MaterialManager.AddTextureMaterial(firstTexture)
            : finModel.MaterialManager.AddNullMaterial();
        this.Material = finMaterial;
      } else {
        var finMaterial = finModel.MaterialManager.AddFixedFunctionMaterial();
        this.Material = finMaterial;

        // TODO: Implement fixed-function material logic

        if (!cmbMaterial.alphaTestEnabled) {
          finMaterial.SetAlphaCompare(AlphaOp.Or,
                                      AlphaCompareType.Always,
                                      0,
                                      AlphaCompareType.Always,
                                      0);
        } else {
          finMaterial.SetAlphaCompare(
              AlphaOp.Or,
              cmbMaterial.alphaTestFunction switch {
                  TestFunc.Always   => AlphaCompareType.Always,
                  TestFunc.Equal    => AlphaCompareType.Equal,
                  TestFunc.Gequal   => AlphaCompareType.GEqual,
                  TestFunc.Greater  => AlphaCompareType.Greater,
                  TestFunc.Never    => AlphaCompareType.Never,
                  TestFunc.Less     => AlphaCompareType.Less,
                  TestFunc.Lequal   => AlphaCompareType.LEqual,
                  TestFunc.Notequal => AlphaCompareType.NEqual,
              },
              cmbMaterial.alphaTestReferenceValue,
              AlphaCompareType.Never,
              0);
        }

      }

      this.Material.Name = $"material{materialIndex}";
      this.Material.CullingMode = cmbMaterial.faceCulling switch {
          CullMode.FrontAndBack => CullingMode.SHOW_BOTH,
          CullMode.Front        => CullingMode.SHOW_FRONT_ONLY,
          CullMode.BackFace     => CullingMode.SHOW_BACK_ONLY,
          CullMode.Never        => CullingMode.SHOW_NEITHER,
      };
    }

    public IMaterial Material { get; }

    public WrapMode CmbToFinWrapMode(TextureWrapMode cmbMode)
      => cmbMode switch {
          TextureWrapMode.ClampToBorder => WrapMode.CLAMP,
          TextureWrapMode.Repeat => WrapMode.REPEAT,
          TextureWrapMode.ClampToEdge => WrapMode.CLAMP,
          TextureWrapMode.Mirror => WrapMode.MIRROR_REPEAT,
      };
  }
}
