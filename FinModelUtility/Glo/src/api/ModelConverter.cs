using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;
using glo.schema;
using System.Numerics;

namespace glo.api {
  public class ModelConverter {
    public IModel Convert(
        Glo glo,
        IDirectory outputDirectory,
        float fps) {
      var finModel = new ModelImpl();

      var finRoot = finModel.Skeleton.Root;

      var meshQueue = new Queue<(GloMesh, IBone)>();
      meshQueue.Enqueue((glo.Meshes[0], finRoot));

      while (meshQueue.Count > 0) {
        var (gloMesh, parentFinBone) = meshQueue.Dequeue();

        var position = gloMesh.MoveKeys[0];

        var rotation = gloMesh.RotateKeys[0];
        var quaternion = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
        var xyzRadians = QuaternionUtil.ToEulerRadians(quaternion);

        var finBone = parentFinBone.AddChild(position.X, position.Y, position.Z).SetLocalRotationRadians(xyzRadians.X, xyzRadians.Y, xyzRadians.Z);

        var finSkin = finModel.Skin;
        var gloVertices = gloMesh.Vertices;

        var gloFaces = gloMesh.Faces;
        var finTriangles = new (IVertex, IVertex, IVertex)[gloFaces.Length];
        for (var i = 0; i < gloFaces.Length; ++i) {
          var gloFace = gloFaces[i];

          var gloFaceVertices = new IVertex[3];
          for (var v = 0; v < 3; ++v) {
            var gloVertexRef = gloFace.VertexRefs[v];
            var gloVertex = gloVertices[gloVertexRef.Index];

            gloFaceVertices[v] = finSkin.AddVertex(gloVertex.X, gloVertex.Y, gloVertex.Z).SetUv(gloVertexRef.U, gloVertexRef.V);
          }

          finTriangles[i] = (gloFaceVertices[0], gloFaceVertices[1], gloFaceVertices[2]);
        }

        var finMesh = finSkin.AddMesh();
        finMesh.AddTriangles(finTriangles);

        var child = gloMesh.Pointers.Child;
        if (child != null) {
          meshQueue.Enqueue((child, finBone));
        }

        var next = gloMesh.Pointers.Next;
        if (next != null) {
          meshQueue.Enqueue((next, parentFinBone));
        }
      }

      return finModel;
    }
  }
}