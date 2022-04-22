using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;
using glo.schema;
using System.Drawing;
using System.Numerics;

namespace glo.api {
  public class ModelConverter {
    public IModel Convert(
        Glo glo,
        IDirectory outputDirectory,
        IList<IDirectory> textureDirectories,
        float fps) {
      var finModel = new ModelImpl();

      var finRoot = finModel.Skeleton.Root;

      var meshQueue = new Queue<(GloMesh, IBone)>();
      meshQueue.Enqueue((glo.Meshes[0], finRoot));

      var finMaterials = new Dictionary<string, IMaterial>();

      while (meshQueue.Count > 0) {
        var (gloMesh, parentFinBone) = meshQueue.Dequeue();

        var position = gloMesh.MoveKeys[0];

        var rotation = gloMesh.RotateKeys[0];
        var quaternion = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
        var xyzRadians = QuaternionUtil.ToEulerRadians(quaternion);

        // TODO: This does not seem to actually be the bone's position
        var finBone = parentFinBone.AddChild(position.X, position.Y, position.Z).SetLocalRotationRadians(xyzRadians.X, xyzRadians.Y, xyzRadians.Z);

        var finSkin = finModel.Skin;
        var gloVertices = gloMesh.Vertices;

        var finMesh = finSkin.AddMesh();

        var gloFaces = gloMesh.Faces;
        for (var i = 0; i < gloFaces.Length; ++i) {
          var gloFace = gloFaces[i];
          var textureFilename = new string(gloFace.TextureFilename).Replace("\0", "");

          if (!finMaterials.TryGetValue(textureFilename, out var finMaterial)) {
            foreach (var textureDirectory in textureDirectories) {
              try {
                var textureFile = textureDirectory.GetExistingFile(textureFilename);
                if (textureFile != null) {
                  var textureImage = Bitmap.FromFile(textureFile.FullName) as Bitmap;
                  finMaterial = finModel.MaterialManager.AddTextureMaterial(finModel.MaterialManager.CreateTexture(textureImage!));
                  finMaterials[textureFilename] = finMaterial;
                }
              } catch { }
            }

            if (finMaterial == null) {
              ;
            }
          }

          var gloFaceVertices = new IVertex[3];
          for (var v = 0; v < 3; ++v) {
            var gloVertexRef = gloFace.VertexRefs[v];
            var gloVertex = gloVertices[gloVertexRef.Index];

            gloFaceVertices[v] = finSkin.AddVertex(gloVertex.X, gloVertex.Y, gloVertex.Z).SetUv(gloVertexRef.U, gloVertexRef.V);
          }

          // TODO: Merge triangles together
          var finTriangles = new (IVertex, IVertex, IVertex)[1];
          finTriangles[0] = (gloFaceVertices[0], gloFaceVertices[2], gloFaceVertices[1]);
          finMesh.AddTriangles(finTriangles).SetMaterial(finMaterial!);
        }

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