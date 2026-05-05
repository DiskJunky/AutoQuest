namespace AutoQuest.Engine.Models
{
    /// <summary>
    /// This contains details about an item that will be displayed in the activity log.
    /// </summary>
    public class ActivityLog
    {
        /// <summary>
        /// The date/time format to use when displaying the timestamp of the activity log.
        /// </summary>
        public const string DateTimeFormat = "HH:mm:ss.fff";

        /// <summary>
        /// The default constructor used to initialize the activity log with the provided message.
        /// </summary>
        /// <param name="message"></param>
        public ActivityLog(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets/sets the timestamp of the activity.
        /// </summary>
        public DateTimeOffset Timestamp { get; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets/sets the message of the activity.
        /// </summary>
        public string Message { get; protected set; }

        /// <inheritdoc/>
        public override string ToString() => $"[{Timestamp:DateTimeFormat}] {Message}";

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Timestamp, Message);
    }
}
