using Quad64.src.Scripts;

using System.Numerics;

using f3dzex2.image;
using f3dzex2.model;

using Quad64.memory;

using fin.math;

using Quad64.Scripts;

using sm64.scripts.geo;


namespace Quad64 {
  public class Model3DLods {
    private readonly IN64Hardware<ISm64Memory> sm64Hardware_;
    private List<Model3D> lods_ = new();
    private List<DlModelBuilder> lods2_ = new();

    public Model3DLods(IN64Hardware<ISm64Memory> sm64Hardware) {
      this.sm64Hardware_ = sm64Hardware;
      this.AddLod(null);
    }

    public IReadOnlyList<Model3D> Lods => this.lods_;
    public IReadOnlyList<DlModelBuilder> Lods2 => this.lods2_;

    public Model3D HighestLod
      => this.Lods.OrderBy(lod => lod.getNumberOfTrianglesInModel())
             .Last();

    public DlModelBuilder HighestLod2
      => this.Lods2.OrderBy(lod => lod.GetNumberOfTriangles())
             .Last();


    public Model3D Current => this.Lods.LastOrDefault()!;
    public DlModelBuilder Current2 => this.Lods2.LastOrDefault()!;


    public void AddLod(GeoScriptNode? node) {
      this.lods_.Add(new(node));
      this.lods2_.Add(new(this.sm64Hardware_));
    }

    public void AddDl(uint address,
                      int currentDepth = 0) {
      var displayList = new F3dParser().Parse(this.sm64Hardware_.Memory, address);
      this.Current2.Matrix = this.Current.Node?.GetTotalMatrix() ??
                             FinMatrix4x4.IDENTITY;
      this.Current2.AddDl(displayList);
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
      public Vector3?[] normals;
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
        m.normals = builder.getNormals(i);
        m.indices = builder.getIndices(i);
      }
    }
  }
}