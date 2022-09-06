using System.IO;

using fin.util.asserts;

using schema;


namespace fin.io {
  public static class FileUtil {
    public static void AssertExists(string path, string? message = null)
      => Asserts.True(File.Exists(path), message);

    public static T ReadNew<T>(string path, Endianness endianness)
        where T : IDeserializable, new() {
      using var er =
          new EndianBinaryReader(FileUtil.OpenRead(path), endianness);
      return er.ReadNew<T>();
    }

    public static byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
    public static string ReadAllText(string path) => File.ReadAllText(path);

    public static StreamReader OpenReadAsText(string path) =>
        File.OpenText(path);

    public static StreamWriter OpenWriteAsText(string path) => new(path);

    public static FileStream OpenRead(string path) => File.OpenRead(path);
    public static FileStream OpenWrite(string path) => File.OpenWrite(path);
  }
}