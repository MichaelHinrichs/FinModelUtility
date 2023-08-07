using fin.util.strings;

using schema.binary;


namespace modl.schema {
  public static class SectionHeaderUtil {
    public static void ReadName(
        IEndianBinaryReader er,
        out string sectionName) {
      er.PushMemberEndianness(Endianness.LittleEndian);
      sectionName = er.ReadString(4).Reverse();
      er.PopEndianness();
    }

    public static void AssertName(
        IEndianBinaryReader er,
        string expectedSectionName) {
      er.PushMemberEndianness(Endianness.LittleEndian);
      er.AssertString(expectedSectionName.Reverse());
      er.PopEndianness();
    }


    public static void ReadSize(
        IEndianBinaryReader er,
        out uint size) {
      er.PushMemberEndianness(Endianness.LittleEndian);
      size = er.ReadUInt32();
      er.PopEndianness();
    }

    public static void AssertSize(
        IEndianBinaryReader er,
        uint expectedSize) {
      er.PushMemberEndianness(Endianness.LittleEndian);
      er.AssertUInt32(expectedSize);
      er.PopEndianness();
    }


    public static void ReadNameAndSize(
        IEndianBinaryReader er,
        out string sectionName,
        out uint size) {
      SectionHeaderUtil.ReadName(er, out sectionName);
      SectionHeaderUtil.ReadSize(er, out size);
    }

    public static void AssertNameAndReadSize(
        IEndianBinaryReader er,
        string expectedSectionName,
        out uint size) {
      SectionHeaderUtil.AssertName(er, expectedSectionName);
      SectionHeaderUtil.ReadSize(er, out size);
    }

    public static void AssertNameAndSize(
        IEndianBinaryReader er,
        string expectedSectionName,
        uint expectedSize) {
      SectionHeaderUtil.AssertName(er, expectedSectionName);
      SectionHeaderUtil.AssertSize(er, expectedSize);
    }
  }
}