using System;
using System.Collections.Generic;

using CommandLine;
using CommandLine.Text;

using uni.games.animal_crossing;
using uni.games.ocarina_of_time_3d;
using uni.games.pikmin_1;
using uni.games.pikmin_2;
using uni.games.super_mario_sunshine;

namespace uni.cli {
  public class Cli {
    public static int Main(string[] args) {
      IEnumerable<Error>? errors = null;

      var parserResult =
          Parser.Default.ParseArguments(
                    args,
                    typeof(AnimalCrossingOptions),
                    typeof(OcarinaOfTime3dOptions),
                    typeof(Pikmin1Options),
                    typeof(Pikmin2Options),
                    typeof(SuperMarioSunshineOptions),
                    typeof(DebugOptions))
                .WithParsed((AnimalCrossingOptions automaticOpts) => {
                  new AnimalCrossingExtractor().ExtractAll();
                })
                .WithParsed((OcarinaOfTime3dOptions automaticOpts) => {
                  new OcarinaOfTime3dExtractor().ExtractAll();
                })
                .WithParsed((Pikmin1Options automaticOpts) => {
                  new Pikmin1Extractor().ExtractAll();
                })
                .WithParsed((Pikmin2Options manualOpts) => {
                  new Pikmin2Extractor().ExtractAll();
                })
                .WithParsed((SuperMarioSunshineOptions debugOpts) => {
                  new SuperMarioSunshineExtractor().ExtractAll();
                })
                .WithParsed((DebugOptions debugOptions) => {})
                .WithNotParsed(parseErrors => errors = parseErrors);

      if (errors != null) {
        var helpText = HelpText.AutoBuild(parserResult);
        helpText.AutoHelp = true;

        throw new Exception();
      }

      return 0;
    }
  }
}