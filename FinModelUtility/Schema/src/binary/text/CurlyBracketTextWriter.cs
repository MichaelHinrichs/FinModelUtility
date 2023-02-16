using System;
using System.IO;

namespace schema.binary.text {
  public interface ICurlyBracketTextWriter {
    public ICurlyBracketTextWriter EnterBlock(string prefix = "");
    public ICurlyBracketTextWriter WriteLine(string text);
    public ICurlyBracketTextWriter ExitBlock();
  }

  public class CurlyBracketTextWriter : ICurlyBracketTextWriter {
    private readonly TextWriter impl_;
    private int indentLevel_ = 0;

    public CurlyBracketTextWriter(TextWriter impl) {
      this.impl_ = impl;
    }

    public ICurlyBracketTextWriter EnterBlock(string prefix = "") {
      if (prefix.Length > 0) {
        prefix = $"{prefix} ";
      }

      this.PrintIndent_();
      this.impl_.WriteLine($"{prefix}{{");
      ++this.indentLevel_;
      return this;
    }

    public ICurlyBracketTextWriter WriteLine(string text) {
      var lines = text.Split('\n');
      foreach (var line in lines) {
        foreach (var c in line) {
          if (c == '}') {
            --this.indentLevel_;
          }
        }

        this.PrintIndent_();
        this.impl_.WriteLine(line);

        foreach (var c in line) {
          if (c == '{') {
            ++this.indentLevel_;
          }
        }
      }

      return this;
    }

    public ICurlyBracketTextWriter ExitBlock() {
      --this.indentLevel_;
      this.PrintIndent_();
      this.impl_.WriteLine("}");

      if (this.indentLevel_ < 0) {
        throw new Exception("Exited an extra block!");
      }

      return this;
    }

    private void PrintIndent_() {
      for (var i = 0; i < this.indentLevel_; ++i) {
        this.impl_.Write("  ");
      }
    }
  }
}