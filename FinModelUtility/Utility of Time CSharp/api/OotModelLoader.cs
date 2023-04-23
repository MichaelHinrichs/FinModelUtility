using System.IO;

using f3dzex2.io;

using fin.data;
using fin.data.queue;
using fin.model;
using fin.model.impl;

using UoT.memory;
using UoT.model;


namespace UoT.api {
  public enum OotSegmentIndex : uint {
    GAMEPLAY_KEEP = 4,
    GAMEPLAY_FIELD_KEEP = 5,
    ZOBJECT = 6,
  }

  public class OotModelLoader : IModelLoader<OotModelFileBundle> {
    public IModel LoadModel(OotModelFileBundle modelFileBundle) {
      var n64Memory = new N64Memory(modelFileBundle.OotRom.ReadAllBytes(),
                                    Endianness.BigEndian);

      var gameplayKeep = Segments.GAMEPLAY_KEEP;
      n64Memory.AddSegment((uint) OotSegmentIndex.GAMEPLAY_KEEP,
                           gameplayKeep.Offset,
                           gameplayKeep.Length);

      var gameplayFieldKeep = Segments.GAMEPLAY_FIELD_KEEP;
      n64Memory.AddSegment((uint) OotSegmentIndex.GAMEPLAY_FIELD_KEEP,
                           gameplayFieldKeep.Offset,
                           gameplayFieldKeep.Length);

      n64Memory.AddSegment((uint) OotSegmentIndex.ZOBJECT,
                           modelFileBundle.Offset,
                           modelFileBundle.Length);

      var finModel = new ModelImpl();

      var ootLimbs =
          new LimbHierarchyReader2().GetHierarchies(
              n64Memory,
              modelFileBundle.FileName is "object_link_boy"
                                          or "object_link_child");
      if (ootLimbs != null) {
        var ootLimbQueue =
            new FinTuple2Queue<IBone, ILimb2>(
                (finModel.Skeleton.Root, ootLimbs[0]));
        while (ootLimbQueue.TryDequeue(out var parentFinBone,
                                       out var ootLimb)) {
          var finBone = parentFinBone.AddChild(ootLimb.X, ootLimb.Y, ootLimb.Z);

          // TODO: Handle DLs
          // TODO: Handle animations

          var firstChildIndex = ootLimb.FirstChildIndex;
          if (firstChildIndex != -1) {
            ootLimbQueue.Enqueue((finBone, ootLimbs[firstChildIndex]));
          }

          var nextSiblingIndex = ootLimb.NextSiblingIndex;
          if (nextSiblingIndex != -1) {
            ootLimbQueue.Enqueue((parentFinBone, ootLimbs[nextSiblingIndex]));
          }
        }
      }

      return finModel;
    }
  }
}