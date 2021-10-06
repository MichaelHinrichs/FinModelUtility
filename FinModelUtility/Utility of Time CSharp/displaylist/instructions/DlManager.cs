using System.Collections;
using System.Collections.Generic;

namespace UoT {
  /// <summary>
  ///   Class that manages a group of display lists. Parses models/animations,
  ///   creates textures, renders.
  /// </summary>
  public class DlManager : IDisplayListManager {
    private IList<N64DisplayList> impl_ = new List<N64DisplayList>();

    // TODO: Make this private.
    public bool HasLimbs { get; set; } = false;

    public void DoSomething() {}

    /**
     * Display list creation
     */
    public int Count => this.impl_.Count;

    public void Clear() {
      this.HasLimbs = false;
      this.impl_.Clear();
    }

    public IEnumerator<N64DisplayList> GetEnumerator() 
      => this.impl_.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public void Add(N64DisplayList displayList) {
      this.impl_.Add(displayList);
    }

    public N64DisplayList GetDisplayListByIndex(int index) 
      => this.impl_[index];

    public int GetIndexByAddress(uint address) {
      IoUtil.SplitAddress(address, out var bank, out var offset);
      
      for (var i = 0; i < this.impl_.Count; ++i) {
        var zSegment = this.impl_[i].StartPos;
        if (zSegment.Bank == bank && zSegment.Offset == offset) {
          return i;
        }
      }

      return -1;
    }
  }
}