using System;

using AutoQuest.Engine.Models;

namespace AutoQuest.Engine;

/// <summary>
/// This manages the main game elements, takes the game configuration, and progresses
/// characters.
/// </summary>
public class GameEngine
{
    #region Fields
    private readonly Random _random = new Random();
    #endregion
    
    #region Properties
    public Player Player { get; protected set; } = new Player();

    public List<Combatant> CombatGroup { get; protected set; } = new List<Combatant>();

    public int Location { get; protected set; } = 0;

    public int TickTravel { get; protected set; } = 1;
    
    public int Destination { get; protected set; } = 10;

    public bool Running
        => Player.State != QuiddityState.Dead
           && Location < Destination;
    #endregion
    
    #region Methods
    /// <summary>
    /// Initializes the game engine.
    /// </summary>
    /// <param name="logger">The activity log to write out to.</param>
    public void Initialize(Action<string> logger)
    {
        logger("[yellow italic]Reluctantly waking up...[/]");
        
        logger("[italic gray]Buttering up muses...[/]");
        TickTravel = 1;
        
        logger("[italic gray]Discovering id...[/]");
        Location = 0;
        
        logger("[italic gray]Dereferencing Suissac...[/]");
        Destination = 10;
        
        logger("[italic gray]Initiating baryogenesis...[/]");
        Player = new Player();
        Player.Mode = PlayerMode.Travel;
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

        var to = Location + TickTravel;     // potential until after combat...
        if (Player.Mode == PlayerMode.Travel)
        {
            // go to place (determine distance to travel)
            logger($"[italic]Moving [yellow]{TickTravel}[/] KM to position [yellow]{to}[/]...[/]");

            // for each unit travelled [one unit per tick] {
            // random chance of encounter (X%)
            bool encounter = _random.Next(10) <= 3;
            if (encounter)
            {
                CombatGroup.Add(new Combatant("Foddear"));
                Player.Mode = PlayerMode.Combat;
                logger($"[gray italic][cyan]{Player.Name}[/] encountered a monster![/]");
            }
        }

        if (Player.Mode == PlayerMode.Combat)
        {
            // take a turn at combat

            int playerHitDamage = _random.Next(0, 6);
            int combatantHitDamage = _random.Next(0, 2);

            var combatant = CombatGroup[0];
            Player.Health -= combatantHitDamage;
            combatant.Health -= playerHitDamage;

            logger($"[cyan]{Player.Name}[/][olive]({Player.Health})[/] deals [red]{playerHitDamage}[/] to [darkgoldenrod]{combatant.Name}[/][olive]({combatant.Health})[/]");
            logger($"[darkgoldenrod]{combatant.Name}[/][olive]({combatant.Health})[/] deals [red]{combatantHitDamage}[/] to [cyan]{Player.Name}[/][olive]({Player.Health})[/]");

            // did anyone die
            if (Player.State == QuiddityState.Dead)
            {
                logger($"[cyan]{Player.Name}[/] has died :(");
                return;
            }
            if (combatant.State == QuiddityState.Dead)
            {
                var xp = _random.Next(1, 3);
                Player.Experience += xp;
                logger($"[darkgoldenrod]{combatant.Name}[/] is dead, [cyan]{Player.Name}[/] gains [blue]{xp}[/] experience.");
                
                // remove the combatant from the group and continue travelling
                CombatGroup.Clear();
                Player.Mode = PlayerMode.Travel;
            }
        }

        // only move after encounter is finished
        if (Player.Mode == PlayerMode.Travel)
        {
            Location = to;
        }
        
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
        //Thread.Sleep(200);
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
    #endregion
}