using AutoQuest.Engine;

using Spectre.Console;

namespace  AutoQuest.Terminal;

public static class Program
{
    public static void Main(string[] args)
    {
        // load
        AnsiConsole.MarkupLine("[yellow italic]Reluctantly waking up...[/]");
        var engine = new GameEngine();

        // temporary hook to get output from the engine...
        void Log(string message) => AnsiConsole.MarkupLine(message);
        
        // kick the engine...
        engine.Initialize(Log);
        do
        {
            engine.DoTick(Log);
        } while (engine.Player.State != PlayerState.Dead 
                 && engine.Location < engine.Destination);

        Log(string.Empty);
        var player = engine.Player;
        string stateColor = player.State == PlayerState.Alive ? "green" : "red";
        Log($"[gray]Player [/][cyan bold]{player.Name}[/][gray] is [/][{stateColor}]{player.State}[/][gray]![/]");
        Log($"[gray]Travelled: [/][yellow bold]{engine.Location}[/][gray] KMs and got [/][blue bold]{player.Experience}[/][gray] xp![/]");
        
        // complete!
        Log("[white bold underline]Complete.[/]");
    }
}

