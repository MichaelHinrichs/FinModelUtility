namespace fin.model.impl {
  // TODO: Add logic for optimizing the model.
  public partial class ModelImpl : IModel {
    public ModelImpl() {
      this.Skin = new SkinImpl();
    }

    // TODO: Rewrite this to take in options instead.
    public ModelImpl(int vertexCount) {
      this.Skin = new SkinImpl(vertexCount);
    }
  }
}