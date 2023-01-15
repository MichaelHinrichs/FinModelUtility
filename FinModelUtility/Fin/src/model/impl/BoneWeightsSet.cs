using System.Collections.Generic;

namespace fin.model.impl {
  public class BoneWeightsSet {
    private Dictionary<int, IBoneWeights> boneWeightsByHashcode_ = new();

    public void Add(IBoneWeights boneWeights)
      => this.boneWeightsByHashcode_[boneWeights.GetHashCode()] = boneWeights;

    public bool TryGetExisting(
      PreprojectMode preprojectMode,
      IReadOnlyList<IBoneWeight> weights,
      out IBoneWeights boneWeights) {
      var hashcode = GetHashCode(preprojectMode, weights);
      return this.boneWeightsByHashcode_.TryGetValue(hashcode, out boneWeights);
    }

    public static int GetHashCode(PreprojectMode preprojectMode, IReadOnlyList<IBoneWeight> weights) {
      int hash = 216613626;
      var sub = 16780669;
      hash = hash * sub ^ preprojectMode.GetHashCode();
      foreach (var weight in weights) {
        hash = hash * sub ^ weight.GetHashCode();
      }
      return hash;
    }
  }
}
