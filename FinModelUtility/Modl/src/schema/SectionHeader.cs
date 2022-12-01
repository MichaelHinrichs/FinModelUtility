using fin.util.strings;


namespace modl.schema {
  public static class SectionHeaderUtil {
    public static void ReadName(
        EndianBinaryReader er,
        out string sectionName) {
      er.PushMemberEndianness(Endianness.LittleEndian);
      sectionName = er.ReadString(4).Reverse();
      er.PopEndianness();
    }

    public static void AssertName(
        EndianBinaryReader er,
        string expectedSectionName) {
      er.PushMemberEndianness(Endianness.LittleEndian);
      er.AssertString(expectedSectionName.Reverse());
      er.PopEndianness();
    }


    public static void ReadSize(
        EndianBinaryReader er,
        out uint size) {
      er.PushMemberEndianness(Endianness.LittleEndian);
      size = er.ReadUInt32();
      er.PopEndianness();
    }

    public static void AssertSize(
        EndianBinaryReader er,
        uint expectedSize) {
      er.PushMemberEndianness(Endianness.LittleEndian);
      er.AssertUInt32(expectedSize);
      er.PopEndianness();
    }


    public static void ReadNameAndSize(
        EndianBinaryReader er,
        out string sectionName,
        out uint size) {
      SectionHeaderUtil.ReadName(er, out sectionName);
      SectionHeaderUtil.ReadSize(er, out size);
    }

    public static void AssertNameAndReadSize(
        EndianBinaryReader er,
        string expectedSectionName,
        out uint size) {
      SectionHeaderUtil.AssertName(er, expectedSectionName);
      SectionHeaderUtil.ReadSize(er, out size);
    }

    public static void AssertNameAndSize(
        EndianBinaryReader er,
        string expectedSectionName,
        uint expectedSize) {
      SectionHeaderUtil.AssertName(er, expectedSectionName);
      SectionHeaderUtil.AssertSize(er, expectedSize);
    }
  }
}