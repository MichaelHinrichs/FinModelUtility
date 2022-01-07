using System;
using System.Text;

namespace schema.text {
  internal interface ICurlyBracketStringBuilder {
    public ICurlyBracketStringBuilder EnterBlock(string prefix = "");
    public ICurlyBracketStringBuilder WriteLine(string text);
    public ICurlyBracketStringBuilder ExitBlock();
  }

  internal class CurlyBracketStringBuilder : ICurlyBracketStringBuilder {
    private StringBuilder impl_ = new();
    private int indentLevel_ = 0;

    public ICurlyBracketStringBuilder EnterBlock(string prefix = "") {
      this.PrintIndent_();
      this.impl_.Append($"{prefix} {{");
      ++this.indentLevel_;
      return this;
    }

    public ICurlyBracketStringBuilder WriteLine(string text) {
      var lines = text.Split('\n');
      foreach (var line in lines) {
        this.PrintIndent_();
        this.impl_.Append($"{line}\n");
      }
      return this;
    }

    public ICurlyBracketStringBuilder ExitBlock() {
      this.PrintIndent_();
      this.impl_.Append($"}}");
      --this.indentLevel_;

      if (this.indentLevel_ < 0) {
        throw new Exception("Exited an extra block!");
      }

      return this;
    }

    private void PrintIndent_() {
      for (var i = 0; i < this.indentLevel_; ++i) {
        this.impl_.Append("  ");
      }
    }

    public string ToString() {
      var text = this.impl_.ToString();

      var level = 0;
      foreach (var c in text) {
        if (c == '{') {
          ++level;
        } else if (c == '}') {
          --level;
        }

        if (level < 0) {
          throw new Exception("Exited an extra block!");
        }
      }

      if (level != 0) {
        throw new Exception("Not an equal number of brackets!");
      }

      return text;
    }
  }
}