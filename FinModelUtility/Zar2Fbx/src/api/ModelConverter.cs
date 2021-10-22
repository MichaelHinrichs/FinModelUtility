﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.math;
using fin.model;
using fin.model.impl;
using fin.util.asserts;

using zar.format.cmb;

namespace zar.api {
  public class ModelConverter {
    // TODO: Split these out into separate classes
    // TODO: Reading from the file here is gross
    public IModel Convert(EndianBinaryReader r, Cmb cmb) {
      var model = new ModelImpl();

      var finBones = new IBone[cmb.skl.bones.Length];
      var boneQueue = new Queue<(Bone, IBone?)>();
      boneQueue.Enqueue((cmb.skl.bones[0], null));
      while (boneQueue.Count > 0) {
        var (cmbBone, finBoneParent) = boneQueue.Dequeue();

        var translation = cmbBone.translation;
        var radians = cmbBone.rotation;
        var scale = cmbBone.scale;

        var finBone =
            (finBoneParent ?? model.Skeleton.Root)
            .AddChild(
                translation[0],
                translation[1],
                translation[2])
            .SetLocalRotationRadians(
                radians[0],
                radians[1],
                radians[2])
            .SetLocalScale(
                scale[0],
                scale[1],
                scale[2]);
        finBones[cmbBone.id] = finBone;

        foreach (var child in cmbBone.children) {
          boneQueue.Enqueue((child, finBone));
        }
      }

      // Creates meshes & textures
      // TODO: Emulate fixed-function materials
      var finMaterials = new List<IMaterial>();
      var ctrTexture = new CtrTexture();
      foreach (var cmbMaterial in cmb.mat.materials) {
        // Get associated texture
        var texMapper = cmbMaterial.texMappers[0];
        var cmbTexture = cmb.tex.textures[texMapper.textureId];

        r.Position = cmb.startOffset +
                     cmb.header.textureDataOffset +
                     cmbTexture.dataOffset;
        var data = r.ReadBytes((int) cmbTexture.dataLength);
        var bitmap = ctrTexture.DecodeImage(data, cmbTexture);

        var finTexture = model.MaterialManager.CreateTexture(bitmap);
        finTexture.Name = cmbTexture.name;
        finTexture.WrapModeU = this.CmbToFinWrapMode(texMapper.wrapS);
        finTexture.WrapModeV = this.CmbToFinWrapMode(texMapper.wrapT);

        // Create material
        var finMaterial = model.MaterialManager.AddTextureMaterial(finTexture);
        finMaterials.Add(finMaterial);
      }

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
        var bIndices = new List<short>();
        foreach (var pset in shape.primitiveSets) {
          foreach (var i in pset.primitive.indices) {
            if (hasBi && pset.skinningMode != SkinningMode.Single) {
              r.Position = cmb.startOffset +
                           cmb.header.vatrOffset +
                           cmb.vatr.bIndices.startOffset +
                           shape.bIndices.start +
                           i * shape.boneDimensions;
              for (var bi = 0; bi < shape.boneDimensions; ++bi) {
                var boneTableIndex = shape.bIndices.scale *
                                     DataTypeUtil.Read(
                                         r,
                                         shape.bIndices.dataType);
                bIndices.Add(pset.boneTable[(int) boneTableIndex]);
              }
            } else {
              bIndices.Add(shape.primitiveSets[0].boneTable[0]);
            }
          }
        }

        var finMesh = model.Skin.AddMesh();

        // TODO: Encapsulate these reads somewhere else
        // Get vertices
        var finVertices = new IVertex[vertexCount];
        for (var i = 0; i < vertexCount; ++i) {
          // Position
          r.Position = cmb.startOffset +
                       cmb.header.vatrOffset +
                       cmb.vatr.position.startOffset +
                       shape.position.start +
                       3 * DataTypeUtil.GetSize(shape.position.dataType) * i;
          var positionValues =
              DataTypeUtil.Read(r, 3, shape.position.dataType)
                          .Select(value => value * shape.position.scale)
                          .ToArray();

          var finVertex = model.Skin.AddVertex(positionValues[0],
                                               positionValues[1],
                                               positionValues[2]);
          finVertices[i] = finVertex;

          if (hasNrm) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.normal.startOffset +
                         shape.normal.start +
                         3 * DataTypeUtil.GetSize(shape.normal.dataType) * i;
            var normalValues =
                DataTypeUtil.Read(r, 3, shape.normal.dataType)
                            .Select(value => value * shape.normal.scale)
                            .ToArray();
            finVertex.SetLocalNormal(normalValues[0],
                                     normalValues[1],
                                     normalValues[2]);
          }

          if (hasClr) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.color.startOffset +
                         shape.color.start +
                         4 * DataTypeUtil.GetSize(shape.color.dataType) * i;
            var colorValues =
                DataTypeUtil.Read(r, 4, shape.color.dataType)
                            .Select(value => value * shape.color.scale)
                            .ToArray();

            Asserts.Equal(DataType.Float, shape.color.dataType);
            finVertex.SetColorBytes((byte) (colorValues[0] * 255),
                                    (byte) (colorValues[1] * 255),
                                    (byte) (colorValues[2] * 255),
                                    (byte) (colorValues[3] * 255));
          }

          if (hasUv0) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.uv0.startOffset +
                         shape.uv0.start +
                         2 * DataTypeUtil.GetSize(shape.uv0.dataType) * i;
            var uv0Values =
                DataTypeUtil.Read(r, 2, shape.uv0.dataType)
                            .Select(value => value * shape.uv0.scale)
                            .ToArray();

            finVertex.SetUv(0, uv0Values[0], uv0Values[1]);
          }
          if (hasUv1) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.uv1.startOffset +
                         shape.uv1.start +
                         2 * DataTypeUtil.GetSize(shape.uv1.dataType) * i;
            var uv1Values =
                DataTypeUtil.Read(r, 2, shape.uv1.dataType)
                            .Select(value => value * shape.uv1.scale)
                            .ToArray();

            finVertex.SetUv(1, uv1Values[0], uv1Values[1]);
          }
          if (hasUv2) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.uv2.startOffset +
                         shape.uv2.start +
                         2 * DataTypeUtil.GetSize(shape.uv2.dataType) * i;
            var uv2Values =
                DataTypeUtil.Read(r, 2, shape.uv2.dataType)
                            .Select(value => value * shape.uv2.scale)
                            .ToArray();

            finVertex.SetUv(2, uv2Values[0], uv2Values[1]);
          }

          if (hasBw) {
            r.Position = cmb.startOffset +
                         cmb.header.vatrOffset +
                         cmb.vatr.bWeights.startOffset +
                         shape.bWeights.start +
                         i * shape.boneDimensions;

            var totalWeight = 0f;
            var boneWeights = new List<BoneWeight>();
            for (var j = 0; j < shape.boneDimensions; ++j) {
              // TODO: Looks like this is rounded to the nearest 2 in the original??
              var weight = DataTypeUtil.Read(r, shape.bWeights.dataType) *
                           shape.bWeights.scale;
              totalWeight += weight;

              if (weight > 0) {
                var bone =
                    finBones[bIndices[i * shape.boneDimensions + j]];
                var boneWeight =
                    new BoneWeight(bone,
                                   new FinMatrix4x4().SetIdentity(),
                                   weight);

                boneWeights.Add(boneWeight);
              }
            }

            Asserts.True(boneWeights.Count > 0);
            Asserts.True(Math.Abs(1 - totalWeight) < .0001);
            finVertex.SetBones(boneWeights.ToArray());
          } else {
            var boneIndex = bIndices[i];
            finVertex.SetBone(finBones[boneIndex]);
          }

          finVertex.Preproject =
              shape.primitiveSets[0].skinningMode != SkinningMode.Smooth;
        }

        // Adds faces. Thankfully, it's all just triangles!
        var triangleVertices = new IVertex[meshIndices.Count];
        for (var i = 0; i < meshIndices.Count; i += 3) {
          // TODO: Encapsulate this option in the primitive
          // Have to flip faces to get this to work
          triangleVertices[i] = finVertices[meshIndices[i]];
          triangleVertices[i + 1] = finVertices[meshIndices[i + 2]];
          triangleVertices[i + 2] = finVertices[meshIndices[i + 1]];
        }
        finMesh.AddTriangles(triangleVertices)
               .SetMaterial(finMaterials[cmbMesh.materialIndex]);
      }

      return model;
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