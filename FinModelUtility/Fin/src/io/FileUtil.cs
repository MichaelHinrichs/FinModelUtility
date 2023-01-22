using System.IO;
using System.IO.Abstractions;

using fin.util.asserts;

using schema;


namespace fin.io {
  public static class FileUtil {
    public static void AssertExists(string path, string? message = null)
      => Asserts.True(FinFileSystem.File.Exists(path), message);

    public static T ReadNew<T>(string path)
        where T : IDeserializable, new() {
      using var er = new EndianBinaryReader(OpenRead(path));
      return er.ReadNew<T>();
    }

    public static T ReadNew<T>(string path, Endianness endianness)
        where T : IDeserializable, new() {
      using var er = new EndianBinaryReader(OpenRead(path), endianness);
      return er.ReadNew<T>();
    }

    public static byte[] ReadAllBytes(string path)
      => FinFileSystem.File.ReadAllBytes(path);

    public static string ReadAllText(string path)
      => FinFileSystem.File.ReadAllText(path);

    public static StreamReader OpenReadAsText(string path) =>
        File.OpenText(path);

    public static StreamWriter OpenWriteAsText(string path) => new(path);

    public static FileSystemStream OpenRead(string path)
      => FinFileSystem.File.OpenRead(path);

    public static FileSystemStream OpenWrite(string path)
      => FinFileSystem.File.OpenWrite(path);
  }
}