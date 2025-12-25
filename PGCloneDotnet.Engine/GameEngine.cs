using System;

namespace PGCloneDotNet.Engine;

/// <summary>
/// This manages the main game elements, takes the game configuration, and progresses
/// characters.
/// </summary>
public class GameEngine
{
    /// <summary>
    /// The main program loop that iterates the game along one tick in game time.
    /// </summary>
    /// <param name="logger">The method to log status to.</param>
    public void DoTick(Action<string> logger)
    {
        // roll character
        
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
        
        
        // start in origin
        // go to place (determine distance to travel)
        
        // for each unit travelled [one unit per tick] {
            // random chance of encounter (50%)
            // if encounter {
                // do {
                    // hit/take damage
                // } until (dead || monster killed)
                
                // if (dead) {
                    // return to origin, lose experience
                // }
                
                // gain experience/gold
            // }
            
            // rest(?)
        // } until reached destination
        
        // print score
        
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
    }
}