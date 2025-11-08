using System.Net;

namespace Table
{
    // перегрузка методов
    // ЗАБЫЛ про ref

    enum PlayerType
    {
        Knight,
        Ranger,
        Mage
    }

    class Player
    {
        public string Name;
        public PlayerType Type;

        private int hp;

        public Player(string name) : this(name, PlayerType.Ranger, 100)
        {
        }

        public Player(string name, PlayerType type, int hp)
        {
            Name = name;
            Type = type;
            this.hp = hp;
        }

        public int Add()
        {
            return 3;
        }
    }

    static class Example
    {
        public static string Name;

        public static int Add()
        {
            return 3;
        }
    }

    class Program
    {
        public static void Main()
        {
            Player p1 = new Player("Vanya", PlayerType.Ranger, 100);

            p1.Name = "Vanya";
            p1.Type = PlayerType.Knight;
            p1.Type = (PlayerType) 1;
        }
    }
}
