using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.math.interpolation {
  [TestClass]
  public class InterpolatorWithTangentsTest {
    [TestMethod]
    public void TestInterpolationStartAndEnd() {
      var fromTime = 1;
      var fromValue = 2;
      var fromTangent = 3;
      var toTime = 4;
      var toValue = 5;
      var toTangent = 6;

      Assert.AreEqual(fromValue,
                      InterpolatorWithTangents.InterpolateFloats(
                          fromTime, fromValue, fromTangent,
                          toTime, toValue, toTangent,
                          fromTime));

      Assert.AreEqual(toValue,
                      InterpolatorWithTangents.InterpolateFloats(
                          fromTime, fromValue, fromTangent,
                          toTime, toValue, toTangent,
                          toTime));
    }

    /*[TestMethod]
    public void TestLinearInterpolation() {
      var fromTime = 1;
      var fromValue = 2;
      var fromTangent = 0;
      var toTime = 3;
      var toValue = 4;
      var toTangent = 0;

      var n = 100;
      for (var i = 0; i < n; ++i) {
        var progress = 1f * i / n;

        var expectedValue = fromValue * (1 - progress) + toValue * progress;
        var actualValue =
            InterpolatorWithTangents.InterpolateFloats(
                fromTime, fromValue, fromTangent, toTime, toValue, toTangent,
                fromTime * (1 - progress) + toTime * progress);

        Assert.AreEqual(expectedValue, actualValue, .001);
      }
    }*/
  }
}