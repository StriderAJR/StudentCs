using GameEngine.Models;

namespace GameEngine.Utils;

public static class PlayerUtils
{
    public static Player CreateDefaultPlayer(string name)
    {
        return new Player
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Hero" : name.Trim(),
            Health = GameSettings.DefaultPlayerHealth,
            MaxHealth = GameSettings.DefaultPlayerHealth,
            Gold = 0,
            Position = MapUtils.CreateCenterPosition(MapUtils.CreateDefaultMap()),
            Inventory = InventoryUtils.CreateStarterInventory()
        };
    }

    public static bool IsDead(Player player)
    {
        return player.Health <= 0;
    }

    public static void Heal(Player player, int amount)
    {
        if (amount <= 0 || IsDead(player))
        {
            return;
        }

        player.Health = Math.Min(player.MaxHealth, player.Health + amount);
    }

    public static void TakeDamage(Player player, int amount)
    {
        if (amount <= 0 || IsDead(player))
        {
            return;
        }

        player.Health = Math.Max(0, player.Health - amount);
    }

    public static void MoveTo(Player player, Position position)
    {
        player.Position = new Position
        {
            X = position.X,
            Y = position.Y
        };
    }

    public static void AddGold(Player player, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        player.Gold = Math.Min(GameSettings.MaxGold, player.Gold + amount);
    }

    public static bool SpendGold(Player player, int amount)
    {
        if (!HasEnoughGold(player, amount))
        {
            return false;
        }

        player.Gold -= amount;
        return true;
    }

    public static bool HasEnoughGold(Player player, int amount)
    {
        return amount >= 0 && player.Gold >= amount;
    }
}
