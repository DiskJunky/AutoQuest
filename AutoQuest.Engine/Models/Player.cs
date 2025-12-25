namespace AutoQuest.Engine.Models;

public class Player
{
    private int _health;
    
    public Player(string name = "Jimmy", 
                  int health = 10)
    {
        Name = name;
        Health = health;
    }
            
    public string Name { get; protected set; }
    public int Experience { get; protected set; } = 0;
    public PlayerState State { get; protected set; } = PlayerState.Alive;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                State = PlayerState.Dead;
                _health = 0;
            }
        }
    }
}