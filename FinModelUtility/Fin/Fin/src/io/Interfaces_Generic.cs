using System;
using System.IO;
using System.IO.Abstractions;

using schema.binary;
using schema.text;

namespace fin.io {
  public interface IReadOnlyGenericFile {
    string DisplayFullPath { get; }

    FileSystemStream OpenRead();
    StreamReader OpenReadAsText();

    T ReadNew<T>() where T : IBinaryDeserializable, new();
    T ReadNew<T>(Endianness endianness) where T : IBinaryDeserializable, new();

    T ReadNewFromText<T>() where T : ITextDeserializable, new();

    byte[] ReadAllBytes();
    string ReadAllText();
    T Deserialize<T>();
  }

  public interface IGenericFile : IReadOnlyGenericFile {
    FileSystemStream OpenWrite();
    StreamWriter OpenWriteAsText();

    void WriteAllBytes(ReadOnlyMemory<byte> bytes);
    void WriteAllText(string text);

    void Serialize<T>(T instance) where T : notnull;
  }
}