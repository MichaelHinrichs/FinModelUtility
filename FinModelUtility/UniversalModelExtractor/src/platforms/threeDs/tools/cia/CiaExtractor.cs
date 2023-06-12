using fin.io;
using fin.log;
using fin.util.asserts;

using schema.binary;
using schema.binary.attributes.align;
using schema.binary.attributes.child_of;
using schema.binary.attributes.sequence;


namespace uni.platforms.threeDs.tools.cia {
  public partial class CiaExtractor {
    public bool Extract(ISystemFile ciaFile) {
      Asserts.True(ciaFile.Exists,
                   $"Could not extract CIA because it does not exist: {ciaFile.FullName}");
      Asserts.Equal(".cia",
                    ciaFile.Extension,
                    $"Could not extract file because it is not a CIA: {ciaFile.FullName}");

      var directoryPath = ciaFile.FullNameWithoutExtension;
      var directory = new FinDirectory(directoryPath);

      /*if (directory.Exists) {
        return false;
      }*/

      var logger = Logging.Create<CiaExtractor>();
      logger.LogInformation($"Extracting CIA {ciaFile.Name}...");

      var cia = ciaFile.ReadNew<Cia>(Endianness.LittleEndian);

      ;

      return true;
    }
  }
}