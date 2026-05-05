using AutoQuest.Engine;
using AutoQuest.Engine.Models;

namespace  AutoQuest.Terminal;

public static class Program
{
    /// <summary>
    /// Start the program through initializing and start rendering the ui.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        // initialize state
        var gameView = new GameView();
        var engine = new GameEngine(gameView);
        
        // render the UI
        var ui = new PresentationEngine();
        ui.Render(engine);
    }
}
