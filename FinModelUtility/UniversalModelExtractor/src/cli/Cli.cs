using System;
using System.Collections.Generic;

using CommandLine;
using CommandLine.Text;

using uni.games.animal_crossing;
using uni.games.battalion_wars;
using uni.games.luigis_mansion;
using uni.games.luigis_mansion_3d;
using uni.games.mario_kart_double_dash;
using uni.games.ocarina_of_time;
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
                    typeof(BattalionWarsOptions),
                    typeof(LuigisMansionOptions),
                    typeof(LuigisMansion3dOptions),
                    typeof(MarioKartDoubleDashOptions),
                    typeof(OcarinaOfTime3dOptions),
                    typeof(OcarinaOfTimeOptions),
                    typeof(PaperMarioTheThousandYearDoorOptions),
                    typeof(Pikmin1Options),
                    typeof(Pikmin2Options),
                    typeof(SuperMarioSunshineOptions),
                    typeof(WindWakerOptions),
                    typeof(DebugOptions))
                .WithParsed((AnimalCrossingOptions _) => {
                  new AnimalCrossingExtractor().ExtractAll();
                })
                .WithParsed((BattalionWarsOptions _) => {
                  new BattalionWarsExtractor().ExtractAll();
                })
                .WithParsed((LuigisMansionOptions _) => {
                  new LuigisMansionExtractor().ExtractAll();
                })
                .WithParsed((LuigisMansion3dOptions _) => {
                  new LuigisMansion3dExtractor().ExtractAll();
                })
                .WithParsed((MarioKartDoubleDashOptions _) => {
                  new MarioKartDoubleDashExtractor().ExtractAll();
                })
                .WithParsed((OcarinaOfTime3dOptions _) => {
                  new OcarinaOfTime3dExtractor().ExtractAll();
                })
                .WithParsed((OcarinaOfTimeOptions _) => {
                  new OcarinaOfTimeExtractor().ExtractAll();
                })
                .WithParsed((PaperMarioTheThousandYearDoorOptions _) => {
                  new PaperMarioTheThousandYearDoorExtractor().ExtractAll();
                })
                .WithParsed((Pikmin1Options _) => {
                  new Pikmin1Extractor().ExtractAll();
                })
                .WithParsed((Pikmin2Options _) => {
                  new Pikmin2Extractor().ExtractAll();
                })
                .WithParsed((SuperMarioSunshineOptions _) => {
                  new SuperMarioSunshineExtractor().ExtractAll();
                })
                .WithParsed((WindWakerOptions _) => {
                  new WindWakerExtractor().ExtractAll();
                })
                .WithParsed((DebugOptions _) => {})
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