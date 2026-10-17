namespace TempProject;

internal class Program
{
    static void Attack(ref int mana)
    {
        int cost = 10;
        if (mana < cost)
        {
            throw new InvalidOperationException("Недостаточно маны!");
        }

        mana = mana - cost;
        Console.WriteLine("Огненный шар!");
    }

    public static void Main()
    {
        try
        {
            int mana = 25;
            while(true)
            {
                Attack(ref mana);
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Используем обычную атаку.");
        }
    }
}