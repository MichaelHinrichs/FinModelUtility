using System;
using System.Collections.Generic;


namespace fin.data {
  public class ShuffledListView<T> {
    private readonly IReadOnlyList<T> impl_;

    public ShuffledListView(IReadOnlyList<T> impl) {
      this.impl_ = impl;
    }

    // TODO: Implement an algorithm that "feels more random"
    public bool TryGetNext(out T value) {
      var count = this.impl_.Count;

      if (count == 0) {
        value = default;
        return false;
      }

      value = this.impl_[Random.Shared.Next(count)];
      return true;
    } 
  }
}