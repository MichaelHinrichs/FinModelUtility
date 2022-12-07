using System;
using System.IO;
using System.Threading.Tasks;


namespace schema.attributes.memory {
  /// <summary>
  ///   Pointer that encodes the relative difference between some address and
  ///   the start of the containing stream.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class PointerToAttribute : BMemberAttribute {
    private readonly string otherMemberName_;

    public PointerToAttribute(string otherMemberName) {
      this.otherMemberName_ = otherMemberName;
    }

    protected override void InitFields() {
      this.TypeChainToOtherMember =
          this.GetTypeChainRelativeToStructure(
              this.otherMemberName_, false);
    }

    public ITypeChain TypeChainToOtherMember { get; private set; }
  }


  /// <summary>
  ///   Attribute that specifies that the current data is located in some block.
  ///   Reads/writes some immediate value that represents the relative offset
  ///   compared to that block.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class IndirectViaBlock : BMemberAttribute {
    private readonly string blockFieldName_;
    private readonly string? relativeToPointerFieldName_;

    public IndirectViaBlock(SchemaIntegerType immediateType,
                            string blockField,
                            string? relativeToPointerField = null) {
      this.ImmediateType = immediateType;
      this.blockFieldName_ = blockField;
      this.relativeToPointerFieldName_ = relativeToPointerField;
    }

    protected override void InitFields() {
      this.BlockField =
          GetMemberRelativeToStructure<IBlock>(this.blockFieldName_);
      if (this.relativeToPointerFieldName_ != null) {
        this.RelativeToPointerField =
            GetMemberRelativeToStructure<IOffset>(
                this.relativeToPointerFieldName_);
      }
    }

    public IMemberReference<IBlock> BlockField { get; private set; }

    public IMemberReference<IOffset> RelativeToPointerField {
      get;
      private set;
    }

    public SchemaIntegerType ImmediateType { get; }
  }


  public interface IOffset { }

  public interface IOffset<out TPosition> : IOffset {
    TPosition AbsolutePosition { get; }
    TPosition RelativePosition { get; }
  }

  public interface IReadOffset : IOffset<long> { }

  public interface IWriteOffset : IOffset<Task<long>> { }



  public interface IBlock { }

  public interface IReadBlock : IBlock, IReadOffset { }

  /// <summary>
  ///   Interface for writing arbitrary data into a block of memory. This can
  ///   be written into at any point before the stream is completed and
  ///   flushed, allowing data to be written to it from other classes.
  /// </summary>
  public interface IWriteBlock : IBlock, IWriteOffset {
    ISubEndianBinaryWriter EndianBinaryWriter { get; }
  }
}