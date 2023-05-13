using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.language.writer {
  public abstract class BAsyncStringBuilder {
    public abstract BAsyncStringBuilder Write(string content);
    public abstract BAsyncStringBuilder Write(Task<string> content);

    public abstract BAsyncStringBuilder WriteLine(string content);
    public abstract BAsyncStringBuilder WriteLine(Task<string> content);


    public new abstract Task<string> ToString();
  }

  public abstract class BAsyncNestedStringBuilder : BAsyncStringBuilder {
    public abstract override BAsyncNestedStringBuilder Write(string content);

    public abstract override BAsyncNestedStringBuilder Write(
        Task<string> content);

    public abstract override BAsyncNestedStringBuilder
        WriteLine(string content);

    public abstract override BAsyncNestedStringBuilder WriteLine(
        Task<string> content);

    
    public abstract string Indent { get; }
    
    public abstract BAsyncNestedStringBuilder Nest(string content);

    public abstract BAsyncNestedStringBuilder Nest(Task<string> content);

    public abstract BAsyncNestedStringBuilder Nest(
        Action<BAsyncNestedStringBuilder> callback);
  }
}