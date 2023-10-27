using dat.schema;

using fin.io;
using fin.language.equations.fixedFunction;
using fin.language.equations.fixedFunction.impl;
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

            var finTextures = new ITexture[tObjsAndOffsets.Length];
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

                  finTexture.SetScale(tObj.ScaleS, tObj.ScaleT);

                  finTexturesByTObjOffset[tObjOffset] = finTexture;
                }

                finTextures[i] = finTexture;
              }
            }

            var fixedFunctionMaterial =
                finMaterialManager.AddFixedFunctionMaterial();
            finMaterial = fixedFunctionMaterial;

            this.PopulateFixedFunctionMaterial_(mObj,
                                                finTextures,
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
                                             pObjWeight.JObj.InverseBindMatrix,
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
                  GxOpcode.DRAW_TRIANGLES => finMesh.AddTriangles(finVertices),
                  GxOpcode.DRAW_QUADS     => finMesh.AddQuads(finVertices),
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

    private void PopulateFixedFunctionMaterial_(
        MObj mObj,
        IReadOnlyList<ITexture> finTextures,
        IFixedFunctionMaterial fixedFunctionMaterial) {
      var equations = fixedFunctionMaterial.Equations;

      var colorOps = new ColorFixedFunctionOps(equations);
      var scalarOps = new ScalarFixedFunctionOps(equations);

      var vertexColor = equations.CreateOrGetColorInput(
          FixedFunctionSource.VERTEX_COLOR_0);
      var vertexAlpha = equations.CreateOrGetScalarInput(
          FixedFunctionSource.VERTEX_ALPHA_0);

      IColorValue textureColor = colorOps.One;
      IScalarValue textureAlpha = scalarOps.One;
      if (finTextures.Count > 0) {
        fixedFunctionMaterial.SetTextureSource(0, finTextures[0]);
        textureColor = equations.CreateOrGetColorInput(
            FixedFunctionSource.TEXTURE_COLOR_0);
        textureAlpha = equations.CreateOrGetScalarInput(
            FixedFunctionSource.TEXTURE_ALPHA_0);
      }

      var renderMode = mObj.RenderMode;
      var material = mObj.Material;

      var outputColor = textureColor;

      if (renderMode.CheckFlag(RenderMode.VERTEX)) {
        outputColor = colorOps.Multiply(outputColor, vertexColor);
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
      } else if (renderMode.CheckFlag(RenderMode.CONSTANT)) {
        var diffuseRgba = material.DiffuseColor;
        var diffuseColor = equations.CreateColorConstant(diffuseRgba.Rf,
          diffuseRgba.Gf,
          diffuseRgba.Bf);

        outputColor = colorOps.Multiply(outputColor, diffuseColor);
      }

      var outputAlpha = textureAlpha;
      if (renderMode.CheckFlag(RenderMode.ALPHA_VTX)) {
        outputAlpha = scalarOps.Multiply(outputAlpha, vertexAlpha);
      }

      if (renderMode.CheckFlag(RenderMode.ALPHA_MAT)) {
        outputAlpha =
            scalarOps.MultiplyWithConstant(outputAlpha, material.Alpha);
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