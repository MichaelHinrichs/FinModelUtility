using Assimp.Unmanaged;

using dat.schema;

using fin.io;
using fin.language.equations.fixedFunction;
using fin.language.equations.fixedFunction.impl;
using fin.math.matrix.four;
using fin.math.rotations;
using fin.model;
using fin.model.impl;
using fin.model.io.importers;
using fin.shaders.glsl;
using fin.util.enums;
using fin.util.hex;

using gx;

using schema.binary;

namespace dat.api {
  public class DatModelImporter : IModelImporter<DatModelFileBundle> {
    public IModel ImportModel(DatModelFileBundle modelFileBundle) {
      var dat =
          modelFileBundle.PrimaryDatFile.ReadNew<Dat>(Endianness.BigEndian);

      var finModel = new ModelImpl();
      var finSkin = finModel.Skin;

      // Adds skeleton
      var finBoneByJObj = new Dictionary<JObj, IBone>();
      var boneWeightsByJObj = new Dictionary<JObj, IBoneWeights>();
      var boneQueue = new Queue<(IBone finParentBone, JObj datBone)>();
      foreach (var datRootBone in dat.RootJObjs) {
        boneQueue.Enqueue((finModel.Skeleton.Root, datRootBone));
      }

      while (boneQueue.Count > 0) {
        var (finParentBone, jObj) = boneQueue.Dequeue();

        var datBoneData = jObj.Data;

        var finBone =
            finParentBone.AddChild(datBoneData.Position.X,
                                   datBoneData.Position.Y,
                                   datBoneData.Position.Z)
                         .SetLocalRotationRadians(
                             datBoneData.RotationRadians.X,
                             datBoneData.RotationRadians.Y,
                             datBoneData.RotationRadians.Z)
                         .SetLocalScale(
                             datBoneData.Scale.X,
                             datBoneData.Scale.Y,
                             datBoneData.Scale.Z);
        finBone.Name = jObj.Name;

        finBoneByJObj[jObj] = finBone;
        boneWeightsByJObj[jObj] =
            finSkin.GetOrCreateBoneWeights(VertexSpace.BONE, finBone);

        foreach (var datChildBone in jObj.Children) {
          boneQueue.Enqueue((finBone, datChildBone));
        }
      }

      // Adds mesh and materials
      var finMaterialManager = finModel.MaterialManager;
      var finMaterialsByMObjOffset = new Dictionary<uint, IMaterial>();
      var finTexturesByTObjOffset = new Dictionary<uint, ITexture>();

      var finMesh = finSkin.AddMesh();
      foreach (var jObj in dat.JObjs) {
        var defaultBoneWeights = boneWeightsByJObj[jObj];

        foreach (var dObj in jObj.DObjs) {
          // Gets material
          IMaterial? finMaterial = null;
          var mObj = dObj.MObj;
          var mObjOffset = dObj.Header.MObjOffset;
          if (mObj != null && !finMaterialsByMObjOffset.TryGetValue(
                  mObjOffset,
                  out finMaterial)) {
            var tObjsAndOffsets = mObj.TObjsAndOffsets.ToArray();

            var tObjsAndFinTextures =
                new (TObj, ITexture)[tObjsAndOffsets.Length];
            if (tObjsAndOffsets.Length > 0) {
              for (var i = 0; i < tObjsAndOffsets.Length; i++) {
                var (tObjOffset, tObj) = tObjsAndOffsets[i];
                if (!finTexturesByTObjOffset.TryGetValue(
                        tObjOffset,
                        out var finTexture)) {
                  finTexture = finMaterialManager.CreateTexture(tObj.Image);
                  finTexture.Name = tObj.Name ?? tObjOffset.ToHex();

                  finTexture.WrapModeU = tObj.WrapS.ToFinWrapMode();
                  finTexture.WrapModeV = tObj.WrapT.ToFinWrapMode();

                  finTexture.UvType = tObj.Flags.GetCoord() switch {
                      Coord.UV         => UvType.STANDARD,
                      Coord.REFLECTION => UvType.SPHERICAL,
                      _                => UvType.STANDARD
                  };

                  // Why tf does Melee have 3D texture transforms......
                  // https://github.com/Ploaj/HSDLib/blob/93a906444f34951c6eed4d8c6172bba43d4ada98/HSDRawViewer/Converters/ModelExporter.cs#L526

                  var tObjTranslation = tObj.Translation;
                  var tObjRotationRadians = tObj.RotationRadians;
                  var tObjScale = tObj.Scale;

                  var rawTranslation = new Position(
                      tObjTranslation.X,
                      tObjTranslation.Y,
                      tObjTranslation.Z);
                  var rawQuaternion = QuaternionUtil.CreateZyx(
                      tObjRotationRadians.X,
                      tObjRotationRadians.Y,
                      tObjRotationRadians.Z);
                  var rawScale =
                      new Scale(tObjScale.X, tObjScale.Y, tObjScale.Z);

                  // This is an absolute nightmare, but it works.
                  FinMatrix4x4Util.FromTrs(rawTranslation,
                                           rawQuaternion,
                                           rawScale)
                                  .InvertInPlace()
                                  .Decompose(out var outTranslation,
                                             out var outQuaternion,
                                             out var outScale);

                  var outRotationRadians =
                      QuaternionUtil.ToEulerRadians(outQuaternion);

                  finTexture.SetOffset3d(
                      outTranslation.X,
                      outTranslation.Y,
                      outTranslation.Z);
                  finTexture.SetRotationRadians3d(
                      outRotationRadians.X,
                      outRotationRadians.Y,
                      outRotationRadians.Z);
                  finTexture.SetScale3d(
                      tObj.RepeatS * outScale.X,
                      tObj.RepeatT * outScale.Y,
                      outScale.Z);

                  finTexturesByTObjOffset[tObjOffset] = finTexture;
                }

                tObjsAndFinTextures[i] = (tObj, finTexture);
              }
            }

            var fixedFunctionMaterial =
                finMaterialManager.AddFixedFunctionMaterial();
            finMaterial = fixedFunctionMaterial;

            this.PopulateFixedFunctionMaterial_(mObj,
                                                tObjsAndFinTextures,
                                                fixedFunctionMaterial);
          }

          finMaterial.Name = mObj.Name ?? mObjOffset.ToHex();

          finMaterialsByMObjOffset[mObjOffset] = finMaterial;

          // Adds polygons
          foreach (var pObj in dObj.PObjs) {
            var vertexSpace = pObj.VertexSpace;
            var finWeights =
                pObj.Weights
                    ?.Select(pObjWeights => finSkin.GetOrCreateBoneWeights(
                                 vertexSpace,
                                 pObjWeights
                                     .Select(
                                         pObjWeight => new BoneWeight(
                                             finBoneByJObj[pObjWeight.JObj],
                                             pObjWeight.JObj
                                                 .InverseBindMatrix,
                                             pObjWeight.Weight
                                         ))
                                     .ToArray()))
                    .ToArray();

            foreach (var datPrimitive in pObj.Primitives) {
              var finVertices =
                  datPrimitive
                      .Vertices
                      .Select(datVertex => {
                        var finVertex = finSkin.AddVertex(datVertex.Position);

                        finVertex.SetLocalNormal(datVertex.Normal);
                        finVertex.SetColor(datVertex.Color);

                        if (datVertex.Uv0 != null) {
                          var uv0 = datVertex.Uv0.Value;
                          finVertex.SetUv(0, uv0.X, uv0.Y);
                        }

                        if (datVertex.Uv1 != null) {
                          var uv1 = datVertex.Uv1.Value;
                          finVertex.SetUv(1, uv1.X, uv1.Y);
                        }

                        // TODO: Is this right???
                        if (datVertex.WeightId != null) {
                          if (finWeights != null) {
                            finVertex.SetBoneWeights(
                                finWeights[datVertex.WeightId.Value]);
                          }
                        } else {
                          finVertex.SetBoneWeights(defaultBoneWeights);
                        }

                        return finVertex;
                      })
                      .ToArray();

              var finPrimitive = datPrimitive.Type switch {
                  GxOpcode.DRAW_TRIANGLES =>
                      finMesh.AddTriangles(finVertices),
                  GxOpcode.DRAW_QUADS => finMesh.AddQuads(finVertices),
                  GxOpcode.DRAW_TRIANGLE_STRIP => finMesh.AddTriangleStrip(
                      finVertices)
              };

              if (finMaterial != null) {
                finPrimitive.SetMaterial(finMaterial);
              }
            }
          }
        }
      }

      return finModel;
    }

    /// <summary>
    ///   Shamelessly copied from:
    ///   https://github.com/Ploaj/HSDLib/blob/93a906444f34951c6eed4d8c6172bba43d4ada98/HSDRawViewer/Shader/gx_lightmap.frag
    /// </summary>
    private void PopulateFixedFunctionMaterial_(
        MObj mObj,
        IReadOnlyList<(TObj, ITexture)> tObjsAndFinTextures,
        IFixedFunctionMaterial fixedFunctionMaterial) {
      var equations = fixedFunctionMaterial.Equations;

      var colorOps = new ColorFixedFunctionOps(equations);
      var scalarOps = new ScalarFixedFunctionOps(equations);

      var vertexColor = equations.CreateOrGetColorInput(
          FixedFunctionSource.VERTEX_COLOR_0);
      var vertexAlpha = equations.CreateOrGetScalarInput(
          FixedFunctionSource.VERTEX_ALPHA_0);

      for (var i = 0; i < tObjsAndFinTextures.Count; ++i) {
        var (_, finTexture) = tObjsAndFinTextures[i];
        fixedFunctionMaterial.SetTextureSource(i, finTexture);
      }

      var renderMode = mObj.RenderMode;
      var material = mObj.Material;

      IColorValue? outputColor = colorOps.One;
      if (renderMode.CheckFlag(RenderMode.CONSTANT)) {
        var diffuseRgba = material.DiffuseColor;
        var diffuseColor = equations.CreateColorConstant(diffuseRgba.Rf,
          diffuseRgba.Gf,
          diffuseRgba.Bf);

        outputColor = diffuseColor;
      }

      if (renderMode.CheckFlag(RenderMode.VERTEX)) {
        outputColor = colorOps.Multiply(outputColor, vertexColor);
      }

      for (var i = 0; i < tObjsAndFinTextures.Count; ++i) {
        var (tObj, _) = tObjsAndFinTextures[i];
        var textureColor = equations.CreateOrGetColorInput(
            FixedFunctionSource.TEXTURE_COLOR_0 + i);

        switch (tObj.Flags.GetColorMap()) {
          case ColorMap.NONE: {
            // TODO: Is this right?
            break;
          }
          case ColorMap.ALPHA_MASK: {
            // TODO: What should this do?
            break;
          }
          case ColorMap.RGB_MASK: {
            // TODO: What should this do?
            break;
          }
          case ColorMap.BLEND: {
            // TODO: Is this right?
            outputColor = colorOps.Multiply(outputColor, textureColor);
            break;
          }
          case ColorMap.MODULATE: {
            // TODO: Is this right?
            outputColor = colorOps.Multiply(outputColor, textureColor);
            break;
          }
          case ColorMap.REPLACE: {
            // TODO: Is this right?
            outputColor = textureColor;
            break;
          }
          case ColorMap.PASS: {
            // TODO: What should this do?
            break;
          }
          case ColorMap.ADD: {
            outputColor = colorOps.Add(outputColor, textureColor);
            break;
          }
          case ColorMap.SUB: {
            outputColor = colorOps.Subtract(outputColor, textureColor);
            break;
          }
        }
      }

      // TODO: Is this right??
      if (renderMode.CheckFlag(RenderMode.DIFFUSE)) {
        var ambientRgba = material.AmbientColor;
        var ambientColor =
            equations.CreateColorConstant(ambientRgba.Rf,
                                          ambientRgba.Gf,
                                          ambientRgba.Bf);
        var diffuseRgba = material.DiffuseColor;
        var diffuseColor =
            colorOps.Multiply(
                equations.CreateOrGetColorInput(
                    FixedFunctionSource.LIGHT_0_COLOR),
                equations.CreateColorConstant(diffuseRgba.Rf,
                                              diffuseRgba.Gf,
                                              diffuseRgba.Bf));

        var lightColor = colorOps.Add(ambientColor, diffuseColor);
        outputColor = colorOps.Multiply(outputColor, lightColor);
      }

      IScalarValue? outputAlpha = scalarOps.One;

      if (renderMode.CheckFlag(RenderMode.ALPHA_VTX)) {
        outputAlpha = vertexAlpha;
      }

      if (renderMode.CheckFlag(RenderMode.ALPHA_MAT)) {
        outputAlpha =
            scalarOps.MultiplyWithConstant(outputAlpha, material!.Alpha);
      }

      for (var i = 0; i < tObjsAndFinTextures.Count; ++i) {
        var (tObj, _) = tObjsAndFinTextures[i];
        var textureAlpha = equations.CreateOrGetScalarInput(
            FixedFunctionSource.TEXTURE_ALPHA_0);
        switch (tObj.Flags.GetAlphaMap()) {
          case AlphaMap.NONE: {
            // TODO: Is this right?
            break;
          }
          case AlphaMap.ALPHA_MASK: {
            // TODO: What should this do?
            break;
          }
          case AlphaMap.BLEND: {
            // TODO: Is this right?
            outputAlpha = scalarOps.Multiply(outputAlpha, textureAlpha);
            break;
          }
          case AlphaMap.MODULATE: {
            // TODO: Is this right?
            outputAlpha = scalarOps.Multiply(outputAlpha, textureAlpha);
            break;
          }
          case AlphaMap.REPLACE: {
            // TODO: Is this right?
            outputAlpha = textureAlpha;
            break;
          }
          case AlphaMap.PASS: {
            // TODO: What should this do?
            break;
          }
          case AlphaMap.ADD: {
            outputAlpha = scalarOps.Add(outputAlpha, textureAlpha);
            break;
          }
          case AlphaMap.SUB: {
            outputAlpha = scalarOps.Subtract(outputAlpha, textureAlpha);
            break;
          }
        }
      }

      equations.CreateColorOutput(FixedFunctionSource.OUTPUT_COLOR,
                                  outputColor ?? colorOps.Zero);
      equations.CreateScalarOutput(FixedFunctionSource.OUTPUT_ALPHA,
                                   outputAlpha ?? scalarOps.Zero);

      fixedFunctionMaterial.SetAlphaCompare(
          AlphaOp.Or,
          AlphaCompareType.GEqual,
          GlslConstants.MIN_ALPHA_BEFORE_DISCARD,
          AlphaCompareType.Never,
          0);
    }
  }
}