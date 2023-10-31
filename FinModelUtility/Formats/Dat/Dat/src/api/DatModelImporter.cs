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

            var mObjMaterial = mObj.Material;
            finMaterial.Shininess = mObjMaterial.Shininess;
            if (mObj.RenderMode.CheckFlag(RenderMode.NO_ZUPDATE)) {
              finMaterial.DepthMode = DepthMode.SKIP_WRITE_TO_DEPTH_BUFFER;
            }

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

      for (var i = 0; i < tObjsAndFinTextures.Count; ++i) {
        var (_, finTexture) = tObjsAndFinTextures[i];
        fixedFunctionMaterial.SetTextureSource(i, finTexture);
      }

      var vertexColor = equations.CreateOrGetColorInput(
          FixedFunctionSource.VERTEX_COLOR_0);
      var vertexAlpha = equations.CreateOrGetScalarInput(
          FixedFunctionSource.VERTEX_ALPHA_0);

      var renderMode = mObj.RenderMode;
      var material = mObj.Material;

      IColorValue? diffuseSurfaceColor = colorOps.One;
      IScalarValue? diffuseSurfaceAlpha = scalarOps.One;

      // Constant color
      var diffuseRgba = material.DiffuseColor;
      diffuseSurfaceColor = equations.CreateColorConstant(
          diffuseRgba.Rf,
          diffuseRgba.Gf,
          diffuseRgba.Bf);
      diffuseSurfaceAlpha = scalarOps.MultiplyWithConstant(
          diffuseSurfaceAlpha,
          material.DiffuseColor.Af * material!.Alpha);

      // Vertex color
      if (renderMode.CheckFlag(RenderMode.VERTEX)) {
        diffuseSurfaceColor =
            colorOps.Multiply(diffuseSurfaceColor, vertexColor);
        diffuseSurfaceAlpha =
            scalarOps.Multiply(diffuseSurfaceAlpha, vertexAlpha);
      }

      IColorValue? ambientSurfaceColor = equations.CreateColorConstant(
          material.AmbientColor.Rf,
          material.AmbientColor.Gf,
          material.AmbientColor.Bf);

      IScalarValue? ambientSurfaceAlpha = equations.CreateScalarConstant(
          material.AmbientColor.Af);

      IColorValue? specularSurfaceColor = equations.CreateColorConstant(
          material.SpecularColor.Rf,
          material.SpecularColor.Gf,
          material.SpecularColor.Bf);

      IScalarValue? specularSurfaceAlpha = equations.CreateScalarConstant(
          material.SpecularColor.Af);


      // Lighting passes
      var hasConstantRenderMode = renderMode.CheckFlag(RenderMode.CONSTANT);
      var hasDiffuseRenderMode = renderMode.CheckFlag(RenderMode.DIFFUSE);
      var hasSpecularRenderMode = renderMode.CheckFlag(RenderMode.SPECULAR);

      IColorValue? ambientLightColor = null;
      IColorValue diffuseLightColor = colorOps.One;
      IColorValue? specularLightColor = null;

      var lightingPasses = new LinkedList<TObjFlags>();
      lightingPasses.AddLast(TObjFlags.LIGHTMAP_DIFFUSE);

      // Shamelessly stolen from:
      // https://github.com/Ploaj/HSDLib/blob/93a906444f34951c6eed4d8c6172bba43d4ada98/HSDRawViewer/Shader/gx_material.frag#L81
      if (!(hasConstantRenderMode && !hasDiffuseRenderMode)) {
        lightingPasses.AddFirst(TObjFlags.LIGHTMAP_AMBIENT);
        ambientSurfaceColor = equations.CreateOrGetColorInput(
            FixedFunctionSource.LIGHT_AMBIENT_COLOR);

        if (hasDiffuseRenderMode) {
          diffuseLightColor = equations.GetMergedLightDiffuseColor();
        }

        if (hasSpecularRenderMode) {
          lightingPasses.AddLast(TObjFlags.LIGHTMAP_SPECULAR);
          specularLightColor = equations.GetMergedLightSpecularColor();
        }
      }

      foreach (var lightingPass in lightingPasses) {
        IColorValue? color;
        IScalarValue? alpha;

        switch (lightingPass) {
          case TObjFlags.LIGHTMAP_DIFFUSE: {
            color = diffuseSurfaceColor;
            alpha = diffuseSurfaceAlpha;
            break;
          }
          case TObjFlags.LIGHTMAP_AMBIENT: {
            color = ambientSurfaceColor;
            alpha = ambientSurfaceAlpha;
            break;
          }
          case TObjFlags.LIGHTMAP_SPECULAR: {
            color = specularSurfaceColor;
            alpha = specularSurfaceAlpha;
            break;
          }
          default: throw new NotImplementedException();
        }

        for (var i = 0; i < tObjsAndFinTextures.Count; ++i) {
          var (tObj, _) = tObjsAndFinTextures[i];
          if (!tObj.Flags.CheckFlag(lightingPass)) {
            continue;
          }

          var textureColor = equations.CreateOrGetColorInput(
              FixedFunctionSource.TEXTURE_COLOR_0 + i);
          var textureAlpha = equations.CreateOrGetScalarInput(
              FixedFunctionSource.TEXTURE_ALPHA_0 + i);

          switch (tObj.Flags.GetColorMap()) {
            case ColorMap.NONE:
            case ColorMap.PASS: {
              // As you might guess from the name, does nothing.
              break;
            }
            case ColorMap.ALPHA_MASK: {
              // TODO: Is this right?
              color = colorOps.MixWithScalar(color, textureColor, textureAlpha);
              break;
            }
            case ColorMap.RGB_MASK: {
              // TODO: What should this do?
              break;
            }
            case ColorMap.BLEND: {
              color = colorOps.MixWithConstant(color,
                                               textureColor,
                                               tObj.Blending);
              break;
            }
            case ColorMap.MODULATE: {
              color = colorOps.Multiply(color, textureColor);
              break;
            }
            case ColorMap.REPLACE: {
              color = textureColor;
              break;
            }
            case ColorMap.ADD: {
              color = colorOps.Add(
                  color,
                  colorOps.MultiplyWithScalar(textureColor, textureAlpha));
              break;
            }
            case ColorMap.SUB: {
              color = colorOps.Subtract(
                  color,
                  colorOps.MultiplyWithScalar(textureColor, textureAlpha));
              break;
            }
          }

          switch (tObj.Flags.GetAlphaMap()) {
            case AlphaMap.NONE:
            case AlphaMap.PASS: {
              // As you might guess from the name, does nothing.
              break;
            }
            case AlphaMap.ALPHA_MASK: {
              // TODO: What should this do?
              break;
            }
            case AlphaMap.BLEND: {
              alpha = scalarOps.MixWithConstant(
                  alpha,
                  textureAlpha,
                  tObj.Blending);
              break;
            }
            case AlphaMap.MODULATE: {
              alpha = scalarOps.Multiply(alpha, textureAlpha);
              break;
            }
            case AlphaMap.REPLACE: {
              alpha = textureAlpha;
              break;
            }
            case AlphaMap.ADD: {
              alpha = scalarOps.Add(alpha, textureAlpha);
              break;
            }
            case AlphaMap.SUB: {
              alpha = scalarOps.Subtract(alpha, textureAlpha);
              break;
            }
          }
        }

        switch (lightingPass) {
          case TObjFlags.LIGHTMAP_DIFFUSE: {
            diffuseSurfaceColor = color;
            diffuseSurfaceAlpha = alpha;
            break;
          }
          case TObjFlags.LIGHTMAP_AMBIENT: {
            ambientSurfaceColor = color;
            ambientSurfaceAlpha = alpha;
            break;
          }
          case TObjFlags.LIGHTMAP_SPECULAR: {
            specularSurfaceColor = color;
            specularSurfaceAlpha = alpha;
            break;
          }
        }
      }

      var ambientAndDiffuseLightingColor = colorOps.Add(
          colorOps.Multiply(ambientSurfaceColor, ambientLightColor),
          diffuseLightColor);

      // We double it because all the other kids do. (Other fixed-function games.)
      ambientAndDiffuseLightingColor =
          colorOps.MultiplyWithConstant(ambientAndDiffuseLightingColor, 2);

      var ambientAndDiffuseComponent = colorOps.Multiply(
          ambientAndDiffuseLightingColor,
          diffuseSurfaceColor);

      var specularComponent =
          colorOps.Multiply(specularSurfaceColor, specularLightColor);

      var outputColor = colorOps.Add(
          ambientAndDiffuseComponent,
          specularComponent);

      var outputAlpha = diffuseSurfaceAlpha;

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