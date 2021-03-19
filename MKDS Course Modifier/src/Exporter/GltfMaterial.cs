using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using MKDS_Course_Modifier.GCN;

using SharpGLTF.Materials;

using Tao.OpenGl;

namespace mkds.exporter {
  public class GltfMaterial {
    public GltfMaterial(
        BMD.MAT3Section.MaterialEntry materialEntry,
        BMD bmd,
        IList<GltfTexture> textures) {
      var material = new MaterialBuilder()
          .WithDoubleSide(true);

      this.MaterialBuilder = material;

      var validTexStages = materialEntry
                           .TexStages.Where(GltfMaterial.IsValidTexStage);

      var validTextures =
          validTexStages.Select(texStage
                                    => textures[
                                        bmd.MAT3.TextureIndieces[texStage]]);

      // TODO: Include texGenType (?) to determine which are masks,
      // reflections, etc.
      // TODO: Group these up into pairs, mask/non-mask?
      // TODO: Can we assume the last pair is diffuse?
      // TODO: This does *not* seem like a safe assumption for mask vs. non-mask.
      var nonMaskTextures = validTextures.Where(
          texture => texture.Header.Format ==
                     BMD.TEX1Section.TextureFormat.S3TC1);

      var hasTexture = nonMaskTextures.Any();
      if (!hasTexture) {
        material.WithUnlitShader();
        return;
      }
      material.WithAlpha(AlphaMode.MASK)
              .WithSpecularGlossinessShader()
              .WithSpecularGlossiness(new Vector3(0), 0);

      // Notes...
      // - Looks like textures are merged in different channels, i.e. layers?
      // - Can we merge these layers programmatically before exporting?
      // - Seems like reflections are listed earlier than diffuse?
      // - Looks like channel is applied, multiplied by mask (alpha), and then
      //   next is applied?
      // - Looks like masks are used with reflections to limit which part of
      //   the surface is shiny. This seems to be gloss/specular?
      foreach (var diffuseTexture in new[] {nonMaskTextures.Last()}) {
        material.UseChannel(KnownChannel.Diffuse)
                .UseTexture()
                .WithPrimaryImage(diffuseTexture.MemoryImage)
                .WithSampler(diffuseTexture.WrapModeS,
                             diffuseTexture.WrapModeT);
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

    public static bool IsValidTexStage(ushort texStage)
      => texStage != ushort.MaxValue;
  }
}