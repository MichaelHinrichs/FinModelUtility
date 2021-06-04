using System;

using fin.data;
using fin.util.strings;
using System.Threading.Tasks;

namespace fin.language.writer {
  public class AsyncBlockStringBuilderImpl : BAsyncBlockStringBuilder {
    private readonly AsyncCollector<string> impl_ = new();

    public override BAsyncBlockStringBuilder Write(string content) {
      this.impl_.Add(content);
      return this;
    }

    public override BAsyncBlockStringBuilder Write(Task<string> content) {
      this.impl_.Add(content);
      return this;
    }


    public override BAsyncBlockStringBuilder WriteBlock(
        string prefix,
        string content,
        string suffix) {
      this.impl_.Add(prefix);
      this.impl_.Add(content);
      this.impl_.Add(suffix);
      return this;
    }

    public override BAsyncBlockStringBuilder WriteBlock(
        string prefix,
        Task<string> content,
        string suffix) {
      this.impl_.Add(prefix);
      this.impl_.Add(content);
      this.impl_.Add(suffix);
      return this;
    }


    public override BAsyncBlockStringBuilder WriteBlock(
        string prefix,
        Action<BAsyncBlockStringBuilder> callback,
        string suffix) {
      this.impl_.Add(prefix);
      callback(this);
      this.impl_.Add(suffix);
      return this;
    }


    public override async Task<string> ToString()
      => StringUtil.Concat(await this.impl_.ToArray());
  }
}
