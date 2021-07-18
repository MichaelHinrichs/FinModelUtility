namespace UoT {
  public class Interpolation {
    private static double DifferenceInDegrees_(double degLhs, double degRhs) {
      return ((((degLhs - degRhs) % 360) + 540) % 360) - 180;
    }

    public static double Degrees(double start, double finish, double frac) {
      return start + Interpolation.DifferenceInDegrees_(finish, start) * frac;
    }
  }
}
