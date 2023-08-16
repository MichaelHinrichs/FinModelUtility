using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;

using fin.util.asserts;

using schema.binary;

namespace fin.io {
  public static class FileUtil {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReplaceInvalidFilenameCharacters(this string path)
      => string.Join("_", path.Split(Path.GetInvalidFileNameChars()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertExists(string path, string? message = null)
      => Asserts.True(FinFileSystem.File.Exists(path), message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadNew<T>(string path)
        where T : IBinaryDeserializable, new() {
      using var er = new EndianBinaryReader(OpenRead(path));
      return er.ReadNew<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadNew<T>(string path, Endianness endianness)
        where T : IBinaryDeserializable, new() {
      using var er = new EndianBinaryReader(OpenRead(path), endianness);
      return er.ReadNew<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadAllBytes(string path)
      => FinFileSystem.File.ReadAllBytes(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadAllText(string path)
      => FinFileSystem.File.ReadAllText(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamReader OpenReadAsText(string path)
      => File.OpenText(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamWriter OpenWriteAsText(string path) => new(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileSystemStream OpenRead(string path)
      => FinFileSystem.File.OpenRead(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileSystemStream OpenWrite(string path)
      => FinFileSystem.File.OpenWrite(path);
  }
}