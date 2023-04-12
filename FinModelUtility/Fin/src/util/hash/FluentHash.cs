namespace fin.util.hash {
  public struct FluentHash {
    public int Hash { get; private set; }
    private readonly int primeCoefficient_;

    public FluentHash(int startingPrimeHash, int primeCoefficient) {
      this.Hash = startingPrimeHash;
      this.primeCoefficient_ = primeCoefficient;
    }

    public static FluentHash Start() => FluentHash.Start(17, 23);

    public static FluentHash Start(int startingPrimeHash,
                                   int primeCoefficient)
      => new FluentHash(startingPrimeHash, primeCoefficient);

    public FluentHash With<T>(T other) where T : notnull {
      this.Hash = this.Hash * this.primeCoefficient_ + other.GetHashCode();
      return this;
    }

    public static implicit operator int(FluentHash d) => d.Hash;

    public override int GetHashCode() => this.Hash;

    public override bool Equals(object? other)
      => other?.GetHashCode() == this.GetHashCode();
  }
}