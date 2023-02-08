using System.Collections.Generic;
using System.IO;
using System.Linq;

using schema.binary;

namespace cmb.schema.csab {
  public class AnimationNode : IBinaryDeserializable {
    private readonly Csab parent_;

    public AnimationNode(Csab parent) {
      this.parent_ = parent;

      this.TranslationAxes = Enumerable.Range(0, 3)
                                       .Select(_ => new CsabTrack(parent) {
                                           ValueType = TrackType.POSITION
                                       })
                                       .ToArray();
      this.RotationAxes = Enumerable.Range(0, 3)
                                    .Select(_ => new CsabTrack(parent) {
                                        ValueType = TrackType.ROTATION
                                    })
                                    .ToArray();
      this.ScaleAxes = Enumerable.Range(0, 3)
                                 .Select(_ => new CsabTrack(parent) {
                                     ValueType = TrackType.SCALE
                                 })
                                 .ToArray();
    }

    public ushort BoneIndex { get; set; }

    public IReadOnlyList<CsabTrack> TranslationAxes { get; }
    public IReadOnlyList<CsabTrack> RotationAxes { get; }
    public IReadOnlyList<CsabTrack> ScaleAxes { get; }

    public bool IsPastVersion4 => this.parent_.IsPastVersion4;

    public void Read(IEndianBinaryReader r) {
      var basePosition = r.Position;

      r.AssertMagicText("anod");

      this.BoneIndex = r.ReadUInt16();

      var isRotationShort = r.ReadUInt16() != 0;
      
      foreach (var translationAxis in TranslationAxes) {
        var offset = r.ReadUInt16();
        if (offset != 0) {
          r.Subread(basePosition + offset, translationAxis.Read);
        }
      }

      foreach (var rotationAxis in RotationAxes) {
        rotationAxis.AreRotationsShort = isRotationShort;

        var offset = r.ReadUInt16();
        if (offset != 0) {
          r.Subread(basePosition + offset, rotationAxis.Read);
        }
      }

      foreach (var scaleAxis in ScaleAxes) {
        var offset = r.ReadUInt16();
        if (offset != 0) {
          r.Subread(basePosition + offset, scaleAxis.Read);
        }
      }

      r.AssertUInt16(0x00);
    }
  }
}