using fin.data.queues;

namespace fin.language.equations.fixedFunction {
  public partial class FixedFunctionEquations<TIdentifier> :
      IFixedFunctionEquations<TIdentifier> {
    public bool HasInput(TIdentifier identifier)
      => this.ColorInputs.ContainsKey(identifier) ||
         this.ScalarInputs.ContainsKey(identifier);

    public bool DoOutputsDependOn(TIdentifier[] outputIdentifiers,
                                  IValue value) {
      var colorQueue = new FinQueue<IColorValue>();
      var scalarQueue = new FinQueue<IScalarValue>();

      foreach (var outputIdentifier in outputIdentifiers) {
        if (this.colorOutputs_.TryGetValue(outputIdentifier,
                                           out var colorOutput)) {
          colorQueue.Enqueue(colorOutput);
        }

        if (this.scalarOutputs_.TryGetValue(outputIdentifier,
                                            out var scalarOutput)) {
          scalarQueue.Enqueue(scalarOutput);
        }
      }

      bool didUpdate;
      do {
        didUpdate = false;
        if (colorQueue.TryDequeue(out var colorValue)) {
          didUpdate = true;

          if (colorValue == value) {
            return true;
          }

          switch (colorValue) {
            case IColorIdentifiedValue<TIdentifier> colorIdentifiedValue: {
              colorQueue.Enqueue(colorIdentifiedValue.ColorValue);
              break;
            }
            case IColorNamedValue colorNamedValue: {
              colorQueue.Enqueue(colorNamedValue.ColorValue);
              break;
            }
            case IColorExpression colorExpression: {
              colorQueue.Enqueue(colorExpression.Terms);
              break;
            }
            case IColorTerm colorTerm: {
              colorQueue.Enqueue(colorTerm.NumeratorFactors);
              if (colorTerm.DenominatorFactors != null) {
                colorQueue.Enqueue(colorTerm.DenominatorFactors);
              }

              break;
            }
            case IColorValueTernaryOperator colorValueTernaryOperator: {
              scalarQueue.Enqueue(colorValueTernaryOperator.Lhs);
              scalarQueue.Enqueue(colorValueTernaryOperator.Rhs);
              colorQueue.Enqueue(colorValueTernaryOperator.TrueValue);
              colorQueue.Enqueue(colorValueTernaryOperator.FalseValue);
              break;
            }
            default: {
              if (colorValue.Intensity != null) {
                scalarQueue.Enqueue(colorValue.Intensity);
              } else {
                scalarQueue.Enqueue(colorValue.R);
                scalarQueue.Enqueue(colorValue.G);
                scalarQueue.Enqueue(colorValue.B);
              }

              break;
            }
          }
        }

        if (scalarQueue.TryDequeue(out var scalarValue)) {
          didUpdate = true;

          if (scalarValue == value) {
            return true;
          }

          switch (scalarValue) {
            case IScalarIdentifiedValue<TIdentifier> scalarIdentifiedValue: {
              scalarQueue.Enqueue(scalarIdentifiedValue.ScalarValue);
              break;
            }
            case IScalarNamedValue scalarNamedValue: {
              scalarQueue.Enqueue(scalarNamedValue.ScalarValue);
              break;
            }
            case IColorValueSwizzle colorValueSwizzle: {
              colorQueue.Enqueue(colorValueSwizzle.Source);
              break;
            }
            case IScalarExpression scalarExpression: {
              scalarQueue.Enqueue(scalarExpression.Terms);
              break;
            }
            case IScalarTerm scalarTerm: {
              scalarQueue.Enqueue(scalarTerm.NumeratorFactors);
              if (scalarTerm.DenominatorFactors != null) {
                scalarQueue.Enqueue(scalarTerm.DenominatorFactors);
              }

              break;
            }
          }
        }
      } while (didUpdate);

      return false;
    }
  }
}