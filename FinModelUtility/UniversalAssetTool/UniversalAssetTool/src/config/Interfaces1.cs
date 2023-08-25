using System.Text.RegularExpressions;

namespace uni.config {
  public interface IExtractorConfig {
    IExtractorHeader Header { get; }
    IList<IExtractorAction> Actions { get; }
  }

  public interface IExtractorHeader {
    string GameName { get; }
  }

  public interface IExtractorAction { }

  public interface IInPlaceExtractorAction : IExtractorAction { }


  public interface IForEachMatchingDirectory : IExtractorAction {
    IList<IExtractorAction> Actions { get; }
  }

  public interface IForEachMatchingFile : IExtractorAction {
    IList<IInPlaceExtractorAction> Actions { get; }
  }

  public interface ISplitFiles : IExtractorAction {
    IList<ISplitFileAction> For { get; }
    IList<IInPlaceExtractorAction> ForRest { get; }
  }

  public interface ISplitFileAction {
    Regex Paths { get; }
    IList<IInPlaceExtractorAction> Actions { get; }
  }


  public enum RomType {
    GCM,
    CIA
  }

  public interface IExtractRomFilesystem : IInPlaceExtractorAction {
    RomType Type { get; }
    Regex RomPath { get; }
    string ScratchPath { get; }
  }
}