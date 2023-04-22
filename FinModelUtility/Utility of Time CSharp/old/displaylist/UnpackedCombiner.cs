using UoT.util.array;

namespace UoT.displaylist {
  public class UnpackedCombiner {
    public uint[] cA { get; }
    public uint[] cB { get; }
    public uint[] cC { get; }
    public uint[] cD { get; }
    public uint[] aA { get; }
    public uint[] aB { get; }
    public uint[] aC { get; }
    public uint[] aD { get; }

    public UnpackedCombiner() {
      this.cA = new uint[2];
      this.cB = new uint[2];
      this.cC = new uint[2];
      this.cD = new uint[2];
      this.aA = new uint[2];
      this.aB = new uint[2];
      this.aC = new uint[2];
      this.aD = new uint[2];
    }

    private UnpackedCombiner(
        uint[] cA,
        uint[] cB,
        uint[] cC,
        uint[] cD,
        uint[] aA,
        uint[] aB,
        uint[] aC,
        uint[] aD) {
      this.cA = cA;
      this.cB = cB;
      this.cC = cC;
      this.cD = cD;
      this.aA = aA;
      this.aB = aB;
      this.aC = aC;
      this.aD = aD;
    }

    public UnpackedCombiner Clone() => new UnpackedCombiner(
        ArrayUtil.Copy(this.cA),
        ArrayUtil.Copy(this.cB),
        ArrayUtil.Copy(this.cC),
        ArrayUtil.Copy(this.cD),
        ArrayUtil.Copy(this.aA),
        ArrayUtil.Copy(this.aB),
        ArrayUtil.Copy(this.aC),
        ArrayUtil.Copy(this.aD));
  }
}