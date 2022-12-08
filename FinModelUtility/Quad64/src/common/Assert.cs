namespace Quad64.common {
  public static class Assert {
    public static void Fail(string? message = null) 
      => throw new Exception(message ?? "Failed");

    public static T Nonnull<T>(T? value, string? message = null) {
      if (value == null) {
        Assert.Fail(message ?? "Expected value to be nonnull!");
      }
      return value!;
    }
  }
}