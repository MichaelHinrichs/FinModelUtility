namespace UoT {
  public interface ILimb {
    bool Visible { get; }


    // TODO: Merge these into a single field.
    short X { get; }
    short Y { get; }
    short Z { get; }


    /// <summary>
    ///   Display list.
    /// </summary>
    IDisplayList Dl { get; }

    /// <summary>
    ///   Far display list used for LOD. Only used for Link.
    /// </summary>
    IDisplayList FarDl { get; }

    ILimb FirstChild { get; }
    ILimb NextSibling { get; }
  }
}
