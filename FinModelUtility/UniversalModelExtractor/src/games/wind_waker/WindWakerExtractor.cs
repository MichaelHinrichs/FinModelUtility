using bmd.api;

using fin.io;
using fin.log;
using fin.util.asserts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using fin.util.data;
using fin.util.strings;

using uni.msg;
using uni.platforms;
using uni.platforms.gcn;
using uni.platforms.gcn.tools;
using uni.util.io;

namespace uni.games.wind_waker {
  public class WindWakerExtractor {
    private readonly ILogger logger_ =
        Logging.Create<WindWakerExtractor>();

    public void ExtractAll() {
      var windWakerRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "wind_waker.gcm");

      var options = GcnFileHierarchyExtractor.Options.Standard();
      var fileHierarchy =
          new GcnFileHierarchyExtractor()
              .ExtractFromRom(options, windWakerRom);

      var objectDirectory = fileHierarchy.Root.TryToGetSubdir(@"res\Object");
      {
        var yaz0Dec = new Yaz0Dec();
        var didDecompress = false;
        foreach (var arcFile in objectDirectory.FilesWithExtension(".arc")) {
          didDecompress |= yaz0Dec.Run(arcFile, true);
        }

        if (didDecompress) {
          objectDirectory.Refresh();
        }

        var rarcDump = new RarcDump();
        var didDump = false;
        foreach (var rarcFile in objectDirectory.FilesWithExtension(".rarc")) {
          didDump |= rarcDump.Run(rarcFile,
                                  true,
                                  new HashSet<string>(new[] {"archive"}));
        }

        if (didDump) {
          objectDirectory.Refresh(true);
        }
      }

      this.ExtractObjects_(objectDirectory);

      /*{
        var relsDirectory = fileHierarchy.Root.TryToGetSubdir("rels");
        var mapFiles = fileHierarchy.Root.TryToGetSubdir("maps").Files;

        var yaz0Dec = new Yaz0Dec();
        var didDecompress = false;
        foreach (var relFile in relsDirectory.FilesWithExtension(".rel")) {
          didDecompress |= yaz0Dec.Run(relFile, false);
        }

        if (didDecompress) {
          relsDirectory.Refresh();
        }

        var didDump = false;
        var relDump = new RelDump();
        foreach (var rarcFile in relsDirectory.FilesWithExtension(".rarc")) {
          var mapFile = mapFiles.Single(
              file => file.NameWithoutExtension ==
                      rarcFile.NameWithoutExtension);

          didDump |= relDump.Run(rarcFile, mapFile, true);
        }

        if (didDump) {
          relsDirectory.Refresh(true);
        }
      }*/
    }

    private void ExtractObjects_(IFileHierarchyDirectory directory) {
      foreach (var subdir in directory.Subdirs) {
        this.ExtractObject_(subdir);
      }
    }

    private void ExtractObject_(IFileHierarchyDirectory directory) {
      // TODO: What the heck is the difference between these directories?
      // Is there any besides the model type within?
      var bdlSubdir =
          directory.Subdirs.SingleOrDefault(subdir => subdir.Name == "bdl");
      var bdlmSubdir =
          directory.Subdirs.SingleOrDefault(subdir => subdir.Name == "bdlm");
      var bmdSubdir =
          directory.Subdirs.SingleOrDefault(subdir => subdir.Name == "bmd");
      var bmdcSubdir =
          directory.Subdirs.SingleOrDefault(subdir => subdir.Name == "bmdc");
      var bmdmSubdir =
          directory.Subdirs.SingleOrDefault(subdir => subdir.Name == "bmdm");

      var bmdOrBdlFiles = ListUtil.ReadonlyConcat(
          //bdlSubdir?.FilesWithExtension(".bdl").ToArray(),
          //bdlmSubdir?.FilesWithExtension(".bdl").ToArray(),
          bmdSubdir?.FilesWithExtension(".bmd").ToArray(),
          bmdcSubdir?.FilesWithExtension(".bmd").ToArray(),
          bmdmSubdir?.FilesWithExtension(".bmd").ToArray());

      var bckSubdir =
          directory.Subdirs.SingleOrDefault(
              subdir => subdir.Name == "bck" || subdir.Name == "bcks");
      var bckFiles = bckSubdir?.FilesWithExtension(".bck").ToList();

      if (bmdOrBdlFiles.Count == 1 ||
          (bckFiles == null && bmdOrBdlFiles.Count > 0)) {
        this.ExtractModels_(directory,
                            bmdOrBdlFiles,
                            bckFiles?
                                .Select(file => file.Impl)
                                .ToArray());
      } else if (bmdOrBdlFiles.Count > 0) {
        IOrganizeMethod organizeMethod;
        switch (directory.Name) {
          case "Sh": {
            organizeMethod = new SuffixOrganizeMethod(1);
            break;
          }
          case "Oq": {
            organizeMethod = new NameMatchOrganizeMethod(directory.Name);
            break;
          }
          case "Ylesr00": {
            organizeMethod = new PrefixOrganizeMethod();
            break;
          }
          default:
            throw new NotImplementedException();
        }

        this.ExtractFilesByOrganizing_(directory,
                                       bmdOrBdlFiles.ToArray(),
                                       bckFiles,
                                       organizeMethod);
      }
    }

    public interface IOrganizeMethod {
      IList<IFileHierarchyFile> GetBcksForBmd(
          IFileHierarchyFile bmdFile,
          IList<IFileHierarchyFile> bckFiles);
    }

    public class PrefixOrganizeMethod : IOrganizeMethod {
      public IList<IFileHierarchyFile> GetBcksForBmd(
          IFileHierarchyFile bmdFile,
          IList<IFileHierarchyFile> bckFiles) {
        var prefix = StringUtil.UpTo(bmdFile.NameWithoutExtension, "_");
        return bckFiles.Where(file => file.Name.StartsWith(prefix)).ToArray();
      }
    }

    public class NameMatchOrganizeMethod : IOrganizeMethod {
      private string name_;

      public NameMatchOrganizeMethod(string name) {
        this.name_ = name.ToLower();
      }

      public IList<IFileHierarchyFile> GetBcksForBmd(
          IFileHierarchyFile bmdFile,
          IList<IFileHierarchyFile> bckFiles) {
        if (bmdFile.NameWithoutExtension.ToLower().Contains(this.name_)) {
          return bckFiles;
        }

        return Array.Empty<IFileHierarchyFile>();
      }
    }

    public class SuffixOrganizeMethod : IOrganizeMethod {
      private readonly int suffixLength_;

      public SuffixOrganizeMethod(int suffixLength) {
        this.suffixLength_ = suffixLength;
      }

      public IList<IFileHierarchyFile> GetBcksForBmd(
          IFileHierarchyFile bmdFile,
          IList<IFileHierarchyFile> bckFiles) {
        var suffix =
            bmdFile.NameWithoutExtension.Substring(
                bmdFile.NameWithoutExtension.Length -
                this.suffixLength_);

        return bckFiles.Where(file => file.Name.StartsWith(suffix)).ToArray();
      }
    }

    private void ExtractFilesByOrganizing_(
        IFileHierarchyDirectory directory,
        IList<IFileHierarchyFile> bmdFiles,
        IList<IFileHierarchyFile> bckFiles,
        IOrganizeMethod organizeMethod) {
      if (organizeMethod is PrefixOrganizeMethod) {
        bmdFiles.OrderByDescending(
            file => StringUtil
                    .UpTo(file.NameWithoutExtension, "_")
                    .Length);
      }

      var unclaimedBckFiles = new HashSet<IFileHierarchyFile>(bckFiles);

      var bmdFileToBckFiles =
          new Dictionary<IFileHierarchyFile, IList<IFileHierarchyFile>>();
      foreach (var bmdFile in bmdFiles) {
        var claimedBckFiles = organizeMethod.GetBcksForBmd(bmdFile, bckFiles);
        bmdFileToBckFiles[bmdFile] = claimedBckFiles;

        foreach (var bckFile in claimedBckFiles) {
          unclaimedBckFiles.Remove(bckFile);
        }
      }

      Asserts.Equal(0, unclaimedBckFiles.Count);

      foreach (var (bmdFile, claimedBckFiles) in bmdFileToBckFiles) {
        this.ExtractModels_(directory,
                            new[] {bmdFile},
                            claimedBckFiles.Select(file => file.Impl)
                                           .ToArray());
      }
    }

    private void ExtractModels_(
        IFileHierarchyDirectory directory,
        IReadOnlyList<IFileHierarchyFile> bmdFiles,
        IReadOnlyList<IFile>? bcxFiles = null,
        IReadOnlyList<IFile>? btiFiles = null,
        bool allowMultipleAnimatedModels = false
    ) {
      Asserts.True(bmdFiles.Count > 0);

      var outputDirectory =
          GameFileHierarchyUtil.GetOutputDirectoryForDirectory(directory);

      var matches = 0;
      var existingModelFiles =
          outputDirectory.GetExistingFiles()
                         .Where(file => file.Extension == ".fbx" ||
                                        file.Extension == ".glb")
                         .ToArray();

      foreach (var bmdFile in bmdFiles) {
        if (existingModelFiles.Any(
            existingModelFile => {
              var existingName = existingModelFile.Name.Substring(
                  0,
                  existingModelFile.Name.Length -
                  existingModelFile.Extension.Length);
              var bmdName =
                  bmdFile.Name.Substring(0,
                                         bmdFile.Name.Length - ".mod".Length);

              return bmdName == existingName ||
                     bmdName + "_gltf" == existingName;
            })) {
          ++matches;
        }
      }

      if (matches == bmdFiles.Count) {
        this.logger_.LogInformation(
            $"Model(s) already processed from {directory.LocalPath}");
        return;
      }

      bcxFiles ??= new List<IFile>();
      btiFiles ??= new List<IFile>();

      if (bmdFiles.Count == 1) {
        MessageUtil.LogExtracting(this.logger_, bmdFiles[0]);
      } else {
        MessageUtil.LogExtracting(this.logger_, directory, bmdFiles);
      }

      try {
        new ManualBmd2FbxApi().Process(outputDirectory,
                                       bmdFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       bcxFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       btiFiles.Select(file => file.FullName)
                                               .ToArray(),
                                       !allowMultipleAnimatedModels,
                                       60);
      } catch (Exception e) {
        this.logger_.LogError(e.ToString());
      }
      this.logger_.LogInformation(" ");
    }
  }
}