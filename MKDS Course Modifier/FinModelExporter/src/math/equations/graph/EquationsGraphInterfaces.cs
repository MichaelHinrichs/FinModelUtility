namespace fin.math.equations.graph {
  public interface IEquationGraph {

  }

  public interface IScope {

  }

  public interface IExpression {
    IVariable ToVariable(string name);
  }

  public interface ITerm : IExpression {
    ITerm Add(ITerm other);
  }

  public interface IFactor : ITerm {
    IFactor Multiply(IFactor other);
  }

  public interface IParentheses : IFactor {
    IExpression Inside { get; }
  }

  public interface IVariable : IFactor {
    string Name { get; set; }
  }
}
