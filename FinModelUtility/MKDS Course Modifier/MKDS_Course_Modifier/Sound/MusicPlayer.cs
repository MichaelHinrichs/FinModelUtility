// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.MusicPlayer
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using DirectMidi;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class MusicPlayer
  {
    private CDirectMusic CDMusic = new CDirectMusic();
    private CDLSLoader CLoader = new CDLSLoader();
    private COutputPort COutPort = new COutputPort();
    private CAPathPerformance CAPathPerformance = new CAPathPerformance();
    private CCollection CCollectionA = new CCollection();
    private CSegment CSegment1 = new CSegment();
    private bool UseDLS = false;
    private string DLS = "";
    private string Midi = "";
    private bool useLoop = false;
    private INFOPORT PortInfo;
    private int LoopStart;
    private int LoopEnd;
    private int times;

    public MusicPlayer()
    {
      this.CDMusic.Initialize();
      this.CAPathPerformance.Initialize(this.CDMusic, (object) null, (Control) null, DMUS_APATH.DYNAMIC_STEREO, 128U);
      this.CLoader.Initialize();
      this.COutPort.Initialize(this.CDMusic);
    }

    public void SetDLS(string Path)
    {
      if (this.DLS != "")
      {
        File.Delete(this.DLS);
        this.DLS = "";
      }
      this.DLS = Path;
      this.UseDLS = true;
    }

    public void UnloadDLS()
    {
      this.UseDLS = false;
    }

    public void SetMidi(string Path)
    {
      this.Midi = Path;
    }

    public int GetLength()
    {
      if (this.CAPathPerformance != null)
        this.CAPathPerformance.StopAll();
      this.CSegment1.ReleaseSegment();
      this.CSegment1.UnloadAllPerformances();
      this.CLoader.UnloadCollection(this.CCollectionA);
      this.CLoader.LoadSegment(this.Midi, out this.CSegment1, true);
      if (this.UseDLS)
      {
        this.CLoader.LoadDLS(this.DLS, out this.CCollectionA);
        this.CSegment1.ConnectToDLS(this.CCollectionA);
      }
      this.CSegment1.Download((CPerformance) this.CAPathPerformance);
      int pmtLength;
      this.CSegment1.GetLength(out pmtLength);
      return pmtLength;
    }

    public void Play()
    {
      if (this.CAPathPerformance != null)
        this.CAPathPerformance.StopAll();
      this.CSegment1.ReleaseSegment();
      this.CSegment1.UnloadAllPerformances();
      this.CLoader.UnloadCollection(this.CCollectionA);
      this.CLoader.LoadSegment(this.Midi, out this.CSegment1, true);
      if (this.UseDLS)
      {
        this.CLoader.LoadDLS(this.DLS, out this.CCollectionA);
        this.CSegment1.ConnectToDLS(this.CCollectionA);
        this.CLoader.UnloadCollection(this.CCollectionA);
        this.CCollectionA.Dispose();
        this.CCollectionA = new CCollection();
      }
      this.CSegment1.Download((CPerformance) this.CAPathPerformance);
      if (this.useLoop)
      {
        this.CSegment1.SetLoopPoints(this.LoopStart, this.LoopEnd);
        if (this.times != -1)
          this.CSegment1.SetRepeats((uint) this.times);
        else
          this.CSegment1.SetRepeats(DMUS_SEG.REPEAT_INFINITE);
      }
      this.CAPathPerformance.PlaySegment((ISegment) this.CSegment1, (IAudioPath) null);
    }

    public void SetLoop(int Start, int End, int times)
    {
      this.useLoop = true;
      this.LoopEnd = End;
      this.times = times;
      this.LoopStart = Start;
    }

    public void Unload()
    {
      this.useLoop = false;
      this.Midi = "";
      this.times = -1;
      if (this.CAPathPerformance != null)
        this.CAPathPerformance.StopAll();
      this.CSegment1.ReleaseSegment();
      this.CSegment1.UnloadAllPerformances();
      this.CSegment1.Dispose();
      this.CLoader.Dispose();
      this.COutPort.ReleasePort();
      this.CSegment1 = (CSegment) null;
      this.CLoader = (CDLSLoader) null;
      this.CSegment1 = new CSegment();
      this.CLoader = new CDLSLoader();
      this.CLoader.Initialize();
      this.UseDLS = false;
    }

    public void Stop()
    {
      this.CAPathPerformance.StopAll();
    }
  }
}
