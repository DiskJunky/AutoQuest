using PGCloneDotNet.Engine;

using Spectre.Console;

namespace  PGCloneDotnet.Terminal;

public static class Program
{
    public static void Main(string[] args)
    {
        // load
        AnsiConsole.MarkupLine("[yellow]Loading...[/]");

        AnsiConsole.MarkupLine("Running...");
        var engine = new GameEngine();

        // temporary hook to get output from the engine...
        void Log(string message) => AnsiConsole.MarkupLine(message);
        
        // kick the engine...
        engine.DoTick(Log);
        
        // complete!
        AnsiConsole.MarkupLine("[white bold underline]Complete.[/]");
    }
}

