using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.exporter;
using fin.exporter.assimp.indirect;
using fin.io;
using fin.model;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace fin.testing {
  public static class ModelGoldenAssert {
    private const string TMP_NAME = "tmp";

    public static IEnumerable<IFileHierarchyDirectory> GetGoldenDirectories(
        IDirectory rootGoldenDirectory) {
      var hierarchy = new FileHierarchy(rootGoldenDirectory);
      return hierarchy.Root.Subdirs.Where(
          subdir => subdir.Name != TMP_NAME);
    }

    /// <summary>
    ///   Asserts model goldens. Assumes that directories will be stored as the following:
    ///
    ///   - {goldenDirectory}
    ///     - {goldenName1}
    ///       - input
    ///         - {raw golden files}
    ///       - output
    ///         - {exported files}
    ///     - {goldenName2}
    ///       ... 
    /// </summary>
    public static void AssertExportGoldens<TModelBundle>(
        IDirectory rootGoldenDirectory,
        IModelLoader<TModelBundle> modelLoader,
        Func<IFileHierarchyDirectory, TModelBundle>
            gatherModelBundleFromInputDirectory)
        where TModelBundle : IModelFileBundle {
      var tmpDirectory = rootGoldenDirectory.GetSubdir(TMP_NAME, true);

      var extensions = new[] {".glb"};

      foreach (var goldenSubdir in GetGoldenDirectories(rootGoldenDirectory)) {
        tmpDirectory.DeleteContents();

        var inputDirectory = goldenSubdir.GetExistingSubdir("input");
        var modelBundle = gatherModelBundleFromInputDirectory(inputDirectory);

        var outputDirectory = goldenSubdir.GetExistingSubdir("output");
        var hasGoldenExport =
            outputDirectory.Files
                           .Any(file => extensions.Contains(file.Extension));

        var targetDirectory =
            hasGoldenExport ? tmpDirectory : outputDirectory.Impl;

        var model = modelLoader.LoadModel(modelBundle);
        new AssimpIndirectExporter().ExportExtensions(
            new ExporterParams {
                Model = model,
                OutputFile =
                    new FinFile(Path.Combine(targetDirectory.FullName,
                                             $"{modelBundle.MainFile.NameWithoutExtension}.foo")),
            },
            extensions);

        if (hasGoldenExport) {
          AssertFilesInDirectoriesAreIdentical_(
              tmpDirectory,
              outputDirectory.Impl);
        }

        tmpDirectory.DeleteContents();
      }
    }

    private static void AssertFilesInDirectoriesAreIdentical_(
        IDirectory lhs,
        IDirectory rhs) {
      var lhsFiles = lhs.GetExistingFiles()
                        .ToDictionary(file => (string) file.Name);
      var rhsFiles = rhs.GetExistingFiles()
                        .ToDictionary(file => (string) file.Name);

      Assert.IsTrue(lhsFiles.Keys.ToHashSet()
                            .SetEquals(rhsFiles.Keys.ToHashSet()));

      foreach (var (name, lhsFile) in lhsFiles) {
        var rhsFile = rhsFiles[name];
        AssertFilesAreIdentical_(lhsFile, rhsFile);
      }
    }

    private static void AssertFilesAreIdentical_(IFile lhs, IFile rhs) {
      using var lhsStream = lhs.OpenRead();
      using var rhsStream = rhs.OpenRead();

      Assert.AreEqual(lhsStream.Length, rhsStream.Length);

      for (var i = 0; i < lhsStream.Length; i++) {
        Assert.AreEqual(lhsStream.ReadByte(),
                        rhsStream.ReadByte(),
                        $"Files differed at byte #:{i}");
      }
    }
  }
}