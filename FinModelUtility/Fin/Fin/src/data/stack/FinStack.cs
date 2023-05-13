using System.Collections.Generic;

namespace fin.data.stack {
  public interface IStack<T> {
    int Count { get; }
    T Top { get; set; }

    void Clear();

    bool TryPop(out T item);
    T Pop();
    void Push(T item);
  }

  public class FinStack<T> : IStack<T> {
    private readonly Stack<T> impl_;

    public FinStack() {
      this.impl_ = new Stack<T>();
    }

    public FinStack(T item) {
      this.impl_ = new Stack<T>();
      this.impl_.Push(item);
    }

    public FinStack(IEnumerable<T> items) {
      this.impl_ = new Stack<T>(items);
    }

    public int Count => this.impl_.Count;

    public T Top {
      get => this.impl_.Peek();
      set {
        this.impl_.Pop();
        this.impl_.Push(value);
      }
    }

    public void Clear() => this.impl_.Clear();

    public bool TryPop(out T item) => this.impl_.TryPop(out item);
    public T Pop() => this.impl_.Pop();
    public void Push(T item) => this.impl_.Push(item);
  }
}