using StrategyGame.ConsoleGame.UI.CustomConsole;
using StrategyGame.ConsoleGame.UI.Windows;
using StrategyGame.ConsoleGame.UI;
using StrategyGame.ConsoleGame.Game.Units;
using StrategyGame.ConsoleGame.Game.PlayerTypes;
using StrategyGame.ConsoleGame.Game.MapTypes;

namespace StrategyGame.ConsoleGame.Game;

/// <summary>
/// Battle session between player units and monsters or defenders at a given map position.
/// Handles simple UI for selecting attacks, auto-battle and turn order.
/// </summary>
public class BattleSession
{
    private readonly Map gameMap;
    private readonly Player currentPlayer;
    private readonly List<UnitBase> enemies;
    private readonly Coordinate position;
    private readonly Random random = new();

    private bool autoBattle = false;

    /// <summary>
    /// Construct a new battle session.
    /// </summary>
    /// <param name="map">Game map where the battle occurs.</param>
    /// <param name="player">Player who initiates the battle.</param>
    /// <param name="enemies">Enemy units (monsters or defenders).</param>
    /// <param name="position">Map coordinate where the battle happens.</param>
    public BattleSession(Map map, Player player, List<UnitBase> enemies, Coordinate position)
    {
        this.gameMap = map;
        this.currentPlayer = player;
        this.enemies = enemies;
        this.position = position;

        // Ensure owner is set for player's units
        for (int slotIndex = 0; slotIndex < currentPlayer.UnitSlots; slotIndex++)
        {
            var unit = currentPlayer.GetUnitSlot(slotIndex);
            if (unit != null)
                unit.Owner = currentPlayer;
        }
    }

    /// <summary>
    /// Run the battle until one side is defeated.
    /// </summary>
    /// <returns>True if player wins, false otherwise.</returns>
    public bool Start()
    {
        // battle loop
        while (currentPlayer.HasAliveUnits() && enemies.Any(enemy => enemy.IsAlive))
        {
            Draw("Ход игрока");

            // player turn
            PlayerTurn();

            if (!enemies.Any(enemy => enemy.IsAlive))
                break;

            Draw("Ход противника");

            // enemy turn
            EnemyTurn();
        }

        bool playerWon = currentPlayer.HasAliveUnits();
        Draw(playerWon ? "Победа" : "Поражение");

        if (playerWon)
        {
            new ConsoleWindow<int>("Вы победили в бою!", "Успех").Show();
            gameMap.RemoveMonstersAt(position);
        }
        else
        {
            new ConsoleWindow<int>("Ваши войска были уничтожены.", "Поражение").Show();
        }

        return playerWon;
    }

    /// <summary>
    /// Render battle UI: title, player's units and enemy units, footer controls.
    /// </summary>
    /// <param name="subtitle">Subtitle to show (phase of battle).</param>
    private void Draw(string subtitle)
    {
        GameConsole.Clear();

        int w = GameConsole.WindowWidth;
        int h = GameConsole.WindowHeight;

        // title
        GameConsole.ForegroundColor = ConsoleColor.White;
        string title = $"=== Бой === {subtitle} ";
        int titleX = Math.Max(2, (w - title.Length) / 2);
        GameConsole.SetCursorPosition(titleX, 0);
        GameConsole.Write(title);

        // player units
        int leftX = 2;
        int leftY = 2;
        GameConsole.ForegroundColor = ConsoleColor.Green;
        GameConsole.SetCursorPosition(leftX, leftY - 1);
        GameConsole.Write("Союзные войска:");

        DrawPlayerUnits(leftX, leftY);

        // enemy units
        int rightX = Math.Max(w / 2, 40);
        int rightY = 2;
        GameConsole.ForegroundColor = ConsoleColor.Red;
        GameConsole.SetCursorPosition(rightX, rightY - 1);
        GameConsole.Write("Противник:");

        DrawEnemyUnits(rightX, rightY);

        // footer
        GameConsole.ForegroundColor = ConsoleColor.Yellow;
        string footer = $"[A] Авто: {(autoBattle ? "Вкл" : "Выкл")}   [Esc] Сдаться";
        int footerX = Math.Max(2, (w - footer.Length) / 2);
        GameConsole.SetCursorPosition(footerX, h - 3);
        GameConsole.Write(footer);

        GameConsole.ForegroundColor = ConsoleColor.Gray;
        GameConsole.Flush();
    }

    private void DrawPlayerUnits(int x, int y)
    {
        int w = GameConsole.WindowWidth;
        int maxLen = Math.Max(10, Math.Min((w / 2) - 4, 60));

        for (int slotIndex = 0; slotIndex < currentPlayer.UnitSlots; slotIndex++)
        {
            var unit = currentPlayer.GetUnitSlot(slotIndex);
            int lineY = y + slotIndex;
            ClearLineFrom(x, lineY);
            GameConsole.SetCursorPosition(x, lineY);
            if (unit == null || (unit is IUnitStack stackUnit && stackUnit.Count == 0))
            {
                GameConsole.Write(Truncate($"[{slotIndex + 1}] Пусто", maxLen));
            }
            else
            {
                string alive = unit.IsAlive ? string.Empty : " (мёртв)";
                string line;
                if (unit is IUnitStack unitStack)
                {
                    line = $"[{slotIndex + 1}] {unit.TypeName} x{unitStack.Count} HP:{unit.CurrentHp}/{unit.MaxHp} ATK:{unit.Attack}{alive}";
                }
                else
                {
                    line = $"[{slotIndex + 1}] {unit.TypeName} HP:{unit.CurrentHp}/{unit.MaxHp} ATK:{unit.Attack}{alive}";
                }

                GameConsole.Write(Truncate(line, maxLen));
            }
        }
    }

    private void DrawEnemyUnits(int x, int y)
    {
        int w = GameConsole.WindowWidth;
        int maxLen = Math.Max(10, Math.Min((w / 2) - 4, 60));

        for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
        {
            var enemy = enemies[enemyIndex];
            int lineY = y + enemyIndex;
            ClearLineFrom(x, lineY);
            GameConsole.SetCursorPosition(x, lineY);
            if (enemy == null)
                GameConsole.Write(Truncate($"[{enemyIndex + 1}] Пусто", maxLen));
            else
            {
                string alive = enemy.IsAlive ? string.Empty : " (мёртв)";
                string line = $"[{enemyIndex + 1}] {enemy.TypeName} HP:{enemy.CurrentHp}/{enemy.MaxHp} ATK:{enemy.Attack}{alive}";
                GameConsole.Write(Truncate(line, maxLen));
            }
        }
    }

    private static string Truncate(string s, int max)
    {
        if (s == null) return string.Empty;
        if (s.Length <= max) return s;
        if (max <= 3) return s.Substring(0, max);
        return s.Substring(0, max - 3) + "...";
    }

    private void ClearLineFrom(int x, int y)
    {
        GameConsole.SetCursorPosition(x, y);
        int w = GameConsole.WindowWidth;
        int count = Math.Max(0, w - x - 1);
        GameConsole.Write(new string(' ', count));
    }

    private void PlayerTurn()
    {
        for (int slotIndex = 0; slotIndex < currentPlayer.UnitSlots; slotIndex++)
        {
            var unit = currentPlayer.GetUnitSlot(slotIndex);
            var unitStack = unit as IUnitStack;
            if (unitStack == null || unitStack.Count == 0 || !unit.IsAlive)
                continue;

            if (!enemies.Any(enemy => enemy.IsAlive))
                break;

            if (autoBattle)
            {
                var aliveEnemies = enemies.Where(enemy => enemy.IsAlive).ToList();
                if (aliveEnemies.Count == 0) break;
                var target = aliveEnemies[random.Next(aliveEnemies.Count)];
                Attack(unit, target);
                Draw("Ход игрока");
                continue;
            }

            var promptResult = HandlePreAttackPrompt(unit, slotIndex);
            if (promptResult == PreAttackResult.Surrendered)
                return;

            if (promptResult == PreAttackResult.Attacked)
                continue;

            if (autoBattle)
            {
                var aliveEnemies = enemies.Where(enemy => enemy.IsAlive).ToList();
                if (aliveEnemies.Count == 0) break;
                var target = aliveEnemies[random.Next(aliveEnemies.Count)];
                Attack(unit, target);
                Draw("Ход игрока");
            }
        }
    }

    private void EnemyTurn()
    {
        var alivePlayerUnits = currentPlayer.Units.Where(u => u != null && u is IUnitStack s && s.Count > 0 && u.IsAlive).ToList();
        var aliveEnemies = enemies.Where(enemy => enemy != null && enemy.IsAlive).ToList();

        foreach (var enemy in aliveEnemies)
        {
            if (!currentPlayer.HasAliveUnits()) break;

            alivePlayerUnits = currentPlayer.Units.Where(u => u != null && u is IUnitStack st && st.Count > 0 && u.IsAlive).ToList();
            if (alivePlayerUnits.Count == 0) break;

            var target = alivePlayerUnits[random.Next(alivePlayerUnits.Count)];
            target.TakeDamage(enemy.Attack, currentPlayer.GetDefenseForUnit(target));
            Draw("Ход противника");
        }
    }

    private void Attack(ICombatant attacker, ICombatant defender)
    {
        if (attacker.Owner == null && currentPlayer.ContainsUnit(attacker))
            attacker.Owner = currentPlayer;

        attacker.AttackTarget(defender);
    }

    private enum PreAttackResult { None, Attacked, Surrendered, ToggledAuto }

    private PreAttackResult HandlePreAttackPrompt(ICombatant unit, int unitIndex)
    {
        while (true)
        {
            Draw("Ход игрока");
            int promptY = GameConsole.WindowHeight - 5;
            ClearLineFrom(2, promptY);
            GameConsole.SetCursorPosition(2, promptY);
            GameConsole.ForegroundColor = ConsoleColor.Cyan;
            GameConsole.Write("Подсказки: [Enter] выбрать цель, [A] авто, [Esc] сдаться");
            GameConsole.ForegroundColor = ConsoleColor.Gray;
            GameConsole.Flush();

            var key = GameConsole.ReadKey(true).Key;
            if (key == ConsoleKey.A)
            {
                autoBattle = !autoBattle;
                return PreAttackResult.ToggledAuto;
            }
            else if (key == ConsoleKey.Escape)
            {
                // surrender: kill all player's units
                for (int i = 0; i < currentPlayer.UnitSlots; i++)
                {
                    var u = currentPlayer.GetUnitSlot(i);
                    if (u != null)
                        u.Kill();
                }

                return PreAttackResult.Surrendered;
            }
            else if (key == ConsoleKey.Enter)
            {
                var choices = enemies.Select((enemy, enemyIndex) => enemy.IsAlive
                    ? $"{enemyIndex + 1}. {enemy.TypeName} HP:{enemy.CurrentHp}/{enemy.MaxHp} ATK:{enemy.Attack}"
                    : $"{enemyIndex + 1}. (мертв)").ToArray();

                int selection = new MenuWindow($"Выберите цель для [{unitIndex + 1}] {unit.TypeName}", choices,
                    "Цель", buttonPosition: ButtonPosition.CenterVertically).Show();

                var chosenEnemy = enemies[selection];
                if (chosenEnemy.IsAlive)
                {
                    Attack(unit, chosenEnemy);
                    return PreAttackResult.Attacked;
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
