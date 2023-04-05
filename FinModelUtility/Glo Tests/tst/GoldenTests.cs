using System.Reflection;

using fin.exporter;
using fin.exporter.assimp.indirect;
using fin.io;
using fin.util.strings;

using glo.api;
using glo.schema;

using NUnit.Framework;

using schema.binary.testing;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace glo {
  public class GoldenTests {
    [Test]
    public async Task TestReadingAndWriting() {
      this.GetGloGoldenBundles_(out _, out var gloGoldenBundles);
      foreach (var goldenBundle in gloGoldenBundles) {
        var er = new EndianBinaryReader(goldenBundle.GloFile.OpenRead());
        await BinarySchemaAssert.ReadsAndWritesIdentically<Glo>(er);
      }
    }

    [Test]
    public void TestExportsExactModel() {
      this.GetGloGoldenBundles_(out var gloGoldensDirectory,
                                out var gloGoldenBundles);

      var tmpDirectory = gloGoldensDirectory.Impl.GetSubdir("tmp", true);

      var extensions = new[] { ".glb" };

      foreach (var goldenBundle in gloGoldenBundles) {
        foreach (var file in tmpDirectory.GetExistingFiles()) {
          file.Delete();
        }

        var inputDirectory = goldenBundle.GloFile.Parent;
        var outputDirectory = inputDirectory.Parent.GetExistingSubdir("output");

        var hasGoldenExport =
            outputDirectory.Files.Any(
                file => extensions.Contains(file.Extension));

        var targetDirectory =
            hasGoldenExport ? tmpDirectory : outputDirectory.Impl;

        var model = new GloModelLoader().LoadModel(goldenBundle);
        new AssimpIndirectExporter().ExportExtensions(
            new ExporterParams {
                Model = model,
                OutputFile =
                    new FinFile(Path.Combine(targetDirectory.FullName,
                                             $"{goldenBundle.GloFile.NameWithoutExtension}.foo")),
            },
            extensions);

        if (hasGoldenExport) {
          AssertSameFiles_(tmpDirectory, outputDirectory.Impl);
        }
      }
    }

    private void AssertSameFiles_(IDirectory lhs, IDirectory rhs) {
      var lhsFiles = lhs.GetExistingFiles()
                        .ToDictionary(file => (string) file.Name);
      var rhsFiles = rhs.GetExistingFiles()
                        .ToDictionary(file => (string) file.Name);

      Assert.IsTrue(lhsFiles.Keys.ToHashSet()
                            .SetEquals(rhsFiles.Keys.ToHashSet()));

      foreach (var (name, lhsFile) in lhsFiles) {
        var rhsFile = rhsFiles[name];

        using var lhsStream = lhsFile.OpenRead();
        using var rhsStream = rhsFile.OpenRead();

        Assert.AreEqual(lhsStream.Length, rhsStream.Length);

        for (var i = 0; i < lhsStream.Length; i++) {
          //Assert.AreEqual(lhsStream.ReadByte(), rhsStream.ReadByte());
          if (lhsStream.ReadByte() != rhsStream.ReadByte()) {
            Assert.Fail();
          }
        }
      }
    }

    private void GetGloGoldenBundles_(
        out IFileHierarchyDirectory gloGoldensDirectory,
        out IEnumerable<GloModelFileBundle> gloGoldenBundles) {
      var executingAssembly = Assembly.GetExecutingAssembly();
      var assemblyName =
          StringUtil.UpTo(executingAssembly.ManifestModule.Name, ".dll");

      var executingAssemblyDll =
          new FinFile(Assembly.GetExecutingAssembly().Location);
      var executingAssemblyDir = executingAssemblyDll.GetParent();

      var currentDir = executingAssemblyDir;
      while (currentDir.Name != assemblyName) {
        currentDir = currentDir.GetParent();
      }

      Assert.IsNotNull(currentDir);

      var gloTestsDir = currentDir;
      var goldensDirectory = gloTestsDir.GetSubdir("goldens");

      var hierarchy = new FileHierarchy(goldensDirectory);

      gloGoldensDirectory = hierarchy.Root;
      gloGoldenBundles = gloGoldensDirectory.Subdirs.Select(subdir => {
                                              if (subdir.TryToGetExistingSubdir(
                                                      "input",
                                                      out var inputDir)) {
                                                var gloFile =
                                                    inputDir
                                                        .FilesWithExtension(
                                                            ".glo")
                                                        .Single();
                                                return new GloModelFileBundle(
                                                    gloFile,
                                                    new[] { inputDir });
                                              }

                                              return null;
                                            })
                                            .Where(bundle => bundle != null);
    }
  }
}