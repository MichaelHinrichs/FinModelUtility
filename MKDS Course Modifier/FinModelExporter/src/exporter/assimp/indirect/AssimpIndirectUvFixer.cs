using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Assimp;

using fin.math;
using fin.model;
using fin.model.impl;
using fin.src.data;

namespace fin.src.exporter.assimp.indirect {
  public class AssimpIndirectUvFixer {
    public void Fix(IModel model, Scene sc) {
      var assMeshes = sc.Meshes;

      // Fix the UVs.
      var finVertices = model.Skin.Vertices;

      foreach (var assMesh in assMeshes) {
        var assUvIndices =
            assMesh.TextureCoordinateChannels[0].Select(uv => uv.X).ToList();
        var assColorIndices =
            assMesh.TextureCoordinateChannels[0].Select(uv => 1 - uv.Y).ToList();

        var assUvs = assMesh.TextureCoordinateChannels;
        for (var t = 0; t < 8; ++t) {
          assUvs[t].Clear();
        }

        var hadUv = new bool[8];
        foreach (var assUvIndexFloat in assUvIndices) {
          var assUvIndex = (int) Math.Round(assUvIndexFloat);

          var finVertex = assUvIndex != -1 ? finVertices[assUvIndex] : null;
          for (var t = 0; t < 8; ++t) {
            var uv = finVertex?.GetUv(t);
            if (uv != null) {
              hadUv[t] = true;
              assUvs[t].Add(new Vector3D(uv.U, 1 - uv.V, 0));
            } else {
              assUvs[t].Add(default);
            }
          }
        }

        var assColors = assMesh.VertexColorChannels[0];
        assColors.Clear();

        foreach (var assColorIndexFloat in assColorIndices) {
          var assColorIndex = (int) Math.Round(assColorIndexFloat);

          var finVertex = assColorIndex != -1 ? finVertices[assColorIndex] : null;
          var finColor = finVertex?.Color;
          if (finColor != null) {
            assColors.Add(new Color4D(finColor.Rf,
                                      finColor.Gf,
                                      finColor.Bf,
                                      finColor.Af));
          } else {
            assColors.Add(new Color4D(1, 1, 1, 1));
          }
        }

        for (var t = 0; t < 8; ++t) {
          if (!hadUv[t]) {
            assUvs[t].Clear();
          }
        }
      }
    }
  }
}