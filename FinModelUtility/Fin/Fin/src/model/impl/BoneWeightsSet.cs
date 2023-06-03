﻿using System.Collections.Generic;

using fin.util.hash;

namespace fin.model.impl {
  public class BoneWeightsSet {
    private Dictionary<int, IBoneWeights> boneWeightsByHashcode_ = new();

    public void Add(IBoneWeights boneWeights)
      => this.boneWeightsByHashcode_[boneWeights.GetHashCode()] = boneWeights;

    public bool TryGetExisting(
        VertexSpace vertexSpace,
        IReadOnlyList<IBoneWeight> weights,
        out IBoneWeights boneWeights) {
      var hashcode = GetHashCode(vertexSpace, weights);
      return this.boneWeightsByHashcode_.TryGetValue(hashcode, out boneWeights);
    }

    public static int GetHashCode(VertexSpace vertexSpace,
                                  IReadOnlyList<IBoneWeight> weights) {
      var hash = FluentHash.Start().With(vertexSpace);
      foreach (var weight in weights) {
        hash = hash.With(weight);
      }

      return hash;
    }
  }
}