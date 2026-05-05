using AutoQuest.Engine;
using AutoQuest.Engine.Models;

using Spectre.Console;

namespace  AutoQuest.Terminal;

public static class Program
{
    public static void Main(string[] args)
    {
        // load
        var gameView = new GameView();
        var engine = new GameEngine(gameView);
        Render(engine);
    }

    /// <summary>
    /// This renders the game state and initiates the main game loop until the
    /// end conditions are met (<see cref="GameEngine.Running"/>).
    /// </summary>
    /// <param name="engine">The game engine to render until complete.</param>
    private static void Render(GameEngine engine)
    {
        // add the player panel
        var playerStats = new Grid();
        playerStats.AddColumns(2);

        var player = engine.View.Player;
        playerStats.AddRow("Name:", $"[cyan bold]{player.Name}[/]");
        playerStats.AddRow("Health:", $"[green bold]{player.Health}[/]");
        playerStats.AddRow("Exp.:", $"[blue bold]{player.Experience}[/]");

        var playerPnl = new Panel(playerStats).Header("Player").RoundedBorder();
        
        // add the activity panel
        var activity = new Table().Border(TableBorder.None)
            .Expand();

        activity.AddColumn("Time", c => c.Width(12));
        activity.AddColumn("Message");
        activity.AddRow(new Rule(), new Rule());
        var actHeader = new Panel(activity).Header("Activity")
            .BorderColor(Color.White)
            .RoundedBorder()
            .Expand();
        
        // create the overall layout
        var title = new FigletText(FigletFont.Default, "AutoQuest!");
        title.Justification = Justify.Center;
        var desktop = new Table().RoundedBorder()
            .BorderColor(Color.Gray)
            .Expand()
            .HideHeaders();
        desktop.AddColumn("");

        // add the sub-panels details
        desktop.AddRow(title);
        desktop.AddRow(playerPnl);
        desktop.AddRow(actHeader);
        
        // setup live updating
        AnsiConsole.Live(desktop)
            .Start(ctx =>
            {
                
                // temporary hook to get output from the engine...
                void Log(string message)
                {
                    // only display the last 5 entries of activity
                    activity.InsertRow(1, $"{DateTime.Now:HH:mm:ss.fff}", message);
                    // TODO: in order to handle fading entries, store the activity in a separate collection
                    //       and handle the rendering of it here as there doesn't appear to be a way of
                    //       querying the rendered data from the console widgets after the fact.
                    // if (activity.Rows > 2)
                    // {
                    //     activity.UpdateCell(2, 0, new Text(activity.Rows[2][0].ToString()));
                    // }
                    if (activity.Rows.Count > 16)
                    {
                        activity.Rows.RemoveAt(activity.Rows.Count  - 1);
                    }
                    
                    // TODO: update the player stats
                    // playerStats.("Health:", $"[green bold]{player.Health}[/]");
                    // playerStats.AddRow("Exp.:", $"[blue bold]{player.Experience}[/]");                    
            
                    // re-render the display
                    Thread.Sleep(200);
                    ctx.Refresh();
                }
        
                // kick the engine...
                engine.Initialize(Log);
                player = engine.View.Player;
        
                // render the display
                ctx.Refresh();
        
                do
                {
                    engine.DoTick(Log);
                } while (engine.Running);

                Log(string.Empty);
                
                string stateColor = player.State == QuiddityState.Alive ? "green" : "red";
                Log($"[gray]Player [/][cyan bold]{player.Name}[/][olive]({player.Health})[/][gray] is [/][{stateColor}]{player.State}[/][gray]![/]");
                Log($"[gray]Travelled: [/][yellow bold]{engine.View.Location}[/][gray] KMs and got [/][blue bold]{player.Experience}[/][gray] xp![/]");
            
                // only display the last 5 entries of activity
                if (activity.Rows.Count > 5)
                {
                    activity.Rows.RemoveAt(activity.Rows.Count  - 1);
                }
            
                // re-render the display
                ctx.Refresh();
            });
    }
}
