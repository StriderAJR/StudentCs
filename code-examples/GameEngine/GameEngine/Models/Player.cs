namespace GameEngine.Models;

public class Player
{
    public string Name { get; set; } = string.Empty;
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Gold { get; set; }
    public Position Position { get; set; } = new();
    public Inventory Inventory { get; set; } = new();
}
