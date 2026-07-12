using GameEngine.Models;

namespace GameEngine.Utils;

public static class InventoryUtils
{
    public static Inventory CreateStarterInventory()
    {
        return new Inventory
        {
            Items =
            [
                new Item { Name = "Potion", Count = 2 },
                new Item { Name = "Bread", Count = 1 }
            ]
        };
    }

    public static bool IsEmpty(Inventory inventory)
    {
        return inventory.Items.Count == 0;
    }

    public static bool IsFull(Inventory inventory)
    {
        return inventory.Items.Count >= GameSettings.MaxInventorySize;
    }

    public static Item? FindItem(Inventory inventory, string name)
    {
        return inventory.Items.FirstOrDefault(item =>
            string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public static bool Contains(Inventory inventory, string name)
    {
        return FindItem(inventory, name) is not null;
    }

    public static bool AddItem(Inventory inventory, Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name) || item.Count <= 0)
        {
            return false;
        }

        var existingItem = FindItem(inventory, item.Name);
        if (existingItem is not null)
        {
            existingItem.Count += item.Count;
            return true;
        }

        if (IsFull(inventory))
        {
            return false;
        }

        inventory.Items.Add(new Item { Name = item.Name.Trim(), Count = item.Count });
        return true;
    }

    public static bool RemoveItem(Inventory inventory, string name, int count = 1)
    {
        if (count <= 0)
        {
            return false;
        }

        var item = FindItem(inventory, name);
        if (item is null || item.Count < count)
        {
            return false;
        }

        item.Count -= count;
        if (item.Count == 0)
        {
            inventory.Items.Remove(item);
        }

        return true;
    }
}
