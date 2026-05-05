
namespace AutoQuest.Engine.Models
{
    /// <summary>
    /// This contains the main game elements and state that can be updated or queried by the presentation engine.
    /// </summary>
    public class GameView
    {
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
        #endregion
    }
}
