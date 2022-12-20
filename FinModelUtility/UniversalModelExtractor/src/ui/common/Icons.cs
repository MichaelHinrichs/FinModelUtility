using fin.io;
using System.Reflection;


namespace uni.ui.common {
  public static class Icons {
    private static readonly Assembly assembly_ =
        Assembly.GetExecutingAssembly();

    public static readonly Image folderClosedImage =
        EmbeddedResourceUtil.Load(Icons.assembly_,
                                  "uni.img.folder_closed.png");

    public static readonly Image folderOpenImage =
        EmbeddedResourceUtil.Load(Icons.assembly_,
                                  "uni.img.folder_open.png");

    public static readonly Image modelImage =
        EmbeddedResourceUtil.Load(Icons.assembly_, "uni.img.model.png");

    public static readonly Image sceneImage =
        EmbeddedResourceUtil.Load(Icons.assembly_, "uni.img.scene.png");

    public static readonly Image musicImage =
        EmbeddedResourceUtil.Load(Icons.assembly_, "uni.img.music.png");
  }
}