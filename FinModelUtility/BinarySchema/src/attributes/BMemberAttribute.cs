using System;

using asserts;

using Microsoft.CodeAnalysis;

using schema.parser;


namespace schema.attributes {
  public abstract class BMemberAttribute : Attribute {
    private static readonly TypeInfoParser parser_ = new();

    private ITypeInfo structureTypeInfo_;
    private IMemberReference memberThisIsAttachedTo_;

    protected abstract void InitFields();


    public void Init(ITypeInfo structureTypeInfo,
                     string memberName) {
      this.structureTypeInfo_ = structureTypeInfo;
      this.memberThisIsAttachedTo_ =
          this.GetMemberRelativeToStructure(memberName);
      this.InitFields();
    }

    public void Init(ITypeSymbol structureTypeSymbol,
                     string memberName) {
      this.structureTypeInfo_ = BMemberAttribute.parser_.AssertParseTypeSymbol(
          structureTypeSymbol);
      this.memberThisIsAttachedTo_ =
          this.GetMemberRelativeToStructure(memberName);
      this.InitFields();
    }

    public void Init(IMemberReference memberThisIsAttachedTo) {
      this.structureTypeInfo_ = memberThisIsAttachedTo.StructureTypeInfo;
      this.memberThisIsAttachedTo_ = memberThisIsAttachedTo;
      this.InitFields();
    }


    protected IMemberReference GetMemberRelativeToStructure(
        string memberName) {
      var memberTypeSymbol =
          SymbolTypeUtil.GetTypeFromMember(
              this.structureTypeInfo_.TypeSymbol,
              memberName);
      var memberTypeInfo = BMemberAttribute.parser_.AssertParseTypeSymbol(
          memberTypeSymbol);
      return new MemberReference(memberName,
                                 this.structureTypeInfo_,
                                 memberTypeInfo);
    }


    protected IMemberReference<T> GetMemberRelativeToStructure<T>(
        string memberName) {
      var memberTypeSymbol =
          SymbolTypeUtil.GetTypeFromMember(this.structureTypeInfo_.TypeSymbol,
                                           memberName);
      var memberTypeInfo = BMemberAttribute.parser_.AssertParseTypeSymbol(
          memberTypeSymbol);

      // TODO: Assert type

      return new MemberReference<T>(
          memberName,
          this.structureTypeInfo_,
          memberTypeInfo);
    }
  }


  public interface IMemberReference {
    string Name { get; }
    ITypeInfo StructureTypeInfo { get; }
    ITypeInfo MemberTypeInfo { get; }

    bool IsInteger { get; }
    IMemberReference AssertIsInteger();
  }

  public interface IMemberReference<T> : IMemberReference { }


  public class MemberReference : IMemberReference {
    public MemberReference(
        string name,
        ITypeInfo structureTypeInfo,
        ITypeInfo memberTypeInfo) {
      this.Name = name;
      this.StructureTypeInfo = structureTypeInfo;
      this.MemberTypeInfo = memberTypeInfo;
    }

    public string Name { get; }
    public ITypeInfo StructureTypeInfo { get; }
    public ITypeInfo MemberTypeInfo { get; }

    public bool IsInteger => this.MemberTypeInfo is IIntegerTypeInfo;

    public IMemberReference AssertIsInteger() {
      if (!this.IsInteger) {
        Asserts.Fail($"Expected {this.Name} to refer to an integer!");
      }
      return this;
    }
  }

  public class MemberReference<T> : MemberReference, IMemberReference<T> {
    public MemberReference(
        string name,
        ITypeInfo structureTypeInfo,
        ITypeInfo memberTypeInfo)
        : base(name,
               structureTypeInfo,
               memberTypeInfo) { }
  }
}