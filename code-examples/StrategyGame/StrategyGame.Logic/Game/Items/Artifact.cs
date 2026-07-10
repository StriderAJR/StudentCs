namespace StrategyGame.ConsoleGame.Game.Items;

/// <summary>
/// Типы артефактов.
/// </summary>
public enum ArtifactType
{
    /// <summary>
    /// Подвеска-клевер (удача).
    /// </summary>
    Clover,

    /// <summary>
    /// Подзорная труба (видимость/разведка).
    /// </summary>
    Spyglass
}

/// <summary>
/// Артефакт, который может давать пассивные эффекты.
/// </summary>
public class Artifact : Item
{
    /// <summary>
    /// Тип артефакта.
    /// </summary>
    public ArtifactType Type { get; }

    public override string Name => Type.ToString("G");

    public override string Description => $"Артефакт {Name}";

    public Artifact(ArtifactType type) : base()
    {
        Type = type;
    }
}
