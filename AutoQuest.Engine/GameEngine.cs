using System;

using AutoQuest.Engine.Models;

namespace AutoQuest.Engine;

/// <summary>
/// This manages the main game elements, takes the game configuration, and progresses
/// characters.
/// </summary>
public class GameEngine
{
    public Player Player { get; protected set; } = new Player();

    public int Location { get; protected set; } = 0;
    
    /// <summary>
    /// The main program loop that iterates the game along one tick in game time.
    /// </summary>
    /// <param name="logger">The method to log status to.</param>
    public void DoTick(Action<string> logger)
    {
        #region Roll character
        // map story progress {
            // generate story {
                // Prologue
                // Act 1
                // Act 2
                // Act 3
                // Epilogue
            // }
            
            // start player at story point 0;
            
            // determine story point completion criteria?
        // }
        #endregion

        // start in origin
        int startPos = 0;
        int tickTravel = 1;
        int destination = 10;
        Location = startPos;
        do
        {
            logger($"[gray]Player [/][cyan bold]{Player.Name}[/][gray]: H=[/][green bold]{Player.Health}[/][gray], Exp=[/][darkgoldenrod]{Player.Experience}[/]");

            // go to place (determine distance to travel)
            var from = Location;
            var to = Location + tickTravel;
            logger($"[gray italic]Moving [yellow]{tickTravel}[/] KM [yellow]{to}[/]...[/]");

            Location = to;
            
            // for each unit travelled [one unit per tick] {
            // random chance of encounter (50%)
            // if encounter {
            // do {
            // hit/take damage
            // } until (dead || monster killed)
            
            #region Player respawn?
            // if (dead) {
            // return to origin, lose experience
            // }

            // gain experience/gold
            // }

            // rest(?)
            #endregion

        // } until reached destination
        } while (Player.State != PlayerState.Dead && Location < destination);

        logger(string.Empty);
        string stateColor = Player.State == PlayerState.Alive ? "green" : "red";
        logger($"[gray]Player [/][cyan bold]{Player.Name}[/][gray] is [/][{stateColor}]{Player.State}[/][gray]![/]");
        logger($"[gray]Travelled: [/][yellow bold]{Location}[/][gray] KMs and got [/][blue bold]{Player.Experience}[/][gray] xp![/]");

        #region Future features...
        // --------------------------------------
        // TODOs
        // --------------------------------------
        // * player attributes
        //      * [applying!] player attributes
        // * battle system
        //      * monster generation
        // * story system
        // * gear system
        // * player buffs/debuffs
        // * player experience/levelling
        #endregion
    }
}