using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.math;
using fin.model;
using fin.model.impl;
using fin.util.asserts;
using fin.util.data;

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

      // TODO: Keep these separate in the exported model
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
        var inc = 0;
        var hasNrm = BitLogic.ExtractFromRight(shape.vertFlags, inc++, 1) != 0;
        if (cmb.header.version > CmbVersion.OCARINA_OF_TIME_3D) {
          // Skip "HasTangents" for now
          inc++;
        }
        var hasClr = BitLogic.ExtractFromRight(shape.vertFlags, inc++, 1) != 0;
        var hasUv0 = BitLogic.ExtractFromRight(shape.vertFlags, inc++, 1) != 0;
        var hasUv1 = BitLogic.ExtractFromRight(shape.vertFlags, inc++, 1) != 0;
        var hasBi = BitLogic.ExtractFromRight(shape.vertFlags, inc++, 1) != 0;
        var hasBw = BitLogic.ExtractFromRight(shape.vertFlags, inc++, 1) != 0;

        // Gets bone indices
        var bIndices = new List<short>();
        foreach (var pset in shape.primitiveSets) {
          foreach (var i in pset.primitive.indices) {
            if (hasBi && pset.skinningMode != SkinningMode.Single) {
              r.Position = cmb.startOffset +
                           cmb.header.vatrOffset +
                           shape.bIndices.start +
                           i * shape.boneDimensions;
              for (var bi = 0; bi < shape.boneDimensions; ++bi) {
                var boneTableIndex = shape.bIndices.scale *
                                     shape.bIndices.dataType switch {
                                         _ => throw new
                                                  NotImplementedException()
                                     };
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
            Asserts.Equal(1, totalWeight);
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
        for (var i = 0; i < meshIndices.Count; ++i) {
          triangleVertices[i] = finVertices[meshIndices[i]];
        }
        finMesh.AddTriangles(triangleVertices);
      }

      /*# UV0
            if hasUv0:
            f.seek(cmb.vatrOfs +
                   vb.uv0.startOfs +
                   shape.uv0.start +
                   2 * getDataTypeSize(shape.uv0.dataType) * i +
                   startOff)
            v.uv0 =  [e
            *shape.uv0.scale for e in
            readArray(f, 2, shape.uv0.dataType)]

      # UV1
            if hasUv1:
            f.seek(cmb.vatrOfs +
                   vb.uv1.startOfs +
                   shape.uv1.start +
                   2 * getDataTypeSize(shape.uv1.dataType) * i +
                   startOff)
            v.uv1 =  [e
            *shape.uv1.scale for e in
            readArray(f, 2, shape.uv1.dataType)]

      # UV2
            if hasUv2:
            f.seek(cmb.vatrOfs +
                   vb.uv2.startOfs +
                   shape.uv2.start +
                   2 * getDataTypeSize(shape.uv2.dataType) * i +
                   startOff)
            v.uv2 =  [e
            *shape.uv2.scale for e in
            readArray(f, 2, shape.uv2.dataType)]


            vertices.append(v)
          }*/


      return model;
    }
  }
}