namespace AutoQuest.Engine;

/// <summary>
/// The different states a <see cref="Models.Player"/> can be in.
/// </summary>
public enum PlayerState
{
    /// <summary>
    /// The state is unknown or unspecified.
    /// </summary>
    None,
    
    /// <summary>
    /// The player is alive.
    /// </summary>
    Alive,
    
    /// <summary>
    /// The player has died.
    /// </summary>
    Dead
}