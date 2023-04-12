﻿using Quad64.src.Scripts;

using System.Numerics;


namespace Quad64 {
  public class Model3DLods {
    private List<Model3D> lods_ = new();

    public Model3DLods() {
      this.Lods = this.lods_;
      this.Add(null);
    }

    public IReadOnlyList<Model3D> Lods { get; }

    public Model3D HighestLod
      => this.Lods.OrderBy(lod => lod.getNumberOfTrianglesInModel())
             .Last();

    public Model3D Current => this.Lods.LastOrDefault()!;

    public void Add(GeoScriptNode? node) {
      this.lods_.Add(new(node));
    }

    public void BuildBuffers() {
      foreach (var lod in this.lods_) {
        lod.buildBuffers();
      }
    }
  }

  public class Model3D {
    public GeoScriptNode? Node { get; set; }

    public Model3D(GeoScriptNode? node) {
      this.Node = node;
      this.builder = new ModelBuilder(this);
    }

    public class MeshData {
      public Vector3[] vertices;
      public Vector2[] texCoord;
      public Vector4[] colors;
      public uint[] indices;
      public Texture2D texture;
      public ModelBuilder.ModelBuilderMaterial Material;

      public override string ToString() {
        return "Texture [ID/W/H]: [" + texture.Id + "/" + texture.Width + "/" +
               texture.Height + "]";
      }
    }

    public uint GeoDataSegAddress { get; set; }
    public ModelBuilder builder;
    public List<MeshData> meshes = new List<MeshData>();

    public List<uint> geoDisplayLists = new List<uint>();

    public bool hasGeoDisplayList(uint value) {
      for (int i = 0; i < geoDisplayLists.Count; i++) {
        if (geoDisplayLists[i] == value)
          return true;
      }
      geoDisplayLists.Add(value);
      return false;
    }

    public int getNumberOfTrianglesInModel() {
      return this.meshes.Where(t => t != null && t.indices != null)
                 .Sum(t => (t.indices.Length / 3));
    }

    public void buildBuffers() {
      builder.BuildData(ref meshes);
      //Console.WriteLine("#meshes = " + meshes.Count);
      for (int i = 0; i < meshes.Count; i++) {
        MeshData m = meshes[i];
        m.Material = builder.GetMaterial(i);
        m.vertices = builder.getVertices(i);
        m.texCoord = builder.getTexCoords(i);
        m.colors = builder.getColors(i);
        m.indices = builder.getIndices(i);
      }
    }
  }
}