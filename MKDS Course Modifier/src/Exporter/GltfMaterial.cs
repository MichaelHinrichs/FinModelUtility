using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using MKDS_Course_Modifier.GCN;

using SharpGLTF.Materials;

namespace mkds.exporter {
  public class DetailMaskPair {
    public GltfTexture Detail { get; }
    public GltfTexture? Mask { get; }

    public DetailMaskPair(GltfTexture detail, GltfTexture? mask = null) {
      this.Detail = detail;
      this.Mask = mask;
    }
  }

  public class GltfMaterial {
    public GltfMaterial(
        int materialEntryIndex,
        BMD bmd,
        IList<GltfTexture> textures) {
      var materialEntry = bmd.MAT3.MaterialEntries[materialEntryIndex];
      var materialName = bmd.MAT3.StringTable[materialEntryIndex];

      var material = new MaterialBuilder(materialName)
          .WithDoubleSide(true);

      this.MaterialBuilder = material;

      var texStagesAndMatrixIndices =
          materialEntry.TexStages.Select(
              (texStage, i) => {
                var matrixIndex = materialEntry.TexMatrices[i];
                return (i, texStage, matrixIndex);
              });

      var validTexStagesAndMatrixIndices =
          texStagesAndMatrixIndices.Where(
              texStageAndMatrixIndex
                  => GltfMaterial.IsValidIndex_(
                      texStageAndMatrixIndex.texStage));

      var validTexturesAndMatrices =
          validTexStagesAndMatrixIndices
              .Select(
                  texStageAndMatrixIndex => {
                    var texture =
                        textures[
                            bmd.MAT3.TextureIndieces[
                                texStageAndMatrixIndex.texStage]];
                    var matrix =
                        GltfMaterial.IsValidIndex_(
                            texStageAndMatrixIndex.matrixIndex)
                            ? bmd.MAT3.TextureMatrices[
                                texStageAndMatrixIndex.matrixIndex]
                            : null;

                    return (texStageAndMatrixIndex.i, texture, matrix);
                  });

      // TODO: Use spherical environment mapping once glTF supports it.
      // TODO: Include texGenType (?) to determine which are masks,
      // reflections, etc.
      // TODO: Group these up into pairs, mask/non-mask?
      // TODO: Can we assume the last pair is diffuse?
      // TODO: This does *not* seem like a safe assumption for mask vs. non-mask.
      var nonMaskTexturesAndMatrices = validTexturesAndMatrices.Where(
          textureAndMatrix
              => !GltfMaterial.IsMaskTexture_(textureAndMatrix.texture.Header));

      var channels = new[] {KnownChannel.Diffuse};
      var diffuseChannelCount = channels.Length;
      var surfaceTextureAndMatrices =
          nonMaskTexturesAndMatrices
              .Skip(nonMaskTexturesAndMatrices.Count() - diffuseChannelCount)
              .Take(diffuseChannelCount)
              .ToList();

      // TODO: This might not be a valid assumption--materials might need to
      // be carved up based on which textures are actually used in a given
      // shape.
      this.CurrentTexStageIndices = surfaceTextureAndMatrices
                                    .Select(textureAndMatrix
                                                => textureAndMatrix.i)
                                    .ToList();

      var hasTexture = surfaceTextureAndMatrices.Any();
      if (!hasTexture) {
        material.WithUnlitShader();
        return;
      }
      material.WithAlpha(AlphaMode.MASK)
              .WithSpecularGlossinessShader()
              .WithSpecularGlossiness(new Vector3(0), 0);

      // TODO: Detect when a reflection would have been present--in these cases,
      // crank the glossiness way up.
      // TODO: In the case where a reflection would have been present, if a
      // mask is grouped, pass it in the specular/glossiness.

      // Notes...
      // - Looks like textures are merged in different channels, i.e. layers?
      // - Can we merge these layers programmatically before exporting?
      // - Seems like reflections are listed earlier than diffuse?
      // - Looks like channel is applied, multiplied by mask (alpha), and then
      //   next is applied?
      // - Looks like masks are used with reflections to limit which part of
      //   the surface is shiny. This seems to be gloss/specular?
      for (var t = 0; t < surfaceTextureAndMatrices.Count; ++t) {
        var (i, diffuseTexture, matrix) = surfaceTextureAndMatrices[t];
        var channel = channels[t];

        var textureBuilder = material.UseChannel(channel)
                                     .UseTexture();

        textureBuilder.WithPrimaryImage(
                          diffuseTexture.MemoryImage)
                      .WithCoordinateSet(i)
                      .WithSampler(diffuseTexture.WrapModeS,
                                   diffuseTexture.WrapModeT);

        if (matrix != null) {
          // TODO: Are matrices this needed anywhere?
        }
      }

      /*for (int index = 0; index < 8; ++index) {
        if (this.MAT3.MaterialEntries[
                    (int) this.MAT3.MaterialEntryIndieces[
                        (int) entry.Index]]
                .TexStages[index] !=
            ushort.MaxValue)
          Gl.glBindTexture(3553,
                           (int) this.MAT3.TextureIndieces[
                               (int) this
                                     .MAT3.MaterialEntries[
                                         (int) this
                                               .MAT3
                                               .MaterialEntryIndieces
                                               [(int) entry
                                                    .Index]]
                                     .TexStages[index]] +
                           1);
        else
          Gl.glBindTexture(3553, 0);
      }

      Gl.glMatrixMode(5888);
      this.MAT3.glAlphaCompareglBendMode(
          (int) this
                .MAT3.MaterialEntries[
                    (int) this.MAT3.MaterialEntryIndieces[
                        (int) entry.Index]]
                .Indices2[1],
          (int) this
                .MAT3.MaterialEntries[
                    (int) this.MAT3.MaterialEntryIndieces[
                        (int) entry.Index]]
                .Indices2[2],
          (int) this
                .MAT3.MaterialEntries[
                    (int) this.MAT3.MaterialEntryIndieces[
                        (int) entry.Index]]
                .Indices2[3]);
      this.Shaders[
              (int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]]
          .Enable();*/
    }

    public MaterialBuilder MaterialBuilder { get; }

    public IList<int> CurrentTexStageIndices { get; }

    private static bool IsValidIndex_(ushort texStage)
      => texStage != ushort.MaxValue;

    private static bool IsMaskTexture_(BMD.TEX1Section.TextureHeader header)
      => header.Format switch {
          BMD.TEX1Section.TextureFormat.R5_G6_B5 => false,
          BMD.TEX1Section.TextureFormat.A3_RGB5  => false,
          BMD.TEX1Section.TextureFormat.ARGB8    => false,
          BMD.TEX1Section.TextureFormat.S3TC1    => false,
          _                                      => true,
      };
  }
}