using schema.util;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace schema.io {
  public interface IOutOfOrderDictionary<in TKey, TValue>
      where TKey : notnull {
    void AssertAllCompleted();

    void Clear();

    Task<TValue> Get(TKey key);
    void Set(TKey key, TValue value);
  }

  public class OutOfOrderDictionary<TKey, TValue>
      : IOutOfOrderDictionary<TKey, TValue> {
    private readonly Dictionary<TKey, TaskCompletionSource<TValue>> impl_ =
        new();

    public void AssertAllCompleted() {
      var incompleted = new List<TKey>();
      foreach (var pair in this.impl_) {
        if (!pair.Value.Task.IsCompleted) {
          incompleted.Add(pair.Key);
        }
      }

      if (incompleted.Count > 0) {
        Asserts.Fail(
            $"Expected for all keys in the out-of-order dictionary to be provided values, but still has {incompleted.Count}/{this.impl_.Count
            } waiting!");
      }
    }

    public void Clear() => this.impl_.Clear();


    public Task<TValue> Get(TKey key)
      => this.GetOrCreateTaskCompletionSource_(key).Task;

    public void Set(TKey key, TValue value)
      => this.GetOrCreateTaskCompletionSource_(key).SetResult(value);

    public void Set(TKey key, Task<TValue> valueTask) {
      var taskCompletionSource = this.GetOrCreateTaskCompletionSource_(key);

      Task.Run(async () => {
        var value = await valueTask;
        taskCompletionSource.SetResult(value);
      });
    }

    private TaskCompletionSource<TValue> GetOrCreateTaskCompletionSource_(
        TKey key) {
      if (!this.impl_.TryGetValue(key, out var taskCompletionSource)) {
        taskCompletionSource = new TaskCompletionSource<TValue>();
        this.impl_[key] = taskCompletionSource;
      }

      return taskCompletionSource;
    }
  }
}