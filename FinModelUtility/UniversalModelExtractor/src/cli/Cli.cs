using System;
using System.Collections.Generic;

using CommandLine;
using CommandLine.Text;

using uni.games.animal_crossing;
using uni.games.luigis_mansion;
using uni.games.mario_kart_double_dash;
using uni.games.ocarina_of_time_3d;
using uni.games.paper_mario_the_thousand_year_door;
using uni.games.pikmin_1;
using uni.games.pikmin_2;
using uni.games.super_mario_sunshine;
using uni.games.wind_waker;

namespace uni.cli {
  public class Cli {
    public static int Main(string[] args) {
      IEnumerable<Error>? errors = null;

      var parserResult =
          Parser.Default.ParseArguments(
                    args,
                    typeof(AnimalCrossingOptions),
                    typeof(LuigisMansionOptions),
                    typeof(MarioKartDoubleDashOptions),
                    typeof(OcarinaOfTime3dOptions),
                    typeof(PaperMarioTheThousandYearDoorOptions),
                    typeof(Pikmin1Options),
                    typeof(Pikmin2Options),
                    typeof(SuperMarioSunshineOptions),
                    typeof(WindWakerOptions),
                    typeof(DebugOptions))
                .WithParsed((AnimalCrossingOptions automaticOpts) => {
                  new AnimalCrossingExtractor().ExtractAll();
                })
                .WithParsed((LuigisMansionOptions automaticOpts) => {
                  new LuigisMansionExtractor().ExtractAll();
                })
                .WithParsed((MarioKartDoubleDashOptions automaticOpts) => {
                  new MarioKartDoubleDashExtractor().ExtractAll();
                })
                .WithParsed((OcarinaOfTime3dOptions automaticOpts) => {
                  new OcarinaOfTime3dExtractor().ExtractAll();
                })
                .WithParsed((PaperMarioTheThousandYearDoorOptions automaticOpts) => {
                  new PaperMarioTheThousandYearDoorExtractor().ExtractAll();
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
                .WithParsed((WindWakerOptions debugOpts) => {
                  new WindWakerExtractor().ExtractAll();
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