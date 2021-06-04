namespace fin.shader {
  public interface IGlslAction {
  }

  public interface IGlslFunc<TOut>
      where TOut : IGlslValue {
    TOut Call();
  }

  /*public interface IGlslFunc<TIn, TOut>
      where TIn : IGlslValue
      where TOut : IGlslValue {
    TIn 

    TOut Call(TIn v);
  }*/

  public interface IGlslFunc<TIn1, TIn2, TOut>
      where TIn1 : IGlslValue
      where TIn2 : IGlslValue
      where TOut : IGlslValue {
    TOut Call(TIn1 v1, TIn2 v2);
  }
}