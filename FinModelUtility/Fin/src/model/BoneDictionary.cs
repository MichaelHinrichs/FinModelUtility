using System.Collections.Generic;

using fin.util.optional;


namespace fin.model {
  public class IBoneDictionaryValue<T> {
    IBone Bone { get; }
    Optional<T> Value { get; }
  }

  public class BoneDictionary<T> {
    private readonly List<BoneDictionaryValue<T>> impl_ = new();

    public T this[IBone bone] {
      get => this.impl_[bone.Id].Value.Assert();
      set {
        var id = bone.Id;

        while (this.impl_.Count < id + 1) {
          this.impl_.Add(new BoneDictionaryValue<T> {
              Bone = null,
              Value = Optional<T>.None(),
          });
        }

        var current = this.impl_[bone.Id];
        current.Bone = bone;
        current.Value = Optional<T>.Of(value);
      }
    }

    public bool TryGet(IBone bone, out T value) {
      var id = bone.Id;

      if (this.impl_.Count < id + 1) {
        value = default;
        return false;
      }

      var boneDictionaryValue = this.impl_[id];
      return boneDictionaryValue.Value.Try(out value);
    }

    private class BoneDictionaryValue<T> : IBoneDictionaryValue<T> {
      public IBone Bone { get; set; }
      public Optional<T> Value { get; set; }
    }
  }
}