using System;
using System.Collections.Generic;
using System.Linq;

using CommandLine;
using CommandLine.Text;

using fin.io;
using fin.log;
using fin.util.asserts;

namespace mod.cli {
  // TODO: Hook downstream classes into this for args by system.
  public class Args {
    public bool Automatic { get; private set; }
    public bool Verbose { get; private set; }

    public IDirectory OutputDirectory { get; private set; }

    public IList<IFile> ModFiles { get; private set; } =
      new List<IFile>();

    public IList<IFile> AnmFiles { get; private set; } =
      new List<IFile>();

    /// <summary>
    ///   Populates the static args from the command line arguments passed in.
    ///
    ///   Throws an error if parsing failed.
    /// </summary>
    public void PopulateFromArgs(string[] args) {
      IEnumerable<Error>? errors = null;

      var parserResult =
          Parser.Default.ParseArguments(
                    args,
                    typeof(AutomaticOptions),
                    typeof(ManualOptions),
                    typeof(DebugOptions))
                .WithParsed((AutomaticOptions automaticOpts) => {
                  this.Automatic = true;
                  this.Verbose = automaticOpts.Verbose;
                  this.OutputDirectory =
                      new FinDirectory(automaticOpts.OutputPath);
                  this.ModFiles = Files.GetFilesWithExtension("mod", true);
                  this.AnmFiles = Files.GetFilesWithExtension("anm", true);
                })
                .WithParsed((ManualOptions manualOpts) => {
                  this.Automatic = false;
                  this.Verbose = manualOpts.Verbose;
                  this.OutputDirectory =
                      new FinDirectory(manualOpts.OutputPath);
                  this.ModFiles =
                      manualOpts.ModPaths
                                .Select(modPath => new FinFile(modPath))
                                .ToList<IFile>();
                  this.AnmFiles =
                      manualOpts.AnmPaths
                                .Select(anmPath => new FinFile(anmPath))
                                .ToList<IFile>();
                })
                .WithParsed((DebugOptions debugOpts) => {
                  this.Automatic = false;
                  this.Verbose = debugOpts.Verbose;

                  this.GetForTesting(out var outputDirectory,
                                     out var modFiles,
                                     out var anmFiles);

                  this.OutputDirectory = outputDirectory;
                  this.ModFiles = modFiles;
                  this.AnmFiles = anmFiles;
                })
                .WithNotParsed(parseErrors => errors = parseErrors);

      if (errors != null) {
        var helpText = HelpText.AutoBuild(parserResult);
        helpText.AutoHelp = true;

        throw new Exception();
      }

      Logging.Initialize(this.Verbose);
    }

    private IDirectory GetOutputDirectory_(string name) {
      var basePath = "R:/Documents/CSharpWorkspace/Pikmin2Utility/";
      return new FinDirectory($"{basePath}cli/out/pkmn1/{name}/");
    }

    public void GetForTesting(
        out IDirectory outputDirectory,
        out IList<IFile> modFiles,
        out IList<IFile> anmFiles) {
      /*outputDirectory = Args.GetOutputDirectory_("beatle");
      modFiles = new IFile[] {
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\beatle\beatle.mod")
      };
      anmFiles = new IFile[] {
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\beatle\beatle.anm")
      };*/

      /*Args.Automatic = true;
      Args.GetFromDirectory(new FinDirectory(
                                @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\bosses\kogane\"),
                            out outputDirectory,
                            out modFiles,
                            out anmFiles);

      /*outputDirectory = Args.GetOutputDirectory_("kingback");
      var modFile = new FinFile(
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\bosses\kingback\se_col.mod");
      IFile anmFile = null;
      //new FinFile(@"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\bosses\kingback\kingback.anm");

      /*outputDirectory = Args.GetOutputDirectory_("logo");
      modFiles = new IFile[] {
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\cinemas\titles\logo.mod")
      };
      anmFiles = new IFile[] {
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\cinemas\titles\logo.anm")
      };*/

      /*modFile = new FinFile(
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\frog\frog.mod");
      anmFile = new FinFile(
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\frog\frog.anm");*/

      this.Automatic = true;
      /*Args.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappy\"),
          out outputDirectory,
          out modFiles,
          out anmFiles);*/
      /*Args.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappy\"),
          out outputDirectory,
          out modFiles,
          out anmFiles);*/
      this.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pikmin_1.gcm_dir\dataDir\bosses\slime\"),
          out outputDirectory,
          out modFiles,
          out anmFiles);
      /*Args.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\courses\stage1"),
          out outputDirectory,
          out modFiles,
          out anmFiles);*/
      /*Args.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\courses\stage2"),
          out outputDirectory,
          out modFiles,
          out anmFiles);*/

      /*modFiles = new[] {modFile};

      if (anmFile != null) {
        anmFiles = new[] {anmFile};
      } else {
        anmFiles = new List<IFile>();
      }*/

      /*Program.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\bosses\pom\"),
          out modFile,
          out anmFile);

      /*Program.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappb\"),
          out modFile,
          out anmFile);*/
    }

    public void GetOneFromDirectory(
        IDirectory directory,
        out IDirectory outputDirectory,
        out IFile modFile,
        out IFile? anmFile) {
      outputDirectory = this.GetOutputDirectory_(directory.Name);

      modFile = Files.GetFileWithExtension(directory, "mod");

      var anmFiles = Files.GetFilesWithExtension(directory, "anm");
      Asserts.True(anmFiles.Length <= 1, "Found more than one anm file!");
      anmFile = anmFiles[0];
    }

    public void GetFromDirectory(
        IDirectory directory,
        out IDirectory outputDirectory,
        out IList<IFile> modFiles,
        out IList<IFile> anmFiles) {
      outputDirectory = this.GetOutputDirectory_(directory.Name);

      modFiles = Files.GetFilesWithExtension(directory, "mod");
      anmFiles = Files.GetFilesWithExtension(directory, "anm");
    }
  }
}