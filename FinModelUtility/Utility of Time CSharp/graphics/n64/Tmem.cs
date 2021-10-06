using System;

using UoT.util;

namespace UoT {
  // TODO: Delete statics, use this within DlManager.
  /// <summary>
  ///   TMEM (texture memory) implementation. TMEM seems to be a middleman for
  ///   N64 texture loading: textures are loaded from RAM into TMEM, and tile
  ///   descriptors point into TMEM at a specific offset.
  /// </summary>
  public class Tmem {
    private readonly TextureCache cache_;

    private const int TMEM_WORD_COUNT = 512;
    private const int TMEM_BYTE_COUNT = TMEM_WORD_COUNT * 8;

    private readonly byte[] impl_ = new byte[TMEM_BYTE_COUNT];

    public Tmem(TextureCache cache) {
      this.cache_ = cache;
    }

    /// <summary>
    ///   Shamelessly copied from GLideN64's source.
    /// </summary>
    public void LoadTile(
        ref TileDescriptor tileDescriptor,
        ushort uls,
        ushort ult,
        ushort lrs,
        ushort lrt,
        TimgArgs timgArgs) {
      // TODO: Implement this method.
      // TODO: To verify behavior, load contents into a new texture and save to
      // file.

      tileDescriptor.ULS = uls;
      tileDescriptor.ULT = ult;
      tileDescriptor.LRS = lrs;
      tileDescriptor.LRT = lrt;

      var fUls = IoUtil.Fixed2Float(uls, 2);
      var fUlt = IoUtil.Fixed2Float(ult, 2);
      var fLrs = IoUtil.Fixed2Float(lrs, 2);
      var fLrt = IoUtil.Fixed2Float(lrt, 2);

      var timgImageAddress = timgArgs.Address;
      var timgBitSize = timgArgs.BitSize;
      var timgWidth = timgArgs.Width;
      var timgBpl = timgWidth << (int) timgBitSize >> 1;

      var tmem = tileDescriptor.TmemOffset;
      var line = tileDescriptor.LineSize;
      var maskS = tileDescriptor.MaskS;
      var maskT = tileDescriptor.MaskT;

      byte imageBank;
      uint offset;
      IoUtil.SplitAddress(timgImageAddress, out imageBank, out offset);
      tileDescriptor.ImageBank = imageBank;
      tileDescriptor.Offset = (int) offset;

      var texture = this.cache_[tileDescriptor];
      if (texture != null) {
        return;
      }

      if (lrs < uls || lrt < ult) {
        return;
      }

      var width = (lrs - uls + 1) & 0x03FF;
      var height = (lrt - ult + 1) & 0x03FF;
      var bpl = line << 3;

      var alignedWidth = width;
      var wmask = 0;

      switch (timgBitSize) {
        case BitSize.S_8B:
          wmask = 7;
          break;
        case BitSize.S_16B:
          wmask = 3;
          break;
        case BitSize.S_32B:
          wmask = 1;
          break;
        default:
          throw new NotSupportedException();
      }

      if ((width & wmask) != 0) {
        alignedWidth = (width & (~wmask)) + wmask + 1;
      }
      var bpr = alignedWidth << (int) timgBitSize >> 1;

      // Start doing the loading.
      var infoTexAddress = timgImageAddress;
      var infoWidth = (ushort) (maskS != 0
                                    ? Math.Min(width, 1U << maskS)
                                    : width);
      var infoHeight = (ushort) (maskT != 0
                                     ? Math.Min(height,
                                                1U << maskT)
                                     : height);
      var infoTexWidth = timgWidth;
      var infoSize = timgBitSize;
      var infoBytes = bpl * height;

      if (timgBitSize == BitSize.S_32B) {
        // 32 bit texture loaded into lower and upper half of TMEM, thus actual bytes doubled.
        infoBytes *= 2;
      }

      if (line == 0) {
        return;
      }

      if (maskS == 0) {
        tileDescriptor.LoadWidth =
            Math.Max(tileDescriptor.LoadWidth, infoWidth);
      }
      if (maskT == 0) {
        if (Gdp.CycleMode != (int) RdpCycleMode.G_CYC_2CYCLE &&
            tmem % line == 0) {
          var theight = (ushort) (infoHeight + tmem / line);
          tileDescriptor.LoadHeight =
              Math.Max(tileDescriptor.LoadHeight, theight);
        } else {
          tileDescriptor.LoadHeight =
              Math.Max(tileDescriptor.LoadHeight, infoHeight);
        }
      }

      var address = timgImageAddress +
                    ult * timgBpl +
                    (uls << (int) timgBitSize >> 1);

      /*
      u32 bpl2 = bpl;
      if (gDP.loadTile->lrs > timgWidth)
        bpl2 = (gDP.textureImage.width - gDP.loadTile->uls);
      var height2 = height;

      if (gDP.loadTile->lrt > gDP.scissor.lry)
        height2 = static_cast<u32>(gDP.scissor.lry) - gDP.loadTile->ult;

      if (CheckForFrameBufferTexture(address, info.width, bpl2 * height2))
        return;*/

      if (timgBitSize == BitSize.S_32B) {
        this.LoadTile32b_(ref tileDescriptor, timgArgs);
      } else {
        ;
        /*u32 tmemAddr = gDP.loadTile->tmem;
        const u32 line = gDP.loadTile->line;
        const u32 qwpr = bpr >> 3;
        for (u32 y = 0; y < height; ++y) {
          if (address + bpl > RDRAMSize)
            UnswapCopyWrap(RDRAM,
                           address,
                           reinterpret_cast<u8*>(TMEM),
                           tmemAddr << 3,
                           0xFFF,
                           RDRAMSize - address);
          else
            UnswapCopyWrap(RDRAM,
                           address,
                           reinterpret_cast<u8*>(TMEM),
                           tmemAddr << 3,
                           0xFFF,
                           bpr);
          if (y & 1)
            DWordInterleaveWrap(reinterpret_cast<u32*>(TMEM),
                                tmemAddr << 1,
                                0x3FF,
                                qwpr);

          address += gDP.textureImage.bpl;
          if (address >= RDRAMSize)
            break;
          tmemAddr += line;
        }*/
      }
    }

    /// <summary>
    ///   Shamelessly copied from GLideN64's source.
    /// </summary>
    private void LoadTile32b_(
        ref TileDescriptor tileDescriptor,
        TimgArgs timgArgs) {
      var uls = tileDescriptor.ULS;
      var ult = tileDescriptor.ULT;
      var lrs = tileDescriptor.LRS;
      var lrt = tileDescriptor.LRT;

      var width = lrs - uls + 1;
      var height = lrt - ult + 1;

      var line = tileDescriptor.LineSize << 2;
      var tbase = tileDescriptor.TmemOffset << 2;

      IoUtil.SplitAddress(timgArgs.Address, out var bank, out var offset);

      var targetBank = Asserts.Assert(RamBanks.GetBankByIndex(bank));

      var timgWidth = timgArgs.Width;

      for (var j = 0; j < height; ++j) {
        var tline = tbase + line * j;
        var s = ((j + ult) * timgWidth) + uls;
        var xorval = (j & 1) != 0 ? 3 : 1;

        for (var i = 0; i < width; ++i) {
          var addr = offset + s + i;
          var c = IoUtil.ReadUInt32(targetBank, (uint) (4 * addr));

          var ptr = ((tline + i) ^ xorval) & 0x3ff;

          var offset1 = 2 * ptr;
          IoUtil.WriteInt16(targetBank, ref offset1, (ushort) (c >> 16));

          var offset2 = 2 * (ptr | 0x400);
          IoUtil.WriteInt16(targetBank, ref offset2, (ushort) (c & 0xffff));
        }
      }
    }

    /// <summary>
    ///   Shamelessly copied from GLideN64's source.
    /// </summary>
    private void CalcTileSize(ref TileDescriptor tileDescriptor) {
      //, TileSizes &_sizes, gDPTile* _pLoadTile) {
      /*gDPTile* pTile = _t < 2 ? gSP.textureTile[_t] : &gDP.tiles[_t];
      pTile->masks = pTile->originalMaskS;
      pTile->maskt = pTile->originalMaskT;

      u32 tileWidth = ((pTile->lrs - pTile->uls) & 0x03FF) + 1;
      u32 tileHeight = ((pTile->lrt - pTile->ult) & 0x03FF) + 1;

      const u32 tMemMask = gDP.otherMode.textureLUT == G_TT_NONE ? 0x1FF : 0xFF;
      gDPLoadTileInfo & info = gDP.loadInfo[pTile->tmem & tMemMask];
      if (pTile->tmem == gDP.loadTile->tmem) {
        if (gDP.loadTile->loadWidth != 0 && gDP.loadTile->masks == 0)
          info.width = gDP.loadTile->loadWidth;
        if (gDP.loadTile->loadHeight != 0 && gDP.loadTile->maskt == 0) {
          info.height = gDP.loadTile->loadHeight;
          info.bytes = info.height * (gDP.loadTile->line << 3);
          if (gDP.loadTile->size == G_IM_SIZ_32b)
            // 32 bit texture loaded into lower and upper half of TMEM, thus actual bytes doubled.
            info.bytes *= 2;
        }
        gDP.loadTile->loadWidth = gDP.loadTile->loadHeight = 0;
      }
      _sizes.bytes = info.bytes;

      if (tileWidth == 1 && tileHeight == 1 &&
        gDP.otherMode.cycleType == G_CYC_COPY &&
        _pLoadTile != nullptr) {
        const u32 ulx = _SHIFTR(RDP.w1, 14, 10);
        const u32 uly = _SHIFTR(RDP.w1, 2, 10);
        const u32 lrx = _SHIFTR(RDP.w0, 14, 10);
        const u32 lry = _SHIFTR(RDP.w0, 2, 10);
        tileWidth = lrx - ulx + 1;
        tileHeight = lry - uly + 1;
      }

      u32 width = 0, height = 0;
      if (info.loadType == LOADTYPE_TILE) {
        width = min(info.width, info.texWidth);
        if (width == 0)
          width = tileWidth;
        if (info.size > pTile->size)
          width <<= info.size - pTile->size;

        height = info.height;
        if (height == 0)
          height = tileHeight;
        if ((config.generalEmulation.hacks & hack_MK64) != 0 && (height % 2) != 0)
          height--;
      } else {
        const TextureLoadParameters &loadParams =
          ImageFormat::get().tlp[gDP.otherMode.textureLUT][pTile->size][pTile->format];

        int tile_width = pTile->lrs - pTile->uls + 1;
        int tile_height = pTile->lrt - pTile->ult + 1;

        int mask_width = (pTile->masks == 0) ? (tile_width) : (1 << pTile->masks);
        int mask_height = (pTile->maskt == 0) ? (tile_height) : (1 << pTile->maskt);

        if (pTile->clamps)
          width = min(mask_width, tile_width);
        else if ((u32)(mask_width * mask_height) <= loadParams.maxTexels)
          width = mask_width;
        else
          width = tileWidth;

        if (pTile->clampt)
          height = min(mask_height, tile_height);
        else if ((u32)(mask_width * mask_height) <= loadParams.maxTexels)
          height = mask_height;
        else
          height = tileHeight;
      }

      _sizes.clampWidth = (pTile->clamps && gDP.otherMode.cycleType != G_CYC_COPY) ? tileWidth : width;
      _sizes.clampHeight = (pTile->clampt && gDP.otherMode.cycleType != G_CYC_COPY) ? tileHeight : height;

      _sizes.width = (info.loadType == LOADTYPE_TILE &&
              pTile->clamps != 0 &&
              pTile->masks == 0) ?
              _sizes.clampWidth :
              width;
      _sizes.height = (info.loadType == LOADTYPE_TILE &&
              pTile->clampt != 0 &&
              pTile->maskt == 0) ?
              _sizes.clampHeight :
              height;
    }*/
    }


    // TODO: The other method passes in "texels", not "lrs"???
    /// <summary>
    ///   Shamelessly copied from GLideN64's source.
    /// </summary>
    public void LoadBlock(
        ref TileDescriptor tileDescriptor,
        uint uls,
        uint ult,
        uint lrs,
        uint dxt,
        TimgArgs timgArgs) {
      tileDescriptor.ULS = (int) uls >> 2;
      tileDescriptor.ULT = (int) ult >> 2;
      // TODO: This feels like a bug?
      tileDescriptor.LRS = (int) lrs >> 2;
      tileDescriptor.LRT = (int) dxt >> 2;

      var tmem = tileDescriptor.TmemOffset;
      var colorFormat = tileDescriptor.ColorFormat;
      var bitSize = tileDescriptor.BitSize;

      /*if (gSP.DMAOffsets.tex_offset != 0) {
        if (gSP.DMAOffsets.tex_shift % (((lrs >> 2) + 1) << 3)) {
          gDP.textureImage.address -= gSP.DMAOffsets.tex_shift;
          gSP.DMAOffsets.tex_offset = 0;
          gSP.DMAOffsets.tex_shift = 0;
          gSP.DMAOffsets.tex_count = 0;
        } else
          ++gSP.DMAOffsets.tex_count;
      }*/

      var timgAddress = timgArgs.Address;
      var timgBitSize = timgArgs.BitSize;
      var timgWidth = timgArgs.Width;
      var timgBpl = timgWidth << (int) timgBitSize >> 1;


      IoUtil.SplitAddress(timgAddress, out var bank, out var offset);
      tileDescriptor.Address = (int) timgAddress;
      tileDescriptor.ImageBank = (int) bank;
      tileDescriptor.Offset = (int) offset;

      var texture = this.cache_[tileDescriptor];
      if (texture != null) {
        return;
      }

      /*gDPLoadTileInfo & info = gDP.loadInfo[gDP.loadTile->tmem];
      info.texAddress = gDP.loadTile->imageAddress;
      info.uls = static_cast<u16>(gDP.loadTile->uls);
      info.ult = static_cast<u16>(gDP.loadTile->ult);
      info.lrs = static_cast<u16>(gDP.loadTile->lrs);
      info.lrt = static_cast<u16>(gDP.loadTile->lrt);
      info.width = static_cast<u16>(gDP.loadTile->lrs);
      info.dxt = dxt;
      info.size = static_cast<u8>(gDP.textureImage.size);
      info.loadType = LOADTYPE_BLOCK;*/

      // TODO: This doesn't look right?
      uint jankWidth = (lrs - uls + 1) & 0x0FFF;
      uint bytes = jankWidth << (int) bitSize >> 1;
      if ((bytes & 7) != 0) {
        bytes = (bytes & (~7U)) + 8;
      }


      //info.bytes = bytes;
      uint address = (uint) (timgAddress +
                             ult * timgBpl +
                             (uls << (int) timgBitSize >> 1));
      IoUtil.SplitAddress(address, out var specBank, out var specOffset);



      /*if (bytes == 0 || (address + bytes) > RDRAMSize) {
        DebugMsg(DEBUG_NORMAL | DEBUG_ERROR, "// Attempting to load texture block out of range\n");
        DebugMsg(DEBUG_NORMAL, "gDPLoadBlock( %i, %i, %i, %i, %i );\n", tile, uls, ult, lrs, dxt);
        return;
      }*/

      /*
      gDP.loadTile->frameBufferAddress = 0;
      CheckForFrameBufferTexture(address, info.width, bytes); // Load data to TMEM even if FB texture is found. See comment to texturedRectDepthBufferCopy
      */
      /*var texLowerBound = tileDescriptor.TmemOffset;
      var texUpperBound = texLowerBound + (bytes >> 3);
      for (var i = 0; i < tile; ++i) {
        if (gDP.tiles[i].tmem >= texLowerBound && gDP.tiles[i].tmem < texUpperBound) {
          gDPLoadTileInfo & info = gDP.loadInfo[gDP.tiles[i].tmem];
          info.loadType = LOADTYPE_BLOCK;
        }
      }*/

      var targetBuffer = RamBanks.GetBankByIndex(specBank);
      if (targetBuffer == null) {
        return;
      }

      uint tmemAddr = tmem;
      if (bitSize == BitSize.S_32B) {
        //gDPLoadBlock32(gDP.loadTile->uls, gDP.loadTile->lrs, dxt);
      } else if (colorFormat == ColorFormat.YUV) {
        //memcpy(TMEM, &RDRAM[address], bytes); // HACK!
      } else {
        for (var i = 0; i < bytes; ++i) {
          this.impl_[i] = targetBuffer[(int) (specOffset + i)];
        }

        // TODO: Figure out what the heck this stuff below does.
        /*this.UnswapCopyWrap_(targetBuffer,
                             specOffset,
                             this.impl_,
                             tmemAddr << 3,
                             0xFFF,
                             bytes);*/
        /*if (dxt != 0) {
          uint dxtCounter = 0;
          uint qwords = (bytes >> 3);
          uint line = 0;
          while (true) {
            do {
              ++tmemAddr;
              --qwords;
              if (qwords == 0)
                goto end_dxt_test;
              dxtCounter += dxt;
            } while ((dxtCounter & 0x800) == 0);
            do {
              ++line;
              --qwords;
              if (qwords == 0)
                goto end_dxt_test;
              dxtCounter += dxt;
            } while ((dxtCounter & 0x800) != 0);
            this.DWordInterleaveWrap_(this.impl_, tmemAddr << 1, 0x3FF, line);
            tmemAddr += line;
            line = 0;
          }
          end_dxt_test:

          this.DWordInterleaveWrap_(this.impl_, tmemAddr << 1, 0x3FF, line);
        }*/
      }
    }


    public void LoadTexture(ref TileDescriptor tileDescriptor) {
      var tmem = tileDescriptor.TmemOffset;
      var maskS = tileDescriptor.MaskS;
      var maskT = tileDescriptor.MaskT;
      var bitSize = tileDescriptor.BitSize;

      // TODO: Throw an error if we can't figure out the size.
      // TODO: Might not always work w/ maskS/maskT, if 0.
      var width = (int)Math.Pow(2, maskS);
      var height = (int)Math.Pow(2, maskT);

      //tileDescriptor.TexBytes = bytes;
      tileDescriptor.LoadWidth = width;
      tileDescriptor.LoadHeight = height;

      // TODO: This might need to be set from the original?
      // TODO: How is this set?
      tileDescriptor.LineSize = width << (int)bitSize >> 1 >> 3;

      var generator = new OglTextureConverter();
      generator.GenerateAndAddToCache(this.impl_,
                                      tmem << 3,
                                      ref tileDescriptor,
                                      tileDescriptor.Palette32,
                                      this.cache_,
                                      true);
    }


    private void UnswapCopyWrap_(
        byte[] src,
        uint srcIdx,
        byte[] dest,
        uint destIdx,
        uint destMask,
        uint numBytes) {
      // copy leading bytes
      uint leadingBytes = srcIdx & 3;
      if (leadingBytes != 0) {
        leadingBytes = 4 - leadingBytes;
        if (leadingBytes > numBytes)
          leadingBytes = numBytes;
        numBytes -= leadingBytes;

        srcIdx ^= 3;
        for (uint i = 0; i < leadingBytes; i++) {
          dest[destIdx & destMask] = src[srcIdx];
          ++destIdx;
          --srcIdx;
        }
        srcIdx += 5;
      }

      // copy dwords
      int numDWords = (int) numBytes >> 2;
      while (numDWords-- > 0) {
        dest[(destIdx + 3) & destMask] = src[srcIdx++];
        dest[(destIdx + 2) & destMask] = src[srcIdx++];
        dest[(destIdx + 1) & destMask] = src[srcIdx++];
        dest[(destIdx + 0) & destMask] = src[srcIdx++];
        destIdx += 4;
      }

      // copy trailing bytes
      int trailingBytes = (int) numBytes & 3;
      if (trailingBytes > 0) {
        srcIdx ^= 3;
        for (int i = 0; i < trailingBytes; i++) {
          dest[destIdx & destMask] = src[srcIdx];
          ++destIdx;
          --srcIdx;
        }
      }
    }

    private void DWordInterleaveWrap_(
        byte[] src,
        uint srcIdx,
        uint srcMask,
        uint numQWords) {
      uint p0;
      int idx0, idx1;
      while (numQWords-- > 0) {
        idx0 = (int) (srcIdx++ & srcMask);
        idx1 = (int) (srcIdx++ & srcMask);

        // TODO: Original logic was meant for u32, so we need to think in terms
        // of ints.

        var offset0 = 4 * idx0;
        var offset1 = 4 * idx1;

        p0 = IoUtil.ReadUInt32(src, (uint) offset0);
        var p1 = IoUtil.ReadUInt32(src, (uint) offset1);

        IoUtil.WriteInt32(src, p1, ref offset0);
        IoUtil.WriteInt32(src, p0, ref offset1);
      }
    }
  }
}