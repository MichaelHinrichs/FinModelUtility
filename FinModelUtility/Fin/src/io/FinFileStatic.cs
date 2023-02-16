using fin.util.json;

using schema.binary;

using System.IO.Abstractions;
using System.IO;
using System.Runtime.CompilerServices;

namespace fin.io {
  public static class FinFileStatic {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Exists(string fullName)
      => FinFileSystem.File.Exists(fullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Delete(string fullName) {
      if (!FinFileStatic.Exists(fullName)) {
        return false;
      }

      FinFileSystem.File.Delete(fullName);
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetExtension(string fullName) {
      int length = fullName.Length;
      for (int i = length; --i >= 0;) {
        char ch = fullName[i];
        if (ch == '.')
          return fullName.Substring(i, length - i);
        if (ch == '\\' || ch == '/' || ch == Path.VolumeSeparatorChar)
          break;
      }

      return string.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetNameWithoutExtension(string name)
      => name.Substring(0,
                        name.Length - FinFileStatic.GetExtension(name).Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadNew<T>(string fullName)
        where T : IBinaryDeserializable, new()
      => FileUtil.ReadNew<T>(fullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadNew<T>(string fullName, Endianness endianness)
        where T : IBinaryDeserializable, new()
      => FileUtil.ReadNew<T>(fullName, endianness);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadAllBytes(string fullName)
      => FinFileSystem.File.ReadAllBytes(fullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadAllText(string fullName)
      => FinFileSystem.File.ReadAllText(fullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAllBytes(string fullName, byte[] bytes)
      => File.WriteAllBytes(fullName, bytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamReader OpenReadAsText(string fullName)
      => FileUtil.OpenReadAsText(fullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamWriter OpenWriteAsText(string fullName)
      => FileUtil.OpenWriteAsText(fullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileSystemStream OpenRead(string fullName)
      => FileUtil.OpenRead(fullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileSystemStream OpenWrite(string fullName)
      => FileUtil.OpenWrite(fullName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Deserialize<T>(string fullName) {
      var text = ReadAllText(fullName);
      return JsonUtil.Deserialize<T>(text);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize<T>(string fullName, T instance) where T : notnull {
      using var writer = OpenWriteAsText(fullName);
      writer.Write(JsonUtil.Serialize(instance));
    }
  }
}