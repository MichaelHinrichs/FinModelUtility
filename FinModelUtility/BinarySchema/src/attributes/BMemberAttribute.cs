using System;
using Microsoft.CodeAnalysis;
using schema.parser;
using schema.util;
using System.Collections.Generic;


namespace schema.attributes {
  public abstract class BMemberAttribute<T> : BMemberAttribute {
    protected override void SetMemberFromName(string memberName) {
      this.memberThisIsAttachedTo_ =
          this.GetMemberRelativeToStructure<T>(memberName);
    }
  }

  public abstract class BMemberAttribute : Attribute {
    private static readonly TypeInfoParser parser_ = new();
    private IList<Diagnostic> diagnostics_;

    private ITypeInfo structureTypeInfo_;
    protected IMemberReference memberThisIsAttachedTo_;

    protected abstract void InitFields();

    protected virtual void SetMemberFromName(string memberName) {
      this.memberThisIsAttachedTo_ =
          this.GetMemberRelativeToStructure(memberName);
    }


    public void Init(
        IList<Diagnostic> diagnostics,
        ITypeSymbol structureTypeSymbol,
        string memberName) {
      this.diagnostics_ = diagnostics;
      this.structureTypeInfo_ = BMemberAttribute.parser_.AssertParseTypeSymbol(
          structureTypeSymbol);
      this.SetMemberFromName(memberName);
      this.InitFields();
    }


    protected IMemberReference GetMemberRelativeToStructure(
        string memberName) {
      var otherMemberTypeSymbol =
          SymbolTypeUtil.GetTypeFromMember(
              this.diagnostics_,
              this.structureTypeInfo_.TypeSymbol,
              memberName);
      var otherMemberTypeInfo = BMemberAttribute.parser_.AssertParseTypeSymbol(
          otherMemberTypeSymbol.TypeSymbol);
      return new MemberReference(memberName,
                                 this.structureTypeInfo_,
                                 otherMemberTypeSymbol.MemberSymbol,
                                 otherMemberTypeInfo);
    }


    protected IMemberReference<T> GetMemberRelativeToStructure<T>(
        string memberName) {
      var memberTypeSymbol =
          SymbolTypeUtil.GetTypeFromMember(
              this.diagnostics_,
              this.structureTypeInfo_.TypeSymbol,
              memberName);
      var memberTypeInfo = BMemberAttribute.parser_.AssertParseTypeSymbol(
          memberTypeSymbol.TypeSymbol);

      if (!SymbolTypeUtil.CanBeStoredAs(memberTypeSymbol.TypeSymbol,
                                        typeof(T))) {
        Asserts.Fail(
            $"Type of member, {memberTypeInfo.TypeSymbol}, does not match expected type: {typeof(T)}");
      }

      return new MemberReference<T>(
          memberName,
          this.structureTypeInfo_,
          memberTypeSymbol.MemberSymbol,
          memberTypeInfo);
    }

    protected IMemberReference GetOtherMemberRelativeToStructure(
        string otherMemberName) {
      var memberTypeSymbol =
          SymbolTypeUtil.GetTypeFromMemberRelativeToAnother(
              this.diagnostics_,
              this.structureTypeInfo_.TypeSymbol,
              otherMemberName,
              this.memberThisIsAttachedTo_.Name);
      var memberTypeInfo = BMemberAttribute.parser_.AssertParseTypeSymbol(
          memberTypeSymbol.TypeSymbol);

      return new MemberReference(
          otherMemberName,
          this.structureTypeInfo_,
          memberTypeSymbol.MemberSymbol,
          memberTypeInfo);
    }

    protected IMemberReference<T> GetOtherMemberRelativeToStructure<T>(
        string otherMemberName) {
      var memberTypeSymbol =
          SymbolTypeUtil.GetTypeFromMemberRelativeToAnother(
              this.diagnostics_,
              this.structureTypeInfo_.TypeSymbol,
              otherMemberName,
              this.memberThisIsAttachedTo_.Name);
      var memberTypeInfo = BMemberAttribute.parser_.AssertParseTypeSymbol(
          memberTypeSymbol.TypeSymbol);

      if (!SymbolTypeUtil.CanBeStoredAs(memberTypeSymbol.TypeSymbol,
                                        typeof(T))) {
        Asserts.Fail(
            $"Type of other member, {memberTypeInfo.TypeSymbol}, does not match expected type: {typeof(T)}");
      }

      return new MemberReference<T>(
          otherMemberName,
          this.structureTypeInfo_,
          memberTypeSymbol.MemberSymbol,
          memberTypeInfo);
    }

    protected IMemberReference GetSourceRelativeToStructure(
        string otherMemberName) {
      var source = this.GetOtherMemberRelativeToStructure(otherMemberName);

      if (!IsMemberWritePrivate_(source.MemberSymbol)) {
        this.diagnostics_.Add(
            Rules.CreateDiagnostic(source.MemberSymbol,
                                   Rules.SourceMustBePrivate));
      }

      return source;
    }

    protected IMemberReference<T> GetSourceRelativeToStructure<T>(
        string otherMemberName) {
      var source = this.GetOtherMemberRelativeToStructure<T>(otherMemberName);

      if (!IsMemberWritePrivate_(source.MemberSymbol)) {
        this.diagnostics_.Add(
            Rules.CreateDiagnostic(source.MemberSymbol,
                                   Rules.SourceMustBePrivate));
      }

      return source;
    }

    private bool IsMemberWritePrivate_(ISymbol symbol)
      => symbol switch {
          IPropertySymbol propertySymbol
              => (propertySymbol.SetMethod
                                ?.DeclaredAccessibility ??
                  Accessibility.Private) == Accessibility.Private,
          IFieldSymbol fieldSymbol
              => fieldSymbol.DeclaredAccessibility == Accessibility.Private,
      };
  }


  public interface IMemberReference {
    string Name { get; }
    ITypeInfo StructureTypeInfo { get; }
    ISymbol MemberSymbol { get; }
    ITypeInfo MemberTypeInfo { get; }

    bool IsInteger { get; }
    IMemberReference AssertIsInteger();

    bool IsBool { get; }
    IMemberReference AssertIsBool();
  }

  public interface IMemberReference<T> : IMemberReference { }


  public class MemberReference : IMemberReference {
    public MemberReference(
        string name,
        ITypeInfo structureTypeInfo,
        ISymbol memberSymbol,
        ITypeInfo memberTypeInfo) {
      this.Name = name;
      this.StructureTypeInfo = structureTypeInfo;
      this.MemberSymbol = memberSymbol;
      this.MemberTypeInfo = memberTypeInfo;
    }

    public string Name { get; }
    public ITypeInfo StructureTypeInfo { get; }
    public ISymbol MemberSymbol { get; }
    public ITypeInfo MemberTypeInfo { get; }

    public bool IsInteger => this.MemberTypeInfo is IIntegerTypeInfo;

    public IMemberReference AssertIsInteger() {
      if (!this.IsInteger) {
        Asserts.Fail($"Expected {this.Name} to refer to an integer!");
      }
      return this;
    }

    public bool IsBool => this.MemberTypeInfo is IBoolTypeInfo;

    public IMemberReference AssertIsBool() {
      if (!this.IsBool) {
        Asserts.Fail($"Expected {this.Name} to refer to an bool!");
      }
      return this;
    }
  }

  public class MemberReference<T> : MemberReference, IMemberReference<T> {
    public MemberReference(
        string name,
        ITypeInfo structureTypeInfo,
        ISymbol memberSymbol,
        ITypeInfo memberTypeInfo)
        : base(name,
               structureTypeInfo,
               memberSymbol,
               memberTypeInfo) { }
  }
}