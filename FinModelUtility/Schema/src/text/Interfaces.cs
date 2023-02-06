using System.IO;

namespace schema.text {
  public interface ITextDeserializable {
    void Read(ITextReader er);
  }

  public interface ITextConvertible : ITextDeserializable { }
}