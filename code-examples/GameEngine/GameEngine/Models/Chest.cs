namespace GameEngine.Models;

public class Chest
{
    public string Name { get; set; } = string.Empty;
    public Position Position { get; set; } = new();
    public bool Opened { get; set; }
    public Inventory Inventory { get; set; } = new();
}
