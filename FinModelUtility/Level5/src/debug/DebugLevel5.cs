using fin.io;
using level5.schema;

namespace level5.debug {
  public class DebugLevel5 {
    public void Run() {
      var file = new FinFile(@"C:\Users\Ryan\Documents\CSharpWorkspace\FinModelUtility\cli\roms\professor_layton_vs_phoenix_wright\vs1\chr\c101.xc");

      var xc = file.ReadNew<Xc>(Endianness.LittleEndian);

      ;
    }
  }
}
