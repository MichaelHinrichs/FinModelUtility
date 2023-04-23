namespace UoT.memory {
  public class Segment {
    public required uint Offset { get; init; }
    public required uint Length { get; init; }
  }

  public static class Segments {
    public static Segment GAMEPLAY_KEEP { get; set; }
    public static Segment GAMEPLAY_FIELD_KEEP { get; set; }
  }
}
