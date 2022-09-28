using fin.math.matrix;
using fin.util.asserts;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.math {
  [TestClass]
  public class MatrixTests {
    [TestMethod]
    public void TestInvert() {
      var inputMatrix = new FinMatrix4x4();
      inputMatrix[0, 0] = 2;
      inputMatrix[0, 1] = 5;
      inputMatrix[0, 2] = 0;
      inputMatrix[0, 3] = 8;
      inputMatrix[1, 0] = 1;
      inputMatrix[1, 1] = 4;
      inputMatrix[1, 2] = 2;
      inputMatrix[1, 3] = 6;
      inputMatrix[2, 0] = 7;
      inputMatrix[2, 1] = 8;
      inputMatrix[2, 2] = 9;
      inputMatrix[2, 3] = 3;
      inputMatrix[3, 0] = 1;
      inputMatrix[3, 1] = 5;
      inputMatrix[3, 2] = 7;
      inputMatrix[3, 3] = 8;

      var actualMatrix = inputMatrix.CloneAndInvert();

      var expectedMatrix = new FinMatrix4x4();
      expectedMatrix[0, 0] = 172f / 179;
      expectedMatrix[0, 1] = -343f / 179;
      expectedMatrix[0, 2] = 14f / 179;
      expectedMatrix[0, 3] = 80f / 179;
      expectedMatrix[1, 0] = -185f / 179;
      expectedMatrix[1, 1] = 422f / 179;
      expectedMatrix[1, 2] = 12f / 179;
      expectedMatrix[1, 3] = -136f / 179;
      expectedMatrix[2, 0] = -1f / 179;
      expectedMatrix[2, 1] = -49f / 179;
      expectedMatrix[2, 2] = 2f / 179;
      expectedMatrix[2, 3] = 37f / 179;
      expectedMatrix[3, 0] = 95f / 179;
      expectedMatrix[3, 1] = -178f / 179;
      expectedMatrix[3, 2] = -11f / 179;
      expectedMatrix[3, 3] = 65f / 179;

      for (var r = 0; r < 4; r++) {
        for (var c = 0; c < 4; c++) {
          Assert.AreEqual(expectedMatrix[r, c], actualMatrix[r, c], .0001f);
        }
      }
    }

    [TestMethod]
    public void TestMultiplyByInverse() {
      var inputMatrix = new FinMatrix4x4();
      inputMatrix[0, 0] = 2;
      inputMatrix[0, 1] = 5;
      inputMatrix[0, 2] = 0;
      inputMatrix[0, 3] = 8;
      inputMatrix[1, 0] = 1;
      inputMatrix[1, 1] = 4;
      inputMatrix[1, 2] = 2;
      inputMatrix[1, 3] = 6;
      inputMatrix[2, 0] = 7;
      inputMatrix[2, 1] = 8;
      inputMatrix[2, 2] = 9;
      inputMatrix[2, 3] = 3;
      inputMatrix[3, 0] = 1;
      inputMatrix[3, 1] = 5;
      inputMatrix[3, 2] = 7;
      inputMatrix[3, 3] = 8;

      var inverseMatrix = inputMatrix.CloneAndInvert();

      var actualMatrix = inputMatrix.CloneAndMultiply(inverseMatrix);
      var expectedMatrix = new FinMatrix4x4().SetIdentity();

      for (var r = 0; r < 4; r++) {
        for (var c = 0; c < 4; c++) {
          Assert.AreEqual(expectedMatrix[r, c], actualMatrix[r, c], .0001f);
        }
      }
    }

    [TestMethod]
    public void TestCloseEquals() {
      var identityMatrix = new FinMatrix4x4().SetIdentity();
      var closeToIdentityMatrix = this.GetCloseToIdentityMatrix_();
      Asserts.True(identityMatrix.Equals(closeToIdentityMatrix));
    }

    [TestMethod]
    public void TestCloseHashCode() {
      var identityMatrix = new FinMatrix4x4().SetIdentity();
      var closeToIdentityMatrix = this.GetCloseToIdentityMatrix_();
      Asserts.Equal(identityMatrix.GetHashCode(),
                    closeToIdentityMatrix.GetHashCode());
    }

    [TestMethod]
    public void TestDifferentEquals() {
      var identityMatrix = new FinMatrix4x4().SetIdentity();
      var differentFromIdentityMatrix = this.GetDifferentFromIdentityMatrix_();
      Asserts.False(identityMatrix.Equals(differentFromIdentityMatrix));
    }

    [TestMethod]
    public void TestDifferentHashCode() {
      var identityMatrix = new FinMatrix4x4().SetIdentity();
      var differentFromIdentityMatrix = this.GetDifferentFromIdentityMatrix_();
      Assert.AreNotEqual(identityMatrix.GetHashCode(),
                         differentFromIdentityMatrix.GetHashCode());
    }


    private IReadOnlyFinMatrix4x4 GetCloseToIdentityMatrix_() {
      var closeToIdentityMatrix = new FinMatrix4x4().SetZero();

      var error = .00001;
      closeToIdentityMatrix[0, 0] = 1 + error;
      closeToIdentityMatrix[0, 1] = error;
      closeToIdentityMatrix[0, 2] = -error;
      closeToIdentityMatrix[0, 3] = error;
      closeToIdentityMatrix[1, 0] = -error;
      closeToIdentityMatrix[1, 1] = 1 - error;
      closeToIdentityMatrix[1, 2] = error;
      closeToIdentityMatrix[1, 3] = -error;
      closeToIdentityMatrix[2, 0] = error;
      closeToIdentityMatrix[2, 1] = -error;
      closeToIdentityMatrix[2, 2] = 1 + error;
      closeToIdentityMatrix[2, 3] = error;
      closeToIdentityMatrix[3, 0] = -error;
      closeToIdentityMatrix[3, 1] = error;
      closeToIdentityMatrix[3, 2] = -error;
      closeToIdentityMatrix[3, 3] = 1 - error;

      return closeToIdentityMatrix;
    }

    private IReadOnlyFinMatrix4x4 GetDifferentFromIdentityMatrix_() {
      var closeToIdentityMatrix = new FinMatrix4x4().SetZero();

      var error = .001;
      closeToIdentityMatrix[0, 0] = 1 + error;
      closeToIdentityMatrix[0, 1] = error;
      closeToIdentityMatrix[0, 2] = -error;
      closeToIdentityMatrix[0, 3] = error;
      closeToIdentityMatrix[1, 0] = -error;
      closeToIdentityMatrix[1, 1] = 1 - error;
      closeToIdentityMatrix[1, 2] = error;
      closeToIdentityMatrix[1, 3] = -error;
      closeToIdentityMatrix[2, 0] = error;
      closeToIdentityMatrix[2, 1] = -error;
      closeToIdentityMatrix[2, 2] = 1 + error;
      closeToIdentityMatrix[2, 3] = error;
      closeToIdentityMatrix[3, 0] = -error;
      closeToIdentityMatrix[3, 1] = error;
      closeToIdentityMatrix[3, 2] = -error;
      closeToIdentityMatrix[3, 3] = 1 - error;

      return closeToIdentityMatrix;
    }
  }
}