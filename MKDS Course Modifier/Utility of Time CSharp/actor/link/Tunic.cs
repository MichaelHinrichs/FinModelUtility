namespace UoT {
  public class Tunic {
    /**
     * Selected tunic id @ 15E6D0
     * - 17: kokiri
     * - 18: goron
     * - 19: zora
     *
     * Doesn't impact color until unpause.
     */

    public const int TUNIC_COLOR_DL_RDRAM_OFFSET = 0x1B478C;
    public static readonly byte[] TUNIC_COLOR_KOKIRI = {0x1E, 0x69, 0x1B, 0xFF };
    public static readonly byte[] TUNIC_COLOR_GORON = { 0x64, 0x14, 0x00, 0xFF };
    public static readonly byte[] TUNIC_COLOR_ZORA = { 0x00, 0x3C, 0x64, 0xFF };
    
    public const int TUNIC_TYPE_RDRAM_OFFSET = 0x2246FC;
    public enum TunicType {
      KOKIRI = 0,
      GORON = 1,
      ZORA = 2,
    }
  }
}
