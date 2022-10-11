using ast.schema;
using fin.audio;
using fin.io;


namespace ast.api {
  public class AstAudioFileBundle : IAudioFileBundle {
    public AstAudioFileBundle(IFileHierarchyFile astFile) {
      this.AstFile = astFile;
    }

    public IFileHierarchyFile MainFile => this.AstFile;

    public IFileHierarchyFile AstFile { get; }
  }

  public class AstAudioLoader : IAudioLoader<AstAudioFileBundle> {
    public IAudioBuffer<short> LoadAudio(
        IAudioManager<short> audioManager,
        AstAudioFileBundle audioFileBundle) {
      var astFile = audioFileBundle.AstFile;
      var ast = astFile.Impl.ReadNew<Ast>(Endianness.BigEndian);

      var mutableBuffer = audioManager.CreateMutableBuffer();

      mutableBuffer.Frequency = (int)ast.StrmHeader.SampleRate;

      var channelData =
          ast.ChannelData.Select(data => data.ToArray()).ToArray();
      mutableBuffer.SetPcm(channelData);

      return mutableBuffer;
    }
  }
}