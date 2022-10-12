using System;
using System.Collections.Generic;


namespace fin.data {
  public class ShuffledListView<T> {
    private readonly IReadOnlyList<T> impl_;

    public ShuffledListView(IReadOnlyList<T> impl) {
      this.impl_ = impl;
    }

    // TODO: Implement an algorithm that "feels more random"
    public T Next() => this.impl_[Random.Shared.Next(this.impl_.Count)];
  }
}