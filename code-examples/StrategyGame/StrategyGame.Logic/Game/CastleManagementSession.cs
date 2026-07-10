using StrategyGame.ConsoleGame.UI.Windows;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;
using StrategyGame.ConsoleGame.Game.Buildings;
using StrategyGame.ConsoleGame.Game.Buildings.CastleBuilding;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.Resources;
using StrategyGame.ConsoleGame.UI.Panels;

namespace StrategyGame.ConsoleGame.Game;

/// <summary>
/// Interactive session which allows a player to manage a castle: view and build castle buildings,
/// manage garrison slots and the player's unit slots, and transfer units between player and castle.
/// This class implements a console-based UI loop and handles user input until the session is closed.
/// </summary>
public class CastleManagementSession
{
    private readonly Castle castle;
    private readonly Player player;

    // Panel enum for clarity
    private enum Panel { Buildings = 0, Garrison = 1, PlayerSlots = 2 }

    /// <summary>
    /// Current focused panel (which column has focus)
    /// </summary>
    private Panel focusedPanel = Panel.Buildings;

    /// <summary>
    /// Selected indexes for each panel.
    /// Use dictionary keyed by Panel for readability instead of array indexes.
    /// </summary>
    private readonly Dictionary<Panel, int> selectedIndexes = new()
    {
        [Panel.Buildings] = 0,
        [Panel.Garrison] = 0,
        [Panel.PlayerSlots] = 0
    };

    public CastleManagementSession(Castle castle, Player player)
    {
        this.castle = castle;
        this.player = player;
    }

    /// <summary>
    /// Runs the interactive management loop. The method does not return until the player
    /// exits the session (by pressing Escape). All console input and drawing for the session
    /// is performed synchronously on the calling thread.
    /// </summary>
    public void Run()
    {
        while (true)
        {
            // Prepare lists directly
            var castleBuildings = GetCastleBuildings();
            var buildingLines = BuildBuildingLines(castleBuildings);
            var garrisonLines = BuildGarrisonLines();
            var playerSlotLines = BuildPlayerSlotLines();

            // draw UI (no key reading here)
            DrawUI(buildingLines, garrisonLines, playerSlotLines);

            // read key and process
            var keyInfo = GameConsole.ReadKey(true);
            var key = keyInfo.Key;

            if (key == ConsoleKey.Escape)
                break;

            if (ProcessKey(key, castleBuildings, buildingLines, garrisonLines, playerSlotLines))
                continue;
        }
    }

    private bool ProcessKey(ConsoleKey key, List<CastleBuilding> castleBuildings, List<string> buildingLines, List<string> garrisonLines, List<string> playerSlotLines)
    {
        // Transfer between panels with Left/Right when a unit is selected
        if (key == ConsoleKey.RightArrow && focusedPanel == Panel.Garrison)
        {
            TransferBetweenSlots(selectedIndexes[Panel.Garrison], true, selectedIndexes[Panel.PlayerSlots]);
            return true;
        }

        if (key == ConsoleKey.LeftArrow && focusedPanel == Panel.PlayerSlots)
        {
            TransferBetweenSlots(selectedIndexes[Panel.PlayerSlots], false, selectedIndexes[Panel.Garrison]);
            return true;
        }

        // navigation (left/right/up/down) extracted to helper
        if (HandleNavigation(key, buildingLines, garrisonLines, playerSlotLines))
            return true;

        if (key == ConsoleKey.Enter)
        {
            HandleEnter(castleBuildings, buildingLines, garrisonLines, playerSlotLines);
            return true;
        }

        return false;
    }

    // Extracted navigation logic: handles left/right focus and up/down selection for the focused panel.
    private bool HandleNavigation(ConsoleKey key, List<string> buildingLines, List<string> garrisonLines, List<string> playerSlotLines)
    {
        // left/right: change focused panel (transfer actions handled earlier)
        if (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
        {
            int delta = key == ConsoleKey.LeftArrow ? -1 : 1;
            int min = (int)Panel.Buildings;
            int max = (int)Panel.PlayerSlots;
            int newPanel = Math.Clamp((int)focusedPanel + delta, min, max);
            focusedPanel = (Panel)newPanel;
            return true;
        }

        // up/down: move selection within currently focused panel
        if (key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow)
        {
            int delta = key == ConsoleKey.UpArrow ? -1 : 1;
            var counts = new Dictionary<Panel, int>
            {
                [Panel.Buildings] = buildingLines.Count,
                [Panel.Garrison] = garrisonLines.Count,
                [Panel.PlayerSlots] = playerSlotLines.Count
            };

            if (counts[focusedPanel] > 0)
            {
                selectedIndexes[focusedPanel] = Math.Clamp(selectedIndexes[focusedPanel] + delta, 0, counts[focusedPanel] - 1);
            }
            return true;
        }

        return false;
    }

    private List<CastleBuilding> GetCastleBuildings()
    {
        return castle.Buildings.Select(b => b).Cast<CastleBuilding>().ToList();
    }

    private List<string> BuildBuildingLines(List<CastleBuilding> castleBuildings)
    {
        var buildingLines = new List<string>();
        for (int i = 0; i < castleBuildings.Count; i++)
        {
            var building = castleBuildings[i];
            var costs = string.Join(' ', building.ResourceCosts.Select(kv => $"{kv.Value} x {kv.Key.Name}"));
            buildingLines.Add($"{building.Name} ({(building.IsBuilt ? "Построено" : "Не построено")}) {(building.IsBuilt ? "" : $"- Стоимость: {costs}")}");
        }
        return buildingLines;
    }

    private List<string> BuildGarrisonLines()
    {
        var garrisonLines = new List<string>();
        for (int i = 0; i < castle.GarrisonSlots; i++)
        {
            var slotContent = castle.GetGarrisonSlot(i);
            garrisonLines.Add(slotContent != null && slotContent.Count > 0 ? $"{i + 1}. {slotContent.TypeName} x{slotContent.Count}" : $"{i + 1}. пуст");
        }
        return garrisonLines;
    }

    private List<string> BuildPlayerSlotLines()
    {
        var playerSlotLines = new List<string>();
        for (int i = 0; i < player.UnitSlots; i++)
        {
            var slotContent = player.GetUnitSlot(i);
            if (slotContent == null)
                playerSlotLines.Add($"{i + 1}. пуст");
            else if (slotContent is IUnitStack stack)
                playerSlotLines.Add($"{i + 1}. {slotContent.TypeName} x{stack.Count}");
            else
                playerSlotLines.Add($"{i + 1}. {slotContent.TypeName}");
        }
        return playerSlotLines;
    }

    private void DrawUI(List<string> buildingLines, List<string> garrisonLines, List<string> playerSlotLines)
    {
        GameConsole.Clear();
        int w = GameConsole.WindowWidth;
        int h = GameConsole.WindowHeight;

        string title = "=== Управление замком (Esc - выход) ===";
        int titleX = Math.Max(2, (w - title.Length) / 2);
        GameConsole.SetCursorPosition(titleX, 0);
        GameConsole.ForegroundColor = ConsoleColor.White;
        GameConsole.Write(title);

        int leftX = 2;
        int leftY = 3;
        int colW = Math.Max(20, (w - 8) / 3);
        int contentH = Math.Max(0, h - leftY - 4);

        // Draw headers
        GameConsole.SetCursorPosition(leftX, leftY - 1);
        GameConsole.ForegroundColor = ConsoleColor.Cyan;
        GameConsole.Write("Здания:");

        GameConsole.SetCursorPosition(leftX + colW + 2, leftY - 1);
        GameConsole.ForegroundColor = ConsoleColor.Yellow;
        GameConsole.Write("Гарнизон:");

        GameConsole.SetCursorPosition(leftX + 2 * (colW + 2), leftY - 1);
        GameConsole.ForegroundColor = ConsoleColor.Green;
        GameConsole.Write("Слоты игрока:");

        GameConsole.ForegroundColor = ConsoleColor.Gray;

        // Use UIPanel-derived ListPanel to draw frames and content (ensures identical look)
        var l1 = new ListPanel(leftX - 1, leftY - 1, colW + 2, contentH + 1, buildingLines.ToArray(), Orientation.Vertical, true);
        var l2 = new ListPanel(leftX + colW + 1, leftY - 1, colW + 2, contentH + 1, garrisonLines.ToArray(), Orientation.Vertical, true);
        var l3 = new ListPanel(leftX + 2 * (colW + 2) - 1, leftY - 1, colW + 2, contentH + 1, playerSlotLines.ToArray(), Orientation.Vertical, true);

        l1.IsFocused = (focusedPanel == Panel.Buildings);
        l1.SelectedIndex = selectedIndexes[Panel.Buildings];
        l2.IsFocused = (focusedPanel == Panel.Garrison);
        l2.SelectedIndex = selectedIndexes[Panel.Garrison];
        l3.IsFocused = (focusedPanel == Panel.PlayerSlots);
        l3.SelectedIndex = selectedIndexes[Panel.PlayerSlots];

        l1.Draw("Здания");
        l2.Draw("Гарнизон");
        l3.Draw("Слоты");

        GameConsole.Flush();

        // sync back selections (in case UI changed them directly)
        selectedIndexes[Panel.Buildings] = l1.SelectedIndex;
        selectedIndexes[Panel.Garrison] = l2.SelectedIndex;
        selectedIndexes[Panel.PlayerSlots] = l3.SelectedIndex;

        // Draw resources above footer, using map glyphs (like map screen)
        try
        {
            bool useFallback = MapSymbols.Settings?.UseMonospace ?? false;
            string GlyphFor(Type t)
            {
                MapCell cell = t == typeof(Wood) ? MapCell.Wood : t == typeof(Stone) ? MapCell.Stone : t == typeof(Gold) ? MapCell.Gold : MapCell.Empty;
                if (MapSymbols.CellToGlyph != null && MapSymbols.CellToGlyph.TryGetValue(cell, out var arr))
                {
                    int idx = useFallback ? 1 : 0;
                    if (arr != null && arr.Length > idx && !string.IsNullOrEmpty(arr[idx])) return arr[idx];
                }
                return t.Name;
            }

            GameConsole.SetCursorPosition(2, h - 3);
            GameConsole.ForegroundColor = ConsoleColor.DarkYellow;
            var resStr = string.Join(' ', player.Resources.Select(r => $"{r.Amount}x{GlyphFor(r.GetType())}"));
            GameConsole.Write($"Ресурсы: {resStr}");
            GameConsole.ForegroundColor = ConsoleColor.Gray;
        }
        catch
        {
            // ignore any issues reading map symbols — fallback to simple text
            GameConsole.SetCursorPosition(2, h - 3);
            GameConsole.ForegroundColor = ConsoleColor.DarkYellow;
            var resStr = string.Join(' ', player.Resources.Select(r => $"{r.Amount}x{r.GetType().Name}"));
            GameConsole.Write($"Ресурсы: {resStr}");
            GameConsole.ForegroundColor = ConsoleColor.Gray;
        }

        // footer (help)
        GameConsole.SetCursorPosition(2, h - 2);
        GameConsole.ForegroundColor = ConsoleColor.Gray;
        GameConsole.Write("Стрелки: Навигация  Enter: Действие  Esc: Выйти  (?/? при выборе юнита — переместить");
        GameConsole.Flush();
    }

    /// <summary>
    /// Generalized transfer routine used by both directional transfer actions.
    /// targetIsGarrison is always the opposite of sourceIsGarrison in current UI flows,
    /// so computing it from sourceIsGarrison simplifies the signature and avoids invalid states.
    /// </summary>
    private void TransferBetweenSlots(int sourceIndex, bool sourceIsGarrison, int targetIndex)
    {
        bool targetIsGarrison = !sourceIsGarrison;

        ICombatant? GetSlot(bool garrison, int idx) => garrison ? castle.GetGarrisonSlot(idx) : player.GetUnitSlot(idx);
        bool TrySetSlot(bool garrison, int idx, ICombatant unit)
        {
            if (garrison) return castle.TrySetGarrisonSlot(idx, unit);
            return player.TrySetUnitSlot(idx, unit);
        }

        var sourceObj = GetSlot(sourceIsGarrison, sourceIndex);
        var sourceStack = sourceObj as IUnitStack;
        if (sourceStack == null || sourceStack.Count == 0) return; // nothing to move

        var targetObj = GetSlot(targetIsGarrison, targetIndex);
        var targetStack = targetObj as IUnitStack;
        int quantityToMove = sourceStack.Count;

        if (targetStack == null)
        {
            var created = UnitFactory.Create(sourceStack.TypeName, 0);
            if (TrySetSlot(targetIsGarrison, targetIndex, created))
            {
                targetObj = GetSlot(targetIsGarrison, targetIndex);
                targetStack = targetObj as IUnitStack;
            }
        }

        if (targetStack != null)
        {
            int freeSpace = 99 - targetStack.Count;
            int toAdd = Math.Min(freeSpace, quantityToMove);
            if (toAdd > 0)
            {
                sourceStack.Add(-toAdd);
                targetStack.Add(toAdd);
            }
        }
    }

    private void HandleEnter(List<CastleBuilding> castleBuildings, List<string> buildingLines, List<string> garrisonLines, List<string> playerSlotLines)
    {
        if (focusedPanel == Panel.Buildings)
        {
            HandleEnterBuilding(castleBuildings, selectedIndexes[Panel.Buildings]);
        }
        else if (focusedPanel == Panel.Garrison)
        {
            HandleEnterGarrison(selectedIndexes[Panel.Garrison]);
        }
        else if (focusedPanel == Panel.PlayerSlots)
        {
            HandleEnterPlayer(selectedIndexes[Panel.PlayerSlots]);
        }
    }

    private void HandleEnterBuilding(List<CastleBuilding> castleBuildings, int buildingIndex)
    {
        if (castleBuildings.Count == 0) return;

        var chosenBuilding = castleBuildings[buildingIndex];
        if (!chosenBuilding.IsBuilt)
        {
            TryBuildBuilding(chosenBuilding);
        }
        else
        {
            ShowBuildingInfoAndHandleActions(chosenBuilding);
        }
    }

    private void TryBuildBuilding(CastleBuilding chosenBuilding)
    {
        int conf = new MenuWindow($"Построить {chosenBuilding.Name}?", new[] { "Да", "Нет" }, "Подтверждение").Show();
        if (conf != 0) return;

        // check resources
        bool ok = true;
        foreach (var kv in chosenBuilding.ResourceCosts)
        {
            var r = player.Resources.FirstOrDefault(x => x.GetType() == kv.Key);
            if (r == null || r.Amount < kv.Value) { ok = false; break; }
        }
        if (!ok)
        {
            new ConsoleWindow<int>("Недостаточно ресурсов.", "Ошибка").Show();
        }
        else
        {
            foreach (var kv in chosenBuilding.ResourceCosts)
                player.TryConsumeResource(kv.Key, kv.Value);
            chosenBuilding.Build();
        }
    }

    private void ShowBuildingInfoAndHandleActions(CastleBuilding chosenBuilding)
    {
        var details = new List<string>();
        details.Add($"Здание: {chosenBuilding.Name}");
        details.Add(chosenBuilding.IsBuilt ? "Статус: Построено" : "Статус: Не построено");
        if (chosenBuilding.ResourceCosts != null && chosenBuilding.ResourceCosts.Count > 0)
        {
            details.Add("Стоимость:");
            foreach (var kv in chosenBuilding.ResourceCosts)
                details.Add($"  {kv.Value} x {kv.Key.Name}");
        }

        if (chosenBuilding.ProducedUnits != null && chosenBuilding.ProducedUnits.Count > 0)
        {
            details.Add("Произведённые юниты:");
            foreach (var kv in chosenBuilding.ProducedUnits)
                details.Add($"  {kv.Key.Name} x{kv.Value}");
        }

        // build options depending on availability of action
        var opts = new List<string>();
        if (chosenBuilding.HasAction) opts.Add("Использовать действие");
        opts.Add("Купить юнитов");
        opts.Add("Отмена");

        int sel = new MenuWindow(string.Join("\n", details), opts.ToArray(), "Инфо").Show();
        if (sel == 0 && chosenBuilding.HasAction)
        {
            chosenBuilding.UseAction(castle, player);
        }
        else if ((sel == 0 && !chosenBuilding.HasAction) || (sel == 1 && chosenBuilding.HasAction))
        {
            BuyUnitsFlow(chosenBuilding);
        }
    }

    private void BuyUnitsFlow(CastleBuilding chosenBuilding)
    {
        var unitTypes = chosenBuilding.ProducedUnits.Where(kv => kv.Value > 0).Select(kv => kv.Key).ToList();
        if (unitTypes.Count == 0)
        {
            new ConsoleWindow<int>("Нечего покупать.", "Покупка").Show();
            return;
        }

        var unitOptions = unitTypes.Select(t => $"{t.Name} x{chosenBuilding.GetProducedCount(t)}").Append("Отмена").ToArray();
        int uidx = new MenuWindow("Выберите юнита", unitOptions, "Покупка").Show();
        if (uidx == unitOptions.Length - 1) return;

        Type unitType = unitTypes[uidx];
        int maxAvail = chosenBuilding.GetProducedCount(unitType);

        string qtyStr = new InputWindow($"Введите количество (макс {maxAvail})").Show();
        if (!int.TryParse(qtyStr, out int quantity) || quantity <= 0) return;
        quantity = Math.Min(quantity, maxAvail);

        var unitCost = chosenBuilding.GetUnitCost(unitType);
        if (unitCost != null)
        {
            bool enough = true;
            foreach (var kv in unitCost)
            {
                var res = player.Resources.FirstOrDefault(x => x.GetType() == kv.Key);
                if (res == null || res.Amount < kv.Value * quantity) { enough = false; break; }
            }
            if (!enough) { new ConsoleWindow<int>("Недостаточно ресурсов для покупки.", "Ошибка").Show(); return; }

            foreach (var kv in unitCost)
                player.TryConsumeResource(kv.Key, kv.Value * quantity);
        }

        if (!chosenBuilding.ConsumeProduced(unitType, quantity))
        {
            new ConsoleWindow<int>("Ошибка: недостаточно юнитов в здании.", "Ошибка").Show();
            return;
        }

        AddUnitsToGarrison(unitType, quantity);
    }

    private void AddUnitsToGarrison(Type unitType, int quantity)
    {
        int remaining = quantity;
        for (int slot = 0; slot < castle.GarrisonSlots && remaining > 0; slot++)
        {
            var garrisonSlot = castle.GetGarrisonSlot(slot);
            if (garrisonSlot == null)
            {
                var created = UnitFactory.Create(unitType.Name, 0);
                castle.TrySetGarrisonSlot(slot, created);
                garrisonSlot = created;
            }

            var stack = garrisonSlot as IUnitStack;
            if (stack == null) continue;

            int freeSpace = 99 - stack.Count;
            int toAdd = Math.Min(freeSpace, remaining);
            stack.Add(toAdd);
            remaining -= toAdd;
        }
    }

    private void HandleEnterGarrison(int garrisonIndex)
    {
        var garrisonObj = castle.GetGarrisonSlot(garrisonIndex);
        var garrisonStack = garrisonObj as IUnitStack;
        if (garrisonStack == null || garrisonStack.Count == 0)
        {
            new ConsoleWindow<int>("Пустой слот гарнизона.", "Инфо").Show();
            return;
        }

        int chosenSlot = ChoosePlayerSlot();
        if (chosenSlot < 0) return;

        int quantity = PromptQuantity(garrisonStack.Count);
        if (quantity <= 0) return;

        MoveQuantityBetweenSlots(true, garrisonIndex, chosenSlot, quantity);
    }

    private void HandleEnterPlayer(int playerSlotIndex)
    {
        var playerSlotObj = player.GetUnitSlot(playerSlotIndex);
        var playerStack = playerSlotObj as IUnitStack;
        if (playerStack == null || playerStack.Count == 0)
        {
            new ConsoleWindow<int>("Пустой слот игрока.", "Инфо").Show();
            return;
        }

        int action = new MenuWindow($"Слот {playerSlotIndex + 1}: {playerSlotObj.TypeName} x{playerStack.Count}", new[] { "Отправить в гарнизон", "Отмена" }, "Действие").Show();
        if (action == 0)
        {
            int quantity = PromptQuantity(playerStack.Count);
            if (quantity <= 0) return;

            // remove from player, then add to first available garrison slots
            playerStack.Add(-quantity);
            MovePlayerStackToGarrison(playerStack, quantity);
        }
    }
    /// <summary>
    /// Helper: prompt for integer quantity up to max, returns 0 on invalid/cancel
    /// </summary>
    private int PromptQuantity(int max)
    {
        string qtyStr = new InputWindow($"Введите количество (макс {max})").Show();
        if (!int.TryParse(qtyStr, out int quantity) || quantity <= 0) return 0;
        return Math.Min(quantity, max);
    }

    /// <summary>
    /// Helper: present player slot choices, return index or -1 when cancelled
    /// </summary>
    private int ChoosePlayerSlot()
    {
        var slotOptions = new List<string>();
        for (int i = 0; i < player.UnitSlots; i++)
        {
            var ps = player.GetUnitSlot(i);
            slotOptions.Add(ps == null ? $"Слот {i + 1}: пуст" : $"Слот {i + 1}: {ps.TypeName} {(ps is IUnitStack s ? $"x{s.Count}" : "")} ");
        }
        slotOptions.Add("Отмена");
        int chosenSlot = new MenuWindow("Выберите слот игрока", slotOptions.ToArray(), "Перемещение").Show();
        if (chosenSlot == slotOptions.Count - 1) return -1;
        return chosenSlot;
    }

    /// <summary>
    /// Helper: move up to requestedQuantity from source to target, creating target if necessary
    /// </summary>
    private void MoveQuantityBetweenSlots(bool sourceIsGarrison, int sourceIndex, int targetIndex, int requestedQuantity)
    {
        bool targetIsGarrison = !sourceIsGarrison;

        ICombatant? GetSlot(bool garrison, int idx) => garrison ? castle.GetGarrisonSlot(idx) : player.GetUnitSlot(idx);
        bool TrySetSlot(bool garrison, int idx, ICombatant unit)
        {
            if (garrison) return castle.TrySetGarrisonSlot(idx, unit);
            return player.TrySetUnitSlot(idx, unit);
        }

        var sourceObj = GetSlot(sourceIsGarrison, sourceIndex);
        var sourceStack = sourceObj as IUnitStack;
        if (sourceStack == null || sourceStack.Count == 0) return;

        var targetObj = GetSlot(targetIsGarrison, targetIndex);
        var targetStack = targetObj as IUnitStack;

        int quantityToMove = Math.Min(requestedQuantity, sourceStack.Count);

        if (targetStack == null)
        {
            var created = UnitFactory.Create(sourceStack.TypeName, 0);
            if (TrySetSlot(targetIsGarrison, targetIndex, created))
            {
                targetObj = GetSlot(targetIsGarrison, targetIndex);
                targetStack = targetObj as IUnitStack;
            }
        }

        if (targetStack == null) return;

        int freeSpace = 99 - targetStack.Count;
        int toAdd = Math.Min(freeSpace, quantityToMove);
        if (toAdd <= 0) return;

        sourceStack.Add(-toAdd);
        targetStack.Add(toAdd);
    }

    /// <summary>
    /// Helper: move from player stack to first available garrison slots
    /// </summary>
    private void MovePlayerStackToGarrison(IUnitStack playerStack, int quantity)
    {
        int remaining = quantity;
        for (int slot = 0; slot < castle.GarrisonSlots && remaining > 0; slot++)
        {
            var garrisonSlot = castle.GetGarrisonSlot(slot);
            if (garrisonSlot == null)
            {
                var created = UnitFactory.Create(playerStack.TypeName, 0);
                castle.TrySetGarrisonSlot(slot, created);
                garrisonSlot = created;
            }

            var stack = garrisonSlot as IUnitStack;
            if (stack == null) continue;
            int freeSpace = 99 - stack.Count;
            int toAdd = Math.Min(freeSpace, remaining);
            if (toAdd <= 0) continue;
            stack.Add(toAdd);
            remaining -= toAdd;
        }
    }
}
