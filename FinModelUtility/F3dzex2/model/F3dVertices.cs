using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using f3dzex2.displaylist.opcodes;
using f3dzex2.image;

using fin.math;
using fin.model;
using fin.util.enums;


namespace f3dzex2.model {
  public interface IF3dVertices {
    void ClearVertices();

    void LoadVertices(IReadOnlyList<F3dVertex> vertices, int startIndex);
    F3dVertex GetVertexDefinition(int index);
    IVertex GetOrCreateVertexAtIndex(byte index);

    Color DiffuseColor { get; set; }
  }

  public class F3dVertices : IF3dVertices {
    private readonly IN64Hardware n64Hardware_;
    private readonly IModel model_;

    private const int VERTEX_COUNT = 32;

    private readonly F3dVertex[] vertexDefinitions_ =
        new F3dVertex[VERTEX_COUNT];

    private readonly IVertex?[] vertices_ = new IVertex?[VERTEX_COUNT];
    private Color diffuseColor_;


    public F3dVertices(IN64Hardware n64Hardware, IModel model) {
      this.n64Hardware_ = n64Hardware;
      this.model_ = model;
    }

    public void ClearVertices() => Array.Fill(this.vertices_, null);

    public void LoadVertices(IReadOnlyList<F3dVertex> newVertices,
                             int startIndex) {
      for (var i = 0; i < newVertices.Count; ++i) {
        var index = startIndex + i;
        this.vertexDefinitions_[index] = newVertices[i];
        this.vertices_[index] = null;
      }

    }


    public F3dVertex GetVertexDefinition(int index)
      => this.vertexDefinitions_[index];

    public IVertex GetOrCreateVertexAtIndex(byte index) {
      var existing = this.vertices_[index];
      if (existing != null) {
        return existing;
      }

      var definition = this.vertexDefinitions_[index];

      var position = definition.GetPosition();
      GlMatrixUtil.ProjectPosition(this.n64Hardware_.Rsp.Matrix.Impl,
                                   ref position);

      var textureParams = this.n64Hardware_.Rdp.Tmem.GetMaterialParams()
                    .TextureParams0;
      var bmpWidth = Math.Max(textureParams.Width, (ushort) 0);
      var bmpHeight = Math.Max(textureParams.Height, (ushort) 0);

      var newVertex = this.model_.Skin.AddVertex(position)
                          .SetUv(definition.GetUv(
                                     this.n64Hardware_.Rsp.TexScaleXFloat /
                                     (bmpWidth * 32),
                                     this.n64Hardware_.Rsp.TexScaleYFloat /
                                     (bmpHeight * 32)));

      if (this.n64Hardware_.Rsp.GeometryMode.CheckFlag(
              GeometryMode.G_LIGHTING)) {
        var normal = definition.GetNormal();
        GlMatrixUtil.ProjectNormal(this.n64Hardware_.Rsp.Matrix.Impl,
                                   ref normal);
        newVertex.SetLocalNormal(normal)
                 // TODO: Get rid of this, seems to come from combiner instead
                 .SetColor(this.DiffuseColor);
      } else {
        newVertex.SetColor(definition.GetColor());
      }

      this.vertices_[index] = newVertex;
      return newVertex;
    }

    public Color DiffuseColor {
      get => this.diffuseColor_;
      set => this.diffuseColor_ = value;
    }
  }
}