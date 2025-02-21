// Copyright(c) 2021 SceneGate
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using SceneGate.Ekona.Security;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace SceneGate.Ekona.Tests.Containers.Rom
{
    [TestFixture]
    public class Binary2RomHeaderTests
    {
        public static IEnumerable<TestCaseData> GetFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Containers");
            string listPath = Path.Combine(basePath, "header.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"{{m}}({data[1]})"));
        }

        [TestCaseSource(nameof(GetFiles))]
        public void DeserializeRomHeaderMatchInfo(string infoPath, string headerPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(headerPath);

            string yaml = File.ReadAllText(infoPath);
            RomHeader expected = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build()
                .Deserialize<RomHeader>(yaml);

            using Node node = NodeFactory.FromFile(headerPath, FileOpenMode.Read);
            node.Invoking(n => n.TransformWith<Binary2RomHeader>()).Should().NotThrow();

            node.GetFormatAs<RomHeader>().Should().BeEquivalentTo(
                expected,
                opts => opts
                    .Excluding(p => p.CopyrightLogo)
                    .Excluding((FluentAssertions.Equivalency.IMemberInfo info) => info.Type == typeof(NitroProgramCodeParameters))
                    .Excluding((FluentAssertions.Equivalency.IMemberInfo info) => info.Type == typeof(HashInfo)));
        }

        [TestCaseSource(nameof(GetFiles))]
        public void DeserializeRomHeaderWithoutKeysHasHashes(string infoPath, string headerPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(headerPath);

            using Node node = NodeFactory.FromFile(headerPath, FileOpenMode.Read);
            node.Invoking(n => n.TransformWith<Binary2RomHeader>()).Should().NotThrow();

            ProgramInfo programInfo = node.GetFormatAs<RomHeader>().ProgramInfo;
            programInfo.ChecksumSecureArea.Status.Should().Be(HashStatus.NotValidated);
            programInfo.ChecksumLogo.Status.Should().Be(HashStatus.Valid);
            programInfo.ChecksumHeader.Status.Should().Be(HashStatus.Valid);

            bool isDsi = programInfo.UnitCode != DeviceUnitKind.DS;
            if (isDsi || programInfo.ProgramFeatures.HasFlag(DsiRomFeatures.NitroBannerSigned)) {
                programInfo.BannerMac.Should().NotBeNull();
                programInfo.BannerMac.Status.Should().Be(HashStatus.NotValidated);
            }

            if (programInfo.ProgramFeatures.HasFlag(DsiRomFeatures.NitroProgramSigned)) {
                programInfo.NitroOverlaysMac.Should().NotBeNull();
                programInfo.NitroOverlaysMac.Status.Should().Be(HashStatus.NotValidated);

                programInfo.NitroProgramMac.Should().NotBeNull();
                programInfo.NitroProgramMac.Status.Should().Be(HashStatus.NotValidated);
            }

            if (isDsi || programInfo.ProgramFeatures.HasFlag(DsiRomFeatures.NitroProgramSigned)) {
                programInfo.Signature.Should().NotBeNull();
                programInfo.Signature.Status.Should().Be(HashStatus.NotValidated);
            }

            if (isDsi) {
                programInfo.DsiInfo.Arm9SecureMac.Status.Should().Be(HashStatus.NotValidated);
                programInfo.DsiInfo.Arm7Mac.Status.Should().Be(HashStatus.NotValidated);
                programInfo.DsiInfo.DigestMain.Status.Should().Be(HashStatus.NotValidated);
                programInfo.DsiInfo.Arm9iMac.Status.Should().Be(HashStatus.NotValidated);
                programInfo.DsiInfo.Arm7iMac.Status.Should().Be(HashStatus.NotValidated);
                programInfo.DsiInfo.Arm9Mac.Status.Should().Be(HashStatus.NotValidated);
            }
        }

        [TestCaseSource(nameof(GetFiles))]
        public void TwoWaysIdenticalRomHeaderStream(string infoPath, string headerPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(headerPath);

            using Node node = NodeFactory.FromFile(headerPath, FileOpenMode.Read);

            var header = (RomHeader)ConvertFormat.With<Binary2RomHeader>(node.Format!);
            var generatedStream = (BinaryFormat)ConvertFormat.With<RomHeader2Binary>(header);

            var originalStream = new DataStream(node.Stream!, 0, header.SectionInfo.HeaderSize);
            generatedStream.Stream.Length.Should().Be(originalStream.Length);
            generatedStream.Stream.Compare(originalStream).Should().BeTrue();
        }

        [TestCaseSource(nameof(GetFiles))]
        public void ThreeWaysIdenticalRomHeaderObjects(string infoPath, string headerPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(headerPath);

            using Node node = NodeFactory.FromFile(headerPath, FileOpenMode.Read);

            var originalHeader = (RomHeader)ConvertFormat.With<Binary2RomHeader>(node.Format!);
            using var generatedStream = (BinaryFormat)ConvertFormat.With<RomHeader2Binary>(originalHeader);
            var generatedHeader = (RomHeader)ConvertFormat.With<Binary2RomHeader>(generatedStream);

            generatedHeader.Should().BeEquivalentTo(
                originalHeader,
                opts => opts.Excluding((FluentAssertions.Equivalency.IMemberInfo info) => info.Type == typeof(HashInfo)));
        }
    }
}
