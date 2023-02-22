/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Xml;

namespace geo.schema.str {
  public class Program {
    private static string GetExecutableName() {
      return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
    }

    public static void Main(string[] args) {
      bool verbose = false;
      bool overwriteFiles = true;
      bool showHelp = false;
      bool debugMode = false;

      List<string> extra;

      string inputPath = extra[0];
      string outputPath = extra.Count > 1 ? extra[1] : Path.ChangeExtension(inputPath, null) + "_unpacked";

      Stream input = File.OpenRead(inputPath);
      Directory.CreateDirectory(outputPath);

      var set = new StreamSetFile();
      set.Deserialize(input);

      int counter = 0;
      for (int i = 0; i < set.Contents.Count;) {
        var headerInfo = set.Contents[i];
        if (headerInfo.Type != StreamSet.ContentType.Header) {
          //throw new FormatException("excepted header");
          i++;
          continue;
        }

        input.Seek(headerInfo.Offset, SeekOrigin.Begin);

        var fileInfo = new StreamSet.FileInfo();
        fileInfo.Deserialize(input, set.Endian);

        if (input.Position > headerInfo.Offset + headerInfo.Size) {
          throw new FormatException("read too much header data?");
        }

        string fileName;

        fileName =
            counter.ToString("D4") + "_" +
            fileInfo.GetSaneFileName();
        counter++;

        fileName =
            Path.Combine(fileInfo.TypeName, fileName);

        i++;

        string outputName = Path.Combine(outputPath, fileName);

        if (overwriteFiles == false &&
            File.Exists(outputName) == true) {
          continue;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(outputName));

        using var output = File.Create(outputName);
        uint readSize = 0;
        while (readSize < fileInfo.TotalSize) {
          uint leftSize = fileInfo.TotalSize - readSize;

          var dataInfo = set.Contents[i];
          if (dataInfo.Type != StreamSet.ContentType.Data &&
              dataInfo.Type != StreamSet.ContentType.CompressedData) {
            throw new InvalidOperationException();
          }

          input.Seek(dataInfo.Offset, SeekOrigin.Begin);

          if (dataInfo.Type == StreamSet.ContentType.CompressedData) {
            var compressedSize = input.ReadValueU32(set.Endian);
            if (4 + compressedSize > dataInfo.Size) {
              throw new InvalidOperationException();
            }

            var compressedStream = input.ReadToMemoryStream((int) compressedSize);
            var compressedData = Gibbed.RefPack.Decompression.Decompress(
                compressedStream);

            uint writeSize = Math.Min(leftSize, (uint) compressedData.Length);
            output.Write(compressedData, 0, (int) writeSize);
            readSize += writeSize;
          } else {
            uint writeSize = Math.Min(leftSize, dataInfo.Size);
            output.WriteFromStream(input, writeSize);
            readSize += writeSize;
          }

          ++i;
        }
      }
    }
  }
}