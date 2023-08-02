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

using fin.schema.data;

using visceral.schema.str.content;

using schema.binary;
using schema.binary.attributes;

namespace visceral.schema.str {
  [Endianness(Endianness.LittleEndian)]
  [BinarySchema]
  public partial class StreamSetFile : IBinaryConvertible {
    private readonly BlockType magic_ = BlockType.Options;
    private readonly uint size_ = 12;

    /* Dead Space:
     * unknown00 = 2
     * unknown02 = 259
     * 
     * Dead Space 2:
     * unknown00 = 2
     * unknown02 = 259
     * 
     * Dante's Inferno:
     * unknown00 = 2
     * unknown02 = 1537
     */
    public ushort Unknown00 { get; set; }
    public ushort Unknown02 { get; set; }

    [RSequenceUntilEndOfStream]
    public List<BlockWrapper> Contents { get; } = new();

    [BinarySchema]
    public partial class BlockWrapper : IBinaryConvertible {
      public SwitchMagicUInt32SizedSection<BlockType, IBlock> Impl {
        get;
      }
        = new(-8,
              er => (BlockType) er.ReadUInt32(),
              (ew, magic) => ew.WriteUInt32((uint) magic),
              magic => magic switch {
                  BlockType.Options => new NoopBlock(),
                  BlockType.Content => new ContentBlock(),
                  BlockType.Padding => new NoopBlock(),
              });

      public override string ToString() => this.Impl.ToString();
    }
  }
}