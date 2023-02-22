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

using fin.io;
using fin.log;
using fin.util.asserts;
using fin.util.linq;

using geo.decompression;
using geo.schema.str;
using geo.schema.str.content;

using FileInfo = geo.schema.str.content.FileInfo;

namespace geo.api {
  public class StrExtractor {
    private readonly ILogger logger_ = Logging.Create<StrExtractor>();

    public bool Extract(IFileHierarchyFile strFile, IDirectory outputDir) {
      this.logger_.LogInformation($"Extracting {strFile.LocalPath}...");

      outputDir.Create();

      var set = strFile.Impl.ReadNew<StreamSetFile>();

      var contentBlocks =
          set.Contents
             .Select(block => block.Impl.Data)
             .WhereIs<IBlock, ContentBlock>()
             .ToArray();

      int counter = 0;
      for (int i = 0; i < contentBlocks.Length;) {
        var headerInfo = contentBlocks[i];
        if (headerInfo.Impl.Data is not FileInfo fileInfo) {
          throw new FormatException("excepted header");
        }

        var fileName =
            counter.ToString("D4") + "_" +
            fileInfo.GetSaneFileName();
        counter++;

        fileName =
            Path.Combine(fileInfo.TypeName, fileName);

        i++;

        IFile outputFile =
            new FinFile(Path.Join(outputDir.FullName, fileName));
        Asserts.False(outputFile.Exists);
        outputFile.GetParent().Create();

        using var output = outputFile.OpenWrite();
        var readSize = 0L;
        while (readSize < fileInfo.TotalSize) {
          var leftSize = fileInfo.TotalSize - readSize;

          var dataInfo = contentBlocks[i];
          switch (dataInfo.Impl.Data) {
            case UncompressedData uncompressedData: {
              var data = uncompressedData.Bytes;
              var writeSize = Math.Min(leftSize, data.Length);
              output.Write(data, 0, (int) writeSize);
              readSize += writeSize;

              break;
            }
            case RefPackCompressedData compressedData: {
              Asserts.True(
                  new RefPackDecompressor().TryDecompress(
                      compressedData.RawBytes,
                      out var data));
              var writeSize = Math.Min(leftSize, (uint) data.Length);
              output.Write(data, 0, (int) writeSize);
              readSize += writeSize;

              break;
            }
            default:
              throw new InvalidOperationException();
          }

          ++i;
        }
      }

      return true;
    }
  }
}