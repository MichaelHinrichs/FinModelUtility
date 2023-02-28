using CommandLine;
using CommandLine.Text;

using level5.debug;

using uni.ui;


namespace uni.cli {
  public class Cli {
    [STAThread]
    public static int Main(string[] args) {
      IEnumerable<Error>? errors = null;

      var extractorOptionTypes =
          AppDomain.CurrentDomain.GetAssemblies()
                   .SelectMany(s => s.GetTypes())
                   .Where(typeof(IExtractorOptions).IsAssignableFrom);

      ConsoleUtil.ShowConsole();
      var parserResult =
          Parser.Default.ParseArguments(
                    args,
                    extractorOptionTypes
                        .Concat(new[] {
                            typeof(UiOptions), typeof(DebugOptions),
                        })
                        .ToArray())
                .WithParsed(
                    (IExtractorOptions extractorOptions)
                        => extractorOptions.CreateExtractor().ExtractAll())
                .WithParsed((UiOptions _) => {
                  DesignModeUtil.InDesignMode = false;
                  ApplicationConfiguration.Initialize();
                    Application.Run(new UniversalModelExtractorForm());
                })
                .WithParsed((DebugOptions _) => {
                  /*var window = new DebugWindow();
                  window.Run();*/
                  //new DebugProgram().Run();
                  new DebugLevel5().Run();
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