using System.Collections;
using System.Collections.Generic;


namespace fin.data {
  public class CounterArray : IEnumerable<int> {
    private readonly List<int> impl_ = new();

    public int Count => this.impl_.Count;

    public void Clear() => this.impl_.Clear();

    public int Increment(int index) {
      while (this.impl_.Count < index + 1) {
        this.impl_.Add(0);
      }

      return this.impl_[index] += 1;
    }

    public int this[int index] => this.impl_[index];

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<int> GetEnumerator() => this.impl_.GetEnumerator();
  }
}