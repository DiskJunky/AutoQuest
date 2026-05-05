
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AutoQuest.Engine.Models
{
    /// <summary>
    /// This contains the main game elements and state that can be updated or queried by the presentation engine.
    /// </summary>
    public class GameView
    {
        #region Fields
        private readonly object _threadSafeLock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// The default constructor used to initialize the game view with default values. The
        /// <see cref="LogFile"/> is also initialized.
        /// </summary>
        public GameView()
        {
            var name = Assembly.GetEntryAssembly().GetName().Name;
            LogFile = Path.GetFullPath($"{name}_{System.DateTimeOffset.Now:yyyyMMdd_HHmmss}.log");
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets the player details.
        /// </summary>
        public Player Player { get; internal set; } = new Player();

        /// <summary>
        /// Gets/sets the current combat group that the player is interacting with.
        /// </summary>
        public List<Combatant> CombatGroup { get; internal set; } = new List<Combatant>();

        /// <summary>
        /// Gets sets the current location of the player.
        /// </summary>
        public int Location { get; internal set; } = 0;

        /// <summary>
        /// Gets/sets the distance the player can travel in one tick of game time.
        /// </summary>
        public int TickTravel { get; internal set; } = 1;

        /// <summary>
        /// Gets/sets the destination the player is trying to reach.
        /// </summary>
        public int Destination { get; internal set; } = 10;

        /// <summary>
        /// The full file/path of the log file to use.
        /// </summary>
        public string LogFile { get; protected set; }

        public bool IsDirty { get; set; }

        /// <summary>
        /// Gets the activity logs to track. We don't care about thread safety here as this 'log' is
        /// intended to be a fire-and-forget stream of events - nothing critical depends on this.
        /// </summary>
        protected List<ActivityLog> ActivityLogs { get; } = new List<ActivityLog>();
        #endregion

        /// <summary>
        /// Sets the field and sets <see cref="IsDirty"/> to <c>true</c> if the value was changed.
        /// Also raises the <see cref="PropertyChanged"/> event for the property.
        /// </summary>
        /// <typeparam name="T">The data type of the field being set.</typeparam>
        /// <param name="field">The field to set.</param>
        /// <param name="value">The value to set the field to.</param>
        /// <param name="propertyName">The name of the property being set.</param>
        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            IsDirty = true;
        }

        /// <summary>
        /// This will log the specified message to the activity collection for recording/display.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="args">Any arguments to format <paramref name="message"/> with.</param>
        public void Log(string message, params object[] args)
        {
            if (args != null) message = string.Format(message, args);

            const int maxLogEntries = 100;      // don't chew memory
            lock (_threadSafeLock)
            {
                ActivityLogs.Add(new ActivityLog(message));
                if (ActivityLogs.Count > maxLogEntries) ActivityLogs.RemoveAt(0);

                File.AppendAllText(LogFile, message);
            }

            IsDirty = true;
        }

        /// <summary>
        /// Gets a copy of the logs for display.
        /// </summary>
        /// <param name="maxEntries">The max entries we want.</param>
        /// <returns>A copy of the logs that can be read in a thread-safe manner.</returns>
        public List<ActivityLog> GetLatestActivityLogs(int maxEntries)
        {
            lock (_threadSafeLock)
            {
                int skip = ActivityLogs.Count > maxEntries ? ActivityLogs.Count - maxEntries : 0;
                var list = ActivityLogs.Skip(skip)
                                       .ToList();
                list.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));    // sort descending
                return list;
            }
        }
    }
}
