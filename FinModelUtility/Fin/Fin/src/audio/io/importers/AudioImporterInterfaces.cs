namespace fin.audio.io.importers {
  public interface IAudioImporter<in TAudioFileBundle>
      where TAudioFileBundle : IAudioFileBundle {
    IAudioBuffer<short> ImportAudio(
        IAudioManager<short> audioManager,
        TAudioFileBundle audioFileBundle);
  }
}