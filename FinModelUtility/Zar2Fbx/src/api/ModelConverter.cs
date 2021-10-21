using System.Collections;
using System.Collections.Generic;
using System.Linq;

using fin.math;
using fin.model;
using fin.model.impl;

using zar.format.cmb;

namespace zar.api {
  public class ModelConverter {
    // TODO: Split these out into separate classes
    public IModel Convert(Cmb cmb) {
      var model = new ModelImpl();

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

        foreach (var child in cmbBone.children) {
          boneQueue.Enqueue((child, finBone));
        }
      }

      // TODO: Keep these separate in the exported model
      foreach (var mesh in cmb.sklm.meshes.meshes) {
        var shape = cmb.sklm.shapes.shapes[mesh.shapeIndex];

        //var indices = shape.primitiveSets.Select(pset => pset.primitive.indices)

        // Gets flags
        var index = 0;
        var hasNrm = BitLogic.ExtractFromRight(shape.vertFlags, index++, 1);
        if (cmb.header.version > CmbVersion.OCARINA_OF_TIME_3D) {
          // Skip "HasTangents" for now
          index++;
        }
        var hasClr = BitLogic.ExtractFromRight(shape.vertFlags, index++, 1);
        var hasUv0 = BitLogic.ExtractFromRight(shape.vertFlags, index++, 1);
        var hasUv1 = BitLogic.ExtractFromRight(shape.vertFlags, index++, 1);
        var hasBi = BitLogic.ExtractFromRight(shape.vertFlags, index++, 1);
        var hasBw = BitLogic.ExtractFromRight(shape.vertFlags, index++, 1);
      }

      return model;
    }
  }
}

/*indices = [faces for pset in shape.primitiveSets for faces in pset.primitive.indices]

      vertexCount = max(indices) + 1

      vertices = []

      bindices = { }

        # Create new mesh
nmesh = bpy.data.meshes.new('Order:{}_VisID:{}'.format(MIndex, mesh.ID))# ID is used for visibility animations
        MIndex += 1
        nmesh.use_auto_smooth = True# Needed for custom split normals
        nmesh.materials.append(bpy.data.materials.get(materialNames[mesh.materialIndex]))# Add material to mesh

        obj = bpy.data.objects.new(nmesh.name, nmesh)# Create new mesh object
        obj.parent = skl_obj# Set parent skeleton

        ArmMod = obj.modifiers.new(skl_obj.name, "ARMATURE")
        ArmMod.object = skl_obj# Set the modifiers armature

        for bone in bpy.data.armatures[skeleton.name].bones.values():
            obj.vertex_groups.new(name = bone.name)

        # Get bone indices. We need to get these first because-
# each primitive has it's own bone table
  for s in shape.primitiveSets:
            for i in s.primitive.indices:
                if (hasBi and s.skinningMode != SkinningMode.Single):
                    f.seek(cmb.vatrOfs + vb.bIndices.startOfs + shape.bIndices.start + i * shape.boneDimensions + startOff)
                    for bi in range(shape.boneDimensions):
                        bindices[i * shape.boneDimensions + bi] = s.boneTable[int(readDataType(f, shape.bIndices.dataType) * shape.bIndices.scale)]
                else: bindices[i] = shape.primitiveSets[0].boneTable[0]# For single-bind meshes

        # Create new bmesh
        bm = bmesh.new()
        bm.from_mesh(nmesh)
        weight_layer = bm.verts.layers.deform.new()# Add new deform layer

        # TODO: Support constants
        # Get vertices
        for i in range(vertexCount):
            v = Vertex()# Ugly because I don't care :)

            # Position
            f.seek(cmb.vatrOfs + vb.position.startOfs + shape.position.start + 3 * getDataTypeSize(shape.position.dataType) * i + startOff)
            bmv = bm.verts.new([e * shape.position.scale * GLOBAL_SCALE for e in readArray(f, 3, shape.position.dataType)])

    if (shape.primitiveSets[0].skinningMode != SkinningMode.Smooth):
                bmv.co = transformPosition(bmv.co, boneTransforms[bindices[i]])

            # Normal
if hasNrm:
                f.seek(cmb.vatrOfs + vb.normal.startOfs + shape.normal.start + 3 * getDataTypeSize(shape.normal.dataType) * i + startOff)
                v.nrm = [e * shape.normal.scale for e in readArray(f, 3, shape.normal.dataType)]

                if (shape.primitiveSets[0].skinningMode != SkinningMode.Smooth):
                    v.nrm = transformNormal(v.nrm, boneTransforms[bindices[i]])

            # Color
if hasClr:
                f.seek(cmb.vatrOfs + vb.color.startOfs + shape.color.start + 4 * getDataTypeSize(shape.color.dataType) * i + startOff)
                v.clr = [e * shape.color.scale for e in readArray(f, 4, shape.color.dataType)]

            # UV0
  if hasUv0:
                f.seek(cmb.vatrOfs + vb.uv0.startOfs + shape.uv0.start + 2 * getDataTypeSize(shape.uv0.dataType) * i + startOff)
                v.uv0 = [e * shape.uv0.scale for e in readArray(f, 2, shape.uv0.dataType)]

            # UV1
  if hasUv1:
                f.seek(cmb.vatrOfs + vb.uv1.startOfs + shape.uv1.start + 2 * getDataTypeSize(shape.uv1.dataType) * i + startOff)
                v.uv1 = [e * shape.uv1.scale for e in readArray(f, 2, shape.uv1.dataType)]

            # UV2
  if hasUv2:
                f.seek(cmb.vatrOfs + vb.uv2.startOfs + shape.uv2.start + 2 * getDataTypeSize(shape.uv2.dataType) * i + startOff)
                v.uv2 = [e * shape.uv2.scale for e in readArray(f, 2, shape.uv2.dataType)]

            # Bone Weights
  if hasBw:
                # For smooth meshes
                f.seek(cmb.vatrOfs + vb.bWeights.startOfs + shape.bWeights.start + i * shape.boneDimensions + startOff)
                for j in range(shape.boneDimensions):
                    weight = round(readDataType(f, shape.bWeights.dataType) * shape.bWeights.scale, 2)
                    if (weight > 0): bmv[weight_layer][bindices[i * shape.boneDimensions + j]] = weight
            else:
                # For single-bind meshes
                bmv[weight_layer][bindices[i]] = 1.0

            vertices.append(v)

        bm.verts.ensure_lookup_table()# Must always be called after adding/removing vertices or accessing them by index
        bm.verts.index_update()# Assign an index value to each vertex

        for i in range(0, len(indices), 3):
            try:
                face = bm.faces.new(bm.verts[j] for j in indices[i: i + 3])
    face.material_index = mesh.materialIndex
                face.smooth = True
            except:# face already exists
                continue

        uv_layer0 = bm.loops.layers.uv.new() if (hasUv0) else None
        uv_layer1 = bm.loops.layers.uv.new() if (hasUv1) else None
        uv_layer2 = bm.loops.layers.uv.new() if (hasUv2) else None
        col_layer = bm.loops.layers.color.new() if (hasClr) else None

        for face in bm.faces:
            for loop in face.loops:
                if hasUv0:
                    uv0 = vertices[loop.vert.index].uv0
                    loop[uv_layer0].uv = (uv0[0], uv0[1]) # Flip Y
                if hasUv1:
                    uv1 = vertices[loop.vert.index].uv1
                    loop[uv_layer1].uv = (uv1[0], 1 - uv1[1]) # Flip Y
                if hasUv2:
                    uv2 = vertices[loop.vert.index].uv2
                    loop[uv_layer2].uv = (uv2[0], 1 - uv2[1]) # Flip Y
                if hasClr:
                    loop[col_layer] = vertices[loop.vert.index].clr*/