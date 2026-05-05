using System;

using AutoQuest.Engine.Models;

namespace AutoQuest.Engine;

/// <summary>
/// This manages the main game elements, takes the game configuration, and progresses
/// characters.
/// </summary>
public class GameEngine
{
    /// <summary>
    /// The default constructor used to initialize the game engine with the provided game view.
    /// </summary>
    /// <param name="view"></param>
    public GameEngine(GameView view)
    {
        View = view;
    }

    #region Fields
    private readonly Random _random = new Random();
    #endregion

    #region Properties
    /// <summary>
    /// Gets/sets the game's details that are available to the presentation engine.
    /// </summary>
    public GameView View { get; protected set; }

    public bool Running
        => View.Player.State != QuiddityState.Dead
           && View.Location < View.Destination;
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
        View.TickTravel = 1;
        
        logger("[italic gray]Discovering id...[/]");
        View.Location = 0;
        
        logger("[italic gray]Dereferencing Suissac...[/]");
        View.Destination = 10;
        
        logger("[italic gray]Initiating baryogenesis...[/]");
        View.Player = new Player();
        View.Player.Mode = PlayerMode.Travel;
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

        var to = View.Location + View.TickTravel;     // potential until after combat...
        if (View.Player.Mode == PlayerMode.Travel)
        {
            // go to place (determine distance to travel)
            logger($"[italic]Moving [yellow]{View.TickTravel}[/] KM to position [yellow]{to}[/]...[/]");
            // for each unit travelled [one unit per tick] {
            // random chance of encounter (X%)
            bool encounter = _random.Next(10) <= 3;
            if (encounter)
            {
                View.CombatGroup.Add(new Combatant("Foddear"));
                View.Player.Mode = PlayerMode.Combat;
                logger($"[gray italic][cyan]{View.Player.Name}[/] encountered a monster![/]");
            }
        }

        if (View.Player.Mode == PlayerMode.Combat)
        {
            // take a turn at combat

            int playerHitDamage = _random.Next(0, 6);
            int combatantHitDamage = _random.Next(0, 2);

            var combatant = View.CombatGroup[0];
            View.Player.Health -= combatantHitDamage;
            combatant.Health -= playerHitDamage;

            logger($"[cyan]{View.Player.Name}[/][olive]({View.Player.Health})[/] deals [red]{playerHitDamage}[/] to [darkgoldenrod]{combatant.Name}[/][olive]({combatant.Health})[/]");
            logger($"[darkgoldenrod]{combatant.Name}[/][olive]({combatant.Health})[/] deals [red]{combatantHitDamage}[/] to [cyan]{View.Player.Name}[/][olive]({View.Player.Health})[/]");

            // did anyone die
            if (View.Player.State == QuiddityState.Dead)
            {
                logger($"[cyan]{View.Player.Name}[/] has died :(");
                return;
            }
            if (combatant.State == QuiddityState.Dead)
            {
                var xp = _random.Next(1, 3);
                View.Player.Experience += xp;
                logger($"[darkgoldenrod]{combatant.Name}[/] is dead, [cyan]{View.Player.Name}[/] gains [blue]{xp}[/] experience.");
                
                // remove the combatant from the group and continue travelling
                View.CombatGroup.Clear();
                View.Player.Mode = PlayerMode.Travel;
            }
        }

        // only move after encounter is finished
        if (View.Player.Mode == PlayerMode.Travel)
        {
            View.Location = to;
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