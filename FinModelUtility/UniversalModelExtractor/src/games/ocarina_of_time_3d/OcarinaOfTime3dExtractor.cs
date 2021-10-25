﻿using System.Linq;

using uni.platforms;
using uni.platforms.threeDs;

using zar.api;

namespace uni.games.ocarina_of_time_3d {
  public class OcarinaOfTime3dExtractor {
    public void ExtractAll() {
      var ocarinaOfTime3dRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "ocarina_of_time_3d.cia");

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor()
              .ExtractFromRom(ocarinaOfTime3dRom);

      var cowSubdir = fileHierarchy.Root.TryToGetSubdir(@"actor\zelda_cow");
      //var cowSubdir = fileHierarchy.Root.TryToGetSubdir(@"actor\zelda_ganon");
      //var cowSubdir = fileHierarchy.Root.TryToGetSubdir(@"actor\zelda_owl");

      new ManualZar2FbxApi().Run(
          GameFileHierarchyUtil.GetOutputDirectoryForDirectory(cowSubdir),
          cowSubdir.TryToGetSubdir("Model")
                   .FilesWithExtension(".cmb")
                   .Select(file => file.Impl)
                   .Take(1)
                   .ToArray(),
          cowSubdir.TryToGetSubdir("Anim")
                   .FilesWithExtension(".csab")
                   .Select(file => file.Impl)
                   .ToArray());
    }
  }
}