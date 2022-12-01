using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using cmb.schema.cmb;
using cmb.schema.csab;
using cmb.schema.ctxb;
using cmb.schema.shpa;
using fin.color;
using fin.data;
using fin.data.queue;
using fin.image;
using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;
using fin.util.asserts;


namespace cmb.api {
  public class CmbModelFileBundle : IModelFileBundle {
    public CmbModelFileBundle(IFileHierarchyFile cmbFile,
                              IReadOnlyList<IFileHierarchyFile>? csabFiles,
                              IReadOnlyList<IFileHierarchyFile>? ctxbFiles,
                              IReadOnlyList<IFileHierarchyFile>? shpaFiles) {
      this.CmbFile = cmbFile;
      this.CsabFiles = csabFiles;
      this.CtxbFiles = ctxbFiles;
      this.ShpaFiles = shpaFiles;
    }

    public IFileHierarchyFile MainFile => this.CmbFile;

    public IFileHierarchyFile CmbFile { get; }
    public IReadOnlyList<IFileHierarchyFile>? CsabFiles { get; }
    public IReadOnlyList<IFileHierarchyFile>? CtxbFiles { get; }
    public IReadOnlyList<IFileHierarchyFile>? ShpaFiles { get; }
  }

  public class CmbModelLoader : IModelLoader<CmbModelFileBundle> {
    // TODO: Split these out into separate classes
    // TODO: Reading from the file here is gross
    public IModel LoadModel(CmbModelFileBundle modelFileBundle) {
      var cmbFile = modelFileBundle.CmbFile;
      var csabFiles = modelFileBundle.CsabFiles;
      var ctxbFiles = modelFileBundle.CtxbFiles;
      var shpaFiles = modelFileBundle.ShpaFiles;

      var fps = 30;

      using var r =
          new EndianBinaryReader(cmbFile.Impl.OpenRead(),
                                 Endianness.LittleEndian);

      var cmb = new Cmb(r);
      r.Position = 0;

      var filesAndCsabs =
          csabFiles?.Select(csabFile => {
                     var csab =
                         csabFile.Impl.ReadNew<Csab>(Endianness.LittleEndian);
                     return (csabFile, csab);
                   })
                   .ToList() ??
          new List<(IFileHierarchyFile shpaFile, Csab csab)>();

      var filesAndCtxbs =
          ctxbFiles?.Select(ctxbFile => {
                     var ctxb = ctxbFile.Impl.ReadNew<Ctxb>();
                     return (ctxbFile, ctxb);
                   })
                   .ToList() ??
          new List<(IFileHierarchyFile shpaFile, Ctxb ctxb)>();

      var filesAndShpas =
          shpaFiles?.Select(shpaFile => {
                     var shpa =
                         shpaFile.Impl.ReadNew<Shpa>(Endianness.LittleEndian);
                     return (shpaFile, shpa);
                   })
                   .ToList() ??
          new List<(IFileHierarchyFile shpaFile, Shpa shpa)>();

      var finModel = new ModelImpl();
      var finSkin = finModel.Skin;

      // Adds bones
      var boneChildren = new ListDictionary<Bone, Bone>();
      foreach (var bone in cmb.skl.bones) {
        var parentId = bone.parentId;
        if (parentId != -1) {
          boneChildren.Add(cmb.skl.bones[parentId], bone);
        }
      }

      var finBones = new IBone[cmb.skl.bones.Length];
      var boneQueue =
          new FinTuple2Queue<Bone, IBone?>((cmb.skl.bones[0], null));
      while (boneQueue.TryDequeue(out var cmbBone, out var finBoneParent)) {
        var translation = cmbBone.translation;
        var radians = cmbBone.rotation;
        var scale = cmbBone.scale;

        var finBone =
            (finBoneParent ?? finModel.Skeleton.Root)
            .AddChild(translation.X, translation.Y, translation.Z)
            .SetLocalRotationRadians(radians.X, radians.Y, radians.Z)
            .SetLocalScale(scale.X, scale.Y, scale.Z);
        finBones[cmbBone.id] = finBone;

        if (boneChildren.TryGetList(cmbBone, out var children)) {
          boneQueue.Enqueue(children!.Select(child => (child, finBone)));
        }
      }

      // Adds animations
      foreach (var (csabFile, csab) in filesAndCsabs) {
        var finAnimation = finModel.AnimationManager.AddAnimation();
        finAnimation.Name = csabFile.NameWithoutExtension;

        finAnimation.FrameCount = (int)csab.Duration;
        finAnimation.FrameRate = fps;

        foreach (var (boneIndex, anod) in csab.BoneIndexToAnimationNode) {
          var boneTracks = finAnimation.AddBoneTracks(finBones[boneIndex]);

          foreach (var translationX in anod.TranslationX.Keyframes) {
            boneTracks.Positions.Set((int)translationX.Time,
                                     0,
                                     translationX.Value,
                                     translationX.IncomingTangent,
                                     translationX.OutgoingTangent);
          }
          foreach (var translationY in anod.TranslationY.Keyframes) {
            boneTracks.Positions.Set((int)translationY.Time,
                                     1,
                                     translationY.Value,
                                     translationY.IncomingTangent,
                                     translationY.OutgoingTangent);
          }
          foreach (var translationZ in anod.TranslationZ.Keyframes) {
            boneTracks.Positions.Set((int)translationZ.Time,
                                     2,
                                     translationZ.Value,
                                     translationZ.IncomingTangent,
                                     translationZ.OutgoingTangent);
          }

          foreach (var scaleX in anod.ScaleX.Keyframes) {
            boneTracks.Scales.Set((int)scaleX.Time,
                                  0,
                                  scaleX.Value,
                                  scaleX.IncomingTangent,
                                  scaleX.OutgoingTangent);
          }
          foreach (var scaleY in anod.ScaleY.Keyframes) {
            boneTracks.Scales.Set((int)scaleY.Time,
                                  1,
                                  scaleY.Value,
                                  scaleY.IncomingTangent,
                                  scaleY.OutgoingTangent);
          }
          foreach (var scaleZ in anod.ScaleZ.Keyframes) {
            boneTracks.Scales.Set((int)scaleZ.Time,
                                  2,
                                  scaleZ.Value,
                                  scaleZ.IncomingTangent,
                                  scaleZ.OutgoingTangent);
          }

          foreach (var rotationX in anod.RotationX.Keyframes) {
            boneTracks.Rotations.Set((int)rotationX.Time,
                                     0,
                                     rotationX.Value,
                                     rotationX.IncomingTangent,
                                     rotationX.OutgoingTangent);
          }
          foreach (var rotationY in anod.RotationY.Keyframes) {
            boneTracks.Rotations.Set((int)rotationY.Time,
                                     1,
                                     rotationY.Value,
                                     rotationY.IncomingTangent,
                                     rotationY.OutgoingTangent);
          }
          foreach (var rotationZ in anod.RotationZ.Keyframes) {
            boneTracks.Rotations.Set((int)rotationZ.Time,
                                     2,
                                     rotationZ.Value,
                                     rotationZ.IncomingTangent,
                                     rotationZ.OutgoingTangent);
          }
        }
      }

      // TODO: Move these reads into the model reading logic
      var ctrTexture = new CtrTexture();
      var textureImages =
          cmb.tex.textures.Select(cmbTexture => {
               var position = cmb.startOffset +
                              cmb.header.textureDataOffset +
                              cmbTexture.dataOffset;
               IImage image;
               if (position != 0) {
                 r.Position = position;
                 var data =
                     r.ReadBytes((int)cmbTexture.dataLength);
                 image =
                     ctrTexture.DecodeImage(data, cmbTexture);
               } else {
                 var ctxb =
                     filesAndCtxbs
                         .Select(
                             fileAndCtxb => fileAndCtxb.Item2)
                         .Single(
                             ctxb => ctxb.Chunk.Entry.name == cmbTexture.name);
                 image =
                     ctrTexture.DecodeImage(ctxb.Data,
                                            cmbTexture);
               }
               return image;
             })
             .ToArray();

      // Creates meshes & textures
      // TODO: Emulate fixed-function materials
      var finMaterials = new List<IMaterial>();
      for (var i = 0; i < cmb.mat.materials.Length; ++i) {
        var cmbMaterial = cmb.mat.materials[i];

        // Get associated texture
        var texMapper = cmbMaterial.texMappers[0];
        var textureId = texMapper.textureId;

        ITexture? finTexture = null;
        if (textureId != -1) {
          var cmbTexture = cmb.tex.textures[textureId];
          var textureImage = textureImages[textureId];

          finTexture = finModel.MaterialManager.CreateTexture(textureImage);
          finTexture.Name = cmbTexture.name;
          finTexture.WrapModeU = this.CmbToFinWrapMode(texMapper.wrapS);
          finTexture.WrapModeV = this.CmbToFinWrapMode(texMapper.wrapT);

          var cmbBorderColor = texMapper.BorderColor;
          finTexture.BorderColor = cmbBorderColor;
        }

        // Create material
        IMaterial finMaterial = finTexture != null
                                    ? finModel.MaterialManager
                                              .AddTextureMaterial(
                                                  finTexture)
                                    : finModel.MaterialManager
                                              .AddNullMaterial();
        finMaterial.Name = $"material{i}";
        finMaterial.CullingMode = cmbMaterial.faceCulling switch {
            CullMode.FrontAndBack => CullingMode.SHOW_BOTH,
            CullMode.Front        => CullingMode.SHOW_FRONT_ONLY,
            CullMode.BackFace     => CullingMode.SHOW_BACK_ONLY,
            CullMode.Never        => CullingMode.SHOW_NEITHER,
            _                     => throw new NotImplementedException(),
        };

        finMaterials.Add(finMaterial);
      }

      /*{
        var nameToTextures = new Dictionary<string, ITexture>();
        foreach (var finMaterial in finMaterials) {
          foreach (var finTexture in finMaterial.Textures) {
            nameToTextures[finTexture.Name] = finTexture;
          }
        }

        foreach (var (_, finTexture) in nameToTextures) {
          finTexture.ImageData.Save(
              Path.Join(outputDirectory.FullName, finTexture.Name + ".png"));
        }
      }*/

      var verticesByIndex = new ListDictionary<int, IVertex>();

      // Adds meshes
      foreach (var cmbMesh in cmb.sklm.meshes.meshes) {
        var shape = cmb.sklm.shapes.shapes[cmbMesh.shapeIndex];

        uint vertexCount = 0;
        var meshIndices = new List<uint>();
        foreach (var pset in shape.primitiveSets) {
          foreach (var index in pset.primitive.indices) {
            meshIndices.Add(index);
            vertexCount = Math.Max(vertexCount, index);
          }
        }
        ++vertexCount;

        var preproject = new bool?[vertexCount];
        var skinningModes = new SkinningMode?[vertexCount];
        foreach (var pset in shape.primitiveSets) {
          foreach (var index in pset.primitive.indices) {
            skinningModes[index] = pset.skinningMode;
            preproject[index] = pset.skinningMode != SkinningMode.Smooth;
          }
        }

        // Gets flags
        var inc = 1;
        var hasNrm = BitLogic.GetFlag(shape.vertFlags, inc++);
        if (cmb.header.version > CmbVersion.OCARINA_OF_TIME_3D) {
          // Skip "HasTangents" for now
          inc++;
        }
        var hasClr = BitLogic.GetFlag(shape.vertFlags, inc++);
        var hasUv0 = BitLogic.GetFlag(shape.vertFlags, inc++);
        var hasUv1 = BitLogic.GetFlag(shape.vertFlags, inc++);
        var hasUv2 = BitLogic.GetFlag(shape.vertFlags, inc++);
        var hasBi = BitLogic.GetFlag(shape.vertFlags, inc++);
        var hasBw = BitLogic.GetFlag(shape.vertFlags, inc++);

        // Gets bone indices
        var boneCount = shape.boneDimensions;
        var bIndices = new short[vertexCount * boneCount];
        foreach (var pset in shape.primitiveSets) {
          foreach (var i in pset.primitive.indices) {
            if (hasBi && pset.skinningMode != SkinningMode.Single) {
              r.Position = cmb.startOffset +
                           cmb.header.vatrOffset +
                           cmb.vatr.bIndices.StartOffset +
                           shape.bIndices.Start +
                           i *
                           DataTypeUtil.GetSize(shape.bIndices.DataType) *
                           shape.boneDimensions;
              for (var bi = 0; bi < shape.boneDimensions; ++bi) {
                var boneTableIndex = shape.bIndices.Scale *
                                     DataTypeUtil.Read(
                                         r,
                                         shape.bIndices.DataType);
                bIndices[i * boneCount + bi] =
                    pset.boneTable[(int)boneTableIndex];
              }
            } else {
              bIndices[i] = shape.primitiveSets[0].boneTable[0];
            }
          }
        }

        var finMesh = finSkin.AddMesh();

        // TODO: Encapsulate these reads somewhere else
        // Get vertices
        var finVertices = new IVertex[vertexCount];
        for (var i = 0; i < vertexCount; ++i) {
          // Position
          r.Position = cmb.startOffset +
                       cmb.header.vatrOffset +
                       cmb.vatr.position.StartOffset +
                       shape.position.Start +
                       3 * DataTypeUtil.GetSize(shape.position.DataType) * i;
          var positionValues =
              DataTypeUtil.Read(r, 3, shape.position.DataType)
                          .Select(value => value * shape.position.Scale)
                          .ToArray();

          var finVertex = finSkin.AddVertex(positionValues[0],
                                            positionValues[1],
                                            positionValues[2]);
          finVertices[i] = finVertex;

          var index = (ushort)(shape.position.Start / 3 + i);
          verticesByIndex.Add(index, finVertex);

          if (hasNrm) {
            float[] normalValues;
            if (shape.normal.Mode == VertexAttributeMode.Constant) {
              normalValues = shape.normal.Constants;
            } else {
              r.Position = cmb.startOffset +
                           cmb.header.vatrOffset +
                           cmb.vatr.normal.StartOffset +
                           shape.normal.Start +
                           3 * DataTypeUtil.GetSize(shape.normal.DataType) * i;
              normalValues =
                  DataTypeUtil.Read(r, 3, shape.normal.DataType)
                              .Select(value => value * shape.normal.Scale)
                              .ToArray();
            }
            finVertex.SetLocalNormal(normalValues[0],
                                     normalValues[1],
                                     normalValues[2]);
          }

          if (hasClr) {
            float[] colorValues;
            if (shape.normal.Mode == VertexAttributeMode.Constant) {
              colorValues = shape.color.Constants;
            } else {
              r.Position = cmb.startOffset +
                           cmb.header.vatrOffset +
                           cmb.vatr.color.StartOffset +
                           shape.color.Start +
                           4 * DataTypeUtil.GetSize(shape.color.DataType) * i;
              colorValues =
                  DataTypeUtil.Read(r, 4, shape.color.DataType)
                              .Select(value => value * shape.color.Scale)
                              .ToArray();
            }
            finVertex.SetColorBytes((byte)(colorValues[0] * 255),
                                    (byte)(colorValues[1] * 255),
                                    (byte)(colorValues[2] * 255),
                                    (byte)(colorValues[3] * 255));
          }

          if (hasUv0) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.uv0.StartOffset +
                         shape.uv0.Start +
                         2 * DataTypeUtil.GetSize(shape.uv0.DataType) * i;
            var uv0Values =
                DataTypeUtil.Read(r, 2, shape.uv0.DataType)
                            .Select(value => value * shape.uv0.Scale)
                            .ToArray();

            finVertex.SetUv(0, uv0Values[0], 1 - uv0Values[1]);
          }
          if (hasUv1) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.uv1.StartOffset +
                         shape.uv1.Start +
                         2 * DataTypeUtil.GetSize(shape.uv1.DataType) * i;
            var uv1Values =
                DataTypeUtil.Read(r, 2, shape.uv1.DataType)
                            .Select(value => value * shape.uv1.Scale)
                            .ToArray();

            finVertex.SetUv(1, uv1Values[0], 1 - uv1Values[1]);
          }
          if (hasUv2) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.uv2.StartOffset +
                         shape.uv2.Start +
                         2 * DataTypeUtil.GetSize(shape.uv2.DataType) * i;
            var uv2Values =
                DataTypeUtil.Read(r, 2, shape.uv2.DataType)
                            .Select(value => value * shape.uv2.Scale)
                            .ToArray();

            finVertex.SetUv(2, uv2Values[0], 1 - uv2Values[1]);
          }

          var preprojectMode = preproject[i].Value
                                   ? PreprojectMode.BONE
                                   : PreprojectMode.NONE;

          if (hasBw) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.bWeights.StartOffset +
                         shape.bWeights.Start +
                         i *
                         DataTypeUtil.GetSize(shape.bWeights.DataType) *
                         boneCount;

            var totalWeight = 0f;
            var boneWeights = new List<BoneWeight>();
            for (var j = 0; j < boneCount; ++j) {
              // TODO: Looks like this is rounded to the nearest 2 in the original??
              var weight = DataTypeUtil.Read(r, shape.bWeights.DataType) *
                           shape.bWeights.Scale;
              totalWeight += weight;

              if (weight > 0) {
                var bone =
                    finBones[bIndices[i * boneCount + j]];
                var boneWeight = new BoneWeight(bone, null, weight);

                boneWeights.Add(boneWeight);
              }
            }

            Asserts.True(boneWeights.Count > 0);
            Asserts.True(Math.Abs(1 - totalWeight) < .0001);
            finVertex.SetBoneWeights(
                finSkin.GetOrCreateBoneWeights(preprojectMode,
                                               boneWeights.ToArray()));
          } else {
            var boneIndex = bIndices[i];
            finVertex.SetBoneWeights(
                finSkin.GetOrCreateBoneWeights(preprojectMode,
                                               finBones[boneIndex]));
          }

          finVertex.SetColor(FinColor.FromSystemColor(Color.White));
        }

        // Adds faces. Thankfully, it's all just triangles!
        var triangleVertices = meshIndices
                               .Select(meshIndex => finVertices[meshIndex])
                               .ToArray();
        finMesh.AddTriangles(triangleVertices)
               .SetMaterial(finMaterials[cmbMesh.materialIndex])
               .SetVertexOrder(VertexOrder.NORMAL);
      }

      // Adds morph targets
      foreach (var (shpaFile, shpa) in filesAndShpas) {
        var shpaIndexToPosi =
            shpa?.Posi.Values.Select((posi, i) => (shpa.Idxs.Indices[i], posi))
                .ToDictionary(indexAndPosi => indexAndPosi.Item1,
                              indexAndPosi => indexAndPosi.posi);

        var morphTarget = finModel.AnimationManager.AddMorphTarget();
        morphTarget.Name = shpaFile.NameWithoutExtension;

        foreach (var (index, position) in shpaIndexToPosi) {
          if (!verticesByIndex.TryGetList(index, out var finVertices)) {
            continue;
          }

          foreach (var finVertex in finVertices) {
            morphTarget.MoveTo(finVertex, position);
          }
        }
      }

      return finModel;
    }

    public WrapMode CmbToFinWrapMode(TextureWrapMode cmbMode)
      => cmbMode switch {
          // TODO: Darn, we can't support border colors
          TextureWrapMode.ClampToBorder => WrapMode.CLAMP,
          TextureWrapMode.Repeat => WrapMode.REPEAT,
          TextureWrapMode.ClampToEdge => WrapMode.CLAMP,
          TextureWrapMode.Mirror => WrapMode.MIRROR_REPEAT,
          _ => throw new ArgumentOutOfRangeException()
      };
  }
}