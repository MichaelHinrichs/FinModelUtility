namespace mod.gcn.animation {
  public interface IAnm {

  }

  public interface IDcx {
    uint JointCount { get; }
    uint FrameCount { get; }
  }
}

/**
DCA

Joint Count - u32
Frame Count - u32

Scaling data chunk 
  Size - u32
  value - float

Rotation data chunk 
  Size - u32
  value - float

Position (translation) data chunk 
  Size - u32
  value - float

Joint Chunk (C++ pseudocode)

  for ( i = 0; i < this->m_numJoints; ++i )
  {
    this->m_info[i].m_jointIdx = (reader.readInt)(a2);
    u32 parentIdx = reader.readInt();
    for ( j = 0; j < 3; ++j )
    {
      scaleparam = &this->m_info[i].m_scaleParams[j];
      scaleparam->m_x = reader.readInt();
      scaleparam->m_y = reader.readInt();
    }
    for ( k = 0; k < 3; ++k )
    {
      rotparam = &this->m_info[i].m_rotationParams[k];
      rotparam->m_x = reader.readInt();
      rotparam->m_y = reader.readInt();
    }
    for ( l = 0; l < 3; ++l )
    {
      posparam = &this->m_info[i].m_positionParams[l];
      posparam->m_x = reader.readInt();
      posparam->m_y = reader.readInt();
    }
  }
  
DCK

Joint Count - u32
Frame Count - u32

Scaling data chunk 
  Size - u32
  value - float

Rotation data chunk 
  Size - u32
  value - float

Position (translation) data chunk 
  Size - u32
  value - float

Joint Chunk (C++ pseudocode)

  for ( i = 0; i < this->m_numJoints; ++i )
  {
    this->m_info[i].m_jointIdx = (reader.readInt)(a2);
    u32 parentIdx = reader.readInt();
    for ( j = 0; j < 3; ++j )
    {
      scaleparam = &this->m_info[i].m_scaleParams[j];
      scaleparam->m_x = reader.readInt();
      scaleparam->m_y = reader.readInt();
      scaleparam->m_z = reader.readInt();
    }
    for ( k = 0; k < 3; ++k )
    {
      rotparam = &this->m_info[i].m_rotationParams[k];
      rotparam->m_x = reader.readInt();
      rotparam->m_y = reader.readInt();
      rotparam->m_z = reader.readInt();
    }
    for ( l = 0; l < 3; ++l )
    {
      posparam = &this->m_info[i].m_positionParams[l];
      posparam->m_x = reader.readInt();
      posparam->m_y = reader.readInt();
      posparam->m_z = reader.readInt();
    }
  }
 */