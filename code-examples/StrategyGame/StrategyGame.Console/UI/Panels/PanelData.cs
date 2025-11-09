namespace StrategyGame.ConsoleGame.UI.Panels;

public readonly struct PanelData
{
    public int Day { get; }
    public int Week { get; }
    public int Wood { get; }
    public int Stone { get; }
    public int Gold { get; }
    public int WoodIncome { get; }
    public int StoneIncome { get; }
    public int GoldIncome { get; }

    public PanelData(int day, int week, int wood, int stone, int gold,
        int woodIncome, int stoneIncome, int goldIncome)
    {
        Day = day;
        Week = week;
        Wood = wood;
        Stone = stone;
        Gold = gold;
        WoodIncome = woodIncome;
        StoneIncome = stoneIncome;
        GoldIncome = goldIncome;
    }
}