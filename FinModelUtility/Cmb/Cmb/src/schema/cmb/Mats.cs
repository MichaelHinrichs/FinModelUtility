using System.Linq;

using schema.binary;
using schema.binary.attributes.ignore;
using schema.binary.attributes.sequence;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class Mats : IBinaryConvertible {
    [SequenceLengthSource(SchemaIntegerType.UINT32)]
    public Material[] Materials { get; set; }

    [Ignore]
    private uint TotalCombinerCount_
      => (uint) this.Materials.Sum(material => material.texEnvStageCount);

    [RSequenceLengthSource(nameof(TotalCombinerCount_))]
    public Combiner[] Combiners;
  }
}
