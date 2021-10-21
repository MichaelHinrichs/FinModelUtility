using System.IO;
using System.Linq;

using fin.io;

using zar.format.cmb;

namespace zar.api {
  public class ManualZar2FbxApi {
    public void Run(IDirectory outputDirectory, IFile[] cmbFiles) {
      var filesAndCmbs =
          cmbFiles.Select(cmbFile => (cmbFile,
                                      new Cmb(
                                          new EndianBinaryReader(
                                              cmbFile.OpenRead(),
                                              Endianness.LittleEndian))))
                  .ToList();
    }
  }
}