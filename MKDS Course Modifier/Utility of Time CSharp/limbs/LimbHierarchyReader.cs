using System;
using System.Collections.Generic;
using System.Windows.Forms;

using UoT.util;

namespace UoT.limbs {
  public static class LimbHierarchyReader {
    /// <summary>
    ///   Parses a limb hierarchy according to the following spec:
    ///   https://wiki.cloudmodding.com/oot/Animation_Format#Hierarchy
    /// </summary>
    public static IList<Limb>? GetHierarchies(
        IBank Data,
        bool isLink,
        DlManager dlManager,
        StaticDlModel model,
        ComboBox dListSelection) {
      uint limbIndexAddress;
      model.Reset();
      int j = 0;
      for (int i = 0, loopTo = Data.Count - 8; i <= loopTo; i += 4) {
        limbIndexAddress = IoUtil.ReadUInt32(Data, (uint) i);
        IoUtil.SplitAddress(limbIndexAddress,
                            out var limbIndexBank,
                            out var limbIndexOffset);
        uint limbCount = Data[i + 4];
        uint limbAddress;

        // Link has an extra set of values for each limb that define LOD model
        // display lists.
        uint limbSize;
        if (isLink) {
          limbSize = 16U;
        } else {
          limbSize = 12U;
        }

        if (RamBanks.IsValidBank((byte) limbIndexBank) & limbCount > 0L) {
          var limbIndexBankBuffer = RamBanks.GetBankByIndex(limbIndexBank);

          if (limbIndexBankBuffer != null &&
              limbIndexOffset + 4L * limbCount < limbIndexBankBuffer.Count) {
            byte firstChild;
            byte nextSibling;
            bool isValid = true;
            bool somethingVisible = false;
            var loopTo1 = (int) (limbCount - 1L);
            for (j = 0; j <= loopTo1; j++) {
              limbAddress = IoUtil.ReadUInt32(limbIndexBankBuffer,
                                              (uint) (limbIndexOffset + j * 4));

              IoUtil.SplitAddress(limbAddress,
                                  out var limbBank,
                                  out var limbOffset);

              if (!RamBanks.IsValidBank(limbBank)) {
                isValid = false;
                goto badLimbIndexOffset;
              }

              var limbBankBuffer = RamBanks.GetBankByIndex(limbBank);
              if (limbBankBuffer == null) {
                isValid = false;
                goto badLimbIndexOffset;
              }

              if (limbOffset + limbSize >= limbBankBuffer.Count) {
                isValid = false;
                goto badLimbIndexOffset;
              }

              firstChild = limbBankBuffer[(int) (limbOffset + 6L)];
              nextSibling = limbBankBuffer[(int) (limbOffset + 7L)];
              if (firstChild == j | nextSibling == j) {
                isValid = false;
                goto badLimbIndexOffset;
              }

              var displayListAddress =
                  IoUtil.ReadUInt32(limbBankBuffer, (uint) (limbOffset + 8L));
              IoUtil.SplitAddress(displayListAddress,
                                  out var displayListBank,
                                  out var displayListOffset);

              if (displayListBank != 0L) {
                somethingVisible = true;
              }

              if (displayListBank != 0L &
                  !RamBanks.IsValidBank((byte) displayListBank)) {
                isValid = false;
                goto badLimbIndexOffset;
              }
            }

            badLimbIndexOffset:

            if (isValid) {
              var tmpHierarchy = new Limb[(int) (limbCount - 1L + 1)];
              for (int k = 0, loopTo2 = (int) (limbCount - 1L);
                   k <= loopTo2;
                   k++) {
                limbAddress = IoUtil.ReadUInt32(limbIndexBankBuffer,
                                                (uint) (limbIndexOffset +
                                                        4 * k));
                IoUtil.SplitAddress(limbAddress,
                                    out var limbBank,
                                    out var limbOffset);
                var limbBankBuffer =
                    Asserts.Assert(RamBanks.GetBankByIndex(limbBank));

                tmpHierarchy[k] = new Limb();
                {
                  var displayListAddress =
                      IoUtil.ReadUInt32(limbBankBuffer,
                                        (uint)(limbOffset + 8L));
                  IoUtil.SplitAddress(displayListAddress,
                                      out var displayListBank,
                                      out var displayListOffset);

                  var visible = displayListAddress > 0;

                  var withBlock = tmpHierarchy[k];

                  withBlock.Visible = visible;
                  withBlock.x =
                      (short) IoUtil.ReadUInt16(
                          limbBankBuffer,
                          (uint) (limbOffset + 0L));
                  withBlock.y =
                      (short) IoUtil.ReadUInt16(
                          limbBankBuffer,
                          (uint) (limbOffset + 2L));
                  withBlock.z =
                      (short) IoUtil.ReadUInt16(
                          limbBankBuffer,
                          (uint) (limbOffset + 4L));
                  withBlock.firstChild =
                      (sbyte) limbBankBuffer[(int) (limbOffset + 6L)];
                  withBlock.nextSibling =
                      (sbyte) limbBankBuffer[(int) (limbOffset + 7L)];
                  
                  model.AddLimb(visible,
                                withBlock.x,
                                withBlock.y,
                                withBlock.z,
                                withBlock.firstChild,
                                withBlock.nextSibling);

                  if (displayListBank != 0L) {
                    var displayListBankBuffer =
                        RamBanks.GetBankByIndex(displayListBank);
                    withBlock.DisplayListAddress = displayListAddress;
                    DisplayListReader.ReadInDL(dlManager,
                                               displayListAddress,
                                               dListSelection);
                  } else if (!somethingVisible) {
                    withBlock.DisplayListAddress = displayListAddress;
                  } else {
                    withBlock.DisplayListAddress = displayListAddress;
                  }

                  // Far model display list (i.e. LOD model). Only used for Link.
                  // If Data(tmpLimbOff + 12) = Bank Then
                  // .DisplayListLow = ReadUInt24(Data, tmpLimbOff + 13)
                  // ReDim Preserve N64DList(N64DList.Length)
                  // ReadInDL(Data, N64DList, .DisplayListLow, N64DList.Length - 1)
                  // Else
                  withBlock.DisplayListLow = default;

                  // End If
                  PickerUtil.NextRgb(out var r, out var g, out var b);
                  withBlock.r = r;
                  withBlock.g = g;
                  withBlock.b = b;

                  tmpHierarchy[k] = withBlock;
                }
              }

              if (isValid & !somethingVisible) {
                throw new NotSupportedException(
                    "model format is not rendering a valid model!");
              }

              return tmpHierarchy;
            }
          }
        }
      }

      return null;
    }
  }
}