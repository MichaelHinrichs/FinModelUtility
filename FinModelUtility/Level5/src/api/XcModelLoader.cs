using fin.data;
using fin.data.queue;
using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;
using level5.schema;
using System.Numerics;


namespace level5.api {
  public class XcModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.XcFile;

    public IFileHierarchyFile XcFile { get; set; }
  }

  public class XcModelLoader : IModelLoader<XcModelFileBundle> {
    public IModel LoadModel(XcModelFileBundle modelFileBundle) {
      var xcFile = modelFileBundle.XcFile;

      var endianness = Endianness.LittleEndian;
      var xc = xcFile.Impl.ReadNew<Xc>(endianness);

      var resourceFile = new Resource(xc.FilesByExtension[".bin"].Single().Data);

      var model = new ModelImpl();

      var finBoneByIndex = new Dictionary<uint, IBone>();
      var finBoneByName = new Dictionary<string, IBone>();
      {
        var mbnFiles = xc.FilesByExtension[".mbn"];

        var mbnByIndex = new Dictionary<uint, Node<Mbn>>();
        var mbnNodeList = mbnFiles.Select(mbnFile => {
          using var er = new EndianBinaryReader(new MemoryStream(mbnFile.Data), endianness);
          var mbn = er.ReadNew<Mbn>();

          var mbnNode = new Node<Mbn> { Value = mbn };
          mbnByIndex[mbn.Id] = mbnNode;
          return mbnNode;
        }).ToArray();

        foreach (var mbnNode in mbnNodeList) {
          if (mbnByIndex.TryGetValue(mbnNode.Value.ParentId, out var parentNode)) {
            mbnNode.Parent = parentNode;
          }
        }

        var mbnQueue = new FinTuple2Queue<Node<Mbn>, IBone>((mbnNodeList[0], model.Skeleton.Root));
        while (mbnQueue.TryDequeue(out var mbnNode, out var parentBone)) {
          var mbn = mbnNode.Value;

          var position = mbn.Position;
          
          var bone = parentBone.AddChild(position.X, position.Y, position.Z);
          bone.Name = resourceFile.GetResourceName(mbn.Id);

          var mat3 = mbn.RotationMatrix3;
          var matrix = new OpenTK.Matrix3(mat3[0], mat3[1], mat3[2],
            mat3[3], mat3[4], mat3[5],
            mat3[6], mat3[7], mat3[8]);
          var openTkQuaternion = matrix.ExtractRotation();
          var quaternion = new Quaternion(openTkQuaternion.X,
            openTkQuaternion.Y,
            openTkQuaternion.Z,
            openTkQuaternion.W);
          var eulerRadians = QuaternionUtil.ToEulerRadians(quaternion);
          bone.SetLocalRotationRadians(eulerRadians.X, eulerRadians.Y, eulerRadians.Z);

          var scale = mbn.Scale;
          bone.SetLocalScale(scale.X, scale.Y, scale.Z);

          finBoneByIndex[mbn.Id] = bone;
          finBoneByName[bone.Name] = bone;

          mbnQueue.Enqueue(mbnNode.Children.Select(childNode => (childNode, bone)));
        }
      }

      {
        var prmFiles = xc.FilesByExtension[".prm"];
        foreach (var prmFile in prmFiles) {
          var prm = new Prm(prmFile.Data);

          var mesh = model.Skin.AddMesh();
          mesh.Name = prm.Name;

          var finVertices = new List<IVertex>();
          foreach (var prmVertex in prm.Vertices) {
            var position = prmVertex.Pos;
            var finVertex = model.Skin.AddVertex(position.X, position.Y, position.Z);

            var boneWeightList = new List<BoneWeight>();
            for (var b = 0; b < 4; ++b) {
              var boneId = prmVertex.Bones[b];
              var weight = prmVertex.Weights[b];

              var hash = BitConverter.ToUInt32(BitConverter.GetBytes(boneId), 0);
              var boneName = resourceFile.GetResourceName(hash);

              if (finBoneByName.TryGetValue(boneName, out var finBone)) {
                boneWeightList.Add(new BoneWeight(finBone, null, weight));
              }
            }
            var boneWeights = model.Skin.GetOrCreateBoneWeights(PreprojectMode.BONE, boneWeightList.ToArray());
            finVertex.SetBoneWeights(boneWeights);

            finVertices.Add(finVertex);
          }

          var finTriangleVertices = prm.Triangles.Select(vertexIndex => finVertices[(int) vertexIndex]).ToArray();
          mesh.AddTriangles(finTriangleVertices);
        }
      }

      return model;
    }
  }
}