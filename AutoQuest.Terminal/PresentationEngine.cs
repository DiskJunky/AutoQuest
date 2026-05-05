using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AutoQuest.Engine;
using AutoQuest.Engine.Models;

using Microsoft.VisualBasic;

using Spectre.Console;

namespace AutoQuest.Terminal
{
    /// <summary>
    /// This class is used to manage and render the game's various UI elements.
    /// </summary>
    public class PresentationEngine
    {
        #region Private Fields
        private const int MaxDisplayedActivityLogs = 15;
        private const int UiCycleSpeed = 200;       // milliseconds
        #endregion

        #region Properties
        /// <summary>
        /// Gets the game view that contains the current state of the game to be rendered by the presentation engine.
        /// </summary>
        public GameView View { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// This creates the markup for the specified message.
        /// </summary>
        /// <param name="message">The message to create markup for.</param>
        /// <param name="foreColor">The foreground color of the text.</param>
        /// <param name="backColor">The background color of the text.</param>
        /// <param name="style">The decoration style of the text.</param>
        /// <returns>The formatted markup string.</returns>
        public string BuildMarkup(string message, 
                                  Color foreColor = default, 
                                  Color backColor = default,
                                  Decoration style = Decoration.None)
        {
            return null;
        }

        /// <summary>
        /// This renders the game state and initiates the main game loop until the
        /// end conditions are met (<see cref="GameEngine.Running"/>).
        /// </summary>
        /// <param name="engine">The game engine to render until complete.</param>
        public void Render(GameEngine engine)
        {
            View = engine.View;

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

            var refresh = new Action<LiveDisplayContext>(ctx =>
            {
                BuildActivityLog(activity);
                ctx.Refresh();
            });

            var Log = new Action<string>(m => View.Log(m));

            // setup live updating
            AnsiConsole.Live(desktop)
                .Start(ctx =>
                {
                    // track execution time across each game tick
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // kick the engine...
                    engine.Initialize(Log);
                    player = engine.View.Player;

                    // render the display
                    refresh(ctx);

                    do
                    {
                        // iterate in the game
                        engine.DoTick(Log);

                        // only redraw when we have to
                        if (engine.View.IsDirty)
                        {
                            refresh(ctx);
                        }

                        // wait the cycle time minus whatever time has already been elapsed
                        // to keep the UI updates on a consistent cycle time
                        var waitTime = Math.Max(0 /*min*/, UiCycleSpeed - stopwatch.ElapsedMilliseconds);
                        Thread.Sleep((int)waitTime);
                        stopwatch.Restart();
                    } while (engine.Running);

                    //string stateColor = player.State == QuiddityState.Alive ? "green" : "red";
                    //Log($"[gray]Player [/][cyan bold]{player.Name}[/][olive]({player.Health})[/][gray] is [/][{stateColor}]{player.State}[/][gray]![/]");
                    //Log($"[gray]Travelled: [/][yellow bold]{engine.View.Location}[/][gray] KMs and got [/][blue bold]{player.Experience}[/][gray] xp![/]");

                    //// only display the last 5 entries of activity
                    //if (activity.Rows.Count > 5)
                    //{
                    //    activity.Rows.RemoveAt(activity.Rows.Count - 1);
                    //}

                    // re-render the display to show final game state
                    refresh(ctx);
                });
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This will build and display the last X entries from the activity log.
        /// </summary>
        /// <param name="activity">The table to display the activity log in.</param>
        private void BuildActivityLog(Table activity)
        {
            var logItemColor = Color.Silver;
            int newestBrightness = logItemColor.R;      // gray has the same value for each R, G, and B, so we only need one channel
            int oldestBrightness = 40;

            // calculate the message width
            var messageWidth = AnsiConsole.Profile.Width - 27;

            // get the most recent logs from newest to oldest, up to the max number of logs to display
            var logs = View.GetLatestActivityLogs(MaxDisplayedActivityLogs);
            activity.Rows.Clear();
            for (int displayRow = 0; displayRow < MaxDisplayedActivityLogs; displayRow++)
            {
                if (displayRow < logs.Count)
                {
                    // fade the date/time as the logs get older to visually represent the ordering
                    int scale = newestBrightness - (displayRow * ((newestBrightness - oldestBrightness) / MaxDisplayedActivityLogs));

                    // check for multi-line messages
                    var log = logs[displayRow];
                    var message = EscapeMessage(log.Message, messageWidth);

                    // add the row item
                    var dateText = log.Timestamp.ToString(ActivityLog.DateTimeFormat);
                    var markupText = $"[rgb({scale},{scale},{scale})]{dateText}[/]";
                    activity.AddRow(new Markup(markupText, logItemColor),
                                    new Markup(message, logItemColor));
                }
                else
                {
                    // leave an empty row if there aren't enough logs to fill the display
                    activity.AddEmptyRow();
                }
            }
        }

        /// <summary>
        /// Cleans up the raw message text and returns something we can directly render.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <param name="messageWidth">The maximum width of the message.</param>
        /// <returns>The cleaned-up message.</returns>
        private string EscapeMessage(string message, int messageWidth)
        {
            // strip down to a single line of multiple
            var lines = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            message = lines[0];    // we can only deal with the first

            // ensure we don't exceed the message column width
            if (message.Length > messageWidth)
            {
                message = message.Substring(0, messageWidth - 3) + "...";
            }

            // auto-highlight exceptions...
            if (message.Contains("Exception"))
            {
                message = $"[{Color.Red}]{message}[/]";
            }

            // truncating the full message may have resulted in bad markup, fallback to non-markup if we can't parse it
            try
            {
                _ = new Markup(message);
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("Unbalanced markup stack"))
            {
                // Fallback: escape and render as plain text
                message = message.Replace("[Red]", string.Empty)
                                 .Replace("[/]", string.Empty);
                message = $"[{Color.Red}]{Markup.Escape(message)}[/]";
            }

            // return the UI-safe message
            return message;
        }
        #endregion
    }
}
