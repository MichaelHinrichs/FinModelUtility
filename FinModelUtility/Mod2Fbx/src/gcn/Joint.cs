﻿using System.Collections.Generic;
using System.IO;

using schema;

namespace mod.gcn {
  [Schema]
  public partial class JointMatPoly : IGcnSerializable {
    public ushort matIdx = 0;
    public ushort meshIdx = 0;
  }

  [Schema]
  public partial class Joint : IGcnSerializable {
    public uint parentIdx = 0;
    public uint flags = 0;
    public readonly Vector3f boundsMax = new();    
    public readonly Vector3f boundsMin = new();
    public float volumeRadius = 0;
    public readonly Vector3f scale = new();
    public readonly Vector3f rotation = new();
    public readonly Vector3f position = new();

    [ArrayLengthSource(IntType.UINT32)]
    public JointMatPoly[] matpolys;
  }
}