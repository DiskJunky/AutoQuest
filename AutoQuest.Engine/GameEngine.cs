using System;

using AutoQuest.Engine.Models;

namespace AutoQuest.Engine;

/// <summary>
/// This manages the main game elements, takes the game configuration, and progresses
/// characters.
/// </summary>
public class GameEngine
{
    private readonly Random _random = new Random();
    
    public Player Player { get; protected set; } = new Player();

    public int Location { get; protected set; } = 0;
    public int TickTravel { get; protected set; } = 1;
    public int Destination { get; protected set; } = 10;

    public void Initialize(Action<string> logger)
    {
        logger("[italic gray]Buttering up muses...[/]");
        TickTravel = 1;
        
        logger("[italic gray]Discovering id...[/]");
        Location = 0;
        
        logger("[italic gray]Dereferencing Suissac...[/]");
        Destination = 10;
        
        logger("[italic gray]Initiating baryogenesis...[/]");
        Player = new Player();
    }
    
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

        //logger($"Player [cyan bold]{Player.Name}[/]: H=[green bold]{Player.Health}[/], Exp=[blue]{Player.Experience}[/]");

        // go to place (determine distance to travel)
        var to = Location + TickTravel;
        logger($"[italic]Moving [yellow]{TickTravel}[/] KM to position [yellow]{to}[/]...[/]");
        
        // for each unit travelled [one unit per tick] {
        // random chance of encounter (X%)
        bool encounter = _random.Next(10) <= 3; // 30%
        if (encounter)
        {
            //logger($"[gray italic][cyan]{Player.Name}[/] encountered a monster![/]");
        }

        // only move after encounter is finished
        // todo: fight state to track of the player is in the middle of something? Player state machine?
        Location = to;
        
        // if encounter {
        // do {
        // hit/take damage
        // } until (dead || monster killed)

        #region Player respawn?

        // if (dead) {
        // return to origin, lose experience
        // }

        // gain experience/gold
        // 

        // rest(?)
        Thread.Sleep(200);
        #endregion

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