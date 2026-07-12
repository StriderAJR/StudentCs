using GameEngine.Models;
using GameEngine.Utils;

namespace GameEngine;

public class Program
{
    public static void Main()
    {
        var map = MapUtils.CreateDefaultMap();
        var center = MapUtils.CreateCenterPosition(map);
        var playerName = ReadText("Введите имя игрока (Enter — Hero): ");
        var player = PlayerUtils.CreateDefaultPlayer(playerName);
        var chestPosition = new Position { X = center.X + 1, Y = center.Y };
        var chest = ChestUtils.CreateChest("Сундук у фонтана", chestPosition);

        ChestUtils.Open(chest);
        ChestUtils.AddItem(chest, new Item { Name = "Sword", Count = 1 });
        ChestUtils.AddItem(chest, new Item { Name = "Potion", Count = 1 });
        ChestUtils.Close(chest);

        PrintWelcome(map, player, chest);

        var isRunning = true;
        while (isRunning)
        {
            PrintMenu();
            var command = ReadInt("Выберите действие: ");

            switch (command)
            {
                case 1:
                    ShowPlayerInformation(player, map, chest);
                    break;
                case 2:
                    HealPlayer(player);
                    break;
                case 3:
                    DamagePlayer(player);
                    break;
                case 4:
                    AddPlayerGold(player);
                    break;
                case 5:
                    SpendPlayerGold(player);
                    break;
                case 6:
                    AddInventoryItem(player);
                    break;
                case 7:
                    FindInventoryItem(player);
                    break;
                case 8:
                    RemoveInventoryItem(player);
                    break;
                case 9:
                    ChangeChestState(player, chest);
                    break;
                case 10:
                    PutItemIntoChest(player, chest);
                    break;
                case 11:
                    TakeItemFromChest(player, chest);
                    break;
                case 12:
                    CheckCoordinates(player, map);
                    break;
                case 13:
                    ShowDistanceToCenter(player, map, chest);
                    break;
                case 0:
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Такого пункта меню нет.");
                    break;
            }

            Console.WriteLine();
        }

        Console.WriteLine("Игра завершена. До встречи!");
    }

    private static void PrintWelcome(GameMap map, Player player, Chest chest)
    {
        Console.WriteLine();
        Console.WriteLine("=== Учебный игровой движок ===");
        Console.WriteLine($"Карта: {map.Width} x {map.Height}");
        Console.WriteLine($"Игрок {player.Name} начинает в точке ({player.Position.X}, {player.Position.Y}).");
        Console.WriteLine($"Рядом находится: {chest.Name}.");

        if (InventoryUtils.Contains(player.Inventory, "Potion"))
        {
            Console.WriteLine("В стартовом инвентаре есть зелье.");
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("1  Показать информацию об игроке");
        Console.WriteLine("2  Лечение");
        Console.WriteLine("3  Получить урон");
        Console.WriteLine("4  Добавить золото");
        Console.WriteLine("5  Потратить золото");
        Console.WriteLine("6  Добавить предмет");
        Console.WriteLine("7  Найти предмет");
        Console.WriteLine("8  Удалить предмет");
        Console.WriteLine("9  Открыть сундук");
        Console.WriteLine("10 Положить предмет в сундук");
        Console.WriteLine("11 Забрать предмет из сундука");
        Console.WriteLine("12 Проверить координаты");
        Console.WriteLine("13 Расстояние до центра карты");
        Console.WriteLine("0  Выход");
    }

    private static void ShowPlayerInformation(Player player, GameMap map, Chest chest)
    {
        Console.WriteLine("--- Игрок ---");
        Console.WriteLine($"Имя: {player.Name}");
        Console.WriteLine($"Здоровье: {player.Health}/{player.MaxHealth}");
        Console.WriteLine($"Золото: {player.Gold}/{GameSettings.MaxGold}");
        Console.WriteLine($"Позиция: ({player.Position.X}, {player.Position.Y})");
        Console.WriteLine($"Позиция на карте: {MapUtils.IsInside(map, player.Position.X, player.Position.Y)}");
        Console.WriteLine($"Игрок мёртв: {PlayerUtils.IsDead(player)}");
        Console.WriteLine($"Можно потратить 10 золота: {PlayerUtils.HasEnoughGold(player, 10)}");
        Console.WriteLine($"Инвентарь пуст: {InventoryUtils.IsEmpty(player.Inventory)}");
        Console.WriteLine($"Инвентарь заполнен: {InventoryUtils.IsFull(player.Inventory)}");
        Console.WriteLine($"Сундук открыт: {ChestUtils.IsOpened(chest)}");
        Console.WriteLine($"Расстояние до сундука: {MapUtils.Distance(player.Position, chest.Position):F1}");

        PrintInventory("Инвентарь игрока", player.Inventory);
        PrintInventory("Инвентарь сундука", chest.Inventory);
    }

    private static void HealPlayer(Player player)
    {
        var amount = ReadPositiveInt("Количество здоровья: ");
        var healthBefore = player.Health;

        PlayerUtils.Heal(player, amount);

        if (PlayerUtils.IsDead(player))
        {
            Console.WriteLine("Нельзя лечить погибшего игрока.");
            return;
        }

        Console.WriteLine($"Здоровье: {healthBefore} -> {player.Health}.");
    }

    private static void DamagePlayer(Player player)
    {
        var amount = ReadPositiveInt("Количество урона: ");
        PlayerUtils.TakeDamage(player, amount);

        Console.WriteLine($"Здоровье игрока: {player.Health}/{player.MaxHealth}.");
        if (PlayerUtils.IsDead(player))
        {
            Console.WriteLine("Игрок погиб. Лечение больше недоступно.");
        }
    }

    private static void AddPlayerGold(Player player)
    {
        var amount = ReadPositiveInt("Количество золота: ");
        var goldBefore = player.Gold;

        PlayerUtils.AddGold(player, amount);

        Console.WriteLine($"Золото: {goldBefore} -> {player.Gold}.");
        if (PlayerUtils.HasEnoughGold(player, GameSettings.MaxGold))
        {
            Console.WriteLine("Достигнут максимальный запас золота.");
        }
    }

    private static void SpendPlayerGold(Player player)
    {
        var amount = ReadPositiveInt("Сколько золота потратить: ");

        if (!PlayerUtils.HasEnoughGold(player, amount))
        {
            Console.WriteLine("Недостаточно золота.");
            return;
        }

        var wasSpent = PlayerUtils.SpendGold(player, amount);
        Console.WriteLine(wasSpent
            ? $"Потрачено {amount}. Осталось: {player.Gold}."
            : "Не удалось потратить золото.");
    }

    private static void AddInventoryItem(Player player)
    {
        var item = ReadItem();
        var alreadyContained = InventoryUtils.Contains(player.Inventory, item.Name);

        if (InventoryUtils.AddItem(player.Inventory, item))
        {
            var storedItem = InventoryUtils.FindItem(player.Inventory, item.Name);
            Console.WriteLine($"Предмет добавлен. Теперь {storedItem!.Name}: {storedItem.Count} шт.");

            if (!alreadyContained && InventoryUtils.IsFull(player.Inventory))
            {
                Console.WriteLine("Инвентарь теперь заполнен.");
            }
        }
        else
        {
            Console.WriteLine("Не удалось добавить предмет: инвентарь заполнен или данные некорректны.");
        }
    }

    private static void FindInventoryItem(Player player)
    {
        var name = ReadRequiredText("Название предмета: ");
        var item = InventoryUtils.FindItem(player.Inventory, name);

        if (InventoryUtils.Contains(player.Inventory, name) && item is not null)
        {
            Console.WriteLine($"Найден предмет: {item.Name}, количество: {item.Count}.");
        }
        else
        {
            Console.WriteLine("Предмет не найден.");
        }
    }

    private static void RemoveInventoryItem(Player player)
    {
        var name = ReadRequiredText("Название предмета: ");
        var count = ReadPositiveInt("Количество: ");

        if (!InventoryUtils.Contains(player.Inventory, name))
        {
            Console.WriteLine("Такого предмета в инвентаре нет.");
            return;
        }

        if (InventoryUtils.RemoveItem(player.Inventory, name, count))
        {
            Console.WriteLine("Предмет удалён.");
            Console.WriteLine($"Инвентарь пуст: {InventoryUtils.IsEmpty(player.Inventory)}.");
        }
        else
        {
            Console.WriteLine("Нельзя удалить больше предметов, чем есть.");
        }
    }

    private static void ChangeChestState(Player player, Chest chest)
    {
        if (!MapUtils.IsNear(player.Position, chest.Position, 1.5))
        {
            Console.WriteLine("Сундук слишком далеко. Подойдите к нему через пункт проверки координат.");
            return;
        }

        if (ChestUtils.IsOpened(chest))
        {
            ChestUtils.Close(chest);
            Console.WriteLine("Сундук закрыт.");
        }
        else
        {
            ChestUtils.Open(chest);
            Console.WriteLine("Сундук открыт.");
        }
    }

    private static void PutItemIntoChest(Player player, Chest chest)
    {
        if (!ChestUtils.IsOpened(chest))
        {
            Console.WriteLine("Сначала откройте сундук.");
            return;
        }

        var name = ReadRequiredText("Название предмета: ");
        var count = ReadPositiveInt("Количество: ");

        if (!InventoryUtils.RemoveItem(player.Inventory, name, count))
        {
            Console.WriteLine("В инвентаре игрока недостаточно предметов.");
            return;
        }

        var wasAdded = ChestUtils.AddItem(chest, new Item { Name = name, Count = count });
        if (wasAdded)
        {
            Console.WriteLine("Предмет положен в сундук.");
        }
        else
        {
            InventoryUtils.AddItem(player.Inventory, new Item { Name = name, Count = count });
            Console.WriteLine("В сундуке нет места. Предмет возвращён игроку.");
        }
    }

    private static void TakeItemFromChest(Player player, Chest chest)
    {
        if (!ChestUtils.IsOpened(chest))
        {
            Console.WriteLine("Сначала откройте сундук.");
            return;
        }

        var name = ReadRequiredText("Название предмета: ");
        var count = ReadPositiveInt("Количество: ");
        var item = ChestUtils.TakeItem(chest, name, count);

        if (item is null)
        {
            Console.WriteLine("В сундуке нет нужного количества предметов.");
            return;
        }

        if (InventoryUtils.AddItem(player.Inventory, item))
        {
            Console.WriteLine("Предмет перенесён в инвентарь игрока.");
        }
        else
        {
            ChestUtils.AddItem(chest, item);
            Console.WriteLine("Инвентарь игрока заполнен. Предмет возвращён в сундук.");
        }
    }

    private static void CheckCoordinates(Player player, GameMap map)
    {
        var x = ReadInt("Координата X: ");
        var y = ReadInt("Координата Y: ");
        var requestedPosition = new Position { X = x, Y = y };

        Console.WriteLine($"Точка внутри карты: {MapUtils.IsInside(map, x, y)}.");
        var safePosition = MapUtils.ClampPosition(map, requestedPosition);
        Console.WriteLine($"Допустимая точка: ({safePosition.X}, {safePosition.Y}).");

        PlayerUtils.MoveTo(player, safePosition);
        Console.WriteLine($"Игрок перемещён в ({player.Position.X}, {player.Position.Y}).");
    }

    private static void ShowDistanceToCenter(Player player, GameMap map, Chest chest)
    {
        var center = MapUtils.CreateCenterPosition(map);
        var distanceToCenter = MapUtils.Distance(player.Position, center);
        var distanceToChest = MapUtils.Distance(player.Position, chest.Position);

        Console.WriteLine($"Центр карты: ({center.X}, {center.Y}).");
        Console.WriteLine($"Расстояние до центра: {distanceToCenter:F2}.");
        Console.WriteLine($"Расстояние до сундука: {distanceToChest:F2}.");
        Console.WriteLine($"Игрок в центре или рядом с ним: {MapUtils.IsNear(player.Position, center, 1)}.");
    }

    private static void PrintInventory(string title, Inventory inventory)
    {
        Console.WriteLine($"--- {title} ---");
        if (InventoryUtils.IsEmpty(inventory))
        {
            Console.WriteLine("(пусто)");
            return;
        }

        foreach (var item in inventory.Items)
        {
            Console.WriteLine($"{item.Name}: {item.Count} шт.");
        }
    }

    private static Item ReadItem()
    {
        return new Item
        {
            Name = ReadRequiredText("Название предмета: "),
            Count = ReadPositiveInt("Количество: ")
        };
    }

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    private static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            var value = ReadText(prompt).Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Console.WriteLine("Введите непустое значение.");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            var value = ReadText(prompt);
            if (int.TryParse(value, out var number))
            {
                return number;
            }

            Console.WriteLine("Введите целое число.");
        }
    }

    private static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            var number = ReadInt(prompt);
            if (number > 0)
            {
                return number;
            }

            Console.WriteLine("Введите число больше нуля.");
        }
    }
}
