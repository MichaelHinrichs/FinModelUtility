using System.Reflection;

using fin.io;
using fin.testing.model;
using fin.util.enumerables;

using modl.api;

namespace modl {
  public class OutModelGoldenTests
      : BModelGoldenTests<OutModelFileBundle, OutModelLoader> {
    [Test]
    [TestCaseSource(nameof(GetGoldenDirectories_))]
    public void TestExportsGoldenAsExpected(
        IFileHierarchyDirectory goldenDirectory)
      => this.AssertGolden(goldenDirectory);

    public override OutModelFileBundle GetFileBundleFromDirectory(
        IFileHierarchyDirectory directory)
      => new() {
          GameName = directory.Parent.Parent.Name,
          GameVersion = directory.Parent.Parent.Name switch {
              "battalion_wars_1" => GameVersion.BW1,
              "battalion_wars_2" => GameVersion.BW2,
          },
          OutFile = directory.FilesWithExtension(".out").Single(),
          TextureDirectories = directory.Yield(),
      };

    private static IFileHierarchyDirectory[] GetGoldenDirectories_()
      => ModelGoldenAssert
         .GetGoldenDirectories(
             ModelGoldenAssert
                 .GetRootGoldensDirectory(Assembly.GetExecutingAssembly())
                 .GetSubdir("out"))
         .SelectMany(dir => dir.Subdirs)
         .ToArray();
  }
}