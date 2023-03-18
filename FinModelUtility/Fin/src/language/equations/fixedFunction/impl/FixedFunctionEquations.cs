namespace fin.language.equations.fixedFunction {
  public partial class FixedFunctionEquations<TIdentifier> :
      IFixedFunctionEquations<TIdentifier> {
    public bool HasInput(TIdentifier identifier)
      => this.ColorInputs.ContainsKey(identifier) ||
         this.ScalarInputs.ContainsKey(identifier);
  }
}