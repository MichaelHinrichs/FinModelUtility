using System;
using System.Threading.Tasks;


namespace schema.memory {
  /// <summary>
  ///   Interface for writing arbitrary data into a block of memory. This can
  ///   be written into at any point before the stream is completed and
  ///   flushed, allowing data to be written to it from other classes.
  /// </summary>
  public interface IWriteBlock {
    Task<long> GetDelayedPositionOfWholeBlock();
    Task<long> GetDelayedLengthOfWholeBlock();

    (Task<long> DelayedPosition, Task<long> DelayedLength) AddIntoBlock(
        Action handler);
  }
}