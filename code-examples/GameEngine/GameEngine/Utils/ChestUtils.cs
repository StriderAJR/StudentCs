using GameEngine.Models;

namespace GameEngine.Utils;

public static class ChestUtils
{
    public static Chest CreateChest(string name, Position position)
    {
        return new Chest
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Chest" : name.Trim(),
            Position = new Position { X = position.X, Y = position.Y },
            Opened = false,
            Inventory = new Inventory()
        };
    }

    public static void Open(Chest chest)
    {
        chest.Opened = true;
    }

    public static void Close(Chest chest)
    {
        chest.Opened = false;
    }

    public static bool IsOpened(Chest chest)
    {
        return chest.Opened;
    }

    public static bool AddItem(Chest chest, Item item)
    {
        return IsOpened(chest) && InventoryUtils.AddItem(chest.Inventory, item);
    }

    public static Item? TakeItem(Chest chest, string itemName, int count = 1)
    {
        if (!IsOpened(chest) || !InventoryUtils.RemoveItem(chest.Inventory, itemName, count))
        {
            return null;
        }

        return new Item { Name = itemName, Count = count };
    }
}
