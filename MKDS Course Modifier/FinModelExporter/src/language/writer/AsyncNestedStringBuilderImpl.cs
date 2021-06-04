using System;
using System.Text;

using fin.data;
using fin.util.strings;

using System.Threading.Tasks;

namespace fin.language.writer {
  public class AsyncNestedStringBuilderImpl : BAsyncNestedStringBuilder {
    private readonly AsyncCollector<(string, int)> impl_ = new();
    private int indentLevel_ = 0;


    public override BAsyncNestedStringBuilder Write(string content) {
      this.Write_(content);
      return this;
    }

    public override BAsyncNestedStringBuilder Write(Task<string> content) {
      this.Write_(content);
      return this;
    }


    public override BAsyncNestedStringBuilder WriteLine(string content) {
      this.Write_(content);
      this.Write_("\n");
      return this;
    }

    public override BAsyncNestedStringBuilder WriteLine(Task<string> content) {
      this.Write_(content);
      this.Write_("\n");
      return this;
    }


    public override string Indent { get; set; } = "";


    public override BAsyncNestedStringBuilder Nest(
        string content) {
      ++this.indentLevel_;
      this.Write_(content);
      --this.indentLevel_;
      return this;
    }

    public override BAsyncNestedStringBuilder Nest(
        Task<string> content) {
      ++this.indentLevel_;
      this.Write_(content);
      --this.indentLevel_;
      return this;
    }


    public override BAsyncNestedStringBuilder Nest(
        Action<BAsyncNestedStringBuilder> callback) {
      ++this.indentLevel_;
      callback(this);
      --this.indentLevel_;
      return this;
    }


    public override async Task<string> ToString() {
      var builder = new StringBuilder();

      var currentLine = new StringBuilder();
      var currentIndent = 0;

      var contentAndIndents = await this.impl_.ToArray();
      foreach (var (content, indent) in contentAndIndents) {
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

    private void Write_(Task<string> content) {
      var indentLevel = this.indentLevel_;
      this.impl_.Add(
          content.ContinueWith(content => (content.Result, indentLevel)));
    }
  }
}