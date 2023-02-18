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

using schema.binary;

namespace geo.schema.str {
  [BinarySchema]
  public partial class FileInfo {
    public FileBuild Build { get; set; }
    public ushort Alignment { get; set; }
    public ushort Flags { get; set; }

    public uint Type { get; set; }

    public uint Unknown0C { get; set; }
    public uint Type2 { get; set; }

    public uint Unknown14 { get; set; }

    // seems to be some kind of hash of the file name
    public uint Unknown18 { get; set; }

    public uint TotalSize { get; set; }

    [NullTerminatedString]
    public string BaseName { get; set; }

    [NullTerminatedString]
    public string FileName { get; set; }

    [NullTerminatedString]
    public string TypeName { get; set; }

    public string GetSaneFileName() {
      var name = this.FileName;
      var pos = name.LastIndexOf('\\');

      if (pos >= 0) {
        name = name.Substring(pos + 1);
      }

      if (name.Length > 50) {
        name = Path.ChangeExtension(name.Substring(0, 50), "." + this.TypeName);
      } else if (name.Length == 0) {
        name = Path.ChangeExtension("unknown", "." + this.TypeName);
      }

      return name;
    }
  }
}