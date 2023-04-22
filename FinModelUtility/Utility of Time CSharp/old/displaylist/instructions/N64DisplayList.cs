namespace UoT {
  public class N64DisplayList : IDisplayList {
    public bool Skip;
    public ZSegment StartPos;
    public ZSegment EndPos;
    public int CommandCount;

    // Picker
    public bool Highlight { get; set; }
    public Color3UByte PickCol { get; set; }

    public IDisplayListInstruction[]? Commands { get; set; }
  }
}