using System;
using System.Collections.Generic;
using System.Text;

namespace fin.language.writer {
  public class NestedStringBuilderImpl : BNestedStringBuilder {
    private readonly List<(string, int)> impl_ = new();
    private int indentLevel_ = 0;

    public NestedStringBuilderImpl(string indent = "") {
      this.Indent = indent;
    }


    public override BNestedStringBuilder Write(string content) {
      this.Write_(content);
      return this;
    }

    public override BNestedStringBuilder WriteLine(string content) {
      this.Write_(content);
      this.Write_("\n");
      return this;
    }


    public override string Indent { get; }


    public override BNestedStringBuilder Nest(
        string content) {
      ++this.indentLevel_;
      this.Write_(content);
      --this.indentLevel_;
      return this;
    }

    public override BNestedStringBuilder Nest(
        Action<BNestedStringBuilder> callback) {
      ++this.indentLevel_;
      callback(this);
      --this.indentLevel_;
      return this;
    }


    public override string ToString() {
      var builder = new StringBuilder();

      var currentLine = new StringBuilder();
      var currentIndent = 0;

      foreach (var (content, indent) in this.impl_) {
        var lines = content.Split('\n');

        var appendContent = currentLine.Length > 0 &&
                            content.Length > 0 &&
                            (currentIndent != indent || indent > 0);
        if (appendContent) {
          builder.Append(currentLine.ToString()).Append('\n');
          currentLine.Clear();
        }

        currentIndent = indent;

        for (var l = 0; l < lines.Length; ++l) {
          var line = lines[l];

          if (l == 0 && line == "" && appendContent) {
            continue;
          }

          var isLastLine = l == lines.Length - 1;
          if (isLastLine && line.Length == 0) {
            continue;
          }

          if (currentLine.Length == 0) {
            for (var i = 0; i < indent; ++i) {
              currentLine.Append(this.Indent);
            }
          }

          currentLine.Append(line);

          if (!isLastLine) {
            builder.Append(currentLine.ToString()).Append('\n');
            currentLine.Clear();
          }
        }
      }

      builder.Append(currentLine.ToString());

      return builder.ToString();
    }


    private void Write_(string content)
      => this.impl_.Add((content, this.indentLevel_));
  }
}