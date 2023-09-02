using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;

using fin.util.asserts;
using fin.util.json;

using schema.binary;

namespace fin.io {
  using IROTreeIoObj =
      ITreeIoObject<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;
  using IROTreeFile =
      ITreeFile<IReadOnlySystemIoObject, IReadOnlySystemDirectory,
          IReadOnlySystemFile, string>;

  public readonly struct FinFile : ISystemFile {
    public FinFile(string fullName) {
      this.FullPath = fullName;
    }

    public override string ToString() => this.DisplayFullName;


    // Equality
    public bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }

      if (other is not IReadOnlySystemIoObject otherSelf) {
        return false;
      }

      return this.Equals(otherSelf);
    }

    public bool Equals(ISystemIoObject? other)
      => this.Equals(other as IReadOnlySystemIoObject);

    public bool Equals(IReadOnlySystemIoObject? other)
      => this.FullPath == other?.FullPath;


    // File fields
    public string Name => FinIoStatic.GetName(this.FullPath);
    public string FullPath { get; }
    public string DisplayFullName => this.FullPath;


    // Ancestry methods
    public string? GetParentFullPath()
      => FinIoStatic.GetParentFullName(this.FullPath);

    IReadOnlySystemDirectory IROTreeIoObj.AssertGetParent()
      => this.AssertGetParent();

    public ISystemDirectory AssertGetParent() {
      if (this.TryGetParent(out ISystemDirectory parent)) {
        return parent;
      }

      throw new Exception("Expected parent directory to exist!");
    }

    public bool TryGetParent(out IReadOnlySystemDirectory parent) {
      parent = default;
      return this.TryGetParent(
          out Unsafe
              .As<IReadOnlySystemDirectory, ISystemDirectory>(ref parent));
    }

    public bool TryGetParent(out ISystemDirectory parent) {
      var parentName = this.GetParentFullPath();
      if (parentName != null) {
        parent = new FinDirectory(parentName);
        return true;
      }

      parent = default;
      return false;
    }

    IEnumerable<IReadOnlySystemDirectory> IROTreeIoObj.GetAncestry()
      => this.GetAncestry();

    public IEnumerable<ISystemDirectory> GetAncestry()
      => this.GetUpwardAncestry_().Reverse();

    private IEnumerable<ISystemDirectory> GetUpwardAncestry_() {
      if (!this.TryGetParent(out ISystemDirectory firstParent)) {
        yield break;
      }

      var current = firstParent;
      while (current.TryGetParent(out var parent)) {
        yield return parent;
        current = parent;
      }
    }


    // File methods
    public bool Exists => FinFileStatic.Exists(this.FullPath);

    string IReadOnlyGenericFile.DisplayFullPath => this.FullPath;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Delete() => FinFileStatic.Delete(this.FullPath);

    public string FileType => FinFileStatic.GetExtension(this.FullPath);

    public string FullNameWithoutExtension
      => FinFileStatic.GetNameWithoutExtension(this.FullPath);

    public string NameWithoutExtension
      => FinFileStatic.GetNameWithoutExtension(this.Name);

    IReadOnlySystemFile IROTreeFile.CloneWithFileType(string newFileType)
      => this.CloneWithFileType(newFileType);

    public ISystemFile CloneWithFileType(string newExtension) {
      Asserts.True(newExtension.StartsWith("."),
                   $"'{newExtension}' is not a valid extension!");
      return new FinFile(this.FullNameWithoutExtension + newExtension);
    }

    // JSON Serialization
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Deserialize<T>() => JsonUtil.Deserialize<T>(this.ReadAllText());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Serialize<T>(T instance) where T : notnull
      => this.WriteAllText(JsonUtil.Serialize(instance));

    // Read methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FileSystemStream OpenRead()
      => FinFileStatic.OpenRead(this.FullPath);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StreamReader OpenReadAsText() => new(this.OpenRead());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadNew<T>() where T : IBinaryDeserializable, new() {
      using var er = new EndianBinaryReader(this.OpenRead());
      return er.ReadNew<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadNew<T>(Endianness endianness)
        where T : IBinaryDeserializable, new() {
      using var er = new EndianBinaryReader(this.OpenRead(), endianness);
      return er.ReadNew<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadAllBytes() {
      using var s = this.OpenRead();
      using var ms = new MemoryStream();
      s.CopyTo(ms);
      return ms.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadAllText() {
      using var sr = this.OpenReadAsText();
      return sr.ReadToEnd();
    }


    // Write methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FileSystemStream OpenWrite()
      => FinFileStatic.OpenWrite(this.FullPath);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StreamWriter OpenWriteAsText() => new(this.OpenWrite());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteAllBytes(ReadOnlyMemory<byte> bytes) {
      using var s = this.OpenWrite();
      s.Write(bytes.Span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteAllText(string text) {
      using var sw = this.OpenWriteAsText();
      sw.Write(text);
    }
  }
}