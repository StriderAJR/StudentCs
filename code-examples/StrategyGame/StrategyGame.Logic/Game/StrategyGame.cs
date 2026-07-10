using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.Units;
using StrategyGame.ConsoleGame.Game.Units.Playable;
using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.ConsoleGame.UI.Panels;
using StrategyGame.ConsoleGame.UI.Windows;
using StrategyGame.ConsoleGame.Game.Buildings;
using StrategyGame.ConsoleGame.Game.Resources;
using System.Collections.ObjectModel;
using StrategyGame.ConsoleGame.Game.Save;

namespace StrategyGame.ConsoleGame.Game;

public class StrategyGame
{
    private Map gameMap;
    private Player currentPlayer; // current active player

    private List<Player> playerList = new List<Player>();
    private int currentPlayerIndex = 0;

    // UI panels
    private SidePanel sidePanel;
    private BottomPanel bottomPanel;

    // Interface settings
    private readonly int sidePanelWidth = 30;
    private readonly int bottomPanelHeight = 5;

    private readonly Random random = new();

    // Auto-end day when moves are exhausted (off by default)
    private bool autoEndDay = false;

    private enum FocusedPanel { Map, Side }
    private FocusedPanel focused = FocusedPanel.Map;
    // persist selected index across panel recreations
    private int sidePanelSelectedIndex = 0;

    /// <summary>
    /// Start the game: choose a map, create players and enter the main game loop.
    /// </summary>
    public void Start()
    {
        // At start offer New or Load if saves exist
        string savesDir = Path.Combine(AppContext.BaseDirectory, "saves");
        bool hasSaves = Directory.Exists(savesDir) && Directory.GetFiles(savesDir).Length > 0;

        if (hasSaves)
        {
            int choice = new MenuWindow("Start", new[] { "Новая игра", "Загрузить игру" }, title: "Старт", buttonPosition: ButtonPosition.Horizontal).Show();
            if (choice == 1)
            {
                var outcome = SaveGameManager.InteractiveLoad(savesDir, Path.Combine(AppContext.BaseDirectory, "maps"));
                if (outcome != null)
                {
                    // apply loaded outcome
                    gameMap = outcome.Map;
                    playerList = outcome.Players;
                    currentPlayerIndex = outcome.CurrentPlayerIndex;
                    day = outcome.Day;
                    week = outcome.Week;

                    currentPlayer = playerList.Count > 0 ? playerList[currentPlayerIndex] : null!;
                    UITheme.CurrentBorderColor = currentPlayer != null ? UITheme.FromPlayerColor(currentPlayer.Color) : UITheme.CurrentBorderColor;

                    ClearScreen();
                    RunGameLoop();
                    return;
                }
            }
        }

        if (!ChooseAndLoadMap(out Coordinate? initialPlayerPosition))
            return;

        AskPlayersAndCreate(initialPlayerPosition);

        ClearScreen();
        RunGameLoop();
    }

    private void SaveGame()
    {
        try
        {
            var model = SaveGameManager.BuildSaveModel(gameMap, playerList, currentPlayerIndex, day, week);
            string savesDir = Path.Combine(AppContext.BaseDirectory, "saves");
            string fullPath = SaveGameManager.SaveToFile(model, savesDir);
            new ConsoleWindow<int>($"Game saved to:\n{fullPath}", "Сохранение").Show();
        }
        catch (Exception ex)
        {
            new ConsoleWindow<int>($"Failed to save game:\n{ex.Message}", "Ошибка").Show();
        }
    }

    private bool ChooseAndLoadMap(out Coordinate? playerPosition)
    {
        playerPosition = null;

        string mapsDir = Path.Combine(AppContext.BaseDirectory, "maps");
        string[] files = Array.Empty<string>();
        if (Directory.Exists(mapsDir))
        {
            files = Directory.GetFiles(mapsDir)
                .Where(f => !string.IsNullOrEmpty(f))
                .ToArray();
        }

        if (files.Length == 0)
        {
            string msg = "No maps found in the 'maps' folder.\n" +
                         "Place map files in the folder and restart the program.\n" +
                         "Press any key to exit.";
            new ConsoleWindow<int>(msg, "No Maps").Show();
            return false;
        }

        var menuItems = files.Select(x => Path.GetFileName(x)).ToArray();

        var selected = new MenuWindow("Select a map:", menuItems,
            buttonPosition: ButtonPosition.CenterVertically).Show();

        var chosenPath = files[selected];
        gameMap = new Map();
        playerPosition = gameMap.LoadFromFile(chosenPath);

        return true;
    }

    /// <summary>
    /// Configure players: ask for count, colors/types or place AI.
    /// </summary>
    private void AskPlayersAndCreate(Coordinate? initialPosition)
    {
        var playersCountChoices = new[] { "1", "2", "3", "4" };
        int playersCount = int.Parse(playersCountChoices[new MenuWindow("How many players will play?", playersCountChoices, buttonPosition: ButtonPosition.CenterVertically).Show()]);

        playerList.Clear();

        var availableColors = Enum.GetValues(typeof(PlayerColor)).Cast<PlayerColor>().ToList();

        // For any number 1..4, ask each player's color and type (all human local players)
        for (int playerIndex = 0; playerIndex < playersCount; playerIndex++)
        {
            int colorSelection = new MenuWindow($"Player {playerIndex + 1}: choose a color", availableColors.Select(c => c.ToString()).ToArray(), buttonPosition: ButtonPosition.CenterVertically).Show();
            var chosenColor = availableColors[colorSelection];
            availableColors.Remove(chosenColor);

            PlayerType playerType = (PlayerType)new MenuWindow($"Player {playerIndex + 1}: choose a type", Enum.GetNames(typeof(PlayerType))).Show();

            Coordinate playerPosition;
            if (playerIndex == 0 && initialPosition != null)
                playerPosition = initialPosition.Value;
            else
            {
                var foundPosition = gameMap.FindRandomEmptyCell(random, playerList.Select(existingPlayer => existingPlayer.position));
                playerPosition = foundPosition ?? gameMap.FindFirstEmptyCell() ?? new Coordinate(1, 1);
            }

            var newPlayer = new Player(playerType, playerPosition, chosenColor);

            playerList.Add(newPlayer);
        }

        // initialize per-player fog and give each player starter units
        gameMap.InitializePlayerFog(playerList);

        foreach (var playerItem in playerList)
        {
            playerItem.AddUnit(new InfantryUnit());
            playerItem.AddUnit(new ArcherUnit());
            playerItem.AddUnit(new BeastUnit());
            gameMap.RevealAround(playerItem, playerItem.position, 3);
        }

        currentPlayerIndex = 0;
        currentPlayer = playerList[currentPlayerIndex];
        UITheme.CurrentBorderColor = UITheme.FromPlayerColor(currentPlayer.Color);
    }

    private void RunGameLoop()
    {
        while (true)
        {
            Draw();

            ConsoleKey key = GameConsole.ReadKey().Key;

            if (key == ConsoleKey.Tab)
            {
                focused = (focused == FocusedPanel.Map) ? FocusedPanel.Side : FocusedPanel.Map;
                if (sidePanel != null) sidePanel.IsFocused = (focused == FocusedPanel.Side);
                continue;
            }

            if (focused == FocusedPanel.Side && sidePanel != null)
            {
                if (sidePanel.HandleKey(key))
                {
                    sidePanelSelectedIndex = sidePanel.SelectedIndex;
                    continue;
                }
            }

            var keyResult = HandleNonMovementKey(key);
            if (keyResult == KeyResult.Exit)
                return;
            if (keyResult == KeyResult.Handled)
                continue;

            var shift = GetShiftFromKey(key);
            if (shift == null)
                continue;

            if (!gameMap.CanMove(currentPlayer.position, shift.Value))
                continue;

            if (currentPlayer.MovesRemaining <= 0)
            {
                if (autoEndDay)
                {
                    EndDay();
                }
                else
                {
                    ShowNoMovesMessage();
                }

                continue;
            }

            bool stillAlive = MoveAndProcess(shift.Value);
            if (!stillAlive)
                return; // only happens if single-player main hero died

            if (currentPlayer.MovesRemaining <= 0 && autoEndDay)
            {
                EndDay();
            }
        }
    }

    private enum KeyResult { NotHandled, Handled, Exit }

    private KeyResult HandleNonMovementKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.I:
                ShowPlayerInfo();
                return KeyResult.Handled;
            case ConsoleKey.E:
                EndDay();
                return KeyResult.Handled;
            case ConsoleKey.T:
                autoEndDay = !autoEndDay;
                return KeyResult.Handled;
            case ConsoleKey.M:
            {
                // Add Save option to the popup menu: Продолжить, Сохранить, Выйти
                int menuSelection = new MenuWindow("Меню", new[] { "Продолжить", "Сохранить игру", "Выйти" }, "Меню", buttonPosition: ButtonPosition.Horizontal).Show();
                if (menuSelection == 1)
                {
                    SaveGame();
                    return KeyResult.Handled;
                }
                if (menuSelection == 2)
                    return KeyResult.Exit;
                return KeyResult.Handled;
            }
            case ConsoleKey.Tab:
            {
                focused = (focused == FocusedPanel.Map) ? FocusedPanel.Side : FocusedPanel.Map;
                return KeyResult.Handled;
            }
            default:
                return KeyResult.NotHandled;
        }
    }

    private Coordinate? GetShiftFromKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                return new Coordinate(-1, 0);
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                return new Coordinate(1, 0);
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                return new Coordinate(0, 1);
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                return new Coordinate(0, -1);
            default:
                return null;
        }
    }

    private void ShowNoMovesMessage()
    {
        new ConsoleWindow<int>("No moves left. Press E to end the day or T to toggle auto-end.", "No Moves").Show();
    }

    private bool MoveAndProcess(Coordinate shift)
    {
        currentPlayer.Move(shift);
        currentPlayer.MovesRemaining--;

        // FIRST: if there is another player at the new position — PvP battle
        var otherPlayer = playerList.FirstOrDefault(candidatePlayer => !ReferenceEquals(candidatePlayer, currentPlayer) && candidatePlayer.position.Equals(currentPlayer.position) && candidatePlayer.HasAliveUnits());
        if (otherPlayer != null)
        {
            // gather defenders from the other player's units
            var defenders = otherPlayer.Units.OfType<UnitBase>().ToList();
            var battle = new BattleSession(gameMap, currentPlayer, defenders, currentPlayer.position);
            bool currentPlayerWon = battle.Start();

            if (currentPlayerWon)
            {
                // if attacker won, simply continue
                return true;
            }
            else
            {
                // attacker lost
                if (playerList.Count == 1)
                {
                    new ConsoleWindow<int>("You have died. Game over.", "Game Over").Show();
                    return false; // end game
                }

                // in multiplayer — do not end the whole process, move to next player
                currentPlayer.MovesRemaining = 0; // finish turn
                EndDay();
                return true;
            }
        }

        // then normal building handling
        var building = gameMap.GetBuildingAt(currentPlayer.position);
        if (building is Castle castle)
        {
            if (castle.Owner != null && ReferenceEquals(castle.Owner, currentPlayer))
            {
                ClearScreen();
                var session = new CastleManagementSession(castle, currentPlayer);
                session.Run();
                return true;
            }
            else
            {
                bool hasDefenders = castle.Garrison.Any(garrisonSlot => garrisonSlot != null && garrisonSlot.Count > 0);
                if (hasDefenders)
                {
                    castle.ApplyDefenderBonus(true);

                    var defenders = castle.Garrison.OfType<UnitBase>().ToList();
                    var battle = new BattleSession(gameMap, currentPlayer, defenders, currentPlayer.position);
                    bool currentPlayerWon = battle.Start();

                    castle.ApplyDefenderBonus(false);

                    if (currentPlayerWon)
                    {
                        castle.Capture(currentPlayer);
                        return true;
                    }
                    else
                    {
                        if (playerList.Count == 1)
                        {
                            new ConsoleWindow<int>("You have died. Game over.", "Game Over").Show();
                            return false;
                        }
                        currentPlayer.MovesRemaining = 0;
                        EndDay();
                        return true;
                    }
                }
                else
                {
                    castle.Capture(currentPlayer);
                    return true;
                }
            }
        }
        else
        {
            var monsters = gameMap.GetMonstersAt(currentPlayer.position);
            if (monsters != null && monsters.Any(monster => monster.IsAlive))
            {
                var battle = new BattleSession(gameMap, currentPlayer, monsters, currentPlayer.position);
                bool currentPlayerWon = battle.Start();
                if (!currentPlayerWon)
                {
                    if (playerList.Count == 1)
                    {
                        new ConsoleWindow<int>("You have died. Game over.", "Game Over").Show();
                        return false;
                    }
                    currentPlayer.MovesRemaining = 0;
                    EndDay();
                    return true;
                }
            }

            gameMap.RevealAround(currentPlayer, currentPlayer.position, 3);
            gameMap.TryCaptureBuilding(currentPlayer.position, currentPlayer);

            return true;
        }
    }

    /// <summary>
    /// Ends the current player's turn and passes control to the next player. If the round of players is completed — increments day/week.
    /// </summary>
    private void EndDay()
    {
        // advance to next player
        int previousIndex = currentPlayerIndex;
        currentPlayerIndex = (currentPlayerIndex + 1) % playerList.Count;
        bool roundCompleted = currentPlayerIndex == 0; // wrapped

        currentPlayer = playerList[currentPlayerIndex];
        UITheme.CurrentBorderColor = UITheme.FromPlayerColor(currentPlayer.Color);

        if (roundCompleted)
        {
            // increment day/week and collect weekly income
            day++;
            bool weekChanged = false;
            if (day > 7)
            {
                day = 1;
                week++;
                weekChanged = true;
                CollectWeeklyIncome();
            }

            // restore moves for all players for new day
            foreach (var playerItem in playerList)
            {
                if (weekChanged)
                {
                    int bonusMoves = (int)Math.Ceiling(playerItem.MaxMoves * (playerItem.TempMoveBonusPercent / 100.0));
                    playerItem.MovesRemaining = playerItem.MaxMoves + bonusMoves;
                    playerItem.TempMoveBonusPercent = 0;
                }
                else
                {
                    playerItem.MovesRemaining = playerItem.MaxMoves;
                }
            }
        }
        else
        {
            // simply restore moves for the next player
            currentPlayer.MovesRemaining = currentPlayer.MaxMoves;
        }
    }

    private void CollectWeeklyIncome()
    {
        foreach (var building in gameMap.Buildings)
        {
            if (building.Owner != null && playerList.Contains(building.Owner))
            {
                if (building is Castle castle)
                {
                    castle.WeeklyTick(building.Owner);
                }
                else
                {
                    // add income directly to the owning player
                    switch (building.Type)
                    {
                        case MapCell.Wood: building.Owner.AddResource<Wood>(building.IncomePerWeek); break;
                        case MapCell.Stone: building.Owner.AddResource<Stone>(building.IncomePerWeek); break;
                        case MapCell.Gold: building.Owner.AddResource<Gold>(building.IncomePerWeek); break;
                    }
                }
            }
        }
    }

    private List<Resource> CalculateWeeklyIncomeForPlayer(Player player)
    {
        int woodIncome = 0, stoneIncome = 0, goldIncome = 0;

        foreach (var building in gameMap.Buildings)
        {
            if (building.Owner != null && ReferenceEquals(building.Owner, player))
            {
                switch (building.Type)
                {
                    case MapCell.Wood:
                        woodIncome += building.IncomePerWeek; break;
                    case MapCell.Stone:
                        stoneIncome += building.IncomePerWeek; break;
                    case MapCell.Gold:
                        goldIncome += building.IncomePerWeek; break;
                }
            }
        }

        var list = new List<Resource>
        {
            new Wood(woodIncome),
            new Stone(stoneIncome),
            new Gold(goldIncome)
        };

        return list;
    }

    private void Draw()
    {
        int totalWidth = GameConsole.WindowWidth;
        int totalHeight = GameConsole.WindowHeight;

        int sideWidth = Math.Min(sidePanelWidth, Math.Max(0, totalWidth - 10));
        int bottomHeight = Math.Min(bottomPanelHeight, Math.Max(0, totalHeight - 4));

        int mapWidth = Math.Max(3, totalWidth - sideWidth);
        int mapHeight = Math.Max(3, totalHeight - bottomHeight);

        gameMap.DrawFrame(0, 0, mapWidth, mapHeight, "Карта");
        gameMap.DrawVisible(0, 0, mapWidth, mapHeight, playerList, currentPlayer);

        if (sidePanel != null)
            sidePanelSelectedIndex = sidePanel.SelectedIndex;

        // Prepare concrete data for panels (avoid passing delegates)
        var sideButtons = GetSideButtons();
        sidePanel = new SidePanel(mapWidth, 0, sideWidth, totalHeight - bottomHeight, sideButtons, currentPlayer, OnSideSelect);
        sidePanel.SelectedIndex = sidePanelSelectedIndex;

        // build data for bottom panel using current player's resources and income
        var incomeList = CalculateWeeklyIncomeForPlayer(currentPlayer);

        var panelData = GetPanelData(incomeList);
        bottomPanel = new BottomPanel(0, mapHeight, totalWidth, bottomHeight, panelData, currentPlayer);

        sidePanel.IsFocused = (focused == FocusedPanel.Side);

        bottomPanel.Draw("Инфо (Tab - переключение панели)");
        sidePanel.Draw("Панель");

        GameConsole.Flush();
    }

    private void OnSideSelect(int index)
    {
        switch (index)
        {
            case 0:
                ShowPlayerInfo();
                break;
            case 1:
                EndDay();
                break;
            case 2:
                autoEndDay = !autoEndDay;
                break;
            case 3:
                // when opened from side panel, show full menu: Continue, Save, Exit
                int menuSelection = new MenuWindow("Меню", new[] { "Продолжить", "Сохранить игру", "Выйти" }, "Меню", buttonPosition: ButtonPosition.Horizontal).Show();
                if (menuSelection == 1)
                {
                    SaveGame();
                }
                else if (menuSelection == 2) Environment.Exit(0);
                break;
        }
    }

    private PanelData GetPanelData(List<Resource> incomeForPlayer)
    {
        // current overall resource amounts come from current player's Resources
        var currentResources = currentPlayer.Resources as IReadOnlyList<Resource>;

        // build income dictionary from incomeForPlayer list
        var incomeDict = new Dictionary<Type, int>();
        foreach (var resource in incomeForPlayer)
        {
            incomeDict[resource.GetType()] = resource.Amount;
        }

        return new PanelData(day, week, currentResources, new ReadOnlyDictionary<Type, int>(incomeDict), currentPlayer.MovesRemaining, currentPlayer.MaxMoves);
    }

    private string[] GetSideButtons()
    {
        string autoText = autoEndDay ? "[T] Авто: Вкл" : "[T] Авто: Выкл";
        return new[] { "[I] Информация об игроке", "[E] Завершить день", autoText, "[M] Меню" };
    }

    private void ShowPlayerInfo()
    {
        var unitsInfo = new System.Text.StringBuilder();
        for (int slotIndex = 0; slotIndex < currentPlayer.UnitSlots; slotIndex++)
        {
            var unit = currentPlayer.GetUnitSlot(slotIndex);
            if (unit == null)
            {
                unitsInfo.AppendLine($"Слот {slotIndex + 1}: пуст");
            }
            else
            {
                // If unit is a stack, show the count
                if (unit is UnitStack unitStack)
                {
                    unitsInfo.AppendLine($"Слот {slotIndex + 1}: {unit.TypeName} x{unitStack.Count} HP:{unit.CurrentHp}/{unit.MaxHp} ATK:{unit.Attack}");
                }
                else
                {
                    unitsInfo.AppendLine($"Слот {slotIndex + 1}: {unit.TypeName} HP:{unit.CurrentHp}/{unit.MaxHp} ATK:{unit.Attack}");
                }
            }
        }

        string head = currentPlayer.HeadArmor != null ? currentPlayer.HeadArmor.Name : "Пусто";
        string body = currentPlayer.BodyArmor != null ? currentPlayer.BodyArmor.Name : "Пусто";
        string artifact = currentPlayer.EquippedArtifact != null ? currentPlayer.EquippedArtifact.Name : "Пусто";
        string weapon = currentPlayer.EquippedWeapon != null ? currentPlayer.EquippedWeapon.Name : "Пусто";

        // show the player's color instead of name
        string resourcesDesc = string.Join(", ", currentPlayer.Resources.Select(r => r.ToString()));

        string msg = $"Цвет: {currentPlayer.Color}\nТип: {currentPlayer.Type}\nМагия: {currentPlayer.MagicRemaining}/{currentPlayer.MaxMagic}\n" +
                     $"Позиция: ({currentPlayer.X},{currentPlayer.Y})\n\n" +
                     $"Оборудование:\n  Голова: {head}\n  Тело: {body}\n  Артефакт: {artifact}\n  Оружие: {weapon}\n\n" +
                     "Юниты:\n" + unitsInfo.ToString() +
                     $"\nРесурсы - {resourcesDesc}\n\nНажмите любую клавиши...";

        new ConsoleWindow<int>(msg, "Информация об игроке").Show();
    }

    private static void ClearScreen()
    {
        GameConsole.Clear();
        GameConsole.Flush();
    }

    // --- Saving logic ---

    private int day = 1;
    private int week = 1;
}
