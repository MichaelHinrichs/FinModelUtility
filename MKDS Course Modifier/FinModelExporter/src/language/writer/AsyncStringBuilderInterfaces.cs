using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.language.writer {
  public abstract class BAsyncStringBuilder {
    public abstract BAsyncStringBuilder Write(string content);
    public abstract BAsyncStringBuilder Write(Task<string> content);

    public new abstract Task<string> ToString();
  }

  public abstract class BAsyncBlockStringBuilder : BAsyncStringBuilder {
    public abstract override BAsyncBlockStringBuilder Write(string content);
    public abstract override BAsyncBlockStringBuilder Write(Task<string> content);

    
    public abstract BAsyncBlockStringBuilder WriteBlock(
        string prefix,
        string content,
        string suffix);

    public abstract BAsyncBlockStringBuilder WriteBlock(
        string prefix,
        Task<string> content,
        string suffix);

    public abstract BAsyncBlockStringBuilder WriteBlock(
        string prefix,
        Action<BAsyncBlockStringBuilder> callback,
        string suffix);
  }
}