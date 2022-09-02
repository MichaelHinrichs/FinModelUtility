// Decompiled with JetBrains decompiler
// Type: System.IO.Endianness
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Collections.Generic;


namespace System.IO {
  public enum Endianness {
    BigEndian,
    LittleEndian,
  }

  public static class EndiannessUtil {
    public static Endianness SystemEndianness
      => BitConverter.IsLittleEndian
             ? Endianness.LittleEndian
             : Endianness.BigEndian;
  }

  public interface IEndiannessStack {
    Endianness Endianness { get; }

    bool Reverse { get; }

    /// <summary>
    ///   Pushes a class's endianness. This will be overwritten by the
    ///   reader/writer/field endianness if those were already pushed.
    /// </summary>
    void PushClassEndianness(Endianness endianness);

    /// <summary>
    ///   Pushes a field's endianness. This will override any other
    ///   endiannesses that were previously pushed.
    /// </summary>
    void PushFieldEndianness(Endianness endianness);

    void PopEndianness();
  }

  public class EndiannessStackImpl : IEndiannessStack {
    private readonly Stack<Endianness?> endiannessStack_ = new();

    public EndiannessStackImpl() {
      this.endiannessStack_.Push(null);
    }

    public Endianness Endianness
      => this.endiannessStack_.Peek() ?? EndiannessUtil.SystemEndianness;

    public bool Reverse => this.Endianness != EndiannessUtil.SystemEndianness;

    /// <summary>
    ///   Pushes a class's endianness. This will be overwritten by the
    ///   reader/writer/field endianness if those were already pushed.
    /// </summary>
    public void PushClassEndianness(Endianness endianness)
      => this.endiannessStack_.Push(this.endiannessStack_.Peek() ?? endianness);

    /// <summary>
    ///   Pushes a field's endianness. This will override any other
    ///   endiannesses that were previously pushed.
    /// </summary>
    /// <param name="endianness"></param>
    public void PushFieldEndianness(Endianness endianness)
      => this.endiannessStack_.Push(endianness);

    public void PopEndianness() => this.endiannessStack_.Pop();
  }
}