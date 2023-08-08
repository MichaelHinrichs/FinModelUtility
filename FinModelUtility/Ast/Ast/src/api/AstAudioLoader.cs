using ast.schema;
using fin.ui.audio;
using fin.io;

using schema.binary;


namespace ast.api {
  public class AstAudioFileBundle : IAudioFileBundle {
    public required string GameName { get; init; }

    public IFileHierarchyFile MainFile => this.AstFile;

    public required IFileHierarchyFile AstFile { get; init; }
  }

  public class AstAudioLoader : IAudioLoader<AstAudioFileBundle> {
    public IAudioBuffer<short> LoadAudio(
        IAudioManager<short> audioManager,
        AstAudioFileBundle audioFileBundle) {
      var astFile = audioFileBundle.AstFile;
      var ast = astFile.ReadNew<Ast>(Endianness.BigEndian);

      var mutableBuffer = audioManager.CreateMutableBuffer();

      mutableBuffer.Frequency = (int)ast.StrmHeader.SampleRate;

      var channelData =
          ast.ChannelData.Select(data => data.ToArray()).ToArray();
      mutableBuffer.SetPcm(channelData);

      return mutableBuffer;
    }
  }
}