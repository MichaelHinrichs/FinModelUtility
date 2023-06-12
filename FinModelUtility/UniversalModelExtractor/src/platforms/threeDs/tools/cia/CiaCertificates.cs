using schema.binary;
using schema.binary.attributes.child_of;

namespace uni.platforms.threeDs.tools.cia {
  public partial class CiaCertificates : IChildOf<Cia>, IBinaryDeserializable {
    public Cia Parent { get; set; }

    public void Read(IEndianBinaryReader er) {
      er.Position += this.Parent.Header.CertificateSize;
    }
  }
}
