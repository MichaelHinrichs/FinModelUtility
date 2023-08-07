using fin.io.archive;

using schema.binary;


namespace uni.platforms.threeDs.tools.cia {
  public class CiaReader : IArchiveReader<SubArchiveContentFile> {
    public bool IsValidArchive(Stream archive) => true;

    public IArchiveStream<SubArchiveContentFile> Decompress(Stream archive)
      => new SubArchiveStream(archive);

    public IEnumerable<SubArchiveContentFile> GetFiles(
        IArchiveStream<SubArchiveContentFile> archiveStream) {
      var er = archiveStream.AsEndianBinaryReader(Endianness.LittleEndian);
      var cia = er.ReadNew<Cia>();

      yield break;
    }
  }
}