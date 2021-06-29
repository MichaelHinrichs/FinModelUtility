using System;
using System.Collections.Generic;
using System.Linq;

using fin.model;

using mkds.gcn.bmd;

using MKDS_Course_Modifier.GCN;

using BlendFactor = fin.model.BlendFactor;
using LogicOp = fin.model.LogicOp;

namespace mkds.exporter {
  using TextureMatrixInfo = BMD.MAT3Section.TextureMatrixInfo;

  /// <summary>
  ///   BMD material, one of the common formats for the GameCube.
  ///
  ///   For more info:
  ///   http://www.amnoid.de/gc/tev.html
  /// </summary>
  public class BmdMaterial {
    public record BmdLayerIndices(
        int Index,
        ushort TexStage,
        ushort MatrixIndex) {
      public bool IsTexStageValid
        => BmdLayerIndices.IsValidIndex_(this.TexStage);

      public bool IsMatrixIndexValid
        => BmdLayerIndices.IsValidIndex_(this.MatrixIndex);

      private static bool IsValidIndex_(ushort index)
        => index != ushort.MaxValue;
    }

    public record BmdLayer(
        BmdLayerIndices LayerIndices,
        BmdTexture Texture,
        TextureMatrixInfo? Matrix);

    public BmdMaterial(
        IMaterialManager materialManager,
        int materialEntryIndex,
        BMD bmd,
        IList<BmdTexture> textures) {
      var materialEntry = bmd.MAT3.MaterialEntries[materialEntryIndex];
      var materialName = bmd.MAT3.StringTable[materialEntryIndex];

      var material = materialManager.AddLayerMaterial();
      material.Name = materialName;

      this.Material = material;

      // Gathers up all tex stages and texture matrices.
      var layerIndices = new List<BmdLayerIndices>();
      for (var i = 0; i < 8; ++i) {
        layerIndices.Add(
            new BmdLayerIndices(i,
                                materialEntry.TexStages[i],
                                materialEntry.TexMatrices[i]));
      }

      var validLayerIndices =
          layerIndices.Where(layerIndices => layerIndices.IsTexStageValid);

      // TODO: Support layers that are just colors.
      var validLayers =
          validLayerIndices
              .Select(layerIndices =>
                          new BmdLayer(layerIndices,
                                       textures[
                                           bmd.MAT3.TextureIndieces[
                                               layerIndices.TexStage]],
                                       layerIndices.IsMatrixIndexValid
                                           ? bmd.MAT3.TextureMatrices[
                                               layerIndices.MatrixIndex]
                                           : null));

      // TODO: Use spherical environment mapping once glTF supports it.
      // TODO: Include texGenType (?) to determine which are masks,
      // reflections, etc.

      // TODO: Detect when a reflection would have been present--in these cases,
      // crank the glossiness way up.
      // TODO: In the case where a reflection would have been present, if a
      // mask is grouped, pass it in the specular/glossiness.
      foreach (var layerData in validLayers) {
        var texture = materialManager.CreateTexture(layerData.Texture.Image);
        texture.Name = layerData.Texture.Name;

        // TODO: This isn't right, there should be unique functions for each 
        var alphaIndex = materialEntry.Indices2[1];
        var blendIndex = materialEntry.Indices2[2];
        var depthIndex = materialEntry.Indices2[3];

        var blendFunction = bmd.MAT3.BlendFunctions[blendIndex];
        var depthFunction = bmd.MAT3.DepthFunctions[depthIndex];

        // TODO: Generate texture from image.
        var layer = material.AddTextureLayer(texture);

        layer.BlendMode = blendFunction.BlendMode switch {
            BmdBlendMode.NONE => BlendMode.NONE,
            BmdBlendMode.ADD => BlendMode.ADD,
            BmdBlendMode.REVERSE_SUBTRACT => BlendMode.REVERSE_SUBTRACT,
            BmdBlendMode.SUBTRACT => BlendMode.SUBTRACT,
            _ => throw new ArgumentOutOfRangeException()
        };

        layer.SrcFactor =
            BmdMaterial.BmdToFinBlendFactor_(blendFunction.SrcFactor);
        layer.DstFactor =
            BmdMaterial.BmdToFinBlendFactor_(blendFunction.DstFactor);

        layer.LogicOp = blendFunction.LogicOp switch {
            BmdLogicOp.CLEAR         => LogicOp.CLEAR,
            BmdLogicOp.AND           => LogicOp.AND,
            BmdLogicOp.AND_REVERSE   => LogicOp.AND_REVERSE,
            BmdLogicOp.COPY          => LogicOp.COPY,
            BmdLogicOp.AND_INVERTED  => LogicOp.AND_INVERTED,
            BmdLogicOp.NOOP          => LogicOp.NOOP,
            BmdLogicOp.XOR           => LogicOp.XOR,
            BmdLogicOp.OR            => LogicOp.OR,
            BmdLogicOp.NOR           => LogicOp.NOR,
            BmdLogicOp.EQUIV         => LogicOp.EQUIV,
            BmdLogicOp.INVERT        => LogicOp.INVERT,
            BmdLogicOp.OR_REVERSE    => LogicOp.OR_REVERSE,
            BmdLogicOp.COPY_INVERTED => LogicOp.COPY_INVERTED,
            BmdLogicOp.OR_INVERTED   => LogicOp.OR_INVERTED,
            BmdLogicOp.NAND          => LogicOp.NAND,
            BmdLogicOp.SET           => LogicOp.SET,
        };

        // TODO: Set blend mode
        // TODO: Set UV type
        // TODO: Set matrix
      }
    }

    public IMaterial Material { get; }

    public IList<int> CurrentTexStageIndices { get; }

    private static BlendFactor BmdToFinBlendFactor_(
        BmdBlendFactor blendFactor) => blendFactor switch {
        BmdBlendFactor.ZERO      => BlendFactor.ZERO,
        BmdBlendFactor.ONE       => BlendFactor.ONE,
        BmdBlendFactor.SRC_COLOR => BlendFactor.SRC_COLOR,
        BmdBlendFactor.ONE_MINUS_SRC_COLOR => BlendFactor
            .ONE_MINUS_SRC_COLOR,
        BmdBlendFactor.SRC_ALPHA => BlendFactor.SRC_ALPHA,
        BmdBlendFactor.ONE_MINUS_SRC_ALPHA => BlendFactor
            .ONE_MINUS_SRC_ALPHA,
        BmdBlendFactor.DST_ALPHA => BlendFactor.DST_ALPHA,
        BmdBlendFactor.ONE_MINUS_DST_ALPHA => BlendFactor
            .ONE_MINUS_DST_ALPHA,
        _ => throw new ArgumentOutOfRangeException()
    };
  }
}