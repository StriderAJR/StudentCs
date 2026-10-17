using NUnit.Framework;

public class CharacterTests
{
    [Test]
    public void TakeDamage_DoesNotMakeHealthNegative()
    {
        var character = new Character("Mage", 100);
        character.TakeDamage(150);
        Assert.That(character.Health, Is.EqualTo(0));
    }
}
