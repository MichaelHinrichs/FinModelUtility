using System;
using System.Threading.Tasks;

namespace fin.language.writer {
  public abstract class BStringBuilder {
    public abstract BStringBuilder Write(string content);
    public abstract BStringBuilder WriteLine(string content);

    public new abstract string ToString();
  }

  public abstract class BNestedStringBuilder : BStringBuilder {
    public abstract override BNestedStringBuilder Write(string content);

    public abstract override BNestedStringBuilder
        WriteLine(string content);

    
    public abstract string Indent { get; }
    
    public abstract BNestedStringBuilder Nest(string content);

    public abstract BNestedStringBuilder Nest(
        Action<BNestedStringBuilder> callback);
  }
}