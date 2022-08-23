using CommandLine;
using CommandLine.Text;

using uni.debug;
using uni.games.animal_crossing;
using uni.games.battalion_wars_1;
using uni.games.glover;
using uni.games.halo_wars;
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
using uni.ui;


namespace uni.cli {
  public class Cli {
    [STAThread]
    public static int Main(string[] args) {
      IEnumerable<Error>? errors = null;

      ConsoleUtil.ShowConsole();
      var parserResult =
          Parser.Default.ParseArguments(
                    args,
                    typeof(UiOptions),
                    typeof(AnimalCrossingOptions),
                    typeof(BattalionWarsOptions),
                    typeof(HaloWarsOptions),
                    typeof(GloverOptions),
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
                .WithParsed((UiOptions _) => {
                  DesignModeUtil.InDesignMode = false;
                  ApplicationConfiguration.Initialize();
                  Application.Run(new UniversalModelExtractorForm());
                })
                .WithParsed((AnimalCrossingOptions _) => {
                  new AnimalCrossingExtractor().ExtractAll();
                })
                .WithParsed((BattalionWarsOptions _) => {
                  new BattalionWars1Extractor().ExtractAll();
                })
                .WithParsed((HaloWarsOptions _) => {
                  new HaloWarsExtractor().ExtractAll();
                })
                .WithParsed((GloverOptions _) => {
                  new GloverExtractor().ExtractAll();
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
                .WithParsed((DebugOptions _) => {
                  /*var window = new DebugWindow();
                  window.Run();*/
                })
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