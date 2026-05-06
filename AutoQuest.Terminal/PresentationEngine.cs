using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

using AutoQuest.Engine;
using AutoQuest.Engine.Models;

using Microsoft.VisualBasic;

using Spectre.Console;

namespace AutoQuest.Terminal
{
    /// <summary>
    /// This class is used to manage and render the game's various UI elements.
    /// </summary>
    public class PresentationEngine(GameView view)
    {

        #region Private Fields
        private const int MaxDisplayedActivityLogs = 15;
        private const int UiCycleSpeed = 200;       // milliseconds
        #endregion

        #region Properties
        /// <summary>
        /// Gets the game view that contains the current state of the game to be rendered by the presentation engine.
        /// </summary>
        public GameView View { get; private set; } = view;
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
            var playerStats = new Table().NoBorder()
                                         .HideHeaders();
            playerStats.AddColumn(new TableColumn("Field"));
            playerStats.AddColumn(new TableColumn("Value"));

            var player = engine.View.Player;
            playerStats.AddRow("Name:", BuildPlayerName);
            playerStats.AddRow("Health:", BuildPlayerHealth);
            playerStats.AddRow("Exp.:", BuildPlayerExp);

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
                playerStats.Rows.Update(0, 1, new Markup(BuildPlayerName));
                playerStats.Rows.Update(1, 1, new Markup(BuildPlayerHealth));
                playerStats.Rows.Update(2, 1, new Markup(BuildPlayerExp));

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

        private string BuildPlayerName => $"[cyan bold]{View.Player.Name}[/]";
        private string BuildPlayerHealth => $"[green bold]{View.Player.Health}[/]";
        private string BuildPlayerExp => $"[blue bold]{View.Player.Experience}[/]";

        /// <summary>
        /// This will build and display the last X entries from the activity log.
        /// </summary>
        /// <param name="activity">The table to display the activity log in.</param>
        private void BuildActivityLog(Table activity)
        {
            var logItemColor = Color.Silver;
            var messageWidth = AnsiConsole.Profile.Width - 27;
            var minDimness = 0.2f;      // 0=dark, 1=bright
            var dimnessRange = 1 - minDimness;

            // get the most recent logs from newest to oldest, up to the max number of logs to display
            var logs = View.GetLatestActivityLogs(MaxDisplayedActivityLogs);
            activity.Rows.Clear();
            for (int displayRow = 0; displayRow < MaxDisplayedActivityLogs; displayRow++)
            {
                if (displayRow < logs.Count)
                {
                    // the top of the row should be bright and we want to fade to the background as each
                    // log item is older.
                    float rowDimness = (displayRow * (dimnessRange / MaxDisplayedActivityLogs));

                    // check for multi-line messages
                    var log = logs[displayRow];
                    var message = EscapeMessage(log.Message, messageWidth);
                    message = ColorizeMessage(message, rowDimness);

                    // calculate the color for this row based on the age of the log item
                    var rowColor = CalculateColor(logItemColor, AnsiConsole.Background, rowDimness);

                    // add the row item
                    var dateText = log.Timestamp.ToString(ActivityLog.DateTimeFormat);
                    var markupText = ColorizeMessage(dateText, rowDimness);
                    activity.AddRow(new Markup(markupText, rowColor),
                                    new Markup(message, rowColor));
                }
                else
                {
                    // leave an empty row if there aren't enough logs to fill the display
                    activity.AddEmptyRow();
                }
            }
        }

        /// <summary>
        /// A static list of named colors that SpectreConsole provides/supports. This is used to parse color names
        /// out of the log messages and convert them to actual RGB values that we can then dim based on the age of
        /// the log item.
        /// </summary>
        static readonly Dictionary<string, Color> SpectreColors =
            typeof(Color)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.PropertyType == typeof(Color))
                .ToDictionary(
                              f => f.Name,
                              f => (Color)f.GetValue(null)!,
                              StringComparer.OrdinalIgnoreCase
                             );

        /// <summary>
        /// This will process the bbcode-style color tags in the message and convert them to use RGB values that are
        /// dimmed based on the provided dimness factor. This allows us to have the log messages fade out as they get
        /// older, while still respecting any color tags that were included in the original message.
        /// </summary>
        /// <param name="message">The message to scan for color codes.</param>
        /// <param name="dimness">The dimness factor to apply to the colors.</param>
        /// <returns>The colorized message.</returns>
        private string ColorizeMessage(string message, float dimness = 1f)
        {

            var regex = new Regex(@"\[(?<content>[^\]/][^\]]*)\]",
                                  RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return regex.Replace(message, match =>
            {

                var content = match.Groups["content"].Value;

                // Split tokens on whitespace
                var tokens = content
                             .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                             .ToList();

                // Find the first token that matches a Spectre color
                var colorIndex = tokens.FindIndex(t => SpectreColors.ContainsKey(t));
                if (colorIndex < 0)
                    return match.Value; // no color found → unchanged

                var colorName = tokens[colorIndex];

                // if we don't have a color match, just use what was specified
                if (!SpectreColors.TryGetValue(colorName, out var color))
                    return match.Value; // unknown → leave as-is

                // if the dimness is outside our expected range, just use the color as-is
                if (dimness > 1f || dimness < 0f)
                    return match.Value;

                // calculate the rbg value using the starting and ending color ranges
                var dimmedColor = CalculateColor(color, AnsiConsole.Background, dimness);

                // Replace ONLY the color token
                tokens[colorIndex] = $"rgb({dimmedColor.R},{dimmedColor.G},{dimmedColor.B})";

                return $"[{string.Join(' ', tokens)}]";
            });
        }

        /// <summary>
        /// Calculates the RGB values for a color that is somewhere between the provided start and end colors based on the
        /// supplied <paramref name="dimness"/> value, where 0 represents the start color and 1 represents the end color.
        /// </summary>
        /// <param name="start">The starting color.</param>
        /// <param name="end">The ending color.</param>
        /// <param name="dimness">The dimness factor, where 0 is the start color and 1 is the end color.</param>
        /// <returns>The calculated color.</returns>
        private Color CalculateColor(Color start, Color end, float dimness)
        {
            var calcChannel = new Func<int, int, float, byte>((s, e, b) => (byte)(s + ((e - s) * b)));
            return new Color(
                calcChannel(start.R, end.R, dimness),
                calcChannel(start.G, end.G, dimness),
                calcChannel(start.B, end.B, dimness)
            );
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
