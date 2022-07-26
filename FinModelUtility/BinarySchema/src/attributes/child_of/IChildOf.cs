namespace schema.attributes.child_of {
  public interface IChildOf<TParent> where TParent : IBiSerializable {
    public TParent Parent { get; set; }
  }
}