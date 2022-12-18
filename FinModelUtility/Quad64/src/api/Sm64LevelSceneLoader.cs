using fin.data;
using fin.image;
using fin.io;
using fin.model;
using fin.model.impl;
using fin.scene;
using OpenTK.Graphics.OpenGL;
using Quad64;
using Quad64.Scripts;
using Quad64.src.JSON;
using Quad64.src.LevelInfo;
using Quad64.src.Scripts;


namespace sm64.api {
  public class Sm64LevelSceneFileBundle : ISceneFileBundle {
    public Sm64LevelSceneFileBundle(
        IFile sm64Rom,
        LevelId levelId) {
      this.Sm64Rom = sm64Rom;
      this.LevelId = levelId;
    }

    public IFileHierarchyFile MainFile => null!;

    public IFile Sm64Rom { get; }
    public LevelId LevelId { get; }
    string IUiFile.FileName => $"{LevelId}";
  }

  public class Sm64LevelSceneLoader : ISceneLoader<Sm64LevelSceneFileBundle> {
    // TODO: Load this as a scene instead

    public IScene LoadScene(Sm64LevelSceneFileBundle levelModelFileBundle) {
      var sm64Level = Sm64LevelSceneLoader.LoadLevel_(levelModelFileBundle);

      var finScene = new SceneImpl();

      var lazyModelDictionary = new LazyDictionary<ushort, IModel?>(
          sm64ModelId => {
            if (sm64Level.ModelIDs.TryGetValue(sm64ModelId,
                                               out var sm64Model)) {
              return CreateModel_(sm64Model.HighestLod);
            }
            return null;
          });

      foreach (var sm64Area in sm64Level.Areas) {
        Sm64LevelSceneLoader.AddAreaToScene_(
            finScene,
            lazyModelDictionary,
            sm64Area);
      }

      return finScene;
    }

    private static Level LoadLevel_(
        Sm64LevelSceneFileBundle levelSceneFileBundle) {
      ROM rom = ROM.Instance;

      rom.readFile(levelSceneFileBundle.Sm64Rom.FullName);

      Globals.objectComboEntries.Clear();
      Globals.behaviorNameEntries.Clear();
      BehaviorNameFile.parseBehaviorNames(
          Globals.getDefaultBehaviorNamesPath());
      ModelComboFile.parseObjectCombos(Globals.getDefaultObjectComboPath());
      rom.setSegment(0x15, Globals.seg15_location[0], Globals.seg15_location[1],
                     false, null);
      rom.setSegment(0x02, Globals.seg02_location[0], Globals.seg02_location[1],
                     rom.isSegmentMIO0(0x02, null), rom.Seg02_isFakeMIO0,
                     rom.Seg02_uncompressedOffset, null);

      var level = new Level((ushort)levelSceneFileBundle.LevelId, 1);
      LevelScripts.parse(ref level, 0x15, 0);
      level.sortAndAddNoModelEntries();
      level.CurrentAreaID = level.Areas[0].AreaID;

      return level;
    }

    private static void AddAreaToScene_(
        IScene finScene,
        LazyDictionary<ushort, IModel?> lazyModelDictionary,
        Area sm64Area) {
      var finArea = finScene.AddArea();
      AddAreaModelToScene_(finArea, sm64Area);

      var objects =
          sm64Area.Objects.Concat(sm64Area.MacroObjects)
                  .Concat(sm64Area.SpecialObjects)
                  .ToArray();

      foreach (var obj in objects) {
        AddAreaObjectToScene_(finArea, lazyModelDictionary, obj);
      }
    }

    private static void AddAreaModelToScene_(ISceneArea finArea, Area sm64Area)
      => finArea.AddObject()
                .AddSceneModel(CreateModel_(sm64Area.AreaModel.HighestLod));

    private static void AddAreaObjectToScene_(
        ISceneArea finArea,
        LazyDictionary<ushort, IModel?> lazyModelDictionary,
        Object3D sm64Object) {
      var finModel = lazyModelDictionary[sm64Object.ModelID];
      if (finModel != null) {
        var finObject = finArea.AddObject();
        finObject.AddSceneModel(finModel);
        finObject.SetPosition(sm64Object.xPos, sm64Object.yPos, sm64Object.zPos);
      }
    }

    private static IModel CreateModel_(Model3D sm64Model) {
      var finModel = new ModelImpl();

      var lazyTextureDictionary = new LazyDictionary<Texture2D, ITexture>(
          sm64Texture => {
            var finTexture = finModel.MaterialManager.CreateTexture(
                FinImage.FromBitmap(sm64Texture.Bmp));

            finTexture.WrapModeU =
                ConvertFromGlWrap_((TextureWrapMode)sm64Texture.TextureParamS);
            finTexture.WrapModeV =
                ConvertFromGlWrap_((TextureWrapMode)sm64Texture.TextureParamT);

            return finTexture;
          });
      var lazyMaterialDictionary = new LazyDictionary<Texture2D, IMaterial>(
          sm64Texture =>
              finModel.MaterialManager.AddTextureMaterial(
                  lazyTextureDictionary[sm64Texture]));

      foreach (var sm64Mesh in sm64Model.meshes) {
        var geometryMode = sm64Mesh.Material.GeometryMode;

        var cullFront = geometryMode.HasFlag(RspGeometryMode.G_CULL_FRONT);
        var cullBack = geometryMode.HasFlag(RspGeometryMode.G_CULL_BACK);
        var finCullingMode = cullFront switch {
            false => cullBack switch {
                true  => CullingMode.SHOW_FRONT_ONLY,
                false => CullingMode.SHOW_BOTH,
            },
            true => cullBack switch {
                false => CullingMode.SHOW_BACK_ONLY,
                true  => CullingMode.SHOW_NEITHER,
            },
        };

        var finMaterial = lazyMaterialDictionary[sm64Mesh.texture];

        var indices = sm64Mesh.indices;
        var colors = sm64Mesh.colors;
        var vertices = sm64Mesh.vertices;
        var uvs = sm64Mesh.texCoord;

        var finVertices = new List<IVertex>();
        foreach (var vertexIndex in indices) {
          var uv = uvs[vertexIndex];
          var color = colors[vertexIndex];
          var vertex = vertices[vertexIndex];

          finVertices.Add(
              finModel.Skin.AddVertex(vertex.X, vertex.Y, vertex.Z)
                      .SetUv(uv.X, uv.Y)
                      .SetColorBytes(
                          (byte)(255 * color.X),
                          (byte)(255 * color.Y),
                          (byte)(255 * color.Z),
                          (byte)(255 * color.W)));
        }

        var finMesh = finModel.Skin.AddMesh();
        finMesh.AddTriangles(finVertices.ToArray())
               .SetMaterial(finMaterial)
               .SetVertexOrder(VertexOrder.NORMAL);
      }

      return finModel;
    }

    private static WrapMode ConvertFromGlWrap_(
        TextureWrapMode wrapMode) =>
        wrapMode switch {
            TextureWrapMode.ClampToEdge    => WrapMode.CLAMP,
            TextureWrapMode.Repeat         => WrapMode.REPEAT,
            TextureWrapMode.MirroredRepeat => WrapMode.MIRROR_REPEAT,
            _ => throw new ArgumentOutOfRangeException(
                     nameof(wrapMode), wrapMode, null)
        };
  }
}