using System;
using System.Collections;
using System.Collections.Generic;

namespace UoT.util.array {
  public interface IIndexable<T> : IList<T> {
    //int Count { get; }
    //byte this[int index] { get; set; }
  }

  // TODO: Delete this, use records instead.
  public abstract class BIndexable : IIndexable<byte> {
    public abstract int Count { get; }
    public abstract byte this[int index] { get; set; }


    // TODO: Get rid of all this crap.
    public IEnumerator<byte> GetEnumerator()
      => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator()
      => throw new NotImplementedException();

    public void Add(byte item) => throw new NotImplementedException();

    public void Clear() {
      throw new NotImplementedException();
    }

    public bool Contains(byte item) {
      throw new NotImplementedException();
    }

    public void CopyTo(byte[] array, int arrayIndex) {
      throw new NotImplementedException();
    }

    public bool Remove(byte item) {
      throw new NotImplementedException();
    }

    public bool IsReadOnly => false;

    public int IndexOf(byte item) {
      throw new NotImplementedException();
    }

    public void Insert(int index, byte item) {
      throw new NotImplementedException();
    }

    public void RemoveAt(int index) {
      throw new NotImplementedException();
    }
  }
}
