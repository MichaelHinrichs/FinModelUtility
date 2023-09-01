using ast.schema;

using fin.audio;

using schema.binary;

namespace ast.api {
  public class AstAudioReader : IAudioReader<AstAudioFileBundle> {
    public IAudioBuffer<short> ReadAudio(
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