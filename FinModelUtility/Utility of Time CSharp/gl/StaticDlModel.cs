using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using Tao.OpenGl;

using UoT.animation.playback;
using UoT.displaylist;
using UoT.limbs;
using UoT.util;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;

using AlphaMode = SharpGLTF.Materials.AlphaMode;

namespace UoT {
  using VERTEX =
      VertexBuilder<VertexPositionNormal, VertexColor1Texture2, VertexJoints4>;

  public interface IDlModel {
    IList<IOldLimb> Limbs { get; }
  }

  public class Vec3d : IVertex {
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public short U { get; set; }
    public short V { get; set; }

    public float NormalX { get; set; }
    public float NormalY { get; set; }
    public float NormalZ { get; set; }

    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
  }


  public class StaticDlModel : IDlModel {
    // TODO: Prune unused values.
    // TODO: Split out vertices into separately indexed position/uv/color arrays.

    // TODO: How to separate limbs?
    // TODO: Support submeshes (e.g. held items).
    // TODO: Simplify some triangles to quads.
    // TODO: Separate common shader params as different materials.

    // TODO: Add remainder of shader params for blend modes/etc.

    // TODO: Some models have gaps again, e.g. Bazaar Shopkeeper.

    private readonly DlShaderManager shaderManager_;
    private bool isComplete_;

    public StaticDlModel(DlShaderManager shaderManager) {
      this.shaderManager_ = shaderManager;
    }


    public bool IsComplete {
      get => this.isComplete_;
      set {
        this.isComplete_ = value;

        this.projectedVertices_.Clear();
        if (value) {
          for (var i = 0; i < this.allVertices_.Count; ++i) {
            this.projectedVertices_.Add(new Vec3d());
          }
        }
      }
    }

    private LimbInstance? activeLimb_;
    private readonly int[] activeVertices_ = new int[32];

    // TODO: Needed for models w/o limbs.
    private LimbInstance? root_;

    private readonly IList<VertexParams>
        allVertices_ = new List<VertexParams>();

    private readonly IList<Vec3d>
        projectedVertices_ = new List<Vec3d>();

    private readonly IList<Texture> allTextures_ = new List<Texture>();

    private readonly IList<LimbInstance> allLimbs_ = new List<LimbInstance>();
    public IList<IOldLimb> Limbs { get; } = new List<IOldLimb>();

    public void Reset() {
      this.IsComplete = false;

      for (var i = 0; i < this.activeVertices_.Length; ++i) {
        this.activeVertices_[i] = -1;
      }

      this.root_ = new LimbInstance {
          firstChild = -1,
          nextSibling = -1
      };
      this.activeLimb_ = this.root_;

      // TODO: Clear vertices and textures.

      foreach (var limb in this.allLimbs_) {
        limb.Triangles.Clear();
        limb.OwnedVertices.Clear();
      }

      this.allLimbs_.Clear();
      this.Limbs.Clear();

      this.allVertices_.Clear();
      this.projectedVertices_.Clear();
      this.allTextures_.Clear();
    }

    public LimbMatrices LimbMatrices { get; } = new LimbMatrices();

    // TODO: Support facial animations.
    public void Draw(
        IAnimation animation,
        IAnimationPlaybackManager animationPlaybackManager) {
      ModelViewMatrixTransformer.Push();

      if (animation != null) {
        var frame = animationPlaybackManager.Frame;
        var totalFrames = animationPlaybackManager.TotalFrames;

        var frameIndex = (int) Math.Floor(frame);
        var frameDelta = frame % 1;

        var startPos = animation.GetPosition(frameIndex);
        var endPos = animation.GetPosition((frameIndex + 1) % totalFrames);

        var f = frameDelta;

        var x = startPos.X * (1 - f) + endPos.X * f;
        var y = startPos.Y * (1 - f) + endPos.Y * f;
        var z = startPos.Z * (1 - f) + endPos.Z * f;

        ModelViewMatrixTransformer.Translate(x, y, z);

        /*If indirectTextureHack IsNot Nothing Then
            Dim face As FacialState = CurrAnimation.GetFacialState(frameIndex)
        indirectTextureHack.EyeState = face.EyeState
        indirectTextureHack.MouthState = face.MouthState
        End If*/
      }

      var hasLimbs = (this.allLimbs_?.Count ?? 0) > 0;

      void ProjectVertices(IEnumerable<int> vertexIndices) {
        foreach (var vertexIndex in vertexIndices) {
          var ownedVertex = this.allVertices_[vertexIndex];
          var projectedVertex = this.projectedVertices_[vertexIndex];

          var x = ownedVertex.X;
          var y = ownedVertex.Y;
          var z = ownedVertex.Z;
          ModelViewMatrixTransformer.ProjectVertex(ref x, ref y, ref z);
          projectedVertex.X = x;
          projectedVertex.Y = y;
          projectedVertex.Z = z;

          projectedVertex.U = ownedVertex.U;
          projectedVertex.V = ownedVertex.V;

          double normalX = ownedVertex.NormalX;
          double normalY = ownedVertex.NormalY;
          double normalZ = ownedVertex.NormalZ;
          ModelViewMatrixTransformer.ProjectNormal(ref normalX,
                                                   ref normalY,
                                                   ref normalZ);
          projectedVertex.NormalX = (float) normalX;
          projectedVertex.NormalY = (float) normalY;
          projectedVertex.NormalZ = (float) normalZ;

          projectedVertex.R = ownedVertex.R;
          projectedVertex.G = ownedVertex.G;
          projectedVertex.B = ownedVertex.B;
          projectedVertex.A = ownedVertex.A;
        }
      }

      if (!hasLimbs) {
        var allVertexIndices = new int[this.allVertices_.Count];
        for (var i = 0; i < allVertexIndices.Length; ++i) {
          allVertexIndices[i] = i;
        }

        ProjectVertices(allVertexIndices);
      } else {
        this.LimbMatrices.UpdateLimbMatrices(this.Limbs,
                                             animation,
                                             (float) animationPlaybackManager
                                                 .Frame);

        this.ForEachLimbRecursively_(
            0,
            (limb, limbIndex) => {
              if (false) {
                var xI = 0.0;
                var yI = 0.0;
                var zI = 0.0;
                ModelViewMatrixTransformer.ProjectVertex(
                    ref xI,
                    ref yI,
                    ref zI);

                double xF = limb.x;
                double yF = limb.y;
                double zF = limb.z;
                ModelViewMatrixTransformer.ProjectVertex(
                    ref xF,
                    ref yF,
                    ref zF);

                Gl.glDepthRange(0, 0);
                Gl.glLineWidth(9);
                Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3f(1, 1, 1);
                Gl.glVertex3d(xI, yI, zI);
                Gl.glVertex3d(xF, yF, zF);
                Gl.glEnd();
                Gl.glDepthRange(0, -0.5);
                Gl.glPointSize(11);
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glColor3f(0, 0, 0);
                Gl.glVertex3d(xF, yF, zF);
                Gl.glEnd();
                /*Gl.glPointSize(8);
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glColor3ub(BoneColorFactor.r,
                              BoneColorFactor.g,
                              BoneColorFactor.b);
                Gl.glVertex3f(xF, yF, zF);
                Gl.glEnd();*/
                Gl.glPointSize(1);
                Gl.glLineWidth(1);
                Gl.glDepthRange(0, 1);
              }

              ModelViewMatrixTransformer.Push();

              var matrix = this.LimbMatrices.GetMatrixForLimb((uint) limbIndex);
              ModelViewMatrixTransformer.Set(matrix);

              ProjectVertices(limb.OwnedVertices);
            },
            (limb, _) => ModelViewMatrixTransformer.Pop());
      }
      ModelViewMatrixTransformer.Pop();

      void RenderTriangles(IList<TriangleParams> triangles) {
        foreach (var triangle in triangles) {
          this.shaderManager_.Params = triangle.ShaderParams;
          this.shaderManager_.PassValuesToShader();

          var textureIds = triangle.TextureIds;
          var texture0Id = textureIds[0];
          var texture1Id = textureIds[1];

          var texture0 = texture0Id > -1
                             ? this.allTextures_[texture0Id]
                             : null;
          var texture1 = texture1Id > -1
                             ? this.allTextures_[texture1Id]
                             : null;
          this.shaderManager_.BindTextures(texture0, texture1);

          Gl.glColor3b(255, 255, 255);
          Gl.glBegin(Gl.GL_TRIANGLES);

          var tileDescriptor0 = texture0?.TileDescriptor;
          var tileDescriptor1 = texture1?.TileDescriptor;

          foreach (var vertexId in triangle.Vertices) {
            var projectedVertex = this.projectedVertices_[vertexId];

            this.shaderManager_.BindTextureUvs(projectedVertex,
                                               tileDescriptor0,
                                               tileDescriptor1);
            this.shaderManager_.PassInVertexAttribs(projectedVertex);
            Gl.glVertex3d(projectedVertex.X,
                          projectedVertex.Y,
                          projectedVertex.Z);
          }

          Gl.glEnd();
        }
        Gl.glFinish();
      }

      if (!hasLimbs) {
        RenderTriangles(this.root_!.Triangles);
      } else {
        this.ForEachLimbRecursively_(
            0,
            (limb, _) => RenderTriangles(limb.Triangles),
            null);
      }
    }

    private void ForEachLimbRecursively_(
        sbyte limbIndex,
        Action<LimbInstance, sbyte>? beforeChildren,
        Action<LimbInstance, sbyte>? afterChildren) {
      var limb = this.allLimbs_[limbIndex];
      beforeChildren?.Invoke(limb, limbIndex);

      var firstChildIndex = limb.firstChild;
      if (firstChildIndex > -1) {
        this.ForEachLimbRecursively_(firstChildIndex,
                                     beforeChildren,
                                     afterChildren);
      }

      afterChildren?.Invoke(limb, limbIndex);

      var nextSiblingIndex = limb.nextSibling;
      if (nextSiblingIndex > -1) {
        this.ForEachLimbRecursively_(nextSiblingIndex,
                                     beforeChildren,
                                     afterChildren);
      }
    }

    public void AddLimb(
        bool visible,
        short x,
        short y,
        short z,
        sbyte firstChild,
        sbyte nextSibling) {
      if (this.IsComplete) {
        return;
      }

      var newLimb = new LimbInstance {
          Visible = visible,
          x = x,
          y = y,
          z = z,
          firstChild = firstChild,
          nextSibling = nextSibling
      };
      this.allLimbs_.Add(newLimb);
      this.Limbs.Add(newLimb);
    }

    public void SetCurrentLimbByMatrixAddress(uint matrixAddress)
      => this.SetCurrentLimbByVisibleLimbIndex(
          LimbMatrices.ConvertAddressToVisibleLimbIndex(matrixAddress));

    public void SetCurrentLimbByVisibleLimbIndex(uint visibleLimbIndex) {
      if (this.IsComplete) {
        return;
      }

      var limbIndex = -1;
      for (var i = 0; i < this.allLimbs_.Count; ++i) {
        var limb = this.allLimbs_[i];
        if (limb.VisibleIndex == visibleLimbIndex) {
          limbIndex = i;
          break;
        }
      }
      Asserts.Assert(limbIndex > -1);

      this.SetCurrentLimbByIndex(limbIndex);
    }

    public void SetCurrentLimbByIndex(int limbIndex) {
      if (this.IsComplete) {
        return;
      }
      this.activeLimb_ = this.allLimbs_[limbIndex];
    }

    public void AddTriangle(
        DlShaderParams shaderParams,
        int vertex1,
        int vertex2,
        int vertex3,
        Texture? texture0,
        Texture? texture1) {
      if (this.IsComplete) {
        return;
      }

      var textureIds = new[] {
          this.AddTexture_(texture0),
          this.AddTexture_(texture1),
      };
      var vertices = new int[3];
      vertices[0] = this.activeVertices_[vertex1];
      vertices[1] = this.activeVertices_[vertex2];
      vertices[2] = this.activeVertices_[vertex3];

// TODO: Merge existing shader params.
      var triangle =
          new TriangleParams(shaderParams.Clone(), textureIds, vertices);

      Asserts.Assert(this.activeLimb_).Triangles.Add(triangle);
    }

    public void UpdateVertex(
        int index,
        Func<VertexParams, VertexParams> modifier) {
      if (this.IsComplete) {
        return;
      }
      VertexParams vertex;

      var oldUuid = this.activeVertices_[index];
      if (oldUuid == -1) {
        vertex = new VertexParams();
      } else {
        vertex = this.allVertices_[oldUuid];
      }
      vertex = modifier(vertex);
      vertex.Uuid = this.allVertices_.Count;
      Asserts.Assert(this.activeLimb_).OwnedVertices.Add(vertex.Uuid);
      this.activeVertices_[index] = vertex.Uuid;
      this.allVertices_.Add(vertex);
    }

    private int AddTexture_(Texture? texture) {
      if (this.IsComplete) {
        Asserts.Fail("Should not try to create a new texture when complete!");
      }

      if (texture == null) {
        return -1;
      }

      for (var i = 0; i < this.allTextures_.Count; ++i) {
        if (this.allTextures_[i].TileDescriptor.Uuid ==
            texture.TileDescriptor.Uuid) {
          return i;
        }
      }

      this.allTextures_.Add(texture);
      return this.allTextures_.Count - 1;
    }

    public static readonly Vector3 GLOSSY_SPECULAR = new Vector3(.1f);
    public static readonly float GLOSSY_GLOSSINESS = .5f;

    public class MaterialPair {
      public MaterialPair(MaterialBuilder materialBuilder) {
        this.Lit = materialBuilder
                   .Clone()
                   .WithSpecularGlossinessShader()
                   .WithSpecularGlossiness(new Vector3(0), 0);
        this.Unlit = materialBuilder.Clone().WithUnlitShader();
        this.Glossy = materialBuilder
                      .WithSpecularGlossinessShader()
                      .WithSpecularGlossiness(StaticDlModel.GLOSSY_SPECULAR,
                                              StaticDlModel.GLOSSY_GLOSSINESS);
      }

      public MaterialPair(
          MaterialBuilder lit,
          MaterialBuilder unlit,
          MaterialBuilder glossy) {
        this.Lit = lit;
        this.Unlit = unlit;
        this.Glossy = glossy;
      }

      /// <summary>
      ///   Lit version of the material. Used for standard surfaces.
      /// </summary>
      public MaterialBuilder Lit { get; }

      /// <summary>
      ///   "Unlit" version of the material. Used for surfaces that shouldn't
      ///   be affected by shadow, e.g. glowing enemy eyes.
      /// </summary>
      public MaterialBuilder Unlit { get; }

      /// <summary>
      ///   Glossy version of the material. Used for shiny surfaces, like
      ///   swords and many items when held over Link's head.
      /// </summary>
      public MaterialBuilder Glossy { get; }
    }

    private TextureWrapMode GetWrapMode_(
        bool mirrored,
        bool clamped) {
      if (mirrored) {
        return TextureWrapMode.MIRRORED_REPEAT;
      }

      if (clamped) {
        return TextureWrapMode.CLAMP_TO_EDGE;
      }

      return TextureWrapMode.REPEAT;
    }

    // TODO: Pull this out.
    public void SaveAsGlTf(
        string objectName,
        IList<IAnimation>? animations) {
      // TODO: Use shader.

      var basePath = $"R:/Noesis/Model/{objectName}";
      Directory.CreateDirectory(basePath);

      var path = $"{basePath}/model.glb";

      // Options
      var includeAnimations = true;

      var model = ModelRoot.CreateModel();

      var scene = model.UseScene("default");

      var skin = model.CreateSkin();

      var jointNodes = new Node[1 + this.allLimbs_.Count];

      var rootNode = scene.CreateNode();
      jointNodes[0] = rootNode;

      // TODO: Use buffers for shader stuff?
      // TODO: Eliminate redundant definitions.
      // TODO: Include face animations, somehow?
      // TODO: Fix large filesize for Link, seems to be animations?
      // TODO: Tweak shininess.
      // TODO: Fix limb matrices for some characters, like Bazaar Shopkeeper?
      // TODO: Environment color isn't used yet, giving weird colors for link.

      var scale = objectName.StartsWith("object_gi_") ? .1 : .001;

      var hasLimbs = this.allLimbs_.Count > 0;
      var hasAnimations = (animations?.Count ?? 0) > 0;

      // Gathers up limbs and their nodes.
      var firstAnimation = hasAnimations ? animations![0] : null;

      var limbsAndNodes = new (LimbInstance, Node)[this.allLimbs_.Count];
      if (hasLimbs) {
        var limbQueue = new Queue<(sbyte, Node)>();
        limbQueue.Enqueue((0, rootNode));
        while (limbQueue.Count > 0) {
          var (limbIndex, parentNode) = limbQueue.Dequeue();

          var limb = this.allLimbs_[limbIndex];

          var position = new Vector3((float) (limb.x * scale),
                                     (float) (limb.y * scale),
                                     (float) (limb.z * scale));
          var node = parentNode.CreateNode()
                               .WithLocalTranslation(position);

          if (firstAnimation != null) {
            var rotation =
                this.LimbMatrices.GetLimbRotationAtFrame(limbIndex,
                                                         firstAnimation,
                                                         0);
            node.WithLocalRotation(rotation);
          }

          jointNodes[1 + limbIndex] = node;
          limbsAndNodes[limbIndex] = (limb, node);

          // Enqueues children and siblings.
          var firstChildIndex = limb.firstChild;
          if (firstChildIndex > -1) {
            limbQueue.Enqueue((firstChildIndex, node));
          }

          var nextSiblingIndex = limb.nextSibling;
          if (nextSiblingIndex > -1) {
            limbQueue.Enqueue((nextSiblingIndex, parentNode));
          }
        }
      }
      skin.BindJoints(jointNodes.ToArray());

      // Gathers up texture materials.
      var materials = new MaterialPair[1 + this.allTextures_.Count];
      materials[0] =
          new MaterialPair(new MaterialBuilder("null").WithDoubleSide(true));
      for (var i = 0; i < this.allTextures_.Count; ++i) {
        var glTexture = this.allTextures_[i];

        var texturePath = $"{basePath}/{glTexture.TileDescriptor.Uuid}.png";
        //glTexture.SaveToFile(texturePath);

        var glTfImage = new MemoryImage(texturePath);
        // TODO: Need to handle wrapping in the shader?
        var wrapModeS =
            this.GetWrapMode_(glTexture.GlMirroredS, glTexture.GlClampedS);
        var wrapModeT =
            this.GetWrapMode_(glTexture.GlMirroredT, glTexture.GlClampedT);

        // TODO: Alpha isn't always needed.
        // TODO: Double-sided isn't always needed.
        var material = new MaterialBuilder($"material{i}")
                       .WithAlpha(AlphaMode.MASK)
                       .WithDoubleSide(true);

        // TODO: Don't always need to create all 3.
        var lit = material;
        var unlit = material.Clone();
        var glossy = material.Clone();

        lit.WithSpecularGlossinessShader()
           .WithSpecularGlossiness(new Vector3(0), 0)
           .UseChannel(KnownChannel.Diffuse)
           .UseTexture()
           .WithPrimaryImage(glTfImage)
           .WithSampler(wrapModeS, wrapModeT);

        unlit.WithUnlitShader()
             .UseChannel(KnownChannel.BaseColor)
             .UseTexture()
             .WithPrimaryImage(glTfImage)
             .WithSampler(wrapModeS, wrapModeT);

        // TODO: Use metal instead?
        // TODO: The texture is actually used as the reflection, so this is
        // wrong.
        glossy.WithSpecularGlossinessShader()
              .WithSpecularGlossiness(StaticDlModel.GLOSSY_SPECULAR,
                                      StaticDlModel.GLOSSY_GLOSSINESS)
              .UseChannel(KnownChannel.Diffuse)
              .UseTexture()
              .WithPrimaryImage(glTfImage)
              .WithSampler(wrapModeS, wrapModeT);

        materials[1 + i] = new MaterialPair(lit, unlit, glossy);
      }

      // Gathers up animations.
      if (includeAnimations) {
        for (var a = 0; a < (animations?.Count ?? 0); ++a) {
          var animation = animations![a];
          var animationName = $"animation{a}";

          var glTfAnimation = model.UseAnimation(animationName);

          // Writes translation(s) for the root node.
          var multiplePositions = animation.PositionCount > 1;
          var translationKeyframes = new Dictionary<float, Vector3>();
          for (var f = 0; f < animation.PositionCount; ++f) {
            var translation = animation.GetPosition(f);

            var time = f / 20f;
            translationKeyframes[time] = new Vector3(
                (float) (translation.X * scale),
                (float) (translation.Y * scale),
                (float) (translation.Z * scale));
          }
          glTfAnimation.CreateTranslationChannel(
              rootNode,
              translationKeyframes,
              multiplePositions);

          // Writes rotations for each bone.
          for (var l = 0; l < limbsAndNodes.Length; ++l) {
            var (_, node) = limbsAndNodes[l];

            var multiFrameTracks = 0;
            for (var t = 0; t < 3; ++t) {
              var track = animation.GetTrack(3 * l + t);
              if (track.Type == 1) {
                ++multiFrameTracks;
              }
            }
            var isMultiFrame = multiFrameTracks > 0;
            var trackFrameCount = isMultiFrame ? animation.FrameCount : 1;

            // TODO: Simplify for constant values, results in big files.
            var rotationKeyframes = new Dictionary<float, Quaternion>();
            for (var f = 0; f < trackFrameCount; ++f) {
              var time = f / 20f;
              rotationKeyframes[time] =
                  this.LimbMatrices.GetLimbRotationAtFrame(l, animation, f);
            }

            glTfAnimation.CreateRotationChannel(
                node,
                rotationKeyframes,
                isMultiFrame);
          }
        }
      }

      // Gathers up vertex builders.
      ModelViewMatrixTransformer.Push();
      ModelViewMatrixTransformer.Identity();

      var vertexBuilders = new VERTEX[this.allVertices_.Count];

      if (!hasLimbs) {
        for (var v = 0; v < this.allVertices_.Count; ++v) {
          var vertex = this.allVertices_[v];
          var position = new Vector3(
              (float) (vertex.X * scale),
              (float) (vertex.Y * scale),
              (float) (vertex.Z * scale));

          vertexBuilders[v] = VERTEX.Create(position).WithSkinning((0, 1));
        }
      } else {
        this.LimbMatrices.UpdateLimbMatrices(this.Limbs,
                                             firstAnimation,
                                             0);

        for (var l = 0; l < limbsAndNodes.Length; ++l) {
          var jointIndex = 1 + l;
          var (limb, _) = limbsAndNodes[l];

          ModelViewMatrixTransformer.Set(
              this.LimbMatrices.GetMatrixForLimb((uint) l));

          foreach (var vertexId in limb.OwnedVertices) {
            var vertex = this.allVertices_[vertexId];

            var x = vertex.X;
            var y = vertex.Y;
            var z = vertex.Z;

            // Model MUST be pre-projected to match the orientation of the rig!
            ModelViewMatrixTransformer.ProjectVertex(ref x, ref y, ref z);

            var position = new Vector3(
                (float) (x * scale),
                (float) (y * scale),
                (float) (z * scale));

            vertexBuilders[vertexId] = VERTEX
                                       .Create(position)
                                       .WithSkinning((jointIndex, 1));
          }
        }
      }

      // Builds mesh.
      var meshBuilder = VERTEX.CreateCompatibleMesh();

      void AddTrianglesToMesh(IList<TriangleParams> triangles) {
        foreach (var triangle in triangles) {
          var shaderParams = triangle.ShaderParams;
          var enableLighting = shaderParams.EnableLighting;
          var withNormal = enableLighting;
          var withPrimColor = withNormal && shaderParams.EnableCombiner;

          // TODO: Should be possible to merge these by texture/shader.

          var texture0MaterialPair = materials[1 + triangle.TextureIds[0]];

          var texture0Material = shaderParams.EnableSphericalUv
                                     ? texture0MaterialPair.Glossy
                                     : enableLighting
                                         ? texture0MaterialPair.Lit
                                         : texture0MaterialPair.Unlit;

          var trianglePrimitive = meshBuilder.UsePrimitive(texture0Material);
          var triangleVertexBuilders = new List<IVertexBuilder>();

          foreach (var vertexId in triangle.Vertices) {
            var vertex = this.allVertices_[vertexId];

            // TODO: How does environment color fit in?
            var color = withNormal
                            ? withPrimColor
                                  ? new Vector4(shaderParams.PrimColor[0],
                                                shaderParams.PrimColor[1],
                                                shaderParams.PrimColor[2],
                                                shaderParams.PrimColor[3])
                                  : new Vector4(1)
                            : new Vector4(vertex.R / 255f,
                                          vertex.G / 255f,
                                          vertex.B / 255f,
                                          vertex.A / 255f);

            var vertexBuilder = vertexBuilders[vertexId];

            var texture0Id = triangle.TextureIds[0];
            var texture0 = texture0Id >= 0
                               ? this.allTextures_[triangle.TextureIds[0]]
                               : null;
            var tileDescriptor0 = texture0?.TileDescriptor;

            var u = (float) (vertex.U * tileDescriptor0?.TextureWRatio ?? 0);
            var v = (float) (vertex.V * tileDescriptor0?.TextureHRatio ?? 0);
            vertexBuilder =
                vertexBuilder.WithMaterial(color, new Vector2(u, v));

            if (withNormal) {
              // TODO: Normals seem broken?
              // TODO: Might need to pre-project?

              double x = vertex.NormalX;
              double y = vertex.NormalY;
              double z = vertex.NormalZ;
              ModelViewMatrixTransformer.ProjectNormal(ref x, ref y, ref z);

              var normal =
                  Vector3.Normalize(
                      new Vector3((float) x, (float) y, (float) z));
              vertexBuilder =
                  vertexBuilder.WithGeometry(vertexBuilder.Position, normal);
            }

            triangleVertexBuilders.Add(vertexBuilder);
          }

          trianglePrimitive.AddTriangle(triangleVertexBuilders[0],
                                        triangleVertexBuilders[1],
                                        triangleVertexBuilders[2]);
        }
      }

      if (!hasLimbs) {
        AddTrianglesToMesh(this.root_!.Triangles);
      } else {
        for (var l = 0; l < limbsAndNodes.Length; ++l) {
          var (limb, _) = limbsAndNodes[l];

          ModelViewMatrixTransformer.Set(
              this.LimbMatrices.GetMatrixForLimb((uint) l));

          AddTrianglesToMesh(limb.Triangles);
        }
      }
      ModelViewMatrixTransformer.Pop();

      var mesh = model.CreateMesh(meshBuilder);

      scene.CreateNode()
           .WithSkinnedMesh(mesh, rootNode.WorldMatrix, jointNodes.ToArray());

      var di = new DirectoryInfo(basePath);
      foreach (FileInfo file in di.GetFiles()) {
        file.Delete();
      }

      var settings = new WriteSettings();
      settings.ImageWriting = ResourceWriteMode.SatelliteFile;

      model.Save(path, settings);
    }
  }

  public class LimbInstance : IOldLimb {
    public IList<TriangleParams> Triangles { get; } =
      new List<TriangleParams>();

    public IList<int> OwnedVertices { get; } = new List<int>();

    public bool Visible { get; set; }
    public int VisibleIndex { get; set; }

    public short x { get; set; }
    public short y { get; set; }
    public short z { get; set; }

    public sbyte firstChild { get; set; }
    public sbyte nextSibling { get; set; }
  }

  public class TriangleParams {
    public TriangleParams(
        DlShaderParams shaderParams,
        int[] textureIds,
        int[] vertices) {
      this.ShaderParams = shaderParams;

      this.TextureIds = textureIds;
      this.Vertices = vertices;
    }

    public DlShaderParams ShaderParams { get; }
    public int[] TextureIds { get; }
    public int[] Vertices { get; }
  }

  public struct VertexParams : IVertex {
    public int Uuid { get; set; }

    public double X;
    public double Y;
    public double Z;

    public short U { get; set; }
    public short V { get; set; }

    public float NormalX { get; set; }
    public float NormalY { get; set; }
    public float NormalZ { get; set; }

    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
  }

  public struct Normal {
    public double X { get; }
    public double Y { get; }
    public double Z { get; }
  }

  public struct Rgba {
    public double R { get; set; }
    public double G { get; set; }
    public double B { get; set; }
    public double A { get; set; }
  }
}