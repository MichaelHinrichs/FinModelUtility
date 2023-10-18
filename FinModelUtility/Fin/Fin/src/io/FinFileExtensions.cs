using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;

using fin.util.asserts;
using fin.util.json;

using schema.binary;
using schema.text;
using schema.text.reader;
using schema.util.strings;

using TextReader = schema.text.reader.TextReader;

namespace fin.io {
  public static class FinFileExtensions {
    // JSON Serialization
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Deserialize<T>(this IReadOnlyGenericFile file)
      => JsonUtil.Deserialize<T>(file.ReadAllText());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize<T>(this IGenericFile file, T instance)
        where T : notnull
      => file.WriteAllText(JsonUtil.Serialize(instance));


    // Read methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamReader OpenReadAsText(this IReadOnlyGenericFile file)
      => new(file.OpenRead());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadNew<T>(this IReadOnlyGenericFile file)
        where T : IBinaryDeserializable, new() {
      using var er = new EndianBinaryReader(file.OpenRead());
      return er.ReadNew<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadNew<T>(this IReadOnlyGenericFile file,
                               Endianness endianness)
        where T : IBinaryDeserializable, new() {
      using var er = new EndianBinaryReader(file.OpenRead(), endianness);
      return er.ReadNew<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadNewFromText<T>(this IReadOnlyGenericFile file)
        where T : ITextDeserializable, new() {
      using var er = new TextReader(file.OpenRead());
      return er.ReadNew<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadAllBytes(this IReadOnlyGenericFile file) {
      using var s = file.OpenRead();
      using var ms = new MemoryStream();
      s.CopyTo(ms);
      return ms.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadAllText(this IReadOnlyGenericFile file) {
      using var sr = file.OpenReadAsText();
      return sr.ReadToEnd();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] ReadAllLines(this IReadOnlyGenericFile file)
      => file.ReadAllText()
             .Split(TextReaderConstants.NEWLINE_STRINGS,
                    StringSplitOptions.None);


    // Write methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamWriter OpenWriteAsText(this IGenericFile file)
      => new(file.OpenWrite());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAllBytes(this IGenericFile file,
                                     ReadOnlyMemory<byte> bytes) {
      using var s = file.OpenWrite();
      s.Write(bytes.Span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAllText(this IGenericFile file,
                                    string text) {
      using var sw = file.OpenWriteAsText();
      sw.Write(text);
    }
  }
}