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
  public readonly struct FinFile : ISystemFile {
    public FinFile(string fullName) {
      this.FullName = fullName;
    }

    public override string ToString() => this.DisplayFullName;


    // Equality
    public bool Equals(object? other) {
      if (object.ReferenceEquals(this, other)) {
        return true;
      }

      if (other is not ISystemIoObject otherSelf) {
        return false;
      }

      return this.Equals(otherSelf);
    }

    public bool Equals(ISystemIoObject? other)
      => this.FullName == other?.FullName;


    // File fields
    public string Name => FinIoStatic.GetName(this.FullName);
    public string FullName { get; }
    public string DisplayFullName => this.FullName;


    // Ancestry methods
    public string? GetParentFullName()
      => FinIoStatic.GetParentFullName(this.FullName);

    public ISystemDirectory GetParent() {
      if (this.TryGetParent(out var parent)) {
        return parent;
      }

      throw new Exception("Expected parent directory to exist!");
    }

    public bool TryGetParent(out ISystemDirectory parent) {
      var parentName = this.GetParentFullName();
      if (parentName != null) {
        parent = new FinDirectory(parentName);
        return true;
      }

      parent = default;
      return false;
    }

    public ISystemDirectory[] GetAncestry() {
      if (!this.TryGetParent(out var firstParent)) {
        return Array.Empty<ISystemDirectory>();
      }

      var parents = new LinkedList<ISystemDirectory>();
      var current = firstParent;
      while (current.TryGetParent(out var parent)) {
        parents.AddLast(parent);
        current = parent;
      }

      return parents.ToArray();
    }


    // File methods
    public bool Exists => FinFileStatic.Exists(this.FullName);

    string IReadOnlyGenericFile.DisplayPath => this.FullName;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Delete() => FinFileStatic.Delete(FullName);

    public string Extension => FinFileStatic.GetExtension(FullName);

    public string FullNameWithoutExtension
      => FinFileStatic.GetNameWithoutExtension(this.FullName);

    public string NameWithoutExtension
      => FinFileStatic.GetNameWithoutExtension(this.Name);

    public ISystemFile CloneWithExtension(string newExtension) {
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
      => FinFileStatic.OpenRead(this.FullName);

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
      => FinFileStatic.OpenWrite(this.FullName);

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