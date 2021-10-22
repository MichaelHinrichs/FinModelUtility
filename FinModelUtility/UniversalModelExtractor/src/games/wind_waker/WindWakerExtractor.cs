using bmd.api;

using fin.io;
using fin.log;
using fin.util.asserts;

using System;
using System.Collections.Generic;
using System.Linq;

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
      var bmdmSubdir =
          directory.Subdirs.SingleOrDefault(subdir => subdir.Name == "bmdm");
      if (bmdmSubdir != null) {
        var bmdFiles =
            bmdmSubdir.Files.Single(file => file.Extension == ".bmd");
        this.ExtractModels_(directory, new[] {bmdFiles});
        return;
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