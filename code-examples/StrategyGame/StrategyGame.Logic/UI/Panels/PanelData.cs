using System.Collections.ObjectModel;
using StrategyGame.ConsoleGame.Game.Resources;

namespace StrategyGame.ConsoleGame.UI.Panels;

public readonly struct PanelData
{
    public int Day { get; }
    public int Week { get; }

    // New richer data: list of current resources and per-type weekly incomes
    public IReadOnlyList<Resource> Resources { get; }
    public IReadOnlyDictionary<Type, int> IncomeByType { get; }

    public int MovesRemaining { get; }
    public int MaxMoves { get; }

    // Constructor that accepts resource list and income dictionary
    public PanelData(int day, int week, IReadOnlyList<Resource> resources, IReadOnlyDictionary<Type, int> incomeByType,
        int movesRemaining = 0, int maxMoves = 0)
    {
        Day = day;
        Week = week;
        Resources = resources ?? new List<Resource>().AsReadOnly();
        IncomeByType = incomeByType ?? new ReadOnlyDictionary<Type, int>(new Dictionary<Type, int>());
        MovesRemaining = movesRemaining;
        MaxMoves = maxMoves;
    }
}