using StrategyGame.ConsoleGame.Game.Units;
using StrategyGame.ConsoleGame.Game.Resources;
using StrategyGame.ConsoleGame.UI.Windows;
using StrategyGame.ConsoleGame.Game.Items;

namespace StrategyGame.ConsoleGame.Game.PlayerTypes;

/// <summary>
/// Представляет игрока в игре: позиция, оборудование, ресурсы и отряд юнитов.
/// </summary>
public class Player
{
    public Coordinate position { get; set; }
    public int X { get => position.X; }
    public int Y { get => position.Y; }
    public PlayerColor Color { get; set; }

    public readonly PlayerType Type;

    /// <summary>
    /// Максимальное количество ходов в день.
    /// </summary>
    public int MaxMoves { get; }
    /// <summary>
    /// Оставшиеся ходы в текущем дне.
    /// </summary>
    public int MovesRemaining { get; set; }

    /// <summary>
    /// Временный бонус к передвижению в процентах, применяемый к следующей неделе (например от строения).
    /// </summary>
    public int TempMoveBonusPercent { get; set; }

    /// <summary>
    /// Максимальное количество магии.
    /// </summary>
    public int MaxMagic { get; }
    /// <summary>
    /// Текущее количество магии.
    /// </summary>
    public int MagicRemaining { get; set; }

    /// <summary>
    /// Список ресурсов игрока (дерево, камень, золото и т.д.).
    /// </summary>
    public List<Resource> Resources { get; } = new List<Resource>();

    // internal mutable storage for unit slots
    private readonly ICombatant[] units = new ICombatant[3];

    /// <summary>
    /// Читаемый список слотов юнитов.
    /// </summary>
    public IReadOnlyList<ICombatant> Units => Array.AsReadOnly(units);

    public int UnitSlots => units.Length;

    public Armor HeadArmor { get; set; }
    public Armor BodyArmor { get; set; }
    public Artifact EquippedArtifact { get; set; }
    public Weapon EquippedWeapon { get; set; }

    public Player(PlayerType type, Coordinate position, PlayerColor color)
    {
        Type = type;
        this.position = position;
        Color = color;
        MaxMoves = GetMaxMoves(type);
        MovesRemaining = MaxMoves;

        MaxMagic = GetMaxMagic(type);
        MagicRemaining = MaxMagic;

        // инициализация типов ресурсов по умолчанию (количество 0)
        Resources.Add(new Wood(0));
        Resources.Add(new Stone(0));
        Resources.Add(new Gold(0));
    }

    /// <summary>
    /// Добавить один юнит заданного типа. Если где-то уже есть стек того же типа — увеличиваем его.
    /// Иначе помещаем в первый свободный слот. В случае отсутствия свободных слотов и отсутствия такого типа — выводим сообщение и возвращаем false.
    /// </summary>
    public bool AddUnit(UnitBase unitPrototype)
    {
        if (unitPrototype == null)
        {
            new ConsoleWindow<int>("Невозможно добавить: прототип равен null.", "Ошибка").Show();
            return false;
        }

        var unitType = unitPrototype.GetType();

        // попробовать найти существующий UnitStack<T> того же типа
        for (int i = 0; i < units.Length; i++)
        {
            var slot = units[i];
            if (slot == null) continue;

            var slotType = slot.GetType();
            if (slotType.IsGenericType && slotType.GetGenericTypeDefinition() == typeof(UnitStack<>))
            {
                var arg = slotType.GetGenericArguments()[0];
                if (arg == unitType && slot is IUnitStack existingStack)
                {
                    existingStack.Add(1);
                    existingStack.Owner = this;
                    return true;
                }
            }
        }

        // найти первый свободный слот и создать UnitStack<unitType>
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == null)
            {
                try
                {
                    var stackType = typeof(UnitStack<>).MakeGenericType(unitType);
                    var created = (ICombatant?)Activator.CreateInstance(stackType, new object[] { 1 });
                    if (created != null)
                    {
                        units[i] = created;
                        if (created is IUnitStack unitStackInstance) unitStackInstance.Owner = this;
                        return true;
                    }
                }
                catch
                {
                    new ConsoleWindow<int>($"Не удалось создать стек для {unitType.Name}.", "Ошибка").Show();
                    return false;
                }
            }
        }

        new ConsoleWindow<int>("Нет свободных слотов для добавления данного типа юнита.", "Ошибка").Show();
        return false;
    }

    /// <summary>
    /// Добавить один юнит в конкретный слот. Если слот занят — выводим сообщение и возвращаем false.
    /// В этом режиме не происходит суммирования с существующими стеками того же типа.
    /// </summary>
    public bool AddUnit(UnitBase unitPrototype, int slotIndex)
    {
        if (unitPrototype == null)
        {
            new ConsoleWindow<int>("Невозможно добавить: прототип равен null.", "Ошибка").Show();
            return false;
        }

        if (slotIndex < 0 || slotIndex >= units.Length)
        {
            new ConsoleWindow<int>("Неверный индекс слота.", "Ошибка").Show();
            return false;
        }

        if (units[slotIndex] != null)
        {
            new ConsoleWindow<int>($"Слот {slotIndex + 1} уже занят.", "Ошибка").Show();
            return false;
        }

        var unitType = unitPrototype.GetType();
        try
        {
            var stackType = typeof(UnitStack<>).MakeGenericType(unitType);
            var created = (ICombatant?)Activator.CreateInstance(stackType, new object[] { 1 });
            if (created != null)
            {
                units[slotIndex] = created;
                if (created is IUnitStack unitStackInstance) unitStackInstance.Owner = this;
                return true;
            }
        }
        catch
        {
            new ConsoleWindow<int>($"Не удалось создать стек для {unitType.Name}.", "Ошибка").Show();
            return false;
        }

        new ConsoleWindow<int>($"Не удалось создать стек для {unitType.Name}.", "Ошибка").Show();
        return false;
    }

    /// <summary>
    /// Переместить игрока на смещение.
    /// </summary>
    public void Move(Coordinate shift)
    {
        position = position + shift;
    }

    /// <summary>
    /// Вычислить максимальное число ходов в день для данного типа игрока.
    /// </summary>
    private static int GetMaxMoves(PlayerType type)
    {
        // Ranger ходит больше всего, Knight меньше, Mage посередине
        return type switch
        {
            PlayerType.Ranger => 6,
            PlayerType.Mage => 4,
            PlayerType.Knight => 3,
            _ => 4
        };
    }

    /// <summary>
    /// Вычислить макс. количество магии для данного типа игрока.
    /// </summary>
    private static int GetMaxMagic(PlayerType type)
    {
        return type switch
        {
            PlayerType.Mage => 20,
            PlayerType.Knight => 8,
            PlayerType.Ranger => 3,
            _ => 5
        };
    }

    /// <summary>
    /// Вычислить дополнительную защиту от надетой брони (сумма эффектов брони).
    /// </summary>
    public int GetEquippedDefense()
    {
        int def = 0;
        if (HeadArmor != null) def += HeadArmor.GetDefenseBonus(this, null);
        if (BodyArmor != null) def += BodyArmor.GetDefenseBonus(this, null);
        return def;
    }

    /// <summary>
    /// Вычислить бонус атаки для юнитов от экипировки и артефактов.
    /// </summary>
    public int GetWeaponAttackBonus(UnitBase unit = null)
    {
        int bonus = 0;
        if (EquippedWeapon != null) bonus += EquippedWeapon.GetAttackBonus(this, unit);
        if (EquippedArtifact != null && EquippedArtifact is IEquipmentEffect eff)
            bonus += eff.GetAttackBonus(this, unit);
        return bonus;
    }

    /// <summary>
    /// Собрать агрегированную защиту для конкретного участка (учитывает броню и артефакт).
    /// </summary>
    public int GetDefenseForUnit(ICombatant unit)
    {
        int def = 0;
        if (HeadArmor != null) def += HeadArmor.GetDefenseBonus(this, unit as UnitBase);
        if (BodyArmor != null) def += BodyArmor.GetDefenseBonus(this, unit as UnitBase);
        if (EquippedArtifact != null && EquippedArtifact is IEquipmentEffect eff)
            def += eff.GetDefenseBonus(this, unit as UnitBase);
        return def;
    }

    /// <summary>
    /// Найти ресурс игрока по типу.
    /// </summary>
    public Resource? GetResource<T>() where T : Resource
    {
        return Resources.FirstOrDefault(r => r.GetType() == typeof(T));
    }

    /// <summary>
    /// Получить количество ресурса заданного типа.
    /// </summary>
    public int GetResourceAmount<T>() where T : Resource
    {
        var r = GetResource<T>();
        return r != null ? r.Amount : 0;
    }

    /// <summary>
    /// Добавить количество ресурса (создаст запись, если её нет).
    /// </summary>
    public void AddResource<T>(int amount) where T : Resource
    {
        var r = GetResource<T>();
        if (r == null)
        {
            // создать ресурс, если он отсутствует
            r = (Resource?)Activator.CreateInstance(typeof(T), new object[] { 0 });
            if (r != null) Resources.Add(r);
        }

        if (r != null)
            r.Amount += amount;
    }

    /// <summary>
    /// Попытаться потребить указанное количество ресурса; вернуть true если успешно.
    /// </summary>
    public bool TryConsumeResource(Type resourceType, int amount)
    {
        var r = Resources.FirstOrDefault(x => x.GetType() == resourceType);
        if (r == null || r.Amount < amount) return false;
        r.Amount -= amount;
        return true;
    }

    /// <summary>
    /// Проверить, остались ли у игрока живые юниты.
    /// </summary>
    public bool HasAliveUnits()
    {
        foreach (var slot in units)
        {
            if (slot != null)
            {
                if (slot is IUnitStack s)
                {
                    if (s.Count > 0 && s.IsAlive) return true;
                }
                else if (slot.IsAlive) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Получить слот юнита по индексу.
    /// </summary>
    public ICombatant? GetUnitSlot(int index)
    {
        if (index < 0 || index >= units.Length) return null;
        return units[index];
    }

    /// <summary>
    /// Попытаться установить слот юнита (используется для загрузки/переноса).
    /// </summary>
    public bool TrySetUnitSlot(int index, ICombatant unit)
    {
        if (index < 0 || index >= units.Length) return false;
        units[index] = unit;
        return true;
    }

    /// <summary>
    /// Проверяет, содержит ли игрок указанный ICombatant в своих слотах.
    /// </summary>
    public bool ContainsUnit(ICombatant unit)
    {
        if (unit == null) return false;
        foreach (var slot in units)
        {
            if (ReferenceEquals(slot, unit)) return true;
        }
        return false;
    }

    /// <summary>
    /// Get an enumerable of the player's equipped items (head, body, artifact, weapon) as Equipment references.
    /// </summary>
    public IEnumerable<Item> GetEquippedItems()
    {
        // Note: explicit casting to base Equipment
        if (HeadArmor != null) yield return HeadArmor;
        if (BodyArmor != null) yield return BodyArmor;
        if (EquippedArtifact != null) yield return EquippedArtifact;
        if (EquippedWeapon != null) yield return EquippedWeapon;
    }

    /// <summary>
    /// Собрать базовые имена (без учета количества) всех юнит-стеков игрока.
    /// </summary>
    public IEnumerable<string> GetUnitStackBaseNames()
    {
        foreach (var slot in units)
        {
            if (slot != null)
            {
                if (slot is IUnitStack stack) yield return stack.TypeName;
            }
        }
    }
}
